﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DeckOfCards.WebApi.ViewModels
{
    public class GetDeckByIdViewModel
    {
        [FromRoute]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "An Id must be at least 1 character and less than 128.")]
        public string DeckId { get; set; }

    }
}
