using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoverMobile.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        /// <summary>
        /// Gets the last item selected in the items page, or null if nothing is selected.
        /// </summary>
        /// <returns></returns>
        Task<T> GetSelectedItem();

        Task SelectItem(string id);
        void Refresh();
    }
}
