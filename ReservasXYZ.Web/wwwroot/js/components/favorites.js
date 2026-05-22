const getAntiForgeryToken = () => {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput instanceof HTMLInputElement ? tokenInput.value : '';
};

const setFavoriteButtonState = (button, isFavorite) => {
    const label = isFavorite ? 'Quitar de favoritos' : 'Agregar a favoritos';

    button.classList.toggle('is-favorite', isFavorite);
    button.setAttribute('aria-pressed', isFavorite ? 'true' : 'false');
    button.setAttribute('aria-label', label);
    button.setAttribute('title', label);

    const hiddenLabel = button.querySelector('.sr-only');
    if (hiddenLabel instanceof HTMLElement) {
        hiddenLabel.textContent = label;
    }
};

const showFavoriteFeedback = (message, type = 'success') => {
    if (typeof window.showToast === 'function') {
        window.showToast(message, type);
    }
};

export function initRoomFavoriteButtons() {
    if (document.documentElement.dataset.roomFavoritesReady === 'true') {
        return;
    }

    document.documentElement.dataset.roomFavoritesReady = 'true';

    document.addEventListener('click', async (event) => {
        const target = event.target;

        if (!(target instanceof Element)) {
            return;
        }

        const button = target.closest('[data-room-favorite]');

        if (!(button instanceof HTMLButtonElement)) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();

        if (button.dataset.favoriteBusy === 'true') {
            return;
        }

        const roomId = button.dataset.roomId;
        const token = getAntiForgeryToken();

        if (!roomId || !token) {
            showFavoriteFeedback('No se pudo validar la solicitud.', 'error');
            return;
        }

        button.dataset.favoriteBusy = 'true';
        button.disabled = true;

        try {
            const response = await fetch(button.dataset.favoriteUrl || '/Favorites/ToggleRoom', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: new URLSearchParams({ roomId })
            });

            if (response.status === 401) {
                showFavoriteFeedback('Inicia sesión para guardar favoritos.', 'info');
                return;
            }

            if (!response.ok) {
                const error = await response.json().catch(() => null);
                throw new Error(error?.message || 'No se pudo actualizar favoritos.');
            }

            const result = await response.json();
            const isFavorite = Boolean(result.isFavorite);

            setFavoriteButtonState(button, isFavorite);
            showFavoriteFeedback(
                isFavorite ? 'Guardado en favoritos.' : 'Eliminado de favoritos.',
                isFavorite ? 'success' : 'removed'
            );
        } catch (error) {
            showFavoriteFeedback(error instanceof Error ? error.message : 'No se pudo actualizar favoritos.', 'error');
        } finally {
            button.disabled = false;
            delete button.dataset.favoriteBusy;
        }
    }, true);
}