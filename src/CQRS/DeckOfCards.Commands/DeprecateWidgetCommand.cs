using DeckOfCards.CommandResults;
using DeckOfCards.CQRS;
using MediatR;

namespace DeckOfCards.Commands
{
    public class DeprecateWidgetCommand : ICommand, IRequest<DeprecateWidgetCommandResult>
    {
        public int WidgetId { get; set; }

        // as requirements change, the internal command object can evolve independently of the Api contracts
    }
}
