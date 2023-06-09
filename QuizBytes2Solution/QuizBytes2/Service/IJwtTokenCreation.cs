using QuizBytes2.Models;

namespace QuizBytes2.Service;

public interface IJwtTokenCreation
{
    string CreateToken(User user, IConfiguration configuration);
}
