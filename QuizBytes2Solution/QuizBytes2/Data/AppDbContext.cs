using Microsoft.EntityFrameworkCore;
using QuizBytes2.Models;

namespace QuizBytes2.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

   
    public DbSet<Question> Questions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<LastQuizResult> LastQuizResults { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToContainer("Questions");
            entity.HasPartitionKey(q => q.Id);
            entity.Property(q => q.Text).IsRequired();
            entity.Property(q => q.Hint).IsRequired();
            entity.Property(q => q.Subject).IsRequired();
            entity.Property(q => q.Course).IsRequired();
            entity.Property(q => q.Chapter).IsRequired();
            entity.Property(q => q.DifficultyLevel).IsRequired();
            entity.OwnsMany(q => q.CorrectAnswers, answer =>
            {
                answer.Property<string>("Value").IsRequired();
            });

            entity.OwnsMany(q => q.WrongAnswers, answer =>
            {
                answer.Property<string>("Value").IsRequired();
            });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToContainer("Users");
            entity.HasPartitionKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired();
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.TotalPoints);
            entity.Property(u => u.SpendablePoints);

            entity.OwnsOne(u => u.LastQuizResult, quizResult =>
            {
                quizResult.Property(qr => qr.Id);
                quizResult.Property(qr => qr.SubmitTimestamp).IsRequired();
                quizResult.Property(qr => qr.CorrectAnswers);
                quizResult.Property(qr => qr.WrongAnswers);
                quizResult.Property(qr => qr.DifficultyLevel);
            });
        });
    }
}
