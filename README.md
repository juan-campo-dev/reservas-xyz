# ReservasXYZ — Sistema de Gestión y Reservas Hoteleras

**Fondo de Empleados XYZ · Prueba técnica — Desarrollador .NET**

---

## Descripción

ReservasXYZ es un sistema web de gestión integral de reservas hoteleras desarrollado para el **Fondo de Empleados XYZ (FODUN)**. Permite a los asociados consultar disponibilidad de sedes recreativas y apartamentos, calcular tarifas por temporada y número de huéspedes, y gestionar reservas de forma autónoma. El panel administrativo ofrece control total sobre sedes, alojamientos, habitaciones, temporadas, tarifas y reservas.

## Objetivo del proyecto

Demostrar competencias técnicas en desarrollo .NET mediante la implementación de un sistema completo que cubre:

- Arquitectura en capas con separación clara de responsabilidades.
- Acceso a datos con Entity Framework Core y Stored Procedures SQL Server.
- Autenticación y autorización con ASP.NET Core Identity.
- Motor de tarificación dinámico basado en temporada, día de semana y personas adicionales.
- Interfaz responsiva con Tailwind CSS v4.
- Notificaciones por correo electrónico vía SMTP.

---

## Arquitectura

El proyecto implementa **Clean Architecture** con cuatro capas independientes:

```
ReservasXYZ.Domain          → Entidades, enums, interfaces de repositorio
ReservasXYZ.Application     → DTOs, servicios de aplicación, validadores, mappings
ReservasXYZ.Infrastructure  → DbContext, repositorios, email, seed, stored procedures
ReservasXYZ.Web             → Controllers MVC, Razor Views, Identity Pages, wwwroot
```

### Flujo de dependencias

| Capa | Depende de | Responsabilidad |
|------|-----------|----------------|
| **Domain** | — | Entidades de negocio, enums, contratos de repositorio |
| **Application** | Domain | DTOs, lógica de aplicación, validación, AutoMapper |
| **Infrastructure** | Domain, Application | EF Core, SQL Server, SMTP, Identity, Seed |
| **Web** | Application, Infrastructure | MVC Controllers, Razor Views, DI |

---

## Stack tecnológico

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET | 9.0 | Runtime y SDK |
| ASP.NET Core MVC | 9.0 | Framework web |
| Razor Views / Pages | 9.0 | Motor de vistas |
| Entity Framework Core | 9.0 | ORM y migraciones |
| SQL Server | 2019+ / LocalDB | Motor de base de datos |
| ASP.NET Core Identity | 9.0 | Autenticación y autorización |
| AutoMapper | 13.x | Mapeo entidades ↔ DTOs |
| FluentValidation | 11.x | Validación de DTOs |
| MailKit | 4.x | Envío SMTP |
| Tailwind CSS | 4.3.0 | Framework CSS utility-first |
| Node.js | 18+ | Build de CSS |

---

## Estructura del proyecto

```
ReservasXYZ.sln
│
├── ReservasXYZ.Domain/
│   ├── Entities/              # Site, Accommodation, Room, Season, Rate,
│   │                          # Reservation, ReservationDetail, ApplicationUser, Favorite
│   ├── Enums/                 # RoomType, ReservationStatus, RateKind
│   └── Interfaces/            # IRepository<T>, IRoomRepository, IRateRepository,
│                              # IReservationRepository
│
├── ReservasXYZ.Application/
│   ├── DTOs/                  # SiteDtos, RoomDtos, ReservationDtos, SeasonRateDtos, etc.
│   ├── Interfaces/            # ISiteService, IRoomService, IReservationService, etc.
│   ├── Services/              # SiteService, RoomService, ReservationService, etc.
│   ├── Validators/            # FluentValidation validators por DTO
│   └── Mappings/              # AutoMapper MappingProfile
│
├── ReservasXYZ.Infrastructure/
│   ├── Data/
│   │   ├── Context/           # ApplicationDbContext
│   │   ├── Configurations/    # Fluent API por entidad
│   │   ├── Repositories/      # Repository<T>, RoomRepository, RateRepository,
│   │   │                      # ReservationRepository
│   │   └── Seed/              # DataSeeder (roles, admin, catálogo FODUN, SPs)
│   ├── DependencyInjection/   # ServiceCollectionExtensions
│   ├── Email/                 # EmailService, EmailTemplateService, IdentityEmailSender
│   ├── Identity/              # SpanishIdentityErrorDescriber,
│   │                          # SpanishFluentValidationLanguageManager
│   ├── Services/              # DashboardService, GuestPortalService, FavoriteService
│   └── StoredProcedures/      # StoredProcedures.sql
│
├── ReservasXYZ.Web/
│   ├── Controllers/           # Home, Sites, Accommodations, Rooms, Seasons,
│   │                          # Rates, Reservations, Favorites
│   ├── Areas/Identity/        # Login, Register, ConfirmEmail, ForgotPassword,
│   │                          # ResetPassword, ResendEmailConfirmation
│   ├── Views/                 # Razor Views por módulo
│   ├── ViewModels/            # ViewModels adicionales
│   ├── wwwroot/
│   │   ├── css/               # site.css (Tailwind input), output.css (compilado)
│   │   ├── js/                # Módulos JS: feedback, favorites, auth, availability
│   │   └── images/            # Recursos estáticos
│   ├── Program.cs             # Startup, DI, middleware, cultura es-CO
│   └── appsettings.json       # ConnectionString, SMTP, Identity config
│
├── database/
│   ├── Database.sql           # Script completo de estructura (idempotente)
│   └── SeedCatalog.sql        # Datos de catálogo FODUN
│
└── docs/
    ├── reference/             # Documentación técnica
    ├── requirements/          # Prueba técnica y anexo FODUN
    └── setup/                 # Guías de instalación, deploy, BD
```

---

## Base de datos

### Modelo relacional

| Entidad | Descripción | Relaciones |
|---------|-------------|------------|
| **Site** | Sede recreativa o apartamento (8 sedes FODUN) | → Accommodations |
| **Accommodation** | Bloque de alojamiento dentro de una sede | → Site, → Rooms |
| **Room** | Habitación individual con tipo y capacidad | → Accommodation, → Rates, → ReservationDetails, → Favorites |
| **Season** | Temporada con rango de fechas y multiplicador | → Rates |
| **Rate** | Tarifa por habitación + temporada + tipo | → Room, → Season |
| **Reservation** | Reserva del usuario con estado de flujo | → User, → ReservationDetails |
| **ReservationDetail** | Línea de detalle por habitación reservada | → Reservation, → Room |
| **ApplicationUser** | Usuario Identity extendido con nombre, apellido y documento | → Reservations, → Favorites |
| **Favorite** | Relación usuario ↔ habitación favorita | → User, → Room |

### Sedes FODUN (catálogo seed)

| # | Sede | Ciudad |
|---|------|--------|
| 1 | Villeta | Villeta |
| 2 | El Placer | Fusagasugá |
| 3 | Gonzalo Morante | Chinchiná |
| 4 | Tablones | Palmira |
| 5 | Manguruma | Santa Fe de Antioquia |
| 6 | Federmán | Bogotá |
| 7 | Suramericana | Medellín |
| 8 | El Rodadero | Santa Marta |

---

## Stored Procedures

### `sp_GetAvailableRoomsByDates`

Consulta habitaciones disponibles en un rango de fechas.

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `@CheckIn` | `DATE` | Fecha de entrada |
| `@CheckOut` | `DATE` | Fecha de salida |
| **Retorno** | Tabla | Id, RoomNumber, Type, MaxGuests, BasePrice, AccommodationName, SiteName |

### `sp_GetAvailableRoomsByDatesAndGuests`

Igual que el anterior, filtrando por capacidad mínima de huéspedes.

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `@CheckIn` | `DATE` | Fecha de entrada |
| `@CheckOut` | `DATE` | Fecha de salida |
| `@Guests` | `INT` | Número de huéspedes |

### `sp_GetRoomRates`

Retorna las tarifas configuradas para una habitación.

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `@RoomId` | `INT` | ID de la habitación |
| **Retorno** | Tabla | RateId, PricePerNight, SeasonName, StartDate, EndDate, PriceMultiplier |

### `sp_CalculateTotalRate`

Motor de tarificación FODUN. Calcula el precio total iterando día por día.

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `@RoomId` | `INT` | ID de la habitación |
| `@CheckIn` | `DATE` | Fecha de entrada |
| `@CheckOut` | `DATE` | Fecha de salida |
| `@TotalGuests` | `INT` | Número de huéspedes |
| `@TotalRate` | `DECIMAL OUTPUT` | Tarifa total calculada |

**Reglas de negocio:**

1. Itera cada noche del rango `[CheckIn, CheckOut)`.
2. Busca la tarifa activa (Rate + Season) vigente para esa fecha.
3. Si el día es lunes a jueves y existe tarifa `SpecialWeekday`, la usa directamente (sin multiplicador de temporada).
4. Para tarifa `Standard`: `PricePerNight × Season.PriceMultiplier`.
5. Suma extras por persona: `MAX(0, TotalGuests − BaseGuests) × ExtraPersonPrice`.
6. Si no hay tarifa configurada, usa `Room.BasePrice` como fallback.

### `sp_ValidateOverbooking`

Verifica si una habitación tiene reservas solapadas (excluyendo canceladas).

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `@RoomId` | `INT` | ID de la habitación |
| `@CheckIn` | `DATE` | Fecha de entrada |
| `@CheckOut` | `DATE` | Fecha de salida |
| `@ExcludeReservationId` | `INT` | ID a excluir (para edición) |
| `@IsOverbooked` | `BIT OUTPUT` | `1` si hay solapamiento |

### `sp_GetRatesByCriteria`

Consulta tarifas cruzando sede, temporada, alojamiento y número de personas.

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `@SiteId` | `INT NULL` | Filtro por sede |
| `@AccommodationId` | `INT NULL` | Filtro por alojamiento |
| `@SeasonId` | `INT NULL` | Filtro por temporada |
| `@Guests` | `INT NULL` | Número de huéspedes |
| `@CheckIn` | `DATE NULL` | Fecha de entrada |
| `@CheckOut` | `DATE NULL` | Fecha de salida |

---

## Configuración local

### Requisitos previos

| Requisito | Versión mínima |
|-----------|---------------|
| .NET SDK | 9.0 |
| SQL Server | 2019+ o LocalDB |
| Node.js | 18+ |
| npm | 9+ |

### Variables de configuración (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ReservasXYZDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "tu-email@gmail.com",
    "SenderName": "Reservas XYZ",
    "Password": "tu-app-password"
  },
  "Identity": {
    "RequireConfirmedAccount": true
  }
}
```

> **Nota:** Para Gmail, genere una contraseña de aplicación en la configuración de seguridad de su cuenta Google.

---

## Instalación paso a paso

```powershell
# 1. Clonar el repositorio
git clone <url-repositorio>
cd Hotel

# 2. Restaurar paquetes NuGet
dotnet restore ReservasXYZ.sln

# 3. Instalar dependencias de Node.js (Tailwind CSS)
cd ReservasXYZ.Web
npm install

# 4. Compilar CSS
npm run css:build

# 5. Volver a la raíz
cd ..

# 6. Compilar la solución
dotnet build ReservasXYZ.sln

# 7. Ejecutar el proyecto
dotnet run --project .\ReservasXYZ.Web --launch-profile http
```

La aplicación estará disponible en **http://localhost:5263**.

### Base de datos

La base de datos se crea y migra automáticamente al iniciar la aplicación. El `DataSeeder` ejecuta en orden:

1. Migraciones de EF Core pendientes.
2. Creación de roles (`Admin`, `Receptionist`, `Guest`).
3. Creación del usuario administrador.
4. Aplicación de los 6 Stored Procedures.
5. Seed del catálogo FODUN (8 sedes, 10 alojamientos, 54 habitaciones, 3 temporadas, tarifas).

### Credenciales de desarrollo

| Campo | Valor |
|-------|-------|
| Email | `admin@hotel.com` |
| Contraseña | `Admin123!` |
| Rol | `Admin` |

---

## Funcionalidades

### Portal público

- **Registro** con nombre, apellido, documento e email.
- **Confirmación de email** por SMTP con plantilla HTML profesional.
- **Inicio de sesión** con bloqueo por intentos fallidos (5 intentos, 15 min).
- **Recuperación de contraseña** vía enlace por email.

### Portal de asociados (Guest)

- **Búsqueda de disponibilidad** por sede, fechas y número de huéspedes.
- **Resultados** con precio calculado por el motor de tarificación (SP).
- **Reserva** con selección de habitaciones, resumen de costos y desglose de extras.
- **Mis reservas** con filtro por estado y búsqueda.
- **Favoritos** (guardar/quitar habitaciones).
- **Notificación por email** al crear reserva.

### Panel administrativo (Admin)

- **Dashboard** con métricas: reservas activas, ingresos, ocupación.
- **CRUD Sedes** — crear, editar, desactivar sedes.
- **CRUD Alojamientos** — asociados a sedes.
- **CRUD Habitaciones** — tipo, capacidad, precio base, amenidades.
- **CRUD Temporadas** — rangos de fechas, multiplicador de precio.
- **CRUD Tarifas** — por habitación + temporada + tipo (Standard / SpecialWeekday).
- **Gestión de reservas** — confirmar, check-in, check-out, cancelar.
- Modales AJAX para formularios de creación/edición.

---

## Seguridad

| Control | Implementación |
|---------|---------------|
| **Autenticación** | ASP.NET Core Identity con cookies |
| **Autorización** | Roles (`Admin`, `Receptionist`, `Guest`) con `[Authorize]` |
| **Confirmación de email** | Obligatoria antes de iniciar sesión |
| **Bloqueo de cuenta** | 5 intentos fallidos → 15 minutos de bloqueo |
| **Contraseña** | Mínimo 8 caracteres, mayúscula, minúscula, dígito, carácter especial |
| **CSRF** | `[ValidateAntiForgeryToken]` en todos los POST |
| **Overposting** | DTOs específicos para Create/Update (sin exponer campos internos) |
| **Overbooking** | SP `sp_ValidateOverbooking` + transacción `SERIALIZABLE` |
| **XSS** | Razor escapa automáticamente; `HtmlEncoder` en enlaces de email |
| **SQL Injection** | EF Core parametrizado + SPs con parámetros tipados |
| **Mensajes genéricos** | Login no revela si el email existe o no |
| **Identity errors** | `SpanishIdentityErrorDescriber` para mensajes en español |

---

## Decisiones técnicas

| Decisión | Justificación |
|----------|---------------|
| **Clean Architecture en 4 capas** | Separación clara de responsabilidades. Domain sin dependencias externas. |
| **Stored Procedures + EF Core** | Requisito FODUN para consultas de disponibilidad y tarificación. SP para lógica compleja día por día; EF Core para CRUD simple. |
| **FluentValidation** | Validación declarativa en capa Application, independiente del framework web. Mensajes en español configurados globalmente. |
| **AutoMapper** | Mapeo automático entre entidades y DTOs, reduce boilerplate. |
| **Transacción SERIALIZABLE** | Previene race conditions en reservas concurrentes sobre la misma habitación. |
| **Soft delete** | `IsActive = false` en lugar de eliminar registros, preservando integridad referencial. |
| **CultureInfo es-CO** | Formato de moneda, fechas y validación en español colombiano. |
| **Tailwind CSS v4** | Compilación local, sin CDN. Clases utilitarias para UI responsiva. |
| **Fallback LINQ en repositorios** | Si un SP falla o no existe, el repositorio ejecuta la lógica equivalente con LINQ. |
| **Propiedades en inglés / UI en español** | Convención .NET estándar: código fuente en inglés, toda la interfaz de usuario y mensajes visibles en español. |

---

## Mejoras futuras

- [ ] Tests unitarios y de integración.
- [ ] Paginación server-side en listados grandes.
- [ ] Exportación de reportes a PDF/Excel.
- [ ] Galería de fotos por habitación.
- [ ] Sistema de reseñas de huéspedes.
- [ ] Notificaciones push en tiempo real (SignalR).
- [ ] Multi-idioma con archivos de recursos (`.resx`).
- [ ] API REST con Swagger/OpenAPI para integraciones externas.
- [ ] Deploy con Docker Compose (SQL Server + Web).

---

## Troubleshooting

| Problema | Solución |
|----------|---------|
| Error de conexión a BD | Verificar `ConnectionStrings:DefaultConnection` en `appsettings.json`. Confirmar que SQL Server/LocalDB está corriendo. |
| No se envían emails | Configurar `EmailSettings` con credenciales SMTP válidas. Para Gmail usar contraseña de aplicación. |
| CSS no se actualiza | Ejecutar `npm run css:build` en `ReservasXYZ.Web/`. Ctrl+F5 en el navegador. |
| Error de migración | Ejecutar `dotnet ef database update --project ReservasXYZ.Infrastructure --startup-project ReservasXYZ.Web`. |
| Seed no se ejecuta | Verificar que la BD es accesible al inicio. Revisar logs en consola. |
| Puerto 5263 ocupado | Detener procesos: `Get-NetTCPConnection -LocalPort 5263` y `Stop-Process -Id <PID>`. |

---

## Entrega

Documentos complementarios:

- `docs/setup/INSTALL.md` — Guía de instalación.
- `docs/setup/DEPLOYMENT.md` — Guía de despliegue.
- `docs/setup/DATABASE_SETUP.md` — Configuración de base de datos.
- `docs/reference/TECHNICAL_DOCUMENTATION.md` — Documentación técnica detallada.
- `docs/requirements/` — Prueba técnica y anexo FODUN.
- `database/Database.sql` — Script SQL completo.
- `database/SeedCatalog.sql` — Datos de catálogo FODUN.

---

## Licencia

Proyecto desarrollado como prueba técnica. Uso exclusivamente académico y de evaluación.

---

> **ReservasXYZ** · Fondo de Empleados XYZ · ASP.NET Core 9.0 · SQL Server · 2026
