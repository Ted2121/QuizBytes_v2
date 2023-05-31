namespace QuizBytes2.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public string Email { get; set; }
    public int ElapsedSecondsInChallenge { get; set; }
    public int CorrectAnswers { get; set; }
}
