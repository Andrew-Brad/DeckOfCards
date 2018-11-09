using AB.Extensions;

namespace ApiKickstart.WebApi.Views
{
    public interface IPagedResultView
    {
        PagingResponse Paging { get; set; }
    }
}
