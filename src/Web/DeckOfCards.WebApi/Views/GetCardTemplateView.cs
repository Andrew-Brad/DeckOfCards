using AB.Extensions;
using DeckOfCards.Domain;
using System.Collections.Generic;

namespace DeckOfCards.WebApi.Views
{
    /// <summary>
    /// The View represents the clients view of the data.  Response model contract.
    /// </summary>
    public class GetCardTemplateView //: IPagedResultView
    {
        public string CardName { get; set; }
        public string Rank { get; set; }
        public string Suit { get; set; }

        //public Card Widgets { get; set; }
        //public PagingResponse Paging { get; set; }
    }
}
