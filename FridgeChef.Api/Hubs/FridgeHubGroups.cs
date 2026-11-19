namespace FridgeChef.Api.Hubs;

public static class FridgeHubGroups
{
    public static string ForItem(int userId, int itemId)
    {
        return $"fridge-item-{userId}-{itemId}";
    }

    public static string ForUser(int userId)
    {
        return $"fridge-user-{userId}";
    }
}