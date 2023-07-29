using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists;

public class Genre
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name {get; set; } = default!;

    public List<Artist> Artists { get; set; } = default!;
}
