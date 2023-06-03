using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public interface IQuizPointCalculator
{
    int CalculatePoints(QuizSubmitDto quizSubmitDto);
}
