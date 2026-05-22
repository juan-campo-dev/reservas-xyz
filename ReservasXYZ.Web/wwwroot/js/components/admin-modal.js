import { initCustomSelects } from './custom-select.js';

export class AdminModal {
    #overlay = null;
    #panel = null;
    #onCloseCallback = null;
    #previousOverflow = '';

    open(contentHtml, { wide = false, onClose = null } = {}) {
        this.close();
        this.#onCloseCallback = onClose;
        this.#previousOverflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';

        this.#overlay = document.createElement('div');
        this.#overlay.className = 'admin-modal-overlay';
        this.#overlay.innerHTML = `
            <div class="admin-modal-backdrop"></div>
            <div class="admin-modal-container">
                <div class="admin-modal-panel${wide ? ' admin-modal-panel--wide' : ''}"></div>
            </div>`;

        this.#panel = this.#overlay.querySelector('.admin-modal-panel');
        if (typeof contentHtml === 'string') {
            this.#panel.innerHTML = contentHtml;
        } else {
            this.#panel.appendChild(contentHtml);
        }

        this.#overlay.querySelector('.admin-modal-backdrop')
            .addEventListener('click', () => this.close());

        document.addEventListener('keydown', this.#handleKey);
        document.body.appendChild(this.#overlay);

        initCustomSelects();

        const firstInput = this.#panel.querySelector('input, select, textarea');
        if (firstInput) requestAnimationFrame(() => firstInput.focus());
    }

    close() {
        if (!this.#overlay) return;
        document.removeEventListener('keydown', this.#handleKey);
        document.body.style.overflow = this.#previousOverflow;
        this.#overlay.remove();
        this.#overlay = null;
        this.#panel = null;
        if (this.#onCloseCallback) this.#onCloseCallback();
        this.#onCloseCallback = null;
    }

    get panel() {
        return this.#panel;
    }

    get isOpen() {
        return !!this.#overlay;
    }

    #handleKey = (e) => {
        if (e.key === 'Escape') this.close();
    };
}

export const adminModal = new AdminModal();

export class AdminConfirm {
    static show({ title, description, confirmLabel = 'Eliminar', cancelLabel = 'Cancelar', variant = 'danger' }) {
        return new Promise((resolve) => {
            const modal = new AdminModal();

            const iconSvg = variant === 'danger'
                ? '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>'
                : '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>';

            const html = `
                <div class="admin-confirm-body">
                    <div class="admin-confirm-icon admin-confirm-icon--${variant}">${iconSvg}</div>
                    <h3 class="admin-confirm-title">${title}</h3>
                    <p class="admin-confirm-desc">${description}</p>
                    <div class="admin-confirm-actions">
                        <button type="button" class="admin-modal-btn admin-modal-btn--cancel" data-action="cancel">${cancelLabel}</button>
                        <button type="button" class="admin-modal-btn admin-modal-btn--${variant}" data-action="confirm">${confirmLabel}</button>
                    </div>
                </div>`;

            modal.open(html, {
                onClose: () => resolve(false)
            });

            modal.panel.querySelector('[data-action="cancel"]')
                .addEventListener('click', () => { modal.close(); resolve(false); });

            modal.panel.querySelector('[data-action="confirm"]')
                .addEventListener('click', () => { modal.close(); resolve(true); });
        });
    }
}
