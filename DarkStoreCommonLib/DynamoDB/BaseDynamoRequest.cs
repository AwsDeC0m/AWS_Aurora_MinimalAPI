namespace DarkStoreCommonLib.DynamoDB;

public class BaseDynamoRequest
{
    /// <summary>
    /// DynamoDB Table name
    /// </summary>
    public string TableName { get; set; } = "TestDynamoDbTable";

    /// <summary>
    /// Mode for requests
    /// </summary>
    public int RequestCommandMode { get; set; } = 0;

}