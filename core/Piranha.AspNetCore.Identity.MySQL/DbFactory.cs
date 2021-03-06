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

namespace Piranha.AspNetCore.Identity.MySQL
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<IdentityMySQLDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public IdentityMySQLDb CreateDbContext(string[] args) 
        {
            var builder = new DbContextOptionsBuilder<IdentityMySQLDb>();
            builder.UseMySql("server=localhost;port=3306;database=piranha;uid=root;password=password");
            return new IdentityMySQLDb(builder.Options);
        }
    }
}
#endif