using KioskService.Core.DTO;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace KioskService.Persistance.Services
{
    public class ResultsService : IResultsService
    {
        Utils.Mappers mappers;
        DatabaseContext context;
        public ResultsService(Utils.Mappers mappers, DatabaseContext context)
        {
            this.mappers = mappers;
            this.context = context;
        }

        public async Task<PaginatedList<ResultsPreview>> GetPreviousResults(int page = 1)
        {
            PaginatedList<ResultsPreview> pagesList = new PaginatedList<ResultsPreview>();

            pagesList.pagesCount = (await context.Results.CountAsync()) / 10 + 1;

            pagesList.results = await context.Results
                .Skip(10 * (page - 1))
                .Take(10)
                .Select(x => new ResultsPreview()
                {
                    id = x.Id,
                    localDate = x.TimeStamp.ToLocalTime(),
                })
                .ToListAsync();

            return pagesList;
        }

        public async Task<Results?> GetResults(int id)
        {
            ResultsEntity? results = await context.Results
                .AsNoTracking() 
                .FirstOrDefaultAsync(x => x.Id == id);

            if (results != null)
            {
                return mappers.resultsMapper.MapToCore(results);
            }

            return null;
        }

        public async Task<int> SaveResults(Results dto)
        {
            ResultsEntity newResults = mappers.resultsMapper.MapToDB(dto);

            await context.Results.AddAsync(newResults);

            await context.SaveChangesAsync();

            return newResults.Id;
        }
    }
}
