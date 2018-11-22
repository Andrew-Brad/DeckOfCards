using DeckOfCards.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DeckOfCards.Commands.CustomValidators
{
    public class RanksValidationAttribute : ValidationAttribute//, IClientModelValidator
    {
        public static string[] RankStrings { get; } = RanksEnumeration.List.Select(x => x.Name).ToArray();
        public static ushort[] RankIds { get; } = RanksEnumeration.List.Select(x => x.Value).ToArray();

        public RanksValidationAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("Value must be provided.");
            string input = value.ToString();

            for (int i = 0; i < RankStrings.Length; i++)
            {
                if (string.Equals(RankStrings[i], input, StringComparison.InvariantCultureIgnoreCase))
                    return ValidationResult.Success;
            }            
            return new ValidationResult("Classification could not be recognized.");
        }
    }
}
