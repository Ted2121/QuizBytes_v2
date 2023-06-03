namespace QuizBytes2.DTOs;

public class QuizSubmitDto
{
    // This time is for display purposes only and should not be used server side for validation
    public string ClientSubmitTime { get; set; }
    public IEnumerable<UserAnswerDto> SubmittedAnswers { get; set; }
}
