using System.ComponentModel.DataAnnotations;

namespace API.Validation;

public class CurrencyValidator : ValidationAttribute
{
    readonly string[] validValues;

    public CurrencyValidator(params string[] validValues)
    {
        this.validValues = validValues;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (validValues.Contains((string)value))
            return ValidationResult.Success;
        return new ValidationResult("Invalid Currency. Valid values are: GPB, USD, EUR");
    }
}