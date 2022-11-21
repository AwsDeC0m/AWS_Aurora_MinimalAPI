namespace DarkStoreCommonLib.DynamoDB;

public class DarkStoreDynamo
{
    public string City { get; set; }

    public int NrStore { get; set; }

    public virtual DarkStoreOptionsDynamo Options { get; set; }
}