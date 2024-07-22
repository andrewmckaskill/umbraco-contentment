// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';

const ELEMENT_NAME = 'contentment-property-editor-ui-content-source';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIContentSourceElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ type: Object })
	public value?: unknown;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
	}

	#onChange(event: CustomEvent & { target: { data: unknown } }) {
		this.value = event.target.data ?? {};
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		return html`
			<umb-input-content-picker-document-root .data=${this.value} @change=${this.#onChange}>
			</umb-input-content-picker-document-root>
		`;
	}
}

export { ContentmentPropertyEditorUIContentSourceElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIContentSourceElement;
	}
}
