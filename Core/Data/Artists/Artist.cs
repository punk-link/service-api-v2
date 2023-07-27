using Core.Data.Common;
using Core.Data.Releases;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists;

[Index(nameof(SpotifyId), IsUnique = true)]
[Index(nameof(LabelId), IsUnique = true)]
public class Artist : Metadata
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column(TypeName = "jsonb")]
    public List<ImageDetails> ImageDetails { get; set; } = default!;
    public int LabelId { get; set; }
    public string Name { get; set; } = default!;
    public string SpotifyId { get; set; } = default!;

    public List<Release> Releases { get; set; } = default!;
}
