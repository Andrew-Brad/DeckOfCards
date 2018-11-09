using DeckOfCards.CQRS;
using DeckOfCards.Domain;
using System;

namespace DeckOfCards.CommandResults
{
    public class CreateWidgetCommandResult : CommandResultBase
    {
        public CardTemplate Widget { get; set; }
    }
}
