namespace KioskService.WEB.Interfaces
{
    public interface IConnectionStorage : IEnumerable<string>
    {
        public void Add(string id);
        public void Delete(string id);
    }
}
