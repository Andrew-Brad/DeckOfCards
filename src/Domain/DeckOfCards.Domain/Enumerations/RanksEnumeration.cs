using Ardalis.SmartEnum;

namespace DeckOfCards.Domain
{
    /// <summary>
    /// These enumerations should be defined at design time, use terminology as defined and understood by the business, and not
    /// really care about your foreign key constraints.
    /// They are immutable, and facilitate writing code against business rules that involve them.
    /// </summary>
    public class RanksEnumeration : SmartEnum<RanksEnumeration, ushort>
    {
        public static RanksEnumeration Ace = new RanksEnumeration(nameof(Ace), 1);
        public static RanksEnumeration Two = new RanksEnumeration(nameof(Two), 2);
        public static RanksEnumeration Three = new RanksEnumeration(nameof(Three), 3);
        public static RanksEnumeration Four = new RanksEnumeration(nameof(Four), 4);
        public static RanksEnumeration Five = new RanksEnumeration(nameof(Five), 5);
        public static RanksEnumeration Six = new RanksEnumeration(nameof(Six), 6);
        public static RanksEnumeration Seven = new RanksEnumeration(nameof(Seven), 7);
        public static RanksEnumeration Eight = new RanksEnumeration(nameof(Eight), 8);
        public static RanksEnumeration Nine = new RanksEnumeration(nameof(Nine), 9);
        public static RanksEnumeration Ten = new RanksEnumeration(nameof(Ten), 10);
        public static RanksEnumeration Jack = new RanksEnumeration(nameof(Jack), 11);
        public static RanksEnumeration Queen = new RanksEnumeration(nameof(Queen), 12);
        public static RanksEnumeration King = new RanksEnumeration(nameof(King), 13);

        protected RanksEnumeration(string name, ushort value) : base(name, value)
        {
        }
    }
}
