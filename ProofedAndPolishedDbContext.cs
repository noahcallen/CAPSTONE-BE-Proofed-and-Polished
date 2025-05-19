using Microsoft.EntityFrameworkCore;
using ProofedAndPolished.Models;
using System;

public class ProofedAndPolishedDbContext : DbContext
{
    public ProofedAndPolishedDbContext(DbContextOptions<ProofedAndPolishedDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User: uid and email must be unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Uid)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Book: firebaseKey must be unique
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.FirebaseKey)
            .IsUnique();

        // Favorite: composite unique key on userId + bookId
        modelBuilder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.BookId })
            .IsUnique();

        // Book FK: User (by UID)
        modelBuilder.Entity<Book>()
            .HasOne<User>()
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.Uid)
            .HasPrincipalKey(u => u.Uid)
            .OnDelete(DeleteBehavior.Cascade);

        // Book FK: Genre
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.SetNull);

        // Book FK: Service
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Service)
            .WithMany(s => s.Books)
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Favorite FKs
        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Book)
            .WithMany(b => b.Favorites)
            .HasForeignKey(f => f.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Seed Data ---

        // Users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Uid = "admin-123", Name = "Admin User", Email = "admin@example.com", Role = "admin" },
            new User { Id = 2, Uid = "va-456", Name = "VA User", Email = "va@example.com", Role = "va" }
        );

        // Services
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Editing" },
            new Service { Id = 2, Name = "Proofreading" }
        );

        // Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Fantasy" },
            new Genre { Id = 2, Name = "Romance" }
        );

        // Books
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                FirebaseKey = "bk-001",
                Uid = "admin-123",
                Author = "Jane Smith",
                PenName = "J.S.",
                Title = "The Enchanted Forest",
                GenreId = 1,
                SubGenre = "Epic",
                ServiceId = 1,
                Image = "http://example.com/image1.jpg",
                AmazonLink = "http://amazon.com/book1",
                Date = DateTime.UtcNow.Date,
                WordCount = 50000,
                Hours = 20,
                Rate = 0.03m,
                HourlyRate = 75.00m,
                InvoicedAmount = 1500.00m,
                WPH = 2500.0m,
                PostedToFacebook = null,
                PostedToWebsite = null
            },
            new Book
            {
                Id = 2,
                FirebaseKey = "bk-002",
                Uid = "va-456",
                Author = "Tom Writer",
                PenName = "T.W.",
                Title = "Love in Shadows",
                GenreId = 2,
                SubGenre = "Contemporary",
                ServiceId = 2,
                Image = "http://example.com/image2.jpg",
                AmazonLink = "http://amazon.com/book2",
                Date = DateTime.UtcNow.Date.AddDays(-5),
                WordCount = 30000,
                Hours = 12,
                Rate = 0.04m,
                HourlyRate = 60.00m,
                InvoicedAmount = 720.00m,
                WPH = 2500.0m,
                PostedToFacebook = DateTime.UtcNow.Date,
                PostedToWebsite = DateTime.UtcNow.Date
            }
        );

        // Favorites
        modelBuilder.Entity<Favorite>().HasData(
            new Favorite
            {
                Id = 1,
                UserId = 2,
                BookId = 1,
                Note = "Want to use this as a formatting reference"
            }
        );
    }
}
