using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebIeltsFree.Models;

public class WritingPromptViewModel
{
    [Required(ErrorMessage = "Task type is required.")]
    [MaxLength(10)]
    public string TaskType { get; set; } = "task1";

    [Required(ErrorMessage = "Prompt text is required.")]
    public string PromptText { get; set; } = string.Empty;

    public string? SampleAnswer { get; set; }

    [Range(0, 9, ErrorMessage = "Band score must be between 0 and 9.")]
    public decimal TargetBand { get; set; } = 6;

    public IFormFile? ImageFile { get; set; }
    
    public string? Action { get; set; } // Used in Edit to check for "submit_review"
}
