namespace FridgeChef.Models;

public sealed record Course(int Id, string Code, string Title, int Credits, int TeacherId);
