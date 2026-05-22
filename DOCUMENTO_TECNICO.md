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
  @page:first {
    margin: 0;
    @top-center { content: none; }
    @bottom-center { content: none; }
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

  /* ═══════════════════════════════════════════════════════ */
  /*  PORTADA — CORPORATE WHITEPAPER                        */
  /* ═══════════════════════════════════════════════════════ */

  .cover {
    page-break-after: always;
    width: 210mm;
    height: 297mm;
    margin: -20mm -22mm;
    padding: 0;
    display: flex;
    flex-direction: column;
    position: relative;
    overflow: hidden;
    background: #fff;
  }
  .cover-bar {
    width: 100%;
    height: 6px;
    background: var(--c-accent);
  }
  .cover-body {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
    padding: 38mm 35mm 30mm 35mm;
  }
  .cover-label {
    font-size: 9pt;
    font-weight: 600;
    letter-spacing: 3px;
    text-transform: uppercase;
    color: var(--c-accent);
    margin-bottom: 12mm;
  }
  .cover-title {
    font-size: 28pt;
    font-weight: 700;
    color: var(--c-text);
    line-height: 1.15;
    margin: 0 0 4mm 0;
    border: none !important;
    border-bottom: none !important;
    padding: 0;
  }
  .cover-sub {
    font-size: 14pt;
    font-weight: 400;
    color: var(--c-muted);
    margin: 0 0 20mm 0;
  }
  .cover-rule {
    width: 40mm;
    height: 1.5px;
    background: var(--c-accent);
    border: none;
    margin: 0 0 10mm 0;
  }
  .cover-meta {
    font-size: 10pt;
    line-height: 1.8;
    color: var(--c-muted);
  }
  .cover-meta strong { color: var(--c-text); font-weight: 600; }
  .cover-stack {
    margin-top: 10mm;
    font-size: 9pt;
    color: var(--c-muted);
    line-height: 1.6;
  }
  .cover-footer {
    padding: 8mm 35mm;
    border-top: 1px solid #e0e0e0;
    font-size: 7.5pt;
    color: #aaa;
    text-align: center;
    letter-spacing: 0.5px;
  }

  /* ═══════════════════════════════════════════════════════ */
  /*  TABLA DE CONTENIDO — BOOK STYLE                       */
  /* ═══════════════════════════════════════════════════════ */

  .toc {
    page-break-after: always;
  }
  .toc h1 {
    font-size: 16pt;
    border-bottom: 2px solid var(--c-accent);
    padding-bottom: 4mm;
    margin-bottom: 8mm;
    color: var(--c-accent);
  }
  .toc-entry {
    display: flex;
    align-items: baseline;
    font-size: 10.5pt;
    line-height: 2.2;
    color: var(--c-text);
    text-decoration: none;
  }
  .toc-num {
    font-weight: 600;
    min-width: 8mm;
    color: var(--c-accent);
  }
  .toc-title {
    flex-shrink: 0;
  }
  .toc-dots {
    flex: 1;
    border-bottom: 1px dotted #bbb;
    margin: 0 2mm;
    min-width: 10mm;
  }
  .toc-page {
    flex-shrink: 0;
    font-weight: 600;
    color: var(--c-accent);
    min-width: 6mm;
    text-align: right;
  }

  /* ═══════════════════════════════════════════════════════ */
  /*  HEADINGS                                              */
  /* ═══════════════════════════════════════════════════════ */

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

  /* ═══════════════════════════════════════════════════════ */
  /*  TEXT                                                  */
  /* ═══════════════════════════════════════════════════════ */

  p {
    margin: 0 0 2.5mm 0;
    text-align: justify;
    hyphens: auto;
  }
  ul, ol { margin: 0 0 3mm 0; padding-left: 6mm; }
  li { margin-bottom: 1mm; }
  strong { font-weight: 600; }

  /* ═══════════════════════════════════════════════════════ */
  /*  TABLES — PRINT STYLE                                  */
  /* ═══════════════════════════════════════════════════════ */

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

  /* ═══════════════════════════════════════════════════════ */
  /*  IMAGES — PRINT EDITORIAL                              */
  /* ═══════════════════════════════════════════════════════ */

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

  /* 2-up gallery for compact evidence */
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

  /* ═══════════════════════════════════════════════════════ */
  /*  CAPTIONS (em after image / table)                     */
  /* ═══════════════════════════════════════════════════════ */

  p > em:only-child {
    display: block;
    font-size: 9pt;
    color: var(--c-muted);
    text-align: center;
    margin: 1mm 0 4mm 0;
    font-style: italic;
    line-height: 1.3;
  }

  /* Standalone image + caption pairs: keep together on page */
  p:has(> img) {
    page-break-inside: avoid;
    text-align: center;
    margin: 4mm 0 1mm 0;
  }
  p:has(> img) + p {
    page-break-before: avoid;
  }

  /* ═══════════════════════════════════════════════════════ */
  /*  CODE BLOCKS                                           */
  /* ═══════════════════════════════════════════════════════ */

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

  /* ═══════════════════════════════════════════════════════ */
  /*  HR                                                    */
  /* ═══════════════════════════════════════════════════════ */

  hr {
    border: none;
    height: 0.5pt;
    background: var(--c-rule);
    margin: 5mm 0;
  }

  /* ═══════════════════════════════════════════════════════ */
  /*  BLOCKQUOTE — NOTE BOX                                 */
  /* ═══════════════════════════════════════════════════════ */

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

  /* ═══════════════════════════════════════════════════════ */
  /*  PAGE BREAKS                                           */
  /* ═══════════════════════════════════════════════════════ */

  .pb { page-break-after: always; }
</style>

<!-- ══════════════════════════════════════════════════════════ -->
<!--                     PORTADA                               -->
<!-- ══════════════════════════════════════════════════════════ -->

<div class="cover">
  <div class="cover-bar"></div>
  <div class="cover-body">
    <div class="cover-label">Prueba Técnica · Analista Desarrollador .NET</div>
    <h1 class="cover-title">Sistema de Reservas XYZ</h1>
    <p class="cover-sub">Documento Técnico — Sistema de Gestión y Reservas Hoteleras</p>
    <hr class="cover-rule">
    <div class="cover-meta">
      <strong>Autor</strong> &emsp; Juan Campo<br>
      <strong>Cargo</strong> &emsp; Analista Desarrollador .NET<br>
      <strong>Versión</strong> &emsp; 1.0<br>
      <strong>Fecha</strong> &emsp; Mayo 2026<br>
      <strong>Ciudad</strong> &emsp; Colombia
    </div>
    <div class="cover-stack">
      ASP.NET Core 8.0 MVC &nbsp;·&nbsp; Entity Framework Core 8.0 &nbsp;·&nbsp; SQL Server 2022<br>
      ASP.NET Core Identity &nbsp;·&nbsp; MailKit SMTP &nbsp;·&nbsp; Docker &nbsp;·&nbsp; Nginx &nbsp;·&nbsp; Let's Encrypt<br>
      Tailwind CSS v4 &nbsp;·&nbsp; FluentValidation &nbsp;·&nbsp; AutoMapper
    </div>
  </div>
  <div class="cover-footer">DOCUMENTO CONFIDENCIAL — SOLO PARA EVALUACIÓN TÉCNICA — FONDO DE EMPLEADOS XYZ (FODUN)</div>
</div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--               TABLA DE CONTENIDO                          -->
<!-- ══════════════════════════════════════════════════════════ -->

<div class="toc">

<h1>Tabla de Contenido</h1>

<a class="toc-entry" href="#1-introducción"><span class="toc-num">1</span><span class="toc-title">Introducción</span><span class="toc-dots"></span><span class="toc-page">3</span></a>
<a class="toc-entry" href="#2-arquitectura-del-proyecto"><span class="toc-num">2</span><span class="toc-title">Arquitectura del Proyecto</span><span class="toc-dots"></span><span class="toc-page">5</span></a>
<a class="toc-entry" href="#3-tecnologías-utilizadas"><span class="toc-num">3</span><span class="toc-title">Tecnologías Utilizadas</span><span class="toc-dots"></span><span class="toc-page">7</span></a>
<a class="toc-entry" href="#4-modelo-relacional"><span class="toc-num">4</span><span class="toc-title">Modelo Relacional</span><span class="toc-dots"></span><span class="toc-page">9</span></a>
<a class="toc-entry" href="#5-base-de-datos"><span class="toc-num">5</span><span class="toc-title">Base de Datos</span><span class="toc-dots"></span><span class="toc-page">11</span></a>
<a class="toc-entry" href="#6-stored-procedures"><span class="toc-num">6</span><span class="toc-title">Stored Procedures</span><span class="toc-dots"></span><span class="toc-page">13</span></a>
<a class="toc-entry" href="#7-identity-y-seguridad"><span class="toc-num">7</span><span class="toc-title">Identity y Seguridad</span><span class="toc-dots"></span><span class="toc-page">17</span></a>
<a class="toc-entry" href="#8-flujo-funcional"><span class="toc-num">8</span><span class="toc-title">Flujo Funcional</span><span class="toc-dots"></span><span class="toc-page">19</span></a>
<a class="toc-entry" href="#9-servicio-de-email-smtp"><span class="toc-num">9</span><span class="toc-title">Servicio de Email (SMTP)</span><span class="toc-dots"></span><span class="toc-page">24</span></a>
<a class="toc-entry" href="#10-docker-y-contenedorización"><span class="toc-num">10</span><span class="toc-title">Docker y Contenedorización</span><span class="toc-dots"></span><span class="toc-page">25</span></a>
<a class="toc-entry" href="#11-despliegue-en-vps"><span class="toc-num">11</span><span class="toc-title">Despliegue en VPS</span><span class="toc-dots"></span><span class="toc-page">27</span></a>
<a class="toc-entry" href="#12-instrucciones-de-ejecución"><span class="toc-num">12</span><span class="toc-title">Instrucciones de Ejecución</span><span class="toc-dots"></span><span class="toc-page">29</span></a>
<a class="toc-entry" href="#13-conclusiones"><span class="toc-num">13</span><span class="toc-title">Conclusiones</span><span class="toc-dots"></span><span class="toc-page">31</span></a>

</div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--                 1. INTRODUCCIÓN                           -->
<!-- ══════════════════════════════════════════════════════════ -->

# 1. Introducción

## 1.1 Objetivo

Desarrollar un sistema web completo de gestión de reservas hoteleras para el Fondo de Empleados XYZ (FODUN), que permita a los asociados consultar disponibilidad de sedes recreativas y apartamentos, visualizar tarifas según temporada y número de personas, y realizar reservas en línea.

## 1.2 Contexto del negocio

El Fondo XYZ cuenta con 8 sedes distribuidas en diferentes ciudades de Colombia:

| # | Sede | Ciudad | Capacidad |
|---|------|--------|-----------|
| 1 | Villeta | Villeta | 32 personas |
| 2 | El Placer | Fusagasugá | 34 personas |
| 3 | Gonzalo Morante | Chinchiná | 30 personas |
| 4 | Tablones | Palmira | 24 personas |
| 5 | Manguruma | Santa Fe de Antioquia | 46 personas |
| 6 | Federmán | Bogotá | 8 personas |
| 7 | Suramericana | Medellín | 9 personas |
| 8 | El Rodadero | Santa Marta | 20 personas |

*Tabla 1. Sedes recreativas del Fondo de Empleados XYZ.*

## 1.3 Necesidad

Los asociados requieren un sistema web para:

- Consultar la disponibilidad de alojamientos según fechas de viaje.
- Visualizar tarifas por sitio, temporada, número de personas y tipo de alojamiento.
- Calcular el costo total de su estadía.
- Realizar y gestionar reservas de forma autónoma.

## 1.4 Pantalla principal

![Pantalla principal del sistema](docs/screenshots/01-home.png)

*Figura 1. Landing page del sistema ReservasXYZ con las 8 sedes disponibles, tarifas de referencia y accesos a registro y login.*

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--            2. ARQUITECTURA DEL PROYECTO                   -->
<!-- ══════════════════════════════════════════════════════════ -->

# 2. Arquitectura del Proyecto

## 2.1 Patrón arquitectónico

El proyecto implementa **Clean Architecture** con separación en cuatro capas independientes, siguiendo los principios SOLID y el patrón de inversión de dependencias.

## 2.2 Diagrama de capas

```
 ┌──────────────────────────────────────────────────────┐
 │                   ReservasXYZ.Web                     │
 │  Controllers MVC · Razor Views · Identity Pages      │
 │  wwwroot (CSS/JS) · DI Configuration · Middleware    │
 ├──────────────────────────────────────────────────────┤
 │                ReservasXYZ.Application                │
 │     DTOs · Servicios · Validadores · AutoMapper      │
 ├──────────────────────────────────────────────────────┤
 │               ReservasXYZ.Infrastructure             │
 │  DbContext · Repositorios · Email · Seed · SPs · DI │
 ├──────────────────────────────────────────────────────┤
 │                  ReservasXYZ.Domain                   │
 │           Entidades · Enums · Interfaces             │
 └──────────────────────────────────────────────────────┘
```

## 2.3 Flujo de dependencias

```
Web ──→ Application ──→ Domain ←── Infrastructure
```

- **Domain** no depende de ninguna otra capa (capa pura de negocio).
- **Application** depende exclusivamente de Domain.
- **Infrastructure** implementa las interfaces definidas en Domain.
- **Web** orquesta todo a través de inyección de dependencias.

## 2.4 Responsabilidades por capa

| Capa | Proyecto | Responsabilidad |
|------|----------|----------------|
| Presentación | `ReservasXYZ.Web` | Controllers MVC, Razor Views, Identity Pages, middleware, DI, wwwroot |
| Aplicación | `ReservasXYZ.Application` | DTOs, servicios de aplicación, validadores FluentValidation, AutoMapper |
| Infraestructura | `ReservasXYZ.Infrastructure` | DbContext, repositorios, email (MailKit), DataSeeder, Fluent API, Identity |
| Dominio | `ReservasXYZ.Domain` | Entidades de negocio, enums, interfaces de repositorio |

*Tabla 2. Responsabilidades por capa de la arquitectura Clean Architecture.*

## 2.5 Estructura del proyecto

```
ReservasXYZ.sln
├── ReservasXYZ.Domain/
│   ├── Entities/          → Site, Accommodation, Room, Season, Rate,
│   │                        Reservation, ReservationDetail, Favorite
│   ├── Enums/             → RoomType, ReservationStatus, RateKind
│   └── Interfaces/        → IRepository<T>, IRoomRepository, IRateRepository
│
├── ReservasXYZ.Application/
│   ├── DTOs/              → SiteDtos, RoomDtos, ReservationDtos, SeasonRateDtos
│   ├── Interfaces/        → ISiteService, IRoomService, IReservationService
│   ├── Services/          → SiteService, RoomService, ReservationService
│   ├── Validators/        → FluentValidation por DTO
│   └── Mappings/          → AutoMapper MappingProfile
│
├── ReservasXYZ.Infrastructure/
│   ├── Data/              → DbContext, Config, Repositories, Seed
│   ├── Email/             → EmailService, EmailTemplateService
│   ├── Identity/          → SpanishIdentityErrorDescriber
│   ├── Services/          → DashboardService, GuestPortalService
│   └── StoredProcedures/  → StoredProcedures.sql (6 SPs)
│
├── ReservasXYZ.Web/
│   ├── Controllers/       → CRUD Controllers + API
│   ├── Areas/Identity/    → Login, Register, ForgotPassword
│   ├── Views/             → Razor Views + Shared layout
│   └── wwwroot/           → CSS (Tailwind), JS, imágenes
│
├── database/              → Database.sql, SeedCatalog.sql
├── Dockerfile             → Multi-stage build
└── docker-compose.yml     → 4 servicios
```

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--            3. TECNOLOGÍAS UTILIZADAS                      -->
<!-- ══════════════════════════════════════════════════════════ -->

# 3. Tecnologías Utilizadas

## 3.1 Stack tecnológico

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET SDK | 8.0 | Runtime y compilación |
| ASP.NET Core MVC | 8.0 | Framework web |
| Razor Views / Pages | 8.0 | Motor de vistas server-side |
| Entity Framework Core | 8.0.25 | ORM y migraciones |
| SQL Server | 2022 Express | Motor de base de datos |
| ASP.NET Core Identity | 8.0.25 | Autenticación y autorización |
| AutoMapper | 13.x | Mapeo entidades ↔ DTOs |
| FluentValidation | 11.x | Validación declarativa |
| MailKit | 4.x | Cliente SMTP |
| Tailwind CSS | 4.3.0 | Framework CSS |
| Node.js | 20+ | Build de Tailwind CSS |
| Docker | 29.x | Contenedores |
| Docker Compose | v2 | Orquestación |
| Nginx | Alpine | Reverse proxy + SSL |
| Certbot | Latest | Certificados Let's Encrypt |

*Tabla 3. Stack tecnológico del sistema ReservasXYZ.*

## 3.2 Patrones de diseño

| Patrón | Implementación |
|--------|---------------|
| Repository Pattern | `IRepository<T>` genérico + repositorios especializados |
| Unit of Work | `ApplicationDbContext.SaveChangesAsync()` |
| DTO Pattern | Objetos separados para Create, Update y View |
| Dependency Injection | Contenedor nativo de ASP.NET Core |
| Strategy Pattern | Motor de tarificación con Kind (Standard/SpecialWeekday) |
| Template Method | EmailTemplateService para tipos de email |
| Soft Delete | `IsActive = false` en lugar de eliminar registros |

*Tabla 4. Patrones de diseño implementados.*

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--              4. MODELO RELACIONAL                         -->
<!-- ══════════════════════════════════════════════════════════ -->

# 4. Modelo Relacional

## 4.1 Entidades del dominio

```
Sites (Sedes)
  └── Accommodations (Alojamientos)
        └── Rooms (Habitaciones)
              ├── Rates (Tarifas) ←─── Seasons (Temporadas)
              ├── ReservationDetails ←─ Reservations ← ApplicationUser
              └── Favorites ←────────── ApplicationUser
```

## 4.2 Relaciones

| Relación | Tipo | FK | Descripción |
|----------|------|----|-------------|
| Site → Accommodation | 1:N | `SiteId` | Una sede tiene múltiples alojamientos |
| Accommodation → Room | 1:N | `AccommodationId` | Un alojamiento tiene múltiples habitaciones |
| Room → Rate | 1:N | `RoomId` | Una habitación tiene tarifas por temporada |
| Season → Rate | 1:N | `SeasonId` | Una temporada define tarifas múltiples |
| User → Reservation | 1:N | `UserId` | Un usuario tiene múltiples reservas |
| Reservation → Detail | 1:N | `ReservationId` | Una reserva incluye múltiples habitaciones |
| Room → Detail | 1:N | `RoomId` | Una habitación en múltiples reservas |
| User → Favorite | 1:N | `UserId` | Favoritos por usuario |
| Room → Favorite | 1:N | `RoomId` | Habitaciones marcadas |

*Tabla 5. Relaciones entre entidades del modelo de datos.*

## 4.3 Índices

| Tabla | Índice | Tipo | Columnas |
|-------|--------|------|----------|
| Rooms | `IX_Rooms_Accommodation_Number` | UNIQUE | (AccommodationId, RoomNumber) |
| Rates | `IX_Rates_Room_Season_Kind` | UNIQUE | (RoomId, SeasonId, Kind) |
| Favorites | `IX_Favorites_User_Room` | UNIQUE | (UserId, RoomId) |
| ReservationDetails | `IX_ReservationDetail_Room` | INDEX | (RoomId) |

*Tabla 6. Índices para optimización de consultas.*

## 4.4 Constraints de integridad

- **CASCADE DELETE**: Site → Accommodations → Rooms.
- **RESTRICT DELETE**: Room → ReservationDetails.
- **SOFT DELETE**: `IsActive` flag en todas las entidades.
- **CHECK**: `CheckOut > CheckIn` validado en capa de aplicación.

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--                5. BASE DE DATOS                           -->
<!-- ══════════════════════════════════════════════════════════ -->

# 5. Base de Datos

## 5.1 Diseño

Base de datos normalizada (3NF). **14 tablas** en total: 9 de dominio + 5 de Identity.

**Dominio:** Sites, Accommodations, Rooms, Seasons, Rates, Reservations, ReservationDetails, Favorites, ApplicationUser.

**Identity:** AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetUserTokens.

## 5.2 Estrategia de reservas

1. `Reservation` — entidad padre con datos generales (fechas, usuario, total, estado).
2. `ReservationDetail` — habitación individual dentro de la reserva.
3. Precio calculado y almacenado al crear (snapshot de precio).
4. Flujo: `Pending → Confirmed → CheckedIn → CheckedOut`.

## 5.3 Prevención de overbooking

Doble control implementado:

1. **SQL**: SP `sp_ValidateOverbooking` verifica solapamiento de fechas.
2. **Aplicación**: Transacción `SERIALIZABLE` que bloquea registros durante verificación y creación.

```csharp
using var transaction = await _context.Database
    .BeginTransactionAsync(IsolationLevel.Serializable);
// 1. Verificar solapamiento → 2. Crear reserva → 3. Commit/Rollback
```

## 5.4 Catálogo FODUN

Script `SeedCatalog.sql` idempotente que carga: 3 temporadas, 8 sedes, 10 alojamientos, 54 habitaciones y tarifas completas con reglas de BaseGuests, ExtraPersonPrice y SpecialWeekday.

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--              6. STORED PROCEDURES                         -->
<!-- ══════════════════════════════════════════════════════════ -->

# 6. Stored Procedures

Se implementaron **6 Stored Procedures** — 4 requeridos + 2 auxiliares.

## 6.1 SP1 — sp_GetAvailableRoomsByDates

**Propósito:** Habitaciones disponibles en un rango de fechas.

| Parámetro | Tipo | Dir. | Descripción |
|-----------|------|------|-------------|
| `@CheckIn` | DATE | IN | Fecha de entrada |
| `@CheckOut` | DATE | IN | Fecha de salida |

*Tabla 7. Parámetros de sp_GetAvailableRoomsByDates.*

Selecciona habitaciones activas sin reservas solapadas. Condición: `CheckIn < @CheckOut AND CheckOut > @CheckIn`.

```sql
EXEC sp_GetAvailableRoomsByDates
    @CheckIn = '2026-06-01', @CheckOut = '2026-06-05';
```

## 6.2 SP2 — sp_GetAvailableRoomsByDatesAndGuests

**Propósito:** Disponibilidad por fechas + capacidad mínima.

| Parámetro | Tipo | Dir. | Descripción |
|-----------|------|------|-------------|
| `@CheckIn` | DATE | IN | Fecha de entrada |
| `@CheckOut` | DATE | IN | Fecha de salida |
| `@Guests` | INT | IN | Número de huéspedes |

*Tabla 8. Parámetros de sp_GetAvailableRoomsByDatesAndGuests.*

Misma lógica que SP1 + filtro `MaxGuests >= @Guests`.

## 6.3 SP3 — sp_GetRoomRates

**Propósito:** Tarifas configuradas para una habitación.

| Parámetro | Tipo | Dir. | Descripción |
|-----------|------|------|-------------|
| `@RoomId` | INT | IN | ID de la habitación |

*Tabla 9. Parámetros de sp_GetRoomRates.*

## 6.4 SP4 — sp_CalculateTotalRate

**Propósito:** Tarifa total de una estadía aplicando reglas FODUN.

| Parámetro | Tipo | Dir. | Descripción |
|-----------|------|------|-------------|
| `@RoomId` | INT | IN | ID de la habitación |
| `@CheckIn` | DATE | IN | Fecha de entrada |
| `@CheckOut` | DATE | IN | Fecha de salida |
| `@TotalGuests` | INT | IN | Número de huéspedes |
| `@TotalRate` | DECIMAL(18,2) | OUT | Tarifa calculada |

*Tabla 10. Parámetros de sp_CalculateTotalRate.*

**Motor de tarificación:**

1. `SET DATEFIRST 1` — lunes = día 1.
2. Itera cada noche del rango `[CheckIn, CheckOut)`.
3. Para cada noche: determina si es Lun-Jue, busca tarifa activa vigente.
4. Si Lun-Jue **y** existe `SpecialWeekday` **y** `PriceMultiplier ≤ 1`: usa tarifa especial.
5. Si no: usa Standard × Multiplier.
6. Suma extras: `MAX(0, TotalGuests − BaseGuests) × ExtraPersonPrice`.
7. Fallback a `Room.BasePrice` si no hay tarifa configurada.

```sql
DECLARE @Total DECIMAL(18,2);
EXEC sp_CalculateTotalRate @RoomId=1, @CheckIn='2026-06-01',
    @CheckOut='2026-06-04', @TotalGuests=6, @TotalRate=@Total OUTPUT;
SELECT @Total AS TarifaTotal;
```

**Ejemplo** — 3 noches, 6 personas, temporada baja, Villeta:

| Noche | Día | Tarifa | Extra (2 pers) | Total |
|-------|-----|--------|----------------|-------|
| 1 | Lunes | $27.000 | 2 × $11.000 | $49.000 |
| 2 | Martes | $27.000 | 2 × $11.000 | $49.000 |
| 3 | Miércoles | $27.000 | 2 × $11.000 | $49.000 |
| **Total** | | | | **$147.000** |

*Tabla 11. Ejemplo de cálculo de tarifa para 3 noches con SP4.*

## 6.5 SP5 — sp_ValidateOverbooking

Verifica solapamiento de fechas para prevenir overbooking. Parámetro `@IsOverbooked` (BIT, OUT).

## 6.6 SP6 — sp_GetRatesByCriteria

Consulta avanzada cruzando sitio, temporada, alojamiento y personas. Todos los parámetros opcionales. Retorna `EffectivePricePerNight`.

## 6.7 Seguridad en SPs

- Parámetros tipados (prevención de SQL Injection).
- `SET NOCOUNT ON` en todos los SPs.
- Invocación desde EF Core con `SqlParameter` tipados.

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--            7. IDENTITY Y SEGURIDAD                        -->
<!-- ══════════════════════════════════════════════════════════ -->

# 7. Identity y Seguridad

## 7.1 Configuración

| Parámetro | Valor |
|-----------|-------|
| Longitud mínima contraseña | 8 caracteres |
| Requiere mayúscula | Sí |
| Requiere minúscula | Sí |
| Requiere dígito | Sí |
| Requiere carácter especial | Sí |
| Intentos antes de bloqueo | 5 |
| Tiempo de bloqueo | 15 minutos |

*Tabla 12. Políticas de seguridad en ASP.NET Core Identity.*

## 7.2 Roles

| Rol | Acceso |
|-----|--------|
| Admin | CRUD completo, gestión de reservas, dashboard |
| Receptionist | Gestión de reservas, check-in/check-out |
| Guest / Cliente | Disponibilidad, reservas propias, favoritos |

*Tabla 13. Roles del sistema.*

## 7.3 Controles OWASP

| Vulnerabilidad | Control |
|---------------|---------|
| A01: Broken Access Control | `[Authorize(Roles = "Admin")]`, verificación de propiedad |
| A02: Cryptographic Failures | PBKDF2 con salt aleatorio |
| A03: Injection | Queries parametrizadas, `SqlParameter` tipados |
| A05: Security Misconfiguration | Secrets en variables de entorno |
| A07: Auth Failures | Bloqueo tras 5 intentos, mensajes genéricos |
| A08: Data Integrity | `[ValidateAntiForgeryToken]`, tokens anti-CSRF |

*Tabla 14. Controles de seguridad según OWASP Top 10.*

## 7.4 Autenticación y registro

<div class="fig-row">
<div class="fig">

![Login](docs/screenshots/02-login.png)

<div class="fig-caption">Figura 2. Formulario de inicio de sesión.</div>
</div>
<div class="fig">

![Registro](docs/screenshots/03-registro.png)

<div class="fig-caption">Figura 3. Formulario de registro de usuario.</div>
</div>
</div>

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--              8. FLUJO FUNCIONAL                           -->
<!-- ══════════════════════════════════════════════════════════ -->

# 8. Flujo Funcional

## 8.1 Flujo general

```
Usuario anónimo
  ├─► Registro → Confirmación email
  ├─► Login
  │     ├─► ¿Olvidó contraseña? → Email SMTP → Reset
  │     └─► Autenticado:
  │           ├─► [Cliente]
  │           │     ├─► Búsqueda disponibilidad → Selección → Tarifa → Reserva
  │           │     ├─► Mis reservas
  │           │     └─► Favoritos
  │           └─► [Admin]
  │                 ├─► Dashboard
  │                 ├─► CRUD Sedes/Alojamientos/Habitaciones
  │                 ├─► CRUD Temporadas/Tarifas
  │                 └─► Gestión Reservas
  └─► Cerrar sesión
```

## 8.2 Estados de reserva

```
  Pending ──► Confirmed ──► CheckedIn ──► CheckedOut
    │              │
    ▼              ▼
  Cancelled      NoShow
```

## 8.3 Panel administrativo

<div class="fig-row">
<div class="fig">

![Dashboard](docs/screenshots/04-dashboard-admin.png)

<div class="fig-caption">Figura 4. Dashboard con métricas de reservas, ingresos y ocupación.</div>
</div>
<div class="fig">

![Sedes](docs/screenshots/05-sites-crud.png)

<div class="fig-caption">Figura 5. CRUD de sedes recreativas.</div>
</div>
</div>

<div class="fig-row">
<div class="fig">

![Alojamientos](docs/screenshots/06-accommodations-crud.png)

<div class="fig-caption">Figura 6. CRUD de alojamientos.</div>
</div>
<div class="fig">

![Habitaciones](docs/screenshots/07-rooms-crud.png)

<div class="fig-caption">Figura 7. CRUD de habitaciones.</div>
</div>
</div>

<div class="fig-row">
<div class="fig">

![Temporadas](docs/screenshots/08-seasons-crud.png)

<div class="fig-caption">Figura 8. CRUD de temporadas.</div>
</div>
<div class="fig">

![Tarifas](docs/screenshots/09-rates-crud.png)

<div class="fig-caption">Figura 9. CRUD de tarifas.</div>
</div>
</div>

## 8.4 Gestión de reservas

![Gestión de reservas](docs/screenshots/10-reservations-crud.png)

*Figura 10. Panel de gestión de reservas con filtros por estado y acciones de confirmación, check-in/out y cancelación.*

## 8.5 Portal del asociado

<div class="fig-row">
<div class="fig">

![Disponibilidad](docs/screenshots/11-availability-search.png)

<div class="fig-caption">Figura 11. Consulta de disponibilidad con precios calculados por SPs.</div>
</div>
<div class="fig">

![Mis reservas](docs/screenshots/15-my-reservations.png)

<div class="fig-caption">Figura 12. Historial de reservas del asociado.</div>
</div>
</div>

## 8.6 Recuperación de contraseña

![Recuperar contraseña](docs/screenshots/16-forgot-password.png)

*Figura 13. Formulario de recuperación de contraseña vía email SMTP.*

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--            9. SERVICIO DE EMAIL (SMTP)                    -->
<!-- ══════════════════════════════════════════════════════════ -->

# 9. Servicio de Email (SMTP)

## 9.1 Arquitectura

```
IEmailSender<ApplicationUser> (Identity)
  └── IdentityEmailSender
        ├── Detecta tipo (confirmación, reset, notificación)
        ├── Construye plantilla HTML (EmailTemplateService)
        └── Envía via SMTP (EmailService → MailKit)
```

## 9.2 Flujo de recuperación

1. Usuario ingresa email en ForgotPassword.
2. Identity genera token de restablecimiento.
3. IdentityEmailSender construye email con plantilla HTML.
4. EmailService conecta al servidor SMTP y envía.
5. Usuario recibe enlace con token → ResetPassword.

## 9.3 Protocolos

| Puerto | Protocolo | Comportamiento |
|--------|-----------|----------------|
| 465 | SSL | Conexión directa (`SslOnConnect`) |
| 587 | STARTTLS | Plain + upgrade a TLS (`StartTls`) |
| Otro | Auto | Detección automática |

*Tabla 15. Protocolos SMTP soportados.*

## 9.4 Seguridad

- Credenciales SMTP en variables de entorno (no en código fuente).
- Enlaces codificados con `HtmlEncoder.Default.Encode()` para prevenir XSS.
- `appsettings.Development.json` excluido de Git via `.gitignore`.

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--          10. DOCKER Y CONTENEDORIZACIÓN                   -->
<!-- ══════════════════════════════════════════════════════════ -->

# 10. Docker y Contenedorización

## 10.1 Multi-stage build

```dockerfile
# Etapa 1: Build (mcr.microsoft.com/dotnet/sdk:8.0)
#   → Restore NuGet (capas cacheadas)
#   → Node.js 20 via NodeSource → npm ci → css:build
#   → dotnet publish -c Release → /app/publish

# Etapa 2: Runtime (mcr.microsoft.com/dotnet/aspnet:8.0)
#   → Copia artefactos (~220 MB)
#   → ASPNETCORE_ENVIRONMENT=Production
#   → ENTRYPOINT ["dotnet", "ReservasXYZ.Web.dll"]
```

## 10.2 Servicios

| Servicio | Imagen | Puerto | Health check |
|----------|--------|--------|-------------|
| `sqlserver` | mssql/server:2022-latest | 1433 | sqlcmd cada 15s |
| `web` | Build local | 5000 | depends_on: sqlserver |
| `nginx` | nginx:alpine | 80, 443 | — |
| `certbot` | certbot/certbot | — | Renewal 6h |

*Tabla 16. Servicios Docker Compose.*

## 10.3 Variables de entorno

```env
SA_PASSWORD=***          SMTP_SERVER=***
SMTP_PORT=587            SMTP_SENDER_NAME=***
SMTP_EMAIL=***           SMTP_PASSWORD=***
DOMAIN=***
```

## 10.4 Evidencia de producción

```
NAMES              STATUS                 PORTS
reservas-certbot   Up 2h                  80, 443
reservas-nginx     Up 2h                  0.0.0.0:80→80, 0.0.0.0:443→443
reservas-web       Up 2h                  0.0.0.0:5000→5000
reservas-db        Up 2h (healthy)        0.0.0.0:1433→1433
```

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--             11. DESPLIEGUE EN VPS                         -->
<!-- ══════════════════════════════════════════════════════════ -->

# 11. Despliegue en VPS

## 11.1 Infraestructura

| Componente | Detalle |
|-----------|---------|
| Proveedor | Google Cloud Platform (GCE) |
| Instancia | e2-medium (2 vCPU, 4 GB RAM, 40 GB SSD) |
| SO | Ubuntu 22.04 LTS |
| Runtime | Docker 29.5.2 + Docker Compose v2 |
| Reverse Proxy | Nginx Alpine |
| SSL | Let's Encrypt via Certbot (renovación cada 6h) |
| DNS | Hostinger (registro A → IP estática GCP) |
| URL | https://reservas-xyz.app-dev.icu |

*Tabla 17. Infraestructura de producción.*

## 11.2 Seguridad VPS

- Firewall GCP: solo puertos 22, 80, 443.
- SSH con clave ED25519, sin contraseña.
- Usuario `deploy` sin acceso root.
- Archivo `.env` solo en servidor.
- Redirección HTTP → HTTPS (301).
- Restart policy: `unless-stopped`.

## 11.3 Persistencia

| Volumen | Contenido |
|---------|-----------|
| `sqldata` | Datos SQL Server |
| `certbot-etc` | Certificados SSL |
| `certbot-var` | Datos Certbot |

*Tabla 18. Volúmenes Docker para persistencia.*

## 11.4 Evidencia

<div class="fig-row">
<div class="fig">

![Producción](docs/screenshots/21-vps-evidence.png)

<div class="fig-caption">Figura 14. Sistema en producción con HTTPS en reservas-xyz.app-dev.icu.</div>
</div>
<div class="fig">

![Responsive](docs/screenshots/22-responsive-view.png)

<div class="fig-caption">Figura 15. Vista responsive móvil (390×844).</div>
</div>
</div>

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--          12. INSTRUCCIONES DE EJECUCIÓN                   -->
<!-- ══════════════════════════════════════════════════════════ -->

# 12. Instrucciones de Ejecución

## 12.1 Opción A — Local

**Requisitos:** .NET SDK 8.0, SQL Server, Node.js 20+.

```powershell
git clone https://github.com/juan-campo-dev/reservas-xyz.git
cd reservas-xyz
dotnet restore ReservasXYZ.sln
cd ReservasXYZ.Web && npm install && npm run css:build && cd ..
dotnet build ReservasXYZ.sln
dotnet run --project ReservasXYZ.Web --launch-profile http
```

Disponible en **http://localhost:5263**.

> La base de datos se crea automáticamente al iniciar: migraciones, roles, usuario admin, 6 SPs y catálogo FODUN (8 sedes, 10 alojamientos, 54 habitaciones).

## 12.2 Opción B — Docker Compose

```bash
cp .env.example .env        # Configurar credenciales
docker compose up -d --build
docker compose ps            # Verificar servicios
```

Disponible en **http://localhost** (puerto 80).

## 12.3 Opción C — Script SQL manual

```sql
CREATE DATABASE ReservasXYZDb;
GO
USE ReservasXYZDb;
GO
-- Ejecutar database/Database.sql + database/SeedCatalog.sql
```

## 12.4 Usuarios de prueba

| Rol | Email | Contraseña |
|-----|-------|------------|
| Admin | admin@hotel.com | Admin123! |
| Admin (test) | admin@test.com | Admin123* |
| Cliente (test) | cliente@test.com | Cliente123* |

*Tabla 19. Credenciales de usuarios de prueba.*

<div class="pb"></div>

<!-- ══════════════════════════════════════════════════════════ -->
<!--                13. CONCLUSIONES                           -->
<!-- ══════════════════════════════════════════════════════════ -->

# 13. Conclusiones

## 13.1 Cumplimiento de requisitos

| # | Requisito | Estado |
|---|-----------|--------|
| 1 | Estructura BD relacional SQL Server | ✅ |
| 2 | SP disponibilidad por fechas | ✅ sp_GetAvailableRoomsByDates |
| 3 | SP disponibilidad por fechas + personas | ✅ sp_GetAvailableRoomsByDatesAndGuests |
| 4 | SP tarifas por sitio/temporada/personas | ✅ sp_GetRoomRates + sp_GetRatesByCriteria |
| 5 | SP cálculo tarifa total | ✅ sp_CalculateTotalRate |
| 6 | Diseño formularios web | ✅ Razor Views |
| 7 | Registro de usuarios | ✅ Nombre, apellido, documento, email |
| 8 | Login (solo autenticados) | ✅ [Authorize] + Identity |
| 9 | Recuperación contraseña SMTP | ✅ MailKit |
| 10 | CRUD formularios | ✅ 6 módulos completos |
| 11 | Consulta disponibilidad | ✅ Por fechas y personas |
| 12 | Guardar reservas | ✅ Transacción SERIALIZABLE |
| 13 | MVC .NET Core | ✅ ASP.NET Core 8.0 |
| 14 | Programación en capas | ✅ Clean Architecture (4 capas) |
| 15 | Entity Framework | ✅ EF Core 8.0 + Fluent API |
| 16 | Identity | ✅ Roles + bloqueo + email |
| 17 | Documento técnico | ✅ Este documento |

*Tabla 20. Matriz de cumplimiento de requisitos.*

## 13.2 Valor agregado

- Motor de tarificación FODUN (Lun-Jue especial, temporadas, extras por persona).
- Prevención de overbooking con transacciones SERIALIZABLE.
- 6 Stored Procedures (4 requeridos + 2 auxiliares).
- Despliegue Docker + VPS + SSL (Let's Encrypt).
- Plantillas de email HTML con branding.
- Interfaz en español colombiano (es-CO).
- Soft delete, fallback LINQ, FluentValidation en español.
- Dashboard con métricas de ocupación e ingresos.
- Sistema de favoritos y diseño responsive.

## 13.3 Decisiones de diseño

| Decisión | Justificación |
|----------|---------------|
| Clean Architecture | Separación de responsabilidades, testabilidad |
| SPs + EF Core | SPs para lógica compleja, EF Core para CRUD, fallback LINQ |
| Transacción SERIALIZABLE | Prevención de overbooking concurrente |
| Docker multi-stage | Imagen optimizada (~220 MB), build reproducible |
| Tailwind CSS v4 | Build local, sin CDN, UI responsive |

*Tabla 21. Decisiones arquitectónicas.*

## 13.4 Mejoras futuras

- Tests unitarios con xUnit.
- Paginación server-side.
- Exportación reportes PDF/Excel.
- Galería de fotos por habitación.
- Notificaciones push (SignalR).
- API REST con Swagger/OpenAPI.

---

*Documento técnico — Prueba técnica Analista Desarrollador .NET — Fondo de Empleados XYZ (FODUN)*

*Mayo 2026 · Juan Campo · v1.0*
