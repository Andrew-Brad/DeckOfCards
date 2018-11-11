using System;

namespace DeckOfCards.Domain
{
    /// <summary>
    /// A <see cref="CardTemplate"/> represents the single logical entity that is "the Ace of Spades".  Not "your" Ace, but "the" Ace.
    /// These singular templates are the starting point for constructing decks of cards, as they can have global metadata attached
    /// to them, such as descriptions and suit/rank attributes that are immutable.
    /// </summary>
    public class CardTemplate : IEquatable<CardTemplate>
    {
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CardName { get; set; }
        //public string Description { get; set; }
        public RanksEnumeration Rank { get; set; }
        public SuitsEnumeration Suit { get; set; }

        public static CardTemplate NewCard(RanksEnumeration rank, SuitsEnumeration suit)
        {
            return new CardTemplate()
            {
                //CreationDate = DateTime.UtcNow,
                //Id = Guid.NewGuid().ToString(),
                Rank = rank ?? RanksEnumeration.Ace,
                Suit = suit ?? SuitsEnumeration.Spades,
                // card name/metadata controlled at runtime
            };
        }

        protected CardTemplate()
        {
            // unused - use static constructor
        }

        public bool IsFaceCard() => false;

        public bool Equals(CardTemplate other)
        {
            return (this.Rank == other.Rank && this.Suit == other.Suit);
        }
    }
}
