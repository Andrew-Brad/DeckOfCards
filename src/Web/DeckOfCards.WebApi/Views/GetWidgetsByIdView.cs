using DeckOfCards.CQRS;
using DeckOfCards.Domain;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DeckOfCards.WebApi.Views
{
    /// <summary>
    /// The View represents the clients view of the data.  Response model contract.
    /// </summary>
    public class GetWidgetsByIdView
    {
        public CardTemplate Widget { get; set; }
    }
}
