using System.Data.SqlClient;
using Labs_5.DTOs;
using Labs_5.Validators;

namespace Labs_5.Endpoints;

public static class AnimalsEndpoints
{
    public static void RegisterAnimalsEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("animals", (IConfiguration configuration, string orderBy = "name") =>
        {
            var availableOrderByValues = new List<string> { "name", "description", "category", "area" };
            if (!availableOrderByValues.Contains(orderBy))
            {
                return Results.BadRequest();    
            }
            var animals = new List<GetAnimalResponse>();
            using var slqConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy}", slqConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                animals.Add(new GetAnimalResponse(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                ));
            }
            return Results.Ok(animals);
        });
        webApplication.MapPost("animals", (IConfiguration configuration, CreateAnimalRequest createAnimalRequest) =>
        {
            var animalRequestValidation = new CreateAnimalRequestValidator().Validate(createAnimalRequest);
            if (!animalRequestValidation.IsValid)
            {
                return Results.ValidationProblem(animalRequestValidation.ToDictionary());
            }
            using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand("INSERT INTO Animal(Name, Description, Category, Area) VALUES (@name, @description, @category, @area)", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@name", createAnimalRequest.Name);
            sqlCommand.Parameters.AddWithValue("@description", createAnimalRequest.Description == null ? DBNull.Value : createAnimalRequest.Description);
            sqlCommand.Parameters.AddWithValue("@category", createAnimalRequest.Category);
            sqlCommand.Parameters.AddWithValue("@area", createAnimalRequest.Area);
            sqlCommand.Connection.Open();
            sqlCommand.ExecuteNonQuery();
            return Results.Created();
        });
        webApplication.MapPut("animals/{id:int}", (IConfiguration configuration, CreateAnimalRequest createAnimalRequest, int id) =>
        {
            var animalRequestValidation = new CreateAnimalRequestValidator().Validate(createAnimalRequest);
            if (!animalRequestValidation.IsValid)
            {
                return Results.ValidationProblem(animalRequestValidation.ToDictionary());
            }
            using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand($"SELECT * FROM Animal Where IdAnimal = {id}", sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            if (!reader.Read())
            { 
                return Results.NotFound();
            }
            reader.Close();
            sqlCommand = new SqlCommand($"UPDATE Animal SET Name = @name, Description = @description, Category = @category, Area = @area WHERE IdAnimal = {id}", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@name", createAnimalRequest.Name);
            sqlCommand.Parameters.AddWithValue("@description", createAnimalRequest.Description == null ? DBNull.Value : createAnimalRequest.Description);
            sqlCommand.Parameters.AddWithValue("@category", createAnimalRequest.Category);
            sqlCommand.Parameters.AddWithValue("@area", createAnimalRequest.Area);
            sqlCommand.ExecuteNonQuery();
            return Results.Ok();
        });
        webApplication.MapDelete("animals/{id:int}", (IConfiguration configuration, int id) =>
        {
            using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand($"SELECT * FROM Animal WHERE IdAnimal = {id}", sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            if (!reader.Read())
            {
                return Results.NotFound();
            }
            reader.Close();
            sqlCommand = new SqlCommand($"DELETE FROM Animal WHERE IdAnimal = {id}", sqlConnection);
            sqlCommand.ExecuteNonQuery();
            return Results.Ok();
        });
    }
}