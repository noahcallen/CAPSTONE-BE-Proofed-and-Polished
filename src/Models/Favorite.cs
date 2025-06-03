namespace ProofedAndPolished.Models;


public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public string Note { get; set; }

    public User User { get; set; }
    public Book Book { get; set; }
}
