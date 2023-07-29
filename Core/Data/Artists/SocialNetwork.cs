using Core.Data.Common;

namespace Core.Data.Artists;

public class SocialNetwork : Metadata
{
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public string NetworkId { get; set; } = default!;
    public string Url { get; set; } = default!;
}
