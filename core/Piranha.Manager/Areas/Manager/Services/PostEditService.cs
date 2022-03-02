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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Piranha.Areas.Manager.Models;
using Piranha.Manager;
using Piranha.Services;

namespace Piranha.Areas.Manager.Services
{
    /// <summary>
    /// The page edit service.
    /// </summary>
    public class PostEditService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        public PostEditService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        /// <summary>
        /// Saves the post model.
        /// </summary>
        /// <param name="model">The post edit model</param>
        /// <param name="alias">The suggested alias</param>
        /// <param name="publish">If the post should be published</param>
        /// <returns>If the post was successfully saved</returns>
        public bool Save(PostEditModel model, out string alias, bool? publish = null)
        {
            var post = _api.Posts.GetById(model.Id);
            alias = null;

            if (post == null)
            {
                post = Piranha.Models.DynamicPost.Create(_api, model.TypeId);
            }
            else
            {
                if (model.Slug != post.Slug && model.Published.HasValue)
                {
                    alias = post.Slug;
                }
            }

            Module.Mapper.Map<PostEditModel, Piranha.Models.PostBase>(model, post);
            SaveRegions(model, post);
            SaveBlocks(model, post);

            // Update category
            var category = _api.Posts.GetCategoryBySlug(model.BlogId, model.SelectedCategory);
            if (category != null)
            {
                post.Category = category;
            }
            else
            {
                post.Category = new Piranha.Models.Taxonomy
                {
                    Title = model.SelectedCategory
                };
            }

            // Update tags
            post.Tags.RemoveAll(t => !model.SelectedTags.Contains(t.Slug));

            foreach (var selectedTag in model.SelectedTags)
            {
                if (!post.Tags.Any(t => t.Slug == selectedTag))
                {
                    var tag = _api.Posts.GetTagBySlug(model.BlogId, selectedTag);

                    if (tag != null)
                    {
                        post.Tags.Add(new Piranha.Models.Taxonomy
                        {
                            Id = tag.Id,
                            Title = tag.Title,
                            Slug = tag.Slug
                        });
                    }
                    else
                    {
                        post.Tags.Add(new Piranha.Models.Taxonomy
                        {
                            Title = selectedTag
                        });
                    }
                }
            }

            if (publish.HasValue)
            {
                if (publish.Value && !post.Published.HasValue)
                {
                    post.Published = DateTime.Now;
                }
                else if (!publish.Value)
                {
                    post.Published = null;
                }
            }
            _api.Posts.Save(post);
            model.Id = post.Id;

            return true;
        }

        /// <summary>
        /// Gets the edit model for the post with the given id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="id">The post id</param>
        /// <returns>The post model</returns>
        public PostEditModel GetById(Guid id)
        {
            var post = _api.Posts.GetById(id);
            if (post != null)
            {
                var page = _api.Pages.GetById(post.BlogId);
                var model = Module.Mapper.Map<Piranha.Models.PostBase, PostEditModel>(post);
                model.PostType = _api.PostTypes.GetById(model.TypeId);
                model.AllCategories = _api.Posts.GetAllCategories(post.BlogId);
                model.AllTags = _api.Posts.GetAllTags(post.BlogId);
                model.SelectedCategory = post.Category.Slug;
                model.SelectedTags = post.Tags.Select(t => t.Slug).ToList();
                model.BlogSlug = page.Slug;

                LoadRegions(post, model);
                LoadBlocks(post, model);

                return model;
            }
            throw new KeyNotFoundException($"No post found with the id '{id}'");
        }

        /// <summary>
        /// Refreshes the model after an unsuccessful save.
        /// </summary>
        public PostEditModel Refresh(PostEditModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.TypeId))
            {
                model.PostType = _api.PostTypes.GetById(model.TypeId);
                model.AllCategories = _api.Posts.GetAllCategories(model.BlogId);
                model.AllTags = _api.Posts.GetAllTags(model.BlogId);
            }
            return model;
        }

        /// <summary>
        /// Creates a new edit model with the given post typeparamref.
        /// </summary>
        /// <param name="postTypeId">The post type id</param>
        /// <param name="blogId">The blog id</param>
        /// <returns>The post edit model</returns>
        public PostEditModel Create(string postTypeId, Guid blogId)
        {
            var type = _api.PostTypes.GetById(postTypeId);
            var page = _api.Pages.GetById(blogId);

            if (type != null && page != null)
            {
                var post = Piranha.Models.DynamicPost.Create(_api, postTypeId);
                var model = Module.Mapper.Map<Piranha.Models.PostBase, PostEditModel>(post);
                model.BlogId = blogId;
                model.PostType = type;
                model.AllCategories = _api.Posts.GetAllCategories(blogId);
                model.AllTags = _api.Posts.GetAllTags(blogId);
                model.BlogSlug = page.Slug;

                LoadRegions(post, model);

                return model;
            }
            throw new KeyNotFoundException($"No post type found with the id '{postTypeId}'");
        }

        /// <summary>
        /// Creates a new edit region model for the given region type and value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <param name="value">The region value</param>
        /// <returns>The edit model</returns>
        public PageEditRegionBase CreateRegion(Piranha.Models.RegionType region, object value)
        {
            PageEditRegionBase editRegion;

            if (region.Collection)
            {
                editRegion = new PageEditRegionCollection();
            }
            else
            {
                editRegion = new PageEditRegion();
            }
            editRegion.Id = region.Id;
            editRegion.Title = region.Title ?? region.Id;
            editRegion.CLRType = editRegion.GetType().FullName;
            editRegion.Description = region.Description;

            IList items = new List<object>();

            if (region.Collection)
            {
                items = (IList)value;
            }
            else
            {
                items.Add(value);
            }

            foreach (var item in items)
            {
                if (region.Fields.Count == 1)
                {
                    var itemTitle = "";

                    // Get the item title if this is a collection region.
                    if (region.Collection)
                    {
                        if (item != null)
                        {
                            itemTitle = ((Extend.IField)item).GetTitle();
                        }

                        if (string.IsNullOrWhiteSpace(itemTitle) && !string.IsNullOrWhiteSpace(region.ListTitlePlaceholder))
                        {
                            itemTitle = region.ListTitlePlaceholder;
                        }
                        else
                        {
                            itemTitle = "Item";
                        }
                    }

                    var set = new PageEditFieldSet
                    {
                        new PageEditField
                        {
                            Id = region.Fields[0].Id,
                            Title = region.Fields[0].Title ?? region.Fields[0].Id,
                            CLRType = item.GetType().FullName,
                            Options = region.Fields[0].Options,
                            Placeholder = region.Fields[0].Placeholder,
                            Value = (Extend.IField)item
                        }
                    };
                    set.ListTitle = itemTitle;
                    set.NoExpand = !region.ListExpand;

                    editRegion.Add(set);
                }
                else
                {
                    var fieldData = (IDictionary<string, object>)item;
                    var fieldSet = new PageEditFieldSet();

                    foreach (var field in region.Fields)
                    {
                        if (fieldData.ContainsKey(field.Id))
                        {
                            // Get the item title if this is a collection region.
                            if (region.Collection)
                            {
                                if (!string.IsNullOrWhiteSpace(region.ListTitleField) && field.Id == region.ListTitleField)
                                {
                                    var itemTitle = "";

                                    if (fieldData[field.Id] != null)
                                    {
                                        itemTitle = ((Extend.IField)fieldData[field.Id]).GetTitle();
                                    }

                                    if (string.IsNullOrWhiteSpace(itemTitle) && !string.IsNullOrWhiteSpace(region.ListTitlePlaceholder))
                                    {
                                        itemTitle = region.ListTitlePlaceholder;
                                    }
                                    else if (string.IsNullOrWhiteSpace(itemTitle))
                                    {
                                        itemTitle = "Item";
                                    }

                                    fieldSet.ListTitle = itemTitle;
                                    fieldSet.NoExpand = !region.ListExpand;
                                }
                            }

                            fieldSet.Add(new PageEditField
                            {
                                Id = field.Id,
                                Title = field.Title ?? field.Id,
                                CLRType = fieldData[field.Id].GetType().FullName,
                                Options = field.Options,
                                Placeholder = field.Placeholder,
                                Value = (Extend.IField)fieldData[field.Id]
                            });
                        }
                    }
                    editRegion.Add(fieldSet);
                }
            }
            return editRegion;
        }

        /// <summary>
        /// Loads all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void LoadRegions(Piranha.Models.DynamicPost src, PostEditModel dest)
        {
            if (dest.PostType != null)
            {
                foreach (var region in dest.PostType.Regions)
                {
                    var regions = (IDictionary<string, object>)src.Regions;

                    if (regions.ContainsKey(region.Id))
                    {
                        var editRegion = CreateRegion(region, regions[region.Id]);
                        dest.Regions.Add(editRegion);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the available blocks from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void LoadBlocks(Piranha.Models.DynamicPost src, PostEditModel dest)
        {
            foreach (var srcBlock in src.Blocks)
            {
                var block = new ContentEditBlock
                {
                    Id = srcBlock.Id,
                    CLRType = srcBlock.GetType().FullName,
                    IsGroup = typeof(Extend.BlockGroup).IsAssignableFrom(srcBlock.GetType()),
                    Value = srcBlock
                };

                if (typeof(Extend.BlockGroup).IsAssignableFrom(srcBlock.GetType()))
                {
                    foreach (var subBlock in ((Extend.BlockGroup)srcBlock).Items)
                    {
                        block.Items.Add(new ContentEditBlock
                        {
                            Id = subBlock.Id,
                            CLRType = subBlock.GetType().FullName,
                            Value = subBlock
                        });
                    }
                }
                dest.Blocks.Add(block);
            }
        }

        private void SaveBlocks(PostEditModel src, Piranha.Models.DynamicPost dest)
        {
            // Clear the block list
            dest.Blocks.Clear();

            // And rebuild it
            foreach (var srcBlock in src.Blocks)
            {
                dest.Blocks.Add(srcBlock.Value);
                
                if (typeof(Extend.BlockGroup).IsAssignableFrom(srcBlock.Value.GetType()))
                {
                    foreach (var subBlock in srcBlock.Items)
                    {
                        ((Extend.BlockGroup)srcBlock.Value).Items.Add(subBlock.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Saves all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private void SaveRegions(PostEditModel src, Piranha.Models.DynamicPost dest)
        {
            var type = _api.PostTypes.GetById(src.TypeId);
            var modelRegions = (IDictionary<string, object>)dest.Regions;

            foreach (var region in src.Regions)
            {
                if (region is PageEditRegion)
                {
                    if (!modelRegions.ContainsKey(region.Id))
                    {
                        modelRegions[region.Id] = _factory.CreateDynamicRegion(type, region.Id);
                    }

                    var reg = (PageEditRegion)region;

                    if (reg.FieldSet.Count == 1)
                    {
                        modelRegions[region.Id] = reg.FieldSet[0].Value;
                    }
                    else
                    {
                        var modelFields = (IDictionary<string, object>)modelRegions[region.Id];

                        foreach (var field in reg.FieldSet)
                        {
                            modelFields[field.Id] = field.Value;
                        }
                    }
                }
                else
                {
                    if (modelRegions.ContainsKey(region.Id))
                    {
                        var list = (Piranha.Models.IRegionList)modelRegions[region.Id];
                        var reg = (PageEditRegionCollection)region;

                        // At this point we clear the values and rebuild them
                        list.Clear();

                        foreach (var set in reg.FieldSets)
                        {
                            if (set.Count == 1)
                            {
                                list.Add(set[0].Value);
                            }
                            else
                            {
                                var modelFields = (IDictionary<string, object>)_factory.CreateDynamicRegion(type, region.Id);

                                foreach (var field in set)
                                {
                                    modelFields[field.Id] = field.Value;
                                }
                                list.Add(modelFields);
                            }
                        }
                    }
                }
            }
        }
    }
}
