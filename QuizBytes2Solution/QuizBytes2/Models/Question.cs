using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class Question
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Text { get; set; }
    [Required]
    public string Hint { get; set; }
    [Required]
    public ICollection<string> CorrectAnswers { get; set; } = new List<string>();
    [Required]
    public ICollection<string> WrongAnswers { get; set; } = new List<string>();
    [Required]
    public string? Subject { get; set; }
    [Required]
    public string? Course { get; set; }
    [Required]
    public string? Chapter { get; set; }
    [Required]
    public int DifficultyLevel { get; set; }
}
