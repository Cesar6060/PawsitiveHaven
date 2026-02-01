using PawsitiveHaven.Web.Models;

namespace PawsitiveHaven.Web.Services;

public class AuthService
{
    private readonly ApiClient _apiClient;
    private readonly AuthStateService _authState;

    public AuthService(ApiClient apiClient, AuthStateService authState)
    {
        _apiClient = apiClient;
        _authState = authState;
    }

    public async Task<AuthResponse> LoginAsync(string username, string password)
    {
        var request = new LoginRequest(username, password);
        var response = await _apiClient.PostAsync<LoginRequest, AuthResponse>("api/auth/login", request);

        if (response?.Success == true)
        {
            _authState.SetAuthState(response);
        }

        return response ?? new AuthResponse(false, null, null, null, null, "Connection error");
    }

    public async Task<AuthResponse> RegisterAsync(string username, string email, string password)
    {
        var request = new RegisterRequest(username, email, password);
        var response = await _apiClient.PostAsync<RegisterRequest, AuthResponse>("api/auth/register", request);

        if (response?.Success == true)
        {
            _authState.SetAuthState(response);
        }

        return response ?? new AuthResponse(false, null, null, null, null, "Connection error");
    }

    public void Logout()
    {
        _authState.ClearAuthState();
    }
}
