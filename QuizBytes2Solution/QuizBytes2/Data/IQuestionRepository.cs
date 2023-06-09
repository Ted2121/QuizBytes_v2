﻿using QuizBytes2.Models;
using System.Linq.Expressions;

namespace QuizBytes2.Data;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAllQuestionsAsync();
    Task<Question> GetQuestionByIdAsync(string id);
    Task<IEnumerable<Question>> GetQuestionsAsync(Expression<Func<Question, bool>> predicate);
    Task<List<Question>> GetRandomQuestionsFromChapterAsync(string chapter, int difficulty, int count);
    Task<string> GetHintForQuestionByIdAsync(string id);
    Task<string> CreateQuestionAsync(Question question);
    Task<bool> UpdateQuestionAsync(Question question);
    Task<bool> DeleteQuestionAsync(string id);
}
