export function initHomeAvailabilitySearch() {
    const form = document.querySelector('[data-availability-form]');
    const surface = document.querySelector('[data-availability-surface]');
    const content = document.querySelector('[data-availability-content]');

    if (!(form instanceof HTMLFormElement)
        || !(surface instanceof HTMLElement)
        || !(content instanceof HTMLElement)
        || form.dataset.availabilityReady === 'true') {
        return;
    }

    form.dataset.availabilityReady = 'true';

    const siteSelect = form.querySelector('[data-availability-site-select]');
    const initialContent = content.innerHTML;
    const hero = document.querySelector('.guest-portal-hero');
    const savedSearchKey = 'reservasxyz:availability-search';

    const getFormDataValue = (formData, name) => {
        const value = formData.get(name);
        return typeof value === 'string' ? value : '';
    };

    const isSavedSearchValid = (state) => state
        && state.checkIn
        && state.checkOut
        && Number(state.guests) > 0;

    const buildSavedSearchState = (formData) => ({
        siteId: getFormDataValue(formData, 'SiteId'),
        checkIn: getFormDataValue(formData, 'CheckIn'),
        checkOut: getFormDataValue(formData, 'CheckOut'),
        guests: getFormDataValue(formData, 'Guests') || '2'
    });

    const saveAvailabilitySearchState = (state) => {
        if (!isSavedSearchValid(state)) {
            return;
        }

        try {
            sessionStorage.setItem(savedSearchKey, JSON.stringify(state));
        } catch {
        }
    };

    const readAvailabilitySearchState = () => {
        try {
            const rawState = sessionStorage.getItem(savedSearchKey);

            if (!rawState) {
                return null;
            }

            const parsedState = JSON.parse(rawState);
            return isSavedSearchValid(parsedState) ? parsedState : null;
        } catch {
            return null;
        }
    };

    const clearAvailabilitySearchState = () => {
        try {
            sessionStorage.removeItem(savedSearchKey);
        } catch {
        }
    };

    const clearAvailabilityRestoreState = () => {
        document.documentElement.classList.remove('availability-search-restoring');
    };

    const setAvailabilitySearchState = (isActive) => {
        surface.dataset.availabilityActive = isActive ? 'true' : 'false';
        document.documentElement.classList.toggle('availability-search-active', isActive);

        if (!isActive) {
            clearAvailabilityRestoreState();
        }

        if (hero instanceof HTMLElement) {
            hero.classList.toggle('is-availability-search-active', isActive);
        }

        document.dispatchEvent(new CustomEvent('availability-search:state', {
            detail: { isActive }
        }));
    };

    const syncAvailabilitySearchState = () => {
        setAvailabilitySearchState(content.querySelector('[data-availability-results]') instanceof HTMLElement);
    };

    const getInputValue = (name) => {
        const input = form.querySelector(`[name="${name}"]`);
        return input instanceof HTMLInputElement ? input.value : '';
    };

    const setSiteValue = (siteId) => {
        if (!(siteSelect instanceof HTMLSelectElement)) {
            return;
        }

        siteSelect.value = siteId;
        siteSelect.dispatchEvent(new Event('change', { bubbles: true }));
    };

    const setMainSearchState = (state, dateRangeCompleted = true) => {
        setSiteValue(state.siteId || '');

        const checkInInput = form.querySelector('[name="CheckIn"]');
        const checkOutInput = form.querySelector('[name="CheckOut"]');
        const guestsInput = form.querySelector('[name="Guests"]');

        if (checkInInput instanceof HTMLInputElement && state.checkIn) {
            checkInInput.value = state.checkIn;
        }

        if (checkOutInput instanceof HTMLInputElement && state.checkOut) {
            checkOutInput.value = state.checkOut;
        }

        if (guestsInput instanceof HTMLInputElement && state.guests) {
            guestsInput.value = String(state.guests);
            guestsInput.dispatchEvent(new Event('input', { bubbles: true }));
            guestsInput.dispatchEvent(new Event('change', { bubbles: true }));
        }

        form.dataset.dateRangeCompleted = dateRangeCompleted ? 'true' : 'false';
        form.dispatchEvent(new CustomEvent('dateRange:set', {
            detail: {
                checkIn: state.checkIn,
                checkOut: state.checkOut,
                completed: dateRangeCompleted
            }
        }));
    };

    const getSearchField = (fieldName) => form.querySelector(`.sf-field--${fieldName}`);
    let activeSearchNotice = null;

    const getSearchNoticeAnchor = (target) => {
        if (!(target instanceof HTMLElement)) {
            return null;
        }

        const panel = target.matches('.sf-field--dates')
            ? document.querySelector('.date-range-panel.is-open')
            : null;

        return panel instanceof HTMLElement ? panel : target;
    };

    const isSearchNoticeAnchorVisible = (target) => {
        const anchor = getSearchNoticeAnchor(target);

        if (!(anchor instanceof HTMLElement)) {
            return false;
        }

        const rect = anchor.getBoundingClientRect();
        return rect.bottom > 12
            && rect.top < window.innerHeight - 12
            && rect.right > 12
            && rect.left < window.innerWidth - 12;
    };

    const clearSearchNotice = () => {
        activeSearchNotice = null;
        form.classList.remove('is-search-missing', 'is-search-shaking');
        form.querySelectorAll('.sf-field.is-search-missing, .sf-field.is-search-shaking').forEach((field) => {
            field.classList.remove('is-search-missing', 'is-search-shaking');
        });
        document.querySelectorAll('[data-search-field-notice]').forEach((notice) => notice.remove());
    };

    const positionSearchNotice = (notice, target, options = {}) => {
        const anchor = getSearchNoticeAnchor(target);

        if (!(notice instanceof HTMLElement) || !(anchor instanceof HTMLElement)) {
            return;
        }

        const targetRect = anchor.getBoundingClientRect();
        const noticeRect = notice.getBoundingClientRect();
        const viewportPadding = 12;
        const alignCenter = options.align === 'center';
        const left = Math.min(
            Math.max(alignCenter ? targetRect.left + ((targetRect.width - noticeRect.width) / 2) : targetRect.left + 16, viewportPadding),
            window.innerWidth - noticeRect.width - viewportPadding
        );
        const preferAbove = options.preferAbove === true || anchor !== target;
        const top = preferAbove
            ? Math.max(viewportPadding, targetRect.top - noticeRect.height - 10)
            : Math.min(targetRect.bottom + 10, window.innerHeight - noticeRect.height - viewportPadding);

        notice.style.setProperty('--sf-notice-left', `${Math.round(left)}px`);
        notice.style.setProperty('--sf-notice-top', `${Math.round(top)}px`);
    };

    const syncActiveSearchNotice = () => {
        if (!(activeSearchNotice?.notice instanceof HTMLElement) || !(activeSearchNotice?.target instanceof HTMLElement)) {
            activeSearchNotice = null;
            return;
        }

        if (!document.body.contains(activeSearchNotice.notice) || !isSearchNoticeAnchorVisible(activeSearchNotice.target)) {
            clearSearchNotice();
            return;
        }

        positionSearchNotice(activeSearchNotice.notice, activeSearchNotice.target, activeSearchNotice.options);
        activeSearchNotice.notice.style.visibility = '';
    };

    const pointToSearchTarget = (target, message, options = {}) => {
        if (!(target instanceof HTMLElement)) {
            return false;
        }

        clearSearchNotice();
        form.classList.add('is-search-missing');

        if (target !== form && target.classList.contains('sf-field')) {
            target.classList.add('is-search-missing');
        }

        const notice = document.createElement('p');
        notice.className = 'sf-field-notice';
        notice.dataset.searchFieldNotice = 'true';
        notice.setAttribute('role', 'status');
        notice.textContent = message;
        notice.style.visibility = 'hidden';
        document.body.append(notice);
        activeSearchNotice = { notice, target, options };

        const showNotice = () => {
            syncActiveSearchNotice();
        };

        requestAnimationFrame(() => {
            form.classList.add('is-search-shaking');

            if (target !== form && target.classList.contains('sf-field')) {
                target.classList.add('is-search-shaking');
            }

            if (!options.openControl) {
                showNotice();
            }
        });

        target.scrollIntoView({ behavior: 'smooth', block: 'center' });

        const focusTarget = options.focusTarget === false
            ? null
            : (options.focusTarget instanceof HTMLElement
                ? options.focusTarget
                : target.querySelector('[data-date-range-trigger], .custom-select-trigger, [data-guests-trigger], select, input, button'));

        if (focusTarget instanceof HTMLElement) {
            window.setTimeout(() => {
                focusTarget.focus({ preventScroll: true });

                if (options.openControl && focusTarget instanceof HTMLButtonElement && focusTarget.getAttribute('aria-expanded') !== 'true') {
                    focusTarget.click();
                    window.setTimeout(showNotice, 80);
                } else {
                    showNotice();
                }
            }, 180);
        } else {
            requestAnimationFrame(showNotice);
        }

        window.setTimeout(() => {
            form.classList.remove('is-search-shaking');

            if (target !== form && target.classList.contains('sf-field')) {
                target.classList.remove('is-search-shaking');
            }
        }, 700);

        return false;
    };

    const pointToSearchField = (fieldName, message, shouldOpenControl = false) => {
        const field = getSearchField(fieldName);
        return pointToSearchTarget(field, message, { openControl: shouldOpenControl });
    };

    const pointToSearchForm = (message) => pointToSearchTarget(form, message, {
        align: 'center',
        preferAbove: true,
        focusTarget: false,
        openControl: false
    });

    const validateMainSearchForm = () => {
        const guests = Number(getInputValue('Guests') || 0);

        if (form.dataset.dateRangeCompleted !== 'true') {
            return pointToSearchField('dates', 'Selecciona entrada y salida.', true);
        }

        if (!Number.isFinite(guests) || guests < 1) {
            return pointToSearchField('guests', 'Selecciona la cantidad de huéspedes.', true);
        }

        clearSearchNotice();
        return true;
    };

    const renderLoading = () => {
        setAvailabilitySearchState(true);
        content.innerHTML = `
            <section class="compute-card rounded-3xl p-10 text-center">
                <div class="mx-auto flex max-w-sm items-center justify-center gap-3 text-sm text-muted-foreground">
                    <span class="h-2.5 w-2.5 animate-pulse rounded-full bg-foreground/30"></span>
                    <span>Consultando disponibilidad...</span>
                </div>
            </section>
        `;
    };

    const renderError = () => {
        setAvailabilitySearchState(true);
        content.innerHTML = `
            <section class="compute-card rounded-3xl p-10 text-center">
                <p class="font-display text-4xl leading-none text-foreground">No se pudo consultar disponibilidad.</p>
                <p class="mx-auto mt-4 max-w-lg text-muted-foreground">Revisa los datos del buscador e intenta nuevamente.</p>
            </section>
        `;
    };

    const formatCurrency = (value) => `$${new Intl.NumberFormat('es-CO', {
        maximumFractionDigits: 0
    }).format(value)}`;

    const getRoomWord = (count) => (count === 1 ? 'habitación' : 'habitaciones');

    const setAvailabilityView = (results, view) => {
        if (!(results instanceof HTMLElement)) {
            return;
        }

        const nextView = view === 'table' ? 'table' : 'list';
        results.dataset.availabilityView = nextView;
        results.querySelectorAll('[data-availability-view-toggle]').forEach((button) => {
            const isActive = button.dataset.availabilityViewToggle === nextView;
            button.classList.toggle('is-active', isActive);
            button.setAttribute('aria-pressed', isActive ? 'true' : 'false');
        });
    };

    const buildRoomSelectionUrl = (root, selectedRoomIds) => {
        const url = new URL(root.dataset.reservationBaseUrl || '/Reservations/Create', window.location.origin);

        selectedRoomIds.forEach((roomId) => {
            url.searchParams.append('roomIds', roomId);
        });

        url.searchParams.set('checkIn', root.dataset.searchCheckIn || getInputValue('CheckIn'));
        url.searchParams.set('checkOut', root.dataset.searchCheckOut || getInputValue('CheckOut'));
        url.searchParams.set('guests', root.dataset.searchGuests || getInputValue('Guests') || '1');

        return `${url.pathname}${url.search}`;
    };

    const syncRoomSelection = (root) => {
        if (!(root instanceof HTMLElement)) {
            return false;
        }

        const requiredGuests = Number(root.dataset.searchGuests || getInputValue('Guests') || 0);
        const toggles = Array.from(root.querySelectorAll('[data-room-selection-toggle]'));
        const selectedToggles = toggles.filter((toggle) => toggle instanceof HTMLInputElement && toggle.checked);
        const selectedRoomIds = selectedToggles.map((toggle) => toggle.value).filter(Boolean);
        const selectedCapacity = selectedToggles.reduce((total, toggle) => total + Number(toggle.dataset.roomCapacity || 0), 0);
        const selectedTotal = selectedToggles.reduce((total, toggle) => total + Number(toggle.dataset.roomTotal || 0), 0);
        const isReady = selectedRoomIds.length > 0 && selectedCapacity >= requiredGuests;
        const status = root.querySelector('[data-room-selection-status]');
        const count = root.querySelector('[data-room-selection-count]');
        const capacity = root.querySelector('[data-room-selection-capacity]');
        const total = root.querySelector('[data-room-selection-total]');
        const submit = root.querySelector('[data-room-selection-submit]');

        root.querySelectorAll('[data-room-selection-card]').forEach((card) => {
            const checkbox = card.querySelector('[data-room-selection-toggle]');
            card.classList.toggle('is-selected', checkbox instanceof HTMLInputElement && checkbox.checked);
        });

        if (count instanceof HTMLElement) {
            count.textContent = `${selectedRoomIds.length} ${getRoomWord(selectedRoomIds.length)}`;
        }

        if (capacity instanceof HTMLElement) {
            capacity.textContent = `${selectedCapacity} / ${requiredGuests} huésped${requiredGuests !== 1 ? 'es' : ''}`;
        }

        if (total instanceof HTMLElement) {
            total.textContent = formatCurrency(selectedTotal);
        }

        if (status instanceof HTMLElement) {
            if (selectedRoomIds.length === 0) {
                status.textContent = `Selecciona habitaciones hasta cubrir ${requiredGuests} huésped${requiredGuests !== 1 ? 'es' : ''}.`;
            } else if (!isReady) {
                status.textContent = `Faltan ${Math.max(requiredGuests - selectedCapacity, 0)} cupos para completar el grupo.`;
            } else {
                status.textContent = `Listo: ${selectedRoomIds.length} ${getRoomWord(selectedRoomIds.length)} cubren ${selectedCapacity} huéspedes.`;
            }
        }

        if (submit instanceof HTMLAnchorElement) {
            submit.classList.toggle('is-disabled', !isReady);
            submit.setAttribute('aria-disabled', isReady ? 'false' : 'true');
            submit.href = isReady ? buildRoomSelectionUrl(root, selectedRoomIds) : '#';
        }

        root.classList.toggle('is-selection-ready', isReady);
        return isReady;
    };

    const syncRoomSelections = (container = document) => {
        container.querySelectorAll('[data-room-selection]').forEach(syncRoomSelection);
    };

    const runSearch = async (formData, submitButton = null) => {
        saveAvailabilitySearchState(buildSavedSearchState(formData));
        const originalLabel = submitButton instanceof HTMLButtonElement ? submitButton.textContent : '';

        if (submitButton instanceof HTMLButtonElement) {
            submitButton.disabled = true;
            submitButton.textContent = 'Buscando';
        }

        renderLoading();
        clearAvailabilityRestoreState();

        try {
            const response = await fetch(form.action, {
                method: (form.method || 'POST').toUpperCase(),
                body: formData,
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const html = await response.text();

            if (!response.ok && response.status !== 400) {
                renderError();
                return false;
            }

            content.innerHTML = html;
            syncAvailabilitySearchState();
            syncRoomSelections(content);
            return true;
        } catch (error) {
            renderError();
            return false;
        } finally {
            if (submitButton instanceof HTMLButtonElement && document.body.contains(submitButton)) {
                submitButton.disabled = false;
                submitButton.textContent = originalLabel || 'Buscar';
            }
        }
    };

    const restoreSavedAvailabilitySearch = () => {
        const savedState = readAvailabilitySearchState();

        if (!savedState) {
            return false;
        }

        setMainSearchState(savedState, true);
        void runSearch(new FormData(form));
        return true;
    };

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const submitButton = form.querySelector('[type="submit"]');

        if (!validateMainSearchForm()) {
            return;
        }

        await runSearch(new FormData(form), submitButton);
    });

    form.addEventListener('dateRange:interacted', clearSearchNotice);
    form.addEventListener('dateRange:completed', clearSearchNotice);
    form.addEventListener('dateRange:closed', clearSearchNotice);
    form.addEventListener('change', () => {
        if (form.dataset.dateRangeCompleted === 'true') {
            clearSearchNotice();
        }
    });

    document.addEventListener('pointerdown', () => {
        if (activeSearchNotice) {
            clearSearchNotice();
        }
    });

    window.addEventListener('scroll', syncActiveSearchNotice, true);
    window.addEventListener('resize', syncActiveSearchNotice);

    document.addEventListener('click', (event) => {
        const target = event.target;

        if (!(target instanceof Element)) {
            return;
        }

        const clearSearchLink = target.closest('[data-clear-availability-search]');

        if (clearSearchLink instanceof HTMLElement) {
            clearAvailabilitySearchState();
        }

        const selectedDate = target.closest('.date-range-day');

        if (selectedDate instanceof HTMLButtonElement && !selectedDate.disabled) {
            clearSearchNotice();
        }

        const siteCard = target.closest('[data-site-card]');

        if (siteCard instanceof HTMLElement) {
            event.preventDefault();
            const siteId = siteCard.dataset.siteId || '';
            setSiteValue(siteId);

            if (!validateMainSearchForm()) {
                return;
            }

            runSearch(new FormData(form), form.querySelector('[type="submit"]'));
            return;
        }

        const viewToggle = target.closest('[data-availability-view-toggle]');

        if (viewToggle instanceof HTMLButtonElement) {
            event.preventDefault();
            setAvailabilityView(viewToggle.closest('[data-availability-results]'), viewToggle.dataset.availabilityViewToggle);
            return;
        }

        const editButton = target.closest('[data-availability-edit]');

        if (editButton instanceof HTMLElement) {
            event.preventDefault();
            const results = editButton.closest('[data-availability-results]');
            setMainSearchState({
                siteId: results?.dataset.searchSiteId || '',
                checkIn: results?.dataset.searchCheckIn || getInputValue('CheckIn'),
                checkOut: results?.dataset.searchCheckOut || getInputValue('CheckOut'),
                guests: results?.dataset.searchGuests || getInputValue('Guests') || '2'
            }, true);
            pointToSearchForm('Ajusta la búsqueda desde aquí.');
            return;
        }

        const resetButton = target.closest('[data-availability-reset]');

        if (resetButton instanceof HTMLElement) {
            event.preventDefault();
            clearAvailabilitySearchState();
            setSiteValue('');
            content.innerHTML = initialContent;
            setAvailabilitySearchState(false);
            syncRoomSelections(content);
            return;
        }

        const selectionSubmit = target.closest('[data-room-selection-submit]');

        if (selectionSubmit instanceof HTMLAnchorElement) {
            const root = selectionSubmit.closest('[data-room-selection]');

            if (!syncRoomSelection(root)) {
                event.preventDefault();
                root?.classList.add('is-selection-warning');
                window.setTimeout(() => root?.classList.remove('is-selection-warning'), 650);
            }

            return;
        }

        const presetButton = target.closest('[data-room-selection-preset]');

        if (presetButton instanceof HTMLButtonElement) {
            event.preventDefault();
            const root = presetButton.closest('[data-room-selection]');
            const presetRoomIds = (presetButton.dataset.roomSelectionPreset || '')
                .split(',')
                .map((roomId) => roomId.trim())
                .filter(Boolean);

            root?.querySelectorAll('[data-room-selection-toggle]').forEach((toggle) => {
                if (toggle instanceof HTMLInputElement) {
                    toggle.checked = presetRoomIds.includes(toggle.value);
                }
            });

            root?.classList.add('is-selection-mode');
            const selectionModeButton = root?.querySelector('[data-room-selection-enable]');

            if (selectionModeButton instanceof HTMLButtonElement) {
                selectionModeButton.setAttribute('aria-pressed', 'true');
                selectionModeButton.textContent = 'Cerrar selección';
            }

            syncRoomSelection(root);
            return;
        }

        const selectionModeButton = target.closest('[data-room-selection-enable]');

        if (selectionModeButton instanceof HTMLButtonElement) {
            event.preventDefault();
            const root = selectionModeButton.closest('[data-room-selection]');
            const shouldEnable = !root?.classList.contains('is-selection-mode');

            root?.classList.toggle('is-selection-mode', shouldEnable);
            selectionModeButton.setAttribute('aria-pressed', shouldEnable ? 'true' : 'false');
            selectionModeButton.textContent = shouldEnable ? 'Cerrar selección' : 'Armar reserva grande';
            syncRoomSelection(root);
            return;
        }

        const selectionCard = target.closest('[data-room-selection-card]');

        if (selectionCard instanceof HTMLElement && !target.closest('a, button, input, label')) {
            const root = selectionCard.closest('[data-room-selection]');

            if (!root?.classList.contains('is-selection-mode')) {
                return;
            }

            const checkbox = selectionCard.querySelector('[data-room-selection-toggle]');

            if (checkbox instanceof HTMLInputElement) {
                checkbox.checked = !checkbox.checked;
                syncRoomSelection(root);
            }
        }
    });

    document.addEventListener('change', (event) => {
        const toggle = event.target;

        if (toggle instanceof HTMLInputElement && toggle.hasAttribute('data-room-selection-toggle')) {
            syncRoomSelection(toggle.closest('[data-room-selection]'));
        }
    });

    if (content.querySelector('[data-availability-results]') instanceof HTMLElement) {
        syncAvailabilitySearchState();
        clearAvailabilityRestoreState();
        syncRoomSelections(content);
        return;
    }

    if (!restoreSavedAvailabilitySearch()) {
        syncAvailabilitySearchState();
        clearAvailabilityRestoreState();
        syncRoomSelections(content);
    }
}
