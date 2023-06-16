namespace RepositoryIntegrationTests;

[TestFixture]
public class QuestionRepositoryIntegrationTests
{
    private Question _question;
    private IQuestionRepository _questionRepository;
    private AppDbContext _appDbContext;
    public const string SKIP_TEARDOWN = "SkipTearDown";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        ConfigureDbContext();
        InitializeRepository();
    }

    [SetUp]
    public void Setup()
    {
        InitializeQuestion();
        Configuration.ValidateModel(_question);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (!CheckForSkipTearDown())
        {
            await _questionRepository.DeleteQuestionAsync(_question.Id);
        }
    }

    [Test]
    public async Task ShouldReturnQuestionIdWhenQuestionIsCreated()
    {
        // Arrange is done in Set up

        // Act
        _question.Id = await _questionRepository.CreateQuestionAsync(_question);

        // Assert
        Assert.That(_question.Id, Is.Not.Null);
    }

    [Category(SKIP_TEARDOWN)]
    [Test]
    public async Task ShouldReturnAListOfQuestionsWhenGettingRandomQuestions()
    {
        Question _question1 = InitializeRandomQuestion();
        Question _question2 = InitializeRandomQuestion();
        Question _question3 = InitializeRandomQuestion();
        Question _question4 = InitializeRandomQuestion();

        try
        {
            // Arrange
            string _chapter = "Chapter 1";
            int _difficulty = 1;
            int _questionCount = 3;


            await _questionRepository.CreateQuestionAsync(_question1);
            await _questionRepository.CreateQuestionAsync(_question2);
            await _questionRepository.CreateQuestionAsync(_question3);
            await _questionRepository.CreateQuestionAsync(_question4);

            // Act
            var returnedQuestions = await _questionRepository.GetRandomQuestionsFromChapterAsync(_chapter, _difficulty, _questionCount);

            // Assert
            Assert.That(returnedQuestions.Any, Is.True);

        }
        finally
        {
            await _questionRepository.DeleteQuestionAsync(_question1.Id);
            await _questionRepository.DeleteQuestionAsync(_question2.Id);
            await _questionRepository.DeleteQuestionAsync(_question3.Id);
            await _questionRepository.DeleteQuestionAsync(_question4.Id);

        }
    }

    [Test]
    public async Task ShouldReturnStringWhenGettingHintByQuestionId()
    {
        // Arrange
        await _questionRepository.CreateQuestionAsync(_question);


        // Act
        var hint = await _questionRepository.GetHintForQuestionByIdAsync(_question.Id);

        // Assert
        Assert.That(hint, Is.Not.Null.Or.Empty);
    }

    [Test]
    public async Task ShouldReturnAnyQuestionWhenGettingAll()
    {
        // Arrange
        await _questionRepository.CreateQuestionAsync(_question);

        // Act
        var questions = await _questionRepository.GetAllQuestionsAsync();

        // Assert
        Assert.That(questions.Any, Is.True);
    }

    [Test]
    public async Task ShouldReturnQuestionWhenGettingById()
    {
        // Arrange
        await _questionRepository.CreateQuestionAsync(_question);

        // Act
        var question = await _questionRepository.GetQuestionByIdAsync(_question.Id);

        // Assert
        Assert.That(question, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnTrueWhenUpdatingQuestion()
    {
        // Arrange
        await _questionRepository.CreateQuestionAsync(_question);

        // Act
        _question.Text = "NewQuestionText";
        var isUpdated = await _questionRepository.UpdateQuestionAsync(_question);

        // Assert
        Assert.That(isUpdated, Is.True);
    }

    [Category(SKIP_TEARDOWN)]
    [Test]
    public async Task ShouldReturnTrueWhenQuestionIsDeleted()
    {
        // Arrange
        await _questionRepository.CreateQuestionAsync(_question);

        // Act
        var isDeleted = await _questionRepository.DeleteQuestionAsync(_question.Id);

        // Assert
        Assert.That(isDeleted, Is.True);
    }

    [Category(SKIP_TEARDOWN)]
    [Test]
    public void ShouldThrowExceptionWhenNullQuestionIsInserted()
    {
        // Arrange
        _question = null;

        // Act


        // Assert
        Assert.That(async () => await _questionRepository.CreateQuestionAsync(_question), Throws.Exception);
    }

    [Test]
    public async Task ShouldReturnQuestionWhenGettingByPredicate()
    {
        // Arrange
        await _questionRepository.CreateQuestionAsync(_question);

        // Act
        var questions = await _questionRepository.GetQuestionsAsync(u => u.Text == "Testloremipsum");

        // Assert
        Assert.That(questions.Any, Is.True);
    }

    private static bool CheckForSkipTearDown()
    {
        var categories = TestContext.CurrentContext.Test?.Properties["Category"];

        bool skipSetup = categories != null && categories.Contains("SkipTearDown");
        return skipSetup;
    }

    private void InitializeQuestion()
    {
        _question = new Question()
        {
            Id = Guid.NewGuid().ToString(),
            Text = "Testloremipsum",
            Hint = "TestHintloremipsum",
            CorrectAnswers = new List<string> { "test1", "test2" },
            WrongAnswers = new List<string> { "test3" },
            Course = "test course lorem ipsum",
            Chapter = "test chapter lorem ipsum",
            DifficultyLevel = 1
        };
    }

    private Question InitializeRandomQuestion()
    {
        return new Question()
        {
            Id = Guid.NewGuid().ToString(),
            Text = "Test",
            Hint = "test",
            CorrectAnswers = new List<string>() { "test1", "test2" },
            WrongAnswers = new List<string>() { "test3", "test4" },
            Course = "course",
            Chapter = "Chapter 1",
            DifficultyLevel = 1
        };
    }
    private void ConfigureDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseCosmos(Configuration.CONNECTION_STRING, Configuration.DATABASE)
            .Options;

        _appDbContext = new AppDbContext(options);
        _appDbContext.Database.EnsureCreated();
    }

    private void InitializeRepository()
    {
        _questionRepository = new QuestionRepository(_appDbContext);
    }
}
