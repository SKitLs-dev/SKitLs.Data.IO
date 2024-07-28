namespace SKitLs.Data.IO.Excel
{
    /// <summary>
    /// Base class for handling common properties and methods related to Excel I/O operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ExcelIOBase"/> class with the specified parameters.
    /// </remarks>
    /// <param name="dataPath">The path to the Excel file.</param>
    /// <param name="worksheetName">The name of the worksheet within the Excel file.</param>
    /// <param name="startRow">The starting row index for reading data.</param>
    /// <param name="startColumn">The starting column index for reading data.</param>
    /// <param name="endColumn">The ending column index for reading data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataPath"/> or <paramref name="worksheetName"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startRow"/>, <paramref name="startColumn"/>, <paramref name="endColumn"/> is less than or equal to 0.</exception>
    public class ExcelIOBase(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1, int endColumn = 100)
    {
        /// <summary>
        /// Gets or sets the data separator used for joining data fields.
        /// </summary>
        public static string DataSep { get; set; } = ";";

        /// <summary>
        /// Gets or sets the row separator used for separating rows.
        /// </summary>
        public static string RowSep { get; set; } = "\n";

        /// <summary>
        /// Gets or sets the name of the data source, defaulting to "Excel File".
        /// </summary>
        public static string SourceName { get; set; } = "Excel File";

        /// <summary>
        /// Gets or sets the path to the Excel file.
        /// </summary>
        public string DataPath { get; set; } = dataPath ?? throw new ArgumentNullException(nameof(dataPath));

        /// <summary>
        /// Gets or sets the name of the worksheet within the Excel file.
        /// </summary>
        public string WorksheetName { get; set; } = worksheetName ?? throw new ArgumentNullException(nameof(worksheetName));

        /// <summary>
        /// Gets or sets the starting row index for reading/writing data from the worksheet.
        /// </summary>
        public int StartRow { get; set; } = startRow > 0 ? startRow : throw new ArgumentOutOfRangeException(nameof(startRow));

        /// <summary>
        /// Gets or sets the starting column index for reading/writing data from the worksheet.
        /// </summary>
        public int StartColumn { get; set; } = startColumn > 0 ? startColumn : throw new ArgumentOutOfRangeException(nameof(startColumn));

        /// <summary>
        /// Gets or sets the ending column index for reading data from the worksheet.
        /// </summary>
        public int EndColumn { get; set; } = endColumn > 0 ? endColumn : throw new ArgumentOutOfRangeException(nameof(endColumn));

        /// <summary>
        /// Gets or sets a value indicating whether to create a new list if the specified list does not exist.
        /// </summary>
        public bool CreateNewList { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to handle inner exceptions during operations.
        /// </summary>
        public bool HandleInnerExceptions { get; set; } = false;
    }
}