using OfficeOpenXml;
using SKitLs.Data.IO.Excel;
using System.Collections;

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
        public virtual bool WriteData(ExcelPartRow value) => WriteDataList([value]);

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataList(IEnumerable{ExcelPartRow})"/>
        /// <exception cref="NotSupportedException">Thrown when writing data of type <typeparamref name="TData"/> is not supported.</exception>
        public virtual bool WriteDataList<T>(IEnumerable<T> items) where T : class
        {
            if (typeof(T) == typeof(TData))
                return WriteDataList(items.Select(x => Convert((x as TData)!)).ToList());
            else if (typeof(T) == typeof(ExcelPartRow))
                return WriteDataList(items.Select(x => (x as ExcelPartRow)!).ToList());
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <exception cref="FileNotFoundException">Thrown when the Excel file specified by <see cref="ExcelIOBase.DataPath"/> does not exist.</exception>
        public virtual bool WriteDataList(IEnumerable<ExcelPartRow> values)
        {
            try
            {
                var sourceFile = new FileInfo(DataPath);

                if (!File.Exists(sourceFile.FullName))
                    throw new FileNotFoundException();

                using var package = new ExcelPackage(sourceFile);
                var worksheet = package.Workbook.Worksheets[WorksheetName];

                if (worksheet is null)
                {
                    if (CreateNewList)
                    {
                        worksheet = package.Workbook.Worksheets.Add(WorksheetName);
                    }
                    else
                    {
                        // Output the names of all worksheets in the workbook for debugging purposes
                        var worksheetNames = package.Workbook.Worksheets.Select(ws => ws.Name);
                        throw new NullReferenceException($"Worksheet not found: {WorksheetName}. Available worksheets: {string.Join(", ", worksheetNames)}");
                    }
                }

                foreach (var part in values)
                {
                    for (int j = 0; j < part.Count; j++)
                    {
                        worksheet.Cells[part.RowIndex, StartColumn + j].Value = part[j];
                    }
                }
                package.Save();
            }
            catch (Exception)
            {
                if (!HandleInnerExceptions)
                    throw;
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData{T}(T)"/>
        public virtual async Task<bool> WriteDataAsync<T>(T item, CancellationTokenSource? cts) where T : class
        {
            cts ??= new();
            if (typeof(T) == typeof(TData))
                return await WriteDataAsync(Convert((item as TData)!), cts);
            else if (typeof(T) == typeof(ExcelPartRow))
                return await WriteDataAsync((item as ExcelPartRow)!, cts);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(ExcelPartRow)"/>
        public virtual async Task<bool> WriteDataAsync(ExcelPartRow value, CancellationTokenSource? cts) => await WriteDataListAsync([value], cts);

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataList{T}(IEnumerable{T})"/>
        public virtual async Task<bool> WriteDataListAsync<T>(IEnumerable<T> items, CancellationTokenSource? cts) where T : class
        {
            cts ??= new();
            if (typeof(T) == typeof(TData))
                return await WriteDataListAsync(items.Select(x => Convert((x as TData)!)).ToList(), cts);
            else if (typeof(T) == typeof(ExcelPartRow))
                return await WriteDataListAsync(items.Select(x => (x as ExcelPartRow)!).ToList(), cts);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataList(IEnumerable{ExcelPartRow})"/>
        public virtual async Task<bool> WriteDataListAsync(IEnumerable<ExcelPartRow> values, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                var sourceFile = new FileInfo(DataPath);

                if (!File.Exists(sourceFile.FullName))
                    throw new FileNotFoundException();

                using var package = new ExcelPackage(sourceFile);
                var worksheet = package.Workbook.Worksheets[WorksheetName];

                if (worksheet is null)
                {
                    if (CreateNewList)
                    {
                        worksheet = package.Workbook.Worksheets.Add(WorksheetName);
                    }
                    else
                    {
                        // Output the names of all worksheets in the workbook for debugging purposes
                        var worksheetNames = package.Workbook.Worksheets.Select(ws => ws.Name);
                        throw new NullReferenceException($"Worksheet not found: {WorksheetName}. Available worksheets: {string.Join(", ", worksheetNames)}");
                    }
                }

                foreach (var part in values)
                {
                    for (int j = 0; j < part.Count; j++)
                    {
                        worksheet.Cells[part.RowIndex, StartColumn + j].Value = part[j];
                    }
                }
                await package.SaveAsync();
            }
            catch (Exception)
            {
                cts.Cancel();
                if (!HandleInnerExceptions)
                    throw;
                return false;
            }
            return true;
        }
    }
}