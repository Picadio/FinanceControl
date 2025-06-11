using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceControl.Database.Entities;

public class User : BaseEntity
{
    [Length(1, 30)]
    public string Name { get; set; }
    [Length(1, 30)]
    public string PasswordHash { get; set; }

    public double Balance { get; set; } = 0;
    
    public ICollection<Payment> Payments { get; set; }
    public ICollection<Receipt> Receipts { get; set; }
}