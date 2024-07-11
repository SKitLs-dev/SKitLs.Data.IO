﻿namespace SKitLs.Data.Core.IO.Excel
{
    /// <summary>
    /// Represents a row of data in an Excel-like structure, identified by its row index and column range.
    /// </summary>
    public class ExcelPartRow
    {
        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Gets or sets the starting index of the first column.
        /// </summary>
        public int StartColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the ending index of the last column.
        /// </summary>
        public int EndColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the list of values in the row.
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelPartRow"/> class with specified row and column indices.
        /// </summary>
        /// <param name="row">The index of the row.</param>
        /// <param name="startIndex">The starting index of the first column.</param>
        /// <param name="endIndex">The ending index of the last column.</param>
        public ExcelPartRow(int row, int startIndex, int endIndex)
        {
            RowIndex = row;
            StartColumnIndex = startIndex;
            EndColumnIndex = endIndex;
            Values = [];
        }

        /// <summary>
        /// Gets or sets the value at the specified column index within the row.
        /// </summary>
        /// <param name="index">The index of the column.</param>
        /// <returns>The value at the specified column index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of range.</exception>
        public string this[int index]
        {
            get
            {
                var relativeIndex = index - StartColumnIndex;
                return relativeIndex >= 0 && relativeIndex < Values.Count ? Values[relativeIndex] : throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Adds a value to the row.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(string value) => Values.Add(value);

        /// <summary>
        /// Gets the number of values in the row.
        /// </summary>
        public int Count => Values.Count;

        /// <summary>
        /// Checks if all values in the row are empty or null.
        /// </summary>
        /// <returns><see langword="true"/> if all values are empty; otherwise, <see langword="false"/>.</returns>
        public bool IsEmpty() => Values.All(x => string.IsNullOrEmpty(x));

        /// <summary>
        /// Joins all values in the row into a single string, separated by the specified separator.
        /// </summary>
        /// <param name="sep">The separator to use (default is ";").</param>
        /// <returns>A string that contains all values joined by the separator.</returns>
        public string Join(string? sep = ";") => string.Join(sep, Values);
    }
}