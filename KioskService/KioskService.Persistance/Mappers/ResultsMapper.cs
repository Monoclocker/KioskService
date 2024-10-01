using KioskService.Core.Models;
using KioskService.Persistance.Entities;
using KioskService.Persistance.Interfaces;
using System.Text;

namespace KioskService.Persistance.Mappers
{
    public class ResultsMapper : IDBMapper<Results, ResultsEntity>
    {
        public Results MapToCore(ResultsEntity entity)
        {
            string? checkOrigin = null;

            if (entity.Check != null)
            {
                checkOrigin = Encoding.UTF8.GetString(Convert.FromBase64String(entity.Check));
            }

            return new Results()
            {
                sum = entity.Sum,
                check = checkOrigin,
                deviceId = entity.DeviceId,
                utcTime = entity.TimeStamp,
                localTime = entity.TimeStamp.ToLocalTime(),
            };
        }

        public ResultsEntity MapToDB(Results entity)
        {
            string? checkB64 = null;

            if (entity.check != null)
            {
                checkB64 = Encoding.UTF8.GetString(Convert.FromBase64String(entity.check));
            }

            return new ResultsEntity()
            {
                Sum = entity.sum,
                TimeStamp = entity.utcTime,
                DeviceId = entity.deviceId,
                Check = checkB64
            };
        }
    }
}
