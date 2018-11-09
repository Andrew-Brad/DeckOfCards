using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using DeckOfCards.CommandResults;
using DeckOfCards.CQRS;
using MediatR;

namespace DeckOfCards.WebApi.ViewModels
{
    public class DeprecateWidgetViewModel
    {
        [FromRoute]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "An Id must be at least 1 character and less than 128.")]
        public string WidgetId { get; set; }
    }
}
