namespace KioskService.Persistance.Interfaces
{
    public interface IDBMapper<CoreType, DBType>
    {
        public DBType MapToDB(CoreType entity);
        public CoreType MapToCore(DBType entity);
    }
}
