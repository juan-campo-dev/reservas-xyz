(function () {
    if (window.__hotelAppBootstrapped || window.__hotelAppCompatLoading) {
        return;
    }

    window.__hotelAppCompatLoading = true;

    if (document.querySelector('script[data-hotel-app-entry="true"]')) {
        return;
    }

    const currentScript = document.currentScript;
    const appUrl = currentScript instanceof HTMLScriptElement
        ? new URL('./app.js', currentScript.src).toString()
        : new URL('/js/app.js', window.location.origin).toString();
    const script = document.createElement('script');
    script.type = 'module';
    script.src = appUrl;
    script.dataset.hotelAppEntry = 'true';
    document.head.appendChild(script);
})();