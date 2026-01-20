using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace OrderSystem.Web.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken ct = default)
    {
        var client = CreateClientWithAuth();
        var resp = await client.GetAsync(relativeUrl, ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return default;
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest body, CancellationToken ct = default)
    {
        var client = CreateClientWithAuth();
        var payload = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var resp = await client.PostAsync(relativeUrl, content, ct);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
    }

    public async Task PostAsync<TRequest>(string relativeUrl, TRequest body, CancellationToken ct = default)
    {
        var client = CreateClientWithAuth();
        var payload = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var resp = await client.PostAsync(relativeUrl, content, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task PutAsync<TRequest>(string relativeUrl, TRequest body, CancellationToken ct = default)
    {
        var client = CreateClientWithAuth();
        var payload = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var resp = await client.PutAsync(relativeUrl, content, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync<TRequest>(string relativeUrl, TRequest body, CancellationToken ct = default)
    {
        var client = CreateClientWithAuth();
        var payload = JsonSerializer.Serialize(body, JsonOptions);

        using var request = new HttpRequestMessage(HttpMethod.Delete, relativeUrl)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var resp = await client.SendAsync(request, ct);
        resp.EnsureSuccessStatusCode();
    }

    private HttpClient CreateClientWithAuth()
    {
        var client = _httpClientFactory.CreateClient("OrderSystemApi");
        var token = _httpContextAccessor.HttpContext?.Session.GetString(AuthSessionKeys.AccessToken);
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }
}