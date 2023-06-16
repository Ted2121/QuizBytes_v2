using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using QuizBytes2.Automapper_Profiles;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Models;
using QuizBytes2.Service;

namespace ServiceIntegrationTests;

[TestFixture]
public class QuestionCacheIntegrationTests
{
    private string _chapter = "Chapter 1";
    private int _difficulty = 1;
    private int _questionCount = 3;

    private Mock<IQuestionRepository> _questionRepositoryMock;
    private IMapper _mapper;

    private IMemoryCache _questionCache;

    private IQuizGenerator _quizGenerator;
    private IQuizPointCalculator _quizPointCalculator;

    private List<Question> _questions;
    private List<QuestionDto> _questionDtos;
    private QuizSubmitDto _quizSubmitDto;

    [Test]
    public async Task ShouldSaveQuestionsInCacheWhenCreatingQuiz()
    {
        // Arrange in setup


        // Act
        var quiz = await _quizGenerator.CreateQuizAsync(_chapter, _difficulty, _questionCount);

        Question questionFromCache;

        _questionCache.TryGetValue(_questions[0].Id, out questionFromCache);

        // Assert
        Assert.That(questionFromCache, Is.Not.Null);
    }

    /// <summary>
    /// For lack of a better way to test the private method that retrieves the question from the cache:
    /// 1. We create a new question that contains the correct answers that the answer has
    /// 2. We add a question to the cache that is not found in the database.
    /// 3. If the method is able to correct the submitted answer against what is obtained from the cache, it means that the cache retrieval was a success
    /// Note: this test relies heavily on how correct the implementation of CountCorrectAnswersAsync method is and is not a good test.
    /// </summary>
    [Test]
    public async Task ShouldGetQuestionFromCacheWhenCountingCorrectAnswers()
    {
        // Arrange
        Question newQuestion = new Question()
        {
            Id = "4",
            Text = "test",
            Hint = "test",
            CorrectAnswers = new List<string>() { "test1", "test2" },
            WrongAnswers = new List<string>() { "test3", "test4" },
            Course = "course",
            Chapter = "chapter",
            DifficultyLevel = 1
        };

        _questionCache.Set(newQuestion.Id, newQuestion, TimeSpan.FromMinutes(45));

        // Act
        var correctAnswers = await _quizPointCalculator.CountCorrectAnswersAsync(_quizSubmitDto);

        var isQuestionRetrieved = correctAnswers > 0;
        // Assert
        Assert.That(isQuestionRetrieved, Is.True);
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        InitializeQuestions();
        InitializeQuestionDtos();
        InitializeQuizSubmitDto();
        InitializeMemoryCache();
        InitializeMocks();
        InitializeMapper();
        InitializeQuizGenerator();
        InitializeQuizPointCalculator();
    }

    private void InitializeMocks()
    {
        InitializeRepositoryMock();
    }

    private void InitializeMapper()
    {

        var config = new MapperConfiguration(cfg => cfg.AddProfile<QuestionProfile>());
        _mapper = config.CreateMapper();
    }

    private void InitializeQuizGenerator()
    {
        _quizGenerator = new QuizGenerator(_questionCache, _questionRepositoryMock.Object, _mapper);
    }

    private void InitializeQuizPointCalculator()
    {
        _quizPointCalculator = new QuizPointCalculator(_questionRepositoryMock.Object, _questionCache);
    }

    private void InitializeRepositoryMock()
    {
        _questionRepositoryMock = new Mock<IQuestionRepository>();
        _questionRepositoryMock.Setup(repo => repo.GetRandomQuestionsFromChapterAsync(_chapter, _difficulty, _questionCount))
            .ReturnsAsync(_questions);
    }

    private void InitializeQuestions()
    {
        _questions = new List<Question>
        {
        new Question {
            Id = "1",
            Text = "Question 1",
            CorrectAnswers = new List<string>()
            {
                "answer 1", "answer 2"
            },
            WrongAnswers = new List<string>()
            {
                "answer 3", "answer 4"
            }
        },
       new Question {
            Id = "2",
            Text = "Question 2",
            CorrectAnswers = new List<string>()
            {
                "answer 1", "answer 2"
            },
            WrongAnswers = new List<string>()
            {
                "answer 3", "answer 4"
            }
        },
       new Question {
            Id = "3",
            Text = "Question 3",
            CorrectAnswers = new List<string>()
            {
                "answer 1", "answer 2"
            },
            WrongAnswers = new List<string>()
            {
                "answer 3", "answer 4"
            }
        },
        };

    }

    private void InitializeQuestionDtos()
    {
        _questionDtos = new List<QuestionDto>()
        {
        new QuestionDto { Id = "1", Text = "Question 1" },
        new QuestionDto { Id = "2", Text = "Question 2" },
        new QuestionDto { Id = "3", Text = "Question 3" }
        };
    }

    private void InitializeMemoryCache()
    {
        var memoryCacheOptions = new MemoryCacheOptions();
        _questionCache = new MemoryCache(memoryCacheOptions);
    }

    private void InitializeQuizSubmitDto()
    {
        _quizSubmitDto = new QuizSubmitDto()
        {
            ClientSubmitTime = DateTime.Now.ToString(),
            DifficultyLevel = 1,
            SubmittedAnswers = new List<UserAnswerDto>()
            {
                new UserAnswerDto
                {
                    QuestionId = "4",
                    SelectedOptions = new List<string>() { "test1", "test2"}
                }
            }
        };
    }
}
