using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuizBytes2.Data;
using QuizBytes2.Service;
using QuizBytes2.Service.Extensions;
using Serilog;

namespace QuizBytes2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region Auth
        //builder.Services.AddSingleton(FirebaseApp.Create());

        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });
        #endregion

        #region CORS
        var AllowSpecificOrigins = "_allowSpecificOrigins";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowSpecificOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("http://127.0.0.1:5173");
                                  policy.WithHeaders("Content-Type", "Authorization", "Origin");
                                  policy.AllowAnyMethod();
                              });
        });
        #endregion

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

        #region Logging
        builder.Host.UseSerilog((ctx, lc) => lc
            .ReadFrom.Configuration(ctx.Configuration));
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
        app.UseCors(AllowSpecificOrigins);
        app.ConfigureExceptionHandler(app.Logger);

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
