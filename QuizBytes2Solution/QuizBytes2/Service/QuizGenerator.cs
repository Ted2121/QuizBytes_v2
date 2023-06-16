using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Service.Extensions;

namespace QuizBytes2.Service;

public class QuizGenerator : IQuizGenerator
{
    private readonly IMemoryCache _questionCache;
    private IQuestionRepository _questionRepository;
    private IMapper _mapper;
    public QuizGenerator(IMemoryCache questionCache, IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionCache = questionCache;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<QuizDto> CreateQuizAsync(string chapter, int difficulty, int questionCount)
    {
        List<QuestionDto> questionDtos = new List<QuestionDto>();

        try
        {
            var questions = await _questionRepository.GetRandomQuestionsFromChapterAsync(chapter, difficulty, questionCount);

            foreach (var question in questions)
            {
                // Add questions to cache
                _questionCache.Set(question.Id, question, TimeSpan.FromMinutes(45));

                var questionDto = _mapper.Map<QuestionDto>(question);

                // we add the correct and wrong answers to the list and we shuffle it
                var possibleAnswers = question.CorrectAnswers
                    .Concat(question.WrongAnswers)
                    .ToList();

                possibleAnswers.Shuffle();

                questionDto.PossibleAnswers = possibleAnswers;

                questionDtos.Add(questionDto);
            }

        }
        catch (ResourceNotFoundException)
        {
            throw;
        }

        return new QuizDto() { Questions = questionDtos};
    }
}
