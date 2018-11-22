using DeckOfCards.CommandResults;
using DeckOfCards.CQRS;
using DeckOfCards.Domain;

namespace DeckOfCards.Commands
{
    public class NewDeckOfCardsCommandResult : CommandResultBase, ICommandResult
    {
        public Deck Deck { get; set; }
    }
}
