using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebIeltsFree.Models;

[Table("tb_writing_submissions")]
public class WritingSubmission
{
    [Key]
    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("prompt")]
    public string? Prompt { get; set; }

    [Column("essay_text")]
    public string? EssayText { get; set; }

    [Column("band_score")]
    [Range(0, 9)]
    public decimal? BandScore { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("ta_score")]
    [Range(0, 9)]
    public decimal? TaScore { get; set; }

    [Column("cc_score")]
    [Range(0, 9)]
    public decimal? CcScore { get; set; }

    [Column("lr_score")]
    [Range(0, 9)]
    public decimal? LrScore { get; set; }

    [Column("gra_score")]
    [Range(0, 9)]
    public decimal? GraScore { get; set; }

    [Column("ai_feedback")]
    public string? AiFeedback { get; set; }

    [Column("feedback_details")]
    public string? FeedbackDetails { get; set; }

    [Column("task_type")]
    public int? TaskType { get; set; } = 2;

    [Column("word_count")]
    public int? WordCount { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
