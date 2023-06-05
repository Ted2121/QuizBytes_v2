using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Service;

public class QuizPointCalculator : IQuizPointCalculator
{
    private IQuestionRepository _questionRepository;
    private readonly IMemoryCache _questionCache;

    public QuizPointCalculator(IQuestionRepository questionRepository, IMemoryCache questionCache)
    {
        _questionRepository = questionRepository;
        _questionCache = questionCache;
    }
    public async Task<int> CountCorrectAnswersAsync(QuizSubmitDto quizSubmitDto)
    {
        var submittedAnswers = quizSubmitDto.SubmittedAnswers.ToList();

        var questions = await GetListOfQuestions(submittedAnswers);

        var correctAnswers = VerifyAnswers(questions, submittedAnswers);

        return correctAnswers;
    }


    public int CalculatePoints(int correctAnswers, int difficultyLevel)
    {
        return correctAnswers * difficultyLevel;
    }

    private async Task<List<Question>> GetListOfQuestions(List<UserAnswerDto> submittedAnswers)
    {
        List<Question> questions = new List<Question>();

        foreach (var answer in submittedAnswers)
        {
            Question question;

            // We try to get the question from the cache before getting it from the database
            if (!_questionCache.TryGetValue(answer.QuestionId, out question))
            {
                question = await _questionRepository.GetQuestionByIdAsync(answer.QuestionId);
                //_questionCache.Set(answer.QuestionId, question, TimeSpan.FromMinutes(45));
            }

            questions.Add(question);
        }
        return questions;
    }

    private int VerifyAnswers(List<Question> questions, List<UserAnswerDto> answers)
    {
        int correctCount = 0;
        int wrongCount = 0;

        foreach (var question in questions)
        {
            var correctAnswers = question.CorrectAnswers;
            var userAnswer = answers.FirstOrDefault(a => a.QuestionId == question.Id);

            if (userAnswer != null)
            {
                var userSelectedOptions = userAnswer.SelectedOptions;

                if (correctAnswers.All(ca => userSelectedOptions.Contains(ca)))
                {
                    // All correct answers are selected by the user
                    correctCount++;
                }
                else
                {
                    // At least one correct answer is missing or one wrong answer is selected
                    wrongCount++;
                }
            }
            else
            {
                // User did not answer this question
                wrongCount++;
            }
        }

        return correctCount;
    }
}
