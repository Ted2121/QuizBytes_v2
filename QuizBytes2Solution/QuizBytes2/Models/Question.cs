using QuizBytes2.Annotations;
using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class Question
{
    [Required]
    public string Id { get; set; }
    [Required]
    [StringLength(300, MinimumLength = 10)]
    public string Text { get; set; }
    [Required]
    [StringLength(300, MinimumLength = 10)]
    public string Hint { get; set; }
    [Required]
    [StringLengthList(5, 100)]
    public ICollection<string> CorrectAnswers { get; set; } = new List<string>();
    [Required]
    [StringLengthList(5, 100)]
    public ICollection<string> WrongAnswers { get; set; } = new List<string>();
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string? Subject { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string? Course { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string? Chapter { get; set; }
    [Required]
    [Range(1, 3, ErrorMessage = "difficulty level must be a positive number between 1 and 3.")]
    public int DifficultyLevel { get; set; }
}
