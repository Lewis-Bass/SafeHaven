/*
 * Copyright (c) 2018 aatmmr
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;

namespace Piranha.AspNetCore.Identity.Postgress
{
    public class IdentityPostgressDb : Identity.Db<IdentityPostgressDb> 
    { 
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public IdentityPostgressDb(DbContextOptions<IdentityPostgressDb> options) : base(options) { }
    }
}