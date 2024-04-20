namespace Labs_5.DTOs;

public record GetAnimalResponse(int IdAnimal, string Name, string? Description, string Category, string Area);
public record CreateAnimalRequest(string Name, string? Description, string Category, string Area);