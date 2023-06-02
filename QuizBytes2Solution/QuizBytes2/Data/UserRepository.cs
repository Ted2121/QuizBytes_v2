using Microsoft.EntityFrameworkCore;
using QuizBytes2.Encryption;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;
using System.Linq.Expressions;
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
                Role = user.Role,
                TotalPoints = user.TotalPoints,
                SpendablePoints = user.SpendablePoints,
                LastQuizResult = user.LastQuizResult
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
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var user =
            _appDbContext.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                throw new NotFoundException($"User with id: {id} not found.");
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
        if (id == null)
        {
            throw new ArgumentNullException();
        }

        try
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null)
            {
                throw new NotFoundException($"User with id: {id} not found.");
            }

            if (user.LastQuizResult == null)
            {
                throw new NotFoundException($"Last quiz of user with id: {id} not found.");
            }

            return user.LastQuizResult;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        if (id == null)
        {
            throw new ArgumentNullException();
        }

        try
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null)
            {
                throw new NotFoundException($"User with id: {id} not found.");
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
                throw new NotFoundException($"User with id: {user.Id} not found.");
            }

            lastQuizResult.Id = Guid.NewGuid().ToString();

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
                throw new NotFoundException($"User with id: {user.Id} not found.");
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

    private async Task<bool> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync() >= 0;
    }
}
