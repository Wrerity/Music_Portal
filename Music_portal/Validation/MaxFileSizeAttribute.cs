using System.ComponentModel.DataAnnotations;

namespace Music_portal.Validation;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxBytes;

    public MaxFileSizeAttribute(int maxBytes)
    {
        _maxBytes = maxBytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is IFormFile file && file.Length > _maxBytes)
        {
            var mb = _maxBytes / (1024 * 1024);
            return new ValidationResult(ErrorMessage ?? $"Максимальный размер файла — {mb} МБ");
        }

        return ValidationResult.Success;
    }
}