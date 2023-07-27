using Core.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists;

public class SocialNetwork : Metadata
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public string NetworkId { get; set; } = default!;
    public string Url { get; set; } = default!;
}
