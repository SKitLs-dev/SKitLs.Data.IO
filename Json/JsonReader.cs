using Newtonsoft.Json;
using SKitLs.Data.Core.IO;
using System.Data;

namespace SKitLs.Data.IO.Json
{
    /// <summary>
    /// Provides functionality to read data from a JSON file.
    /// </summary>
    /// <typeparam name="TData">The type of entity to read.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JsonReader{TData}"/> class with the specified data file path.
    /// </remarks>
    /// <param name="dataPath">The path to the JSON file.</param>
    public class JsonReader<TData>(string dataPath) : JsonIOBase(dataPath), IDataReader<TData>
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <inheritdoc/>
        /// <inheritdoc cref="File.ReadAllText(string)"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="InvalidOperationException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        public IEnumerable<TData> ReadData()
        {
            if (string.IsNullOrWhiteSpace(DataPath))
                throw new InvalidOperationException("File path cannot be null or empty.");

            if (!File.Exists(DataPath))
            {
                if (CreateNewFile)
                    File.Create(DataPath).Close();
                else
                    throw new FileNotFoundException($"JSON file not found at path: {DataPath}", DataPath);
            }

            var jsonData = File.ReadAllText(DataPath);
            if (string.IsNullOrEmpty(jsonData))
                return [];

            return JsonConvert.DeserializeObject<IEnumerable<TData>>(jsonData, JsonSerializerSettings) ?? throw new JsonSerializationException();
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="ReadData()"/>
        /// <exception cref="NotSupportedException">Thrown when the type parameter is not supported.</exception>
        public IEnumerable<T> ReadData<T>() where T : class
        {
            if (typeof(T) == typeof(TData))
                return ReadData().Select(x => (x as T)!);
            else
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="File.ReadAllTextAsync(string, CancellationToken)"/>
        /// <inheritdoc cref="JsonConvert.DeserializeObject(string, JsonSerializerSettings)"/>
        /// <exception cref="InvalidOperationException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error during JSON deserialization.</exception>
        public async Task<IEnumerable<TData>> ReadDataAsync(CancellationTokenSource? cts)
        {
            if (string.IsNullOrWhiteSpace(DataPath))
                throw new InvalidOperationException("File path cannot be null or empty.");

            if (!File.Exists(DataPath))
            {
                if (CreateNewFile)
                    File.Create(DataPath).Close();
                else
                    throw new FileNotFoundException($"JSON file not found at path: {DataPath}", DataPath);
            }

            cts ??= new();
            var jsonData = await File.ReadAllTextAsync(DataPath, cts.Token);
            return JsonConvert.DeserializeObject<IEnumerable<TData>>(jsonData, JsonSerializerSettings) ?? throw new JsonSerializationException();
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="ReadDataAsync(CancellationTokenSource?)"/>
        /// <exception cref="NotSupportedException">Thrown when the type parameter is not supported.</exception>
        public async Task<IEnumerable<T>> ReadDataAsync<T>(CancellationTokenSource? cts) where T : class
        {
            if (typeof(T) == typeof(TData))
                return (await ReadDataAsync(cts)).Select(x => (x as T)!);
            else
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
        }
    }
}