using System.ComponentModel.DataAnnotations;

namespace API.Validation;

public class CVVCodeValidator : ValidationAttribute
{

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult("Invalid CVV code");
        var strValue = (string)value;
        if (strValue.Length != 3)
            return new ValidationResult("Invalid CVV code");
        if (strValue.Any(ch => !char.IsNumber(ch)))
            return new ValidationResult("Invalid CVV code");
        return ValidationResult.Success;
    }
}