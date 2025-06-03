using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ProofedAndPolished.Models;
using Xunit;
using ProofedAndPolished;

public class FavoritesEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FavoritesEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Genre?> CreateGenre()
    {
        var genre = new { Name = "FavoriteGenre", Uid = "test-uid" };
        var response = await _client.PostAsJsonAsync("/api/genres", genre);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"\n❌ Failed to POST /api/genres\nStatus: {response.StatusCode}\nBody: {error}\n");
        }

        return await response.Content.ReadFromJsonAsync<Genre>();
    }

    private async Task<Book?> CreateBook(int genreId)
    {
        var book = new
        {
            Title = "FavoriteBook",
            Author = "Author",
            GenreId = genreId,
            AmazonLink = "http://amazon.com",
            FirebaseKey = "firebase-key-fav",
            Image = "http://image.com",
            PenName = "Pen",
            SubGenre = "Sub",
            Uid = "user-123"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);
        return await response.Content.ReadFromJsonAsync<Book>();
    }

    private async Task<User?> CreateUser()
{
    var user = new { Uid = "user-123", Name = "Tester", Email = "tester@example.com", Role = "admin" };
    var response = await _client.PostAsJsonAsync("/api/users", user);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();
        throw new Exception($"\n❌ Failed to POST /api/users\nStatus: {response.StatusCode}\nBody: {error}\n");
    }

    return await response.Content.ReadFromJsonAsync<User>();
}


    [Fact]
    public async Task PostFavorite_CreatesFavorite()
    {
        var genre = await CreateGenre();
        var book = await CreateBook(genre!.Id);
        var user = await CreateUser();

        var favorite = new
        {
            BookId = book!.Id,
            UserId = user!.Id,
            Note = "Love this book"
        };

        var postResponse = await _client.PostAsJsonAsync("/api/favorites", favorite);
        postResponse.EnsureSuccessStatusCode();

        var created = await postResponse.Content.ReadFromJsonAsync<Favorite>();
        Assert.Equal("Love this book", created!.Note);
    }

    [Fact]
    public async Task PutFavorite_UpdatesNote()
    {
        var genre = await CreateGenre();
        var book = await CreateBook(genre!.Id);
        var user = await CreateUser();

        var favorite = new { BookId = book!.Id, UserId = user!.Id, Note = "Initial" };
        var postResponse = await _client.PostAsJsonAsync("/api/favorites", favorite);
        var created = await postResponse.Content.ReadFromJsonAsync<Favorite>();

        var update = new { BookId = book.Id, UserId = user.Id, Note = "Updated Note" };
        var putResponse = await _client.PutAsJsonAsync($"/api/favorites/{created!.Id}", update);
        putResponse.EnsureSuccessStatusCode();

        var updated = await putResponse.Content.ReadFromJsonAsync<Favorite>();
        Assert.Equal("Updated Note", updated!.Note);
    }

    [Fact]
    public async Task DeleteFavorite_RemovesFavorite()
    {
        var genre = await CreateGenre();
        var book = await CreateBook(genre!.Id);
        var user = await CreateUser();

        var favorite = new { BookId = book!.Id, UserId = user!.Id, Note = "To delete" };
        var postResponse = await _client.PostAsJsonAsync("/api/favorites", favorite);
        var created = await postResponse.Content.ReadFromJsonAsync<Favorite>();

        var deleteResponse = await _client.DeleteAsync($"/api/favorites/{created!.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetFromJsonAsync<List<Favorite>>("/api/favorites");
        Assert.DoesNotContain(getResponse!, f => f.Id == created.Id);
    }
}