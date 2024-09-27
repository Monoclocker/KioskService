using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace KioskService.Persistance.Database
{
    public class UnitOfWork
    {
        IMongoDatabase database;
        public UnitOfWork(IMongoClient client, IConfiguration configuration) 
        {
            this.database = client.GetDatabase(configuration["database"]);
        }
    }
}
