using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists
{
    [Table("artist_social_networks")]
    public class SocialNetwork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("artist_id")]
        public int ArtistId { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("network_id")]
        public string NetworkId { get; set; } = default!;
        [Column("url")]
        public string Url { get; set; } = default!;
        [Column("updated")]
        public DateTime Updated { get; set; }
    }
}
