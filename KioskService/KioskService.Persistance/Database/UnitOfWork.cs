using KioskService.Core.Models;
using KioskService.Persistance.Interfaces;
using KioskService.Persistance.Repositories;
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

        private IRepository<Settings>? _settingsRepository;
        public IRepository<Settings> Settings
        {
            get
            {
                if(_settingsRepository == null)
                {
                    _settingsRepository = new SettingsRepository(this.database);
                }
                return _settingsRepository;
            }
        }
    }
}
