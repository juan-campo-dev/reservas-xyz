export function initDateRangePickers() {
    const forms = document.querySelectorAll('[data-date-range-form]');

    if (!forms.length) {
        return;
    }

    const displayFormatter = new Intl.DateTimeFormat('es-CO', { day: '2-digit', month: 'short', year: 'numeric' });
    const monthFormatter = new Intl.DateTimeFormat('es-CO', { month: 'long', year: 'numeric' });
    const weekdays = ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa'];

    const parseIsoDate = (value) => {
        if (!value) {
            return null;
        }

        const parts = value.split('-').map(Number);

        if (parts.length !== 3 || parts.some(Number.isNaN)) {
            return null;
        }

        return new Date(parts[0], parts[1] - 1, parts[2]);
    };

    const formatIsoDate = (date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    };

    const addDays = (date, days) => new Date(date.getFullYear(), date.getMonth(), date.getDate() + days);
    const addMonths = (date, months) => new Date(date.getFullYear(), date.getMonth() + months, 1);
    const startOfMonth = (date) => new Date(date.getFullYear(), date.getMonth(), 1);
    const compareDates = (leftDate, rightDate) => formatIsoDate(leftDate).localeCompare(formatIsoDate(rightDate));
    const isSameDate = (leftDate, rightDate) => compareDates(leftDate, rightDate) === 0;

    forms.forEach((form) => {
        if (!(form instanceof HTMLFormElement) || form.dataset.dateRangeReady === 'true') {
            return;
        }

        const checkInInput = form.querySelector('[data-date-input="checkin"]');
        const checkOutInput = form.querySelector('[data-date-input="checkout"]');
        const trigger = form.querySelector('[data-date-range-trigger]');
        const checkInLabel = form.querySelector('[data-date-label="checkin"]');
        const checkOutLabel = form.querySelector('[data-date-label="checkout"]');
        const dateSeparator = form.querySelector('[data-date-separator]');
        const anchor = form.querySelector('[data-date-range-anchor]') || trigger?.closest('.sf-field');

        if (!(checkInInput instanceof HTMLInputElement)
            || !(checkOutInput instanceof HTMLInputElement)
            || !(trigger instanceof HTMLButtonElement)
            || !(anchor instanceof HTMLElement)) {
            return;
        }

        form.dataset.dateRangeReady = 'true';

        const fallbackToday = new Date();
        const minDate = parseIsoDate(form.dataset.minDate)
            || new Date(fallbackToday.getFullYear(), fallbackToday.getMonth(), fallbackToday.getDate());
        let checkInDate = parseIsoDate(checkInInput.value) || minDate;
        let checkOutDate = parseIsoDate(checkOutInput.value) || addDays(checkInDate, 1);
        let visibleMonth = startOfMonth(minDate);
        let isSelectingCheckout = false;
        let hasCompletedRange = form.dataset.dateRangeCompleted === 'true';

        if (compareDates(checkOutDate, checkInDate) <= 0) {
            checkOutDate = addDays(checkInDate, 1);
        }

        const panel = document.createElement('div');
        panel.className = 'date-range-panel';
        panel.setAttribute('role', 'dialog');
        panel.setAttribute('aria-label', 'Seleccionar fechas de reserva');
        panel.innerHTML = `
            <div class="date-range-summary">
                <div class="date-range-summary-item">
                    <span class="date-range-summary-label">Entrada</span>
                    <span class="date-range-summary-value" data-range-summary="checkin"></span>
                </div>
                <div class="date-range-summary-item">
                    <span class="date-range-summary-label">Salida</span>
                    <span class="date-range-summary-value" data-range-summary="checkout"></span>
                </div>
            </div>
            <div class="date-range-nav">
                <button type="button" class="date-range-nav-button" data-range-prev aria-label="Mes anterior">
                    <svg viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 0 1-.02 1.06L9.06 10l3.71 3.71a.75.75 0 1 1-1.06 1.06l-4.24-4.24a.75.75 0 0 1 0-1.06l4.24-4.24a.75.75 0 0 1 1.08 0Z" clip-rule="evenodd"/></svg>
                </button>
                <div class="date-range-nav-title" data-range-title></div>
                <button type="button" class="date-range-nav-button" data-range-next aria-label="Mes siguiente">
                    <svg viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 0 1 .02-1.06L10.94 10 7.23 6.29a.75.75 0 1 1 1.06-1.06l4.24 4.24a.75.75 0 0 1 0 1.06l-4.24 4.24a.75.75 0 0 1-1.08 0Z" clip-rule="evenodd"/></svg>
                </button>
            </div>
            <div class="date-range-months" data-range-months></div>
            <div class="date-range-footer">
                <button type="button" class="date-range-close" data-range-close>Listo</button>
            </div>
        `;
        document.body.append(panel);

        const monthContainer = panel.querySelector('[data-range-months]');
        const title = panel.querySelector('[data-range-title]');
        const checkInSummary = panel.querySelector('[data-range-summary="checkin"]');
        const checkOutSummary = panel.querySelector('[data-range-summary="checkout"]');
        const previousButton = panel.querySelector('[data-range-prev]');
        const nextButton = panel.querySelector('[data-range-next]');
        const closeButton = panel.querySelector('[data-range-close]');

        const updateValues = () => {
            const checkInText = displayFormatter.format(checkInDate).replace('.', '');
            const checkOutText = displayFormatter.format(checkOutDate).replace('.', '');
            const fallbackCheckOut = compareDates(checkOutDate, checkInDate) > 0 ? checkOutDate : addDays(checkInDate, 1);

            form.dataset.dateRangeCompleted = hasCompletedRange ? 'true' : 'false';

            checkInInput.value = formatIsoDate(checkInDate);
            checkOutInput.value = formatIsoDate(hasCompletedRange ? checkOutDate : fallbackCheckOut);

            if (!hasCompletedRange && !isSelectingCheckout) {
                if (checkInLabel instanceof HTMLElement) {
                    checkInLabel.textContent = 'Selecciona fechas';
                }

                if (checkOutLabel instanceof HTMLElement) {
                    checkOutLabel.textContent = '';
                }

                if (dateSeparator instanceof HTMLElement) {
                    dateSeparator.hidden = true;
                }
            } else if (isSelectingCheckout) {
                if (checkInLabel instanceof HTMLElement) {
                    checkInLabel.textContent = checkInText;
                }

                if (checkOutLabel instanceof HTMLElement) {
                    checkOutLabel.textContent = 'Selecciona salida';
                }

                if (dateSeparator instanceof HTMLElement) {
                    dateSeparator.hidden = false;
                }
            } else if (checkInLabel instanceof HTMLElement && checkOutLabel instanceof HTMLElement) {
                checkInLabel.textContent = checkInText;
                checkOutLabel.textContent = checkOutText;

                if (dateSeparator instanceof HTMLElement) {
                    dateSeparator.hidden = false;
                }
            }

            if (checkInSummary instanceof HTMLElement) {
                checkInSummary.textContent = isSelectingCheckout || hasCompletedRange ? checkInText : 'Selecciona entrada';
            }

            if (checkOutSummary instanceof HTMLElement) {
                checkOutSummary.textContent = hasCompletedRange ? checkOutText : 'Selecciona salida';
            }
        };

        const renderMonth = (monthDate) => {
            const year = monthDate.getFullYear();
            const month = monthDate.getMonth();
            const section = document.createElement('section');
            const monthTitle = document.createElement('h3');
            const weekdaysRow = document.createElement('div');
            const daysGrid = document.createElement('div');
            const firstDay = new Date(year, month, 1).getDay();
            const totalDays = new Date(year, month + 1, 0).getDate();

            section.className = 'date-range-month';
            monthTitle.className = 'date-range-month-title';
            monthTitle.textContent = monthFormatter.format(monthDate);
            weekdaysRow.className = 'date-range-weekdays';
            daysGrid.className = 'date-range-days';

            weekdays.forEach((weekday) => {
                const weekdayElement = document.createElement('span');
                weekdayElement.className = 'date-range-weekday';
                weekdayElement.textContent = weekday;
                weekdaysRow.append(weekdayElement);
            });

            for (let blankIndex = 0; blankIndex < firstDay; blankIndex += 1) {
                daysGrid.append(document.createElement('span'));
            }

            for (let dayNumber = 1; dayNumber <= totalDays; dayNumber += 1) {
                const currentDate = new Date(year, month, dayNumber);
                const dayButton = document.createElement('button');
                const isBeforeMinimum = compareDates(currentDate, minDate) < 0;
                const hasVisibleStart = isSelectingCheckout || hasCompletedRange;
                const isRangeStart = hasVisibleStart && isSameDate(currentDate, checkInDate);
                const isRangeEnd = hasCompletedRange && isSameDate(currentDate, checkOutDate);
                const isInRange = hasCompletedRange
                    && compareDates(currentDate, checkInDate) > 0
                    && compareDates(currentDate, checkOutDate) < 0;

                dayButton.type = 'button';
                dayButton.className = 'date-range-day';
                dayButton.textContent = dayNumber;
                dayButton.disabled = isBeforeMinimum;
                dayButton.classList.toggle('is-selected', isRangeStart || isRangeEnd);
                dayButton.classList.toggle('is-range-start', isRangeStart);
                dayButton.classList.toggle('is-range-end', isRangeEnd);
                dayButton.classList.toggle('is-in-range', isInRange);
                dayButton.classList.toggle('is-today', isSameDate(currentDate, minDate));
                dayButton.setAttribute('aria-label', displayFormatter.format(currentDate));
                dayButton.addEventListener('click', (event) => {
                    event.preventDefault();
                    event.stopPropagation();
                    selectDate(currentDate);
                });
                daysGrid.append(dayButton);
            }

            section.append(monthTitle, weekdaysRow, daysGrid);
            return section;
        };

        const positionPanel = () => {
            const viewportPadding = 12;
            const anchorRect = anchor.getBoundingClientRect();
            const maxWidth = window.innerWidth - (viewportPadding * 2);
            const preferredWidth = window.innerWidth < 720 ? maxWidth : Math.max(anchorRect.width, 500);
            const panelWidth = Math.min(preferredWidth, maxWidth, 580);
            const left = Math.min(
                Math.max(anchorRect.left, viewportPadding),
                window.innerWidth - panelWidth - viewportPadding
            );
            const top = anchorRect.bottom + 8;

            panel.style.setProperty('--date-range-panel-left', `${Math.round(left)}px`);
            panel.style.setProperty('--date-range-panel-top', `${Math.round(top)}px`);
            panel.style.setProperty('--date-range-panel-width', `${Math.round(panelWidth)}px`);
            panel.style.maxHeight = '';
        };

        const scrollPageToPanel = () => {
            positionPanel();

            const viewportPadding = 12;
            const panelRect = panel.getBoundingClientRect();
            const overflow = panelRect.bottom - (window.innerHeight - viewportPadding);

            if (overflow > 0) {
                window.scrollBy({ top: Math.ceil(overflow), behavior: 'smooth' });
            }
        };

        const render = () => {
            updateValues();

            if (title instanceof HTMLElement) {
                const secondMonth = addMonths(visibleMonth, 1);
                title.textContent = `${monthFormatter.format(visibleMonth)} - ${monthFormatter.format(secondMonth)}`;
            }

            if (monthContainer instanceof HTMLElement) {
                monthContainer.innerHTML = '';
                monthContainer.append(renderMonth(visibleMonth), renderMonth(addMonths(visibleMonth, 1)));
            }

            if (panel.classList.contains('is-open')) {
                requestAnimationFrame(positionPanel);
            }
        };

        function selectDate(selectedDate) {
            if (compareDates(selectedDate, minDate) < 0) {
                return;
            }

            form.dispatchEvent(new CustomEvent('dateRange:interacted', {
                detail: {
                    date: formatIsoDate(selectedDate)
                }
            }));

            if (!isSelectingCheckout || hasCompletedRange) {
                checkInDate = selectedDate;
                checkOutDate = addDays(selectedDate, 1);
                isSelectingCheckout = true;
                hasCompletedRange = false;
                visibleMonth = startOfMonth(checkInDate);
                render();
                return;
            }

            if (compareDates(selectedDate, checkInDate) <= 0) {
                checkInDate = selectedDate;
                checkOutDate = addDays(selectedDate, 1);
                isSelectingCheckout = true;
                hasCompletedRange = false;
                visibleMonth = startOfMonth(checkInDate);
                render();
                return;
            }

            checkOutDate = selectedDate;
            isSelectingCheckout = false;
            hasCompletedRange = true;
            render();
            form.dispatchEvent(new CustomEvent('dateRange:completed', {
                detail: {
                    checkIn: formatIsoDate(checkInDate),
                    checkOut: formatIsoDate(checkOutDate)
                }
            }));
        }

        form.addEventListener('dateRange:set', (event) => {
            const detail = event.detail || {};
            const nextCheckIn = parseIsoDate(detail.checkIn || checkInInput.value);
            const nextCheckOut = parseIsoDate(detail.checkOut || checkOutInput.value);

            if (nextCheckIn) {
                checkInDate = nextCheckIn;
            }

            if (nextCheckOut) {
                checkOutDate = compareDates(nextCheckOut, checkInDate) <= 0 ? addDays(checkInDate, 1) : nextCheckOut;
            } else {
                checkOutDate = addDays(checkInDate, 1);
            }

            isSelectingCheckout = false;
            hasCompletedRange = detail.completed === true;
            visibleMonth = startOfMonth(checkInDate);
            render();
        });

        const close = () => {
            const wasOpen = panel.classList.contains('is-open');

            panel.classList.remove('is-open');
            anchor.classList.remove('is-date-open');
            trigger.setAttribute('aria-expanded', 'false');

            if (wasOpen) {
                form.dispatchEvent(new CustomEvent('dateRange:closed'));
            }
        };

        const open = () => {
            document.querySelectorAll('.custom-select.is-open').forEach((openSelect) => {
                openSelect.classList.remove('is-open');
                openSelect.querySelector('.custom-select-trigger')?.setAttribute('aria-expanded', 'false');
            });

            panel.classList.add('is-open');
            anchor.classList.add('is-date-open');
            trigger.setAttribute('aria-expanded', 'true');
            visibleMonth = startOfMonth(hasCompletedRange || isSelectingCheckout ? checkInDate : minDate);
            render();
            requestAnimationFrame(scrollPageToPanel);
        };

        panel.addEventListener('click', (event) => {
            event.stopPropagation();
        });

        trigger.addEventListener('click', (event) => {
            event.preventDefault();

            if (panel.classList.contains('is-open')) {
                close();
                return;
            }

            open();
        });

        previousButton?.addEventListener('click', () => {
            visibleMonth = addMonths(visibleMonth, -1);
            render();
        });

        nextButton?.addEventListener('click', () => {
            visibleMonth = addMonths(visibleMonth, 1);
            render();
        });

        closeButton?.addEventListener('click', close);

        document.addEventListener('click', (event) => {
            if (!(event.target instanceof Node)) {
                return;
            }

            if (!panel.contains(event.target) && !form.contains(event.target)) {
                close();
            }
        });

        document.addEventListener('keydown', (event) => {
            if (event.key === 'Escape') {
                close();
            }
        });

        window.addEventListener('resize', () => {
            if (panel.classList.contains('is-open')) {
                positionPanel();
            }
        });

        window.addEventListener('scroll', () => {
            if (panel.classList.contains('is-open')) {
                positionPanel();
            }
        }, true);

        render();
    });
}