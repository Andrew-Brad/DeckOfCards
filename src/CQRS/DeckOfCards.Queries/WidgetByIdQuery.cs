using DeckOfCards.QueryResults;
using MediatR;

namespace DeckOfCards.Queries
{
    public class WidgetByIdQuery : IQuery, IRequest<WidgetByIdQueryResult>
    {
        public string WidgetId{ get; set; }
    }
}
