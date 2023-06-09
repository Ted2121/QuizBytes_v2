using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Models;
using System.Security.Cryptography;

namespace QuizBytes2.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private IUserRepository _userRepository;
    private IMapper _mapper;

    public AuthorizationController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public async Task<ActionResult<string>> LoginUserAsync(UserCredentialsDto userDto)
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

        var authorizationCode = GenerateAuthorizationCode();

        return Ok(authorizationCode);
    }

    private string GenerateAuthorizationCode()
    {
        // Generate a random authorization code
        var codeLength = 32; // Adjust the length as per your requirements
        var randomBytes = new byte[codeLength];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }

        // Convert the random bytes to a string
        var authorizationCode = Convert.ToBase64String(randomBytes);

        return authorizationCode;
    }
}
