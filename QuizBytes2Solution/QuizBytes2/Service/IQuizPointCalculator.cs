using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public interface IQuizPointCalculator
{
    Task<int> CalculatePointsAsync(QuizSubmitDto quizSubmitDto);
}
