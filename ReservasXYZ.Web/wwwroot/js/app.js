import { authRoutePrefixes } from './core/auth-routes.js';
import { initAuthModal, initDirectAuthCard } from './components/auth-modal.js';
import { initCustomSelects } from './components/custom-select.js';
import { initDateRangePickers } from './components/date-range.js';
import { registerGlobalFeedback, initStaticToasts } from './components/feedback.js';
import { initGuestPanels } from './components/guest-panel.js';
import { initPasswordVisibilityToggles } from './components/password-toggle.js';
import { initProfileMenus } from './components/profile-menu.js';
import { initPublicHeaderTheme } from './components/public-header.js';
import { initRoomFavoriteButtons } from './components/favorites.js';
import { initHomeAvailabilitySearch } from './pages/home-availability.js';
import { initSearchPanelHeaderState } from './pages/search-panel-header.js';
import { adminModal, AdminConfirm } from './components/admin-modal.js';

function initApp() {
    if (document.documentElement.dataset.hotelAppReady === 'true') {
        return;
    }

    document.documentElement.dataset.hotelAppReady = 'true';
    registerGlobalFeedback();

    const isPublicHomeRoute = document.documentElement.classList.contains('public-home-route');
    const isAuthRoute = authRoutePrefixes.some((route) => window.location.pathname.toLowerCase().startsWith(route));

    document.documentElement.classList.remove('public-nav-start-scrolled');
    initPublicHeaderTheme();
    initStaticToasts();

    if (isPublicHomeRoute) {
        initAuthModal();
    }

    if (isAuthRoute) {
        initDirectAuthCard();
    }

    initProfileMenus();
    initRoomFavoriteButtons();
    initCustomSelects();
    initDateRangePickers();
    initGuestPanels();
    initHomeAvailabilitySearch();
    initSearchPanelHeaderState();
    initPasswordVisibilityToggles();
    window.__adminModal = adminModal;
    window.__AdminConfirm = AdminConfirm;
    window.__hotelAppBootstrapped = true;
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initApp, { once: true });
} else {
    initApp();
}