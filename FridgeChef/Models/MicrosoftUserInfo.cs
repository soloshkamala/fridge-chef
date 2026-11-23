namespace FridgeChef.Models;

public sealed class MicrosoftUserInfo
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
