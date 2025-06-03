using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProofedAndPolished.Models;
using Xunit;

public class UsersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostUser_CreatesUser()
    {
        var testUser = new User
        {
            Uid = Guid.NewGuid().ToString(),
            Name = "Test User",
            Email = "test@example.com",
            Role = "admin"
        };

        var response = await _client.PostAsJsonAsync("/api/users", testUser);

        response.EnsureSuccessStatusCode();
        var createdUser = await response.Content.ReadFromJsonAsync<User>();

        createdUser.Should().NotBeNull();
        createdUser!.Id.Should().BeGreaterThan(0);
        createdUser.Name.Should().Be(testUser.Name);
    }

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/users", new User
        {
            Uid = Guid.NewGuid().ToString(),
            Name = "Fetch Test",
            Email = "fetch@example.com",
            Role = "va"
        });
        var newUser = await postResponse.Content.ReadFromJsonAsync<User>();

        var getResponse = await _client.GetAsync($"/api/users/{newUser!.Id}");
        getResponse.EnsureSuccessStatusCode();

        var fetchedUser = await getResponse.Content.ReadFromJsonAsync<User>();
        fetchedUser!.Id.Should().Be(newUser.Id);
        fetchedUser.Name.Should().Be("Fetch Test");
    }

    [Fact]
    public async Task PutUser_UpdatesUser()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/users", new User
        {
            Uid = Guid.NewGuid().ToString(),
            Name = "Old Name",
            Email = "old@example.com",
            Role = "va"
        });
        var user = await postResponse.Content.ReadFromJsonAsync<User>();

        user!.Name = "Updated Name";
        user.Email = "updated@example.com";

        var putResponse = await _client.PutAsJsonAsync($"/api/users/{user.Id}", user);
        putResponse.EnsureSuccessStatusCode();

        var updated = await putResponse.Content.ReadFromJsonAsync<User>();
        updated!.Name.Should().Be("Updated Name");
        updated.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task DeleteUser_RemovesUser()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/users", new User
        {
            Uid = Guid.NewGuid().ToString(),
            Name = "Delete Me",
            Email = "delete@example.com",
            Role = "admin"
        });
        var user = await postResponse.Content.ReadFromJsonAsync<User>();

        var deleteResponse = await _client.DeleteAsync($"/api/users/{user!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var checkResponse = await _client.GetAsync($"/api/users/{user.Id}");
        checkResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}