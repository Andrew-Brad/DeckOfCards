using ApiKickstart.QueryResults;
using MediatR;

namespace ApiKickstart.Queries
{
    public class WidgetByIdQuery : IQuery, IRequest<WidgetByIdQueryResult>
    {
        public string WidgetId{ get; set; }
    }
}
