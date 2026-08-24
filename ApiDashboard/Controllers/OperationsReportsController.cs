using ApiDashboard.Data;
using ApiDashboard.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiDashboard.Controllers;

[ApiController]
[Route("api/reports")]
public class OperationsReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public OperationsReportsController(AppDbContext context)
    {
        _context = context;
    }

    //GET `/api/reports/critical-assets`
    [HttpGet("critical-assets")]
    public async Task<ActionResult<IEnumerable<CriticalAssetsDto?>>> GetCriticalAssets()
    {
        var criticalAssets = await _context.Assets
            .Include(a => a.AssetLiveStatus)
            .Include(a => a.Unit)
            .Where(a => a.AssetLiveStatus.ProcessedStatus == "Warning"
                        || a.AssetLiveStatus.IsVerified == false)
            .Select(a => new CriticalAssetsDto
            {
                Id = a.Id,
                AssetSerial = a.AssetSerial,
                AssetType = a.AssetType,
                UnitName = a.Unit.UnitName,
                Sector = a.Unit.Sector,
                ProcessedStatus = a.AssetLiveStatus.ProcessedStatus,
                IsVerified = a.AssetLiveStatus.IsVerified,
                LastUpdate = a.AssetLiveStatus.LastUpdate
            })
            .ToListAsync();
       
        return Ok(criticalAssets);
    }
    
    //GET `/api/reports/unit/{unitId}/assets`
    [HttpGet("unit/{unitId}/assets")]
    public async Task<ActionResult<IEnumerable<UnitAssetsDto?>>> GetUnitsAssets(int unitId)
    {
        if (unitId <= 0)
        {
            return BadRequest("unitId lower then zero");
        }
        var unit = await _context.Units
            .FirstOrDefaultAsync(u => u.Id == unitId);
        
        if (unit is null)
        {
            return NotFound($"unit {unitId} not found");
        }

        var unitAssent = await _context.Assets.Where(a => a.UnitId == unitId)
            .Select(a => new UnitAssetsDto
            {
                assetId = a.Id,
                AssetSerial = a.AssetSerial,
                AssetType = a.AssetType,
                ProcessedStatus = a.AssetLiveStatus.ProcessedStatus,
                IsVerified = a.AssetLiveStatus.IsVerified,
                LastUpdate = a.AssetLiveStatus.LastUpdate
            }).ToListAsync();
        
        return Ok(unitAssent);
    }
    
    //GET `/api/reports/summary-by-unit`
    [HttpGet("summary-by-unit")]
    public async Task<ActionResult<IEnumerable<SummaryAssetsDto>>> GetAssetsSummary()
    {
        var summaryAssets = await _context.Units
            .Select(u => new SummaryAssetsDto
            {
                UnitId = u.Id,
                UnitName = u.UnitName,
                Sector = u.Sector,
                TotalAssets = u.Assets.Count(),
                StableAssets = u.Assets.Count(a => a.AssetLiveStatus.ProcessedStatus == "Stable"),
                WarningAssets = u.Assets.Count(a => a.AssetLiveStatus.ProcessedStatus == "Warning"),
                UnverifiedAssets = u.Assets.Count(a => a.AssetLiveStatus.IsVerified == false)
            }).ToListAsync();
        
        return Ok(summaryAssets);
    }
    
}
