namespace ApiDashboard.DTOs;

public class UnitAssetsDto
{
    public int assetId { get; set; }
    public string AssetSerial { get; set; } = string.Empty;
    public string AssetType { get; set; } = "GenericAsset";
    public string? ProcessedStatus { get; set; } = string.Empty;
    public bool? IsVerified { get; set; }
    public DateTime? LastUpdate { get; set; }
}