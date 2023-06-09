﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuizBytes2.Models;

namespace QuizBytes2.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToContainer("Users").HasPartitionKey(e => e.Id);

        modelBuilder.Entity<User>()
        .Property(u => u.ETag)
        .HasConversion(
            etag => $"\"{etag}\"",
            etag => etag.Trim('"'))
        .ToJsonProperty("_etag")
        .IsETagConcurrency();

        modelBuilder.Entity<Question>().ToContainer("Questions").HasPartitionKey(e => e.Chapter)
            .HasNoDiscriminator();

        modelBuilder.Entity<Question>()
        .Property(q => q.CorrectAnswers)
        .HasConversion(
            answers => JsonConvert.SerializeObject(answers),
            json => JsonConvert.DeserializeObject<ICollection<string>>(json)
        )
        .IsRequired();

        modelBuilder.Entity<Question>()
            .Property(q => q.WrongAnswers)
            .HasConversion(
                answers => JsonConvert.SerializeObject(answers),
                json => JsonConvert.DeserializeObject<ICollection<string>>(json)
            )
            .IsRequired();

        

    }
}
