using Newtonsoft.Json;
using OfficeOpenXml.Drawing.Chart;
using SKitLs.Data.Core.IO;
using System.Text;

namespace SKitLs.Data.IO.Json
{
    /// <summary>
    /// Provides functionality to write data to a JSON file.
    /// </summary>
    /// <typeparam name="TData">The type of entity to write.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JsonWriter{TData, TId}"/> class with the specified data file path.
    /// </remarks>
    /// <param name="dataPath">The path to the JSON file.</param>
    public class JsonWriter<TData, TId>(string dataPath) : JsonIOBase(dataPath), IDataWriter<TData> where TData : ModelDso<TId> where TId : notnull, IEquatable<TId>, IComparable<TId>
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsync(TData, CancellationTokenSource?)"/>
        public bool WriteData(TData item)
        {
            return WriteDataAsync(item, null).Result;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsync(TData, CancellationTokenSource?)"/>
        public bool WriteData<T>(T item) where T : class
        {
            return WriteDataAsync(item, null).Result;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataListAsync(IEnumerable{TData}, CancellationTokenSource?)"/>
        public bool WriteDataList(IEnumerable<TData> items)
        {
            return WriteDataListAsync(items, null).Result;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataListAsync(IEnumerable{TData}, CancellationTokenSource?)"/>
        public bool WriteDataList<T>(IEnumerable<T> items) where T : class
        {
            return WriteDataListAsync(items, null).Result;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsyncInternal(TData, CancellationTokenSource?)"/>
        public async Task<bool> WriteDataAsync(TData item, CancellationTokenSource? cts)
        {
            return await WriteDataAsyncInternal(item, cts);
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsyncInternal(TData, CancellationTokenSource?)"/>
        /// <exception cref="NotSupportedException">Thrown when the type parameter is not supported.</exception>
        public async Task<bool> WriteDataAsync<T>(T item, CancellationTokenSource? cts) where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(TData)))
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

            return await WriteDataAsyncInternal((item as TData)!, cts);
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsyncInternal(TData, CancellationTokenSource?)"/>
        public async Task<bool> WriteDataListAsync(IEnumerable<TData> items, CancellationTokenSource? cts)
        {
            return await WriteDataListAsyncInternal(items, cts);
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsyncInternal(TData, CancellationTokenSource?)"/>
        /// <exception cref="NotSupportedException">Thrown when the type parameter is not supported.</exception>
        public async Task<bool> WriteDataListAsync<T>(IEnumerable<T> items, CancellationTokenSource? cts) where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(TData)))
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

            return await WriteDataListAsyncInternal(items.Select(x => (x as TData)!), cts);
        }

        /// <summary>
        /// Writes a single data item to the JSON file.
        /// </summary>
        /// <param name="item">The data item to write.</param>
        /// <param name="cts">The cancellation token source to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous write operation. The task result contains a boolean indicating whether the write operation was successful.</returns>
        /// <inheritdoc cref="List{T}.FindIndex(Predicate{T})"/>
        /// <inheritdoc cref="ReadAllItemsAsync{T}(CancellationTokenSource?)"/>
        /// <inheritdoc cref="WriteAllItemsAsync{T}(List{T}, CancellationTokenSource?)"/>
        private async Task<bool> WriteDataAsyncInternal(TData item, CancellationTokenSource? cts)
        {
            var items = await ReadAllItemsAsync<TData>(cts);
            var existingItemIndex = items.FindIndex(x => x.GetId().Equals(item.GetId()));

            if (existingItemIndex >= 0)
            {
                items[existingItemIndex] = item;
            }
            else
            {
                items.Add(item);
            }

            await WriteAllItemsAsync(items, cts);
            return true;
        }

        /// <summary>
        /// Writes a list of data items to the JSON file.
        /// </summary>
        /// <param name="items">The list of data items to write.</param>
        /// <param name="cts">The cancellation token source to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous write operation. The task result contains a boolean indicating whether the write operation was successful.</returns>
        /// <inheritdoc cref="ReadAllItemsAsync{T}(CancellationTokenSource?)"/>
        /// <inheritdoc cref="WriteAllItemsAsync{T}(List{T}, CancellationTokenSource?)"/>
        private async Task<bool> WriteDataListAsyncInternal(IEnumerable<TData> items, CancellationTokenSource? cts)
        {
            var allItems = await ReadAllItemsAsync<TData>(cts);
            foreach (var item in items)
            {
                var existingItemIndex = allItems.FindIndex(x => x.GetId().Equals(item.GetId()));
                if (existingItemIndex >= 0)
                {
                    allItems[existingItemIndex] = item;
                }
                else
                {
                    allItems.Add(item);
                }
            }

            await WriteAllItemsAsync(allItems, cts);
            return true;
        }

        /// <summary>
        /// Reads all items from the JSON file.
        /// </summary>
        /// <typeparam name="T">The type of items to read.</typeparam>
        /// <param name="cts">The cancellation token source to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous read operation. The task result contains a list of items read from the JSON file.</returns>
        /// <inheritdoc cref="StreamReader.ReadToEndAsync()"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="FileNotFoundException">Thrown when the JSON file is not found.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        private async Task<List<T>> ReadAllItemsAsync<T>(CancellationTokenSource? cts) where T : class
        {
            if (!File.Exists(DataPath))
            {
                if (CreateNewFile)
                {
                    File.Create(DataPath).Close();
                    return [];
                }
                else
                    throw new FileNotFoundException($"JSON file not found at path: {DataPath}", DataPath);
            }

            cts ??= new();
            using var reader = new StreamReader(DataPath);
            var jsonData = await reader.ReadToEndAsync(cts.Token);
            return JsonConvert.DeserializeObject<List<T>>(jsonData, JsonSerializerSettings) ?? throw new JsonSerializationException("Failed to deserialize JSON data.");
        }

        /// <summary>
        /// Writes all items to the JSON file.
        /// </summary>
        /// <typeparam name="T">The type of items to write.</typeparam>
        /// <param name="items">The list of items to write.</param>
        /// <param name="cts">The cancellation token source to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <inheritdoc cref="StreamWriter.WriteAsync(string?)"/>
        /// <inheritdoc cref="JsonConvert.SerializeObject(object?, JsonSerializerSettings?)"/>
        private async Task WriteAllItemsAsync<T>(List<T> items, CancellationTokenSource? cts) where T : class
        {
            cts ??= new();
            if (!File.Exists(DataPath))
            {
                if (CreateNewFile)
                    File.Create(DataPath).Close();
                else
                    throw new FileNotFoundException($"JSON file not found at path: {DataPath}", DataPath);
            }
            
            using var writer = new StreamWriter(DataPath, false);
            var jsonData = JsonConvert.SerializeObject(items, JsonSerializerSettings);
            var builder = new StringBuilder(jsonData);
            await writer.WriteAsync(builder, cts.Token);
        }
    }
}