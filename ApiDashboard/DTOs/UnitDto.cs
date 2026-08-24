using System.ComponentModel.DataAnnotations;
using ApiDashboard.Models;

namespace ApiDashboard.DTOs;

public class UnitDto
{
    public int Id { get; set; }
    public string UnitName { get; set; } = "Unknown Unit";
    public string Sector { get; set; } = "General";
}