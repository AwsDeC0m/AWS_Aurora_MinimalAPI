using System.ComponentModel.DataAnnotations;

namespace DarkStoreCommonLib.Db.Entities;

public class Store : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Address { get; set; }

    public virtual StoreOptions Options { get; set; }
}
