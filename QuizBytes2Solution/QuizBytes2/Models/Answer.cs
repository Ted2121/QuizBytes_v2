namespace QuizBytes2.Models;

public class Answer
{
    public int Id { get; set; }
    public int FKQuestionId { get; set; }
    public string IsCorrect { get; set; }
    public string AnswerText { get; set; }
}
