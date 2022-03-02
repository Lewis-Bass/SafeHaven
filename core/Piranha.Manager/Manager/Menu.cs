﻿/*
 * Copyright (c) 2016-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Piranha.Manager
{
    /// <summary>
    /// Static class for defining the manager menu.
    /// </summary>
    public static class Menu
    {
        /// <summary>
        /// An item in the manager menu.
        /// </summary>
        public class MenuItem
        {
            /// <summary>
            /// Gets/sets the internal id.
            /// </summary>
            public string InternalId { get; set; }

            /// <summary>
            /// Gets/sets the display name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets/sets the optional css class.
            /// </summary>
            public string Css { get; set; }

            /// <summary>
            /// Gets/sets the manager controller.
            /// </summary>
            public string Controller { get; set; }

            /// <summary>
            /// Gets/sets the default action to invoke.
            /// </summary>
            public string Action { get; set; }

            /// <summary>
            /// Gets/sets the policy needed to see this item.
            /// </summary>
            public string Policy { get; set; }

            /// <summary>
            /// Gets/sets the optional menu item params.
            /// </summary>
            public object Params { get; set; }

            /// <summary>
            /// Gets/sets the available items.
            /// </summary>
            public MenuItemList Items { get; set; } = new MenuItemList();
        }

        public class MenuItemList : List<MenuItem>
        {
            /// <summary>
            /// Gets the menu item with the given internal id.
            /// </summary>
            public MenuItem this[string internalId] {
                get
                {
                    return this.FirstOrDefault(i => i.InternalId == internalId);
                }
            }
        }

        /// <summary>
        /// The basic manager menu.
        /// </summary>
        public static MenuItemList Items = new MenuItemList
        {
            new MenuItem
            {
                InternalId = "Content", Name = "Content", Css = "fas fa-pencil-alt", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Pages", Name = "Pages", Controller = "Page", Action = "List", Policy = Permission.Pages, Css = "fas fa-copy"
                    },
                    new MenuItem
                    {
                        InternalId = "Media", Name = "Media", Controller = "Media", Action = "List", Policy = Permission.Media, Css = "fas fa-images"
                    }
                }
            },
            new MenuItem
            {
                InternalId = "Settings", Name = "Settings", Css = "fas fa-wrench", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Aliases", Name = "Aliases", Controller = "Alias", Action = "List", Policy = Permission.Aliases, Css = "fas fa-random"
                    },
                    new MenuItem
                    {
                        InternalId = "Sites", Name = "Sites", Controller = "Site", Action = "List", Policy = Permission.Sites, Css = "fas fa-globe"
                    }
                }
            },
            new MenuItem
            {
                InternalId = "System", Name = "System", Css = "fas fa-cog", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Config", Name = "Config", Controller = "Config", Action = "Edit", Policy = Permission.Config, Css = "fas fa-cogs"
                    },
                    new MenuItem
                    {
                        InternalId = "Modules", Name = "Modules", Controller = "Module", Action = "List", Policy = Permission.Config, Css = "fas fa-code-branch"
                    }
                }
            }
        };

        /// <summary>
        /// Gets the applicable menu structure for the given user.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="auth">The authorization service</param>
        /// <returns></returns>
        public static async Task<MenuItemList> GetForUser(ClaimsPrincipal user, IAuthorizationService auth)
        {
            var result = new MenuItemList();

            foreach (var group in Items)
            {
                if (string.IsNullOrWhiteSpace(group.Policy) || (await auth.AuthorizeAsync(user, group.Policy)).Succeeded)
                {
                    var resultGroup = new MenuItem
                    {
                        InternalId = group.InternalId,
                        Name = group.Name,
                        Css = group.Css
                    };

                    foreach (var item in group.Items)
                    {
                        // Add the item if the given user has access to it
                        if (string.IsNullOrWhiteSpace(item.Policy) || (await auth.AuthorizeAsync(user, item.Policy)).Succeeded)
                        {
                            resultGroup.Items.Add(new MenuItem
                            {
                                InternalId = item.InternalId,
                                Name = item.Name,
                                Controller = item.Controller,
                                Action = item.Action,
                                Policy = item.Policy,
                                Css = item.Css
                            });
                        }
                    }

                    // Only add menu groups that contains items
                    if (resultGroup.Items.Count > 0)
                    {
                        result.Add(resultGroup);
                    }
                }
            }
            return result;
        }
    }
}