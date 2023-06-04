using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Service;

namespace QuizBytes2.Controllers;
[Route("api/v1/[controller]")]
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
    public async Task<ActionResult<QuizDto>> GetQuizAsync([FromQuery] string subject, [FromQuery] string course, [FromQuery] string chapter, [FromQuery] int difficulty)
    {
        // TODO --incomplete
        var quiz = await _quizGenerator.GetQuizAsync(subject, course, chapter, difficulty);

        return Ok(quiz);
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
