using System.ComponentModel.DataAnnotations;

namespace Consumer.Models;

public class AssetLiveStatus
{
    public int AssetId { get; set; }
    
    [AllowedValues("PerimeterSensor", "UAV")]
    public string AssetType { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string RawValue { get; set; } = string.Empty;
    
    [AllowedValues("Stable","Warning")]
    public string ProcessedStatus { get; set; } = string.Empty;
    
    [Required]
    public bool IsVerified { get; set; }
    
    [Required]
    public DateTime LastUpdate { get; set; }
    
    public Asset Asset { get; set; }
}