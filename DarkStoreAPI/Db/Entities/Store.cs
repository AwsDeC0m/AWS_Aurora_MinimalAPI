using System.ComponentModel.DataAnnotations;

namespace DarStoreAPI.Db.Entities;

public class Store : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Address { get; set; }
}
