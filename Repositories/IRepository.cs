using System.Collections.Generic;
using System.Threading.Tasks;

namespace mvvm_edp.Repositories;

public interface IRepository<T>
{
    Task<List<T>> GetAllItemsAsync();
    Task<int> AddItemAsync(T item);
    Task<int> RemoveItemAsync(T item);
    Task RemoveAllItemsAsync(List<T> items);
    Task<int> UpdateItemAsync(T item);
    Task UpdateAllItemsAsync(List<T> items);

}
