using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.ValidationsAndAttributes;

public class AllowedStringsAttribute(string[] allowedValues) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (allowedValues.Contains(value))
            return ValidationResult.Success;
        return new ValidationResult($"Allowed Categories are: {string.Join(",",allowedValues)}");
    }
}