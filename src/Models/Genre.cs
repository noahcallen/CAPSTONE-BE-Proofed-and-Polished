using System.Text.Json.Serialization;

namespace ProofedAndPolished.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<Book> Books { get; set; }
}
