/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Extend
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FieldTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the optional shorthand for type declaration.
        /// </summary>
        public string Shorthand { get; set; }
    }
}
