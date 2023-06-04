using AutoMapper;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Automapper_Profiles;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionCachingDto>().ReverseMap();
        CreateMap<Question, QuestionDto>().ReverseMap();
    }
}
