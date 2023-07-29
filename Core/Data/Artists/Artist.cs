using Core.Data.Common;
using Core.Data.Presentations;
using Core.Data.Releases;

namespace Core.Data.Artists;

public class Artist : Metadata
{
    public int Id { get; set; }
    public List<ImageDetails> ImageDetails { get; set; } = default!;
    public int LabelId { get; set; }
    public string Name { get; set; } = default!;
    public string SpotifyId { get; set; } = default!;

    public PresentationConfig? PresentationConfig { get; set; }
    public List<Release> Releases { get; set; } = default!;
}
