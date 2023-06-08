using QuizBytes2.Data;
using QuizBytes2.Exceptions;

namespace QuizBytes2.Service;

public class HintHandler : IHintHandler
{
    private IUserRepository _userRepository;
    private IQuestionRepository _questionRepository;

    public HintHandler(IUserRepository userRepository, IQuestionRepository questionRepository)
    {
        _userRepository = userRepository;
        _questionRepository = questionRepository;
    }
    public async Task<string> GetHintForQuestionById(string userId, string questionId)
    {
        try
        {
            var hint = await _questionRepository.GetHintForQuestionByIdAsync(questionId);

            if (String.IsNullOrEmpty(hint))
            {
                return null;
            }

            var isUpdated = await UpdateSpendablePoints(userId, 10);

            if (!isUpdated)
            {
                throw new ResourceNotFoundException("Not enough points");
            }

            return hint;
        }
        catch (UserNotFoundException)
        {
            throw;
        }
    }

    private async Task<bool> UpdateSpendablePoints(string userId, int pointsToDeduct)
    {
        try
        {
            return await _userRepository.UpdateUserWithSpentPointsAsync(userId, pointsToDeduct);
        }
        catch (UserNotFoundException)
        {
            throw;
        }
    }
}
