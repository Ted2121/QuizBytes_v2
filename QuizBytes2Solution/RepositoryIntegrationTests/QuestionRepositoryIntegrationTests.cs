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
    public async Task ShouldThrowExceptionWhenNullQuestionIsInserted()
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
        var questions = await _questionRepository.GetQuestionsAsync(u => u.Text == "Test");

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
            Text = "Test",
            Hint = "TestHint",
            CorrectAnswers = new List<string> { "test1", "test2"},
            WrongAnswers = new List<string> { "test3"},
            Subject = "test subject",
            Course = "test course",
            Chapter = "test chapter",
            DifficultyLevel = 1
        };
    }

    private void ConfigureDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseCosmos(Configuration.CONNECTION_STRING, Configuration.DATABASE)
            .Options;

        _appDbContext = new AppDbContext(options);
    }

    private void InitializeRepository()
    {
        _questionRepository = new QuestionRepository(_appDbContext);
    }
}
