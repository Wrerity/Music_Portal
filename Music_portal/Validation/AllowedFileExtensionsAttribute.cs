using System.ComponentModel.DataAnnotations;

namespace Music_portal.Validation;

public class AllowedFileExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedFileExtensionsAttribute(params string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_extensions.Contains(ext))
                return new ValidationResult(ErrorMessage ?? $"Разрешены только файлы: {string.Join(", ", _extensions)}");
        }

        return ValidationResult.Success;
    }
}