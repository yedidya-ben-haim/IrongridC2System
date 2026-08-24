using ApiDashboard.Data;
using ApiDashboard.DTOs;
using ApiDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AssetsController(AppDbContext context)
    {
        _context = context;
    }

    //GET `/api/assets/{id}`
    [HttpGet("{id}")]
    public async Task<ActionResult<AssetDto?>> GetAssetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var asset = await _context.Assets.Select(a => new AssetDto
        {
            Id = a.Id,
            UnitId = a.UnitId,
            AssetSerial = a.AssetSerial,
            AssetType = a.AssetType
        }).FirstOrDefaultAsync(a => a.Id == id);
        if (asset is null)
        {
            return NotFound();
        }

        return Ok(asset);
    }

    //GET `/api/units/{id}`
    [HttpGet("units/{id}")]
    public async Task<ActionResult<UnitDto?>> GetUnitById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var unit = await _context.Units.Select(u =>
            new UnitDto
            {
                Id = u.Id,
                Sector = u.Sector,
                UnitName = u.UnitName
            }).FirstOrDefaultAsync(u => u.Id == id);

        if (unit is null)
        {
            return NotFound();
        }

        return Ok(unit);
    }

    //POST `/api/assets/units`
    [HttpPost("units")]
    public async Task<IActionResult> CreateUnit(UnitDto unit)
    {
        if (unit.Id <= 0)
        {
            return BadRequest("unit Less than zero");
        }
        var unitExist = await _context.Units.AnyAsync(a => a.Id == unit.Id);

        if (unitExist)
        {
            return BadRequest("unit exist");
        }

        var newUnit = new Unit
        {
            Id = unit.Id,
            Sector = unit.Sector,
            UnitName = unit.UnitName
        };
        _context.Units.Add(newUnit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUnitById),
            new { id = newUnit.Id }, "create");
    }

    //PUT `/api/assets/{id}`
    [HttpPut("{id}")]
    public async Task<ActionResult<AssetDto>> UpdateAsset(int id, AssetRequestDto assetRequestDto)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);

        if (asset is null)
        {
            return NotFound($"asset {id} not found");
        }

        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == assetRequestDto.UnitId);
        if (unit is null)
        {
            return NotFound($"unit {assetRequestDto.UnitId} not found");
        }

        asset.UnitId = assetRequestDto.UnitId;
        asset.AssetSerial = assetRequestDto.AssetSerial;
        asset.AssetType = assetRequestDto.AssetType;

        await _context.SaveChangesAsync();
        return Ok(new AssetDto
        {
            Id = asset.Id,
            UnitId = asset.UnitId,
            AssetSerial = asset.AssetSerial,
            AssetType = asset.AssetType
        });
    }
    
    //DELETE `/api/assets/{id}`
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);

        if (asset is null)
        {
            return NotFound($"asset {id} not found");
        }

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}