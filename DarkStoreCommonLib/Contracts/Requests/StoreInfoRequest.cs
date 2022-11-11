using DarkStoreCommonLib.Db.Entities;

namespace DarkStoreCommonLib.Contracts.Requests
{
    public class StoreInfoRequest
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public StoreOptionsDto Options { get; set; }
    }

    public class StoreOptionsDto
    {
        public float Square { get; set; }

        public int ParkingSize { get; set; }

        public bool HasGroceries { get; set; }

        public bool HasHouseholdGoods { get; set; }
    }
}
