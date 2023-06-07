using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO should be used to give the user questions
/// that do not contain correct/wrong answers and hints
/// </summary>
public class QuestionDto
{
    [Required]
    public string Id { get; set; }
    [Required]
    [StringLength(300, MinimumLength = 10)]
    public string Text { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Subject { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Course { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Chapter { get; set; }
    [Required]
    [Range(1, 3, ErrorMessage = "difficulty level must be a positive number between 1 and 3.")]
    public int DifficultyLevel { get; set; }
}
