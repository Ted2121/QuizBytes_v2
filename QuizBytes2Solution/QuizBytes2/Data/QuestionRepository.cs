using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;
using System.Linq;
using System.Linq.Expressions;

namespace QuizBytes2.Data;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _appDbContext;

    public QuestionRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<string> CreateQuestionAsync(Question question)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        try
        {
            question.Id = Guid.NewGuid().ToString();

            await _appDbContext.AddAsync(question);

            var saved = await SaveChangesAsync();

            if (saved)
            {
                return question.Id;
            }
            else
            {
                throw new Exception($"Question with id: {question.Id} could not be saved.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Question with id: {question.Id} could not be created. Exception was {ex.Message}");
        }
    }

    public async Task<bool> DeleteQuestionAsync(string id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var question =
            await _appDbContext.Questions.FindAsync(id);

            if (question == null)
            {
                throw new ResourceNotFoundException($"Question with id: {id} not found.");
            }

            _appDbContext.Remove<Question>(question);
            var isDeleted = await SaveChangesAsync();

            return isDeleted;
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not delete question with id: {id}. Exception was: {ex}");
        }
    }

    public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
    {
        try
        {
            return await _appDbContext.Questions.ToListAsync();
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed getting all questions. Exception was: {ex}");
        }
    }

    public async Task<Question> GetQuestionByIdAsync(string id)
    {
        if (id == null)
        {
            throw new ArgumentNullException();
        }

        try
        {
            var question = await _appDbContext.Questions.FindAsync(id);

            if (question == null)
            {
                throw new ResourceNotFoundException($"Question with id: {id} not found.");
            }

            return question;
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed getting question with id: {id}. Exception was: {ex}");

        }
    }

    public async Task<IEnumerable<Question>> GetQuestionsAsync(Expression<Func<Question, bool>> predicate)
    {
        var query = _appDbContext.Questions
  .Where(predicate)
  .AsAsyncEnumerable();

        List<Question> results = new List<Question>();
        await foreach (var question in query)
        {
            results.Add(question);
        }

        return results;
    }

    public async Task<bool> UpdateQuestionAsync(Question question)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        try
        {
            var questionToUpdate = await _appDbContext.FindAsync<Question>(question.Id);

            if (questionToUpdate == null)
            {
                throw new ResourceNotFoundException($"Question with id: {question.Id} not found.");
            }

            questionToUpdate.Text = question.Text;
            questionToUpdate.Hint = question.Hint;
            questionToUpdate.CorrectAnswers = question.CorrectAnswers;
            questionToUpdate.WrongAnswers = question.WrongAnswers;
            questionToUpdate.Subject = question.Subject;
            questionToUpdate.Course = question.Course;
            questionToUpdate.Chapter = question.Chapter;
            questionToUpdate.DifficultyLevel = question.DifficultyLevel;

            return await SaveChangesAsync();
        }
        catch (Exception ex)
        {

            throw new Exception($"Failed updating question with id: {question.Id}. Exception was: {ex}");
        }
    }

    private async Task<bool> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync() >= 0;
    }
}
