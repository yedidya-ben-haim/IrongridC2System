namespace Producer.Models;

public class AssetLiveReport
{
    public int AssetId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string RawValue { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}