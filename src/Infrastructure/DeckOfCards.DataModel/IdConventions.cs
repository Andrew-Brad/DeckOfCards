using DeckOfCards.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeckOfCards.Persistence
{
    // All Id's in RavenDB are strings
    public static class IdConventions
    {
        public const string CardTemplateIdFormatString = "cards/{0}/{1}";
        public static Func<string,CardTemplate,Task<string>> CardTemplateIdStrategy => (dbname, card) =>
                    Task.FromResult(string.Format(CardTemplateIdFormatString, card.Rank.Name, card.Suit.Name));
    }
}
