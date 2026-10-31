using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FridgeChef.Services;
using Xunit;

namespace FridgeChef.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WithSuccessfulResponse_ReturnsTrue()
    {
        using var client = CreateClient(HttpStatusCode.OK);
        var service = new AuthService(client);

        var result = await service.LoginAsync(
            "student@example.com",
            "password123"
        );

        Assert.True(result);
    }

    [Fact]
    public async Task LoginAsync_WithUnauthorizedResponse_ReturnsFalse()
    {
        using var client = CreateClient(HttpStatusCode.Unauthorized);
        var service = new AuthService(client);

        var result = await service.LoginAsync(
            "wrong@example.com",
            "wrongpassword"
        );

        Assert.False(result);
    }

    private static HttpClient CreateClient(HttpStatusCode statusCode)
    {
        return new HttpClient(new FakeHttpMessageHandler(statusCode))
        {
            BaseAddress = new Uri("http://localhost:5039/")
        };
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        public FakeHttpMessageHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new HttpResponseMessage(_statusCode)
            );
        }
    }
}
