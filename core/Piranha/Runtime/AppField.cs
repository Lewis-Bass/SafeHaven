/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Runtime
{
    public sealed class AppField : AppDataItem
    {
        /// <summary>
        /// Gets/sets the display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the shorthand name.
        /// </summary>
        public string Shorthand { get; set; }
    }
}
