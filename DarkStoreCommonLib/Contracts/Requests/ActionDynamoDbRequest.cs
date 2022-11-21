namespace DarkStoreCommonLib.Contracts.Requests;

public class ActionDynamoDbRequest
{
    public enum DbActions
    {
        NoTableAction = 0,
        CreateTableAction = 1,
        DeleteTableAction = 2
    }

    public DbActions Action { get; set; } = DbActions.NoTableAction;
    public string TableName { get; set; } = "TestDynamoDBTable";
    public string? PartitionKey { get; set; } = "pkId";
    public string? SortKey { get; set; } = "pkId";

}