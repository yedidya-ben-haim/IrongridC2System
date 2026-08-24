using System.ComponentModel.DataAnnotations;

namespace Consumer.Models;

public class Assets
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string AssetSerial { get; set; } = string.Empty;
    
    public string AssetType { get; set; } = "GenericAsset";

    public int UnitId { get; set; }
    
    public Uin
}