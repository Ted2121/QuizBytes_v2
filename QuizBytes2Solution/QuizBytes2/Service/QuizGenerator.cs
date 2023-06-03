using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public class QuizGenerator : IQuizGenerator
{
    public Task<QuizDto> GetQuizAsync(string subject, string course, string chapter, int difficulty)
    {
        throw new NotImplementedException();
    }
}
