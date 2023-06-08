using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;

namespace QuizBytes2.Controllers;
[Route("api/v1/[controller]")]
//TODO authorization
[ApiController]
public class UserController : ControllerBase
{
    private IUserRepository _userRepository;
    private IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
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

        return Ok(); // TODO add token to return
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<int>> CreateUserAsync(UserCredentialsDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userModel = _mapper.Map<User>(userDto);

        try
        {
            var returnedId = await _userRepository.CreateUserAsync(userModel);

            if (String.IsNullOrEmpty(returnedId))
            {
                return BadRequest("User could not be created");
            }

            return Ok(returnedId);
        }
        catch (UserAlreadyExistsException)
        {
            return BadRequest("Username already exists");
        }
    }

    // TODO admin policy
    [Route("{id}")]
    [HttpDelete]
    public async Task<ActionResult> DeleteUserAsync(string id)
    {
        try
        {
            if (!await _userRepository.DeleteUserAsync(id))
            {
                return BadRequest("User could not be deleted");
            }

        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUserAsync()
    {
        //TODO get id from claim
        var id = "";

        if (String.IsNullOrEmpty(id))
        {
            return Forbid("Invalid id in claim");
        }

        try
        {
            if (!await _userRepository.DeleteUserAsync(id))
            {
                return BadRequest("User could not be deleted");
            }

        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }

        return NoContent();
    }

    [Route("{username}")]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username cannot be null or empty");
        }

        var user = await _userRepository.GetUsersAsync(u => u.Username == username);

        if (user == null)
        {
            return NotFound($"User with username: {username} could not be found");
        }

        return Ok(_mapper.Map<UserDto>(user));
    }

    [Route("id")]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUserByIdFromClaimAsync()
    {
        // TODO get id from claim
        var id = "";

        if (String.IsNullOrEmpty(id))
        {
            return Forbid("Invalid id in claim");
        }

        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            return Ok(_mapper.Map<UserDto>(user));

        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }
    }

    // TODO admin policy
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(string id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            return Ok(_mapper.Map<UserDto>(user));

        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }
    }

    [Route("password")]
    [HttpGet]
    public async Task<ActionResult> UpdatePasswordAsync(UserCredentialsDto userDto)
    {
        // TODO get id from claim
        var id = "";

        if (String.IsNullOrEmpty(id))
        {
            return Forbid("Invalid user id in claim");
        }

        try
        {
            var userWithId = await _userRepository.GetUserByIdAsync(id);

            if (userWithId.Username != userDto.Username)
            {
                return Forbid("Username does not match");
            }

            var isUpdated = await _userRepository.UpdatePasswordAsync(userDto.Username, userDto.Password, userDto.NewPassword);

            if (!isUpdated)
            {
                return BadRequest("Password could not be updated");
            }

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }
    }
}