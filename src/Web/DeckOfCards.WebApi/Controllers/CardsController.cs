using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using DeckOfCards.Domain;
using DeckOfCards.Queries;
using DeckOfCards.Commands;
using DeckOfCards.QueryResults;
using DeckOfCards.WebApi.Filters;
using DeckOfCards.WebApi.Response;
using DeckOfCards.WebApi.ViewModels;
using DeckOfCards.WebApi.Views;
using DeckOfCards.CommandResults;
using DeckOfCards.CQRS;
using AutoMapper;
using MediatR;
using NSwag.Annotations;

namespace DeckOfCards.WebApi.Controllers
{
    /// <summary>
    /// "I want a controller to only be responsible for receiving an HTTP request, dispatching an HTTP response, and nothing in between." -Scott Allen
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [PreValidatedModel]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class CardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CardsController> _logger;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public CardsController(IMediator mediator, ILogger<CardsController> logger, IConfiguration config, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
            _mapper = mapper;
        }

        // GET api/v1/cards/templates?rank=ace&suit=spades
        [HttpGet("templates")]
        [SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<GetCardTemplateView>), Description = "Lookup a given card template. Templates represent the single logical card, not a specific instance which was created for use in a player's deck of cards.")]
        public async Task<IActionResult> GetCardByRankAndSuit([FromQuery] GetCardTemplateViewModel model)//, WidgetPagingViewModel sortFilerPaging)// Paging model exists here due to model binder not respecting nested objects (internally, there is a Dictionary design)
        {
            var query = _mapper.Map<CardTemplateQuery>(model);//.Map(sortFilerPaging);
            var queryResult = await _mediator.Send(query);
            //return this.ReturnObjectResult<GetCardView, IPagedQueryResult>(queryResult,_mapper);
            return this.ReturnObjectResult<GetCardTemplateView, IQueryResult>(queryResult, _mapper);
        }

        // GET api/v1/cards/templates/spades/ace
        [HttpGet("templates/{suit}/{rank}")]
        [SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<GetCardTemplateView>), Description = "Lookup a given card template. Templates represent the single logical card, not a specific instance which was created for use in a player's deck of cards.")]
        public async Task<IActionResult> GetCardByRankAndSuitRoute([FromRoute] GetCardTemplateViewModel model)
        {
            var query = _mapper.Map<CardTemplateQuery>(model);
            var queryResult = await _mediator.Send(query);
            return this.ReturnObjectResult<GetCardTemplateView, IQueryResult>(queryResult, _mapper);
        }

        // GET api/v1/cards?rank=ace&suit=spades
        //[HttpGet]
        //[SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<GetWidgetsView>), Description = "View a given card. return all widgets.  In the future, Ids can be passed in and only those widgets will be returned.")]
        //public async Task<IActionResult> GetCardById(GetWidgetsViewModel model, WidgetPagingViewModel sortFilerPaging)// Paging model exists here due to model binder not respecting nested objects (internally, there is a Dictionary design)
        //{
        //    var query = _mapper.Map<WidgetsQuery>(model).Map(sortFilerPaging);
        //    var queryResult = await _mediator.Send(query);
        //    return this.ReturnObjectResult<GetWidgetsView, IPagedQueryResult>(queryResult, _mapper);
        //}

        // GET api/v2/widgets
        //[HttpGet, MapToApiVersion("2")]
        //[SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<GetWidgetsView>), Description = "Will return all widgets.  In the future, Ids can be passed in and only those widgets will be returned.")]
        //public async Task<IActionResult> GetWidgetsV2(GetWidgetsViewModelV2 model)
        //{
        //    var queryResult = await _mediator.Send(_mapper.Map<WidgetsQuery>(model));
        //    return this.ReturnObjectResult<GetWidgetsViewModelV2, WidgetsQueryResult>(queryResult, _mapper);
        //}

        //// GET api/widgets/123
        //[HttpGet("{widgetId}")]
        //[SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<GetWidgetsByIdView>),Description = "Queries for a single widget by its Identifier.")]
        //public async Task<IActionResult> GetWidgetById(GetWidgetByIdViewModel model)
        //{
        //    var queryResult = await _mediator.Send(_mapper.Map<WidgetByIdQuery>(model));
        //    return this.ReturnObjectResult<GetWidgetsByIdView, WidgetByIdQueryResult>(queryResult, _mapper);
        //}

        //// GET api/widgets/classifications
        //[HttpGet("classifications")]
        //[SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<GetWidgetsClassificationView>), Description = "Queries for a single widget by its Identifier.")]
        //public async Task<IActionResult> GetWidgetClassificationEnumeration()
        //{
        //    var queryResult = EnumerationQueryResult<SuitsEnumeration,ushort>.Generate();
        //    return await Task.FromResult(new OkObjectResult(ApiResponse.ApiResponseFactory(
        //        new GetWidgetsClassificationView() { Classifications = queryResult.Enumeration.ToDictionary(x => x.Value, y => y.Name) })));
        //}

        //// POST api/widgets
        //[HttpPost]
        //[SwaggerResponse((int)System.Net.HttpStatusCode.Created, typeof(ApiResponse<CreateWidgetView>), Description = "Creates a new widget.")]
        //public async Task<IActionResult> CreateWidget([FromBody] CreateWidgetViewModel model)
        //{
        //    var commandResult = await _mediator.Send(_mapper.Map<CreateWidgetCommand>(model));
        //    return this.ReturnCreatedCommandResult<CreateWidgetView,CreateWidgetCommandResult>(commandResult, _mapper,nameof(GetWidgetById), new WidgetByIdQuery() { WidgetId = commandResult.Widget.Id });
        //}

        //// POST api/widgets/123/deprecation
        //[HttpPost("{widgetId}/deprecations")]
        //[SwaggerResponse((int)System.Net.HttpStatusCode.Accepted, typeof(Card), Description = "A POST to the deprecation endpoint will kickoff the lengthy process to deprecate a Widget.  Since this process occurs externally of this system, expect a 202 Accepted, since external factors can impact the deprecation flow.")]
        //public async Task<IActionResult> DeprecateWidget(DeprecateWidgetViewModel model)
        //{
        //    var result = await _mediator.Send(_mapper.Map<DeprecateWidgetCommand>(model));
        //    _logger.LogInformation("POST deprecation for {command} processed, executing AcceptAtAction() with route value {routeVal}", nameof(DeprecateWidgetCommand),model.WidgetId);
        //    //return await Task.FromResult(Accepted(new Uri("/api/Widgets/", UriKind.Relative), new { WidgetId = command.WidgetId }));
        //    //for reference (MSFT docs lacking in this area) - http://hamidmosalla.com/2017/03/29/asp-net-core-action-results-explained/
        //    if (result.ResultStatus == CQRS.CommandResultStatus.SuccessfullyProcessed)
        //    return AcceptedAtAction(nameof(GetWidgetById), this.ControllerContext.ActionDescriptor.ControllerName, new WidgetByIdQuery() { WidgetId = model.WidgetId }, _mapper.Map<DeprecateWidgetView>(result));
        //    else return new ObjectResult(null) { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        //}

        //// TODO: PATCH
        //// PUT api/widgets/123
        ////[HttpPut("{" + nameof(WidgetByIdQuery.WidgetId) + "}")]
        ////[SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(Widget), Description = "Update a widget in its entirety.")]
        ////public async Task<IActionResult> ReplaceWidget([FromRoute]int widgetId, [FromBody]ReplaceWidgetCommand command)
        ////{
        ////    command.Widget.Id = widgetId;//a bit opinionated, but the semantics of PUT and immutable identifiers heavily depends on the domain
        ////    var commandResult = await _mediator.Send(command);
        ////    return this.ReturnStatusCodeForPutCommandResult(ApiResponse.ApiResponseFactory(commandResult));
        ////}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        // GET api/widgets/error
        [HttpGet("error")]
        [SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(CardTemplate))]
        public async Task<IActionResult> GetWidgetError()
        {
            //this endpoint is designed to force a critical unhandled exception (which should never pass code reviews, since all handlers need to have an appropriate try/catch
            //helps to double confirm a global ExceptionFilter
            try
            {
                int a = 1;
                int b = 0;
                int error = a / b;
                return await Task.FromResult(Ok());
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, "Oops! Ran into an error! Does it look OK in the console log?");
                return new EmptyResult();
            }
        }
    }
}
