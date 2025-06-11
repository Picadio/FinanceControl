using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Database.Entities;

public class BaseEntity
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();
}