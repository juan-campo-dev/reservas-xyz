import puppeteer from 'puppeteer';
import { mkdir } from 'fs/promises';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const BASE = 'http://localhost:5263';
const OUT = join(__dirname, '..', 'docs', 'screenshots');
const WIDTH = 1920;
const HEIGHT = 1080;

function futureDate(daysFromNow) {
    const d = new Date();
    d.setDate(d.getDate() + daysFromNow);
    return d.toISOString().split('T')[0];
}

async function run() {
    await mkdir(OUT, { recursive: true });

    const browser = await puppeteer.launch({
        headless: true,
        defaultViewport: { width: WIDTH, height: HEIGHT },
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });

    const page = await browser.newPage();
    await page.setViewport({ width: WIDTH, height: HEIGHT });

    const screenshot = async (name, opts = {}) => {
        const path = join(OUT, `${name}.png`);
        await page.screenshot({ path, fullPage: opts.fullPage || false });
        console.log(`  ✓ ${name}.png`);
    };

    const wait = (ms) => new Promise(r => setTimeout(r, ms));
    const checkIn = futureDate(7);
    const checkOut = futureDate(10);

    try {
        // ─── 1. Login page ───
        console.log('1. Login page...');
        await page.goto(`${BASE}/Identity/Account/Login`, { waitUntil: 'networkidle2' });
        await screenshot('guest-01-login');

        // ─── 2. Login as client ───
        console.log('2. Logging in as cliente@test.com...');
        await page.type('input[name="Input.Email"]', 'cliente@test.com');
        await page.type('input[name="Input.Password"]', 'Cliente123*');
        await Promise.all([
            page.waitForNavigation({ waitUntil: 'networkidle2' }),
            page.click('button[type="submit"]')
        ]);
        await wait(1500);
        await screenshot('guest-02-portal');

        // ─── 3. Scroll to see site cards ───
        console.log('3. Sedes del portal...');
        await page.evaluate(() => {
            const cards = document.querySelector('[data-site-card]');
            if (cards) cards.scrollIntoView({ behavior: 'instant', block: 'center' });
            else window.scrollTo(0, 600);
        });
        await wait(800);
        await screenshot('guest-03-sedes');

        // ─── 4. Scroll back up and show search bar ───
        console.log('4. Formulario de búsqueda...');
        await page.evaluate(() => window.scrollTo(0, 0));
        await wait(500);
        await screenshot('guest-04-busqueda-form');

        // ─── 5. Search availability (2 guests, 1 site) ───
        console.log('5. Buscando disponibilidad...');
        const searchOk = await page.evaluate(async (ci, co) => {
            const form = document.querySelector('[data-availability-form]');
            if (!form) return 'no-form';

            const siteSelect = form.querySelector('[name="SiteId"]');
            const ciInput = form.querySelector('[name="CheckIn"]');
            const coInput = form.querySelector('[name="CheckOut"]');
            const gInput = form.querySelector('[name="Guests"]');

            if (siteSelect) {
                const firstOption = siteSelect.querySelector('option[value]:not([value=""])');
                if (firstOption) siteSelect.value = firstOption.value;
            }
            if (ciInput) ciInput.value = ci;
            if (coInput) coInput.value = co;
            if (gInput) gInput.value = '2';

            form.dataset.dateRangeCompleted = 'true';

            const fd = new FormData(form);
            const resp = await fetch(form.action, {
                method: 'POST',
                body: fd,
                credentials: 'same-origin',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            const html = await resp.text();

            const content = document.querySelector('[data-availability-content]');
            if (content) content.innerHTML = html;

            const surface = document.querySelector('[data-availability-surface]');
            if (surface) surface.dataset.availabilityActive = 'true';
            document.documentElement.classList.add('availability-search-active');
            const hero = document.querySelector('.guest-portal-hero');
            if (hero) hero.classList.add('is-availability-search-active');

            const ciLabel = document.querySelector('[data-date-label="checkin"]');
            const coLabel = document.querySelector('[data-date-label="checkout"]');
            const sep = document.querySelector('[data-date-separator]');
            if (ciLabel) ciLabel.textContent = new Date(ci + 'T12:00:00').toLocaleDateString('es-CO', { day: 'numeric', month: 'short' });
            if (coLabel) coLabel.textContent = new Date(co + 'T12:00:00').toLocaleDateString('es-CO', { day: 'numeric', month: 'short', year: 'numeric' });
            if (sep) sep.hidden = false;

            return resp.ok ? 'ok' : `error-${resp.status}`;
        }, checkIn, checkOut);
        console.log(`   Search result: ${searchOk}`);
        await wait(2000);

        const hasResults = await page.evaluate(() => !!document.querySelector('[data-availability-results]'));
        console.log(`   Results visible: ${hasResults}`);

        if (hasResults) {
            await screenshot('guest-05-resultados');

            // Scroll to room cards
            await page.evaluate(() => {
                const cards = document.querySelectorAll('.availability-room-card, [data-room-card]');
                if (cards.length > 2) cards[2].scrollIntoView({ behavior: 'instant', block: 'center' });
                else if (cards.length > 0) cards[0].scrollIntoView({ behavior: 'instant', block: 'center' });
            });
            await wait(800);
            await screenshot('guest-06-habitaciones');

            // ─── 6. Click favorite ───
            console.log('6. Marcando favorito...');
            const favBtn = await page.$('[data-room-favorite]');
            if (favBtn) {
                await favBtn.click();
                await wait(1500);
                await screenshot('guest-07-favorito');
            } else {
                console.log('   No favorite button found, skipping');
            }

            // ─── 7. Group selection (only screenshot, no reservation from here) ───
            console.log('7. Modo selección grupal...');
            const hasGroupBtn = await page.evaluate(() => {
                const btn = document.querySelector('[data-room-selection-enable]');
                if (btn) { btn.click(); return true; }
                return false;
            });
            if (hasGroupBtn) {
                await wait(1000);
                await page.evaluate(() => {
                    const toggles = document.querySelectorAll('[data-room-selection-toggle]');
                    for (let i = 0; i < Math.min(2, toggles.length); i++) {
                        toggles[i].click();
                    }
                });
                await wait(1000);
                await page.evaluate(() => {
                    const meter = document.querySelector('.availability-selection-meter, .availability-group-booking, [data-room-selection]');
                    if (meter) meter.scrollIntoView({ behavior: 'instant', block: 'center' });
                });
                await wait(500);
                await screenshot('guest-08-seleccion-grupal');
            }

            // ─── 8. Individual reservation — 1 room, 2 guests ───
            console.log('8. Reserva individual (1 habitación, 2 huéspedes)...');

            // Get first room's individual reserve link
            const singleRoomLink = await page.evaluate((ci, co) => {
                const links = Array.from(document.querySelectorAll('a[href*="/Reservations/Create"]'));
                for (const link of links) {
                    const href = link.href;
                    if (href.includes('roomId=') && !href.includes('roomIds=')) return href;
                }
                // Fallback: extract first roomId from any link
                for (const link of links) {
                    const m = link.href.match(/roomIds?=(\d+)/);
                    if (m) return `/Reservations/Create?roomId=${m[1]}&checkIn=${ci}&checkOut=${co}&guests=2`;
                }
                return null;
            }, checkIn, checkOut);

            if (singleRoomLink) {
                const url = singleRoomLink.startsWith('http') ? singleRoomLink : `${BASE}${singleRoomLink}`;
                console.log(`   Navigating to: ${url}`);
                await page.goto(url, { waitUntil: 'networkidle2' });
                await wait(1500);
                await screenshot('guest-09-crear-reserva');

                // Fill guest data
                await page.evaluate(() => {
                    const name = document.querySelector('#GuestName');
                    const email = document.querySelector('#GuestEmail');
                    const phone = document.querySelector('#GuestPhone');
                    if (name) { name.value = 'Juan Campo'; name.dispatchEvent(new Event('input', { bubbles: true })); }
                    if (email) { email.value = 'cliente@test.com'; email.dispatchEvent(new Event('input', { bubbles: true })); }
                    if (phone) { phone.value = '3001234567'; phone.dispatchEvent(new Event('input', { bubbles: true })); }
                });
                await wait(500);
                await screenshot('guest-10-formulario-reserva');

                // Submit via the button that references form="reservationForm"
                console.log('9. Enviando reserva...');
                try {
                    await Promise.all([
                        page.waitForNavigation({ waitUntil: 'networkidle2', timeout: 15000 }),
                        page.click('button[form="reservationForm"]')
                    ]);
                } catch {
                    // Fallback: requestSubmit
                    await Promise.all([
                        page.waitForNavigation({ waitUntil: 'networkidle2', timeout: 10000 }).catch(() => { }),
                        page.evaluate(() => document.querySelector('#reservationForm')?.requestSubmit())
                    ]);
                }
                await wait(2000);

                const currentUrl = page.url();
                console.log(`   Current URL: ${currentUrl}`);

                if (currentUrl.includes('/Reservations/Details')) {
                    await screenshot('guest-11-reserva-creada');

                    // Cancel reservation
                    console.log('10. Cancelando reserva...');
                    const hasCancelBtn = await page.evaluate(() => {
                        const forms = document.querySelectorAll('form[action*="Cancel"]');
                        return forms.length;
                    });
                    console.log(`   Cancel forms found: ${hasCancelBtn}`);

                    if (hasCancelBtn > 0) {
                        // Click and wait for page reload (same URL redirect)
                        await page.evaluate(() => {
                            const forms = document.querySelectorAll('form[action*="Cancel"]');
                            const lastForm = forms[forms.length - 1];
                            const btn = lastForm.querySelector('button[type="submit"]');
                            if (btn) btn.click();
                        });
                        await wait(3000);
                        await page.waitForFunction(
                            () => document.readyState === 'complete',
                            { timeout: 15000 }
                        );
                        await wait(1500);
                        await screenshot('guest-12-reserva-cancelada');
                    } else {
                        console.log('   No cancel form found');
                    }
                } else {
                    console.log('   Reservation may not have been created, capturing current state');
                    await screenshot('guest-11-resultado-reserva');
                }
            } else {
                console.log('   No reserve link found');
            }
        } else {
            console.log('   No results - capturing current state');
            await screenshot('guest-05-no-results');
        }

        // ─── 11. My reservations section on portal ───
        console.log('11. Portal - Mis reservas...');
        await page.goto(BASE, { waitUntil: 'networkidle2' });
        await wait(1500);
        await page.evaluate(() => {
            const section = document.querySelector('[data-upcoming-reservations], .guest-reservations, .guest-upcoming');
            if (section) section.scrollIntoView({ behavior: 'instant', block: 'start' });
            else window.scrollTo(0, document.body.scrollHeight * 0.4);
        });
        await wait(800);
        await screenshot('guest-13-mis-reservas');

        // ─── 12. Logout → Forgot password ───
        console.log('12. Recuperar contraseña...');
        const logoutForm = await page.$('form[action*="Logout"]');
        if (logoutForm) {
            await Promise.all([
                page.waitForNavigation({ waitUntil: 'networkidle2' }),
                logoutForm.evaluate(f => f.submit())
            ]);
            await wait(1000);
        }
        await page.goto(`${BASE}/Identity/Account/ForgotPassword`, { waitUntil: 'networkidle2' });
        await wait(500);
        await screenshot('guest-14-forgot-password');

        // ─── 13. Register ───
        console.log('13. Registro...');
        await page.goto(`${BASE}/Identity/Account/Register`, { waitUntil: 'networkidle2' });
        await wait(500);
        await screenshot('guest-15-registro');

        console.log('\n✅ Capturas completadas!');
        console.log(`   Guardadas en: ${OUT}`);
    } catch (err) {
        console.error('Error:', err.message);
        console.error(err.stack);
        await screenshot('error-state').catch(() => { });
    } finally {
        await browser.close();
    }
}

run();
