using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class UserAnswerDto
{
    [Required]
    public string QuestionId { get; set; }
    [Required]
    public string AnswerText { get; set; }
}
