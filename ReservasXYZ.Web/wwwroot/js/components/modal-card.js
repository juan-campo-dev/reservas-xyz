export function createAuthCardCloseControl({ href, onClick } = {}) {
    const closeControl = document.createElement(href ? 'a' : 'button');
    closeControl.className = 'absolute right-4 top-4 z-10 inline-flex h-8 w-8 cursor-pointer items-center justify-center p-0 text-muted-foreground transition hover:text-foreground focus:outline-none';
    closeControl.setAttribute('aria-label', 'Cerrar');
    closeControl.setAttribute('data-auth-card-close', 'true');
    closeControl.innerHTML = `
        <svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" class="h-4 w-4">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 6l12 12M18 6L6 18"></path>
        </svg>
    `;

    if (href) {
        closeControl.href = href;
    } else {
        closeControl.type = 'button';
    }

    if (typeof onClick === 'function') {
        closeControl.addEventListener('click', (event) => {
            event.preventDefault();
            onClick();
        });
    }

    return closeControl;
}

export function decorateAuthCard(container, closeConfig = {}) {
    if (!(container instanceof Element)) {
        return null;
    }

    const card = container.querySelector('.compute-card, .app-modal-card');

    if (!(card instanceof HTMLElement) || card.querySelector('[data-auth-card-close]')) {
        return null;
    }

    card.classList.add('relative');
    card.classList.add('pr-14');

    const closeControl = createAuthCardCloseControl(closeConfig);
    card.prepend(closeControl);
    return closeControl;
}