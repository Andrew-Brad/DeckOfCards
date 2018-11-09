using DeckOfCards.Domain;
using DeckOfCards.QueryResults;
using MediatR;
using Sieve.Models;

namespace DeckOfCards.Queries
{
    public class CardTemplateQuery : IQuery, IRequest<CardTemplateQueryResult>
    {
        public RanksEnumeration Rank { get; set; }
        public SuitsEnumeration Suit { get; set; }

        //public SieveModel SortFilterPaging { get; set; }
    }
}
