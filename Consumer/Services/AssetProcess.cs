using Consumer.Data;
using Consumer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Consumer.Services;

public class AssetProcess
{
    private readonly AppDbContext _context;

    public AssetProcess(AppDbContext context)
    {
        _context = context;
    }


    public async Task<bool> Process(AssetLiveReport report)
    {
        var assetTypes = new[] { "UAV", "PerimeterSensor" };

        if (!assetTypes.Contains(report.AssetType))
        {
            return false;
        }

        var assetExist = await _context.Assets.AnyAsync(a => a.Id == report.AssetId);
        if (!assetExist)
        {
            Console.WriteLine($"The assets {report.AssetId} does not exist.");
            return false;
        }

        string rawValue = report.RawValue;
        string processedStatus = null;
        bool isVerified = false;

        switch (report.AssetType)
        {
            case "UAV":
                (processedStatus, isVerified) = UavProcess(report.RawValue);
                break;
            case "PerimeterSensor":
                (processedStatus, rawValue, isVerified) = PerimeterSensorProcess(report.RawValue);
                break;
        }

        var exitAssetLiveStatus = await _context.AssetLiveStatus
            .FirstOrDefaultAsync(a => a.AssetId == report.AssetId);

        if (exitAssetLiveStatus is null)
        {
            var newAssetLiveStatus = new AssetLiveStatus
            {
                AssetId = report.AssetId,
                AssetType = report.AssetType,
                RawValue = rawValue,
                ProcessedStatus = processedStatus,
                IsVerified = isVerified,
                LastUpdate = report.Timestamp
            };
            _context.AssetLiveStatus.Add(newAssetLiveStatus);
        }
        else
        {
            exitAssetLiveStatus.RawValue = rawValue;
            exitAssetLiveStatus.ProcessedStatus = processedStatus;
            exitAssetLiveStatus.IsVerified = isVerified;
            exitAssetLiveStatus.LastUpdate = report.Timestamp;
        }
        
        await _context.SaveChangesAsync();
        return true;
    }


    public (string ProcessedStatus, bool IsVerified) UavProcess(string rawValue)
    {
        if (int.TryParse(rawValue, out int intRawValue))
        {
            if (intRawValue is >= 20 and <= 100)
            {
                return ("Stable", true);
            }

            if (intRawValue is >= 0 and <= 19)
            {
                return ("Warning", true);
            }

            return (rawValue, false);
        }
        else
        {
            return (rawValue, false);
        }
    }

    public (string ProcessedStatus, string validRawValue, bool IsVerified) PerimeterSensorProcess(string rawValue)
    {
        var goodRawValue = new[] { "Good", "GOOD", "good", "gud" };
        var BadRawValue = new[] { "Bad", "BAD", "bad", "bed" };

        if (goodRawValue.Contains(rawValue))
        {
            return ("Stable", "Good", true);
        }

        if (BadRawValue.Contains(rawValue))
        {
            return ("Warning", "Bad", true);
        }

        return ("Warning", rawValue, false);
    }
}