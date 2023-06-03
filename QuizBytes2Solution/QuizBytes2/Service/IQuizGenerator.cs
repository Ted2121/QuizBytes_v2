using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public interface IQuizGenerator
{
    Task<QuizDto> GetQuizAsync(string subject, string course, string chapter, int difficulty);
}
