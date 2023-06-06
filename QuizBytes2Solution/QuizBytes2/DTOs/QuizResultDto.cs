using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO should be used to give the user their result at the end of a quiz
/// </summary>
public class QuizResultDto
{
    [Required]
    public string ClientSubmitTime { get; set; }
    [Required]
    public int CorrectAnswers { get; set; }
    [Required]
    public int WrongAnswers { get; set; }
}
