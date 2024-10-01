using KioskService.Core.DTO;
using KioskService.Core.Models;

namespace KioskService.Core.Interfaces
{
    public interface IResultsService
    {
        public Task<int> SaveResults(Results dto);
        public Task<Results?> GetResults(int id);
        public Task<PaginatedList<ResultsPreview>> GetPreviousResults(int page); 
    }
}
