using System.ComponentModel.DataAnnotations;

namespace QuizBytes2.DTOs;

public class CourseProgressionDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string CourseName { get; set; }
    [Required]
    public List<string> Chapters { get; set; }
}
