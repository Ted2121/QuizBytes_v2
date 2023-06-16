using QuizBytes2.Annotations;
using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO is used internally by the QuizPointCalculator
/// </summary>
public class UserAnswerDto
{
    [Required]
    [RegularExpression(@"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$",
        ErrorMessage = "The Id field must be a valid GUID format.")]
    public string QuestionId { get; set; }
    [Required]
    [StringLengthCollection(5, 100)]
    public List<string> SelectedOptions { get; set; }
}
