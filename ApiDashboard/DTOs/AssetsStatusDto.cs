namespace ApiDashboard.DTOs;

public class AssetsStatusDto
{
    public int Id { get; set; }
    public int UnitId { get; set; }
    public string AssetSerial { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string? RawValue { get; set; } = string.Empty;
    public string? ProcessedStatus { get; set; } = string.Empty;
    public bool? IsVerified { get; set; }
    public DateTime? LastUpdate { get; set; }
}