using AwsDarkStoreCloudFormation.Db;
using Microsoft.EntityFrameworkCore;
using DarkStoreCommonLib.Db.Entities;
using DarkStoreCommonLib.Contracts.Requests;
using DarkStoreCommonLib.Contracts.Responses;

namespace DarkStoreAWServerless.Services;

public class DarkStoreService
{
    private readonly AppDbContext _appDbContext;

    public DarkStoreService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public List<StoreInfoResponse> GetStoresResponse()
    {
        var stores = _appDbContext.Stores
             .Include(x => x.Options).ToList();

        return stores.Select(a => new StoreInfoResponse()
        {
            StoreId = a.Id,
            StoreName = a.Name,
            StoreAddress = a.Address,
            StoreOptions = a.Options
        }).ToList();
    }

    public StoreInfoResponse GetStore(Guid id)
    {
        var store = _appDbContext.Stores
             .Include(x => x.Options)
             .SingleOrDefault(x => x.Id == id);

        return new StoreInfoResponse()
        {
            StoreId = store.Id,
            StoreName = store.Name,
            StoreAddress = store.Address,
            StoreOptions = store.Options
        };
    }

    public String PostStore(StoreInfoRequest storeInfo)
    {
        if (storeInfo == null)
            throw new ArgumentException($"{nameof(StoreInfoRequest)}is null");

        var curDt = DateTime.Now;

        var storeModel = new Store()
        {
            Id = Guid.NewGuid(),
            Address = storeInfo.Address,
            Name = storeInfo.Name,
            CreatedAt = curDt,
            UpdatedAt = curDt,
            Options = new StoreOptions()
            {
                Id = Guid.NewGuid(),
                Square = storeInfo.Options.Square,
                ParkingSize = storeInfo.Options.ParkingSize,
                HasGroceries = storeInfo.Options.HasGroceries,
                HasHouseholdGoods = storeInfo.Options.HasHouseholdGoods,
                CreatedAt = curDt,
                UpdatedAt = curDt
            }
        };
        try
        {
            _appDbContext.Add(storeModel);
            _appDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception($"Store creating exception: {ex.Message} {ex.InnerException}");
        }

        return storeModel.Id.ToString();
    }

}


