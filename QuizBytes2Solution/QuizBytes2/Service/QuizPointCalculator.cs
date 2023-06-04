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
    private IMapper _mapper;

    public QuizPointCalculator(IQuestionRepository questionRepository, IMemoryCache questionCache, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _questionCache = questionCache;
        _mapper = mapper;
    }
    public async Task<int> CalculatePointsAsync(QuizSubmitDto quizSubmitDto)
    {
        var submittedAnswers = quizSubmitDto.SubmittedAnswers.ToList();
        List<QuestionCachingDto> questions = new List<QuestionCachingDto>();

        foreach(var answer in submittedAnswers)
        {
            QuestionCachingDto cachedQuestion;

            // We try to get the question from the cache before getting it from the database
            if (!_questionCache.TryGetValue(answer.QuestionId, out cachedQuestion))
            {
                var question = await _questionRepository.GetQuestionByIdAsync(answer.QuestionId);
                cachedQuestion = _mapper.Map<QuestionCachingDto>(question);
                _questionCache.Set(answer.QuestionId, cachedQuestion, TimeSpan.FromMinutes(30));
            }
           
            questions.Add(cachedQuestion);
        }

        // TODO now that we have a list of questions, check if answers are correct and make calculations
    }


    private int VerifyAnswers(List<QuestionCachingDto> questions, List<UserAnswerDto> answers)
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
