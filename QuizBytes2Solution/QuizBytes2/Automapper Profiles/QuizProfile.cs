using AutoMapper;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Automapper_Profiles;

public class QuizProfile : Profile
{
    public QuizProfile()
    {
        CreateMap<LastQuizResult, QuizResultDto>();
        CreateMap<QuizSubmitDto, LastQuizResult>();
    }
}
