using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

//TODO validation
public class User
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
    [Required]
    public int TotalPoints { get; set; }
    [Required]
    public int SpendablePoints { get; set; }
    [Required]
    public LastQuizResult? LastQuizResult { get; set; }
}
