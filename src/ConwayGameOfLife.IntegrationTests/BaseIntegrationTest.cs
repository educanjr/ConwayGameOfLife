using ConwayGameOfLife.Data;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace ConwayGameOfLife.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    protected const string JsonHttpContentType = "application/json";

    private readonly IServiceScope _scope;
    protected readonly HttpClient? Client;
    protected readonly ConwayDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ConwayDbContext>();
        Client = factory.CreateClient();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }

    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await Client!.GetAsync(url);
    }

    protected async Task<HttpResponseMessage> PatchAsync(string url)
    {
        return await Client!.PatchAsync(url, default);
    }

    protected async Task<HttpResponseMessage> PostAsync<TRequest>(string url, TRequest? data)
    {
        var jsonData = data is null ? string.Empty : JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonData, Encoding.UTF8, JsonHttpContentType);
        return await Client!.PostAsync(url, content);
    }

    protected static async Task<T> ParseResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"Unexpected status code from HTTP request: {response.StatusCode}. Response: {responseString}");
        }

        var contentStream = await response.Content.ReadAsStringAsync();

        var responseData = JsonConvert.DeserializeObject<T>(contentStream) ??
            throw new InvalidDataException("Json Serializer Error");

        return responseData;
    }
}