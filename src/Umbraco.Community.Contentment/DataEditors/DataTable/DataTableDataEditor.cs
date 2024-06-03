/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataTableDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataTable";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data Table";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "data-table.html";
        internal const string DataEditorIcon = "icon-item-arrangement";

        // TODO: [LK] This property-editor UI needs to be developed.
        internal const string DataEditorUiAlias = "Umb.Contentment.PropertyEditorUi.ReadOnly";
    }
}
