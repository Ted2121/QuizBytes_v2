using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Service;

namespace QuizBytes2.Controllers;
[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class QuizController : ControllerBase
{
    private IUserRepository _userRepository;
    private IQuizGenerator _quizGenerator;
    private IQuizResultHandler _quizResultHandler;
    private IMapper _mapper;
    public QuizController(
        IUserRepository userRepository,
        IQuizGenerator quizGenerator,
        IQuizResultHandler quizResultVerifier,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _quizGenerator = quizGenerator;
        _quizResultHandler = quizResultVerifier;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<QuizDto>> GetQuizAsync([FromQuery] string chapter, [FromQuery] int difficulty, [FromQuery] int count)
    {
        try
        {
            var quiz = await _quizGenerator.CreateQuizAsync(chapter, difficulty, count);

            return Ok(quiz);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound($"No questions found for chapter: {chapter} with difficulty level: {difficulty}");
        }
    }

    [HttpPost]
    public async Task<ActionResult> SubmitQuizAsync(QuizSubmitDto quizSubmitDto)
    {
        // Should always be first line for time accuracy
        var serverTime = DateTime.UtcNow;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // TODO get id from claim
        var userId = "";

        if (String.IsNullOrEmpty(userId))
        {
            return Forbid("Invalid user id in claim");
        }

        try
        {
            // The reason for this time validation is so that a user doesn't run a script to cheat by submitting a quiz multiple times
            var isValid = await _quizResultHandler.ValidateSubmitTimeAsync(userId, quizSubmitDto, serverTime);

            if (!isValid)
            {
                return BadRequest("Invalid quiz");
            }

            var isSubmitted = await _quizResultHandler.SubmitQuizAsync(userId, quizSubmitDto, serverTime);

            if (!isSubmitted)
            {
                return BadRequest("Quiz could not be submitted");
            }

        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }
        catch (ResourceNotFoundException)
        {
            return NotFound("Quiz could not be found");
        }

        return Ok();
    }

    [Route("result")]
    [HttpGet]
    public async Task<ActionResult<QuizResultDto>> GetQuizResult()
    {
        // TODO user id from claim
        var userId = "";

        if (String.IsNullOrEmpty(userId))
        {
            return Forbid("Invalid user id in claim");
        }

        try
        {
            var quizResult = await _userRepository.GetLastQuizByUserIdAsync(userId);

            var quizResultDto = _mapper.Map<QuizResultDto>(quizResult);

            return Ok(quizResultDto);
        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }
        catch (ResourceNotFoundException)
        {
            return NotFound("Quiz result could not be found");
        }
    }
}
