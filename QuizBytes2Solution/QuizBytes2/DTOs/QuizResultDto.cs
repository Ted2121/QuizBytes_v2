using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class QuizResultDto
{
    [Required]
    public string ClientSubmitTime { get; set; }
    [Required]
    public int CorrectAnswers { get; set; }
    [Required]
    public int WrongAnswers { get; set; }
}
