using OfficeOpenXml;
using SKitLs.Data.IO.Excel;

namespace SKitLs.Data.Core.IO.Excel
{
    // TODO Exceptions
    // TODO <inheritdoc> exceptions

    /// <summary>
    /// Base class for reading data from Excel files, providing synchronous and asynchronous operations.
    /// </summary>
    /// <typeparam name="TData">The type to which each row in the Excel file should be converted.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ExcelReaderBase{T}"/> class with the specified parameters.
    /// </remarks>
    /// <param name="dataPath">The path to the Excel file.</param>
    /// <param name="worksheetName">The name of the worksheet within the Excel file.</param>
    /// <param name="startRow">The starting row index for reading data.</param>
    /// <param name="startColumn">The starting column index for reading data.</param>
    /// <param name="endColumn">The ending column index for reading data.</param>
    /// <param name="emptyRowsBreakHit">The maximum number of consecutive empty rows to encounter before stopping.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataPath"/> or <paramref name="worksheetName"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startRow"/>, <paramref name="startColumn"/>, <paramref name="endColumn"/>, or <paramref name="emptyRowsBreakHit"/> is less than or equal to 0.</exception>
    public abstract class ExcelReaderBase<TData>(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1, int endColumn = 100, int emptyRowsBreakHit = 3) : ExcelIOBase(dataPath, worksheetName, startRow, startColumn, endColumn), IDataReader<ExcelPartRow> where TData : class
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <summary>
        /// Gets or sets the maximum number of consecutive empty rows to encounter before stopping.
        /// </summary>
        public int EmptyRowsBreakHit { get; set; } = emptyRowsBreakHit > 0 ? emptyRowsBreakHit : throw new ArgumentOutOfRangeException(nameof(emptyRowsBreakHit));

        /// <summary>
        /// Converts an <see cref="ExcelPartRow"/> instance into the desired type <typeparamref name="TData"/>.
        /// </summary>
        /// <param name="row">The <see cref="ExcelPartRow"/> instance to convert.</param>
        /// <returns>The converted object of type <typeparamref name="TData"/>.</returns>
        public abstract TData Convert(ExcelPartRow row);

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Thrown when conversion to <typeparamref name="T"/> is not implemented.</exception>
        public virtual IEnumerable<T> ReadData<T>() where T : class
        {
            if (typeof(T) == typeof(string))
            {
                var read = ReadData();
                return read.Select(x => (x.Join(DataSep) as T)!);
            }
            else if (typeof(T) == typeof(TData))
            {
                var read = ReadData();
                return read.Select(x => (Convert(x) as T)!);
            }
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <exception cref="NotImplementedException">Thrown when conversion to <typeparamref name="T"/> is not implemented.</exception>
        public virtual async Task<IEnumerable<T>> ReadDataAsync<T>(CancellationTokenSource? cts = default) where T : class
        {
            if (typeof(T) == typeof(string))
            {
                var read = await ReadDataAsync(cts);
                return read.Select(x => (x.Join(DataSep) as T)!);
            }
            else if (typeof(T) == typeof(TData))
            {
                var read = await ReadDataAsync();
                return read.Select(x => (Convert(x) as T)!);
            }
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <exception cref="FileNotFoundException">Thrown when the Excel file specified by <see cref="ExcelIOBase.DataPath"/> does not exist.</exception>
        public virtual IEnumerable<ExcelPartRow> ReadData()
        {
            var result = new List<ExcelPartRow>();
            var sourceFile = new FileInfo(DataPath);

            if (!File.Exists(sourceFile.FullName))
                throw new FileNotFoundException();

            using var package = new ExcelPackage(sourceFile);
            var worksheet = package.Workbook.Worksheets[WorksheetName];

            var emptyCounter = 0;
            for (int row_i = StartRow; row_i <= worksheet.Dimension.End.Row; row_i++)
            {
                var row = new ExcelPartRow(row_i, StartColumn, EndColumn);
                for (int j = StartColumn; j <= EndColumn; j++)
                {
                    row.Add(worksheet.Cells[row_i, j].Text);
                }

                if (row.IsEmpty())
                {
                    emptyCounter++;
                }
                else
                {
                    emptyCounter = 0;
                    result.Add(row);
                }
                if (emptyCounter > EmptyRowsBreakHit)
                    break;
            }
            return result;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="ReadData()"/>
        public virtual async Task<IEnumerable<ExcelPartRow>> ReadDataAsync(CancellationTokenSource? cts = default)
        {
            cts ??= new();
            try
            {
                return await Task.Run(ReadData);
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }
    }
}