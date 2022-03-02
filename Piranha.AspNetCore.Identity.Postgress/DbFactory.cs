#if DEBUG
/*
 * Copyright (c) 2018 aatmmr
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Piranha.AspNetCore.Identity.Postgress
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<IdentityPostgressDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public IdentityPostgressDb CreateDbContext(string[] args) 
        {
            var builder = new DbContextOptionsBuilder<IdentityPostgressDb>();
            builder.UseNpgsql("Host=127.0.0.1;Database=SaveHaven;Username=postgres;Password=2conjoKids");
            return new IdentityPostgressDb(builder.Options);
        }
    }
}
#endif