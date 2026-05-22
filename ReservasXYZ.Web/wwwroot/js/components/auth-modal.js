import { isAuthModalUrl } from '../core/auth-routes.js';
import { setBodyScrollLock } from '../core/scroll-lock.js';
import { refreshOutputStylesheetOnLocalhost } from '../core/stylesheet.js';
import { decorateAuthCard } from './modal-card.js';

export function initDirectAuthCard() {
    const authBody = document.querySelector('[data-auth-modal-body]');
    const closeUrl = authBody?.getAttribute('data-auth-close-url');

    if (!(authBody instanceof HTMLElement) || !closeUrl) {
        return;
    }

    decorateAuthCard(authBody, { href: closeUrl });
}

export function initAuthModal() {
    const modalState = {
        overlay: null,
        panel: null,
        activeTrigger: null,
        lastFocusedElement: null,
        currentUrl: null
    };

    const resolveFormSubmitUrl = (form) => {
        const explicitAction = form.getAttribute('action');
        if (explicitAction && explicitAction.trim() !== '') {
            return form.action;
        }
        if (modalState.currentUrl) {
            return modalState.currentUrl;
        }
        return window.location.href;
    };

    const closeAuthModal = () => {
        if (!(modalState.overlay instanceof HTMLElement) || !(modalState.panel instanceof HTMLElement)) {
            return;
        }

        modalState.overlay.classList.add('hidden');
        modalState.panel.innerHTML = '';
        modalState.currentUrl = null;
        setBodyScrollLock(false);

        if (modalState.activeTrigger instanceof HTMLElement) {
            modalState.activeTrigger.focus();
        } else if (modalState.lastFocusedElement instanceof HTMLElement) {
            modalState.lastFocusedElement.focus();
        }

        modalState.activeTrigger = null;
    };

    const ensureModal = () => {
        if (modalState.overlay instanceof HTMLElement && modalState.panel instanceof HTMLElement) {
            return;
        }

        const overlay = document.createElement('div');
        overlay.id = 'auth-modal-overlay';
        overlay.className = 'fixed inset-0 z-[90] hidden';
        overlay.innerHTML = `
            <div class="absolute inset-0 bg-[rgba(9,12,18,0.62)] backdrop-blur-[10px]" data-auth-modal-backdrop></div>
            <div class="relative flex min-h-screen items-center justify-center overflow-y-auto px-4 py-6 sm:px-6 lg:px-8">
                <div class="relative z-[1] flex w-full justify-center" data-auth-modal-panel></div>
            </div>
        `;

        document.body.appendChild(overlay);

        const panel = overlay.querySelector('[data-auth-modal-panel]');

        if (!(panel instanceof HTMLElement)) {
            overlay.remove();
            return;
        }

        modalState.overlay = overlay;
        modalState.panel = panel;

        overlay.addEventListener('click', (event) => {
            if (event.target === overlay || (event.target instanceof Element && event.target.hasAttribute('data-auth-modal-backdrop'))) {
                closeAuthModal();
            }
        });

        document.addEventListener('keydown', (event) => {
            if (event.key === 'Escape' && modalState.overlay && !modalState.overlay.classList.contains('hidden')) {
                closeAuthModal();
            }
        });
    };

    const renderLoadingState = () => {
        if (!(modalState.panel instanceof HTMLElement)) {
            return;
        }

        modalState.panel.innerHTML = `
            <div class="compute-card w-full max-w-[438px] rounded-3xl p-6 lg:p-7">
                <div class="flex items-center justify-center gap-3 py-12 text-sm text-muted-foreground">
                    <span class="h-2.5 w-2.5 animate-pulse rounded-full bg-foreground/30"></span>
                    <span>Cargando...</span>
                </div>
            </div>
        `;
    };

    const extractAuthBody = (htmlText) => {
        const parser = new DOMParser();
        const documentFragment = parser.parseFromString(htmlText, 'text/html');
        return documentFragment.querySelector('[data-auth-modal-body]');
    };

    const bindAuthModalInteractions = () => {
        if (!(modalState.panel instanceof HTMLElement)) {
            return;
        }

        modalState.panel.querySelectorAll('a[href]').forEach((link) => {
            if (!(link instanceof HTMLAnchorElement) || !isAuthModalUrl(link.href)) {
                return;
            }

            link.addEventListener('click', (event) => {
                event.preventDefault();
                loadAuthModal(link.href, link);
            });
        });

        modalState.panel.querySelectorAll('form').forEach((form) => {
            if (!(form instanceof HTMLFormElement)) {
                return;
            }

            form.addEventListener('submit', (event) => {
                event.preventDefault();
                submitAuthModalForm(form);
            });
        });
    };

    const updateAuthModalContent = (htmlText) => {
        const authBody = extractAuthBody(htmlText);

        if (!(authBody instanceof HTMLElement) || !(modalState.panel instanceof HTMLElement)) {
            return false;
        }

        modalState.panel.innerHTML = authBody.innerHTML;
        decorateAuthCard(modalState.panel, { onClick: closeAuthModal });
        bindAuthModalInteractions();
        return true;
    };

    const loadAuthModal = async (url, trigger = null) => {
        ensureModal();

        if (!(modalState.overlay instanceof HTMLElement) || !(modalState.panel instanceof HTMLElement)) {
            return;
        }

        refreshOutputStylesheetOnLocalhost();

        if (trigger instanceof HTMLElement) {
            modalState.activeTrigger = trigger;
        }

        modalState.lastFocusedElement = document.activeElement instanceof HTMLElement ? document.activeElement : null;
        modalState.overlay.classList.remove('hidden');
        setBodyScrollLock(true);
        renderLoadingState();

        try {
            const response = await fetch(url, {
                method: 'GET',
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const htmlText = await response.text();

            if (!response.ok || !updateAuthModalContent(htmlText)) {
                window.location.href = new URL(url, window.location.origin).toString();
                return;
            }

            modalState.currentUrl = response.url || new URL(url, window.location.origin).toString();

            const firstFocusable = modalState.panel.querySelector('input, button, a[href], select, textarea');
            firstFocusable?.focus();
        } catch (error) {
            window.location.href = new URL(url, window.location.origin).toString();
        }
    };

    const submitAuthModalForm = async (form) => {
        const submitButton = form.querySelector('[type="submit"]');
        const originalLabel = submitButton instanceof HTMLButtonElement ? submitButton.innerHTML : '';

        if (submitButton instanceof HTMLButtonElement) {
            submitButton.disabled = true;
            submitButton.innerHTML = 'Procesando...';
        }

        const submitUrl = resolveFormSubmitUrl(form);

        try {
            const response = await fetch(submitUrl, {
                method: (form.method || 'POST').toUpperCase(),
                body: new FormData(form),
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response.redirected) {
                const redirectUrl = response.url;

                if (isAuthModalUrl(redirectUrl)) {
                    await loadAuthModal(redirectUrl);
                    return;
                }

                window.location.href = redirectUrl;
                return;
            }

            const htmlText = await response.text();

            if (!response.ok || !updateAuthModalContent(htmlText)) {
                window.location.href = submitUrl;
            } else {
                modalState.currentUrl = response.url || submitUrl;
            }
        } catch (error) {
            window.location.href = submitUrl;
        } finally {
            if (submitButton instanceof HTMLButtonElement && document.body.contains(submitButton)) {
                submitButton.disabled = false;
                submitButton.innerHTML = originalLabel;
            }
        }
    };

    document.addEventListener('click', (event) => {
        const target = event.target;

        if (!(target instanceof Element)) {
            return;
        }

        const trigger = target.closest('a[href]');

        if (!(trigger instanceof HTMLAnchorElement) || !isAuthModalUrl(trigger.href)) {
            return;
        }

        event.preventDefault();
        loadAuthModal(trigger.href, trigger);
    });

    const authPageParam = new URLSearchParams(window.location.search).get('authPage');

    if (authPageParam) {
        history.replaceState(null, '', window.location.pathname);
        loadAuthModal(authPageParam);
    }
}