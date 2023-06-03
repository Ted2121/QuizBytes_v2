namespace QuizBytes2.Service;

public class QuizResultHandler : IQuizResultHandler
{
    // TODO we map from quizdto to lastquizresult
    // we assign the server time property lastquizresult.servertime = servertimefromprop
    // we update user: await _userRepository.UpdateUserLastQuizResult(user, _lastquizresult)
}
