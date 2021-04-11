using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.API.Contracts
{
    // Common (CRUD) operations for every Table
    public interface IRepositoryBase<T> where T : class
    {
        Task<IList<T>> FindAll();
        Task<T> FindById(int id);
        Task<bool> Exists(int id);
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save(); // commit to DB

    }
}
