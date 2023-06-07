using AutoMapper;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Automapper_Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserCredentialsDto>().ReverseMap();
    }
}
