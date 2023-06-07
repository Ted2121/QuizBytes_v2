using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class UserCredentialsDto
{
    [Required]
    [StringLength(15, MinimumLength = 5)]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9]*$", ErrorMessage = "Username must start with a letter and can only contain letters and numbers.")]
    public string Username { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 5)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain at least 8 characters, including uppercase, lowercase, numeric, and special characters.")]
    public string Password { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 5)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain at least 8 characters, including uppercase, lowercase, numeric, and special characters.")]
    public string NewPassword { get; set; }
}
