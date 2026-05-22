# Documento Técnico — ReservasXYZ

## Sistema de Gestión y Reservas Hoteleras

**Fondo de Empleados XYZ (FODUN)**

**Prueba Técnica — Analista Desarrollador .NET**

**Fecha:** Mayo 2026

---

## Tabla de Contenidos

1. [Introducción](#1-introducción)
2. [Arquitectura del Proyecto](#2-arquitectura-del-proyecto)
3. [Programación en Capas](#3-programación-en-capas)
4. [Modelo de Base de Datos Relacional](#4-modelo-de-base-de-datos-relacional)
5. [Diagrama de Relaciones](#5-diagrama-de-relaciones)
6. [Explicación de Tablas](#6-explicación-de-tablas)
7. [Entity Framework Core](#7-entity-framework-core)
8. [ASP.NET Core Identity](#8-aspnet-core-identity)
9. [Lógica de Reservas](#9-lógica-de-reservas)
10. [Disponibilidad](#10-disponibilidad)
11. [Motor de Tarificación](#11-motor-de-tarificación)
12. [Stored Procedures](#12-stored-procedures)
13. [Servicio de Email (SMTP)](#13-servicio-de-email-smtp)
14. [Docker y Contenedores](#14-docker-y-contenedores)
15. [Despliegue en VPS](#15-despliegue-en-vps)
16. [Seguridad](#16-seguridad)
17. [Tecnologías y Librerías](#17-tecnologías-y-librerías)
18. [Instrucciones de Ejecución](#18-instrucciones-de-ejecución)
19. [Capturas de Pantalla](#19-capturas-de-pantalla)
20. [Conclusiones](#20-conclusiones)

---

## 1. Introducción

### Contexto

El Fondo de Empleados XYZ (FODUN) cuenta con múltiples sedes recreativas y apartamentos distribuidos en diferentes ciudades de Colombia. Actualmente, la información de disponibilidad y reservas se gestiona manualmente, lo que genera ineficiencias operativas y dificultades para los asociados al momento de planificar su uso.

### Objetivo

Desarrollar un sistema web que permita a los asociados del Fondo consultar la disponibilidad de sedes recreativas y apartamentos, visualizar tarifas según temporada y número de personas, y realizar reservas en línea. El sistema debe incluir un panel administrativo para la gestión integral de sedes, alojamientos, habitaciones, temporadas, tarifas y reservas.

### Alcance

El sistema cubre:

- Registro y autenticación de usuarios con Identity.
- Consulta de disponibilidad por fechas y capacidad.
- Cálculo de tarifas dinámico basado en temporada, día de semana y personas adicionales.
- Creación y gestión de reservas con control de overbooking.
- Panel administrativo con CRUD completo.
- Notificaciones por correo electrónico (SMTP).
- Despliegue con Docker en VPS con SSL.

---

## 2. Arquitectura del Proyecto

El proyecto implementa **Clean Architecture** (Arquitectura Limpia) con separación en cuatro capas independientes. Esta arquitectura fue elegida por las siguientes razones:

- **Independencia del framework**: El dominio no depende de ASP.NET Core ni de EF Core.
- **Testabilidad**: Cada capa puede probarse de forma aislada.
- **Mantenibilidad**: Los cambios en una capa no afectan a las demás.
- **Separación de responsabilidades**: Cada capa tiene un propósito claro y definido.

### Diagrama de capas

```
┌─────────────────────────────────────────────────┐
│              ReservasXYZ.Web                     │
│    Controllers · Razor Views · Identity Pages    │
│    wwwroot · DI Configuration · Middleware        │
├─────────────────────────────────────────────────┤
│           ReservasXYZ.Application                │
│      DTOs · Services · Validators · Mappings     │
├─────────────────────────────────────────────────┤
│          ReservasXYZ.Infrastructure              │
│    DbContext · Repositories · Email · Seed · SPs │
├─────────────────────────────────────────────────┤
│             ReservasXYZ.Domain                   │
│        Entities · Enums · Interfaces             │
└─────────────────────────────────────────────────┘
```

### Flujo de dependencias

```
Web → Application → Domain ← Infrastructure
```

- **Domain** no depende de nada externo.
- **Application** depende solo de Domain.
- **Infrastructure** depende de Domain y Application.
- **Web** depende de Application e Infrastructure (solo para DI).

---

## 3. Programación en Capas

### Capa de Dominio (`ReservasXYZ.Domain`)

Contiene las entidades de negocio puras, sin dependencias de frameworks externos:

- **Entities**: `Site`, `Accommodation`, `Room`, `Season`, `Rate`, `Reservation`, `ReservationDetail`, `ApplicationUser`, `Favorite`.
- **Enums**: `RoomType` (8 tipos), `ReservationStatus` (6 estados), `RateKind` (Standard, SpecialWeekday).
- **Interfaces de repositorio**: `IRepository<T>`, `IRoomRepository`, `IRateRepository`, `IReservationRepository`.

Las interfaces de repositorio definen contratos que la capa de Infrastructure implementa. Esto permite la inversión de dependencias (Principio D de SOLID).

### Capa de Aplicación (`ReservasXYZ.Application`)

Orquesta la lógica de aplicación sin conocer los detalles de implementación:

- **DTOs**: Objetos de transferencia de datos para cada operación (Create, Update, View). Evitan exponer las entidades directamente a la capa Web.
- **Interfaces de servicios**: `ISiteService`, `IRoomService`, `IReservationService`, `IRateService`, `ISeasonService`, `IAccommodationService`, `IGuestPortalService`, `IDashboardService`, `IFavoriteService`.
- **Servicios**: Implementaciones que coordinan repositorios y aplican reglas de negocio.
- **Validators**: Validadores FluentValidation para cada DTO, con mensajes en español.
- **Mappings**: Perfil de AutoMapper para conversión entidad ↔ DTO.

### Capa de Infraestructura (`ReservasXYZ.Infrastructure`)

Implementa los contratos definidos en las capas superiores:

- **ApplicationDbContext**: Contexto de EF Core con configuraciones Fluent API.
- **Repositorios**: `Repository<T>` genérico + especializados (`RoomRepository`, `RateRepository`, `ReservationRepository`).
- **DataSeeder**: Carga automática de roles, usuarios, SPs y catálogo FODUN.
- **Email**: `EmailService` (MailKit), `EmailTemplateService` (plantillas HTML), `IdentityEmailSender`.
- **Identity**: Personalización de mensajes de error al español.
- **StoredProcedures**: 6 SPs SQL Server cargados al iniciar.

### Capa Web (`ReservasXYZ.Web`)

Interfaz de usuario y punto de entrada de la aplicación:

- **Controllers**: `HomeController`, `SitesController`, `AccommodationsController`, `RoomsController`, `SeasonsController`, `RatesController`, `ReservationsController`, `FavoritesController`.
- **Razor Views**: Vistas organizadas por controlador con layout compartido.
- **Areas/Identity**: Páginas personalizadas de Login, Register, ForgotPassword, ResetPassword, ConfirmEmail.
- **wwwroot**: CSS (Tailwind), JS, imágenes.
- **Program.cs**: Configuración de DI, middleware y cultura es-CO.

---

## 4. Modelo de Base de Datos Relacional

El modelo relacional fue diseñado para representar fielmente la estructura organizacional del Fondo XYZ:

```
Sites (1) ──── (N) Accommodations (1) ──── (N) Rooms
                                                  │
                                            (1) ──┼── (N) Rates
                                                  │        │
                                                  │   Seasons (1) ── (N) Rates
                                                  │
                                            (1) ──┼── (N) ReservationDetails
                                                  │        │
                                                  │   Reservations (1) ── (N) ReservationDetails
                                                  │        │
                                                  │   ApplicationUser (1) ── (N) Reservations
                                                  │
                                            (1) ──┼── (N) Favorites
                                                         │
                                                  ApplicationUser (1) ── (N) Favorites
```

---

## 5. Diagrama de Relaciones

### Relaciones principales

| Relación                        | Tipo | Descripción                                              |
| ------------------------------- | ---- | -------------------------------------------------------- |
| Site → Accommodation            | 1:N  | Una sede tiene múltiples alojamientos                    |
| Accommodation → Room            | 1:N  | Un alojamiento tiene múltiples habitaciones              |
| Room → Rate                     | 1:N  | Una habitación tiene tarifas por temporada               |
| Season → Rate                   | 1:N  | Una temporada define tarifas para múltiples habitaciones |
| ApplicationUser → Reservation   | 1:N  | Un usuario puede tener múltiples reservas                |
| Reservation → ReservationDetail | 1:N  | Una reserva puede incluir múltiples habitaciones         |
| Room → ReservationDetail        | 1:N  | Una habitación puede estar en múltiples reservas         |
| ApplicationUser → Favorite      | 1:N  | Un usuario puede marcar habitaciones como favoritas      |
| Room → Favorite                 | 1:N  | Una habitación puede ser favorita de múltiples usuarios  |

### Índices

| Tabla              | Índice                           | Tipo   | Columnas                    |
| ------------------ | -------------------------------- | ------ | --------------------------- |
| Rooms              | IX_Rooms_Accommodation_Number    | Unique | AccommodationId, RoomNumber |
| Rates              | IX_Rates_Room_Season_Kind        | Unique | RoomId, SeasonId, Kind      |
| Favorites          | IX_Favorites_User_Room           | Unique | UserId, RoomId              |
| ReservationDetails | IX_ReservationDetail_Reservation | —      | ReservationId               |
| ReservationDetails | IX_ReservationDetail_Room        | —      | RoomId                      |

---

## 6. Explicación de Tablas

### Sites (Sedes)

Representa las sedes recreativas y apartamentos del Fondo XYZ.

| Columna     | Tipo          | Descripción                          |
| ----------- | ------------- | ------------------------------------ |
| Id          | int (PK)      | Identificador único                  |
| Name        | nvarchar(200) | Nombre de la sede                    |
| City        | nvarchar(100) | Ciudad                               |
| Address     | nvarchar(500) | Dirección                            |
| Description | nvarchar(MAX) | Descripción detallada                |
| MaxCapacity | int           | Capacidad máxima de personas         |
| ImageUrl    | nvarchar(500) | URL de imagen                        |
| IsActive    | bit           | Estado activo/inactivo (soft delete) |

**Sedes FODUN cargadas**: Villeta, El Placer (Fusagasugá), Gonzalo Morante (Chinchiná), Tablones (Palmira), Manguruma (Santa Fe de Antioquia), Federmán (Bogotá), Suramericana (Medellín), El Rodadero (Santa Marta).

### Accommodations (Alojamientos)

Bloques de alojamiento dentro de una sede.

| Columna     | Tipo             | Descripción             |
| ----------- | ---------------- | ----------------------- |
| Id          | int (PK)         | Identificador único     |
| Name        | nvarchar(200)    | Nombre del alojamiento  |
| Description | nvarchar(MAX)    | Descripción             |
| SiteId      | int (FK → Sites) | Sede a la que pertenece |
| IsActive    | bit              | Estado activo/inactivo  |

### Rooms (Habitaciones)

Habitaciones individuales con tipo y capacidad.

| Columna         | Tipo                      | Descripción                                                         |
| --------------- | ------------------------- | ------------------------------------------------------------------- |
| Id              | int (PK)                  | Identificador único                                                 |
| RoomNumber      | nvarchar(20)              | Número/código de habitación                                         |
| Type            | int (enum RoomType)       | Tipo: Single, Double, Twin, Suite, Deluxe, Family, Cabin, Apartment |
| MaxGuests       | int                       | Capacidad máxima de huéspedes                                       |
| BasePrice       | decimal(18,2)             | Precio base por noche (fallback si no hay Rate)                     |
| Description     | nvarchar(MAX)             | Descripción                                                         |
| AccommodationId | int (FK → Accommodations) | Alojamiento al que pertenece                                        |
| IsActive        | bit                       | Estado activo/inactivo                                              |

### Seasons (Temporadas)

Períodos con multiplicador de precio.

| Columna         | Tipo          | Descripción                                |
| --------------- | ------------- | ------------------------------------------ |
| Id              | int (PK)      | Identificador único                        |
| Name            | nvarchar(100) | Nombre (Baja, Alta, Especial)              |
| StartDate       | date          | Fecha de inicio                            |
| EndDate         | date          | Fecha de fin                               |
| PriceMultiplier | decimal(5,2)  | Multiplicador de precio (1.0 = sin cambio) |
| IsActive        | bit           | Estado activo/inactivo                     |

### Rates (Tarifas)

Tarifa por habitación + temporada + tipo.

| Columna          | Tipo                | Descripción                                |
| ---------------- | ------------------- | ------------------------------------------ |
| Id               | int (PK)            | Identificador único                        |
| PricePerNight    | decimal(18,2)       | Precio por noche                           |
| BaseGuests       | int                 | Huéspedes incluidos en el precio base      |
| ExtraPersonPrice | decimal(18,2)       | Precio por persona adicional               |
| Kind             | int (enum RateKind) | 0 = Standard, 1 = SpecialWeekday (Lun-Jue) |
| RoomId           | int (FK → Rooms)    | Habitación                                 |
| SeasonId         | int (FK → Seasons)  | Temporada                                  |
| IsActive         | bit                 | Estado activo/inactivo                     |

### Reservations (Reservas)

Reservas de los usuarios.

| Columna     | Tipo                             | Descripción               |
| ----------- | -------------------------------- | ------------------------- |
| Id          | int (PK)                         | Identificador único       |
| CheckIn     | date                             | Fecha de entrada          |
| CheckOut    | date                             | Fecha de salida           |
| TotalGuests | int                              | Número total de huéspedes |
| TotalPrice  | decimal(18,2)                    | Precio total calculado    |
| Status      | int (enum ReservationStatus)     | Estado del flujo          |
| Notes       | nvarchar(MAX)                    | Notas adicionales         |
| UserId      | nvarchar(450) (FK → AspNetUsers) | Usuario que reservó       |
| CreatedAt   | datetime2                        | Fecha de creación         |
| UpdatedAt   | datetime2                        | Última modificación       |

**Estados del flujo**: Pending (0) → Confirmed (1) → CheckedIn (2) → CheckedOut (3). También: Cancelled (5), NoShow (4).

### ReservationDetails (Detalle de reserva)

Líneas de detalle por habitación reservada.

| Columna       | Tipo                    | Descripción                               |
| ------------- | ----------------------- | ----------------------------------------- |
| Id            | int (PK)                | Identificador único                       |
| ReservationId | int (FK → Reservations) | Reserva padre                             |
| RoomId        | int (FK → Rooms)        | Habitación reservada                      |
| PricePerNight | decimal(18,2)           | Precio por noche al momento de la reserva |
| Subtotal      | decimal(18,2)           | Subtotal de esta línea                    |
| GuestsInRoom  | int                     | Huéspedes asignados a esta habitación     |

---

## 7. Entity Framework Core

### Versión y proveedor

- **EF Core 8.0.25** con proveedor `Microsoft.EntityFrameworkCore.SqlServer`.
- **Code-First** con migraciones automáticas al iniciar.

### Configuraciones Fluent API

Cada entidad tiene su configuración Fluent API en archivos separados:

- `SiteConfiguration`, `AccommodationConfiguration`, `RoomConfiguration`
- `SeasonConfiguration`, `RateConfiguration`
- `ReservationConfiguration`, `ReservationDetailConfiguration`
- `FavoriteConfiguration`, `ApplicationUserConfiguration`

### Características utilizadas

- **Índices únicos**: Combinación (AccommodationId, RoomNumber), (RoomId, SeasonId, Kind), (UserId, RoomId).
- **Cascade delete**: Site → Accommodations → Rooms. Reservation → ReservationDetails.
- **Restrict delete**: Room → ReservationDetails (no se puede eliminar una habitación con reservas).
- **Shadow properties**: No utilizadas; todas las propiedades son explícitas.
- **Migraciones**: Registradas manualmente en `__EFMigrationsHistory` por el DataSeeder.

### Patrón Repository

```
IRepository<T> (interfaz genérica)
  ├── GetByIdAsync(id)
  ├── GetAllAsync()
  ├── FindAsync(predicate)
  ├── AddAsync(entity)
  ├── Update(entity)
  ├── Delete(entity)
  └── SaveChangesAsync()

IRoomRepository (especializado)
  ├── GetAvailableRoomsAsync(checkIn, checkOut)
  ├── GetAvailableRoomsByGuestsAsync(checkIn, checkOut, guests)
  └── IsRoomAvailableAsync(roomId, checkIn, checkOut)

IRateRepository (especializado)
  └── CalculateTotalRateAsync(roomId, checkIn, checkOut, guests)

IReservationRepository (especializado)
  └── HasOverlappingReservationAsync(roomId, checkIn, checkOut, excludeId)
```

---

## 8. ASP.NET Core Identity

### Configuración

| Parámetro                         | Valor                    |
| --------------------------------- | ------------------------ |
| **Longitud mínima de contraseña** | 8 caracteres             |
| **Requiere mayúscula**            | Sí                       |
| **Requiere minúscula**            | Sí                       |
| **Requiere dígito**               | Sí                       |
| **Requiere carácter especial**    | Sí                       |
| **Bloqueo por intentos fallidos** | 5 intentos → 15 minutos  |
| **Confirmación de email**         | Configurable por entorno |

### Roles

| Rol              | Permisos                                                   |
| ---------------- | ---------------------------------------------------------- |
| **Admin**        | CRUD completo, gestión de reservas, dashboard              |
| **Receptionist** | Gestión de reservas y check-in/check-out                   |
| **Guest**        | Consultar disponibilidad, crear reservas, ver mis reservas |
| **Cliente**      | Igual que Guest (alias para asociados)                     |

### Personalización

- **SpanishIdentityErrorDescriber**: Traduce todos los mensajes de error de Identity al español.
- **SpanishFluentValidationLanguageManager**: Traduce los mensajes de FluentValidation al español.
- **ApplicationUser**: Extiende IdentityUser con `FirstName`, `LastName`, `DocumentNumber`, `IsActive`.

### Páginas de Identity personalizadas

- Login (`/Identity/Account/Login`)
- Registro (`/Identity/Account/Register`)
- Confirmación de email (`/Identity/Account/ConfirmEmail`)
- Olvidé contraseña (`/Identity/Account/ForgotPassword`)
- Restablecer contraseña (`/Identity/Account/ResetPassword`)
- Reenviar confirmación (`/Identity/Account/ResendEmailConfirmation`)

---

## 9. Lógica de Reservas

### Flujo de estados

```
     ┌──────────┐     ┌───────────┐     ┌───────────┐     ┌────────────┐
     │ Pending  │ ──→ │ Confirmed │ ──→ │ CheckedIn │ ──→ │ CheckedOut │
     └──────────┘     └───────────┘     └───────────┘     └────────────┘
          │                │
          ▼                ▼
     ┌───────────┐   ┌───────────┐
     │ Cancelled │   │  NoShow   │
     └───────────┘   └───────────┘
```

### Proceso de creación

1. El usuario busca disponibilidad (sede, fechas, personas).
2. El sistema ejecuta SP1/SP2 para encontrar habitaciones disponibles.
3. El usuario selecciona una o más habitaciones.
4. El sistema ejecuta SP4 para calcular el precio total de cada habitación.
5. Se verifica overbooking con SP5 + transacción SERIALIZABLE.
6. Se crea la reserva con estado "Pending".
7. Se genera un ReservationDetail por cada habitación seleccionada.
8. Se envía notificación por email al usuario.

### Prevención de overbooking

Se implementa un doble control:

1. **SP5 (`sp_ValidateOverbooking`)**: Verifica en SQL Server si existe solapamiento de fechas para la misma habitación.
2. **Transacción SERIALIZABLE**: El repositorio envuelve la verificación y creación en una transacción con nivel de aislamiento `SERIALIZABLE`, que bloquea los registros hasta completar la operación. Esto previene condiciones de carrera en escenarios concurrentes.

---

## 10. Disponibilidad

### Algoritmo de disponibilidad

Una habitación se considera **disponible** si:

1. `IsActive = 1` (no está desactivada).
2. No tiene reservas activas (estado ≠ Cancelled) que se solapen con las fechas solicitadas.

### Solapamiento de fechas

Dos reservas se solapan cuando:

```
ReservaExistente.CheckIn < NuevaReserva.CheckOut
AND ReservaExistente.CheckOut > NuevaReserva.CheckIn
```

Esta lógica está implementada tanto en los Stored Procedures como en los repositorios LINQ (fallback).

### Filtro por capacidad

SP2 agrega el filtro `r.MaxGuests >= @Guests` para mostrar solo habitaciones con capacidad suficiente.

---

## 11. Motor de Tarificación

### Reglas de negocio FODUN

El motor de tarificación implementa las reglas específicas del Fondo XYZ:

| Concepto                       | Regla                                                                  |
| ------------------------------ | ---------------------------------------------------------------------- |
| **Tarifa base**                | Precio por noche para hasta `BaseGuests` personas                      |
| **Persona adicional**          | `ExtraPersonPrice` por cada huésped adicional sobre `BaseGuests`       |
| **Tarifa especial Lun-Jue**    | Precio reducido aplicable de lunes a jueves, excepto en alta temporada |
| **Multiplicador de temporada** | `PriceMultiplier` de la temporada vigente (1.0 = baja, >1.0 = alta)    |
| **Fallback**                   | Si no hay tarifa configurada, se usa `Room.BasePrice`                  |

### Cálculo día por día

El SP4 itera cada noche del rango `[CheckIn, CheckOut)`:

```
Para cada noche:
  1. Determinar si es Lun-Jue
  2. Buscar tarifa activa (Rate + Season) vigente para esa fecha
  3. Si es Lun-Jue Y existe tarifa SpecialWeekday Y PriceMultiplier ≤ 1:
     → Usar tarifa especial (sin multiplicador)
  4. Si no:
     → Usar tarifa Standard × PriceMultiplier
  5. Sumar extras: MAX(0, TotalGuests − BaseGuests) × ExtraPersonPrice
  6. Acumular en TotalRate
```

### Ejemplo de cálculo

Reserva de 3 noches (viernes a lunes) para 6 personas:

- Habitación: Tarifa base $70.000/noche para 4 personas, extra $16.000/persona.
- Temporada baja (PriceMultiplier = 1.0).
- Tarifa especial Lun-Jue: $27.000/noche para 4 personas, extra $11.000/persona.

| Noche | Día     | Tarifa base   | Extra (2 personas) | Total noche |
| ----- | ------- | ------------- | ------------------ | ----------- |
| 1     | Viernes | $70.000 × 1.0 | 2 × $16.000        | $102.000    |
| 2     | Sábado  | $70.000 × 1.0 | 2 × $16.000        | $102.000    |
| 3     | Domingo | $70.000 × 1.0 | 2 × $16.000        | $102.000    |

**Total**: $306.000

Si el lunes estuviera incluido: $27.000 + (2 × $11.000) = $49.000 esa noche.

---

## 12. Stored Procedures

Se implementaron **6 Stored Procedures** en SQL Server, cubriendo los 4 requeridos por la prueba técnica más 2 auxiliares.

### SP1: `sp_GetAvailableRoomsByDates`

**Requisito**: Encontrar habitaciones disponibles en un rango de fechas.

| Parámetro   | Tipo  | Dirección |
| ----------- | ----- | --------- |
| `@CheckIn`  | DATE  | Entrada   |
| `@CheckOut` | DATE  | Entrada   |
| Resultado   | Tabla | Salida    |

**Lógica**: Selecciona todas las habitaciones activas cuyo Id no aparezca en ReservationDetails de reservas no canceladas que se solapen con el rango solicitado. Ordena por precio.

### SP2: `sp_GetAvailableRoomsByDatesAndGuests`

**Requisito**: Encontrar habitaciones disponibles por fechas + número de personas.

| Parámetro   | Tipo  | Dirección |
| ----------- | ----- | --------- |
| `@CheckIn`  | DATE  | Entrada   |
| `@CheckOut` | DATE  | Entrada   |
| `@Guests`   | INT   | Entrada   |
| Resultado   | Tabla | Salida    |

**Lógica**: Igual que SP1, agregando el filtro `MaxGuests >= @Guests`.

### SP3: `sp_GetRoomRates`

**Requisito**: Ver tarifas según sitio, temporada, personas y alojamiento.

| Parámetro | Tipo  | Dirección |
| --------- | ----- | --------- |
| `@RoomId` | INT   | Entrada   |
| Resultado | Tabla | Salida    |

**Lógica**: JOIN entre Rates, Seasons y Rooms para obtener todas las tarifas configuradas de una habitación con sus temporadas.

### SP4: `sp_CalculateTotalRate`

**Requisito**: Calcular la tarifa a cancelar según sitio, habitaciones, personas, alojamiento y temporada.

| Parámetro      | Tipo          | Dirección |
| -------------- | ------------- | --------- |
| `@RoomId`      | INT           | Entrada   |
| `@CheckIn`     | DATE          | Entrada   |
| `@CheckOut`    | DATE          | Entrada   |
| `@TotalGuests` | INT           | Entrada   |
| `@TotalRate`   | DECIMAL(18,2) | Salida    |

**Lógica**: Motor de tarificación iterativo (día por día). Usa `SET DATEFIRST 1` para que lunes sea día 1. Aplica tarifa especial Lun-Jue cuando PriceMultiplier ≤ 1. Fallback a BasePrice si no hay tarifa configurada. Incluye cálculo de personas extra.

### SP5: `sp_ValidateOverbooking` (auxiliar)

Verifica si una habitación tiene reservas solapadas.

| Parámetro               | Tipo     | Dirección |
| ----------------------- | -------- | --------- |
| `@RoomId`               | INT      | Entrada   |
| `@CheckIn`              | DATE     | Entrada   |
| `@CheckOut`             | DATE     | Entrada   |
| `@ExcludeReservationId` | INT NULL | Entrada   |
| `@IsOverbooked`         | BIT      | Salida    |

### SP6: `sp_GetRatesByCriteria` (auxiliar)

Consulta avanzada de tarifas cruzando múltiples filtros opcionales. Calcula `EffectivePricePerNight` incluyendo multiplicador de temporada y extras por persona.

| Parámetro          | Tipo      | Dirección |
| ------------------ | --------- | --------- |
| `@SiteId`          | INT NULL  | Entrada   |
| `@AccommodationId` | INT NULL  | Entrada   |
| `@SeasonId`        | INT NULL  | Entrada   |
| `@Guests`          | INT NULL  | Entrada   |
| `@CheckIn`         | DATE NULL | Entrada   |
| `@CheckOut`        | DATE NULL | Entrada   |
| Resultado          | Tabla     | Salida    |

### Seguridad en SPs

- Todos los parámetros son **tipados** (DATE, INT, DECIMAL), lo que previene SQL Injection.
- Se usa `SET NOCOUNT ON` para evitar mensajes innecesarios.
- Los SPs se ejecutan desde EF Core usando `ExecuteSqlRawAsync` con parámetros `SqlParameter`, nunca con concatenación de cadenas.

---

## 13. Servicio de Email (SMTP)

### Arquitectura

```
IEmailService (interfaz)
  └── EmailService (MailKit) → Conexión SMTP

IEmailTemplateService (interfaz)
  └── EmailTemplateService → Plantillas HTML

IEmailSender<ApplicationUser> (Identity)
  └── IdentityEmailSender → Detecta tipo + aplica plantilla + envía
```

### Configuración

Las credenciales SMTP se configuran mediante variables de entorno (en Docker) o `appsettings.json` (local):

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.ejemplo.com",
    "SmtpPort": 587,
    "SenderName": "Reservas XYZ",
    "SenderEmail": "correo@ejemplo.com",
    "Password": "contraseña-aplicación"
  }
}
```

### Protocolos soportados

| Puerto | Protocolo | Uso                        |
| ------ | --------- | -------------------------- |
| 465    | SSL       | Conexión directa con SSL   |
| 587    | STARTTLS  | Conexión con upgrade a TLS |
| Otro   | Auto      | Detección automática       |

### Tipos de email

| Tipo                           | Plantilla               | Descripción                        |
| ------------------------------ | ----------------------- | ---------------------------------- |
| Confirmación de email          | `ConfirmEmailTemplate`  | Enlace para verificar cuenta       |
| Restablecimiento de contraseña | `ResetPasswordTemplate` | Enlace para crear nueva contraseña |
| Notificación de reserva        | `ReservationTemplate`   | Confirmación de reserva creada     |

### Seguridad SMTP

- Las credenciales SMTP **no están en el código fuente** ni en el repositorio Git.
- En desarrollo: se usa `appsettings.Development.json` (excluido de Git vía `.gitignore`).
- En producción (Docker): se inyectan como variables de entorno desde `.env`.
- Los enlaces en emails se codifican con `HtmlEncoder.Default.Encode()` para prevenir XSS.

---

## 14. Docker y Contenedores

### Dockerfile (Multi-stage build)

```
Etapa 1: Build (SDK 8.0)
  ├── Restore de paquetes NuGet
  ├── Instalación de Node.js 20 (NodeSource)
  ├── npm ci + npm run css:build (Tailwind CSS)
  └── dotnet publish -c Release

Etapa 2: Runtime (aspnet:8.0)
  ├── Copia de artefactos publicados
  ├── Puerto 5000
  └── Entrypoint: dotnet ReservasXYZ.Web.dll
```

### Docker Compose

| Servicio  | Imagen                   | Puertos | Volúmenes                            | Dependencias        |
| --------- | ------------------------ | ------- | ------------------------------------ | ------------------- |
| sqlserver | mssql/server:2022-latest | 1433    | sqldata                              | —                   |
| web       | Build local              | 5000    | —                                    | sqlserver (healthy) |
| nginx     | nginx:alpine             | 80, 443 | nginx.conf, certbot-etc, certbot-var | web                 |
| certbot   | certbot/certbot          | —       | certbot-etc, certbot-var             | nginx               |

### Health checks

SQL Server tiene un healthcheck con:

- Dual path fallback (`/opt/mssql-tools18/` o `/opt/mssql-tools/`).
- Intervalo: 15s, timeout: 10s, retries: 15, start_period: 60s.

### Restart policy

Todos los servicios usan `restart: unless-stopped` para reinicio automático.

---

## 15. Despliegue en VPS

### Infraestructura

| Componente        | Detalle                      |
| ----------------- | ---------------------------- |
| Proveedor         | Google Cloud Platform        |
| Tipo de instancia | e2-medium (2 vCPU, 4 GB RAM) |
| Sistema operativo | Ubuntu 22.04 LTS             |
| Almacenamiento    | 40 GB SSD                    |
| IP estática       | Asignada                     |
| Dominio           | Gestionado en Hostinger      |
| SSL               | Let's Encrypt (Certbot)      |

### Proceso de despliegue

1. Clonar repositorio en el VPS.
2. Copiar `.env.example` a `.env` y configurar credenciales.
3. Configurar `deploy/nginx.conf` con el dominio.
4. Ejecutar `docker compose up -d --build`.
5. Obtener certificado SSL con Certbot.
6. Activar bloque HTTPS en nginx.conf.
7. Reiniciar nginx.

### Seguridad del VPS

- Acceso SSH por clave ED25519 (sin contraseña).
- Firewall: solo puertos 22, 80, 443.
- Usuario `deploy` no-root.
- Certificados SSL renovados automáticamente cada 6 horas.
- HTTP redirige a HTTPS (301).

---

## 16. Seguridad

### Resumen de controles implementados

| Vulnerabilidad OWASP               | Control                   | Implementación                                                        |
| ---------------------------------- | ------------------------- | --------------------------------------------------------------------- |
| **A01: Broken Access Control**     | Autorización por roles    | `[Authorize(Roles = "Admin")]`, verificación de propiedad de reservas |
| **A02: Cryptographic Failures**    | Hashing de contraseñas    | ASP.NET Identity (PBKDF2 con salt)                                    |
| **A03: Injection**                 | Parámetros tipados        | EF Core parametrizado, SPs con SqlParameter                           |
| **A04: Insecure Design**           | Transacción SERIALIZABLE  | Prevención de overbooking en reservas concurrentes                    |
| **A05: Security Misconfiguration** | Variables de entorno      | Secrets en .env (no en código), .gitignore configurado                |
| **A06: Vulnerable Components**     | Dependencias actualizadas | NuGet packages 8.0.25                                                 |
| **A07: Auth Failures**             | Bloqueo de cuenta         | 5 intentos → 15 min lockout                                           |
| **A08: Data Integrity**            | Anti-forgery tokens       | `[ValidateAntiForgeryToken]` en todos los POST                        |
| **A09: Logging Failures**          | Logging estructurado      | Microsoft.Extensions.Logging                                          |
| **A10: SSRF**                      | No aplica                 | No hay llamadas HTTP a URLs externas definidas por usuario            |

### Gestión de secretos

| Secreto                        | Ubicación                                                | Expuesto en Git         |
| ------------------------------ | -------------------------------------------------------- | ----------------------- |
| Contraseña SMTP                | `.env` (Docker) / `appsettings.Development.json` (local) | ❌ No                   |
| Password SA SQL Server         | `.env` (Docker)                                          | ❌ No                   |
| Connection string producción   | Variable de entorno Docker                               | ❌ No                   |
| `appsettings.Development.json` | Local solamente                                          | ❌ No (en `.gitignore`) |
| `appsettings.json`             | Git                                                      | ✅ Solo placeholders    |
| `appsettings.Production.json`  | Git                                                      | ✅ Solo placeholders    |
| `.env.example`                 | Git                                                      | ✅ Solo placeholders    |

---

## 17. Tecnologías y Librerías

| Librería                                            | Versión | Propósito                           |
| --------------------------------------------------- | ------- | ----------------------------------- |
| Microsoft.AspNetCore (SDK)                          | 8.0     | Framework web MVC                   |
| Microsoft.EntityFrameworkCore                       | 8.0.25  | ORM                                 |
| Microsoft.EntityFrameworkCore.SqlServer             | 8.0.25  | Proveedor SQL Server                |
| Microsoft.AspNetCore.Identity                       | 8.0.25  | Autenticación y autorización        |
| Microsoft.AspNetCore.Identity.UI                    | 8.0.25  | Páginas de Identity predeterminadas |
| AutoMapper                                          | 13.x    | Mapeo entidades ↔ DTOs              |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 13.x    | Integración con DI                  |
| FluentValidation                                    | 11.x    | Validación declarativa              |
| FluentValidation.AspNetCore                         | 11.x    | Integración con ASP.NET Core        |
| MailKit                                             | 4.x     | Cliente SMTP                        |
| MimeKit                                             | 4.x     | Construcción de mensajes email      |
| Tailwind CSS                                        | 4.3.0   | Framework CSS utility-first         |
| @tailwindcss/cli                                    | 4.x     | CLI para compilación de CSS         |

---

## 18. Instrucciones de Ejecución

### Opción A: Ejecución local

**Requisitos**: .NET SDK 8.0, SQL Server (LocalDB o instancia), Node.js 20+.

```powershell
# 1. Clonar repositorio
git clone https://github.com/juan-campo-dev/reservas-xyz.git
cd reservas-xyz

# 2. Restaurar paquetes
dotnet restore ReservasXYZ.sln

# 3. Instalar dependencias Node.js
cd ReservasXYZ.Web
npm install

# 4. Compilar CSS
npm run css:build

# 5. Volver a raíz
cd ..

# 6. Ejecutar
dotnet run --project ReservasXYZ.Web --launch-profile http
```

La aplicación estará en **http://localhost:5263**.

La base de datos se crea automáticamente al iniciar (migraciones + seed + SPs + catálogo FODUN).

### Opción B: Docker Compose

**Requisitos**: Docker Engine 20.10+, Docker Compose v2+.

```bash
# 1. Copiar variables de entorno
cp .env.example .env
# Editar .env con credenciales reales

# 2. Construir y levantar
docker compose up -d --build

# 3. Verificar servicios
docker compose ps
```

La aplicación estará en **http://localhost** (puerto 80).

### Opción C: Restaurar BD desde script SQL

Si se prefiere crear la base de datos manualmente:

```sql
-- 1. Ejecutar database/Database.sql (crea estructura + SPs)
-- 2. Ejecutar database/SeedCatalog.sql (carga catálogo FODUN)
```

---

## 19. Capturas de Pantalla

_Nota: Las capturas del sistema en funcionamiento se pueden generar accediendo a:_

- **Landing page**: http://localhost:5263
- **Login**: http://localhost:5263/Identity/Account/Login
- **Portal de asociados**: http://localhost:5263 (autenticado)
- **Búsqueda de disponibilidad**: Sección "Consultar Disponibilidad" en el portal
- **Panel admin**: http://localhost:5263 (login como admin@hotel.com)
- **Dashboard**: Métricas de reservas e ingresos
- **CRUD Sedes**: Listado y formulario de sedes
- **Producción**: https://reservas-xyz.app-dev.icu

---

## 20. Conclusiones

### Cumplimiento de requisitos

El sistema implementa la totalidad de los requisitos solicitados en la prueba técnica:

1. ✅ **Estructura de BD relacional en SQL Server**: 9 tablas + Identity + índices + FK.
2. ✅ **SP de disponibilidad por fechas**: `sp_GetAvailableRoomsByDates`.
3. ✅ **SP de disponibilidad por fechas + personas**: `sp_GetAvailableRoomsByDatesAndGuests`.
4. ✅ **SP de tarifas por sitio/temporada/personas/alojamiento**: `sp_GetRoomRates` + `sp_GetRatesByCriteria`.
5. ✅ **SP de cálculo de tarifa total**: `sp_CalculateTotalRate` (motor iterativo día por día).
6. ✅ **Formularios web**: Razor Views para todas las funcionalidades.
7. ✅ **Registro de usuarios**: Con nombre, apellido, documento.
8. ✅ **Login funcional**: Solo autenticados acceden al sistema.
9. ✅ **Recuperación de contraseña SMTP**: Email con enlace de restablecimiento.
10. ✅ **CRUD de formularios**: Sedes, Alojamientos, Habitaciones, Temporadas, Tarifas, Reservas.
11. ✅ **Consulta de disponibilidad**: Por fechas y personas usando SPs.
12. ✅ **Guardar reservas**: Con detalle por habitación y cálculo de precio.
13. ✅ **Modelo MVC**: ASP.NET Core MVC con Razor Views.
14. ✅ **Programación en capas**: Clean Architecture (4 capas).
15. ✅ **Entity Framework**: EF Core 8.0 con Fluent API.
16. ✅ **Identity**: Autenticación completa con roles y bloqueo.
17. ✅ **Documento técnico**: Este documento.

### Valor agregado (más allá de los requisitos)

- Motor de tarificación FODUN con reglas de negocio complejas (Lun-Jue, temporadas, extras).
- Prevención de overbooking con transacciones SERIALIZABLE.
- 6 SPs (4 requeridos + 2 auxiliares).
- Despliegue en producción con Docker + VPS + SSL.
- Plantillas de email HTML profesionales.
- Interfaz en español colombiano (es-CO).
- Soft delete en todas las entidades.
- Fallback LINQ en repositorios.
- Validación con FluentValidation + mensajes en español.
- Dashboard administrativo con métricas.

### Decisiones de diseño

La arquitectura fue diseñada priorizando:

- **Robustez**: Transacciones serializables, validación en múltiples capas.
- **Mantenibilidad**: Clean Architecture, DTOs, Repository Pattern.
- **Seguridad**: Identity, CSRF, parametrización, gestión de secretos.
- **Operabilidad**: Docker, health checks, logs, restart policies.

---

_Documento generado como parte de la entrega de la prueba técnica para el cargo de Analista Desarrollador .NET — Fondo de Empleados XYZ._
