using System.ComponentModel.DataAnnotations;

namespace EmployeeBook.Models
{
    public class ValidationsExtensions
    {
        public class ValidExtensionsAttribute : ValidationAttribute
        {
            private readonly string[] _extensions;
            public ValidExtensionsAttribute(string[] extensions)
            {
                _extensions = extensions;
            }
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is IFormFile file)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (!_extensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
                return ValidationResult.Success;
            }
            public string GetErrorMessage()
            {
                return $"this photo extension is not allowed, try jpg or png format";
            }
        }
    }
}
