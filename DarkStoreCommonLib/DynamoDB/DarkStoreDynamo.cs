using Amazon.DynamoDBv2.DataModel;

namespace DarkStoreCommonLib.DynamoDB;

[DynamoDBTable("DarkStoreNoSQL")]
public class DarkStoreDynamo : BaseDynamoRequest
{
    [DynamoDBHashKey]
    public string City { get; set; }
    [DynamoDBRangeKey]
    public int NrStore { get; set; }
    [DynamoDBProperty("StoreOptions")]
    public virtual DarkStoreOptionsDynamo Options { get; set; }
}

