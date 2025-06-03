using Xunit;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using ProofedAndPolished.Models;

public class GenresEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GenresEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostGenre_CreatesGenre()
    {
        var genre = new { Name = "Fantasy" };
        var post = await _client.PostAsJsonAsync("/api/genres", genre);
        post.EnsureSuccessStatusCode();

        var created = await post.Content.ReadFromJsonAsync<Genre>();
        created.Name.Should().Be("Fantasy");
    }

    [Fact]
    public async Task GetGenre_ReturnsGenre()
    {
        var genre = new { Name = "Drama" };
        var post = await _client.PostAsJsonAsync("/api/genres", genre);
        var created = await post.Content.ReadFromJsonAsync<Genre>();

        var get = await _client.GetAsync($"/api/genres/{created.Id}");
        get.EnsureSuccessStatusCode();

        var found = await get.Content.ReadFromJsonAsync<Genre>();
        found.Name.Should().Be("Drama");
    }

    [Fact]
    public async Task PutGenre_UpdatesGenre()
    {
        var genre = await (await _client.PostAsJsonAsync("/api/genres", new { Name = "Old Name" })).Content.ReadFromJsonAsync<Genre>();
        var updated = new { Id = genre.Id, Name = "New Name" };

        var put = await _client.PutAsJsonAsync($"/api/genres/{genre.Id}", updated);
        put.EnsureSuccessStatusCode();

        var check = await _client.GetAsync($"/api/genres/{genre.Id}");
        var found = await check.Content.ReadFromJsonAsync<Genre>();
        found.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task DeleteGenre_RemovesGenre()
    {
        var genre = await (await _client.PostAsJsonAsync("/api/genres", new { Name = "ToDelete" })).Content.ReadFromJsonAsync<Genre>();

        var del = await _client.DeleteAsync($"/api/genres/{genre.Id}");
        del.EnsureSuccessStatusCode();

        var check = await _client.GetAsync($"/api/genres/{genre.Id}");
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}