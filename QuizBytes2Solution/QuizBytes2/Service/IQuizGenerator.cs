using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public interface IQuizGenerator
{
    Task<QuizDto> CreateQuizAsync(string chapter, int difficulty, int questionCount);
}
