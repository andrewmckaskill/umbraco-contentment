﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Hosting;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
#else
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class JsonDataListSource : DataListToDataPickerSourceBridge, IDataListSource, IContentmentListTemplateItem
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IIOHelper _ioHelper;

#if NET472
        private readonly ILogger _logger;

        public JsonDataListSource(
            ILogger logger,
            IWebHostEnvironment webHostEnvironment,
            IIOHelper ioHelper)
#else
        private readonly ILogger<JsonDataListSource> _logger;

        public JsonDataListSource(
            ILogger<JsonDataListSource> logger,
            IWebHostEnvironment webHostEnvironment,
            IIOHelper ioHelper)
#endif
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _ioHelper = ioHelper;
        }

        public override string Name => "JSON Data";

        public string NameTemplate => default;

        public override string Description => "Configure JSON data to populate the data source.";

        public string DescriptionTemplate => "{{ url }}";

        public override string Icon => "icon-brackets";

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(_ioHelper, $@"<details class=""well well-small"">
<summary><strong>Do you need help with JSONPath expressions?</strong></summary>
<p>This data-source uses Newtonsoft's Json.NET library, with this we are limited to extracting only the 'value' from any key/value-pairs.</p>
<p>If you need assistance with JSONPath syntax, please refer to this resource: <a href=""https://goessner.net/articles/JsonPath/"" target=""_blank""><strong>goessner.net/articles/JsonPath</strong></a>.</p>
<hr>
<p><em>If you are a developer and have ideas on how to extract the <code>key</code> (name) from the items, please do let me know on <a href=""{Constants.Package.RepositoryUrl}/issues/40"" target=""_blank""><strong>GitHub issue: #40</strong></a>.</em></p>
</details>", true),
            new ConfigurationField
            {
                Key = "url",
                Name = "URL",
                Description = "Enter the URL of the JSON data source.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "itemsJsonPath",
                Name = "Items JSONPath",
                Description = "Enter the JSONPath expression to select the items from the JSON data source.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "nameJsonPath",
                Name = "Name JSONPath",
                Description = "Enter the JSONPath expression to select the name from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "valueJsonPath",
                Name = "Value JSONPath",
                Description = "Enter the JSONPath expression to select the value (key) from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "iconJsonPath",
                Name = "Icon JSONPath",
                Description = "<em>(optional)</em> Enter the JSONPath expression to select the icon from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "descriptionJsonPath",
                Name = "Description JSONPath",
                Description = "<em>(optional)</em> Enter the JSONPath expression to select the description from the item.",
                View = "textstring",
            },
        };

        public override Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { "url", "https://leekelleher.com/umbraco/contentment/data.json" },
            { "itemsJsonPath", "$[*]" },
            { "nameJsonPath", "$.name" },
            { "valueJsonPath", "$.value" },
            { "iconJsonPath", "$.icon" },
            { "descriptionJsonPath", "$.description" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var url = config.GetValueAs("url", string.Empty);

            if (string.IsNullOrWhiteSpace(url) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var json = GetJson(url);

            if (json == null)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var itemsJsonPath = config.GetValueAs("itemsJsonPath", string.Empty);

            if (string.IsNullOrWhiteSpace(itemsJsonPath) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

            try
            {
                var tokens = json.SelectTokens(itemsJsonPath);

                if (tokens.Any() == false)
                {
#if NET472
                    _logger.Warn<JsonDataListSource>($"The JSONPath '{itemsJsonPath}' did not match any items in the JSON.");
#else
                    _logger.LogWarning($"The JSONPath '{itemsJsonPath}' did not match any items in the JSON.");
#endif
                    return Enumerable.Empty<DataListItem>();
                }

                // TODO: [UP-FOR-GRABS] How would you get the string-value from a "key"?
                // This project https://github.com/s3u/JSONPath supports "~" to retrieve keys. However this is not in the original jsonpath-specs.
                // We could implement something similar, which checks the JsonPaths for a ~, and the we'll code-extract the keys. However this is a somewhat shady solution.

                var nameJsonPath = config.GetValueAs("nameJsonPath", string.Empty);
                var valueJsonPath = config.GetValueAs("valueJsonPath", string.Empty);
                var iconJsonPath = config.GetValueAs("iconJsonPath", string.Empty);
                var descriptionJsonPath = config.GetValueAs("descriptionJsonPath", string.Empty);

                var items = new List<DataListItem>();

                foreach (var token in tokens)
                {
                    var name = token.SelectToken(nameJsonPath);
                    var value = token.SelectToken(valueJsonPath);

                    var icon = string.IsNullOrEmpty(iconJsonPath) == false
                        ? token.SelectToken(iconJsonPath)
                        : null;

                    var description = string.IsNullOrEmpty(descriptionJsonPath) == false
                        ? token.SelectToken(descriptionJsonPath)
                        : null;

                    // How should we log if either name or value is empty? Note that empty or missing values are totally legal according to json
                    if (name == null)
                    {
#if NET472
                        _logger.Warn<JsonDataListSource>($"The JSONPath '{nameJsonPath}' did not match a 'name' in the item JSON.");
#else
                        _logger.LogWarning($"The JSONPath '{nameJsonPath}' did not match a 'name' in the item JSON.");
#endif
                    }

                    // If value is missing we'll skip this specific item and log as a warning
                    if (value == null)
                    {
#if NET472
                        _logger.Warn<JsonDataListSource>($"The JSONPath '{valueJsonPath}' did not match a 'value' in the item XML. The item was skipped.");
#else
                        _logger.LogWarning($"The JSONPath '{valueJsonPath}' did not match a 'value' in the item XML. The item was skipped.");
#endif
                        continue;
                    }

                    items.Add(new DataListItem
                    {
                        Name = name?.ToString() ?? string.Empty,
                        Value = value?.ToString() ?? string.Empty,
                        Icon = icon?.ToString() ?? string.Empty,
                        Description = description?.ToString() ?? string.Empty
                    });
                }

                return items;
            }
            catch (Exception ex)
            {
#if NET472
                // Error finding items in the JSON. Please check the syntax of your JSONPath expressions.
                _logger.Error<JsonDataListSource>(ex, "Error finding items in the JSON. Please check the syntax of your JSONPath expressions.");
#else
                _logger.LogError(ex, "Error finding items in the JSON. Please check the syntax of your JSONPath expressions.");
#endif
            }

            return Enumerable.Empty<DataListItem>();
        }

        private JToken GetJson(string url)
        {
            var content = string.Empty;

            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                try
                {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    using (var client = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        content = client.DownloadString(url);
                    }
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                }
                catch (WebException ex)
                {
#if NET472
                    _logger.Error<JsonDataListSource>(ex, $"Unable to fetch remote data from URL: {url}");
#else
                    _logger.LogError(ex, $"Unable to fetch remote data from URL: {url}");
#endif
                }
            }
            else
            {
                // assume local file
                var path = _webHostEnvironment.MapPathWebRoot(url);
                if (File.Exists(path) == true)
                {
                    content = File.ReadAllText(path);
                }
                else
                {
#if NET472
                    _logger.Error<JsonDataListSource>(new FileNotFoundException(), $"Unable to find the local file path: {url}");
#else
                    _logger.LogError(new FileNotFoundException(), $"Unable to find the local file path: {url}");
#endif
                    return null;
                }
            }

            if (string.IsNullOrWhiteSpace(content) == true)
            {
#if NET472
                _logger.Warn<JsonDataListSource>($"The contents of '{url}' was empty. Unable to process JSON data.");
#else
                _logger.LogWarning($"The contents of '{url}' was empty. Unable to process JSON data.");
#endif

                return default;
            }

            try
            {
                // Deserialize to a JToken, for general purposes.
                // Inspiration taken from StackOverflow: https://stackoverflow.com/a/38560188/12787
                return JToken.Parse(content);
            }
            catch (Exception ex)
            {
                var trimmed = content.Substring(0, Math.Min(400, content.Length));
#if NET472
                _logger.Error<JsonDataListSource>(ex, $"Error parsing string to JSON: {trimmed}");
#else
                _logger.LogError(ex, $"Error parsing string to JSON: {trimmed}");
#endif
            }

            return default;
        }
    }
}
