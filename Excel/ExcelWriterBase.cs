using OfficeOpenXml;
using SKitLs.Data.IO.Excel;

namespace SKitLs.Data.Core.IO.Excel
{
    // TODO Exceptions

    /// <summary>
    /// Base class for writing data to Excel files, providing synchronous and asynchronous operations.
    /// </summary>
    /// <typeparam name="TData">The type to which each row in the Excel file should be converted.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ExcelWriterBase{T}"/> class with the specified parameters.
    /// </remarks>
    /// <param name="dataPath">The path to the Excel file.</param>
    /// <param name="worksheetName">The name of the worksheet within the Excel file.</param>
    /// <param name="startRow">The starting row index for writing data.</param>
    /// <param name="startColumn">The starting column index for writing data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataPath"/> or <paramref name="worksheetName"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startRow"/> or <paramref name="startColumn"/> is less than or equal to 0.</exception>
    public abstract class ExcelWriterBase<TData>(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1) : ExcelIOBase(dataPath, worksheetName, startRow, startColumn), IDataWriter<ExcelPartRow> where TData : class
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <summary>
        /// Converts data of type <typeparamref name="TData"/> into an <see cref="ExcelPartRow"/>.
        /// </summary>
        /// <param name="data">The data of type <typeparamref name="TData"/> to convert.</param>
        /// <returns>An <see cref="ExcelPartRow"/> instance representing the converted data.</returns>
        public abstract ExcelPartRow Convert(TData data);

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(ExcelPartRow)"/>
        /// <exception cref="NotSupportedException">Thrown when writing data of type <typeparamref name="TData"/> is not supported.</exception>
        public virtual bool WriteData<T>(T item) where T : class
        {
            if (typeof(T) == typeof(TData))
                return WriteData(Convert((item as TData)!));
            else if (typeof(T) == typeof(ExcelPartRow))
                return WriteData((item as ExcelPartRow)!);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <exception cref="FileNotFoundException">Thrown when the Excel file specified by <see cref="ExcelIOBase.DataPath"/> does not exist.</exception>
        public virtual bool WriteData(ExcelPartRow value)
        {
            var sourceFile = new FileInfo(DataPath);

            if (!File.Exists(sourceFile.FullName))
                throw new FileNotFoundException();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets[WorksheetName];

            try
            {
                for (int j = 0; j < value.Count; j++)
                {
                    worksheet.Cells[value.RowIndex, StartColumn + j].Value = value[j];
                }
                package.Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(IEnumerable{ExcelPartRow})"/>
        /// <exception cref="NotSupportedException">Thrown when writing data of type <typeparamref name="TData"/> is not supported.</exception>
        public virtual bool WriteData<T>(IEnumerable<T> items) where T : class
        {
            if (typeof(T) == typeof(TData))
                return WriteData(items.Select(x => Convert((x as TData)!)).ToList());
            else if (typeof(T) == typeof(ExcelPartRow))
                return WriteData(items.Select(x => (x as ExcelPartRow)!).ToList());
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <exception cref="FileNotFoundException">Thrown when the Excel file specified by <see cref="ExcelIOBase.DataPath"/> does not exist.</exception>
        public virtual bool WriteData(IEnumerable<ExcelPartRow> values)
        {
            var sourceFile = new FileInfo(DataPath);

            if (!File.Exists(sourceFile.FullName))
                throw new FileNotFoundException();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets[WorksheetName];

            foreach (var part in values)
            {
                try
                {
                    for (int j = 0; j < part.Count; j++)
                    {
                        worksheet.Cells[part.RowIndex, StartColumn + j].Value = part[j];
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData{T}(T)"/>
        public virtual async Task<bool> WriteDataAsync<T>(T item, CancellationTokenSource? cts) where T : class
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(item);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(ExcelPartRow)"/>
        public virtual async Task<bool> WriteDataAsync(ExcelPartRow value, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(value);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData{T}(IEnumerable{T})"/>
        public virtual async Task<bool> WriteDataAsync<T>(IEnumerable<T> items, CancellationTokenSource? cts) where T : class
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(items);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(IEnumerable{ExcelPartRow})"/>
        public virtual async Task<bool> WriteDataAsync(IEnumerable<ExcelPartRow> values, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(values);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }
    }
}