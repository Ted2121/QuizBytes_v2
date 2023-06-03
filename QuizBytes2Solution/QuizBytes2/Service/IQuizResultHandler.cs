using QuizBytes2.DTOs;

namespace QuizBytes2.Service;

public interface IQuizResultHandler
{
    Task<bool> SubmitQuizAsync(string userId, QuizSubmitDto quizSubmitDto, DateTime serverTime);
    Task<bool> ValidateSubmitTimeAsync(string userId, QuizSubmitDto quizSubmitDto, DateTime serverTime);
}
