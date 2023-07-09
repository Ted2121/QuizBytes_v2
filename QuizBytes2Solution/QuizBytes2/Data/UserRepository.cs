using Microsoft.EntityFrameworkCore;
using QuizBytes2.Encryption;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using User = QuizBytes2.Models.User;

namespace QuizBytes2.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<string> CreateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var existingUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == user.Username.ToLower());
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException($"Username: {user.Username} is already used");
        }

        try
        {
            // This needs to be done so that password hashing doesn't affect the argument's reference
            var userToInsert = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = user.Username,
                Password = PasswordEncryption.HashPassword(user.Password),
                Role = "user",
                TotalPoints = 0,
                SpendablePoints = 0,
                LastQuizResult = null
            };

            await _appDbContext.AddAsync(userToInsert);

            var saved = await SaveChangesAsync();

            if (saved)
            {
                user.Id = userToInsert.Id;
                return user.Id;
            }
            else
            {
                throw new Exception($"User with id: {user.Id} could not be saved.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"User with id: {user.Id} could not be created. Exception was {ex.Message}");
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var user =
            _appDbContext.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                throw new UserNotFoundException($"User with id: {id} not found.");
            }

            _appDbContext.Remove<User>(user);
            var isDeleted = await SaveChangesAsync();

            return isDeleted;
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not delete user with id: {id}. Exception was: {ex}");
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            return await _appDbContext.Users.ToListAsync();
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed getting all users. Exception was: {ex}");
        }
    }

    public async Task<LastQuizResult> GetLastQuizByUserIdAsync(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException();
        }

        try
        {
            var lastQuizResult = await _appDbContext.Users
                .AsNoTracking()
                .Where(u => u.Id.Equals(id))
                .Select(u => u.LastQuizResult)
                .FirstOrDefaultAsync();

            if (lastQuizResult == null)
            {
                return null;
            }

            return lastQuizResult;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed getting last quiz of user with id: {id}. Exception was: {ex}");
        }
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException();
        }

        try
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null)
            {
                throw new UserNotFoundException($"User with id: {id} not found.");
            }

            return user;
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed getting user with id: {id}. Exception was: {ex}");

        }
    }

    public async Task<IEnumerable<User>> GetUsersAsync(Expression<Func<User, bool>> predicate)
    {
        var query = _appDbContext.Users
    .Where(predicate)
    .AsAsyncEnumerable();

        List<User> results = new List<User>();
        await foreach (var user in query)
        {
            results.Add(user);
        }

        return results;
    }

    public User Login(string username, string password)
    {

        try
        {
            var user = _appDbContext.Users.Where(u => u.Username.Equals(username)).FirstOrDefault();

            if (user != null && PasswordEncryption.ValidatePassword(password, user.Password))
            {
                return user;
            }
            else
            {
                throw new Exception($"Invalid password for user with username: {username}");
            }

        }
        catch (Exception ex)
        {

            throw new Exception($"Error logging in User with username: {username}: '{ex.Message}'.");
        }
    }

    public async Task<bool> UpdateUserLastQuizResultAsync(User user, LastQuizResult lastQuizResult)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (lastQuizResult == null)
        {
            throw new ArgumentNullException(nameof(lastQuizResult));
        }

        try
        {
            var userToUpdate = await _appDbContext.FindAsync<User>(user.Id);

            if (userToUpdate == null)
            {
                throw new UserNotFoundException($"User with id: {user.Id} not found.");
            }

            userToUpdate.LastQuizResult = lastQuizResult;
            userToUpdate.TotalPoints = user.TotalPoints;
            userToUpdate.SpendablePoints = user.SpendablePoints;

            return await SaveChangesAsync();
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed updating last quiz results for user with id: {user.Id}. Exception was: {ex}");
        }
    }

    public async Task<bool> UpdatePasswordAsync(string username, string oldPassword, string newPassword)
    {
        try
        {
            var loggedUser = Login(username, oldPassword);
            var newPasswordHashed = PasswordEncryption.HashPassword(newPassword);

            loggedUser.Password = newPasswordHashed;

            return await SaveChangesAsync();

        }
        catch (Exception ex)
        {

            throw new Exception($"Error changing password for user with username: {username}: '{ex.Message}'.");
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            var userToUpdate = await _appDbContext.FindAsync<User>(user.Id);

            if (userToUpdate == null)
            {
                throw new UserNotFoundException($"User with id: {user.Id} not found.");
            }

            userToUpdate.Username = user.Username;
            userToUpdate.Role = user.Role;

            return await SaveChangesAsync();
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed updating user with id: {user.Id}. Exception was: {ex}");
        }
    }

    public async Task<bool> UpdateUserWithSpentPointsAsync(string id, int pointsToDeduct)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (pointsToDeduct < 0)
        {
            throw new ArgumentException(nameof(pointsToDeduct));
        }

        try
        {
            var userToUpdate = await _appDbContext.FindAsync<User>(id);


            if (userToUpdate == null)
            {
                throw new UserNotFoundException($"User with id: {id} not found.");
            }

            var currentETag = _appDbContext.Entry(userToUpdate).Property("ETag").CurrentValue as string;

            if (userToUpdate.SpendablePoints < pointsToDeduct)
            {
                return false;
            }

            userToUpdate.SpendablePoints -= pointsToDeduct;

            _appDbContext.Entry(userToUpdate).Property("ETag").CurrentValue = Guid.NewGuid().ToString();

            _appDbContext.Update(userToUpdate);

            var savedChanges = await SaveChangesWithConcurrencyControlAsync(currentETag);

            return savedChanges;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed updating user with id: {id}. Exception was: {ex}");
        }
    }

    public async Task<bool> UpdateUserWithCourseProgressionAsync(string id, string course, string chapter)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var userToUpdate = await _appDbContext.FindAsync<User>(id);


            if (userToUpdate == null)
            {
                throw new UserNotFoundException($"User with id: {id} not found.");
            }

            if (userToUpdate.CourseProgressions == null || !userToUpdate.CourseProgressions.Any(cp => cp.CourseName.ToLower().Equals(course.ToLower())))
            {
                var newCourseProgression = new CourseProgression
                {
                    CourseName = course.ToLower(),
                    Chapters = new List<string> { chapter }
                };

                userToUpdate.CourseProgressions ??= new List<CourseProgression>();
                userToUpdate.CourseProgressions.Add(newCourseProgression);

            }
            else
            {
                var existingCourseProgression = userToUpdate.CourseProgressions
                    .SingleOrDefault(cp => cp.CourseName.ToLower().Equals(course.ToLower()));

                if (existingCourseProgression != null)
                {
                    existingCourseProgression.Chapters.Add(chapter);
                }
                else
                {
                    throw new Exception("Something went wrong");
                }
            }

            return await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed updating user with id: {id}. Exception was: {ex}");
        }
    }

    public async Task<CourseProgression> GetUserProgressionByCourseNameAsync(string id, string course)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var user = await _appDbContext.FindAsync<User>(id);


            if (user == null)
            {
                throw new UserNotFoundException($"User with id: {id} not found.");
            }

            if (user.CourseProgressions == null)
            {
                return null;
            }

            var courseProgression = user.CourseProgressions.SingleOrDefault(cp => cp.CourseName.ToLower().Equals(course.ToLower()));

            return courseProgression;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed getting course progression for user with id: {id}. Exception was: {ex}");
        }
    }

    private async Task<bool> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync() >= 0;
    }

    private async Task<bool> SaveChangesWithConcurrencyControlAsync(string currentETag, int retryCount = 0, int maxRetries = 2)
    {
        // used to break out of a possible infinite loop due to recursive call
        if (retryCount >= maxRetries)
        {
            return false;
        }

        try
        {
            return await SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is User user)
                {
                    await entry.ReloadAsync();

                    var newETag = _appDbContext.Entry(user).Property("ETag").CurrentValue as string;

                    if (currentETag != newETag)
                    {
                        return false;
                    }

                    _appDbContext.Entry(user).Property("ETag").CurrentValue = Guid.NewGuid().ToString();
                }
            }

            return await SaveChangesWithConcurrencyControlAsync(currentETag, retryCount + 1);
        }
    }


}
