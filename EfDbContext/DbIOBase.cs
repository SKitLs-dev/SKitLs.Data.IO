using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKitLs.Data.IO.EfDbContext
{
    /// <summary>
    /// Base class providing access to a database context for data input and output operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DbIOBase"/> class with the specified database context.
    /// </remarks>
    /// <param name="context">The Entity Framework database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    public class DbIOBase(DbContext context)
    {
        /// <summary>
        /// Gets or sets the name of the data source, defaulting to "DataBase Context".
        /// </summary>
        public static string SourceName { get; set; } = "DataBase Context";

        /// <summary>
        /// Gets or sets the Entity Framework database context.
        /// </summary>
        public DbContext Context { get; set; } = context ?? throw new ArgumentNullException(nameof(context));
    }
}