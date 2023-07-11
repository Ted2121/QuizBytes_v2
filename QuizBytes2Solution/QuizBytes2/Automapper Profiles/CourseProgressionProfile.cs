using AutoMapper;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Automapper_Profiles;

public class CourseProgressionProfile : Profile
{
    public CourseProgressionProfile()
    {
        CreateMap<CourseProgression, CourseProgressionDto>().ReverseMap();
    }
}
