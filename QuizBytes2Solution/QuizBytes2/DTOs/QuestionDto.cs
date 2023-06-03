using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class QuestionDto
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Text { get; set; }
    [Required]
    public string Subject { get; set; }
    [Required]
    public string Course { get; set; }
    [Required]
    public string Chapter { get; set; }
    [Required]
    public int DifficultyLevel { get; set; }
}
