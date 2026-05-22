---
pdf_options:
  format: A4
  margin:
    top: 20mm
    bottom: 20mm
    left: 22mm
    right: 22mm
  printBackground: true
  displayHeaderFooter: true
  headerTemplate: '<div style="width:100%;font-size:7.5pt;font-family:Segoe UI,Arial,sans-serif;color:#888;padding:0 22mm;display:flex;justify-content:space-between;"><span></span><span>Sistema de Reservas XYZ — Documento Técnico v1.0</span></div>'
  footerTemplate: '<div style="width:100%;font-size:7.5pt;font-family:Segoe UI,Arial,sans-serif;color:#888;padding:0 22mm;display:flex;justify-content:space-between;"><span>Juan Campo — Analista Desarrollador .NET</span><span>Página <span class="pageNumber"></span> de <span class="totalPages"></span></span></div>'
body_class: print-doc
---

<style>
  @page {
    size: A4;
    margin: 20mm 22mm;
  }

  :root {
    --c-text: #1a1a1a;
    --c-muted: #555;
    --c-rule: #ccc;
    --c-accent: #1a3c6e;
    --c-bg-code: #f5f6f8;
    --c-tbl-head: #2c3e50;
    --font: 'Segoe UI', 'Calibri', Arial, sans-serif;
    --mono: 'Consolas', 'Cascadia Code', 'Courier New', monospace;
  }

  * { box-sizing: border-box; margin: 0; padding: 0; }

  body {
    font-family: var(--font);
    font-size: 10.5pt;
    line-height: 1.45;
    color: var(--c-text);
    orphans: 3;
    widows: 3;
  }

  h1 {
    font-size: 16pt;
    font-weight: 700;
    color: var(--c-accent);
    border-bottom: 2px solid var(--c-accent);
    padding-bottom: 3mm;
    margin: 10mm 0 5mm 0;
    page-break-after: avoid;
  }
  h2 {
    font-size: 13pt;
    font-weight: 600;
    color: #2c3e50;
    margin: 7mm 0 3mm 0;
    page-break-after: avoid;
  }
  h3 {
    font-size: 11pt;
    font-weight: 600;
    color: #374151;
    margin: 5mm 0 2mm 0;
    page-break-after: avoid;
  }

  p {
    margin: 0 0 2.5mm 0;
    text-align: justify;
    hyphens: auto;
  }
  ul, ol { margin: 0 0 3mm 0; padding-left: 6mm; }
  li { margin-bottom: 1mm; }
  strong { font-weight: 600; }

  table {
    width: 100%;
    border-collapse: collapse;
    margin: 3mm 0 4mm 0;
    font-size: 9pt;
    page-break-inside: avoid;
  }
  thead th {
    background: var(--c-tbl-head);
    color: #fff;
    font-weight: 600;
    text-align: left;
    padding: 2mm 3mm;
    border: 0.5pt solid #1a2a3a;
    font-size: 8.5pt;
  }
  tbody td {
    padding: 1.5mm 3mm;
    border: 0.5pt solid #d0d0d0;
    vertical-align: top;
  }
  tbody tr:nth-child(even) { background: #f7f8fa; }

  .fig {
    page-break-inside: avoid;
    margin: 4mm 0 5mm 0;
    text-align: center;
  }
  .fig img {
    max-width: 88%;
    max-height: 36vh;
    height: auto;
    display: block;
    margin: 0 auto;
    border: 0.5pt solid #d0d0d0;
  }
  .fig-caption {
    font-size: 9pt;
    color: var(--c-muted);
    font-style: italic;
    text-align: center;
    margin-top: 2mm;
    line-height: 1.3;
  }

  .fig-row {
    display: flex;
    gap: 4mm;
    page-break-inside: avoid;
    margin: 4mm 0 5mm 0;
  }
  .fig-row .fig {
    flex: 1;
    margin: 0;
  }
  .fig-row .fig img {
    max-width: 100%;
    max-height: 30vh;
  }

  img {
    max-width: 88%;
    max-height: 36vh;
    height: auto;
    display: block;
    margin: 2mm auto;
    border: 0.5pt solid #d0d0d0;
  }

  p > em:only-child {
    display: block;
    font-size: 9pt;
    color: var(--c-muted);
    text-align: center;
    margin: 1mm 0 4mm 0;
    font-style: italic;
    line-height: 1.3;
  }

  p:has(> img) {
    page-break-inside: avoid;
    text-align: center;
    margin: 4mm 0 1mm 0;
  }
  p:has(> img) + p {
    page-break-before: avoid;
  }

  pre {
    background: var(--c-bg-code);
    border: 0.5pt solid #ddd;
    border-left: 3pt solid var(--c-accent);
    padding: 3mm 4mm;
    font-size: 8pt;
    line-height: 1.5;
    overflow: hidden;
    word-wrap: break-word;
    white-space: pre-wrap;
    page-break-inside: avoid;
    margin: 2mm 0 3mm 0;
  }
  code {
    font-family: var(--mono);
    font-size: 8.5pt;
  }
  p code, li code, td code {
    background: #eef0f3;
    padding: 0.3mm 1.5mm;
    font-size: 8pt;
    color: #c0392b;
  }

  hr {
    border: none;
    height: 0.5pt;
    background: var(--c-rule);
    margin: 5mm 0;
  }

  blockquote {
    background: #f0f4f8;
    border-left: 3pt solid var(--c-accent);
    padding: 2.5mm 4mm;
    margin: 2mm 0 3mm 0;
    font-size: 9.5pt;
    color: #2c3e50;
    page-break-inside: avoid;
  }
  blockquote p { margin: 0; text-align: left; }

  .pb { page-break-after: always; }
</style>

<!-- ══════════════════════════════════════════════════════════ -->
<!--         FLUJO COMPLETO DEL ASOCIADO (CLIENTE)             -->
<!-- ══════════════════════════════════════════════════════════ -->

# Portal del Asociado — Flujo Completo

El portal del asociado es la interfaz principal para los usuarios con rol **Guest/Cliente**. Desde aquí el asociado realiza todo el ciclo de reservas: buscar disponibilidad, comparar habitaciones, marcar favoritos, crear reservas y gestionar sus estadías.

## Acceso al portal

Al iniciar sesión, el asociado accede automáticamente al portal personalizado. La pantalla principal muestra:

- Saludo personalizado con su nombre.
- Formulario de búsqueda de disponibilidad (sede, fechas, huéspedes).
- Listado de las 8 sedes recreativas disponibles con capacidad.
- Sus últimas 3 reservas activas (si las tiene).

![Portal del asociado](docs/screenshots/01-home.png)

*Figura 1. Portal del asociado con formulario de búsqueda de disponibilidad y sedes recreativas.*

---

## Paso 1 — Búsqueda de disponibilidad

El asociado completa el formulario de búsqueda con:

| Campo | Descripción | Requerido |
|-------|-------------|-----------|
| Sede | Seleccionar una sede específica o "Todas las sedes" | Opcional |
| Fecha de entrada | Fecha de check-in (mínimo: hoy) | Sí |
| Fecha de salida | Fecha de check-out (posterior a check-in) | Sí |
| Huéspedes | Número de personas (mínimo: 1) | Sí |

*Tabla 1. Campos del formulario de búsqueda de disponibilidad.*

**Funcionamiento técnico:**

1. El formulario envía un `POST` vía AJAX a `HomeController.SearchAvailability`.
2. El controller invoca `IRoomService.SearchAvailableAsync(search)` que ejecuta el **SP `sp_GetAvailableRoomsByDatesAndGuests`**.
3. Para cada habitación disponible, se ejecuta **`sp_CalculateTotalRate`** con las fechas y huéspedes seleccionados.
4. Los resultados se renderizan como partial view (`_GuestAvailabilityResults.cshtml`) sin recargar la página.
5. La búsqueda se guarda en `sessionStorage` para persistir si el usuario navega y vuelve.

**Validaciones:**

- Fecha de salida debe ser posterior a la de entrada.
- Al menos 1 huésped.
- Anti-forgery token obligatorio (`[ValidateAntiForgeryToken]`).
- Solo usuarios autenticados (`[Authorize]`).
- Administradores no pueden acceder a este flujo (`return Forbid()`).

<div class="pb"></div>

## Paso 2 — Resultados de disponibilidad

Los resultados se presentan agrupados por sede. Para cada sede se muestra:

- Nombre de la sede y ciudad.
- Cantidad de alojamientos y habitaciones disponibles.
- Capacidad total de la sede.

Para cada habitación disponible:

| Dato | Ejemplo |
|------|---------|
| Número de habitación | V01, M-N03, 202 |
| Tipo | Familiar, Cabaña, Apartamento |
| Capacidad máxima | 4, 6, 8 personas |
| Alojamiento | Habitaciones Villeta, Bloque Nuevo |
| Tarifa base por noche | $70.000, $90.000 |
| Tarifa total de la estadía | Calculada por SP4 |
| Personas incluidas en tarifa base | 4 personas |
| Costo persona adicional/noche | $16.000, $11.000 |
| Etiqueta de ajuste | "Mejor opción" o "Solo grupo" |

*Tabla 2. Información mostrada por cada habitación disponible.*

![Resultados de disponibilidad](docs/screenshots/11-availability-search.png)

*Figura 2. Resultados de búsqueda con habitaciones disponibles, precios calculados por SP y botones de favoritos.*

**Vistas disponibles:** El asociado puede alternar entre vista de **Tabla** y vista de **Lista** con el toggle en la barra lateral.

**Acciones desde resultados:**

- **Modificar búsqueda**: cambia parámetros sin volver al inicio.
- **Ver destinos**: vuelve a la pantalla principal con sedes.

<div class="pb"></div>

## Paso 3 — Selección de habitaciones

El asociado tiene tres opciones para seleccionar habitaciones:

### Opción A — Reserva individual

Si la habitación cubre la capacidad necesaria (etiqueta "Mejor opción"), el asociado puede reservar directamente con el botón **"Reservar"** de esa habitación. Este botón redirige a:

```
/Reservations/Create?roomId={id}&checkIn={fecha}&checkOut={fecha}&guests={n}
```

### Opción B — Reserva grupal (selección manual)

Cuando el grupo es grande y ninguna habitación individual alcanza, el asociado puede:

1. Hacer clic en **"Armar reserva grande"**.
2. Se activa el modo de selección múltiple.
3. Marcar habitaciones con checkboxes hasta cubrir la capacidad necesaria.
4. Un medidor en tiempo real muestra: habitaciones seleccionadas, capacidad cubierta y precio total.
5. Cuando la capacidad cubre los huéspedes → botón **"Reservar selección"** se activa.

### Opción C — Reservar toda la sede

Si hay múltiples habitaciones, aparece el botón **"Reservar toda la sede"** que pre-selecciona todas las habitaciones de esa sede.

### Opción D — Sugerencia automática

El botón **"Sugerir grupo"** selecciona automáticamente la combinación óptima de habitaciones (ordenadas por capacidad descendente y precio ascendente) hasta cubrir los huéspedes solicitados.

| Modo de selección | Cuándo aparece | Acción |
|-------------------|----------------|--------|
| Reserva individual | Habitación cubre capacidad | Botón "Reservar" por habitación |
| Armar reserva grande | ≥ 2 habitaciones en sede | Checkboxes + medidor |
| Sugerir grupo | ≥ 2 habitaciones, grupo > 1 | Auto-selecciona combinación |
| Reservar toda la sede | ≥ 2 habitaciones | Pre-selecciona todas |

*Tabla 3. Modos de selección de habitaciones.*

<div class="pb"></div>

## Paso 4 — Favoritos

El asociado puede marcar habitaciones como favoritas haciendo clic en el **ícono de corazón** que aparece en la miniatura de cada habitación.

**Funcionamiento:**

1. Clic en el corazón → `POST` AJAX a `FavoritesController.ToggleRoom(roomId)`.
2. Si ya es favorita → se elimina (`DELETE` en tabla `Favorites`).
3. Si no es favorita → se agrega (`INSERT` en tabla `Favorites`).
4. Respuesta JSON con `{ isFavorite: true/false }`.
5. Feedback visual: corazón se llena/vacía + toast de confirmación.

**Persistencia:** Los favoritos se guardan en la tabla `Favorites` con índice único `(UserId, RoomId)` para evitar duplicados. Son visibles cada vez que el asociado busca disponibilidad.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Id` | INT | PK auto-incremento |
| `UserId` | NVARCHAR | FK → AspNetUsers |
| `RoomId` | INT | FK → Rooms |
| `CreatedAt` | DATETIME | Fecha UTC |

*Tabla 4. Estructura de la tabla Favorites.*

**Seguridad:** Solo usuarios autenticados (`[Authorize]`). Anti-forgery token obligatorio. Verificación de que la habitación existe y está activa.

<div class="pb"></div>

## Paso 5 — Confirmación y creación de reserva

Al seleccionar habitación(es), el asociado es redirigido a la pantalla de confirmación (`Reservations/Create`).

### Datos mostrados en resumen

- Habitación(es) seleccionada(s) con nombre, tipo y capacidad.
- Sede y alojamiento.
- Fechas de check-in y check-out.
- Número de noches.
- Desglose de tarifa por habitación (calculada por `sp_CalculateTotalRate`).
- Total general de la reserva.

### Datos solicitados al asociado

| Campo | Descripción |
|-------|-------------|
| Nombre del huésped | Nombre completo del titular |
| Email | Para envío de confirmación |
| Teléfono | Contacto (opcional) |
| Notas | Observaciones especiales |

*Tabla 5. Datos requeridos para crear la reserva.*

### Proceso de creación (backend)

```
1. Validación de entrada (FluentValidation)
2. BEGIN TRANSACTION (IsolationLevel.Serializable)
3. Para cada habitación:
   a. EXEC sp_ValidateOverbooking → ¿solapamiento?
   b. Si @IsOverbooked = 1 → ROLLBACK → error
   c. EXEC sp_CalculateTotalRate → precio final
   d. INSERT ReservationDetail
4. INSERT Reservation (estado = Pending, total = suma)
5. COMMIT TRANSACTION
6. Envío email de confirmación (MailKit SMTP)
7. Redirect → Detalles de la reserva
```

**Prevención de overbooking:** La transacción `SERIALIZABLE` bloquea las filas de habitaciones durante toda la verificación y creación, impidiendo que dos usuarios reserven la misma habitación simultáneamente.

<div class="pb"></div>

## Paso 6 — Mis reservas

El asociado puede consultar su historial de reservas desde el portal. Se muestran las últimas 10 reservas ordenadas por fecha de check-in descendente.

![Mis reservas](docs/screenshots/15-my-reservations.png)

*Figura 3. Historial de reservas del asociado con estado, fechas y total.*

### Información por reserva

| Dato | Descripción |
|------|-------------|
| # Reserva | ID único |
| Sede | Nombre de la sede |
| Habitación(es) | Números de habitación |
| Check-in / Check-out | Fechas de la estadía |
| Huéspedes | Número de personas |
| Total | Monto calculado por SPs |
| Estado | Pending, Confirmed, CheckedIn, CheckedOut, Cancelled, NoShow |
| Acciones | Ver detalles, Cancelar (si Pending) |

*Tabla 6. Datos mostrados en el historial de reservas del asociado.*

### Estados de la reserva (ciclo de vida)

```
  Pending ──► Confirmed ──► CheckedIn ──► CheckedOut
    │              │
    ▼              ▼
  Cancelled      NoShow
```

| Estado | Quién lo cambia | Descripción |
|--------|----------------|-------------|
| Pending | Sistema | Estado inicial al crear la reserva |
| Confirmed | Admin | Reserva verificada y confirmada |
| CheckedIn | Admin | Huésped se registró en la sede |
| CheckedOut | Admin | Huésped finalizó su estadía |
| Cancelled | Asociado o Admin | Reserva cancelada |
| NoShow | Admin | Huésped no se presentó |

*Tabla 7. Estados de reserva y responsables del cambio.*

<div class="pb"></div>

## Paso 7 — Cancelación de reserva

El asociado puede cancelar sus reservas **solo si están en estado Pending**.

**Reglas de cancelación:**

1. El asociado hace clic en "Cancelar" desde los detalles de la reserva.
2. `ReservationsController.Cancel(id)` verifica:
   - Que la reserva exista.
   - Que pertenezca al usuario actual (`reservation.UserId == currentUser`).
   - Que el estado sea `Pending` (si no es admin).
3. Si pasa validaciones: `_reservationService.CancelAsync(id)` → estado cambia a `Cancelled`.
4. Si el estado no es Pending: `TempData["Error"] = "Solo puedes cancelar reservas pendientes"`.

**Restricciones por rol:**

| Rol | Puede cancelar | Restricción de estado |
|-----|---------------|----------------------|
| Guest/Cliente | Solo sus propias reservas | Solo si es Pending |
| Admin | Cualquier reserva | Sin restricción de estado |

*Tabla 8. Permisos de cancelación por rol.*

**Nota:** Una vez la reserva pasa a `Confirmed`, `CheckedIn` o `CheckedOut`, el asociado no puede cancelarla. Solo un administrador puede hacerlo.

---

## Paso 8 — Recuperación de contraseña

Si el asociado olvida su contraseña:

1. Clic en "¿Olvidaste tu contraseña?" desde el login.
2. Ingresa su email registrado.
3. Identity genera un token de restablecimiento.
4. `IdentityEmailSender` construye email HTML con enlace seguro.
5. `EmailService` envía via SMTP (MailKit).
6. Asociado recibe enlace → formulario de nueva contraseña.
7. Token se valida y contraseña se actualiza.

![Recuperar contraseña](docs/screenshots/16-forgot-password.png)

*Figura 4. Formulario de recuperación de contraseña vía email SMTP.*

<div class="pb"></div>

## Resumen del flujo completo

```
Asociado (Guest/Cliente)
  │
  ├─► LOGIN
  │     └─► Portal personalizado ("Hola, [nombre]")
  │
  ├─► BUSCAR DISPONIBILIDAD
  │     ├─► Seleccionar sede (o todas)
  │     ├─► Seleccionar fechas (check-in / check-out)
  │     ├─► Seleccionar huéspedes
  │     └─► Resultados AJAX (SP1 + SP2 + SP4)
  │
  ├─► EXPLORAR RESULTADOS
  │     ├─► Vista tabla / lista
  │     ├─► Habitaciones agrupadas por sede
  │     ├─► Precios calculados por noche y total
  │     └─► Etiquetas: "Mejor opción" / "Solo grupo"
  │
  ├─► MARCAR FAVORITOS
  │     ├─► Clic en corazón (toggle AJAX)
  │     ├─► Persistencia en tabla Favorites
  │     └─► Visibles en futuras búsquedas
  │
  ├─► SELECCIONAR HABITACIÓN(ES)
  │     ├─► Individual: botón "Reservar"
  │     ├─► Grupal: checkboxes + medidor
  │     ├─► Sugerir grupo (auto-select)
  │     └─► Reservar toda la sede
  │
  ├─► CONFIRMAR RESERVA
  │     ├─► Resumen de costos (SP4)
  │     ├─► Datos del huésped
  │     ├─► Transacción SERIALIZABLE
  │     ├─► Validación overbooking (SP5)
  │     └─► Email de confirmación (SMTP)
  │
  ├─► MIS RESERVAS
  │     ├─► Historial (últimas 10)
  │     ├─► Estado actual
  │     ├─► Ver detalles
  │     └─► Cancelar (solo Pending)
  │
  ├─► CANCELAR RESERVA
  │     ├─► Solo estado Pending
  │     ├─► Verificación de propiedad
  │     └─► Cambio a estado Cancelled
  │
  └─► CERRAR SESIÓN
```

| Paso | Acción | Endpoint | SP utilizado |
|------|--------|----------|-------------|
| 1 | Buscar disponibilidad | `POST /Home/SearchAvailability` | SP1, SP2, SP4 |
| 2 | Ver resultados | Partial view AJAX | — |
| 3 | Marcar favorito | `POST /Favorites/ToggleRoom` | — |
| 4 | Reservar individual | `GET /Reservations/Create?roomId=` | SP4, SP5 |
| 5 | Reservar grupo | `GET /Reservations/Create?roomIds=` | SP4, SP5 |
| 6 | Confirmar reserva | `POST /Reservations/Create` | SP4, SP5 |
| 7 | Ver mis reservas | Portal (Home/Index) | — |
| 8 | Cancelar reserva | `POST /Reservations/Cancel/{id}` | — |
| 9 | Recuperar contraseña | Identity ForgotPassword | — |

*Tabla 9. Resumen de endpoints y SPs por paso del flujo del asociado.*
