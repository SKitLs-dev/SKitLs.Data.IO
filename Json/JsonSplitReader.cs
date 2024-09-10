using Newtonsoft.Json;
using SKitLs.Data.Core.IO;
using SKitLs.Data.IO.Shortcuts;

namespace SKitLs.Data.IO.Json
{
    /// <summary>
    /// A data reader that reads JSON data from a directory, where each file represents a separate object of type <typeparamref name="TData"/>.
    /// </summary>
    /// <typeparam name="TData">The type of data to read. It must implement a model with a unique identifier.</typeparam>
    /// <remarks>
    /// This class reads data from a directory where each file is assumed to contain JSON data representing an instance of <typeparamref name="TData"/>.
    /// </remarks>
    public class JsonSplitReader<TData>(string dataPath) : JsonIOBase(dataPath), IDataReader<TData>
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <inheritdoc/>
        /// <inheritdoc cref="File.ReadAllText(string)"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="InvalidOperationException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        public IEnumerable<TData> ReadData() => ReadDataAsync().Result;

        /// <inheritdoc/>
        /// <inheritdoc cref="File.ReadAllText(string)"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="InvalidOperationException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        public IEnumerable<T> ReadData<T>() where T : class => ReadDataAsync<T>().Result;

        /// <inheritdoc/>
        /// <inheritdoc cref="File.ReadAllText(string)"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="InvalidOperationException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        public async Task<IEnumerable<TData>> ReadDataAsync(CancellationTokenSource? cts = null)
        {
            if (string.IsNullOrWhiteSpace(DataPath))
                throw new InvalidOperationException("Data path cannot be null or empty.");

            if (!Directory.Exists(DataPath))
            {
                if (CreateNewFile)
                    Directory.CreateDirectory(DataPath);
                else
                    throw new DirectoryNotFoundException($"JSON directory storage not found at path: {DataPath}");
            }

            cts ??= new();
            var result = new List<TData>();
            foreach (var fileInfo in Directory.GetFiles(DataPath))
            {
                try
                {
                    var data = await HotIO.LoadJsonAsync<TData>(fileInfo, cts);
                    if (data is not null)
                        result.Add(data);
                }
                catch (JsonSerializationException) { }
            }
            return result;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="File.ReadAllText(string)"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="InvalidOperationException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        public async Task<IEnumerable<T>> ReadDataAsync<T>(CancellationTokenSource? cts = null) where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(TData)))
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

            return (await ReadDataAsync()).Select(x => (x as T)!);
        }
    }
}