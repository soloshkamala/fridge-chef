
using Microsoft.JSInterop;

namespace FridgeChef.Services;

public class AuthStateService
{
    private readonly IJSRuntime js;

    public AuthStateService(IJSRuntime js)
    {
        this.js = js;
    }

    public bool IsLoading { get; private set; }

    public bool IsInitialized { get; private set; }

    public string? ErrorMessage { get; private set; }

    public string? UserEmail { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(UserEmail);

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        UserEmail = await js.InvokeAsync<string?>("fridgeAuth.get");

        IsInitialized = true;
        OnChange?.Invoke();
    }

    public void SetLoading()
    {
        IsLoading = true;
        ErrorMessage = null;
        OnChange?.Invoke();
    }

    public void StopLoading()
    {
        IsLoading = false;
        OnChange?.Invoke();
    }

    public void SetError(string message)
    {
        ErrorMessage = message;
        IsLoading = false;
        OnChange?.Invoke();
    }

    public void SetSuccess(string email)
    {
        UserEmail = email;
        ErrorMessage = null;
        IsLoading = false;
        OnChange?.Invoke();
    }

    public async Task SaveLoginAsync(string email)
    {
        SetSuccess(email);
        await js.InvokeVoidAsync("fridgeAuth.save", email);
    }

    public async Task LogoutAsync()
    {
        UserEmail = null;
        ErrorMessage = null;
        IsLoading = false;

        await js.InvokeVoidAsync("fridgeAuth.clear");
        OnChange?.Invoke();
    }
}
