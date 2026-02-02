using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PawsitiveHaven.Web.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, AuthStateService authState, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _authState = authState;
        _logger = logger;
    }

    private void SetAuthHeader()
    {
        if (!string.IsNullOrEmpty(_authState.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            _logger.LogWarning("GET {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GET {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            _logger.LogWarning("POST {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);

            // Try to read error response
            try
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch
            {
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during POST {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            _logger.LogWarning("PUT {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PUT {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<TResponse?> PatchAsync<TResponse>(string endpoint)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PatchAsync(endpoint, null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            _logger.LogWarning("PATCH {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PATCH {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest request)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PatchAsJsonAsync(endpoint, request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            _logger.LogWarning("PATCH {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PATCH {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DELETE {Endpoint}", endpoint);
            return false;
        }
    }
}
