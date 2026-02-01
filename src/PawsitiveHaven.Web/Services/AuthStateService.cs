using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class AuthStateService
{
    private AuthState _state = new();

    public event Action? OnChange;

    public AuthState State => _state;
    public bool IsAuthenticated => _state.IsAuthenticated;
    public bool IsAdmin => _state.IsAdmin;
    public int? UserId => _state.UserId;
    public string? Username => _state.Username;
    public string? Token => _state.Token;

    public void SetAuthState(string token, int userId, string username, string userLevel)
    {
        _state = new AuthState
        {
            IsAuthenticated = true,
            Token = token,
            UserId = userId,
            Username = username,
            UserLevel = userLevel
        };
        NotifyStateChanged();
    }

    public void SetAuthState(AuthResponse response)
    {
        if (response.Success && response.Token != null)
        {
            SetAuthState(response.Token, response.UserId!.Value, response.Username!, response.UserLevel!);
        }
    }

    public void ClearAuthState()
    {
        _state = new AuthState();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
