namespace FridgeChef.Api.Services;

public static class CatalogLocalizationService
{
    public static bool IsEnglish(HttpRequest request)
    {
        var language = request.GetTypedHeaders()
            .AcceptLanguage
            .OrderByDescending(item => item.Quality ?? 1)
            .FirstOrDefault(item =>
                (item.Quality ?? 1) > 0 &&
                (item.Value.ToString().Split('-')[0]
                    .Equals("en", StringComparison.OrdinalIgnoreCase) ||
                 item.Value.ToString().Split('-')[0]
                    .Equals("uk", StringComparison.OrdinalIgnoreCase)));

        return language?.Value.ToString().Split('-')[0]
            .Equals("en", StringComparison.OrdinalIgnoreCase) == true;
    }

    public static string Translate(
        string original,
        string? english,
        bool isEnglish)
    {
        return isEnglish && !string.IsNullOrWhiteSpace(english)
            ? english
            : original;
    }
}
