/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EditorNotesConfigurationConnector : IDataTypeConfigurationConnector
    {
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private readonly ILocalLinkParser _localLinkParser;
        private readonly IImageSourceParser _imageSourceParser;
        private readonly IMacroParser _macroParser;

        public IEnumerable<string> PropertyEditorAliases => new[] { EditorNotesDataEditor.DataEditorName };

        public EditorNotesConfigurationConnector(
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
            ILocalLinkParser localLinkParser,
            IImageSourceParser imageSourceParser,
            IMacroParser macroParser)
        {
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
            _localLinkParser = localLinkParser;
            _imageSourceParser = imageSourceParser;
            _macroParser = macroParser;
        }

        public object? FromArtifact(IDataType dataType, string? configuration)
            => FromArtifact(dataType, configuration, PassThroughCache.Instance);

        public object? FromArtifact(IDataType dataType, string? configuration, IContextCache contextCache)
        {
            var dataTypeConfigurationEditor = dataType.Editor?.GetConfigurationEditor();

            var db = dataTypeConfigurationEditor?.FromDatabase(configuration, _configurationEditorJsonSerializer);

            if (db is Dictionary<string, object> config &&
                config.TryGetValueAs(EditorNotesConfigurationEditor.Message, out string? notes) == true &&
                string.IsNullOrWhiteSpace(notes) == false)
            {
                notes = _localLinkParser.FromArtifact(notes);
                notes = _imageSourceParser.FromArtifact(notes);
                notes = _macroParser.FromArtifact(notes);

                config[EditorNotesConfigurationEditor.Message] = notes ?? string.Empty;

                return config;
            }

            return db;
        }

        public string? ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies)
            => ToArtifact(dataType, dependencies, PassThroughCache.Instance);

        public string? ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache)
        {
            if (dataType.ConfigurationObject is Dictionary<string, object> config &&
                config.TryGetValueAs(EditorNotesConfigurationEditor.Message, out string? notes) == true &&
                string.IsNullOrWhiteSpace(notes) == false)
            {
                var udis = new List<Udi>();

                notes = _localLinkParser.ToArtifact(notes, udis);
                notes = _imageSourceParser.ToArtifact(notes, udis);
                notes = _macroParser.ToArtifact(notes, udis);

                foreach (var udi in udis)
                {
                    var mode = udi.EntityType == UmbConstants.UdiEntityType.Macro
                        ? ArtifactDependencyMode.Match
                        : ArtifactDependencyMode.Exist;

                    dependencies.Add(new ArtifactDependency(udi, false, mode));
                }

                config[EditorNotesConfigurationEditor.Message] = notes ?? string.Empty;
            }

            var dataTypeConfigurationEditor = dataType.Editor?.GetConfigurationEditor();
            return dataTypeConfigurationEditor?.ToDatabase(dataType.ConfigurationData, _configurationEditorJsonSerializer);
        }
    }
}
