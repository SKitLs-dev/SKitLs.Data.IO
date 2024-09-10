﻿using SKitLs.Data.Core.IO;
using SKitLs.Data.IO.Shortcuts;

namespace SKitLs.Data.IO.Json
{
    /// <summary>
    /// A data writer that serializes each <typeparamref name="TData"/> item into a separate JSON file.
    /// The file name is based on the item's ID.
    /// </summary>
    /// <typeparam name="TData">The type of data to write. It must implement <see cref="ModelDso{TId}"/>.</typeparam>
    /// <typeparam name="TId">The type of the unique identifier for each data item. Must be comparable and equatable.</typeparam>
    /// <remarks>
    /// This class writes each data item into its own file in the specified directory. The file name is generated based on the item's ID.
    /// </remarks>
    /// <param name="dataPath">The directory path where the JSON files will be stored.</param>
    public class JsonSplitWriter<TData, TId>(string dataPath) : JsonIOBase(dataPath), IDataWriter<TData> where TData : ModelDso<TId> where TId : notnull, IEquatable<TId>, IComparable<TId>
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <inheritdoc/>
        public bool WriteData(TData item)
        {
            return WriteDataAsync(item).Result;
        }

        /// <inheritdoc/>
        public bool WriteData<T>(T item) where T : class
        {
            return WriteDataAsync(item).Result;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteDataAsync(TData item, CancellationTokenSource? cts = null)
        {
            return await WriteDataAsync(item, cts);
        }

        /// <inheritdoc/>
        public async Task<bool> WriteDataAsync<T>(T item, CancellationTokenSource? cts = null) where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(TData)))
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

            return await WriteDataAsyncInternal((item as TData)!, cts);
        }

        /// <inheritdoc/>
        public bool WriteDataList(IEnumerable<TData> items)
        {
            return WriteDataListAsync(items).Result;
        }

        /// <inheritdoc/>
        public bool WriteDataList<T>(IEnumerable<T> items) where T : class
        {
            return WriteDataListAsync(items).Result;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteDataListAsync(IEnumerable<TData> items, CancellationTokenSource? cts = null)
        {
            var success = true;
            foreach (var item in items)
            {
                success &= await WriteDataAsync(item, cts);
            }
            return success;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteDataListAsync<T>(IEnumerable<T> items, CancellationTokenSource? cts = null) where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(TData)))
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

            var success = true;
            foreach (var item in items)
            {
                success &= await WriteDataAsync(item, cts);
            }
            return success;
        }

        /// <summary>
        /// Generates the file path for a specific data item based on its ID.
        /// </summary>
        /// <remarks>
        /// The file path is generated by combining the base data path with the item's ID.
        /// </remarks>
        /// <param name="id">The unique identifier for the data item.</param>
        /// <returns>A string representing the file path where the serialized JSON for the item will be saved.</returns>
        private string GetFilePath(TId id) => Path.Combine(DataPath, $"{id}.json");

        /// <summary>
        /// Asynchronously writes a single data item to a JSON file.
        /// </summary>
        /// <remarks>
        /// This method serializes the <paramref name="item"/> into a JSON file, where the file name is based on the item's ID.
        /// </remarks>
        /// <param name="item">The data item to write to a JSON file.</param>
        /// <param name="cts">An optional <see cref="CancellationTokenSource"/> to cancel the asynchronous write operation. If not provided, a default token will be used.</param>
        /// <returns>A task that represents the asynchronous write operation. The result is <see langword="true"/> if the write operation succeeded.</returns>
        /// <exception cref="IOException">Thrown if there is an issue with file I/O during the writing process.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cts"/> token.</exception>
        private async Task<bool> WriteDataAsyncInternal(TData item, CancellationTokenSource? cts = null)
        {
            var filePath = GetFilePath(item.GetId());
            await HotIO.SaveJsonAsync(item, filePath, cts);
            return true;
        }
    }
}