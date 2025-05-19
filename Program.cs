using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using ProofedAndPolished.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from user-secrets
var connStr = builder.Configuration["volunteer-match-APIDbConnectionString"];
if (string.IsNullOrEmpty(connStr))
    throw new InvalidOperationException(
        "Connection string 'volunteer-match-APIDbConnectionString' not found.");

// Register EF Core with Npgsql
builder.Services.AddDbContext<ProofedAndPolishedDbContext>(opts =>
    opts.UseNpgsql(connStr));

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JSON settings: camel‑case plus cycle‑ignore
builder.Services.Configure<JsonOptions>(opts =>
{
    // produce camelCased JSON keys
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opts.SerializerOptions.DictionaryKeyPolicy   = JsonNamingPolicy.CamelCase;

    // still ignore cycles
    opts.SerializerOptions.ReferenceHandler      = ReferenceHandler.IgnoreCycles;
});

// CORS
builder.Services.AddCors(opts =>
    opts.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:3000")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()));

var app = builder.Build();

app.UseCors("AllowFrontend");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Book Endpoints
app.MapGet("/api/books", async (ProofedAndPolishedDbContext db) =>
    await db.Books.Include(b => b.Genre).Include(b => b.Service).ToListAsync());

app.MapGet("/api/books/{id}", async (int id, ProofedAndPolishedDbContext db) =>
    await db.Books.Include(b => b.Genre).Include(b => b.Service).FirstOrDefaultAsync(b => b.Id == id));

app.MapPost("/api/books", async (Book book, ProofedAndPolishedDbContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
});

app.MapPut("/api/books/{id}", async (int id, Book input, ProofedAndPolishedDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book == null) return Results.NotFound();

    db.Entry(book).CurrentValues.SetValues(input);
    await db.SaveChangesAsync();
    return Results.Ok(book);
});

app.MapDelete("/api/books/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book == null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Genre Endpoints
app.MapGet("/api/genres", async (ProofedAndPolishedDbContext db) => await db.Genres.ToListAsync());
app.MapGet("/api/genres/{id}", async (int id, ProofedAndPolishedDbContext db) => await db.Genres.FindAsync(id));
app.MapPost("/api/genres", async (Genre genre, ProofedAndPolishedDbContext db) =>
{
    db.Genres.Add(genre);
    await db.SaveChangesAsync();
    return Results.Created($"/api/genres/{genre.Id}", genre);
});
app.MapPut("/api/genres/{id}", async (int id, Genre input, ProofedAndPolishedDbContext db) =>
{
    var genre = await db.Genres.FindAsync(id);
    if (genre == null) return Results.NotFound();
    db.Entry(genre).CurrentValues.SetValues(input);
    await db.SaveChangesAsync();
    return Results.Ok(genre);
});
app.MapDelete("/api/genres/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var genre = await db.Genres.FindAsync(id);
    if (genre == null) return Results.NotFound();
    db.Genres.Remove(genre);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Service Endpoints
app.MapGet("/api/services", async (ProofedAndPolishedDbContext db) => await db.Services.ToListAsync());
app.MapGet("/api/services/{id}", async (int id, ProofedAndPolishedDbContext db) => await db.Services.FindAsync(id));
app.MapPost("/api/services", async (Service service, ProofedAndPolishedDbContext db) =>
{
    db.Services.Add(service);
    await db.SaveChangesAsync();
    return Results.Created($"/api/services/{service.Id}", service);
});
app.MapPut("/api/services/{id}", async (int id, Service input, ProofedAndPolishedDbContext db) =>
{
    var service = await db.Services.FindAsync(id);
    if (service == null) return Results.NotFound();
    db.Entry(service).CurrentValues.SetValues(input);
    await db.SaveChangesAsync();
    return Results.Ok(service);
});
app.MapDelete("/api/services/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var service = await db.Services.FindAsync(id);
    if (service == null) return Results.NotFound();
    db.Services.Remove(service);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Favorite Endpoints
app.MapGet("/api/favorites", async (HttpContext http, ProofedAndPolishedDbContext db) =>
{
    var uid = http.User.FindFirst("user_id")?.Value;
    var user = await db.Users.FirstOrDefaultAsync(u => u.Uid == uid);
    if (user == null) return Results.Unauthorized();

    return Results.Ok(await db.Favorites.Where(f => f.UserId == user.Id).Include(f => f.Book).ToListAsync());
});

app.MapPost("/api/favorites", async (Favorite favorite, HttpContext http, ProofedAndPolishedDbContext db) =>
{
    var uid = http.User.FindFirst("user_id")?.Value;
    var user = await db.Users.FirstOrDefaultAsync(u => u.Uid == uid);
    if (user == null) return Results.Unauthorized();

    favorite.UserId = user.Id;
    db.Favorites.Add(favorite);
    await db.SaveChangesAsync();
    return Results.Created($"/api/favorites/{favorite.Id}", favorite);
});

app.MapPut("/api/favorites/{id}", async (int id, Favorite input, HttpContext http, ProofedAndPolishedDbContext db) =>
{
    var favorite = await db.Favorites.FindAsync(id);
    if (favorite == null) return Results.NotFound();

    favorite.Note = input.Note;
    await db.SaveChangesAsync();
    return Results.Ok(favorite);
});

app.MapDelete("/api/favorites/{id}", async (int id, HttpContext http, ProofedAndPolishedDbContext db) =>
{
    var favorite = await db.Favorites.FindAsync(id);
    if (favorite == null) return Results.NotFound();

    db.Favorites.Remove(favorite);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// User Endpoint
app.MapGet("/api/users/me", async (HttpContext http, ProofedAndPolishedDbContext db) =>
{
    var uid = http.User.FindFirst("user_id")?.Value;
    var user = await db.Users.FirstOrDefaultAsync(u => u.Uid == uid);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(user);
});

app.MapPut("/api/users/{id}", async (int id, User input, ProofedAndPolishedDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound();
    db.Entry(user).CurrentValues.SetValues(input);
    await db.SaveChangesAsync();
    return Results.Ok(user);
});

app.MapControllers();
app.Run();