using System.ComponentModel.DataAnnotations;

namespace RepositoryIntegrationTests;

[TestFixture]
public class UserRepositoryIntegrationTests
{
    private User _user;
    private LastQuizResult _lastQuizResult;
    //private CourseProgression _courseProgression;
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


        Configuration.ValidateModel(_user);

        //ValidateModel(_user);
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
        var isPasswordUpdated = await _userRepository.UpdatePasswordAsync(_user.Username, _user.Password, "NewPassword1!");

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
    public void ShouldThrowExceptionWhenNullUserIsInserted()
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
        var users = await _userRepository.GetUsersAsync(u => u.Username == "Test1");

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

    [Test]
    public async Task ShouldReturnTrueIfPointsAreUpdated()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);
        InitializeQuizResult();
        await _userRepository.UpdateUserLastQuizResultAsync(_user, _lastQuizResult);

        // Act
        var isUpdated = await _userRepository.UpdateUserWithSpentPointsAsync(_user.Id, 10);

        // Assert
        Assert.That(isUpdated, Is.True);
    }

    [Test]
    public async Task ShouldReturnTrueWhenUpdatingUserWithCourseProgression()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);

        // Act
        var isUpdated = await _userRepository.UpdateUserWithCourseProgressionAsync(_user.Id, "Test", "TestChapter");


        // Assert
        Assert.That(isUpdated, Is.True);
    }

    [Test]
    public async Task ShouldReturnProgressionWhenGettingByCourseName()
    {
        // Arrange
        await _userRepository.CreateUserAsync(_user);
        await _userRepository.UpdateUserWithCourseProgressionAsync(_user.Id, "Test", "TestChapter");

        // Act
        var progression = await _userRepository.GetUserProgressionByCourseNameAsync(_user.Id, "Test");

        // Assert
        Assert.That(progression, Is.Not.Null);
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
            Username = "Test1",
            Password = "Password1!",
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
        _appDbContext.Database.EnsureCreated();
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

    //private void InitializeCourseProgression()
    //{
    //    _courseProgression = new CourseProgression()
    //    {
    //        CourseName = "Test",
    //        Chapters = new List<string> { "Chapter" }
    //    };
    //}

    private void InitializeRepository()
    {
        _userRepository = new UserRepository(_appDbContext);
    }



}
