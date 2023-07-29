using Core.Data.Artists;
using Core.Data.Common;

namespace Core.Data.Releases;

public class Track : Metadata
{
    public int Id { get; set; }
    public int DiskNumber { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsExplicit { get; set; }
    public string Name { get; set; } = default!;
    public string SpotifyId { get; set; } = default!;
    public int TrackNumber { get; set; }

    public List<Artist> Artists { get; set; } = default!;

    public int ReleaseId { get; set; }
    public Release Release { get; set; } = default!;
}
