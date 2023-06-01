using QuizBytes2.Models;
using System.Linq.Expressions;

namespace QuizBytes2.Data;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersAsync(Expression<Func<User, bool>> predicate);
    Task<User> GetUserByIdAsync(string id);
    Task<string> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(string id);
    User Login(string username, string password);
    Task<bool> UpdatePasswordAsync(string username, string oldPassword, string newPassword);
    Task<bool> UpdateLastQuizByUserIdAsync(string id, LastQuizResult lastQuizResult);
    Task<LastQuizResult> GetLastQuizByUserIdAsync(string id);
}
