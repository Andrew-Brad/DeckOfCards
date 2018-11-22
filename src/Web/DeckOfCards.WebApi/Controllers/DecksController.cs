using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
    public class DecksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DecksController> _logger;
        private readonly IMapper _mapper;

        public DecksController(IMediator mediator, ILogger<DecksController> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        // POST api/v1/decks
        [HttpPost]
        [SwaggerResponse((int)System.Net.HttpStatusCode.Created, typeof(ApiResponse<NewDeckView>), Description = "Create a deck of cards.  If no POST body is given, a default creation profile is used.")]
        public async Task<IActionResult> CreateDeckOfCards()
        {
            var command = new NewDeckOfCardsCommand();
            NewDeckOfCardsCommandResult commandResult = await _mediator.Send(command);
            return this.ReturnCreatedCommandResult<NewDeckView, NewDeckOfCardsCommandResult>(commandResult, _mapper,null,null);
        }

        // GET api/v1/decks/123
        [HttpGet("{DeckId}")]
        [SwaggerResponse((int)System.Net.HttpStatusCode.OK, typeof(ApiResponse<DeckByIdView>), Description = "Look up a deck of cards by its unique Id.")]
        public async Task<IActionResult> GetDeckOfCardsById(GetDeckByIdViewModel model)
        {
            var request = new DeckOfCardsQuery() { DeckId = model.DeckId };
            NewDeckOfCardsQueryResult result = await _mediator.Send(request);
            return this.ReturnObjectResult<DeckByIdView, IQueryResult>(result, _mapper);
        }
    }
}
