using QuizBytes2.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class UserDto
{
    public string? Id { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 5)]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9]*$", ErrorMessage = "Username must start with a letter and can only contain letters and numbers.")]
    public string Username { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "TotalPoints must be a positive number.")]
    public int TotalPoints { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "SpendablePoints must be a positive number.")]
    public int SpendablePoints { get; set; }

    public LastQuizResult? LastQuizResult { get; set; }
}
