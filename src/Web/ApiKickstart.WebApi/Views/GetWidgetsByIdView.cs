using ApiKickstart.CQRS;
using ApiKickstart.Domain;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ApiKickstart.WebApi.Views
{
    /// <summary>
    /// The View represents the clients view of the data.  Response model contract.
    /// </summary>
    public class GetWidgetsByIdView
    {
        public CardTemplate Widget { get; set; }
    }
}
