using camping_rating.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Review
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    [Required]
    public int CampingSpotId { get; set; }

    [ForeignKey("CampingSpotId")]
    public CampingSpot? CampingSpot { get; set; }
}