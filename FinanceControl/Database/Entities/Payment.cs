using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Database.Entities;

public class Payment : BaseEntity
{
    [Length(1, 50)]
    public string Name { get; set; }
    [Length(0, 500)]
    public string? Description { get; set; }
    [Range(0, double.MaxValue)]
    public double Sum { get; set; }
    public DateTime DateTime { get; set; }
    public DateOnly? MonthlyPayDate { get; set; }
    public string? WhoShouldReturn { get; set; }
    public string? Check { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public User User { get; set; }
}