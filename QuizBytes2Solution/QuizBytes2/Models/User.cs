using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class User
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
    public int TotalPoints { get; set; }
    public int SpendablePoints { get; set; }
    public LastQuizResult? LastQuizResult { get; set; }
}
