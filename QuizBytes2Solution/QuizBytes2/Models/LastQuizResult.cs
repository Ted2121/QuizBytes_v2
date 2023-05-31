using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class LastQuizResult
{
    public string? Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string SubmitTime { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int DifficultyLevel { get; set; }
    public int PointsEarned { get; set; }
}
