using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Problem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public ProblemType Type { get; set; }

    [Required]
    public ProblemStatus Status { get; set; }

    [Required]
    public MethodsEnum Method { get; set; }

    [Required]
    public string Path { get; set; } = null!;

    [Required]
    public int Occurrences { get; set; } = 1;

    [Required]
    public double LastResponseTime { get; set; }

    [Required]
    public int StatusCode { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime LastSeen { get; set; }

    [Required]
    public DateTime? ResolvedAt { get; set; }
}
