namespace KioskService.Persistance.Interfaces
{
    public interface IRepository<T>
    {
        public Task<T> Get(string id);
        public Task<IEnumerable<T>> GetAll();
        public Task Add(T entity);
        public Task Update(T entity);
        public Task Delete(T entity);
    }
}
