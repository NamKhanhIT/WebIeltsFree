using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebIeltsFree.Models;

[Table("tb_grade_disputes")]
public class GradeDispute
{
    [Key]
    [Column("dispute_id")]
    public int DisputeId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("submission_type")]
    [Required]
    public string SubmissionType { get; set; } = string.Empty;

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Column("reason")]
    [Required]
    public string Reason { get; set; } = string.Empty;

    [Column("status")]
    [Required]
    public string Status { get; set; } = "pending";

    [Column("reviewed_by")]
    public int? ReviewedBy { get; set; }

    [Column("original_score")]
    public decimal OriginalScore { get; set; }

    [Column("revised_score")]
    public decimal? RevisedScore { get; set; }

    [Column("teacher_notes")]
    public string? TeacherNotes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    [ForeignKey("ReviewedBy")]
    public virtual User? Reviewer { get; set; }

    [NotMapped]
    public WritingSubmission? WritingSubmission { get; set; }
    [NotMapped]
    public SpeakingSession? SpeakingSession { get; set; }
}
