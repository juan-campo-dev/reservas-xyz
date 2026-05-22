---
pdf_options:
  format: A4
  margin:
    top: 25.4mm
    bottom: 25.4mm
    left: 25.4mm
    right: 25.4mm
  printBackground: true
  displayHeaderFooter: true
  headerTemplate: '<div style="width:100%;font-size:8pt;font-family:Segoe UI,Arial,sans-serif;color:#999;padding:0 25.4mm;display:flex;justify-content:space-between;"><span></span><span style="font-style:italic;">Sistema de Reservas XYZ — Documento Técnico</span></div>'
  footerTemplate: '<div style="width:100%;font-size:8pt;font-family:Segoe UI,Arial,sans-serif;color:#666;padding:0 25.4mm;display:flex;justify-content:space-between;"><span>Juan Campo — Analista Desarrollador .NET</span><span>Página <span class="pageNumber"></span> de <span class="totalPages"></span></span></div>'
stylesheet: https://cdn.jsdelivr.net/npm/@fontsource/inter@5.0.0/index.css
body_class: document
---

<style>
  /* ═══════════════════════════════════════════════════ */
  /*  DOCUMENTO TÉCNICO PROFESIONAL — ESTILOS APA 7     */
  /*  Fuente: Segoe UI · Tamaño: A4 · Márgenes: 2.54cm */
  /* ═══════════════════════════════════════════════════ */

  @page { size: A4; margin: 25.4mm; }
  @page:first { margin-top: 0; margin-bottom: 0; }

  :root {
    --color-primary: #1a1a2e;
    --color-accent: #0f4c81;
    --color-accent-light: #e8f0fe;
    --color-border: #d0d5dd;
    --color-bg-code: #f8f9fb;
    --color-bg-table-header: #0f4c81;
    --color-text: #1d1d1f;
    --color-text-secondary: #6b7280;
    --color-caption: #4b5563;
    --font-main: 'Segoe UI', system-ui, -apple-system, Arial, sans-serif;
    --font-mono: 'Cascadia Code', 'Fira Code', 'Consolas', 'SF Mono', monospace;
  }

  * { box-sizing: border-box; }

  body {
    font-family: var(--font-main);
    font-size: 11pt;
    line-height: 1.5;
    color: var(--color-text);
    -webkit-font-smoothing: antialiased;
  }

  /* ── PORTADA ────────────────────────────────────── */
  .cover-page {
    page-break-after: always;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    text-align: center;
    padding: 40px 20px;
    background: linear-gradient(180deg, #ffffff 0%, #f0f4f8 100%);
    position: relative;
  }
  .cover-page::before {
    content: '';
    position: absolute;
    top: 0; left: 0; right: 0;
    height: 8px;
    background: linear-gradient(90deg, #0f4c81, #1a73e8, #0f4c81);
  }
  .cover-badge {
    display: inline-block;
    background: var(--color-accent);
    color: white;
    font-size: 9pt;
    font-weight: 600;
    letter-spacing: 2.5px;
    text-transform: uppercase;
    padding: 8px 28px;
    border-radius: 4px;
    margin-bottom: 40px;
  }
  .cover-title {
    font-size: 32pt;
    font-weight: 700;
    color: var(--color-primary);
    margin: 0 0 8px 0;
    letter-spacing: -0.5px;
    line-height: 1.15;
  }
  .cover-subtitle {
    font-size: 16pt;
    font-weight: 400;
    color: var(--color-accent);
    margin: 0 0 50px 0;
  }
  .cover-divider {
    width: 80px;
    height: 3px;
    background: var(--color-accent);
    margin: 0 auto 40px auto;
    border: none;
  }
  .cover-meta {
    font-size: 10.5pt;
    color: var(--color-text-secondary);
    line-height: 2;
  }
  .cover-meta strong {
    color: var(--color-text);
    font-weight: 600;
  }
  .cover-tech {
    margin-top: 40px;
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 8px;
  }
  .cover-tech span {
    background: white;
    border: 1px solid var(--color-border);
    color: var(--color-accent);
    font-size: 8.5pt;
    font-weight: 600;
    padding: 5px 14px;
    border-radius: 20px;
  }
  .cover-footer {
    position: absolute;
    bottom: 60px;
    font-size: 9pt;
    color: #aaa;
    letter-spacing: 1px;
  }

  /* ── HEADINGS ───────────────────────────────────── */
  h1 {
    font-size: 18pt;
    font-weight: 700;
    color: var(--color-primary);
    border-bottom: 3px solid var(--color-accent);
    padding-bottom: 8px;
    margin-top: 36px;
    margin-bottom: 18px;
    letter-spacing: -0.3px;
    page-break-after: avoid;
  }
  h2 {
    font-size: 14pt;
    font-weight: 600;
    color: var(--color-accent);
    margin-top: 28px;
    margin-bottom: 12px;
    page-break-after: avoid;
  }
  h3 {
    font-size: 12pt;
    font-weight: 600;
    color: #374151;
    margin-top: 20px;
    margin-bottom: 8px;
    page-break-after: avoid;
  }

  /* ── PARAGRAPHS ─────────────────────────────────── */
  p { margin: 0 0 10px 0; text-align: justify; }
  ul, ol { margin: 0 0 12px 0; padding-left: 24px; }
  li { margin-bottom: 4px; }

  /* ── TABLES ─────────────────────────────────────── */
  table {
    width: 100%;
    border-collapse: collapse;
    margin: 16px 0 20px 0;
    font-size: 9.5pt;
    page-break-inside: avoid;
  }
  thead th {
    background: var(--color-bg-table-header);
    color: white;
    font-weight: 600;
    text-align: left;
    padding: 10px 12px;
    border: 1px solid #0a3d6b;
    font-size: 9pt;
    letter-spacing: 0.3px;
  }
  tbody td {
    padding: 8px 12px;
    border: 1px solid #e5e7eb;
    vertical-align: top;
  }
  tbody tr:nth-child(even) { background: #f9fafb; }
  tbody tr:hover { background: var(--color-accent-light); }

  /* ── CAPTION (para tablas y figuras) ────────────── */
  .caption, em {
    display: block;
    text-align: center;
    font-size: 9pt;
    color: var(--color-caption);
    margin-top: 6px;
    margin-bottom: 20px;
    font-style: italic;
  }

  /* ── IMAGES ─────────────────────────────────────── */
  img {
    display: block;
    margin: 16px auto 4px auto;
    max-width: 88%;
    height: auto;
    border: 1px solid #e0e0e0;
    border-radius: 6px;
    box-shadow: 0 2px 12px rgba(0,0,0,0.08);
  }
  p > img { margin-bottom: 4px; }

  /* ── CODE BLOCKS ────────────────────────────────── */
  pre {
    background: var(--color-bg-code);
    border: 1px solid #e5e7eb;
    border-left: 4px solid var(--color-accent);
    border-radius: 6px;
    padding: 14px 18px;
    font-size: 8.5pt;
    line-height: 1.55;
    overflow-x: auto;
    page-break-inside: avoid;
    margin: 12px 0 16px 0;
  }
  code {
    font-family: var(--font-mono);
    font-size: 9pt;
  }
  p code, li code, td code {
    background: #f1f3f5;
    padding: 1px 5px;
    border-radius: 3px;
    font-size: 8.5pt;
    color: #c7254e;
  }

  /* ── HORIZONTAL RULES ───────────────────────────── */
  hr {
    border: none;
    height: 1px;
    background: var(--color-border);
    margin: 30px 0;
  }

  /* ── BLOCKQUOTE (Info boxes) ─────────────────────── */
  blockquote {
    background: var(--color-accent-light);
    border-left: 4px solid var(--color-accent);
    padding: 12px 18px;
    margin: 14px 0;
    border-radius: 0 6px 6px 0;
    font-size: 10pt;
    color: #1e3a5f;
    page-break-inside: avoid;
  }
  blockquote p { margin: 0; text-align: left; }

  /* ── STRONG / BOLD ──────────────────────────────── */
  strong { font-weight: 600; }

  /* ── PAGE BREAKS ────────────────────────────────── */
  .page-break { page-break-after: always; }

  /* ── TOC ─────────────────────────────────────────── */
  .toc {
    background: #fafbfc;
    border: 1px solid var(--color-border);
    border-radius: 8px;
    padding: 24px 30px;
    margin: 20px 0;
  }
  .toc h2 {
    text-align: center;
    border-bottom: none;
    margin-top: 0;
  }
  .toc ol { counter-reset: toc-counter; list-style: none; padding-left: 0; }
  .toc ol li { padding: 6px 0; border-bottom: 1px dotted #ddd; font-size: 10.5pt; }
  .toc ol li:last-child { border-bottom: none; }
  .toc a { text-decoration: none; color: var(--color-accent); font-weight: 500; }
  .toc a:hover { text-decoration: underline; }
</style>

<!-- ════════════════════════════════════════════════════ -->
<!--                    PORTADA                          -->
<!-- ════════════════════════════════════════════════════ -->

<div class="cover-page">
  <div class="cover-badge">Prueba Técnica — Analista Desarrollador .NET</div>
  <h1 class="cover-title" style="border:none;padding:0;margin:0 0 8px 0;">Sistema de Reservas XYZ</h1>
  <p class="cover-subtitle">Sistema de Gestión y Reservas Hoteleras</p>
  <hr class="cover-divider">
  <div class="cover-meta">
    <strong>Autor:</strong> Juan Campo<br>
    <strong>Cargo:</strong> Analista Desarrollador .NET<br>
    <strong>Documento:</strong> Documento Técnico v1.0<br>
    <strong>Fecha:</strong> Mayo 2026<br>
    <strong>Ciudad:</strong> Colombia
  </div>
  <div class="cover-tech">
    <span>ASP.NET Core 8.0 MVC</span>
    <span>Entity Framework Core</span>
    <span>SQL Server 2022</span>
    <span>ASP.NET Core Identity</span>
    <span>Docker</span>
    <span>Nginx + SSL</span>
    <span>MailKit SMTP</span>
    <span>Tailwind CSS v4</span>
  </div>
  <div class="cover-footer">DOCUMENTO CONFIDENCIAL — SOLO PARA EVALUACIÓN TÉCNICA</div>
</div>

<!-- ════════════════════════════════════════════════════ -->
<!--              TABLA DE CONTENIDOS                    -->
<!-- ════════════════════════════════════════════════════ -->

<div class="toc">

## Tabla de Contenidos

1. [Introducción](#1-introducción)
2. [Arquitectura del Proyecto](#2-arquitectura-del-proyecto)
3. [Tecnologías Utilizadas](#3-tecnologías-utilizadas)
4. [Modelo Relacional](#4-modelo-relacional)
5. [Base de Datos](#5-base-de-datos)
6. [Stored Procedures](#6-stored-procedures)
7. [Identity y Seguridad](#7-identity-y-seguridad)
8. [Flujo Funcional](#8-flujo-funcional)
9. [Servicio de Email (SMTP)](#9-servicio-de-email-smtp)
10. [Docker y Contenedorización](#10-docker-y-contenedorización)
11. [Despliegue en VPS](#11-despliegue-en-vps)
12. [Instrucciones de Ejecución](#12-instrucciones-de-ejecución)
13. [Conclusiones](#13-conclusiones)

</div>

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--              1. INTRODUCCIÓN                        -->
<!-- ════════════════════════════════════════════════════ -->

# 1. Introducción

## 1.1 Objetivo

Desarrollar un sistema web completo de gestión de reservas hoteleras para el Fondo de Empleados XYZ (FODUN), que permita a los asociados consultar disponibilidad de sedes recreativas y apartamentos, visualizar tarifas según temporada y número de personas, y realizar reservas en línea.

## 1.2 Contexto del negocio

El Fondo XYZ cuenta con 8 sedes distribuidas en diferentes ciudades de Colombia:

| #   | Sede            | Ciudad                | Capacidad   |
| --- | --------------- | --------------------- | ----------- |
| 1   | Villeta         | Villeta               | 32 personas |
| 2   | El Placer       | Fusagasugá            | 34 personas |
| 3   | Gonzalo Morante | Chinchiná             | 30 personas |
| 4   | Tablones        | Palmira               | 24 personas |
| 5   | Manguruma       | Santa Fe de Antioquia | 46 personas |
| 6   | Federmán        | Bogotá                | 8 personas  |
| 7   | Suramericana    | Medellín              | 9 personas  |
| 8   | El Rodadero     | Santa Marta           | 20 personas |

_Tabla 1. Sedes recreativas del Fondo de Empleados XYZ y su capacidad máxima de alojamiento._

## 1.3 Necesidad

Los asociados requieren un sistema web para:

- Consultar la disponibilidad de alojamientos según fechas de viaje.
- Visualizar tarifas por sitio, temporada, número de personas y tipo de alojamiento.
- Calcular el costo total de su estadía.
- Realizar y gestionar reservas de forma autónoma.

## 1.4 Pantalla principal del sistema

![Pantalla principal del sistema](docs/screenshots/01-home.png)

_Figura 1. Pantalla principal (Landing Page) del sistema ReservasXYZ. Muestra las 8 sedes disponibles, información de tarifas y accesos rápidos a registro y login._

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--         2. ARQUITECTURA DEL PROYECTO                -->
<!-- ════════════════════════════════════════════════════ -->

# 2. Arquitectura del Proyecto

## 2.1 Patrón arquitectónico

El proyecto implementa **Clean Architecture** (Arquitectura Limpia) con separación en cuatro capas independientes, siguiendo los principios SOLID y el patrón de inversión de dependencias.

## 2.2 Diagrama de capas

```
 ┌──────────────────────────────────────────────────────────┐
 │                     ReservasXYZ.Web                      │
 │    Controllers MVC · Razor Views · Identity Pages        │
 │    wwwroot (CSS/JS) · DI Configuration · Middleware      │
 ├──────────────────────────────────────────────────────────┤
 │                  ReservasXYZ.Application                 │
 │       DTOs · Servicios · Validadores · AutoMapper        │
 ├──────────────────────────────────────────────────────────┤
 │                 ReservasXYZ.Infrastructure               │
 │    DbContext · Repositorios · Email · Seed · SPs · DI   │
 ├──────────────────────────────────────────────────────────┤
 │                    ReservasXYZ.Domain                    │
 │             Entidades · Enums · Interfaces               │
 └──────────────────────────────────────────────────────────┘
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

| Capa                | Proyecto                     | Responsabilidad                                                                         |
| ------------------- | ---------------------------- | --------------------------------------------------------------------------------------- |
| **Presentación**    | `ReservasXYZ.Web`            | Controllers MVC, Razor Views, Identity Pages, middleware, configuración de DI, wwwroot  |
| **Aplicación**      | `ReservasXYZ.Application`    | DTOs, servicios de aplicación, validadores FluentValidation, perfiles AutoMapper        |
| **Infraestructura** | `ReservasXYZ.Infrastructure` | ApplicationDbContext, repositorios, servicio de email, DataSeeder, Fluent API, Identity |
| **Dominio**         | `ReservasXYZ.Domain`         | Entidades de negocio, enums, interfaces de repositorio                                  |

_Tabla 2. Responsabilidades de cada capa en la arquitectura del sistema._

## 2.5 Estructura del proyecto

```
ReservasXYZ.sln
├── ReservasXYZ.Domain/
│   ├── Entities/          → Site, Accommodation, Room, Season, Rate,
│   │                        Reservation, ReservationDetail, ApplicationUser, Favorite
│   ├── Enums/             → RoomType (8 tipos), ReservationStatus (6 estados), RateKind
│   └── Interfaces/        → IRepository<T>, IRoomRepository, IRateRepository,
│                            IReservationRepository
│
├── ReservasXYZ.Application/
│   ├── DTOs/              → SiteDtos, RoomDtos, ReservationDtos, SeasonRateDtos
│   ├── Interfaces/        → ISiteService, IRoomService, IReservationService
│   ├── Services/          → SiteService, RoomService, ReservationService
│   ├── Validators/        → FluentValidation validators por DTO
│   └── Mappings/          → AutoMapper MappingProfile
│
├── ReservasXYZ.Infrastructure/
│   ├── Data/Context/      → ApplicationDbContext
│   ├── Data/Config/       → Fluent API configurations (8 entidades)
│   ├── Data/Repositories/ → Repository<T>, RoomRepository, RateRepository,
│   │                        ReservationRepository
│   ├── Data/Seed/         → DataSeeder (roles, admin, catálogo FODUN, SPs)
│   ├── Email/             → EmailService, EmailTemplateService, IdentityEmailSender
│   ├── Identity/          → SpanishIdentityErrorDescriber
│   ├── Services/          → DashboardService, GuestPortalService, FavoriteService
│   └── StoredProcedures/  → StoredProcedures.sql (6 SPs)
│
├── ReservasXYZ.Web/
│   ├── Controllers/       → Home, Sites, Accommodations, Rooms, Seasons, Rates,
│   │                        Reservations, Favorites
│   ├── Areas/Identity/    → Login, Register, ForgotPassword, ResetPassword
│   ├── Views/             → Razor Views por módulo + Shared layout
│   └── wwwroot/           → CSS (Tailwind), JS, imágenes
│
├── database/
│   ├── Database.sql       → Script completo de estructura
│   └── SeedCatalog.sql    → Datos de catálogo FODUN
│
├── Dockerfile             → Multi-stage build (SDK 8.0 → Runtime 8.0)
└── docker-compose.yml     → SQL Server + Web + Nginx + Certbot
```

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--         3. TECNOLOGÍAS UTILIZADAS                   -->
<!-- ════════════════════════════════════════════════════ -->

# 3. Tecnologías Utilizadas

## 3.1 Stack tecnológico

| Tecnología            | Versión      | Propósito                           |
| --------------------- | ------------ | ----------------------------------- |
| .NET SDK              | 8.0          | Runtime y compilación               |
| ASP.NET Core MVC      | 8.0          | Framework web (Controllers + Views) |
| Razor Views / Pages   | 8.0          | Motor de vistas server-side         |
| Entity Framework Core | 8.0.25       | ORM y migraciones                   |
| SQL Server            | 2022 Express | Motor de base de datos              |
| ASP.NET Core Identity | 8.0.25       | Autenticación y autorización        |
| AutoMapper            | 13.x         | Mapeo automático entidades ↔ DTOs   |
| FluentValidation      | 11.x         | Validación declarativa de DTOs      |
| MailKit               | 4.x          | Cliente SMTP para envío de emails   |
| Tailwind CSS          | 4.3.0        | Framework CSS utility-first         |
| Node.js               | 20+          | Compilación de Tailwind CSS         |
| Docker                | 29.x         | Contenedores                        |
| Docker Compose        | v2           | Orquestación de servicios           |
| Nginx                 | Alpine       | Reverse proxy + SSL termination     |
| Certbot               | Latest       | Certificados SSL Let's Encrypt      |

_Tabla 3. Stack tecnológico completo del sistema ReservasXYZ._

## 3.2 Patrones de diseño utilizados

| Patrón                   | Implementación                                           |
| ------------------------ | -------------------------------------------------------- |
| **Repository Pattern**   | `IRepository<T>` genérico + repositorios especializados  |
| **Unit of Work**         | A través de `ApplicationDbContext.SaveChangesAsync()`    |
| **DTO Pattern**          | Objetos separados para Create, Update y View             |
| **Dependency Injection** | Contenedor nativo de ASP.NET Core                        |
| **Strategy Pattern**     | Motor de tarificación con Kind (Standard/SpecialWeekday) |
| **Template Method**      | EmailTemplateService para diferentes tipos de email      |
| **Soft Delete**          | `IsActive = false` en lugar de eliminar registros        |

_Tabla 4. Patrones de diseño implementados en el sistema._

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--           4. MODELO RELACIONAL                      -->
<!-- ════════════════════════════════════════════════════ -->

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

## 4.2 Tabla de relaciones

| Relación                        | Tipo | FK                                 | Descripción                                              |
| ------------------------------- | ---- | ---------------------------------- | -------------------------------------------------------- |
| Site → Accommodation            | 1:N  | `Accommodations.SiteId`            | Una sede tiene múltiples alojamientos                    |
| Accommodation → Room            | 1:N  | `Rooms.AccommodationId`            | Un alojamiento tiene múltiples habitaciones              |
| Room → Rate                     | 1:N  | `Rates.RoomId`                     | Una habitación tiene tarifas por temporada               |
| Season → Rate                   | 1:N  | `Rates.SeasonId`                   | Una temporada define tarifas para múltiples habitaciones |
| User → Reservation              | 1:N  | `Reservations.UserId`              | Un usuario puede tener múltiples reservas                |
| Reservation → ReservationDetail | 1:N  | `ReservationDetails.ReservationId` | Una reserva incluye múltiples habitaciones               |
| Room → ReservationDetail        | 1:N  | `ReservationDetails.RoomId`        | Una habitación puede estar en múltiples reservas         |
| User → Favorite                 | 1:N  | `Favorites.UserId`                 | Un usuario marca habitaciones como favoritas             |
| Room → Favorite                 | 1:N  | `Favorites.RoomId`                 | Una habitación puede ser favorita de múltiples usuarios  |

_Tabla 5. Relaciones entre entidades del modelo de datos._

## 4.3 Índices

| Tabla              | Índice                          | Tipo   | Columnas                      | Propósito                             |
| ------------------ | ------------------------------- | ------ | ----------------------------- | ------------------------------------- |
| Rooms              | `IX_Rooms_Accommodation_Number` | UNIQUE | (AccommodationId, RoomNumber) | Evitar duplicados por alojamiento     |
| Rates              | `IX_Rates_Room_Season_Kind`     | UNIQUE | (RoomId, SeasonId, Kind)      | Una tarifa por combinación            |
| Favorites          | `IX_Favorites_User_Room`        | UNIQUE | (UserId, RoomId)              | Un favorito por usuario-habitación    |
| ReservationDetails | `IX_ReservationDetail_Room`     | INDEX  | (RoomId)                      | Optimizar consultas de disponibilidad |

_Tabla 6. Índices de base de datos para optimización de consultas._

## 4.4 Constraints de integridad

- **CASCADE DELETE**: Site → Accommodations → Rooms (eliminar sede elimina todo).
- **RESTRICT DELETE**: Room → ReservationDetails (no se puede eliminar habitación con reservas).
- **SOFT DELETE**: Todas las entidades usan `IsActive` flag en lugar de eliminación física.
- **CHECK**: `Reservations.CheckOut > CheckIn` (validado en capa de aplicación).

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--              5. BASE DE DATOS                       -->
<!-- ════════════════════════════════════════════════════ -->

# 5. Base de Datos

## 5.1 Diseño

La base de datos fue diseñada siguiendo principios de normalización (3NF) para eliminar redundancias y garantizar integridad referencial.

**Tablas de dominio (9):** Sites, Accommodations, Rooms, Seasons, Rates, Reservations, ReservationDetails, Favorites, ApplicationUser.

**Tablas de Identity (5):** AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetUserTokens.

## 5.2 Estrategia de reservas

El sistema implementa una estrategia de **reserva multi-habitación** donde:

1. Una `Reservation` es la entidad padre con datos generales (fechas, usuario, total, estado).
2. Cada `ReservationDetail` representa una habitación individual dentro de la reserva.
3. El precio se calcula y almacena al momento de crear la reserva (snapshot de precio).
4. El estado fluye: `Pending → Confirmed → CheckedIn → CheckedOut`.

## 5.3 Prevención de overbooking

Se implementa un doble control:

1. **A nivel SQL**: SP `sp_ValidateOverbooking` verifica solapamiento de fechas.
2. **A nivel aplicación**: Transacción con nivel de aislamiento `SERIALIZABLE` que bloquea los registros durante la verificación y creación, previniendo condiciones de carrera.

```csharp
using var transaction = await _context.Database
    .BeginTransactionAsync(IsolationLevel.Serializable);
// 1. Verificar que no hay solapamiento
// 2. Crear la reserva
// 3. Commit o Rollback
```

## 5.4 Catálogo FODUN

El script `SeedCatalog.sql` es **idempotente** (se puede ejecutar múltiples veces sin duplicar datos) y carga:

- 3 temporadas (Baja, Alta, Especial).
- 8 sedes con sus descripciones y capacidades reales.
- 10 alojamientos distribuidos en las sedes.
- 54 habitaciones con tipos, capacidades y precios según el PDF de referencia FODUN.
- Tarifas completas con reglas de BaseGuests, ExtraPersonPrice y SpecialWeekday.

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--           6. STORED PROCEDURES                      -->
<!-- ════════════════════════════════════════════════════ -->

# 6. Stored Procedures

Se implementaron **6 Stored Procedures**, cubriendo los 4 requeridos por la prueba técnica más 2 auxiliares.

## 6.1 SP1 — `sp_GetAvailableRoomsByDates`

**Propósito:** Encontrar habitaciones disponibles en un rango de fechas.

| Parámetro   | Tipo | Dirección | Descripción      |
| ----------- | ---- | --------- | ---------------- |
| `@CheckIn`  | DATE | Entrada   | Fecha de entrada |
| `@CheckOut` | DATE | Entrada   | Fecha de salida  |

_Tabla 7. Parámetros de sp_GetAvailableRoomsByDates._

**Lógica:** Selecciona todas las habitaciones activas (`IsActive = 1`) que NO tengan reservas activas (estado ≠ Cancelled) con solapamiento de fechas. El solapamiento se detecta con la condición `CheckIn < @CheckOut AND CheckOut > @CheckIn`.

```sql
EXEC sp_GetAvailableRoomsByDates @CheckIn = '2026-06-01', @CheckOut = '2026-06-05';
```

---

## 6.2 SP2 — `sp_GetAvailableRoomsByDatesAndGuests`

**Propósito:** Encontrar habitaciones disponibles por fechas + capacidad mínima de huéspedes.

| Parámetro   | Tipo | Dirección | Descripción         |
| ----------- | ---- | --------- | ------------------- |
| `@CheckIn`  | DATE | Entrada   | Fecha de entrada    |
| `@CheckOut` | DATE | Entrada   | Fecha de salida     |
| `@Guests`   | INT  | Entrada   | Número de huéspedes |

_Tabla 8. Parámetros de sp_GetAvailableRoomsByDatesAndGuests._

**Lógica:** Misma lógica que SP1 + filtro `MaxGuests >= @Guests`.

```sql
EXEC sp_GetAvailableRoomsByDatesAndGuests
    @CheckIn = '2026-07-10', @CheckOut = '2026-07-15', @Guests = 6;
```

---

## 6.3 SP3 — `sp_GetRoomRates`

**Propósito:** Consultar las tarifas configuradas para una habitación específica.

| Parámetro | Tipo | Dirección | Descripción         |
| --------- | ---- | --------- | ------------------- |
| `@RoomId` | INT  | Entrada   | ID de la habitación |

_Tabla 9. Parámetros de sp_GetRoomRates._

**Lógica:** JOIN entre Rates, Seasons y Rooms. Retorna todas las tarifas activas con sus temporadas, precio por noche, multiplicador y precio base.

---

## 6.4 SP4 — `sp_CalculateTotalRate`

**Propósito:** Calcular la tarifa total a pagar por una estadía, aplicando las reglas de negocio FODUN.

| Parámetro      | Tipo          | Dirección  | Descripción               |
| -------------- | ------------- | ---------- | ------------------------- |
| `@RoomId`      | INT           | Entrada    | ID de la habitación       |
| `@CheckIn`     | DATE          | Entrada    | Fecha de entrada          |
| `@CheckOut`    | DATE          | Entrada    | Fecha de salida           |
| `@TotalGuests` | INT           | Entrada    | Número total de huéspedes |
| `@TotalRate`   | DECIMAL(18,2) | **Salida** | Tarifa total calculada    |

_Tabla 10. Parámetros de sp_CalculateTotalRate._

**Lógica del motor de tarificación:**

1. Establece `SET DATEFIRST 1` (lunes = día 1) para consistencia regional.
2. Itera cada noche del rango `[CheckIn, CheckOut)`.
3. Para cada noche:
   - Determina si es lunes a jueves (`DATEPART(WEEKDAY) BETWEEN 1 AND 4`).
   - Busca la tarifa activa (Rate + Season) vigente para esa fecha.
   - Si es Lun-Jue **Y** existe tarifa `SpecialWeekday` (Kind = 1) **Y** `PriceMultiplier ≤ 1`: usa la tarifa especial sin multiplicador.
   - Si no: usa tarifa Standard × PriceMultiplier.
   - Suma extras: `MAX(0, TotalGuests − BaseGuests) × ExtraPersonPrice`.
4. Si no hay tarifa configurada para una noche, usa `Room.BasePrice` como fallback.

```sql
DECLARE @Total DECIMAL(18,2);
EXEC sp_CalculateTotalRate
    @RoomId = 1,
    @CheckIn = '2026-06-01',
    @CheckOut = '2026-06-04',
    @TotalGuests = 6,
    @TotalRate = @Total OUTPUT;
SELECT @Total AS TarifaTotal;
```

**Ejemplo de cálculo** (3 noches, 6 personas, temporada baja, sede Villeta):

| Noche     | Día       | Tarifa base        | Multiplicador | Extra (2 pers)        | Total noche  |
| --------- | --------- | ------------------ | ------------- | --------------------- | ------------ |
| 1         | Lunes     | $27.000 (especial) | 1.0           | 2 × $11.000 = $22.000 | $49.000      |
| 2         | Martes    | $27.000 (especial) | 1.0           | 2 × $11.000 = $22.000 | $49.000      |
| 3         | Miércoles | $27.000 (especial) | 1.0           | 2 × $11.000 = $22.000 | $49.000      |
| **Total** |           |                    |               |                       | **$147.000** |

_Tabla 11. Ejemplo de cálculo de tarifa con SP4 para una estadía de 3 noches._

---

## 6.5 SP5 — `sp_ValidateOverbooking` (auxiliar)

**Propósito:** Verificar si una habitación tiene reservas solapadas (prevención de overbooking).

| Parámetro               | Tipo     | Dirección  | Descripción                 |
| ----------------------- | -------- | ---------- | --------------------------- |
| `@RoomId`               | INT      | Entrada    | ID de la habitación         |
| `@CheckIn`              | DATE     | Entrada    | Fecha de entrada            |
| `@CheckOut`             | DATE     | Entrada    | Fecha de salida             |
| `@ExcludeReservationId` | INT NULL | Entrada    | ID a excluir (para edición) |
| `@IsOverbooked`         | BIT      | **Salida** | 1 si hay solapamiento       |

_Tabla 12. Parámetros de sp_ValidateOverbooking._

---

## 6.6 SP6 — `sp_GetRatesByCriteria` (auxiliar)

**Propósito:** Consulta avanzada de tarifas cruzando sitio, temporada, alojamiento y personas.

**Parámetros:** Todos opcionales (NULL = sin filtro): `@SiteId`, `@AccommodationId`, `@SeasonId`, `@Guests`, `@CheckIn`, `@CheckOut`.

**Retorno:** Incluye `EffectivePricePerNight` calculado con multiplicador de temporada y extras.

## 6.7 Seguridad en Stored Procedures

- Todos los parámetros son **tipados** (DATE, INT, DECIMAL), previniendo SQL Injection.
- `SET NOCOUNT ON` para evitar mensajes de conteo que podrían exponer información.
- Los SPs se invocan desde EF Core con `SqlParameter` tipados, nunca con concatenación de strings.

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--         7. IDENTITY Y SEGURIDAD                     -->
<!-- ════════════════════════════════════════════════════ -->

# 7. Identity y Seguridad

## 7.1 Configuración de Identity

| Parámetro                  | Valor                    |
| -------------------------- | ------------------------ |
| Longitud mínima contraseña | 8 caracteres             |
| Requiere mayúscula         | Sí                       |
| Requiere minúscula         | Sí                       |
| Requiere dígito            | Sí                       |
| Requiere carácter especial | Sí                       |
| Intentos antes de bloqueo  | 5                        |
| Tiempo de bloqueo          | 15 minutos               |
| Confirmación de email      | Configurable por entorno |

_Tabla 13. Configuración de políticas de seguridad en ASP.NET Core Identity._

## 7.2 Roles y autorización

| Rol                 | Acceso                                                                                                                |
| ------------------- | --------------------------------------------------------------------------------------------------------------------- |
| **Admin**           | CRUD completo de sedes, alojamientos, habitaciones, temporadas, tarifas. Gestión de reservas. Dashboard con métricas. |
| **Receptionist**    | Gestión de reservas y check-in/check-out.                                                                             |
| **Guest / Cliente** | Consultar disponibilidad, crear reservas propias, ver historial, favoritos.                                           |

_Tabla 14. Roles del sistema y sus niveles de acceso._

## 7.3 Controles de seguridad OWASP

| Vulnerabilidad                     | Control implementado                                                                        |
| ---------------------------------- | ------------------------------------------------------------------------------------------- |
| **A01: Broken Access Control**     | `[Authorize(Roles = "Admin")]` en controllers admin. Verificación de propiedad de reservas. |
| **A02: Cryptographic Failures**    | Identity usa PBKDF2 con salt aleatorio para hash de contraseñas.                            |
| **A03: Injection**                 | EF Core genera queries parametrizadas. SPs usan `SqlParameter` tipados.                     |
| **A05: Security Misconfiguration** | Secrets en variables de entorno. `appsettings.Development.json` en `.gitignore`.            |
| **A07: Auth Failures**             | Bloqueo de cuenta tras 5 intentos. Mensajes genéricos.                                      |
| **A08: Data Integrity**            | `[ValidateAntiForgeryToken]` en todos los POST. Tokens anti-CSRF automáticos.               |

_Tabla 15. Controles de seguridad implementados según OWASP Top 10._

## 7.4 Formulario de autenticación

![Formulario de inicio de sesión](docs/screenshots/02-login.png)

_Figura 2. Formulario de inicio de sesión con validación client-side, enlace de recuperación de contraseña y opción de registro._

## 7.5 Formulario de registro

![Formulario de registro de usuario](docs/screenshots/03-registro.png)

_Figura 3. Formulario de registro con campos: nombre, apellido, documento, email y contraseña con requisitos de complejidad._

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--           8. FLUJO FUNCIONAL                        -->
<!-- ════════════════════════════════════════════════════ -->

# 8. Flujo Funcional

## 8.1 Diagrama de flujo general

```
Usuario anónimo
  │
  ├─► Registro (/Identity/Account/Register)
  │     └─► Confirmación email (si está habilitado)
  │
  ├─► Login (/Identity/Account/Login)
  │     ├─► ¿Olvidó contraseña? → ForgotPassword → Email → ResetPassword
  │     └─► Autenticado:
  │           │
  │           ├─► [ROL: Cliente]
  │           │     ├─► Portal de asociados (Home)
  │           │     │     └─► Búsqueda de disponibilidad
  │           │     │           └─► Seleccionar habitaciones
  │           │     │                 └─► Calcular tarifa (SP4)
  │           │     │                       └─► Confirmar reserva
  │           │     ├─► Mis reservas (/Reservations)
  │           │     └─► Favoritos
  │           │
  │           └─► [ROL: Admin]
  │                 ├─► Dashboard (métricas en tiempo real)
  │                 ├─► CRUD Sedes / Alojamientos / Habitaciones
  │                 ├─► CRUD Temporadas / Tarifas
  │                 └─► Gestión Reservas (confirmar, check-in/out, cancelar)
  │
  └─► Cerrar sesión
```

## 8.2 Flujo de estados de reserva

```
  ┌─────────┐     ┌───────────┐     ┌───────────┐     ┌────────────┐
  │ Pending │────►│ Confirmed │────►│ CheckedIn │────►│ CheckedOut │
  └────┬────┘     └─────┬─────┘     └───────────┘     └────────────┘
       │                │
       ▼                ▼
  ┌───────────┐   ┌─────────┐
  │ Cancelled │   │  NoShow │
  └───────────┘   └─────────┘
```

## 8.3 Panel de administración

![Panel de administración — Dashboard](docs/screenshots/04-dashboard-admin.png)

_Figura 4. Dashboard del panel administrativo. Muestra métricas en tiempo real: reservas totales, pendientes, confirmadas, ingresos totales y tasa de ocupación._

## 8.4 CRUD de Sedes

![Gestión de sedes recreativas](docs/screenshots/05-sites-crud.png)

_Figura 5. Módulo de gestión de sedes recreativas con tabla de datos, búsqueda y formulario modal de creación/edición._

## 8.5 CRUD de Alojamientos

![Gestión de alojamientos](docs/screenshots/06-accommodations-crud.png)

_Figura 6. Módulo de gestión de alojamientos. Cada registro está asociado a una sede e incluye tipo y número de habitaciones._

## 8.6 CRUD de Habitaciones

![Gestión de habitaciones](docs/screenshots/07-rooms-crud.png)

_Figura 7. Módulo de gestión de habitaciones con tipos (Single, Double, Twin, Suite, Deluxe, Family, Cabin, Apartment), capacidad y precio base._

## 8.7 CRUD de Temporadas

![Gestión de temporadas](docs/screenshots/08-seasons-crud.png)

_Figura 8. Módulo de gestión de temporadas con rangos de fechas y multiplicador de precio que afecta el cálculo del SP4._

## 8.8 CRUD de Tarifas

![Gestión de tarifas](docs/screenshots/09-rates-crud.png)

_Figura 9. Módulo de gestión de tarifas por habitación y temporada. Incluye tipo (Standard/SpecialWeekday), BaseGuests y precio por persona adicional._

## 8.9 Gestión de Reservas

![Panel de gestión de reservas](docs/screenshots/10-reservations-crud.png)

_Figura 10. Panel de gestión de reservas. El administrador puede confirmar, hacer check-in/check-out o cancelar reservas con filtros por estado._

## 8.10 Consulta de disponibilidad

![Portal de consulta de disponibilidad](docs/screenshots/11-availability-search.png)

_Figura 11. Portal de asociados con formulario de búsqueda de disponibilidad. Los resultados muestran habitaciones con precio calculado por los Stored Procedures._

## 8.11 Mis reservas — Portal del asociado

![Historial de reservas del asociado](docs/screenshots/15-my-reservations.png)

_Figura 12. Listado de reservas del asociado con estado, fechas, precio total y acciones disponibles según el estado actual de cada reserva._

## 8.12 Recuperación de contraseña

![Formulario de recuperación de contraseña](docs/screenshots/16-forgot-password.png)

_Figura 13. Formulario de recuperación de contraseña vía email SMTP con enlace de restablecimiento seguro._

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--         9. SERVICIO DE EMAIL (SMTP)                 -->
<!-- ════════════════════════════════════════════════════ -->

# 9. Servicio de Email (SMTP)

## 9.1 Arquitectura del servicio

```
IEmailSender<ApplicationUser> (Identity)
  └── IdentityEmailSender
        ├── Detecta tipo de email (confirmación, reset, notificación)
        ├── Construye plantilla HTML (EmailTemplateService)
        └── Envía via SMTP (EmailService → MailKit)
```

## 9.2 Flujo de recuperación de contraseña

1. Usuario ingresa su email en `/Identity/Account/ForgotPassword`.
2. Identity genera un token de restablecimiento.
3. `IdentityEmailSender` construye un email con plantilla HTML profesional.
4. `EmailService` conecta al servidor SMTP y envía el email.
5. El usuario recibe un enlace con el token.
6. Al hacer clic, accede a `/Identity/Account/ResetPassword` donde crea nueva contraseña.

## 9.3 Protocolos SMTP soportados

| Puerto | Protocolo | Comportamiento                              |
| ------ | --------- | ------------------------------------------- |
| 465    | SSL       | Conexión directa con SSL (`SslOnConnect`)   |
| 587    | STARTTLS  | Conexión plain + upgrade a TLS (`StartTls`) |
| Otro   | Auto      | Detección automática (`Auto`)               |

_Tabla 16. Protocolos SMTP soportados por el servicio de email._

## 9.4 Seguridad SMTP

- Credenciales SMTP se configuran exclusivamente vía **variables de entorno** en Docker o `appsettings.Development.json` (excluido de Git).
- Los enlaces en emails se codifican con `HtmlEncoder.Default.Encode()` para prevenir XSS.
- No hay credenciales SMTP en el código fuente ni en el historial de commits.

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--       10. DOCKER Y CONTENEDORIZACIÓN                -->
<!-- ════════════════════════════════════════════════════ -->

# 10. Docker y Contenedorización

## 10.1 Dockerfile — Multi-stage build

```dockerfile
# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
  # → Restore de paquetes NuGet (capas cacheadas)
  # → Instalación de Node.js 20 via NodeSource
  # → npm ci + npm run css:build (Tailwind CSS v4)
  # → dotnet publish -c Release → /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
  # → Copia artefactos publicados (~220 MB)
  # → ASPNETCORE_ENVIRONMENT=Production
  # → Puerto 5000
  # → ENTRYPOINT ["dotnet", "ReservasXYZ.Web.dll"]
```

## 10.2 Docker Compose — Servicios

| Servicio    | Imagen                   | Puerto  | Volúmenes                | Health check          |
| ----------- | ------------------------ | ------- | ------------------------ | --------------------- |
| `sqlserver` | mssql/server:2022-latest | 1433    | `sqldata`                | `sqlcmd` cada 15s     |
| `web`       | Build local              | 5000    | —                        | depends_on: sqlserver |
| `nginx`     | nginx:alpine             | 80, 443 | nginx.conf, certbot      | —                     |
| `certbot`   | certbot/certbot          | —       | certbot-etc, certbot-var | Renewal loop 6h       |

_Tabla 17. Servicios definidos en docker-compose.yml._

## 10.3 Variables de entorno

Todas las credenciales se inyectan desde `.env`:

```env
SA_PASSWORD=***               # Contraseña SA de SQL Server
SMTP_SERVER=***               # Servidor SMTP
SMTP_PORT=587                 # Puerto SMTP
SMTP_SENDER_NAME=***          # Nombre del remitente
SMTP_EMAIL=***                # Email del remitente
SMTP_PASSWORD=***             # Contraseña SMTP
DOMAIN=***                    # Dominio para SSL
```

## 10.4 Evidencia de contenedores en producción

```
NAMES              STATUS                 PORTS
reservas-certbot   Up 2 hours             80/tcp, 443/tcp
reservas-nginx     Up 2 hours             0.0.0.0:80→80, 0.0.0.0:443→443
reservas-web       Up 2 hours             0.0.0.0:5000→5000
reservas-db        Up 2 hours (healthy)   0.0.0.0:1433→1433
```

```
REPOSITORY                       TAG           SIZE
reservas-xyz-web                 latest        412MB
nginx                            alpine        93.6MB
certbot/certbot                  latest        308MB
mcr.microsoft.com/mssql/server   2022-latest   2.31GB
```

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--          11. DESPLIEGUE EN VPS                      -->
<!-- ════════════════════════════════════════════════════ -->

# 11. Despliegue en VPS

## 11.1 Infraestructura

| Componente        | Detalle                                                   |
| ----------------- | --------------------------------------------------------- |
| **Proveedor**     | Google Cloud Platform (GCE)                               |
| **Instancia**     | e2-medium (2 vCPU, 4 GB RAM, 40 GB SSD)                   |
| **SO**            | Ubuntu 22.04 LTS                                          |
| **Runtime**       | Docker 29.5.2 + Docker Compose v2                         |
| **Reverse Proxy** | Nginx Alpine (contenedor)                                 |
| **SSL**           | Let's Encrypt via Certbot (renovación automática cada 6h) |
| **DNS**           | Hostinger (registro A → IP estática GCP)                  |
| **URL**           | https://reservas-xyz.app-dev.icu                          |

_Tabla 18. Infraestructura de producción del sistema._

## 11.2 Seguridad del VPS

- **Firewall GCP**: Solo puertos 22 (SSH), 80 (HTTP), 443 (HTTPS) abiertos.
- **Acceso SSH**: Clave ED25519, sin contraseña.
- **Usuario**: `deploy` (sin acceso root directo).
- **Secrets**: Archivo `.env` en el servidor (no en repositorio).
- **SSL/TLS**: Certificados Let's Encrypt con renovación automática.
- **HTTP → HTTPS**: Redirección 301 automática en Nginx.
- **Restart policy**: `unless-stopped` en todos los contenedores.

## 11.3 Persistencia

| Volumen       | Contenido           | Persistencia                      |
| ------------- | ------------------- | --------------------------------- |
| `sqldata`     | Datos de SQL Server | Sobrevive reinicios de contenedor |
| `certbot-etc` | Certificados SSL    | Sobrevive reinicios               |
| `certbot-var` | Datos de Certbot    | Sobrevive reinicios               |

_Tabla 19. Volúmenes Docker para persistencia de datos._

## 11.4 Evidencia del despliegue

![Sistema desplegado en producción con HTTPS](docs/screenshots/21-vps-evidence.png)

_Figura 14. Sistema ReservasXYZ operativo en producción. URL: https://reservas-xyz.app-dev.icu. Certificado SSL Let's Encrypt válido._

## 11.5 Vista responsive

![Vista responsive en dispositivo móvil](docs/screenshots/22-responsive-view.png)

_Figura 15. Vista del sistema en resolución móvil (390×844). Diseño completamente responsive con Tailwind CSS v4._

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--        12. INSTRUCCIONES DE EJECUCIÓN               -->
<!-- ════════════════════════════════════════════════════ -->

# 12. Instrucciones de Ejecución

## 12.1 Opción A — Ejecución local

**Requisitos previos:** .NET SDK 8.0, SQL Server (LocalDB o instancia), Node.js 20+, npm 9+.

```powershell
# 1. Clonar repositorio
git clone https://github.com/juan-campo-dev/reservas-xyz.git
cd reservas-xyz

# 2. Restaurar paquetes NuGet
dotnet restore ReservasXYZ.sln

# 3. Instalar dependencias Node.js y compilar CSS
cd ReservasXYZ.Web
npm install
npm run css:build
cd ..

# 4. Compilar y ejecutar
dotnet build ReservasXYZ.sln
dotnet run --project ReservasXYZ.Web --launch-profile http
```

La aplicación estará disponible en **http://localhost:5263**.

> La base de datos se crea automáticamente al iniciar: migraciones EF Core, roles, usuario administrador, 6 Stored Procedures y catálogo FODUN completo (8 sedes, 10 alojamientos, 54 habitaciones con tarifas).

## 12.2 Opción B — Docker Compose

**Requisitos previos:** Docker Engine 20.10+, Docker Compose v2+.

```bash
# 1. Copiar y configurar variables de entorno
cp .env.example .env

# 2. Construir y levantar servicios
docker compose up -d --build

# 3. Verificar estado
docker compose ps

# 4. Ver logs en tiempo real
docker compose logs -f web
```

La aplicación estará disponible en **http://localhost** (puerto 80).

## 12.3 Opción C — Script SQL manual

```sql
-- 1. Crear la base de datos
CREATE DATABASE ReservasXYZDb;
GO
USE ReservasXYZDb;
GO

-- 2. Ejecutar database/Database.sql (estructura + SPs)
-- 3. Ejecutar database/SeedCatalog.sql (catálogo FODUN)
```

## 12.4 Usuarios de prueba

| Rol                | Email            | Contraseña   |
| ------------------ | ---------------- | ------------ |
| **Admin**          | admin@hotel.com  | Admin123!    |
| **Admin (test)**   | admin@test.com   | Admin123\*   |
| **Cliente (test)** | cliente@test.com | Cliente123\* |

_Tabla 20. Credenciales de usuarios de prueba del sistema._

<div class="page-break"></div>

<!-- ════════════════════════════════════════════════════ -->
<!--             13. CONCLUSIONES                        -->
<!-- ════════════════════════════════════════════════════ -->

# 13. Conclusiones

## 13.1 Cumplimiento de requisitos

| #   | Requisito                               | Estado                                                        |
| --- | --------------------------------------- | ------------------------------------------------------------- |
| 1   | Estructura BD relacional SQL Server     | ✅                                                            |
| 2   | SP disponibilidad por rango de fechas   | ✅ `sp_GetAvailableRoomsByDates`                              |
| 3   | SP disponibilidad por fechas + personas | ✅ `sp_GetAvailableRoomsByDatesAndGuests`                     |
| 4   | SP tarifas por sitio/temporada/personas | ✅ `sp_GetRoomRates` + `sp_GetRatesByCriteria`                |
| 5   | SP cálculo tarifa total                 | ✅ `sp_CalculateTotalRate`                                    |
| 6   | Diseño formularios web                  | ✅ Razor Views para todos los módulos                         |
| 7   | Registro de usuarios                    | ✅ Nombre, apellido, documento, email                         |
| 8   | Login (solo autenticados)               | ✅ `[Authorize]` + Identity                                   |
| 9   | Recuperación contraseña SMTP            | ✅ MailKit + plantilla HTML                                   |
| 10  | CRUD formularios                        | ✅ Sites, Accommodations, Rooms, Seasons, Rates, Reservations |
| 11  | Consulta disponibilidad                 | ✅ Por fechas y personas usando SPs                           |
| 12  | Guardar reservas                        | ✅ Transacción SERIALIZABLE                                   |
| 13  | MVC .NET Core                           | ✅ ASP.NET Core 8.0 MVC                                       |
| 14  | Programación en capas                   | ✅ 4 capas (Clean Architecture)                               |
| 15  | Entity Framework                        | ✅ EF Core 8.0 + Fluent API                                   |
| 16  | Identity                                | ✅ Roles + bloqueo + confirmación email                       |
| 17  | Documento técnico                       | ✅ Este documento                                             |

_Tabla 21. Matriz de cumplimiento de requisitos de la prueba técnica._

## 13.2 Valor agregado

Funcionalidades implementadas más allá de los requisitos básicos:

- Motor de tarificación FODUN con reglas complejas (Lun-Jue tarifa especial, temporadas, extras por persona).
- Prevención de overbooking con transacciones SERIALIZABLE.
- 6 Stored Procedures (4 requeridos + 2 auxiliares).
- Despliegue en producción con Docker + VPS + SSL (Let's Encrypt).
- Plantillas de email HTML profesionales con branding.
- Interfaz completamente en español colombiano (es-CO).
- Soft delete en todas las entidades para integridad de datos.
- Fallback LINQ en repositorios para alta disponibilidad.
- Validación con FluentValidation + mensajes en español.
- Dashboard administrativo con métricas de ocupación e ingresos.
- Sistema de favoritos para habitaciones.
- Diseño responsive para dispositivos móviles.

## 13.3 Decisiones de diseño

| Decisión                 | Justificación                                                                  |
| ------------------------ | ------------------------------------------------------------------------------ |
| Clean Architecture       | Separación de responsabilidades. Domain sin dependencias. Testabilidad.        |
| SPs + EF Core            | SPs para lógica compleja día por día. EF Core para CRUD simple. Fallback LINQ. |
| Transacción SERIALIZABLE | Nivel máximo de aislamiento para prevenir overbooking concurrente.             |
| Docker multi-stage       | Imagen final optimizada (~220 MB). Compilación reproducible.                   |
| Tailwind CSS v4          | Compilación local, sin CDN. UI responsive sin CSS custom extenso.              |

_Tabla 22. Decisiones arquitectónicas y su justificación._

## 13.4 Mejoras futuras

- Tests unitarios y de integración con xUnit.
- Paginación server-side para listados grandes.
- Exportación de reportes a PDF/Excel.
- Galería de fotos por habitación.
- Notificaciones push en tiempo real (SignalR).
- API REST con Swagger/OpenAPI para integraciones externas.
- Multi-idioma con archivos de recursos (.resx).

---

_Documento técnico elaborado como parte de la entrega de la prueba técnica para el cargo de Analista Desarrollador .NET — Fondo de Empleados XYZ (FODUN)._

_Mayo 2026 · Juan Campo · v1.0_
