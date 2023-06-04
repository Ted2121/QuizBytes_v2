using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class LastQuizResult
{
    //[Required]
    //public string Id { get; set; }
    [Required]
    public string ClientSubmitTime { get; set; }
    [Required]
    public string ServerSubmitTime { get; set; }
    [Required]
    public int CorrectAnswers { get; set; }
    [Required]
    public int WrongAnswers { get; set; }
    [Required]
    public int DifficultyLevel { get; set; }
}
