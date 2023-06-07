
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using QuizBytes2.Data;
using QuizBytes2.Service;

namespace QuizBytes2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region Data DI
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Credentials are identical for all CosmosDB Emulator users so no reason to hide them in an env variable
        builder.Services.AddDbContext<AppDbContext>(options => options.UseCosmos(
            "https://localhost:8081",
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            "quizbytes"
            ));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
        #endregion

        #region Service Layer DI
        builder.Services.AddScoped<IQuizGenerator, QuizGenerator>();
        builder.Services.AddScoped<IQuizResultHandler, QuizResultHandler>();
        builder.Services.AddScoped<IQuizPointCalculator, QuizPointCalculator>();
        #endregion

        #region Caching
        builder.Services.AddSingleton<IMemoryCache>(provider =>
        {
            var questionCache = new MemoryCache(new MemoryCacheOptions());

            return questionCache;
        });

        //builder.Services.AddDistributedMemoryCache();
        //builder.Services.AddSingleton<IDistributedCache>(provider =>
        //{
        //    var questionCache = provider.GetService<IDistributedCache>();
        //    return questionCache;
        //});
        #endregion

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //TODO this is only for local database --create database and containers manually in production to ensure service is set up correctly
        #region local database creation
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        #endregion

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
