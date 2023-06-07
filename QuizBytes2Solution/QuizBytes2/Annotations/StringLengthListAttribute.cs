using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Annotations;

public class StringLengthListAttribute : ValidationAttribute
{
    private readonly int _minLength;
    private readonly int _maxLength;

    public StringLengthListAttribute(int minLength, int maxLength)
    {
        _minLength = minLength;
        _maxLength = maxLength;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var list = value as List<string>;
        if (list != null)
        {
            foreach (var item in list)
            {
                if (item.Length < _minLength || item.Length > _maxLength)
                {
                    return new ValidationResult($"Each string must be between {_minLength} and {_maxLength} characters.");
                }
            }
        }
        return ValidationResult.Success;
    }
}
