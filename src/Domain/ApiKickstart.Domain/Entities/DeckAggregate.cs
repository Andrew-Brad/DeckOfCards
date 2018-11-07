using System;
using System.Collections.Generic;
using AB.Extensions;

namespace ApiKickstart.Domain
{
    // https://lostechies.com/gabrielschenker/2015/05/25/ddd-the-aggregate/
    // Invariants are consistency rules that must be maintained whenever data changes, so they represent business rules.
    // https://dotnetcodr.com/2015/06/29/various-topics-from-software-architecture-part-5-aggregate-roots/#more-6083
    public class DeckAggregate
    {
        private List<PlayerCard> _cards; // list?  transactional boundaries around card operations
        // should the operations themselves be objects? Could help for an ES implementation...

        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Generate a new deck of cards from card templates.
        /// </summary>
        /// <param name="cards"></param>
        public DeckAggregate(IList<CardTemplate> cards) // eventually, some deck creation options object
        {
            _cards = new List<PlayerCard>();
            foreach (var card in cards)
            {
                _cards.Add(new PlayerCard()
                {
                    Template = card,
                    Id = Guid.NewGuid(),
                });
            }
        }

        /// <summary>
        /// Shuffles the deck.
        /// </summary>
        public void Shuffle()
        {
            this._cards.Shuffle();
        }
    }
}
