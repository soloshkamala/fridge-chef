
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
    
public string? ErrorMessage { get; private set; }

public void SetError(string message)
{
    ErrorMessage = message;
    IsLoading = false;
    OnChange?.Invoke();
}

public string? UserEmail { get; private set; }

public void SetSuccess(string email)
{
    UserEmail = email;
    ErrorMessage = null;
    IsLoading = false;
    OnChange?.Invoke();
}


}
