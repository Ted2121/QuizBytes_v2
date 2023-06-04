using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class QuestionCachingDto
{
    [Required]
    public string Id { get; set; }
    [Required] 
    public string Hint { get; set; }
    [Required]
    public ICollection<string> CorrectAnswers { get; set; } = new List<string>();
    [Required]
    public ICollection<string> WrongAnswers { get; set; } = new List<string>();
    [Required]
    public int DifficultyLevel { get; set; }
}
