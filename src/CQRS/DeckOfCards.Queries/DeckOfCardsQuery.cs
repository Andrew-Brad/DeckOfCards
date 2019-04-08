using DeckOfCards.QueryResults;
using MediatR;

namespace DeckOfCards.Queries
{
    public class DeckOfCardsQuery : IQuery, IRequest<DeckOfCardsQueryResult>
    {
        public string DeckId{ get; set; }
    }
}
