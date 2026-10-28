namespace FridgeChef.Api.Features.Courses;

public sealed record Course(int Id, string Code, string Title, int Credits, int TeacherId);
