using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO is used internally by the QuizPointCalculator
/// </summary>
public class UserAnswerDto
{
    [Required]
    public string QuestionId { get; set; }
    [Required]
    public List<string> SelectedOptions { get; set; }
}
