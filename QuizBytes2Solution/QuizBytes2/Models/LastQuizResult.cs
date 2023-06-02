using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class LastQuizResult
{
    public string? Id { get; set; }
    [Required]
    public string SubmitTimestamp { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int DifficultyLevel { get; set; }
}
