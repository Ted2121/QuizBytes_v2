using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Annotations;

public class StringLengthCollectionAttribute : ValidationAttribute
{
    private readonly int _minLength;
    private readonly int _maxLength;

    public StringLengthCollectionAttribute(int minLength, int maxLength)
    {
        _minLength = minLength;
        _maxLength = maxLength;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var enumerable = value as IEnumerable<string>;
        if (enumerable != null)
        {
            foreach (var item in enumerable)
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