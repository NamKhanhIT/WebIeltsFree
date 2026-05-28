using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebIeltsFree.Models;

/// <summary>
/// Speaking practice session with AI evaluation
/// </summary>
[Table("tb_speaking_sessions")]
public class SpeakingSession
{
    [Key]
    [Column("session_id")]
    public int SessionId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("topic")]
    public string? Topic { get; set; }

    [Column("fluency_score")]
    [Range(0, 9)]
    public float? FluencyScore { get; set; }

    [Column("pronunciation_score")]
    [Range(0, 9)]
    public float? PronunciationScore { get; set; }

    [Column("grammar_score")]
    [Range(0, 9)]
    public float? GrammarScore { get; set; }

    [NotMapped]
    public float? OverallBand 
    {
        get
        {
            if (FluencyScore == null || PronunciationScore == null || GrammarScore == null) return null;
            return (float)Math.Round((FluencyScore.Value + PronunciationScore.Value + GrammarScore.Value) / 3f * 2f, MidpointRounding.AwayFromZero) / 2f;
        }
    }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("audio_url")]
    public string? AudioUrl { get; set; }

    [Column("transcript")]
    public string? Transcript { get; set; }

    [Column("ai_feedback")]
    public string? AiFeedback { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
