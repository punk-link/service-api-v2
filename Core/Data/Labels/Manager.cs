using Core.Data.Common;

namespace Core.Data.Labels
{
    public class Manager : Metadata
    {
        public int Id { get; set; }
        public int LabelId { get; set; }
        public string Name { get; set; } = default!;


        public Label Label { get; set; } = default!;
    }
}
