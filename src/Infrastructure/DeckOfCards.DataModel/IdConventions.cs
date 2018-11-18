using DeckOfCards.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeckOfCards.Persistence
{
    public static class IdConventions
    {
        public static Func<string,CardTemplate,Task<string>> CardTemplateIdStrategy => (dbname, card) =>
                    Task.FromResult(string.Format("cards/{0}/{1}", card.Rank.Name, card.Suit));
    }
}
