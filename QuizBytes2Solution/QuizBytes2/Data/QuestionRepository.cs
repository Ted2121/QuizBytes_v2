using Microsoft.EntityFrameworkCore;
using QuizBytes2.Exceptions;
using QuizBytes2.Models;
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
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            var question =
            await _appDbContext.Questions.FirstOrDefaultAsync(q => q.Id == id);

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

    public async Task<string> GetHintForQuestionByIdAsync(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException();
        }

        try
        {
            var hint = await _appDbContext.Questions
                    .AsNoTracking()
                    .Where(q => q.Id.Equals(id))
                    .Select(q => q.Hint)
                    .FirstOrDefaultAsync();

            if (String.IsNullOrEmpty(hint))
            {
                return null;
            }

            return hint;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed getting hint for question with id: {id}. Exception was: {ex}");
        }
    }

    public async Task<Question> GetQuestionByIdAsync(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException();
        }

        try
        {
            var question = await _appDbContext.Questions.FirstOrDefaultAsync(q => q.Id == id);


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

    public async Task<List<Question>> GetRandomQuestionsFromChapterAsync(string chapter, int difficulty, int count)
    {
        var questions = await _appDbContext.Questions
        .Where(q => q.Chapter == chapter && q.DifficultyLevel == difficulty).ToListAsync();

        if (!questions.Any())
        {
            throw new ResourceNotFoundException($"No questions found in chapter: {chapter}");
        }

        var randomQuestions = questions.OrderBy(q => Guid.NewGuid()).Take(count).ToList();

        return randomQuestions;
    }

    public async Task<bool> UpdateQuestionAsync(Question question)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        try
        {
            var questionToUpdate = await _appDbContext.Questions.FirstOrDefaultAsync(q => q.Id == question.Id);


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
