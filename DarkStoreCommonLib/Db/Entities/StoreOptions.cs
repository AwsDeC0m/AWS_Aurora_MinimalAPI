namespace DarkStoreCommonLib.Db.Entities
{
    public class StoreOptions : BaseEntity
    {
        public float Square { get; set; }

        public int ParkingSize { get; set; }

        public bool HasGroceries { get; set; }

        public bool HasHouseholdGoods { get; set; }
    }
}
