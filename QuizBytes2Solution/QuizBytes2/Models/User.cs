using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.Models;

public class User
{
    [Key]
    [RegularExpression(@"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$",
        ErrorMessage = "The Id field must be a valid GUID format.")]
    public string Id { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 5)]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9]*$", ErrorMessage = "Username must start with a letter and can only contain letters and numbers.")]
    public string Username { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 5)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain at least 8 characters, including uppercase, lowercase, numeric, and special characters.")]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "TotalPoints must be a positive number.")]
    public int TotalPoints { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "SpendablePoints must be a positive number.")]
    public int SpendablePoints { get; set; }
    public LastQuizResult? LastQuizResult { get; set; }
    [ConcurrencyCheck]
    public string ETag { get; set; }
}
