export function initGuestPanels() {
    document.querySelectorAll('[data-guests-panel]').forEach((container) => {
        if (!(container instanceof HTMLElement) || container.dataset.guestsReady === 'true') {
            return;
        }

        const trigger = container.querySelector('[data-guests-trigger]');
        const summary = container.querySelector('[data-guests-summary]');
        const adultsInput = container.querySelector('[data-guests-input="adults"]');

        if (!(trigger instanceof HTMLButtonElement)
            || !(summary instanceof HTMLElement)
            || !(adultsInput instanceof HTMLInputElement)) {
            return;
        }

        container.dataset.guestsReady = 'true';

        const adultsMin = Number(adultsInput.min) || 1;
        const adultsMax = Number(adultsInput.max) || 20;
        const adultsStep = Number(adultsInput.step) || 1;

        const iconMarkup = {
            minus: '<svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true"><path stroke-linecap="round" d="M5 10h10" /></svg>',
            plus: '<svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true"><path stroke-linecap="round" d="M10 5v10M5 10h10" /></svg>'
        };

        const panel = document.createElement('div');
        panel.className = 'guest-panel';
        panel.setAttribute('role', 'dialog');
        panel.setAttribute('aria-label', 'Seleccionar huéspedes');
        panel.innerHTML = `
            <div class="guest-panel-rows">
                <div class="guest-panel-row">
                    <div class="guest-panel-copy">
                        <span class="guest-panel-title">Huéspedes</span>
                    </div>
                    <div class="guest-panel-stepper" role="group" aria-label="Seleccionar huéspedes">
                        <button type="button" class="guest-panel-stepper-button" data-guests-decrement aria-label="Reducir huéspedes">${iconMarkup.minus}</button>
                        <span class="guest-panel-stepper-value" data-guests-value="adults">2</span>
                        <button type="button" class="guest-panel-stepper-button" data-guests-increment aria-label="Aumentar huéspedes">${iconMarkup.plus}</button>
                    </div>
                </div>
            </div>
            <div class="guest-panel-footer">
                <button type="button" class="guest-panel-close" data-guests-close>Listo</button>
            </div>
        `;
        document.body.append(panel);

        const decrementButton = panel.querySelector('[data-guests-decrement]');
        const incrementButton = panel.querySelector('[data-guests-increment]');
        const adultsValueElement = panel.querySelector('[data-guests-value="adults"]');
        const closeButton = panel.querySelector('[data-guests-close]');

        if (!(decrementButton instanceof HTMLButtonElement)
            || !(incrementButton instanceof HTMLButtonElement)
            || !(adultsValueElement instanceof HTMLElement)
            || !(closeButton instanceof HTMLButtonElement)) {
            panel.remove();
            return;
        }

        const getAdultsValue = () => {
            const value = Number(adultsInput.value);
            return Number.isFinite(value) ? value : adultsMin;
        };

        const clampAdultsValue = (value) => Math.min(Math.max(value, adultsMin), adultsMax);
        const formatLabel = (value, singular, plural) => `${value} ${value === 1 ? singular : plural}`;

        const positionPanel = () => {
            const viewportPadding = 12;
            const anchorRect = container.getBoundingClientRect();
            const maxWidth = window.innerWidth - (viewportPadding * 2);
            const preferredWidth = window.innerWidth < 640 ? maxWidth : Math.max(anchorRect.width, 260);
            const panelWidth = Math.min(preferredWidth, maxWidth, 300);
            const left = Math.min(
                Math.max(anchorRect.left, viewportPadding),
                window.innerWidth - panelWidth - viewportPadding
            );
            const top = anchorRect.bottom + 8;

            panel.style.setProperty('--guest-panel-left', `${Math.round(left)}px`);
            panel.style.setProperty('--guest-panel-top', `${Math.round(top)}px`);
            panel.style.setProperty('--guest-panel-width', `${Math.round(panelWidth)}px`);
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

        const close = () => {
            panel.classList.remove('is-open');
            container.classList.remove('is-guests-open');
            trigger.setAttribute('aria-expanded', 'false');
        };

        const closeOtherPanels = () => {
            document.querySelectorAll('.custom-select.is-open').forEach((openSelect) => {
                openSelect.classList.remove('is-open');
                openSelect.querySelector('.custom-select-trigger')?.setAttribute('aria-expanded', 'false');
            });

            document.querySelectorAll('.date-range-panel.is-open').forEach((openPanel) => {
                openPanel.classList.remove('is-open');
            });

            document.querySelectorAll('.sf-field.is-date-open').forEach((openField) => {
                openField.classList.remove('is-date-open');
            });

            document.querySelectorAll('[data-date-range-trigger][aria-expanded="true"]').forEach((openTrigger) => {
                openTrigger.setAttribute('aria-expanded', 'false');
            });

            document.querySelectorAll('.guest-panel.is-open').forEach((openPanel) => {
                openPanel.classList.remove('is-open');
            });

            document.querySelectorAll('[data-guests-panel].is-guests-open').forEach((openContainer) => {
                openContainer.classList.remove('is-guests-open');
                openContainer.querySelector('[data-guests-trigger]')?.setAttribute('aria-expanded', 'false');
            });
        };

        const open = () => {
            closeOtherPanels();
            panel.classList.add('is-open');
            container.classList.add('is-guests-open');
            trigger.setAttribute('aria-expanded', 'true');
            requestAnimationFrame(scrollPageToPanel);
        };

        const renderValues = () => {
            const adultsValue = getAdultsValue();
            adultsValueElement.textContent = String(adultsValue);
            summary.textContent = formatLabel(adultsValue, 'huésped', 'huéspedes');
        };

        const updateButtons = () => {
            const adultsValue = getAdultsValue();
            decrementButton.disabled = adultsValue <= adultsMin;
            incrementButton.disabled = adultsValue >= adultsMax;
            renderValues();
        };

        const setAdultsValue = (value) => {
            adultsInput.value = String(clampAdultsValue(value));
            adultsInput.dispatchEvent(new Event('input', { bubbles: true }));
            adultsInput.dispatchEvent(new Event('change', { bubbles: true }));
            updateButtons();
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

        decrementButton.addEventListener('click', () => setAdultsValue(getAdultsValue() - adultsStep));
        incrementButton.addEventListener('click', () => setAdultsValue(getAdultsValue() + adultsStep));
        closeButton.addEventListener('click', close);
        adultsInput.addEventListener('input', updateButtons);
        adultsInput.form?.addEventListener('submit', () => setAdultsValue(getAdultsValue()));

        document.addEventListener('click', (event) => {
            if (!(event.target instanceof Node)) {
                return;
            }

            if (!panel.contains(event.target) && !container.contains(event.target)) {
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

        updateButtons();
    });
}