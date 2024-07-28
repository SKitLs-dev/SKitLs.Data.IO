﻿namespace SKitLs.Data.Core.IO
{
    /// <summary>
    /// Represents a data writer that provides methods for writing data to a specified source.
    /// </summary>
    public interface IDataWriter
    {
        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        /// <returns>The name of the data source.</returns>
        public string GetSourceName();

        /// <summary>
        /// Writes a single data item synchronously to the data source.
        /// </summary>
        /// <typeparam name="TData">The type of data to write.</typeparam>
        /// <param name="item">The data item to write.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public bool WriteData<TData>(TData item) where TData : class;

        /// <summary>
        /// Writes multiple data items synchronously to the data source.
        /// </summary>
        /// <typeparam name="TData">The type of data to write.</typeparam>
        /// <param name="items">The collection of data items to write.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public bool WriteDataList<TData>(IEnumerable<TData> items) where TData : class;

        /// <summary>
        /// Writes a single data item asynchronously to the data source.
        /// </summary>
        /// <typeparam name="TData">The type of data to write.</typeparam>
        /// <param name="item">The data item to write.</param>
        /// <param name="cts">Optional: cancellation token source to cancel the write operation.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public Task<bool> WriteDataAsync<TData>(TData item, CancellationTokenSource? cts) where TData : class;

        /// <summary>
        /// Writes multiple data items asynchronously to the data source.
        /// </summary>
        /// <typeparam name="TData">The type of data to write.</typeparam>
        /// <param name="items">The collection of data items to write.</param>
        /// <param name="cts">Optional: cancellation token source to cancel the write operation.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public Task<bool> WriteDataListAsync<TData>(IEnumerable<TData> items, CancellationTokenSource? cts) where TData : class;
    }
}