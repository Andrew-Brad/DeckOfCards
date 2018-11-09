using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ApiKickstart.WebApi.ViewModels
{
    /// <summary>
    /// Input model for HTTP request contracts.
    /// </summary>
    public class GetWidgetsViewModelV2 // no difference currently
    {
        [Range(1, ulong.MaxValue, ErrorMessage = "Page number must be a valid positive integer.")]
        [FromQuery]
        public ulong Page { get; set; } = 1;
        [Range(1, 1000, ErrorMessage = "Only 1000 widgets can be requested at once.")]
        [FromQuery]
        public ulong RecordsPerPage { get; set; } = 50;
        [FromQuery]
        public bool ExcludeDeprecated { get; set; } = true;
    }
}
