using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class Question
{
    public string? Id { get; set; }
    [Required]
    public string Text { get; set; }
    [Required]
    public string Hint { get; set; }
    [Required]
    public IEnumerable<string> CorrectAnswers { get; set; }
    [Required]
    public IEnumerable<string> WrongAnswers { get; set; }
    [Required]
    public string? Subject { get; set; }
    [Required]
    public string? Course { get; set; }
    [Required]
    public string? Chapter { get; set; }
    [Required]
    public int DifficultyLevel { get; set; }
}
