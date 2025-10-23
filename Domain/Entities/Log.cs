using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Log
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Method { get; set; } = null!;

    [Required]
    public int StatusCode { get; set; }

    [Required]
    public string Path { get; set; } = null!;

    [Required]
    public double ResponseTime { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
