using System.ComponentModel.DataAnnotations;

namespace Consumer.Models;

public class Units
{
    public int Id { get; set; }

    [StringLength(100)]
    public string UnitName { get; set; } = "Unknown Unit";

    [StringLength(100)]
    public string Sector { get; set; } = "General";
    
    public ICollection<Asset> Assets { get; set; }
}