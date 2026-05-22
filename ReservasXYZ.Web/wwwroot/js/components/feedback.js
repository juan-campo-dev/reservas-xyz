const NOTIFICATION_VARIANTS = {
    success: {
        title: 'Éxito',
        role: 'status',
        icon: `
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"></path>
            </svg>
        `
    },
    removed: {
        title: 'Eliminado',
        role: 'status',
        icon: `
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4m0 4h.01M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0Z"></path>
            </svg>
        `
    },
    warning: {
        title: 'Advertencia',
        role: 'alert',
        icon: `
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4m0 4h.01M10.29 3.86 1.82 18a2 2 0 0 0 1.72 3h16.92a2 2 0 0 0 1.72-3L13.71 3.86a2 2 0 0 0-3.42 0Z"></path>
            </svg>
        `
    },
    error: {
        title: 'Error',
        role: 'alert',
        icon: `
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" d="M6 6l12 12M18 6 6 18"></path>
            </svg>
        `
    },
    info: {
        title: 'Información',
        role: 'status',
        icon: `
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 16v-4m0-4h.01M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0Z"></path>
            </svg>
        `
    }
};

function removeToast(toast) {
    if (!(toast instanceof HTMLElement) || toast.dataset.notificationClosing === 'true') {
        return;
    }

    toast.dataset.notificationClosing = 'true';
    toast.classList.add('is-leaving');

    window.setTimeout(() => {
        const stack = toast.parentElement;
        toast.remove();

        if (stack instanceof HTMLElement && stack.dataset.notificationStack === 'dynamic' && stack.childElementCount === 0) {
            stack.remove();
        }
    }, 220);
}

function bindToastLifecycle(toast) {
    if (!(toast instanceof HTMLElement) || toast.dataset.notificationReady === 'true') {
        return;
    }

    toast.dataset.notificationReady = 'true';

    window.setTimeout(() => removeToast(toast), 4200);
}

function ensureNotificationStack() {
    const existingStack = document.querySelector('[data-notification-stack]');

    if (existingStack instanceof HTMLElement) {
        return existingStack;
    }

    const stack = document.createElement('div');
    stack.className = 'notification-stack';
    stack.dataset.notificationStack = 'dynamic';
    document.body.appendChild(stack);
    return stack;
}

function createToastElement(message, type) {
    const variant = NOTIFICATION_VARIANTS[type] || NOTIFICATION_VARIANTS.success;
    const toast = document.createElement('div');

    toast.className = `notification-toast notification-toast--${type}`;
    toast.dataset.notificationToast = 'true';
    toast.setAttribute('role', variant.role);
    toast.setAttribute('aria-live', 'polite');
    toast.innerHTML = `
        <div class="notification-toast__icon" aria-hidden="true">
            ${variant.icon}
        </div>
        <div class="notification-toast__content">
            <p class="notification-toast__title"></p>
            <p class="notification-toast__message"></p>
        </div>
    `;

    const title = toast.querySelector('.notification-toast__title');
    const body = toast.querySelector('.notification-toast__message');

    if (title instanceof HTMLElement) {
        title.textContent = variant.title;
    }

    if (body instanceof HTMLElement) {
        body.textContent = message;
    }

    return toast;
}

export function initStaticToasts() {
    document.querySelectorAll('[data-notification-toast]').forEach((toast) => bindToastLifecycle(toast));
}

export function showConfirmModal(message, onConfirm) {
    const modal = document.createElement('div');
    modal.className = 'fixed inset-0 z-50 flex items-center justify-center bg-black/50';
    modal.innerHTML = `
        <div class="mx-4 max-w-sm rounded-3xl bg-white p-6 shadow-xl">
            <p class="mb-4 font-medium text-gray-800">${message}</p>
            <div class="flex justify-end gap-3">
                <button class="rounded-xl border border-gray-300 px-4 py-2 text-gray-700 hover:bg-gray-50" id="modal-cancel">Cancelar</button>
                <button class="rounded-xl bg-red-500 px-4 py-2 text-white hover:bg-red-600" id="modal-confirm">Confirmar</button>
            </div>
        </div>
    `;

    document.body.appendChild(modal);

    modal.querySelector('#modal-cancel').onclick = () => modal.remove();
    modal.querySelector('#modal-confirm').onclick = () => {
        onConfirm();
        modal.remove();
    };
    modal.onclick = (event) => {
        if (event.target === modal) {
            modal.remove();
        }
    };
}

export function showToast(message, type = 'success') {
    const stack = ensureNotificationStack();
    const nextType = Object.prototype.hasOwnProperty.call(NOTIFICATION_VARIANTS, type) ? type : 'success';
    const toast = createToastElement(message, nextType);

    stack.appendChild(toast);
    bindToastLifecycle(toast);

    window.requestAnimationFrame(() => {
        toast.classList.add('is-visible');
    });
}

export function registerGlobalFeedback() {
    window.showConfirmModal = showConfirmModal;
    window.showToast = showToast;
}