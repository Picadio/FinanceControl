using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanceControl.Database.Entities;

public class User : BaseEntity
{
    [Length(1, 30)]
    public string Name { get; set; }
    [Length(1, 30)]
    [JsonIgnore]
    public string PasswordHash { get; set; }

    public double Balance { get; set; } = 0;
    
    [JsonIgnore]
    public ICollection<Payment> Payments { get; set; }
    [JsonIgnore]
    public ICollection<Receipt> Receipts { get; set; }
}