
namespace FridgeChef.Services;

public class AuthStateService
{
    public bool IsLoading { get; private set; }

    public event Action? OnChange;

    public void SetLoading()
    {
        IsLoading = true;
        OnChange?.Invoke();
    }

    public void StopLoading()
    {
        IsLoading = false;
        OnChange?.Invoke();
    }
}
