using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Models;

namespace QuizBytes2.Controllers;
[Route("api/[controller]")]
//TODO authorization
[ApiController]
public class UsersController : ControllerBase
{
    private IUserRepository _userRepository;
    private IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public async Task<ActionResult<string>> LoginUserAsync(UserLoginDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (userDto == null)
        {
            return BadRequest();
        }

        var userModel = _mapper.Map<User>(userDto);

        var authenticatedUser = await Task.Run(() => _userRepository.Login(userDto.Username, userDto.Password));

        if (authenticatedUser == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(); // TODO add token to return
    }

}