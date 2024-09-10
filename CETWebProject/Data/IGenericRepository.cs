using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public interface IGenericRepository<T> where T: class
    {
        //Create
        Task CreateAsync(T entity);
        //Read
        Task<T> GetByIdAsync(int id);
        //Update
        Task UpdateAsync(T entity);
        //Delete
        Task DeleteAsync(T entity);

        Task<bool> ExistAsync(int id);

        Task<bool> SaveAllAsync();
    }
}