using System.Collections.Generic;

namespace ArtShareServer.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        public List<T> Get();
        public T Get(int id);
        public int Create(T obj);
        public void Delete(int id);
        public T Update(T obj);
    }
}