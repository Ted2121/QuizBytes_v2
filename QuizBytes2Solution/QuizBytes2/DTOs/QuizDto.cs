namespace QuizBytes2.DTOs;

/// <summary>
/// This DTO should be used to give the user a generated quiz
/// </summary>
public class QuizDto
{
    public List<QuestionDto> Questions { get; set; }
}
