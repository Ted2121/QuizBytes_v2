using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO should be used when the user submits a quiz
/// </summary>
public class QuizSubmitDto
{
    // This time is for display purposes only and should not be used server side for validation
    [Required]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", ErrorMessage = "Invalid date and time format.")]
    public string ClientSubmitTime { get; set; }
    [Required]
    [Range(1, 3, ErrorMessage = "difficulty level must be a positive number between 1 and 3.")]
    public int DifficultyLevel { get; set; }
    [Required]
    public IEnumerable<UserAnswerDto> SubmittedAnswers { get; set; }
}
