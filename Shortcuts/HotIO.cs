using Newtonsoft.Json;

namespace SKitLs.Data.IO.Shortcuts
{
    /// <summary>
    /// Provides utility methods for saving and loading data to and from files.
    /// </summary>
    [Obsolete("Beta")]
    public static class HotIO
    {
        /// <summary>
        /// Gets or sets the JSON files extensions. Use <see langword="null"/> to prevent from automatic Json path fit.
        /// </summary>
        public static string? JsonExtension { get; set; } = ".json";

        private static string FitJsonPath(string path)
        {
            if (JsonExtension is not null && path.EndsWith(JsonExtension))
                path += JsonExtension;
            return path;
        }

        // TODO

        /// <summary>
        /// Gets or sets the JSON serializer settings used for serialization.
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings { get; set; } = new();

        /// <summary>
        /// Saves an object to a specified file in JSON format.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="path">The path to the file where the object will be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="IOException">Thrown when there is an error writing to the file.</exception>
        [Obsolete("Beta")]
        public static void SaveJson(object obj, string path)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentException.ThrowIfNullOrEmpty(path);
            
            try
            {
                path = FitJsonPath(path);
                var jsonData = JsonConvert.SerializeObject(obj, JsonSerializerSettings);
                File.WriteAllText(path, jsonData);
            }
            catch (Exception ex)
            {
                throw new IOException($"An error occurred while saving JSON data to file: {path}", ex);
            }
        }

        /// <summary>
        /// Asynchronously saves an object to a specified file in JSON format.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="path">The path to the file where the object will be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="IOException">Thrown when there is an error writing to the file.</exception>
        [Obsolete("Beta")]
        public static async Task SaveJsonAsync(object obj, string path)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentException.ThrowIfNullOrEmpty(path);
            
            try
            {
                path = FitJsonPath(path);
                var jsonData = JsonConvert.SerializeObject(obj, JsonSerializerSettings);
                await File.WriteAllTextAsync(path, jsonData);
            }
            catch (Exception ex)
            {
                throw new IOException($"An error occurred while saving JSON data to file: {path}", ex);
            }
        }

        /// <summary>
        /// Loads an object from a specified file in JSON format.
        /// </summary>
        /// <param name="path">The path to the file where the object is saved.</param>
        /// <returns>The deserialized object from the file.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading from the file.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error deserializing the JSON data.</exception>
        [Obsolete("Beta")]
        public static object LoadJson(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            try
            {
                path = FitJsonPath(path);
                if (!File.Exists(path))
                    throw new FileNotFoundException($"The file specified does not exist: {path}");

                string jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject(jsonData, JsonSerializerSettings) ?? throw new JsonSerializationException($"Unable to deserialize object.");
            }
            catch (Exception ex) when (ex is IOException || ex is JsonSerializationException)
            {
                throw new IOException($"An error occurred while loading JSON data from file: {path}", ex);
            }
        }

        /// <summary>
        /// Asynchronously loads an object from a specified file in JSON format.
        /// </summary>
        /// <param name="path">The path to the file where the object is saved.</param>
        /// <returns>The deserialized object from the file.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading from the file.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error deserializing the JSON data.</exception>
        [Obsolete("Beta")]
        public static async Task<object> LoadJsonAsync(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            try
            {
                path = FitJsonPath(path);
                if (!File.Exists(path))
                    throw new FileNotFoundException($"The file specified does not exist: {path}");

                string jsonData = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject(jsonData, JsonSerializerSettings) ?? throw new JsonSerializationException($"Unable to deserialize object.");
            }
            catch (Exception ex) when (ex is IOException || ex is JsonSerializationException)
            {
                throw new IOException($"An error occurred while loading JSON data from file: {path}", ex);
            }
        }

        /// <summary>
        /// Loads an object of a specified type from a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to load.</typeparam>
        /// <param name="path">The path to the file where the object is saved.</param>
        /// <returns>The deserialized object from the file.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading from the file.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error deserializing the JSON data.</exception>
        [Obsolete("Beta")]
        public static T LoadJson<T>(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            try
            {
                path = FitJsonPath(path);
                if (!File.Exists(path))
                    throw new FileNotFoundException($"The file specified does not exist: {path}");

                var jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(jsonData, JsonSerializerSettings) ?? throw new JsonSerializationException($"Unable to deserialize object.");
            }
            catch (Exception ex) when (ex is IOException || ex is JsonSerializationException)
            {
                throw new IOException($"An error occurred while loading JSON data from file: {path}", ex);
            }
        }

        /// <summary>
        /// Asynchronously loads an object of a specified type from a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to load.</typeparam>
        /// <param name="path">The path to the file where the object is saved.</param>
        /// <returns>The deserialized object from the file.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading from the file.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an error deserializing the JSON data.</exception>
        [Obsolete("Beta")]
        public static async Task<T> LoadJsonAsync<T>(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            try
            {
                path = FitJsonPath(path);
                if (!File.Exists(path))
                    throw new FileNotFoundException($"The file specified does not exist: {path}");

                var jsonDataLines = await File.ReadAllLinesAsync(path);
                return JsonConvert.DeserializeObject<T>(string.Join("\n", jsonDataLines), JsonSerializerSettings) ?? throw new JsonSerializationException($"Unable to deserialize object.");
            }
            catch (Exception ex) when (ex is IOException || ex is JsonSerializationException)
            {
                throw new IOException($"An error occurred while loading JSON data from file: {path}", ex);
            }
        }

        /// <summary>
        /// Converts an object to its JSON string representation.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <returns>A JSON string representation of the object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
        /// <exception cref="IOException">Thrown when there is an error during serialization.</exception>
        [Obsolete("Beta")]
        public static string Json<T>(this T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            try
            {
                return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                throw new IOException("An error occurred while serializing the object to JSON.", ex);
            }
        }

        /// <summary>
        /// Saves a string to a specified file.
        /// </summary>
        /// <param name="text">The text to save.</param>
        /// <param name="path">The path to the file where the text will be saved.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="IOException">Thrown when there is an error writing to the file.</exception>
        [Obsolete("Beta")]
        public static void Save(this string text, string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            try
            {
                File.WriteAllText(path, text);
            }
            catch (Exception ex)
            {
                throw new IOException($"An error occurred while saving data to file: {path}", ex);
            }
        }

        /// <summary>
        /// Asynchronously saves a string to a specified file.
        /// </summary>
        /// <param name="text">The text to save.</param>
        /// <param name="path">The path to the file where the text will be saved.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="IOException">Thrown when there is an error writing to the file.</exception>
        [Obsolete("Beta")]
        public static async Task SaveAsync(this string text, string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            try
            {
                await File.WriteAllTextAsync(path, text);
            }
            catch (Exception ex)
            {
                throw new IOException($"An error occurred while saving data to file: {path}", ex);
            }
        }
    }
}