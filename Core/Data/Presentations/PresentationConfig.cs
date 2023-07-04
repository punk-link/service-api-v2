using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Presentations;

[Table("artist_presentation_configs")]
public class PresentationConfig
{
    [Column("id")]
    public int Id { get; set; }
    [Column("created")]
    public DateTime Created { get; set; }
    [Column("value")]
    public string Value { get; set; } = default!;
    [Column("updated")]
    public DateTime Updated { get; set; }
}
