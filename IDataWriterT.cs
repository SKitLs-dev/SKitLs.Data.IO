namespace SKitLs.Data.Core.IO
{
    /// <summary>
    /// Interface for a generic data writer that reads data of type <typeparamref name="TData"/>.
    /// </summary>
    /// <typeparam name="TData">The type of data to read.</typeparam>
    public interface IDataWriter<TData> : IDataWriter where TData : class
    {
        /// <summary>
        /// Writes a single data item synchronously to the data source.
        /// </summary>
        /// <param name="item">The data item to write.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public bool WriteData(TData item);

        /// <summary>
        /// Writes multiple data items synchronously to the data source.
        /// </summary>
        /// <param name="items">The collection of data items to write.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public bool WriteData(IEnumerable<TData> items);

        /// <summary>
        /// Writes a single data item asynchronously to the data source.
        /// </summary>
        /// <param name="item">The data item to write.</param>
        /// <param name="cts">The optional cancellation token source to cancel the write operation.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public Task<bool> WriteDataAsync(TData item, CancellationTokenSource? cts);

        /// <summary>
        /// Writes multiple data items asynchronously to the data source.
        /// </summary>
        /// <param name="items">The collection of data items to write.</param>
        /// <param name="cts">The optional cancellation token source to cancel the write operation.</param>
        /// <returns><see langword="true"/> if the write operation was successful; otherwise, <see langword="false"/>.</returns>
        public Task<bool> WriteDataAsync(IEnumerable<TData> items, CancellationTokenSource? cts);
    }
}