export function initPasswordVisibilityToggles() {
    document.addEventListener('click', (event) => {
        const target = event.target;

        if (!(target instanceof Element)) {
            return;
        }

        const toggle = target.closest('[data-password-toggle]');

        if (!(toggle instanceof HTMLElement)) {
            return;
        }

        event.preventDefault();

        const inputId = toggle.getAttribute('data-password-toggle');

        if (!inputId) {
            return;
        }

        const targetInput = document.getElementById(inputId);

        if (!(targetInput instanceof HTMLInputElement)) {
            return;
        }

        const shouldRevealPassword = targetInput.type === 'password';
        targetInput.type = shouldRevealPassword ? 'text' : 'password';
        toggle.classList.toggle('is-revealed', shouldRevealPassword);
        toggle.setAttribute('aria-label', shouldRevealPassword ? 'Ocultar contraseña' : 'Ver contraseña');
        toggle.setAttribute('aria-pressed', shouldRevealPassword ? 'true' : 'false');
    });
}