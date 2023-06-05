using QuizBytes2.DTOs;
using QuizBytes2.Service;

namespace ServiceUnitTests;

[TestFixture]
public class QuizGeneratorUnitTests
{
    private string _chapter = "Chapter 1";
    private int _difficulty = 1;
    private int _questionCount = 3;
    private List<Question> _questions;
    private List<QuestionDto> _questionDtos;
    private Mock<IMemoryCache> _memoryCacheMock;
    private Mock<IQuestionRepository> _questionRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private QuizGenerator _quizGenerator;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        InitializeQuestions();
        InitializeQuestionDtos();
        InitializeMocks();
        InitializeQuizGenerator();
    }

    [Test]
    public async Task ShouldCreateQuizFromAListOfQuestions()
    {
        // Arrange is done in SetUp
        var cacheEntryMock = new Mock<ICacheEntry>();

        _memoryCacheMock.Setup(cache => cache.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);
        cacheEntryMock.Setup(entry => entry.SetValue(It.IsAny<object>()));
        cacheEntryMock.Setup(entry => entry.SetAbsoluteExpiration(It.IsAny<TimeSpan>()));

        // Act
        await _quizGenerator.CreateQuizAsync(_chapter, _difficulty, _questionCount);

        // Assert
        foreach (var questionDto in _questionDtos)
        {
            _memoryCacheMock.Verify(cache => cache.Set(questionDto.Id, questionDto, TimeSpan.FromMinutes(45)), Times.Once);
        }
    }

    private void InitializeQuestions()
    {
        _questions = new List<Question>
    {
        new Question { Id = "1", Text = "Question 1" },
        new Question { Id = "2", Text = "Question 2" },
        new Question { Id = "3", Text = "Question 3" }
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
    private void InitializeMocks()
    {
        InitializeMemoryCacheMock();
        InitializeMapperMock();
        InitializeRepositoryMock();
    }

    private void InitializeMapperMock()
    {

        _mapperMock = new Mock<IMapper>();
        _mapperMock.Setup(mapper => mapper.Map<List<QuestionDto>>(_questions))
            .Returns(_questionDtos);
    }

    private void InitializeRepositoryMock()
    {
       

        _questionRepositoryMock = new Mock<IQuestionRepository>();
        _questionRepositoryMock.Setup(repo => repo.GetRandomQuestionsFromChapterAsync(_chapter, _difficulty, _questionCount))
            .ReturnsAsync(_questions);
    }

    private void InitializeQuizGenerator()
    {
        _quizGenerator = new QuizGenerator(_memoryCacheMock.Object, _questionRepositoryMock.Object, _mapperMock.Object);
    }

    private void InitializeMemoryCacheMock()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        
    }
}
