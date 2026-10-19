
namespace FridgeChef.Services;

public class AuthStateService
{
    public bool IsLoading { get; private set; }

    public void SetLoading()
    {
        IsLoading = true;
    }
}
