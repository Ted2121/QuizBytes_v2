namespace QuizBytes2.Service;

public interface IHintHandler
{
    Task<string> GetHintForQuestionById(string userId, string questionId);
}
