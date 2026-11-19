using System.Diagnostics;

namespace FridgeChef.Api.Observability;

public static class FridgeTelemetry
{
    public static readonly ActivitySource ActivitySource =
        new("FridgeChef.Api.Fridge");
}