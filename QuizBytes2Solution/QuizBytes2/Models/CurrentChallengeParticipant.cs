namespace QuizBytes2.Models;

public class CurrentChallengeParticipant
{
    public int Id { get; set; }
    public int FKWebUserId { get; set; }
    public int FKCourseId { get; set; }
}
