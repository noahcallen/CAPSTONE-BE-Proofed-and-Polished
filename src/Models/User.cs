namespace ProofedAndPolished.Models;


public class User
{
    public int Id { get; set; }
    public string Uid { get; set; } // Firebase UID
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } // 'admin' or 'va'

    public ICollection<Book> Books { get; set; }
    public ICollection<Favorite> Favorites { get; set; }
}
