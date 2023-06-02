using Microsoft.EntityFrameworkCore;
using QuizBytes2.Data;
using QuizBytes2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryIntegrationTests;

[TestFixture]
public class UserRepositoryIntegrationTests
{
    private User _user;
    private IUserRepository _userRepository;
    private AppDbContext _appDbContext;

    public const string SKIP_TEARDOWN = "SkipTearDown";

    string testConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    string testDatabaseName = "quizbytestests";

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
            .UseCosmos(testConnectionString, testDatabaseName)
            .Options;

        _appDbContext = new AppDbContext(options);
    }

    private void InitializeRepository()
    {
        _userRepository = new UserRepository(_appDbContext);
    }

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
}
