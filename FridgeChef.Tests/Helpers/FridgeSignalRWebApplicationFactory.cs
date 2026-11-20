
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace FridgeChef.Tests.Helpers;

public class FridgeSignalRWebApplicationFactory
    : FridgeChefWebApplicationFactory
{
    public Mock<IHubContext<FridgeChefHub>> HubContextMock { get; } = new();

    public Mock<IHubClients> HubClientsMock { get; } = new();

    public Mock<IClientProxy> ClientProxyMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        HubClientsMock
            .Setup(x => x.Groups(It.IsAny<string[]>()))
            .Returns(ClientProxyMock.Object);

        HubContextMock
            .Setup(x => x.Clients)
            .Returns(HubClientsMock.Object);

        ClientProxyMock
            .Setup(x => x.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHubContext<FridgeChefHub>>();

            services.AddSingleton(HubContextMock.Object);
        });
    }
}
