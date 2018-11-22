using DeckOfCards.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DeckOfCards.Commands.CustomValidators
{
    public class SuitsValidationAttribute : ValidationAttribute//, IClientModelValidator
    {
        public static string[] SuitStrings { get; } = SuitsEnumeration.List.Select(x => x.Name).ToArray();
        public static ushort[] SuitIds { get; } = SuitsEnumeration.List.Select(x => x.Value).ToArray();

        public SuitsValidationAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("Value must be provided.");
            string suitInput = value.ToString();

            for (int i = 0; i < SuitStrings.Length; i++)
            {
                if (string.Equals(SuitStrings[i], suitInput, StringComparison.InvariantCultureIgnoreCase))
                    return ValidationResult.Success;
            }            
            return new ValidationResult("Classification could not be recognized.");
        }
    }
}
