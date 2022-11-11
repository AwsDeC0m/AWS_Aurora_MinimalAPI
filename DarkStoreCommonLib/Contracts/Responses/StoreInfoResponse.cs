
using DarkStoreCommonLib.Db.Entities;

namespace DarkStoreCommonLib.Contracts.Responses;

public class StoreInfoResponse
{
    public Guid StoreId { get; set; }

    public string StoreName { get; set; }

    public string StoreAddress { get; set; }

    public StoreOptions StoreOptions { get; set; }
}
