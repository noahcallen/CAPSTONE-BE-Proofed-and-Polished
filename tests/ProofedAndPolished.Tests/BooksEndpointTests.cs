using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using ProofedAndPolished.Models;

public class BooksEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostBook_CreatesAndReturnsBook()
    {
        var genreResponse = await _client.PostAsJsonAsync("/api/genres", new { Name = "Fiction" });
        var genre = JsonSerializer.Deserialize<Genre>(await genreResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var book = new
        {
            Title = "Test Book",
            Author = "Test Author",
            GenreId = genre.Id,
            AmazonLink = "https://example.com",
            FirebaseKey = "book-1",
            Image = "http://example.com/image.jpg",
            PenName = "Pen Test",
            SubGenre = "Mystery",
            Uid = "uid-1"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Book>();

        created.Title.Should().Be(book.Title);
        created.Author.Should().Be(book.Author);
    }

    [Fact]
    public async Task GetBook_ReturnsBookById()
    {
        var post = await _client.PostAsJsonAsync("/api/genres", new { Name = "Horror" });
        var genre = await post.Content.ReadFromJsonAsync<Genre>();

        var book = new
        {
            Title = "Book Title",
            Author = "Author A",
            GenreId = genre.Id,
            AmazonLink = "http://book.com",
            FirebaseKey = "book-get",
            Image = "http://book.com/img.jpg",
            PenName = "Pen A",
            SubGenre = "Sub",
            Uid = "user-a"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);
        var created = await response.Content.ReadFromJsonAsync<Book>();

        var get = await _client.GetAsync($"/api/books/{created.Id}");
        get.EnsureSuccessStatusCode();
        var found = await get.Content.ReadFromJsonAsync<Book>();

        found.Title.Should().Be(book.Title);
    }

    [Fact]
    public async Task PutBook_UpdatesBook()
    {
        var genre = await (await _client.PostAsJsonAsync("/api/genres", new { Name = "Sci-Fi" })).Content.ReadFromJsonAsync<Genre>();

        var book = new
        {
            Title = "Original",
            Author = "A",
            GenreId = genre.Id,
            AmazonLink = "http://orig.com",
            FirebaseKey = "put-key",
            Image = "http://orig.com/img.jpg",
            PenName = "Orig",
            SubGenre = "Fic",
            Uid = "uid-x"
        };

        var created = await (await _client.PostAsJsonAsync("/api/books", book)).Content.ReadFromJsonAsync<Book>();

        var updated = new
        {
            Id = created.Id,
            Title = "Updated",
            Author = "B",
            GenreId = genre.Id,
            AmazonLink = "http://upd.com",
            FirebaseKey = "put-key",
            Image = "http://upd.com/img.jpg",
            PenName = "Upd",
            SubGenre = "Real",
            Uid = "uid-x"
        };

        var put = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);
        put.EnsureSuccessStatusCode();

        var verify = await (await _client.GetAsync($"/api/books/{created.Id}")).Content.ReadFromJsonAsync<Book>();
        verify.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteBook_RemovesBook()
    {
        var genre = await (await _client.PostAsJsonAsync("/api/genres", new { Name = "Delete" })).Content.ReadFromJsonAsync<Genre>();

        var book = new
        {
            Title = "ToDelete",
            Author = "Gone",
            GenreId = genre.Id,
            AmazonLink = "http://del.com",
            FirebaseKey = "del-key",
            Image = "http://del.com/img.jpg",
            PenName = "Del",
            SubGenre = "Gone",
            Uid = "uid-del"
        };

        var created = await (await _client.PostAsJsonAsync("/api/books", book)).Content.ReadFromJsonAsync<Book>();
        var del = await _client.DeleteAsync($"/api/books/{created.Id}");
        del.EnsureSuccessStatusCode();

        var check = await _client.GetAsync($"/api/books/{created.Id}");
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}