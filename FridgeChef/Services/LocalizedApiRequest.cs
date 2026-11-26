using System.Globalization;
using System.Net.Http.Headers;

namespace FridgeChef.Services;

public static class LocalizedApiRequest
{
    public static HttpRequestMessage Create(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);

        request.Headers.AcceptLanguage.Add(
            new StringWithQualityHeaderValue(
                CultureInfo.CurrentUICulture.Name));

        return request;
    }
}
