<div style="text-align: center; padding: 120px 40px;">

# ReservasXYZ

## Sistema de Gestión y Reservas Hoteleras

### Documento Técnico

---

**Autor:** Juan Campo  
**Cargo:** Analista Desarrollador .NET  
**Fecha:** Mayo 2026  
**Versión:** 1.0

---

**Tecnologías principales:**  
ASP.NET Core 8.0 · Entity Framework Core 8.0 · SQL Server 2022  
ASP.NET Core Identity · MailKit · Docker · Nginx · Let's Encrypt · Tailwind CSS v4

</div>

<div style="page-break-after: always;"></div>

---

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
10. [Docker](#10-docker)
11. [Despliegue en VPS](#11-despliegue-en-vps)
12. [Instrucciones de Ejecución](#12-instrucciones-de-ejecución)
13. [Conclusiones](#13-conclusiones)

<div style="page-break-after: always;"></div>

---

## 1. Introducción

### 1.1 Objetivo

Desarrollar un sistema web completo de gestión de reservas hoteleras para el Fondo de Empleados XYZ (FODUN), que permita a los asociados consultar disponibilidad de sedes recreativas y apartamentos, visualizar tarifas según temporada y número de personas, y realizar reservas en línea.

### 1.2 Contexto del negocio

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

### 1.3 Necesidad

Los asociados requieren un sistema web para:

- Consultar la disponibilidad de alojamientos según fechas de viaje.
- Visualizar tarifas por sitio, temporada, número de personas y tipo de alojamiento.
- Calcular el costo total de su estadía.
- Realizar y gestionar reservas de forma autónoma.

### 1.4 Pantalla principal del sistema

![Pantalla principal - Landing Page](docs/screenshots/01-home.png)

_Figura 1.1: Landing page del sistema ReservasXYZ. Muestra las 8 sedes disponibles, información de tarifas y accesos rápidos a registro y login. Diseño responsive con Tailwind CSS v4._

<div style="page-break-after: always;"></div>

---

## 2. Arquitectura del Proyecto

### 2.1 Patrón arquitectónico

El proyecto implementa **Clean Architecture** (Arquitectura Limpia) con separación en cuatro capas independientes, siguiendo los principios SOLID y el patrón de inversión de dependencias.

### 2.2 Diagrama de capas

```
┌─────────────────────────────────────────────────────────┐
│                    ReservasXYZ.Web                       │
│       Controllers MVC · Razor Views · Identity Pages     │
│       wwwroot (CSS/JS) · DI Configuration · Middleware   │
├─────────────────────────────────────────────────────────┤
│                 ReservasXYZ.Application                   │
│        DTOs · Servicios · Validadores · AutoMapper        │
├─────────────────────────────────────────────────────────┤
│                ReservasXYZ.Infrastructure                 │
│     DbContext · Repositorios · Email · Seed · SPs · DI   │
├─────────────────────────────────────────────────────────┤
│                   ReservasXYZ.Domain                      │
│            Entidades · Enums · Interfaces                 │
└─────────────────────────────────────────────────────────┘
```

### 2.3 Flujo de dependencias

```
Web ──→ Application ──→ Domain ←── Infrastructure
```

- **Domain** no depende de ninguna otra capa (capa pura de negocio).
- **Application** depende exclusivamente de Domain.
- **Infrastructure** implementa las interfaces definidas en Domain.
- **Web** orquesta todo a través de inyección de dependencias.

### 2.4 Responsabilidades por capa

| Capa                | Proyecto                     | Responsabilidad                                                                                                                                                   |
| ------------------- | ---------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Presentación**    | `ReservasXYZ.Web`            | Controllers MVC, Razor Views, Identity Pages, middleware, configuración de DI, wwwroot (Tailwind CSS, JS)                                                         |
| **Aplicación**      | `ReservasXYZ.Application`    | DTOs (Create/Update/View), servicios de aplicación, validadores FluentValidation, perfiles AutoMapper, interfaces de servicios                                    |
| **Infraestructura** | `ReservasXYZ.Infrastructure` | ApplicationDbContext (EF Core), repositorios genérico y especializados, servicio de email (MailKit), DataSeeder, configuraciones Fluent API, Identity customizado |
| **Dominio**         | `ReservasXYZ.Domain`         | Entidades de negocio (Site, Room, Reservation...), enums (RoomType, ReservationStatus), interfaces de repositorio                                                 |

### 2.5 Estructura del proyecto

```
ReservasXYZ.sln
├── ReservasXYZ.Domain/
│   ├── Entities/         → Site, Accommodation, Room, Season, Rate,
│   │                       Reservation, ReservationDetail, ApplicationUser, Favorite
│   ├── Enums/            → RoomType (8 tipos), ReservationStatus (6 estados), RateKind
│   └── Interfaces/       → IRepository<T>, IRoomRepository, IRateRepository, IReservationRepository
│
├── ReservasXYZ.Application/
│   ├── DTOs/             → SiteDtos, RoomDtos, ReservationDtos, SeasonRateDtos, etc.
│   ├── Interfaces/       → ISiteService, IRoomService, IReservationService, etc.
│   ├── Services/         → SiteService, RoomService, ReservationService, etc.
│   ├── Validators/       → FluentValidation validators por DTO
│   └── Mappings/         → AutoMapper MappingProfile
│
├── ReservasXYZ.Infrastructure/
│   ├── Data/Context/     → ApplicationDbContext
│   ├── Data/Config/      → Fluent API configurations (8 entidades)
│   ├── Data/Repositories/→ Repository<T>, RoomRepository, RateRepository, ReservationRepository
│   ├── Data/Seed/        → DataSeeder (roles, admin, catálogo FODUN, SPs)
│   ├── Email/            → EmailService, EmailTemplateService, IdentityEmailSender
│   ├── Identity/         → SpanishIdentityErrorDescriber, SpanishFluentValidationLanguageManager
│   ├── Services/         → DashboardService, GuestPortalService, FavoriteService
│   └── StoredProcedures/ → StoredProcedures.sql (6 SPs)
│
├── ReservasXYZ.Web/
│   ├── Controllers/      → Home, Sites, Accommodations, Rooms, Seasons, Rates, Reservations, Favorites
│   ├── Areas/Identity/   → Login, Register, ForgotPassword, ResetPassword, ConfirmEmail
│   ├── Views/            → Razor Views por módulo + Shared layout
│   └── wwwroot/          → CSS (Tailwind), JS, imágenes
│
├── database/
│   ├── Database.sql      → Script completo de estructura
│   └── SeedCatalog.sql   → Datos de catálogo FODUN
│
├── Dockerfile            → Multi-stage build (SDK 8.0 → Runtime 8.0)
└── docker-compose.yml    → SQL Server + Web + Nginx + Certbot
```

<div style="page-break-after: always;"></div>

---

## 3. Tecnologías Utilizadas

### 3.1 Stack tecnológico

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

### 3.2 Patrones de diseño utilizados

| Patrón                   | Implementación                                           |
| ------------------------ | -------------------------------------------------------- |
| **Repository Pattern**   | `IRepository<T>` genérico + repositorios especializados  |
| **Unit of Work**         | A través de `ApplicationDbContext.SaveChangesAsync()`    |
| **DTO Pattern**          | Objetos separados para Create, Update y View             |
| **Dependency Injection** | Contenedor nativo de ASP.NET Core                        |
| **Strategy Pattern**     | Motor de tarificación con Kind (Standard/SpecialWeekday) |
| **Template Method**      | EmailTemplateService para diferentes tipos de email      |
| **Soft Delete**          | `IsActive = false` en lugar de eliminar registros        |

<div style="page-break-after: always;"></div>

---

## 4. Modelo Relacional

### 4.1 Entidades del dominio

```
Sites (Sedes)
  └── Accommodations (Alojamientos)
        └── Rooms (Habitaciones)
              ├── Rates (Tarifas) ← Seasons (Temporadas)
              ├── ReservationDetails (Detalle reserva) ← Reservations (Reservas) ← ApplicationUser
              └── Favorites (Favoritos) ← ApplicationUser
```

### 4.2 Tabla de relaciones

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

### 4.3 Índices

| Tabla              | Nombre                          | Tipo   | Columnas                      | Propósito                                                 |
| ------------------ | ------------------------------- | ------ | ----------------------------- | --------------------------------------------------------- |
| Rooms              | `IX_Rooms_Accommodation_Number` | UNIQUE | (AccommodationId, RoomNumber) | Evitar duplicados de número de habitación por alojamiento |
| Rates              | `IX_Rates_Room_Season_Kind`     | UNIQUE | (RoomId, SeasonId, Kind)      | Una sola tarifa por combinación habitación-temporada-tipo |
| Favorites          | `IX_Favorites_User_Room`        | UNIQUE | (UserId, RoomId)              | Un usuario solo puede marcar una vez como favorita        |
| ReservationDetails | `IX_ReservationDetail_Room`     | INDEX  | (RoomId)                      | Optimizar consultas de disponibilidad                     |

### 4.4 Constraints de integridad

- **CASCADE DELETE**: Site → Accommodations → Rooms (eliminar sede elimina todo).
- **RESTRICT DELETE**: Room → ReservationDetails (no se puede eliminar habitación con reservas).
- **SOFT DELETE**: Todas las entidades usan `IsActive` flag en lugar de eliminación física.
- **CHECK**: Reservations.CheckOut > CheckIn (validado en capa de aplicación).

<div style="page-break-after: always;"></div>

---

## 5. Base de Datos

### 5.1 Diseño

La base de datos fue diseñada siguiendo principios de normalización (3NF) para eliminar redundancias y garantizar integridad referencial.

**Tablas de dominio (9):** Sites, Accommodations, Rooms, Seasons, Rates, Reservations, ReservationDetails, Favorites, ApplicationUser.

**Tablas de Identity (5):** AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetUserTokens.

### 5.2 Estrategia de reservas

El sistema implementa una estrategia de **reserva multi-habitación** donde:

1. Una `Reservation` es la entidad padre con datos generales (fechas, usuario, total, estado).
2. Cada `ReservationDetail` representa una habitación individual dentro de la reserva.
3. El precio se calcula y almacena al momento de crear la reserva (snapshot de precio).
4. El estado fluye: Pending → Confirmed → CheckedIn → CheckedOut.

### 5.3 Prevención de overbooking

Se implementa un doble control:

1. **A nivel SQL**: SP `sp_ValidateOverbooking` verifica solapamiento de fechas.
2. **A nivel aplicación**: Transacción con nivel de aislamiento `SERIALIZABLE` que bloquea los registros durante la verificación y creación, previniendo condiciones de carrera.

```csharp
// ReservationRepository.cs - Fragmento
using var transaction = await _context.Database
    .BeginTransactionAsync(IsolationLevel.Serializable);
// 1. Verificar que no hay solapamiento
// 2. Crear la reserva
// 3. Commit
```

### 5.4 Catálogo FODUN

El script `SeedCatalog.sql` es **idempotente** (se puede ejecutar múltiples veces sin duplicar datos) y carga:

- 3 temporadas (Baja, Alta, Especial).
- 8 sedes con sus descripciones y capacidades reales.
- 10 alojamientos distribuidos en las sedes.
- 54 habitaciones con tipos, capacidades y precios según el PDF de referencia FODUN.
- Tarifas completas con reglas de BaseGuests, ExtraPersonPrice y SpecialWeekday.

<div style="page-break-after: always;"></div>

---

## 6. Stored Procedures

Se implementaron **6 Stored Procedures**, cubriendo los 4 requeridos por la prueba técnica más 2 auxiliares.

### 6.1 SP1: `sp_GetAvailableRoomsByDates`

**Propósito**: Encontrar habitaciones disponibles en un rango de fechas.

**Parámetros:**

| Nombre      | Tipo | Dirección | Descripción      |
| ----------- | ---- | --------- | ---------------- |
| `@CheckIn`  | DATE | Entrada   | Fecha de entrada |
| `@CheckOut` | DATE | Entrada   | Fecha de salida  |

**Lógica**: Selecciona todas las habitaciones activas (`IsActive = 1`) que NO tengan reservas activas (estado ≠ Cancelled) con solapamiento de fechas. El solapamiento se detecta con la condición `CheckIn < @CheckOut AND CheckOut > @CheckIn`. Incluye JOINs con Accommodations y Sites para contexto completo.

**Ejemplo de uso:**

```sql
EXEC sp_GetAvailableRoomsByDates @CheckIn = '2026-06-01', @CheckOut = '2026-06-05';
```

**Resultado esperado:** Lista de habitaciones con Id, RoomNumber, Type, MaxGuests, BasePrice, AccommodationName, SiteName.

---

### 6.2 SP2: `sp_GetAvailableRoomsByDatesAndGuests`

**Propósito**: Encontrar habitaciones disponibles por fechas + capacidad mínima de huéspedes.

**Parámetros:**

| Nombre      | Tipo | Dirección | Descripción         |
| ----------- | ---- | --------- | ------------------- |
| `@CheckIn`  | DATE | Entrada   | Fecha de entrada    |
| `@CheckOut` | DATE | Entrada   | Fecha de salida     |
| `@Guests`   | INT  | Entrada   | Número de huéspedes |

**Lógica**: Misma lógica que SP1 + filtro `MaxGuests >= @Guests`.

**Ejemplo de uso:**

```sql
EXEC sp_GetAvailableRoomsByDatesAndGuests
    @CheckIn = '2026-07-10', @CheckOut = '2026-07-15', @Guests = 6;
```

---

### 6.3 SP3: `sp_GetRoomRates`

**Propósito**: Consultar las tarifas configuradas para una habitación específica.

**Parámetros:**

| Nombre    | Tipo | Dirección | Descripción         |
| --------- | ---- | --------- | ------------------- |
| `@RoomId` | INT  | Entrada   | ID de la habitación |

**Lógica**: JOIN entre Rates, Seasons y Rooms. Retorna todas las tarifas activas con sus temporadas, precio por noche, multiplicador y precio base.

---

### 6.4 SP4: `sp_CalculateTotalRate`

**Propósito**: Calcular la tarifa total a pagar por una estadía, aplicando las reglas de negocio FODUN.

**Parámetros:**

| Nombre         | Tipo          | Dirección  | Descripción               |
| -------------- | ------------- | ---------- | ------------------------- |
| `@RoomId`      | INT           | Entrada    | ID de la habitación       |
| `@CheckIn`     | DATE          | Entrada    | Fecha de entrada          |
| `@CheckOut`    | DATE          | Entrada    | Fecha de salida           |
| `@TotalGuests` | INT           | Entrada    | Número total de huéspedes |
| `@TotalRate`   | DECIMAL(18,2) | **Salida** | Tarifa total calculada    |

**Lógica del motor de tarificación:**

1. Establece `SET DATEFIRST 1` (lunes = día 1) para consistencia regional.
2. Itera cada noche del rango `[CheckIn, CheckOut)`.
3. Para cada noche:
   - Determina si es lunes a jueves (`DATEPART(WEEKDAY) BETWEEN 1 AND 4`).
   - Busca la tarifa activa (Rate + Season) vigente para esa fecha.
   - Si es Lun-Jue Y existe tarifa `SpecialWeekday` (Kind = 1) Y `PriceMultiplier ≤ 1`:
     → Usa la tarifa especial sin multiplicador.
   - Si no:
     → Usa tarifa Standard × PriceMultiplier.
   - Suma extras: `MAX(0, TotalGuests − BaseGuests) × ExtraPersonPrice`.
4. Si no hay tarifa configurada para una noche, usa `Room.BasePrice` como fallback.

**Ejemplo de uso:**

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

**Ejemplo de cálculo (3 noches, 6 personas, temporada baja, sede Villeta):**

| Noche     | Día       | Tarifa base        | Multiplicador   | Extra (2 pers × $16.000) | Total noche  |
| --------- | --------- | ------------------ | --------------- | ------------------------ | ------------ |
| 1         | Lunes     | $27.000 (especial) | 1.0 (no aplica) | 2 × $11.000 = $22.000    | $49.000      |
| 2         | Martes    | $27.000 (especial) | 1.0 (no aplica) | 2 × $11.000 = $22.000    | $49.000      |
| 3         | Miércoles | $27.000 (especial) | 1.0 (no aplica) | 2 × $11.000 = $22.000    | $49.000      |
| **Total** |           |                    |                 |                          | **$147.000** |

---

### 6.5 SP5: `sp_ValidateOverbooking` (auxiliar)

**Propósito**: Verificar si una habitación tiene reservas solapadas (prevención de overbooking).

**Parámetros:**

| Nombre                  | Tipo     | Dirección  | Descripción                 |
| ----------------------- | -------- | ---------- | --------------------------- |
| `@RoomId`               | INT      | Entrada    | ID de la habitación         |
| `@CheckIn`              | DATE     | Entrada    | Fecha de entrada            |
| `@CheckOut`             | DATE     | Entrada    | Fecha de salida             |
| `@ExcludeReservationId` | INT NULL | Entrada    | ID a excluir (para edición) |
| `@IsOverbooked`         | BIT      | **Salida** | 1 si hay solapamiento       |

---

### 6.6 SP6: `sp_GetRatesByCriteria` (auxiliar)

**Propósito**: Consulta avanzada de tarifas cruzando sitio, temporada, alojamiento y personas.

**Parámetros:** Todos opcionales (NULL = sin filtro): `@SiteId`, `@AccommodationId`, `@SeasonId`, `@Guests`, `@CheckIn`, `@CheckOut`.

**Retorno**: Incluye `EffectivePricePerNight` calculado con multiplicador de temporada y extras.

### 6.7 Seguridad en Stored Procedures

- Todos los parámetros son **tipados** (DATE, INT, DECIMAL), previniendo SQL Injection.
- `SET NOCOUNT ON` para evitar mensajes de conteo que podrían exponer información.
- Los SPs se invocan desde EF Core con `SqlParameter` tipados, nunca con concatenación de strings.

<div style="page-break-after: always;"></div>

---

## 7. Identity y Seguridad

### 7.1 Configuración de Identity

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

### 7.2 Roles y autorización

| Rol                 | Acceso                                                                                                                                                           |
| ------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Admin**           | CRUD completo de sedes, alojamientos, habitaciones, temporadas, tarifas. Gestión de reservas (confirmar, check-in, check-out, cancelar). Dashboard con métricas. |
| **Receptionist**    | Gestión de reservas y check-in/check-out.                                                                                                                        |
| **Guest / Cliente** | Consultar disponibilidad, crear reservas propias, ver historial, favoritos.                                                                                      |

### 7.3 Controles de seguridad OWASP

| Vulnerabilidad                     | Control implementado                                                                                                    |
| ---------------------------------- | ----------------------------------------------------------------------------------------------------------------------- |
| **A01: Broken Access Control**     | `[Authorize(Roles = "Admin")]` en controllers admin. Verificación de propiedad de reservas (usuario solo ve las suyas). |
| **A02: Cryptographic Failures**    | Identity usa PBKDF2 con salt aleatorio para hash de contraseñas. Nunca se almacenan en texto plano.                     |
| **A03: Injection**                 | EF Core genera queries parametrizadas. SPs usan `SqlParameter` tipados.                                                 |
| **A05: Security Misconfiguration** | Secrets en variables de entorno (`.env`). `appsettings.Development.json` en `.gitignore`.                               |
| **A07: Auth Failures**             | Bloqueo de cuenta tras 5 intentos. Mensajes genéricos (no revelan si el email existe).                                  |
| **A08: Data Integrity**            | `[ValidateAntiForgeryToken]` en todos los POST. Razor genera tokens anti-CSRF automáticamente.                          |

### 7.4 Pantalla de login

![Pantalla de login](docs/screenshots/02-login.png)

_Figura 7.1: Formulario de inicio de sesión. Incluye validación client-side, enlace de recuperación de contraseña y opción de registro._

### 7.5 Pantalla de registro

![Pantalla de registro](docs/screenshots/03-registro.png)

_Figura 7.2: Formulario de registro con campos: nombre, apellido, documento, email y contraseña con requisitos de complejidad._

<div style="page-break-after: always;"></div>

---

## 8. Flujo Funcional

### 8.1 Diagrama de flujo general

```
Usuario anónimo
  │
  ├── Registro (/Identity/Account/Register)
  │     └── Confirmación email (si está habilitado)
  │
  ├── Login (/Identity/Account/Login)
  │     ├── ¿Olvidó contraseña? → ForgotPassword → Email SMTP → ResetPassword
  │     └── Autenticado:
  │           │
  │           ├── [ROL: Cliente]
  │           │     ├── Portal de asociados (Home)
  │           │     │     └── Búsqueda de disponibilidad
  │           │     │           └── Seleccionar habitaciones
  │           │     │                 └── Calcular tarifa (SP4)
  │           │     │                       └── Confirmar reserva
  │           │     ├── Mis reservas (/Reservations)
  │           │     └── Favoritos
  │           │
  │           └── [ROL: Admin]
  │                 ├── Dashboard (métricas)
  │                 ├── CRUD Sedes
  │                 ├── CRUD Alojamientos
  │                 ├── CRUD Habitaciones
  │                 ├── CRUD Temporadas
  │                 ├── CRUD Tarifas
  │                 └── Gestión Reservas (confirmar, check-in, check-out, cancelar)
  │
  └── Cerrar sesión
```

### 8.2 Flujo de estados de reserva

```
   Pending ──→ Confirmed ──→ CheckedIn ──→ CheckedOut
     │              │
     ▼              ▼
  Cancelled       NoShow
```

### 8.3 Panel de administración (Dashboard)

![Dashboard administrativo](docs/screenshots/04-dashboard-admin.png)

_Figura 8.1: Dashboard del panel administrativo. Muestra métricas en tiempo real: total de reservas, reservas pendientes, confirmadas, ingresos totales, tasa de ocupación y últimas reservas._

### 8.4 CRUD de Sedes

![CRUD de sedes](docs/screenshots/05-sites-crud.png)

_Figura 8.2: Gestión de sedes recreativas. Tabla con búsqueda, ordenamiento y acciones de editar/desactivar. Formulario modal para creación/edición._

### 8.5 CRUD de Alojamientos

![CRUD de alojamientos](docs/screenshots/06-accommodations-crud.png)

_Figura 8.3: Gestión de alojamientos. Cada alojamiento está asociado a una sede e incluye tipo y número de habitaciones._

### 8.6 CRUD de Habitaciones

![CRUD de habitaciones](docs/screenshots/07-rooms-crud.png)

_Figura 8.4: Gestión de habitaciones. Incluye tipo (Single, Double, Twin, Suite, Deluxe, Family, Cabin, Apartment), capacidad máxima y precio base._

### 8.7 CRUD de Temporadas

![CRUD de temporadas](docs/screenshots/08-seasons-crud.png)

_Figura 8.5: Gestión de temporadas con rangos de fechas y multiplicador de precio. El multiplicador afecta el cálculo en el SP4._

### 8.8 CRUD de Tarifas

![CRUD de tarifas](docs/screenshots/09-rates-crud.png)

_Figura 8.6: Gestión de tarifas por habitación y temporada. Incluye tipo de tarifa (Standard/SpecialWeekday), BaseGuests y precio por persona adicional._

### 8.9 Gestión de Reservas

![Gestión de reservas](docs/screenshots/10-reservations-crud.png)

_Figura 8.7: Panel de gestión de reservas. El administrador puede confirmar, hacer check-in/check-out o cancelar reservas. Filtros por estado y búsqueda._

### 8.10 Consulta de disponibilidad

![Consulta de disponibilidad](docs/screenshots/11-availability-search.png)

_Figura 8.8: Portal de asociados con formulario de búsqueda de disponibilidad. El usuario selecciona sede, fechas y número de personas. Los resultados muestran habitaciones disponibles con precio calculado por los Stored Procedures._

### 8.11 Mis reservas (portal de asociado)

![Mis reservas](docs/screenshots/15-my-reservations.png)

_Figura 8.9: Listado de reservas del asociado. Muestra estado, fechas, precio total y acciones disponibles según el estado actual._

### 8.12 Recuperación de contraseña

![Recuperación de contraseña](docs/screenshots/16-forgot-password.png)

_Figura 8.10: Formulario de recuperación de contraseña. El sistema envía un email con enlace de restablecimiento vía SMTP._

<div style="page-break-after: always;"></div>

---

## 9. Servicio de Email (SMTP)

### 9.1 Arquitectura

```
IEmailSender<ApplicationUser> (Identity)
  └── IdentityEmailSender
        ├── Detecta tipo de email (confirmación, reset, notificación)
        ├── Construye plantilla HTML (EmailTemplateService)
        └── Envía via SMTP (EmailService → MailKit)
```

### 9.2 Flujo de recuperación de contraseña

1. Usuario ingresa su email en `/Identity/Account/ForgotPassword`.
2. Identity genera un token de restablecimiento.
3. `IdentityEmailSender` construye un email con plantilla HTML profesional.
4. `EmailService` conecta al servidor SMTP y envía el email.
5. El usuario recibe un enlace con el token.
6. Al hacer clic, accede a `/Identity/Account/ResetPassword` donde crea nueva contraseña.

### 9.3 Protocolos SMTP soportados

| Puerto | Protocolo | Comportamiento                              |
| ------ | --------- | ------------------------------------------- |
| 465    | SSL       | Conexión directa con SSL (`SslOnConnect`)   |
| 587    | STARTTLS  | Conexión plain + upgrade a TLS (`StartTls`) |
| Otro   | Auto      | Detección automática (`Auto`)               |

### 9.4 Seguridad SMTP

- Credenciales SMTP se configuran exclusivamente via **variables de entorno** en Docker o `appsettings.Development.json` (excluido de Git).
- Los enlacesen emails se codifican con `HtmlEncoder.Default.Encode()` para prevenir XSS.
- No hay credenciales SMTP en el código fuente, en GitHub, ni en ningún commit del historial.

<div style="page-break-after: always;"></div>

---

## 10. Docker

### 10.1 Dockerfile (Multi-stage build)

```
Etapa 1: Build (mcr.microsoft.com/dotnet/sdk:8.0)
  ├── Restore de paquetes NuGet (capas cacheadas)
  ├── Instalación de Node.js 20 via NodeSource
  ├── npm ci + npm run css:build (Tailwind CSS v4)
  └── dotnet publish -c Release → /app/publish

Etapa 2: Runtime (mcr.microsoft.com/dotnet/aspnet:8.0)
  ├── Copia artefactos publicados (~220 MB)
  ├── ASPNETCORE_ENVIRONMENT=Production
  ├── Puerto 5000
  └── ENTRYPOINT ["dotnet", "ReservasXYZ.Web.dll"]
```

### 10.2 Docker Compose

| Servicio    | Imagen                   | Puerto  | Volúmenes                            | Health check                        |
| ----------- | ------------------------ | ------- | ------------------------------------ | ----------------------------------- |
| `sqlserver` | mssql/server:2022-latest | 1433    | `sqldata:/var/opt/mssql`             | `sqlcmd` cada 15s, 60s start_period |
| `web`       | Build local              | 5000    | —                                    | depends_on: sqlserver (healthy)     |
| `nginx`     | nginx:alpine             | 80, 443 | nginx.conf, certbot-etc, certbot-var | —                                   |
| `certbot`   | certbot/certbot          | —       | certbot-etc, certbot-var             | Renewal loop cada 6h                |

### 10.3 Variables de entorno

Todas las credenciales se inyectan desde `.env`:

```env
SA_PASSWORD=***            # Contraseña SA de SQL Server
SMTP_SERVER=***            # Servidor SMTP
SMTP_PORT=587              # Puerto SMTP
SMTP_SENDER_NAME=***      # Nombre del remitente
SMTP_EMAIL=***             # Email del remitente
SMTP_PASSWORD=***          # Contraseña SMTP
DOMAIN=***                 # Dominio para SSL
```

### 10.4 Evidencia Docker en producción

**Contenedores activos en el VPS:**

```
NAMES              STATUS                 PORTS
reservas-certbot   Up 2 hours             80/tcp, 443/tcp
reservas-nginx     Up 2 hours             0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp
reservas-web       Up 2 hours             0.0.0.0:5000->5000/tcp
reservas-db        Up 2 hours (healthy)   0.0.0.0:1433->1433/tcp
```

**Imágenes:**

```
REPOSITORY                       TAG           SIZE
reservas-xyz-web                 latest        412MB
nginx                            alpine        93.6MB
certbot/certbot                  latest        308MB
mcr.microsoft.com/mssql/server   2022-latest   2.31GB
```

<div style="page-break-after: always;"></div>

---

## 11. Despliegue en VPS

### 11.1 Infraestructura

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

### 11.2 Seguridad del VPS

- **Firewall GCP**: Solo puertos 22 (SSH), 80 (HTTP), 443 (HTTPS) abiertos.
- **Acceso SSH**: Clave ED25519, sin contraseña.
- **Usuario**: `deploy` (sin acceso root directo).
- **Secrets**: Archivo `.env` en el servidor (no en repositorio).
- **SSL/TLS**: Certificados Let's Encrypt con renovación automática.
- **HTTP → HTTPS**: Redirección 301 automática en Nginx.
- **Restart policy**: `unless-stopped` en todos los contenedores.

### 11.3 Persistencia

| Volumen       | Contenido           | Persistencia                      |
| ------------- | ------------------- | --------------------------------- |
| `sqldata`     | Datos de SQL Server | Sobrevive reinicios de contenedor |
| `certbot-etc` | Certificados SSL    | Sobrevive reinicios               |
| `certbot-var` | Datos de Certbot    | Sobrevive reinicios               |

### 11.4 Evidencia del despliegue en producción

![Sistema desplegado en producción](docs/screenshots/21-vps-evidence.png)

_Figura 11.1: Sistema ReservasXYZ operativo en producción con HTTPS. URL: https://reservas-xyz.app-dev.icu. Certificado SSL Let's Encrypt válido._

### 11.5 Vista responsive (mobile)

![Vista responsive mobile](docs/screenshots/22-responsive-view.png)

_Figura 11.2: Vista del sistema en resolución mobile (390×844). Diseño completamente responsive con Tailwind CSS v4._

<div style="page-break-after: always;"></div>

---

## 12. Instrucciones de Ejecución

### 12.1 Opción A: Ejecución local

**Requisitos previos:**

- .NET SDK 8.0
- SQL Server (LocalDB o instancia completa)
- Node.js 20+
- npm 9+

**Pasos:**

```powershell
# 1. Clonar repositorio
git clone https://github.com/juan-campo-dev/reservas-xyz.git
cd reservas-xyz

# 2. Restaurar paquetes NuGet
dotnet restore ReservasXYZ.sln

# 3. Instalar dependencias Node.js (Tailwind CSS)
cd ReservasXYZ.Web
npm install

# 4. Compilar CSS
npm run css:build

# 5. Volver a la raíz
cd ..

# 6. Compilar solución
dotnet build ReservasXYZ.sln

# 7. Ejecutar
dotnet run --project ReservasXYZ.Web --launch-profile http
```

La aplicación estará en **http://localhost:5263**.

La base de datos se crea automáticamente al iniciar:

1. Migraciones de EF Core.
2. Creación de roles (Admin, Receptionist, Guest, Cliente).
3. Usuario administrador (admin@hotel.com / Admin123!).
4. Usuarios de prueba.
5. 6 Stored Procedures.
6. Catálogo FODUN (8 sedes, 10 alojamientos, 54 habitaciones, tarifas).

### 12.2 Opción B: Docker Compose

**Requisitos previos:**

- Docker Engine 20.10+
- Docker Compose v2+

**Pasos:**

```bash
# 1. Copiar variables de entorno
cp .env.example .env
# Editar .env con credenciales reales

# 2. Construir y levantar
docker compose up -d --build

# 3. Verificar servicios
docker compose ps

# 4. Ver logs
docker compose logs -f web
```

La aplicación estará en **http://localhost** (puerto 80).

### 12.3 Opción C: Restaurar BD desde script SQL

Si se prefiere crear la base de datos manualmente sin el DataSeeder:

```sql
-- 1. Crear la base de datos
CREATE DATABASE ReservasXYZDb;
GO
USE ReservasXYZDb;
GO

-- 2. Ejecutar database/Database.sql (estructura + SPs)
-- 3. Ejecutar database/SeedCatalog.sql (catálogo FODUN)
```

### 12.4 Usuarios de prueba

| Rol                | Email            | Contraseña   |
| ------------------ | ---------------- | ------------ |
| **Admin**          | admin@hotel.com  | Admin123!    |
| **Admin (test)**   | admin@test.com   | Admin123\*   |
| **Cliente (test)** | cliente@test.com | Cliente123\* |

<div style="page-break-after: always;"></div>

---

## 13. Conclusiones

### 13.1 Cumplimiento de requisitos

El sistema implementa la **totalidad** de los requisitos solicitados en la prueba técnica:

| #   | Requisito                                           | Estado                                                        |
| --- | --------------------------------------------------- | ------------------------------------------------------------- |
| 1   | Estructura BD relacional SQL Server                 | ✅ Implementado                                               |
| 2   | SP disponibilidad por rango de fechas               | ✅ `sp_GetAvailableRoomsByDates`                              |
| 3   | SP disponibilidad por fechas + personas             | ✅ `sp_GetAvailableRoomsByDatesAndGuests`                     |
| 4   | SP tarifas por sitio/temporada/personas/alojamiento | ✅ `sp_GetRoomRates` + `sp_GetRatesByCriteria`                |
| 5   | SP cálculo tarifa total                             | ✅ `sp_CalculateTotalRate`                                    |
| 6   | Diseño formularios web                              | ✅ Razor Views para todos los módulos                         |
| 7   | Registro de usuarios                                | ✅ Nombre, apellido, documento, email                         |
| 8   | Login (solo autenticados)                           | ✅ `[Authorize]` + Identity                                   |
| 9   | Recuperación contraseña SMTP                        | ✅ MailKit + plantilla HTML                                   |
| 10  | CRUD formularios                                    | ✅ Sites, Accommodations, Rooms, Seasons, Rates, Reservations |
| 11  | Consulta disponibilidad                             | ✅ Por fechas y personas usando SPs                           |
| 12  | Guardar reservas                                    | ✅ Transacción SERIALIZABLE                                   |
| 13  | MVC .NET Core                                       | ✅ ASP.NET Core 8.0 MVC                                       |
| 14  | Programación en capas                               | ✅ 4 capas (Clean Architecture)                               |
| 15  | Entity Framework                                    | ✅ EF Core 8.0 + Fluent API                                   |
| 16  | Identity                                            | ✅ Roles + bloqueo + confirmación email                       |
| 17  | Documento técnico                                   | ✅ Este documento                                             |

### 13.2 Valor agregado

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

### 13.3 Decisiones de diseño

| Decisión                 | Justificación                                                                                 |
| ------------------------ | --------------------------------------------------------------------------------------------- |
| Clean Architecture       | Separación clara de responsabilidades. Domain sin dependencias externas. Testabilidad.        |
| SPs + EF Core            | SPs para lógica compleja día por día (tarificación). EF Core para CRUD simple. Fallback LINQ. |
| Transacción SERIALIZABLE | Nivel máximo de aislamiento para prevenir overbooking en reservas concurrentes.               |
| Docker multi-stage       | Imagen final optimizada (~220 MB). Compilación reproducible.                                  |
| Tailwind CSS v4          | Compilación local, sin CDN. Utilidades para UI responsive sin CSS custom extenso.             |

### 13.4 Mejoras futuras

- Tests unitarios y de integración con xUnit.
- Paginación server-side para listados grandes.
- Exportación de reportes a PDF/Excel.
- Galería de fotos por habitación.
- Notificaciones push en tiempo real (SignalR).
- API REST con Swagger/OpenAPI para integraciones externas.
- Multi-idioma con archivos de recursos (.resx).

---

_Documento técnico elaborado como parte de la entrega de la prueba técnica para el cargo de Analista Desarrollador .NET — Fondo de Empleados XYZ (FODUN)._

_Fecha de generación: Mayo 2026_
