using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanceControl.Database.Entities;

public class Receipt : BaseEntity
{
    [Length(1, 50)]
    public string Name { get; set; }
    [Range(0, double.MaxValue)]
    public double Sum { get; set; }
    [Range(1, 31)]
    public int Day { get; set; }
    public bool Enabled { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public User User { get; set; }
}