export function initCustomSelects() {
    document.querySelectorAll('select:not([multiple]):not([data-native-select])').forEach((select) => {
        if (!(select instanceof HTMLSelectElement) || select.dataset.customSelectReady === 'true') {
            return;
        }

        const isInSfBar = select.closest('.sf-bar') !== null;
        const wrapper = document.createElement('div');
        wrapper.className = isInSfBar ? 'custom-select' : 'custom-select custom-select--standalone';

        const trigger = document.createElement('button');
        trigger.type = 'button';
        trigger.className = 'custom-select-trigger';
        trigger.setAttribute('aria-haspopup', 'listbox');
        trigger.setAttribute('aria-expanded', 'false');

        const valueElement = document.createElement('span');
        valueElement.className = 'custom-select-value';

        const chevron = document.createElement('span');
        chevron.className = 'custom-select-chevron';
        chevron.setAttribute('aria-hidden', 'true');
        chevron.innerHTML = '<svg viewBox="0 0 20 20" fill="currentColor"><path fill-rule="evenodd" d="M5.23 7.21a.75.75 0 0 1 1.06.02L10 11.168l3.71-3.938a.75.75 0 1 1 1.08 1.04l-4.25 4.5a.75.75 0 0 1-1.08 0l-4.25-4.5a.75.75 0 0 1 .02-1.06Z" clip-rule="evenodd"/></svg>';
        trigger.append(valueElement, chevron);

        const panel = document.createElement('div');
        panel.className = 'custom-select-panel';
        panel.setAttribute('role', 'listbox');

        select.dataset.customSelectReady = 'true';
        select.classList.add('custom-select-native');
        select.parentNode.insertBefore(wrapper, select);
        wrapper.append(select, trigger, panel);

        const positionPanel = () => {
            const viewportPadding = 12;
            const anchorElement = isInSfBar ? wrapper.closest('.sf-field') || trigger : trigger;
            const anchorRect = anchorElement.getBoundingClientRect();
            const preferredWidth = isInSfBar ? anchorRect.width : trigger.getBoundingClientRect().width;
            const panelWidth = Math.min(preferredWidth, window.innerWidth - (viewportPadding * 2));
            const left = Math.min(
                Math.max(anchorRect.left, viewportPadding),
                window.innerWidth - panelWidth - viewportPadding
            );
            const top = anchorRect.bottom + 8;
            const maxHeight = Math.max(150, Math.min(256, window.innerHeight - top - viewportPadding));

            panel.style.setProperty('--custom-select-panel-left', `${Math.round(left)}px`);
            panel.style.setProperty('--custom-select-panel-top', `${Math.round(top)}px`);
            panel.style.setProperty('--custom-select-panel-width', `${Math.round(panelWidth)}px`);
            panel.style.maxHeight = `${Math.round(maxHeight)}px`;
        };

        const close = () => {
            wrapper.classList.remove('is-open');
            trigger.setAttribute('aria-expanded', 'false');
        };

        const open = () => {
            document.querySelectorAll('.custom-select.is-open').forEach((openSelect) => {
                if (openSelect === wrapper) {
                    return;
                }

                openSelect.classList.remove('is-open');
                openSelect.querySelector('.custom-select-trigger')?.setAttribute('aria-expanded', 'false');
            });

            positionPanel();
            wrapper.classList.add('is-open');
            trigger.setAttribute('aria-expanded', 'true');
        };

        const chooseOption = (index) => {
            if (!select.options[index] || select.options[index].disabled) {
                return;
            }

            select.selectedIndex = index;
            select.dispatchEvent(new Event('change', { bubbles: true }));
            close();
            trigger.focus();
        };

        const focusOption = (index) => {
            const options = Array.from(panel.querySelectorAll('.custom-select-option:not(.is-disabled)'));

            if (options.length) {
                options[Math.max(0, Math.min(index, options.length - 1))].focus();
            }
        };

        const renderOptions = () => {
            panel.innerHTML = '';

            Array.from(select.options).forEach((option, index) => {
                const item = document.createElement('button');
                item.type = 'button';
                item.className = 'custom-select-option'
                    + (option.selected ? ' is-selected' : '')
                    + (option.disabled ? ' is-disabled' : '');
                item.setAttribute('role', 'option');
                item.setAttribute('aria-selected', option.selected ? 'true' : 'false');
                item.dataset.index = index;

                if (option.disabled) {
                    item.disabled = true;
                }

                const label = document.createElement('span');
                label.textContent = option.textContent.trim();

                const check = document.createElement('span');
                check.className = 'custom-select-option-check';
                check.setAttribute('aria-hidden', 'true');
                check.innerHTML = '<svg viewBox="0 0 20 20" fill="currentColor"><path fill-rule="evenodd" d="M16.704 5.29a1 1 0 0 1 .006 1.414l-7.25 7.31a1 1 0 0 1-1.42 0L3.29 9.23a1 1 0 1 1 1.42-1.41l4.04 4.07 6.54-6.594a1 1 0 0 1 1.414-.006Z" clip-rule="evenodd"/></svg>';

                item.append(label, check);
                item.addEventListener('click', () => chooseOption(index));
                item.addEventListener('keydown', (event) => {
                    const enabledOptions = Array.from(panel.querySelectorAll('.custom-select-option:not(.is-disabled)'));
                    const currentIndex = enabledOptions.indexOf(item);

                    if (event.key === 'ArrowDown') {
                        event.preventDefault();
                        focusOption(currentIndex + 1);
                    } else if (event.key === 'ArrowUp') {
                        event.preventDefault();
                        focusOption(currentIndex - 1);
                    } else if (event.key === 'Home') {
                        event.preventDefault();
                        focusOption(0);
                    } else if (event.key === 'End') {
                        event.preventDefault();
                        focusOption(enabledOptions.length - 1);
                    } else if (event.key === 'Escape') {
                        event.preventDefault();
                        close();
                        trigger.focus();
                    }
                });
                panel.append(item);
            });
        };

        const update = () => {
            const option = select.options[select.selectedIndex] || select.options[0];
            valueElement.textContent = option?.textContent.trim() || select.getAttribute('aria-label') || 'Seleccionar';
            trigger.disabled = select.disabled;
            renderOptions();
        };

        trigger.addEventListener('click', () => {
            if (wrapper.classList.contains('is-open')) {
                close();
                return;
            }

            open();
        });

        trigger.addEventListener('keydown', (event) => {
            if (!['ArrowDown', 'ArrowUp', 'Enter', ' '].includes(event.key)) {
                return;
            }

            event.preventDefault();
            open();
            focusOption(Math.max(0, select.selectedIndex));
        });

        const repositionIfOpen = () => {
            if (wrapper.classList.contains('is-open')) {
                positionPanel();
            }
        };

        window.addEventListener('resize', repositionIfOpen);
        window.addEventListener('scroll', repositionIfOpen, true);

        select.addEventListener('change', update);
        select.addEventListener('invalid', () => trigger.focus());
        update();
    });

    if (document.documentElement.dataset.csGlobal === 'true') {
        return;
    }

    document.documentElement.dataset.csGlobal = 'true';

    document.addEventListener('click', (event) => {
        if (!(event.target instanceof Node)) {
            return;
        }

        document.querySelectorAll('.custom-select.is-open').forEach((select) => {
            if (!select.contains(event.target)) {
                select.classList.remove('is-open');
                select.querySelector('.custom-select-trigger')?.setAttribute('aria-expanded', 'false');
            }
        });
    });

    document.addEventListener('keydown', (event) => {
        if (event.key !== 'Escape') {
            return;
        }

        document.querySelectorAll('.custom-select.is-open').forEach((select) => {
            select.classList.remove('is-open');
            select.querySelector('.custom-select-trigger')?.setAttribute('aria-expanded', 'false');
        });
    });
}