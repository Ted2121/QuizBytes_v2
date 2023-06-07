using System.ComponentModel.DataAnnotations;

namespace RepositoryIntegrationTests;
public class Configuration
{
    public const string CONNECTION_STRING = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    public const string DATABASE = "quizbytestests";

    public static void ValidateModel<T>(T model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        if (results.Count > 0)
        {
            throw new ValidationException($"model: {model} is not valid");
        }
    }
}
