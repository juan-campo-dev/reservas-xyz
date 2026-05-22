export function initProfileMenus() {
    const menus = document.querySelectorAll('[data-profile-menu]');

    if (!menus.length) {
        return;
    }

    const closeMenu = (menu) => {
        if (menu instanceof HTMLDetailsElement) {
            menu.removeAttribute('open');
        }
    };

    menus.forEach((menu) => {
        menu.addEventListener('toggle', () => {
            if (!(menu instanceof HTMLDetailsElement) || !menu.open) {
                return;
            }

            menus.forEach((otherMenu) => {
                if (otherMenu !== menu) {
                    closeMenu(otherMenu);
                }
            });
        });
    });

    document.addEventListener('click', (event) => {
        if (!(event.target instanceof Node)) {
            return;
        }

        menus.forEach((menu) => {
            if (!menu.contains(event.target)) {
                closeMenu(menu);
            }
        });
    });

    document.addEventListener('keydown', (event) => {
        if (event.key !== 'Escape') {
            return;
        }

        menus.forEach(closeMenu);
    });
}