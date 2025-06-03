using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProofedAndPolished.Models;
using Xunit;

public class ServicesEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ServicesEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetServices_ReturnsList()
    {
        var response = await _client.GetAsync("/api/services");

        response.EnsureSuccessStatusCode();
        var services = await response.Content.ReadFromJsonAsync<List<Service>>();
        services.Should().NotBeNull();
    }

    [Fact]
    public async Task PostService_CreatesService()
    {
        var testService = new Service { Name = "Editing" };

        var response = await _client.PostAsJsonAsync("/api/services", testService);

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Service>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Editing");

        // Cleanup
        await _client.DeleteAsync($"/api/services/{created.Id}");
    }

    [Fact]
    public async Task GetServiceById_ReturnsService()
    {
        var testService = new Service { Name = "Proofreading" };
        var createResponse = await _client.PostAsJsonAsync("/api/services", testService);
        var created = await createResponse.Content.ReadFromJsonAsync<Service>();

        var response = await _client.GetAsync($"/api/services/{created!.Id}");
        response.EnsureSuccessStatusCode();
        var service = await response.Content.ReadFromJsonAsync<Service>();
        service!.Name.Should().Be("Proofreading");

        // Cleanup
        await _client.DeleteAsync($"/api/services/{created.Id}");
    }

    [Fact]
    public async Task PutService_UpdatesService()
    {
        var testService = new Service { Name = "Formatting" };
        var createResponse = await _client.PostAsJsonAsync("/api/services", testService);
        var created = await createResponse.Content.ReadFromJsonAsync<Service>();

        created!.Name = "Layout Design";
        var updateResponse = await _client.PutAsJsonAsync($"/api/services/{created.Id}", created);
        updateResponse.EnsureSuccessStatusCode();

        var updated = await updateResponse.Content.ReadFromJsonAsync<Service>();
        updated!.Name.Should().Be("Layout Design");

        // Cleanup
        await _client.DeleteAsync($"/api/services/{created.Id}");
    }

    [Fact]
public async Task DeleteService_RemovesService()
{
    // Arrange
    var testService = new Service { Name = "ToDelete" };
    var createResponse = await _client.PostAsJsonAsync("/api/services", testService);
    createResponse.EnsureSuccessStatusCode();
    var created = await createResponse.Content.ReadFromJsonAsync<Service>();

    // Act
    var deleteResponse = await _client.DeleteAsync($"/api/services/{created.Id}");

    // Assert delete response
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    // Confirm it’s deleted by trying to get it again
    var checkResponse = await _client.GetAsync($"/api/services/{created.Id}");
    checkResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
}