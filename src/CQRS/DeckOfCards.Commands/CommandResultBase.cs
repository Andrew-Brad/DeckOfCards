using DeckOfCards.CQRS;

namespace DeckOfCards.CommandResults
{
    public class CommandResultBase : ICommandResult
    {
        public CommandResultStatus ResultStatus { get; set; }
    }
}
