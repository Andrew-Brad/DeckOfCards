using DeckOfCards.Commands.CustomValidators;
using DeckOfCards.Domain;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DeckOfCards.WebApi.ViewModels
{
    /// <summary>
    /// Input model for HTTP request contracts.  Sorting, filtering, and paging resides in a separate library 
    /// since it is widley shareable and extensible.  Controllers should map sort/filter/page objects.  Nested objects inside views
    /// don't play nicely with MVC binding and validation.
    /// </summary>
    public class GetCardTemplateViewModel
    {
        [MinLength(1)]
        [RanksValidation]
        public string Rank { get; set; }

        [MinLength(1)]
        [SuitsValidation]
        public string Suit { get; set; }

    }
}
