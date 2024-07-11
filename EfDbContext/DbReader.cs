using Microsoft.EntityFrameworkCore;
using SKitLs.Data.IO.EfDbContext;

namespace SKitLs.Data.Core.IO.EfDbContext
{
    /// <summary>
    /// Provides functionality to read data from a database using Entity Framework Core.
    /// </summary>
    /// <typeparam name="TData">The type of entity to read.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DbReader{T}"/> class with the specified database context.
    /// </remarks>
    /// <param name="context">The Entity Framework database context.</param>
    public class DbReader<TData>(DbContext context) : DbIOBase(context), IDataReader<TData> where TData : class
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <inheritdoc/>
        public IEnumerable<TData> ReadData()
        {
            return Context.Set<TData>();
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Thrown when attempting to read unsupported data type.</exception>
        public IEnumerable<T> ReadData<T>() where T : class
        {
            if (typeof(T) == typeof(TData))
                return ReadData().Select(x => (x as T)!);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="ReadData()"/>
        public async Task<IEnumerable<TData>> ReadDataAsync(CancellationTokenSource? cts = null)
        {
            cts ??= new CancellationTokenSource();
            try
            {
                return await Task.FromResult(ReadData());
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="ReadDataAsync{TData}(CancellationTokenSource?)"/>
        public async Task<IEnumerable<T>> ReadDataAsync<T>(CancellationTokenSource? cts = null) where T : class
        {
            cts ??= new CancellationTokenSource();
            try
            {
                return await Task.FromResult(ReadData<T>());
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }
    }
}