using KioskService.Core.Models;
using KioskService.Persistance.Interfaces;
using KioskService.Persistance.Utils;
using MongoDB.Driver;

namespace KioskService.Persistance.Repositories
{
    public class SettingsRepository : IRepository<Settings>
    {
        IMongoCollection<Settings> collection; 
        public SettingsRepository(IMongoDatabase database)
        {
            this.collection = database.GetCollection<Settings>(CollectionNames.Settings);
        }
        public Task Add(Settings entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Settings entity)
        {
            throw new NotImplementedException();
        }

        public Task<Settings> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Settings>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Update(Settings entity)
        {
            throw new NotImplementedException();
        }
    }
}
