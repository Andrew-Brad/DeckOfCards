using System.ComponentModel.DataAnnotations;
using DeckOfCards.Commands.CustomValidators;

namespace DeckOfCards.WebApi.ViewModels
{
    /// <summary>
    /// Represents a logical business command to create a new widget.
    /// </summary>
    public class CreateWidgetViewModel
    {
        [Required]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "A name must be at least 3 characters and less than 128.")]
        public string WidgetName { get; set; }
        [StringLength(128, MinimumLength = 3, ErrorMessage = "Description must be at least 3 characters and less than 128.")]
        public string Description { get; set; }
        public int? SupplierId { get; set; }
        public bool Deprecated { get; set; } = false; // future use case for a breaking api/migration change - move to some domain enumeration
        [Required]
        [SuitsValidation]
        public string Classification { get; set; }// will map "1" or "RawMaterial" to internal enum
    }
}
