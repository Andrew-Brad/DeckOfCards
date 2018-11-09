using Ardalis.SmartEnum;

namespace ApiKickstart.Domain
{
    /// <summary>
    /// These enumerations should be defined at design time, use terminology as defined and understood by the business, and not
    /// really care about your foreign key constraints.
    /// They are immutable, and facilitate writing code against business rules that involve them.
    /// </summary>
    public class SuitsEnumeration : SmartEnum<SuitsEnumeration, ushort>
    {
        public static SuitsEnumeration Spades = new SuitsEnumeration(nameof(Spades), 1);
        public static SuitsEnumeration Clubs = new SuitsEnumeration(nameof(Clubs), 2);
        public static SuitsEnumeration Hearts = new SuitsEnumeration(nameof(Hearts), 3);
        public static SuitsEnumeration Diamonds = new SuitsEnumeration(nameof(Diamonds), 4);


        protected SuitsEnumeration(string name, ushort value) : base(name, value)
        {
        }
    }
}
