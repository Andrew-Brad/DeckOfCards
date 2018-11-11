using System;
using System.Collections.Generic;
using System.Linq;
using AB.Extensions;

namespace DeckOfCards.Domain
{
    // https://lostechies.com/gabrielschenker/2015/05/25/ddd-the-aggregate/
    // Invariants are consistency rules that must be maintained whenever data changes, so they represent business rules.
    // https://dotnetcodr.com/2015/06/29/various-topics-from-software-architecture-part-5-aggregate-roots/#more-6083
    public class Deck
    {
        private List<PlayingCard> _cards; // list?  transactional boundaries around card operations
        // should the operations themselves be objects? Could help for an ES implementation...

        // switch back to Guid? https://ravendb.net/docs/article-page/4.1/csharp/client-api/configuration/identifier-generation/type-specific
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Generate a new deck of cards from card templates.
        /// </summary>
        /// <param name="cards"></param>
        private Deck(IList<CardTemplate> cards)
        {
            _cards = new List<PlayingCard>();
            foreach (var card in cards)
            {
                _cards.Add(new PlayingCard()
                {
                    Template = card,
                    Id = Guid.NewGuid(),
                });
            }
        }

        /// <summary>
        /// Create a standard 52 card deck. Assumes the world where only 4 suits exist.
        /// This is a naive form of a creation profile - we use for loops instead of deck creation criteria.
        /// This may get refactored into a deck factory or strategy of some sort.
        /// </summary>
        public static Deck Standard52CardDeck(IList<CardTemplate> cardPool)
        {
            List<CardTemplate> standardCards = new List<CardTemplate>();
            for (int s = 0; s < SuitsEnumeration.List.Count; s++)
            {
                for (int r = 0; r < RanksEnumeration.List.Count; r++)
                {                    
                    CardTemplate logicalCard = cardPool
                        .Single(x => x.Rank == RanksEnumeration.List[r] && x.Suit == SuitsEnumeration.List[s]);
                    standardCards.Add(logicalCard);
                }
            }

            var deck = new Deck(standardCards);
            return deck;
        }

        public static Deck FromProfile()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shuffles the deck.
        /// </summary>
        public void Shuffle()
        {
            this._cards.Shuffle();
        }

        public IReadOnlyList<PlayingCard> ShowCards()
        {
            return _cards.AsReadOnly();
        }
    }
}
