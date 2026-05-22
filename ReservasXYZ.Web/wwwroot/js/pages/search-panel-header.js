export function initSearchPanelHeaderState() {
    const header = document.querySelector('[data-public-header]');

    if (!(header instanceof HTMLElement) || !(document.body instanceof HTMLElement)) {
        document.documentElement.classList.remove('search-panel-open');
        return;
    }

    const syncHeaderState = () => {
        const isPanelOpen = document.querySelector('[data-date-range-anchor].is-date-open') !== null;
        document.documentElement.classList.toggle('search-panel-open', isPanelOpen);
    };

    const observer = new MutationObserver(syncHeaderState);
    observer.observe(document.body, {
        attributes: true,
        attributeFilter: ['class'],
        childList: true,
        subtree: true
    });

    syncHeaderState();
}