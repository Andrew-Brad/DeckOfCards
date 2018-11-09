using System;

namespace ApiKickstart.Domain
{
    public class CardTemplate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        //public DateTimeOffset CreationDate { get; set; }
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
                // card name controlled at runtime
            };
        }

        protected CardTemplate()
        {
            // unused - use static constructor
        }

        public bool IsFaceCard() => false;
    }
}
