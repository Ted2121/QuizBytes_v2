using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizBytes2.Data;
using QuizBytes2.DTOs;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;
using QuizBytes2.Service;
using System.Text.RegularExpressions;

namespace QuizBytes2.Controllers;
[Route("api/v1/[controller]")]
// TODO admin policy to all methods but the getHint method
//[Authorize]
[ApiController]
public class QuestionController : ControllerBase
{
    private IQuestionRepository _questionRepository;
    private IHintHandler _hintHandler;
    private IMapper _mapper;

    public QuestionController(IQuestionRepository questionRepository, IHintHandler hintHandler, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _hintHandler = hintHandler;
        _mapper = mapper;
    }

    [Route("all")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionAdminDto>>> GetAllQuestionsAsync()
    {

        var questions = await _questionRepository.GetAllQuestionsAsync();

        if (questions == null)
        {
            return NotFound("Questions could not be found");
        }

        return Ok(_mapper.Map<QuestionAdminDto>(questions));
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<QuestionAdminDto>> GetQuestionByIdAsync(string id)
    {
        try
        {
            var question = await _questionRepository.GetQuestionByIdAsync(id);

            return Ok(_mapper.Map<QuestionAdminDto>(question));

        }
        catch (ResourceNotFoundException)
        {

            return NotFound($"Question with id: {id} could not be found");
        }
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateQuestionAsync(QuestionAdminDto questionDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var returnedId = await _questionRepository.CreateQuestionAsync(_mapper.Map<Question>(questionDto));

        if (String.IsNullOrEmpty(returnedId))
        {
            return BadRequest("Question could not be created");
        }

        return Ok(returnedId);
    }

    [Route("{id}")]
    [HttpDelete]
    public async Task<ActionResult> DeleteQuestionAsync(string id)
    {
        try
        {
            if (String.IsNullOrEmpty(id))
            {
                return BadRequest("Id cannot be null or empty");
            }

            if (!await _questionRepository.DeleteQuestionAsync(id))
            {
                return BadRequest($"Question with id: {id} could not be deleted");
            }

            return NoContent();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound($"Question with id: {id} could not be found");
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateQuestionAsync(QuestionAdminDto questionDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (!await _questionRepository.UpdateQuestionAsync(_mapper.Map<Question>(questionDto)))
            {
                return BadRequest($"Question with id: {questionDto.Id} could not be updated");
            }

            return NoContent();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound($"Question with id: {questionDto.Id} could not be found");
        }
    }

    [Route("{chapter}/all")]
    [HttpGet]
    public async Task<ActionResult<List<QuestionAdminDto>>> GetAllQuestionByChapterAsync(string chapter)
    {
        if (String.IsNullOrEmpty(chapter))
        {
            return BadRequest("Chapter cannot be null or empty");
        }

        chapter = Regex.Replace(chapter, "[^a-zA-Z0-9_]", "");

        var questions = await _questionRepository.GetQuestionsAsync(q => q.Chapter == chapter);

        if (questions == null)
        {
            return NotFound($"Questions from chapter: {chapter} could not be found");
        }

        return Ok(_mapper.Map<IEnumerable<QuestionAdminDto>>(questions));
    }

    [Route("hint/{id}")]
    [HttpGet]
    public async Task<ActionResult<string>> GetHintByQuestionIdAsync(string id)
    {
        // TODO get user id from claims
        var userId = "";

        if (String.IsNullOrEmpty(userId))
        {
            return Forbid("Invalid user id");
        }

        if (String.IsNullOrEmpty(id))
        {
            return BadRequest("Invalid id");
        }

        try
        {
            var hint = await _hintHandler.GetHintForQuestionById(userId, id);

            if (hint == null)
            {
                return NotFound($"Hint could not be found");
            }

            return Ok(hint);

        }
        catch (UserNotFoundException)
        {
            return NotFound("User could not be found");
        }
        catch (ResourceNotFoundException)
        {
            return NotFound("Not enough points");
        }
    }
}
