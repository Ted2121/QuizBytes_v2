namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO should be used when the user submits a quiz
/// </summary>
public class QuizSubmitDto
{
    // This time is for display purposes only and should not be used server side for validation
    public string ClientSubmitTime { get; set; }
    public int DifficultyLevel { get; set; }
    public IEnumerable<UserAnswerDto> SubmittedAnswers { get; set; }
}
