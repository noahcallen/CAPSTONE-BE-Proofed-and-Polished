using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProofedAndPolished.Models;

var builder = WebApplication.CreateBuilder(args);

// Enable OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow passing DateTimes without timezone data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Connect API to PostgreSQL Database
builder.Services.AddDbContext<ProofedAndPolishedDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ProofedAndPolishedDbConnectionString"]));

// Set JSON serialization options (Prevents circular JSON errors)
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//  CORS Policy (Allow frontend at `localhost:3000`)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// ✅ Enable CORS Middleware BEFORE routing
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ==============
// CALLS
// ==============

// BOOK ENDPOINTS
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

// GENRE ENDPOINTS
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

// SERVICE ENDPOINTS
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

// FAVORITE ENDPOINTS
app.MapGet("/api/favorites", async (ProofedAndPolishedDbContext db) =>
    await db.Favorites.Include(f => f.Book).ToListAsync());

app.MapPost("/api/favorites", async (Favorite favorite, ProofedAndPolishedDbContext db) =>
{
    db.Favorites.Add(favorite);
    await db.SaveChangesAsync();
    return Results.Created($"/api/favorites/{favorite.Id}", favorite);
});

app.MapPut("/api/favorites/{id}", async (int id, Favorite input, ProofedAndPolishedDbContext db) =>
{
    var favorite = await db.Favorites.FindAsync(id);
    if (favorite == null) return Results.NotFound();

    favorite.Note = input.Note;
    await db.SaveChangesAsync();
    return Results.Ok(favorite);
});

app.MapDelete("/api/favorites/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var favorite = await db.Favorites.FindAsync(id);
    if (favorite == null) return Results.NotFound();

    db.Favorites.Remove(favorite);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// USER ENDPOINTS
app.MapGet("/api/users", async (ProofedAndPolishedDbContext db) => await db.Users.ToListAsync());
app.MapGet("/api/users/{id}", async (int id, ProofedAndPolishedDbContext db) => await db.Users.FindAsync(id));
app.MapPost("/api/users", async (User user, ProofedAndPolishedDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", user);
});
app.MapPut("/api/users/{id}", async (int id, User input, ProofedAndPolishedDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound();
    db.Entry(user).CurrentValues.SetValues(input);
    await db.SaveChangesAsync();
    return Results.Ok(user);
});
app.MapDelete("/api/users/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound();
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
