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
        public string Id { get; private set; }
        public RanksEnumeration Rank { get; private set; }
        public SuitsEnumeration Suit { get; private set; }
        public string CardName { get; set; }
        public string ImageUrl { get; set; }

        public static CardTemplate NewTemplate(RanksEnumeration rank, SuitsEnumeration suit)//, string cardName, Uri imageUrl)
        {
            return new CardTemplate()
            {
                //CreationDate = DateTime.UtcNow,
                Id = rank.Name + " of " + suit.Name,
                Rank = rank ?? RanksEnumeration.Ace,
                Suit = suit ?? SuitsEnumeration.Spades,
                // card name/metadata controlled at runtime
            };
        }

        protected CardTemplate()
        {
            // unused - use static constructor
        }

        public bool IsFaceCard()
        {
            return (this.Rank == RanksEnumeration.Jack
                || this.Rank == RanksEnumeration.Queen
                || this.Rank == RanksEnumeration.King);
        }

        public bool Equals(CardTemplate other)
        {
            return (this.Rank == other.Rank && this.Suit == other.Suit);
        }
    }
}
