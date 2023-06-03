using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Service;

namespace QuizBytes2.Controllers;
[Route("api/v2/[controller]")]
[ApiController]
public class QuizController : ControllerBase
{
    private IUserRepository _userRepository;
    private QuizGenerator _quizGenerator;
    private QuizResultHandler _quizResultHandler;
    private IMapper _mapper;
    public QuizController(
        IUserRepository userRepository, 
        QuizGenerator quizGenerator, 
        QuizResultHandler quizResultVerifier, 
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
        var serverTime = DateTime.UtcNow;
        var isValid = await _quizResultHandler.ValidateQuizAsync(quizSubmitDto, serverTime);

        if (!isValid)
        {
            return BadRequest("Invalid quiz");
        }

        var isSubmitted = await _quizResultHandler.SubmitQuizAsync(quizSubmitDto, serverTime);

        if (!isSubmitted)
        {
            return BadRequest("Quiz could not be submitted");
        }

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<QuizResultDto>> GetQuizResult()
    {
        // TODO user id from claim
        var userId = "";

        try
        {
            var quizResult = await _userRepository.GetLastQuizByUserIdAsync(userId);

            if (quizResult == null)
            {
                return NotFound("A quiz result could not be found");
            }
        }
        catch (NotFoundException)
        {

            return NotFound("User could not be found");
        }

        var quizResultDto = 

        return Ok(QuizResultDto)


    }
}
