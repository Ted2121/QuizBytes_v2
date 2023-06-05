using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;

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
        List<QuestionDto> questionDtos;

        try
        {
            var questions = await _questionRepository.GetRandomQuestionsFromChapterAsync(chapter, difficulty, questionCount);

            foreach (var question in questions)
            {
                _questionCache.Set(question.Id, question, TimeSpan.FromMinutes(45));
            }

            questionDtos = _mapper.Map<List<QuestionDto>>(questions);
        }
        catch (ResourceNotFoundException)
        {
            throw;
        }

        return new QuizDto() { Questions = questionDtos};
    }
}
