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

// Connect API to either InMemory or PostgreSQL based on environment
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ProofedAndPolishedDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<ProofedAndPolishedDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

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
{
    var book = await db.Books.FindAsync(id);
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

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

app.MapGet("/api/genres/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var genre = await db.Genres.FindAsync(id);
    return genre is not null ? Results.Ok(genre) : Results.NotFound();
});

app.MapPost("/api/genres", async (Genre genre, ProofedAndPolishedDbContext db) =>
{
    try
    {
        db.Genres.Add(genre);
        await db.SaveChangesAsync();
        return Results.Created($"/api/genres/{genre.Id}", genre);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"GENRE ERROR: {ex.Message}");
        return Results.Problem("Genre creation failed");
    }
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

app.MapGet("/api/services/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var service = await db.Services.FindAsync(id);
    return service == null ? Results.NotFound() : Results.Ok(service);
});

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

app.MapGet("/api/users/{id}", async (int id, ProofedAndPolishedDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/api/users", async (ProofedAndPolishedDbContext db, User user) =>
{
    Console.WriteLine($"[POST] User Received: {user?.Name}, {user?.Email}, {user?.Uid}, {user?.Role}");

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", user);
});
app.MapPut("/api/users/{id}", async (int id, User input, ProofedAndPolishedDbContext db) =>
{
    Console.WriteLine($"[PUT] User Update: {id} -> {input?.Name}");

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

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Unhandled exception:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
        throw;
    }
});


app.Run();
