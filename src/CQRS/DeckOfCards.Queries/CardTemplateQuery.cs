using ApiKickstart.Domain;
using ApiKickstart.QueryResults;
using MediatR;
using Sieve.Models;

namespace ApiKickstart.Queries
{
    public class CardTemplateQuery : IQuery, IRequest<CardTemplateQueryResult>
    {
        public RanksEnumeration Rank { get; set; }
        public SuitsEnumeration Suit { get; set; }

        //public SieveModel SortFilterPaging { get; set; }
    }
}
