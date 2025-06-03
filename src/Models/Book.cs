namespace ProofedAndPolished.Models;

public class Book
{
    public int Id { get; set; }
    public string FirebaseKey { get; set; }
    public string Uid { get; set; } // FK to User.Uid
    public string Author { get; set; }
    public string PenName { get; set; }
    public string Title { get; set; }
    public int? GenreId { get; set; }
    public string SubGenre { get; set; }
    public int? ServiceId { get; set; }
    public string Image { get; set; }
    public string AmazonLink { get; set; }
    public DateTime? Date { get; set; }

    public int? WordCount { get; set; }
    public decimal? Hours { get; set; }
    public decimal? Rate { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? InvoicedAmount { get; set; }
    public decimal? WPH { get; set; }

    public DateTime? PostedToFacebook { get; set; }
    public DateTime? PostedToWebsite { get; set; }

    public Genre Genre { get; set; }
    public Service Service { get; set; }
    public ICollection<Favorite> Favorites { get; set; }
}
