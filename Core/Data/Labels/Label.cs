using Core.Data.Common;

namespace Core.Data.Labels;

public class Label : Metadata
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;


    public List<Manager> Managers { get; set; } = default!;
}
