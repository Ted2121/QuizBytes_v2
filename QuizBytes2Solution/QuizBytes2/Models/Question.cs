namespace QuizBytes2.Models;

public class Question
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string Hint { get; set; } = String.Empty;
    public int FKChapterId { get; set; }
}
