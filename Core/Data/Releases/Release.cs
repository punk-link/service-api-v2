using Core.Data.Artists;
using Core.Data.Common;
using Core.Models.Releases.Enums;

namespace Core.Data.Releases;

public class Release
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; } = default!;
    public List<ImageDetails> ImageDetails { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateOnly ReleaseDate { get; set; }
    public string SpotifyId { get; set; } = default!;
    public int TrackNumber { get; set; }
    public ReleaseType Type { get; set; }
    public string Upc { get; set; } = default!;
    public DateTime Updated { get; set; }


    public List<Artist> ReleaseArtists { get; set; } = default!;
    public List<Track> Tracks { get; set; } = default!;
}
