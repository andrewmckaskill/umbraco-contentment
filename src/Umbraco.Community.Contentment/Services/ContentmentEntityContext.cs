/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentEntityContext : IContentmentEntityContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestAccessor _requestAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentEntityContext(
            IHttpContextAccessor httpContextAccessor,
            IRequestAccessor requestAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _requestAccessor = requestAccessor;
        }

        public string? GetCurrentEntityId(out bool isParent)
        {
            isParent = false;

            // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
            var currentId = _requestAccessor.GetRequestValue("id");
            var parentId = _requestAccessor.GetRequestValue("parentId");
            if (!string.IsNullOrWhiteSpace(currentId))
            {
                return currentId;
            }
            else if (!string.IsNullOrWhiteSpace(parentId))
            {
                isParent = true;

                return parentId;
            }

            var json = _httpContextAccessor.HttpContext?.Request.GetRawBodyStringAsync().GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(json) == false)
            {
                var obj = JsonConvert.DeserializeAnonymousType(json, new { id = "", parentId = "" });
                if (!string.IsNullOrWhiteSpace(obj?.id))
                {
                    return obj.id;
                }
                else if (!string.IsNullOrWhiteSpace(obj?.parentId))
                {
                    isParent = true;
                    return obj.parentId;
                }
            }

            return default;
        }

    }
}
