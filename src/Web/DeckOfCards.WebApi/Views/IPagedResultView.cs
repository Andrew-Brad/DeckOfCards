using AB.Extensions;

namespace DeckOfCards.WebApi.Views
{
    public interface IPagedResultView
    {
        PagingResponse Paging { get; set; }
    }
}
