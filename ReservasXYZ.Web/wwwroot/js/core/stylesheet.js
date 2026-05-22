export function refreshOutputStylesheetOnLocalhost({
    refreshKey = 'modalCssRefresh',
    guardKey = 'modalCssRefreshed'
} = {}) {
    const isLocalhost = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1';

    if (!isLocalhost || document.documentElement.dataset[guardKey] === 'true') {
        return;
    }

    const stylesheet = document.querySelector('link[href*="/css/output.css"]');

    if (!(stylesheet instanceof HTMLLinkElement)) {
        return;
    }

    const refreshedUrl = new URL(stylesheet.href, window.location.origin);
    refreshedUrl.searchParams.set(refreshKey, Date.now().toString());
    stylesheet.href = refreshedUrl.toString();
    document.documentElement.dataset[guardKey] = 'true';
}