using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public interface IQuizPointCalculator
{
    int CalculatePoints(int correctAnswers, int difficultyLevel);
    Task<int> CountCorrectAnswersAsync(QuizSubmitDto quizSubmitDto);
}
