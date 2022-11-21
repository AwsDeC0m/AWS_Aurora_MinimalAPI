using DarkStoreCommonLib.Db.Entities;
namespace DarkStoreCommonLib.DynamoDB;

public class DarkStoreDynamo : BaseEntity
{
    public string City { get; set; }

    public string Name { get; set; }

    public virtual DarkStoreOptionsDynamo Options { get; set; }
}