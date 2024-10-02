using KioskService.Core.DTO;
using System.Diagnostics.CodeAnalysis;

namespace KioskService.Tests.Utils
{
    public class ResultsPaginatedListComparer : IEqualityComparer<PaginatedList<ResultsPreview>>
    {
        public bool Equals(PaginatedList<ResultsPreview>? x, PaginatedList<ResultsPreview>? y)
        {
            if (x == null || y == null)
                return false;

            if (x.results.Count != y.results.Count) 
                return false;

            for(int index = 0; index < x.results.Count; index++)
            {
                if (x.results[index].id != y.results[index].id)
                    return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] PaginatedList<ResultsPreview> obj)
        {
            return obj.GetHashCode();
        }
    }
}
