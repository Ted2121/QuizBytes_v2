using AutoMapper;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;

namespace QuizBytes2.Service;

public class QuizResultHandler : IQuizResultHandler
{
    private IUserRepository _userRepository;
    private IQuizPointCalculator _quizPointCalculator;

    private IMapper _mapper;
    public QuizResultHandler(IUserRepository userRepository, IMapper mapper, IQuizPointCalculator quizPointCalculator)
    {
        _userRepository = userRepository;
        _quizPointCalculator = quizPointCalculator;
        _mapper = mapper;
    }
    public async Task<bool> SubmitQuizAsync(string userId, QuizSubmitDto quizSubmitDto, DateTime serverTime)
    {
        var correctAnswers = await _quizPointCalculator.CountCorrectAnswersAsync(quizSubmitDto);
        var wrongAnswers = quizSubmitDto.SubmittedAnswers.Count() - correctAnswers;
        var pointsToAdd = _quizPointCalculator.CalculatePoints(correctAnswers, quizSubmitDto.DifficultyLevel);

        var quiz = _mapper.Map<LastQuizResult>(quizSubmitDto);
        quiz.ServerSubmitTime = serverTime.ToString();
        quiz.WrongAnswers = wrongAnswers;
        quiz.CorrectAnswers = correctAnswers;

        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            user.SpendablePoints += pointsToAdd;
            user.TotalPoints += pointsToAdd;

            var isUpdated = await _userRepository.UpdateUserLastQuizResultAsync(user, quiz);

            return isUpdated;
        }
        catch (UserNotFoundException)
        {
            throw;
        }
    }

    public async Task<bool> ValidateSubmitTimeAsync(string userId, QuizSubmitDto quizSubmitDto, DateTime serverTime)
    {
        try
        {
            var lastQuiz = await _userRepository.GetLastQuizByUserIdAsync(userId);

            // if it's null it means there is no need for validation
            if (lastQuiz == null)
            {
                return true;
            }

            var lastQuizSubmitTime = lastQuiz.ServerSubmitTime;

            DateTime parsedTime;

            var success = DateTime.TryParse(lastQuizSubmitTime, out parsedTime);
            if (!success)
            {
                throw new Exception("invalid datetime format");
            }

            var timeDifference = serverTime - parsedTime;
            var requiredInterval = TimeSpan.FromMinutes(1);

            if (timeDifference < requiredInterval)
            {
                // The user has already submitted a quiz within the allowed interval
                return false;
            }

            return true;
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (ResourceNotFoundException)
        {
            throw;
        }
    }
}
