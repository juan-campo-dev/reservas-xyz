export function initPublicHeaderTheme() {
    const header = document.querySelector('[data-public-header]');

    if (!(header instanceof HTMLElement)) {
        document.documentElement.classList.remove('nav-starts-over-hero');
        return;
    }

    const heroTargets = Array.from(document.querySelectorAll('.guest-hero-shell, .compute-hero-image'))
        .filter((target) => target instanceof HTMLElement);

    const isAvailabilitySearchActive = () => document.documentElement.classList.contains('availability-search-active');

    const setHeaderState = (isOverHero, isScrolled) => {
        header.classList.toggle('is-over-hero', isOverHero);
        header.classList.toggle('is-on-surface', !isOverHero);
        header.classList.toggle('is-scrolled', isScrolled);
        document.documentElement.classList.toggle('nav-starts-over-hero', isOverHero && !isScrolled);
    };

    const getProbeY = () => {
        const nav = header.querySelector('[data-public-nav]');
        const navRect = nav instanceof HTMLElement ? nav.getBoundingClientRect() : header.getBoundingClientRect();
        return navRect.top + (navRect.height * 0.55);
    };

    const updateHeaderTheme = () => {
        if (!heroTargets.length) {
            setHeaderState(false, window.scrollY > 12);
            return;
        }

        const probeY = getProbeY();
        const isSearchActive = isAvailabilitySearchActive();
        const isScrolled = !isSearchActive && window.scrollY > 12;
        const isOverHero = heroTargets.some((target) => {
            const rect = target.getBoundingClientRect();
            return rect.top <= probeY && rect.bottom >= probeY;
        });

        setHeaderState(isSearchActive || isOverHero, isScrolled);
    };

    updateHeaderTheme();
    requestAnimationFrame(updateHeaderTheme);

    window.addEventListener('scroll', updateHeaderTheme, { passive: true });
    window.addEventListener('resize', updateHeaderTheme);
    document.addEventListener('availability-search:state', updateHeaderTheme);
}