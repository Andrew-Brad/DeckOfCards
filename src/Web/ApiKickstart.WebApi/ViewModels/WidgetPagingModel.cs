using Sieve.Models;

namespace ApiKickstart.WebApi.ViewModels
{
    public class WidgetPagingViewModel : SieveModel
    {
        public WidgetPagingViewModel()
        {
            this.Page = 1;
            this.PageSize = 100;
        }
    }

}
