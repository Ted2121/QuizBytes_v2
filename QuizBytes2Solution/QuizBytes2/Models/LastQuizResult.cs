using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class LastQuizResult
{
    //[Required]
    //public string Id { get; set; }
    [Required]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", ErrorMessage = "Invalid date and time format.")]
    public string ClientSubmitTime { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", ErrorMessage = "Invalid date and time format.")]
    public string ServerSubmitTime { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "correct answers must be a positive number.")]
    public int CorrectAnswers { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "wrong answers must be a positive number.")]
    public int WrongAnswers { get; set; }
    [Required]
    [Range(1, 3, ErrorMessage = "difficulty level must be a positive number between 1 and 3.")]
    public int DifficultyLevel { get; set; }
}
