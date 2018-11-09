//using ApiKickstart.Domain;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using ApiKickstart.Commands;
//using ApiKickstart.CommandResults;
//using System.Threading;

//namespace ApiKickstart.QueryHandlers
//{
//    public class ReplaceWidgetCommandHandler : IRequestHandler<ReplaceWidgetCommand, ReplaceWidgetCommandResult>
//    {
//        private readonly ILogger<ReplaceWidgetCommandHandler> _logger;

//        public ReplaceWidgetCommandHandler(ILogger<ReplaceWidgetCommandHandler> logger)
//        {
//            _logger = logger;
//        }

//        public async Task<ReplaceWidgetCommandResult> Handle(ReplaceWidgetCommand command, CancellationToken cancellationToken)
//        {
//            var commandResult = new ReplaceWidgetCommandResult();
//            try
//            {
//                await Task.CompletedTask;
//                //can't update a widget in memory until we have a real repo layer
//                //var allwidgets = await WidgetsQueryHandler.GetWidgetsAsync();
//                commandResult.Widget = command.Widget;
//                commandResult.ResultStatus = CQRS.CommandResultStatus.SuccessfullyProcessed;
//                _logger.LogTrace("{result} has completed. {widget} returned.", nameof(CreateWidgetCommandResult), commandResult.Widget);
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(new EventId(), e, "Oops! Ran into an error!");//todo: event Id's/logging events and exception dump data
//                //queryResult.
//                throw;//todo: remove after above hypermedia is complete
//            }
//            finally
//            {
//                //log

//            }
//            return commandResult;
//        }

//        private async Task<IList<Widget>> GetWidgetsAsync() => await Task.FromResult(_widgetsDataSource);

//        //todo: move to repository impl/layer
//        public static readonly IList<Widget> _widgetsDataSource = new List<Widget>()
//                    {
//                        new Widget() { Id = 1, WidgetName = "Thingabob", Deprecated = false },
//                        new Widget() { Id = 2, WidgetName = "TinyBit", Deprecated = false },
//                        new Widget() { Id = 3, WidgetName = "Thingamajig", Deprecated = false  },
//                        new Widget() { Id = 3, WidgetName = "Thingamajig", Deprecated = false  },
//                        new Widget() { Id = 4, WidgetName = "Oojamaflip", Deprecated = false  },
//                        new Widget() { Id = 5, WidgetName = "Doohickey", Deprecated = false  },
//                        new Widget() { Id = 6, WidgetName = "Whatchamacallit", Deprecated = true  },
//                        new Widget() { Id = 7, WidgetName = "Yokeamabob", Deprecated = true  },
//                    };
//    }
//}
