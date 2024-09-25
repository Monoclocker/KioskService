using KioskService.WEB.Interfaces;
using System.Collections;

namespace KioskService.WEB.Storages
{
    public class InMemoryConnectionStorage : IConnectionStorage
    {
        List<string> ConnectionIds = new List<string>();

        public void Add(string id)
        {
            ConnectionIds.Add(id);
        }

        public void Delete(string id)
        {
            ConnectionIds.Remove(id);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ConnectionIds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }
    }
}
