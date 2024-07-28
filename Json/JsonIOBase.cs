using Newtonsoft.Json;

namespace SKitLs.Data.IO.Json
{
    /// <summary>
    /// Provides a base class for JSON input/output operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JsonIOBase"/> class with the specified data path.
    /// </remarks>
    /// <param name="dataPath">The path to the JSON file.</param>
    /// <param name="serializerSettings">The JSON serializer settings used for serialization.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataPath"/> is null.</exception>
    public class JsonIOBase(string dataPath, JsonSerializerSettings? serializerSettings = null)
    {
        /// <summary>
        /// Gets or sets the default JSON serializer settings used for serialization.
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets the name of the data source, defaulting to "Json File".
        /// </summary>
        public static string SourceName { get; set; } = "Json File";

        /// <summary>
        /// Gets or sets the path to the JSON file.
        /// </summary>
        public string DataPath { get; set; } = dataPath ?? throw new ArgumentNullException(nameof(dataPath));

        /// <summary>
        /// Gets or sets the JSON serializer settings used for serialization.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = serializerSettings ?? DefaultJsonSerializerSettings;

        /// <summary>
        /// Gets or sets a value indicating whether to create a new file if the specified file does not exist.
        /// </summary>
        public bool CreateNewFile { get; set; } = false;
    }
}