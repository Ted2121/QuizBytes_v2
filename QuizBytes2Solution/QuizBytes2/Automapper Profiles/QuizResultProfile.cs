using AutoMapper;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Automapper_Profiles;

public class QuizResultProfile : Profile
{
    public QuizResultProfile()
    {
        CreateMap<LastQuizResult, QuizResultDto>();
        CreateMap<QuizSubmitDto, LastQuizResult>();
    }
}
