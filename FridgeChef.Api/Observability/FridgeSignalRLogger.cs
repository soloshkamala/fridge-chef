using Microsoft.Extensions.Logging;

namespace FridgeChef.Api.Observability;

public static partial class FridgeSignalRLogger
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Fridge item {ItemId} changed for user {UserId}")]
    public static partial void FridgeItemChanged(
    ILogger logger,
    int itemId,
    int userId);
}