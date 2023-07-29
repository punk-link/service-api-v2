using Core.Data.Artists;
using Core.Data.Common;

namespace Core.Data.Presentations;

public class PresentationConfig : Metadata
{
    public int Id { get; set; }
    public string Value { get; set; } = default!;

    
    public Artist Artist { get; set; } = default!;
    public int ArtistId { get; set; }
}
