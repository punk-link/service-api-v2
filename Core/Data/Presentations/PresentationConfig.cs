using Core.Data.Common;

namespace Core.Data.Presentations;

public class PresentationConfig : Metadata
{
    public int Id { get; set; }
    public string Value { get; set; } = default!;
}
