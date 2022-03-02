/*
 * Copyright (c) 2016-2018 Billy Wolfington, Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Areas.Manager.Controllers
{
    public abstract class ManagerAreaControllerBase : MessageControllerBase
    {
        /// <summary>
        /// The current api
        /// </summary>
        protected readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        protected ManagerAreaControllerBase(IApi api)
        {
            _api = api;
        }
    }
}