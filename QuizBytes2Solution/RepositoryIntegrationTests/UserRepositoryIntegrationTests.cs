namespace RepositoryIntegrationTests;

[TestFixture]
public class UserRepositoryIntegrationTests
{
    private User _user;
    private LastQuizResult _lastQuizResult;
    private IUserRepository _userRepository;
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
        InitializeUser();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (!CheckForSkipTearDown())
        {
            await _userRepository.DeleteUserAsync(_user.Id);
        }
    }

    [Test]
    public async Task ShouldReturnUserIdWhenUserIsCreated()
    {
        // Arrange is done in Set up

        // Act
        _user.Id = await _userRepository.CreateUserAsync(_user);

        // Assert
        Assert.That(_user.Id, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnAnyUserWhenGettingAll()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var users = await _userRepository.GetAllUsersAsync();

        // Assert
        Assert.That(users.Any, Is.True);
    }

    [Test]
    public async Task ShouldReturnUserWhenGettingById()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var user = await _userRepository.GetUserByIdAsync(_user.Id);

        // Assert
        Assert.That(user, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnTrueWhenUpdatingUser()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        _user.Username = "NewUsername";
        var isUpdated = await _userRepository.UpdateUserAsync(_user);

        // Assert
        Assert.That(isUpdated, Is.True);
    }

    [Test]
    public async Task ShouldReturnUserWhenLoggingIn()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var user = _userRepository.Login(_user.Username, _user.Password);

        // Assert
        Assert.That(user, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnTrueWhenPasswordIsUpdated()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var isPasswordUpdated = await _userRepository.UpdatePasswordAsync(_user.Username, _user.Password, "NewPassword");

        // Assert
        Assert.That(isPasswordUpdated, Is.True);
    }

    [Category(SKIP_TEARDOWN)]
    [Test]
    public async Task ShouldReturnTrueWhenUserIsDeleted()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var isDeleted = await _userRepository.DeleteUserAsync(_user.Id);

        // Assert
        Assert.That(isDeleted, Is.True);
    }

    [Category(SKIP_TEARDOWN)]
    [Test]
    public async Task ShouldThrowExceptionWhenNullUserIsInserted()
    {
        // Arrange
        _user = null;

        // Act


        // Assert
        Assert.That(async () => await _userRepository.CreateUserAsync(_user), Throws.Exception);
    }

    [Test]
    public async Task ShouldReturnUserWhenGettingByPredicate()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var users = await _userRepository.GetUsersAsync(u => u.Username == "Test");

        // Assert
        Assert.That(users.Any, Is.True);
    }

    [Test]
    public async Task ShouldReturnTrueWhenUpdatingUserLastQuizResult()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);
        InitializeQuizResult();

        // Act
        var result = await _userRepository.UpdateUserLastQuizResultAsync(_user, _lastQuizResult);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ShouldReturnAnInstanceWhenGettingLastQuizResultOfUser()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);
        InitializeQuizResult();
        await _userRepository.UpdateUserLastQuizResultAsync(_user, _lastQuizResult);

        // Act
        var quiz = await _userRepository.GetLastQuizByUserIdAsync(_user.Id);

        // Assert
        Assert.That(quiz, Is.Not.Null);
    }

    private static bool CheckForSkipTearDown()
    {
        var categories = TestContext.CurrentContext.Test?.Properties["Category"];

        bool skipSetup = categories != null && categories.Contains("SkipTearDown");
        return skipSetup;
    }

    private void InitializeUser()
    {
        _user = new User
        {
            Username = "Test",
            Password = "password",
            Role = "user",
            TotalPoints = 555,
            SpendablePoints = 77,
            LastQuizResult = null
        };
    }

    private void ConfigureDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseCosmos(Configuration.CONNECTION_STRING, Configuration.DATABASE)
            .Options;

        _appDbContext = new AppDbContext(options);
    }

    private void InitializeQuizResult()
    {
        _lastQuizResult = new LastQuizResult()
        {
            ClientSubmitTime = DateTime.UtcNow.ToString(),
            CorrectAnswers = 5,
            WrongAnswers = 5,
            DifficultyLevel = 3
        };
    }

    private void InitializeRepository()
    {
        _userRepository = new UserRepository(_appDbContext);
    }

}
