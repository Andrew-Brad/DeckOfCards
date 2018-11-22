using DeckOfCards.CQRS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeckOfCards.Commands
{
    /// <summary>
    /// To create a new Deck of Cards, use this command with an optional overload of a profile to customize the resulting cards in the deck.
    /// </summary>
    public class NewDeckOfCardsCommand : ICommand, IRequest<NewDeckOfCardsCommandResult>
    {
        // nothing for now
    }
}
