using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace camping_rating.Models
{
    public class CampingSpot
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        public byte[]? Photo { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
