namespace QuizBytes2.Models;

public class Chapter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FKSubjectId { get; set; }
    public string Description { get; set; }
}
