using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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

        /// <summary>
        /// Scans the network for viable servers
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable
        IAsyncEnumerable<string> GetIPAddresses([EnumeratorCancellation] CancellationToken cancellationToken = default);
#pragma warning restore CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable
    }
}
