namespace QuizBytes2.Exceptions;

[Serializable]
public class UserNotFoundException : Exception
{
    public UserNotFoundException() { }

    public UserNotFoundException(string message)
        : base(message) { }

    public UserNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
