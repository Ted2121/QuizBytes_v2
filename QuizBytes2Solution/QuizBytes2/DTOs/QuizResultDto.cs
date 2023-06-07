using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO should be used to give the user their result at the end of a quiz
/// </summary>
public class QuizResultDto
{
    [Required]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", ErrorMessage = "Invalid date and time format.")]
    public string ClientSubmitTime { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "correct answers must be a positive number.")]
    public int CorrectAnswers { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "wrong answers must be a positive number.")]
    public int WrongAnswers { get; set; }
}
