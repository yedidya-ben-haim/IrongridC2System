using ApiDashboard.Data;
using ApiDashboard.DTOs;
using ApiDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiDashboard.Controllers;

[ApiController]
[Route("api/assets-status")]
public class AssetsStatusController : ControllerBase
{
    private readonly AppDbContext _context;
    // private readonly IrongridRedisService _cache;

    public AssetsStatusController(AppDbContext context)
    {
        _context = context;
    }
    
    //GET `/api/assets-status`
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<AssetsStatusDto?>>> GetAllAssets()
    {
        return await _context.Assets
            .Include(a => a.AssetLiveStatus)
            .Select(a => new AssetsStatusDto
            {
                Id = a.Id,
                UnitId = a.UnitId,
                AssetSerial = a.AssetSerial,
                AssetType = a.AssetType,
                RawValue = a.AssetLiveStatus.RawValue,
                ProcessedStatus = a.AssetLiveStatus.ProcessedStatus,
                IsVerified = a.AssetLiveStatus.IsVerified,
                LastUpdate = a.AssetLiveStatus.LastUpdate
            }).ToListAsync();
    }
    
    //GET `/api/assets-status?status={status}`
    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<AssetsStatusDto?>>> GetAssetsByStatus(string? status)
    {
        var query = _context.Assets
            .Include(a => a.AssetLiveStatus)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.AssetLiveStatus.ProcessedStatus == status);
        }
        
        return await query
            .Select(a => new AssetsStatusDto
            {
                Id = a.Id,
                UnitId = a.UnitId,
                AssetSerial = a.AssetSerial,
                AssetType = a.AssetType,
                RawValue = a.AssetLiveStatus.RawValue,
                ProcessedStatus = a.AssetLiveStatus.ProcessedStatus,
                IsVerified = a.AssetLiveStatus.IsVerified,
                LastUpdate = a.AssetLiveStatus.LastUpdate
            }).ToListAsync();
    }
    
    //GET `/api/assets-status/{id}`
    [HttpGet("{id}")]
    public async Task<ActionResult<AssetsStatusDto>> GetStatusById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var assetStatus = await _context.Assets.Include(a => a.AssetLiveStatus)
            .Select(a => new AssetsStatusDto
            {
                Id = a.Id,
                UnitId = a.UnitId,
                AssetSerial = a.AssetSerial,
                AssetType = a.AssetType,
                RawValue = a.AssetLiveStatus.RawValue,
                ProcessedStatus = a.AssetLiveStatus.ProcessedStatus,
                IsVerified = a.AssetLiveStatus.IsVerified,
                LastUpdate = a.AssetLiveStatus.LastUpdate
            }).FirstOrDefaultAsync(a => a.Id == id);
        if (assetStatus is null)
        {
            return NotFound($"asset {id} not found");
        }

        return Ok(assetStatus);

    }

    // [HttpGet("save/{id}")]
    // public async Task<ActionResult<string>> Save(int id)
    // {
    //     var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);
    //     await _cache.SaveAsync(id, asset);
    //     return Ok("save");
    // }
}