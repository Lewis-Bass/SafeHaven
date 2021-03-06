/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using Piranha.Areas.Manager.Models;

namespace Piranha.Manager.Binders
{
    /// <summary>
    /// Binder provider for handling abstract types.
    /// </summary>
    public class AbstractModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Gets the correct binder for the context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The model binder</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (context != null) {
                // We only care about regions, blocks and fields
                if (context.Metadata.ModelType == typeof(PageEditRegionBase) || context.Metadata.ModelType == typeof(Extend.IField) || context.Metadata.ModelType == typeof(Extend.Block)) {
                    var binders = new Dictionary<string, AbstractBinderType>();

                    var metadata = context.MetadataProvider.GetMetadataForType(typeof(PageEditRegion));
                    binders.Add(typeof(PageEditRegion).FullName, new AbstractBinderType() {
                        Type = typeof(PageEditRegion),
                        Binder = context.CreateBinder(metadata)
                    });

                    metadata = context.MetadataProvider.GetMetadataForType(typeof(PageEditRegionCollection));
                    binders.Add(typeof(PageEditRegionCollection).FullName, new AbstractBinderType() {
                        Type = typeof(PageEditRegionCollection),
                        Binder = context.CreateBinder(metadata)
                    });

                    foreach (var fieldType in App.Fields) {
                        metadata = context.MetadataProvider.GetMetadataForType(fieldType.Type);
                        binders.Add(fieldType.TypeName, new AbstractBinderType() {
                            Type = fieldType.Type,
                            Binder = context.CreateBinder(metadata)
                        });
                    }

                    foreach (var blockType in App.Blocks) {
                        metadata = context.MetadataProvider.GetMetadataForType(blockType.Type);
                        binders.Add(blockType.TypeName, new AbstractBinderType() {
                            Type = blockType.Type,
                            Binder = context.CreateBinder(metadata)
                        });                        
                    }
                    return new AbstractModelBinder(context.MetadataProvider, binders);
                }
                return null;
            }
            throw new ArgumentNullException(nameof(context));
        }
    }
}
