using Microsoft.EntityFrameworkCore;
using SKitLs.Data.IO.EfDbContext;

namespace SKitLs.Data.Core.IO.EfDbContext
{
    /// <summary>
    /// Provides functionality to write data to a database using Entity Framework Core.
    /// </summary>
    /// <typeparam name="TData">The type of entity to write.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DbWriter{TData, TId}"/> class with the specified database context.
    /// </remarks>
    /// <param name="context">The Entity Framework database context.</param>
    public class DbWriter<TData, TId>(DbContext context) : DbIOBase(context), IDataWriter<TData> where TData : ModelDso<TId> where TId : notnull, IEquatable<TId>, IComparable<TId>
    {
        /// <inheritdoc/>
        public string GetSourceName() => SourceName;

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(TData)"/>
        /// <exception cref="NotSupportedException">Thrown when the type <typeparamref name="T"/> does not match <typeparamref name="TData"/>.</exception>
        public bool WriteData<T>(T item) where T : class
        {
            if (typeof(T) == typeof(TData))
                return WriteData((item as TData)!);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool WriteData(TData item)
        {
            try
            {
                var dbSet = Context.Set<TData>();
                var existingEntity = dbSet.Find(item.GetId());
                if (existingEntity is null)
                {
                    dbSet.Add(item);
                }
                else
                {
                    Context.Entry(existingEntity).CurrentValues.SetValues(item);
                }
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteData(IEnumerable{TData})"/>
        /// <exception cref="NotSupportedException">Thrown when the type <typeparamref name="T"/> does not match <typeparamref name="TData"/>.</exception>
        public bool WriteData<T>(IEnumerable<T> items) where T : class
        {
            if (typeof(T) == typeof(TData) || items.FirstOrDefault()?.GetType() == typeof(TData))
                return WriteData(items.Select(x => (x as TData)!));
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool WriteData(IEnumerable<TData> items)
        {
            try
            {
                var dbSet = Context.Set<TData>();
                foreach (var item in items)
                {
                    var existingEntity = dbSet.Find(item.GetId());
                    if (existingEntity is null)
                    {
                        dbSet.Add(item);
                    }
                    else
                    {
                        Context.Entry(existingEntity).CurrentValues.SetValues(item);
                    }
                }
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsync(TData, CancellationTokenSource?)"/>
        /// <exception cref="NotSupportedException">Thrown when the type <typeparamref name="T"/> does not match <typeparamref name="TData"/>.</exception>
        public async Task<bool> WriteDataAsync<T>(T item, CancellationTokenSource? cts) where T : class
        {
            if (typeof(T) == typeof(TData))
                return await WriteDataAsync((item as TData)!, cts);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async Task<bool> WriteDataAsync(TData item, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                var dbSet = Context.Set<TData>();
                var existingEntity = dbSet.Find(item.GetId());
                if (existingEntity is null)
                {
                    dbSet.Add(item);
                }
                else
                {
                    Context.Entry(existingEntity).CurrentValues.SetValues(item);
                }
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                cts.Cancel();
                return false;
            }
        }

        /// <inheritdoc/>
        /// <inheritdoc cref="WriteDataAsync(IEnumerable{TData}, CancellationTokenSource?)"/>
        /// <exception cref="NotSupportedException">Thrown when the type <typeparamref name="T"/> does not match <typeparamref name="TData"/>.</exception>
        public async Task<bool> WriteDataAsync<T>(IEnumerable<T> items, CancellationTokenSource? cts) where T : class
        {
            if (typeof(T) == typeof(TData))
                return await WriteDataAsync(items.Select(x => (x as TData)!), cts);
            else
                throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async Task<bool> WriteDataAsync(IEnumerable<TData> items, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                var dbSet = Context.Set<TData>();
                foreach (var item in items)
                {
                    var existingEntity = dbSet.Find(item.GetId());
                    if (existingEntity is null)
                    {
                        dbSet.Add(item);
                    }
                    else
                    {
                        Context.Entry(existingEntity).CurrentValues.SetValues(item);
                    }
                }
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                cts.Cancel();
                return false;
            }
        }
    }
}