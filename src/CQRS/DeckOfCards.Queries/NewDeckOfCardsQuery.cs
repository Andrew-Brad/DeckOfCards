using DeckOfCards.QueryResults;
using MediatR;

namespace DeckOfCards.Queries
{
    public class DeckOfCardsQuery : IQuery, IRequest<NewDeckOfCardsQueryResult>
    {
        public string DeckId{ get; set; }
    }
}
