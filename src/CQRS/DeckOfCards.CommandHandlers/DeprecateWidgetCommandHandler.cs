using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DeckOfCards.Commands;
using DeckOfCards.CommandResults;
using System.Threading;
using DeckOfCards.DomainEvents;
using AutoMapper;

namespace DeckOfCards.CommandHandlers
{
    public class DeprecateWidgetCommandHandler : IRequestHandler<DeprecateWidgetCommand, DeprecateWidgetCommandResult>
    {
        private readonly ILogger<DeprecateWidgetCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public DeprecateWidgetCommandHandler(ILogger<DeprecateWidgetCommandHandler> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<DeprecateWidgetCommandResult> Handle(DeprecateWidgetCommand command, CancellationToken cancellationToken)
        {
            var commandResult = new DeprecateWidgetCommandResult();
            try
            {
                //maybe we prep a bit of initial data up here, but the subsequent mediator publish will kick off a chain of events
                commandResult.WidgetId = command.WidgetId;
                //todo: deprecation flag in DB/ EF migration

                //
                commandResult.Success = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _logger.LogTrace("Deprecation notification publishing via Mediatr...");
                var notificationTask = _mediator.Publish(_mapper.Map<WidgetDeprecatedV1Event>(commandResult));
                _logger.LogTrace("Deprecation notification published. Id:{taskId}", notificationTask.Id);
                notificationTask.ContinueWith((x) => _logger.LogTrace("Message publish completed. TaskId: {taskId}",x.Id));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                commandResult.Success = true;
                commandResult.ResultStatus = CQRS.CommandResultStatus.SuccessfullyProcessed;
                _logger.LogTrace("{result} has completed. Success: {success}.", nameof(DeprecateWidgetCommandResult), commandResult.Success);
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, "Oops! Ran into an error!");
                commandResult.ResultStatus = CQRS.CommandResultStatus.CriticalError;
            }
            finally
            {
                //log

            }
            return await Task.FromResult(commandResult);
        }
    }
}
