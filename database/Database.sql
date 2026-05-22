-- =====================================================================
-- ReservasXYZ - Script completo de base de datos (SQL Server)
-- =====================================================================
-- Genera la base de datos desde cero con:
--   1. CREATE DATABASE (opcional, descomentar para uso manual)
--   2. Tablas de ASP.NET Identity
--   3. Tablas del dominio (Sites, Accommodations, Rooms, Rates, Seasons,
--      Reservations, ReservationDetails, Favorites)
--   4. Indices y claves foraneas
--   5. Stored Procedures (4 requeridos + 1 auxiliar de overbooking)
--   6. Datos iniciales (catalogo de sedes, alojamientos, habitaciones y tarifas)
--
-- Cumple con los 4 SPs exigidos en la prueba tecnica:
--   - sp_GetAvailableRoomsByDates           (disponibilidad por fechas)
--   - sp_GetAvailableRoomsByDatesAndGuests  (disponibilidad por fechas + personas)
--   - sp_GetRoomRates                        (tarifas por habitacion / temporada)
--   - sp_CalculateTotalRate                  (calculo total por rango + sede + temporada)
-- Plus: sp_ValidateOverbooking (utilitario interno).
--
-- El script es idempotente: se puede ejecutar varias veces sin error.
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. Base de datos (opcional)
-- ---------------------------------------------------------------------
-- IF DB_ID(N'ReservasXYZDb') IS NULL
--     CREATE DATABASE ReservasXYZDb;
-- GO
-- USE ReservasXYZDb;
-- GO

-- ---------------------------------------------------------------------
-- 2. ASP.NET Identity
-- ---------------------------------------------------------------------
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

IF OBJECT_ID(N'[AspNetRoles]') IS NULL
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[AspNetUsers]') IS NULL
CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [DocumentNumber] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[AspNetRoleClaims]') IS NULL
CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserClaims]') IS NULL
CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserLogins]') IS NULL
CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserRoles]') IS NULL
CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserTokens]') IS NULL
CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetRoleClaims_RoleId')
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'RoleNameIndex')
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserClaims_UserId')
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserLogins_UserId')
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserRoles_RoleId')
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'EmailIndex')
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UserNameIndex')
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

-- ---------------------------------------------------------------------
-- 3. Dominio: Sites -> Accommodations -> Rooms -> Reservations/Rates
-- ---------------------------------------------------------------------
IF OBJECT_ID(N'[Sites]') IS NULL
CREATE TABLE [Sites] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [City] nvarchar(100) NOT NULL,
    [Country] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [Email] nvarchar(200) NULL,
    [Description] nvarchar(max) NULL,
    [ImageUrl] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Sites] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[Accommodations]') IS NULL
CREATE TABLE [Accommodations] (
    [Id] int NOT NULL IDENTITY,
    [SiteId] int NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Type] nvarchar(50) NOT NULL,
    [TotalRooms] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Accommodations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Accommodations_Sites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites] ([Id]) ON DELETE NO ACTION
);
GO

IF OBJECT_ID(N'[Rooms]') IS NULL
CREATE TABLE [Rooms] (
    [Id] int NOT NULL IDENTITY,
    [AccommodationId] int NOT NULL,
    [RoomNumber] nvarchar(20) NOT NULL,
    [Type] int NOT NULL,
    [MaxGuests] int NOT NULL,
    [BasePrice] decimal(18,2) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Amenities] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rooms_Accommodations_AccommodationId] FOREIGN KEY ([AccommodationId]) REFERENCES [Accommodations] ([Id]) ON DELETE NO ACTION
);
GO

IF OBJECT_ID(N'[Seasons]') IS NULL
CREATE TABLE [Seasons] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [PriceMultiplier] decimal(5,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Seasons] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[Rates]') IS NULL
CREATE TABLE [Rates] (
    [Id] int NOT NULL IDENTITY,
    [RoomId] int NOT NULL,
    [SeasonId] int NOT NULL,
    [PricePerNight] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Rates] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rates_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Rates_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons] ([Id]) ON DELETE NO ACTION
);
GO

IF OBJECT_ID(N'[Reservations]') IS NULL
CREATE TABLE [Reservations] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [GuestName] nvarchar(200) NOT NULL,
    [GuestEmail] nvarchar(200) NOT NULL,
    [GuestPhone] nvarchar(20) NULL,
    [CheckIn] datetime2 NOT NULL,
    [CheckOut] datetime2 NOT NULL,
    [TotalGuests] int NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [Notes] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reservations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

IF OBJECT_ID(N'[ReservationDetails]') IS NULL
CREATE TABLE [ReservationDetails] (
    [Id] int NOT NULL IDENTITY,
    [ReservationId] int NOT NULL,
    [RoomId] int NOT NULL,
    [PricePerNight] decimal(18,2) NOT NULL,
    [Nights] int NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ReservationDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReservationDetails_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReservationDetails_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE NO ACTION
);
GO

-- Plus opcional: Favoritos del huesped
IF OBJECT_ID(N'[Favorites]') IS NULL
CREATE TABLE [Favorites] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [RoomId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_Favorites_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Favorites] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Favorites_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Favorites_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE CASCADE
);
GO

-- Indices del dominio
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Sites_Name')
    CREATE INDEX [IX_Sites_Name] ON [Sites] ([Name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Accommodations_SiteId')
    CREATE INDEX [IX_Accommodations_SiteId] ON [Accommodations] ([SiteId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Rooms_AccommodationId_RoomNumber')
    CREATE UNIQUE INDEX [IX_Rooms_AccommodationId_RoomNumber] ON [Rooms] ([AccommodationId], [RoomNumber]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Seasons_StartDate_EndDate')
    CREATE INDEX [IX_Seasons_StartDate_EndDate] ON [Seasons] ([StartDate], [EndDate]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Rates_RoomId_SeasonId')
    CREATE UNIQUE INDEX [IX_Rates_RoomId_SeasonId] ON [Rates] ([RoomId], [SeasonId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Rates_SeasonId')
    CREATE INDEX [IX_Rates_SeasonId] ON [Rates] ([SeasonId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservations_UserId')
    CREATE INDEX [IX_Reservations_UserId] ON [Reservations] ([UserId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservations_CheckIn_CheckOut')
    CREATE INDEX [IX_Reservations_CheckIn_CheckOut] ON [Reservations] ([CheckIn], [CheckOut]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservations_Status')
    CREATE INDEX [IX_Reservations_Status] ON [Reservations] ([Status]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReservationDetails_ReservationId')
    CREATE INDEX [IX_ReservationDetails_ReservationId] ON [ReservationDetails] ([ReservationId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReservationDetails_RoomId_ReservationId')
    CREATE INDEX [IX_ReservationDetails_RoomId_ReservationId] ON [ReservationDetails] ([RoomId], [ReservationId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Favorites_RoomId')
    CREATE INDEX [IX_Favorites_RoomId] ON [Favorites] ([RoomId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Favorites_UserId_RoomId')
    CREATE UNIQUE INDEX [IX_Favorites_UserId_RoomId] ON [Favorites] ([UserId], [RoomId]);
GO

-- Registrar migraciones EF Core (para que EF no intente re-aplicarlas)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518131427_InitialCreate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260518131427_InitialCreate', N'8.0.25');
GO
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518141553_AddApplicationUserDocumentNumber')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260518141553_AddApplicationUserDocumentNumber', N'8.0.25');
GO
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260519162751_AddSiteImageUrl')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260519162751_AddSiteImageUrl', N'8.0.25');
GO
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260521204901_AddRoomFavorites')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260521204901_AddRoomFavorites', N'8.0.25');
GO

-- =====================================================================
-- 4. Stored Procedures
-- =====================================================================

-- SP1: Buscar habitaciones disponibles por rango de fechas
CREATE OR ALTER PROCEDURE sp_GetAvailableRoomsByDates
    @CheckIn DATE,
    @CheckOut DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.RoomNumber, r.Type, r.MaxGuests, r.BasePrice, r.Description,
           a.Name AS AccommodationName, s.Name AS SiteName
    FROM Rooms r
    INNER JOIN Accommodations a ON r.AccommodationId = a.Id
    INNER JOIN Sites s ON a.SiteId = s.Id
    WHERE r.IsActive = 1
      AND r.Id NOT IN (
          SELECT rd.RoomId
          FROM ReservationDetails rd
          INNER JOIN Reservations res ON rd.ReservationId = res.Id
          WHERE res.Status != 5 -- Not Cancelled
            AND res.CheckIn < @CheckOut
            AND res.CheckOut > @CheckIn
      )
    ORDER BY r.BasePrice;
END
GO

-- SP2: Buscar habitaciones disponibles por fechas + numero de personas
CREATE OR ALTER PROCEDURE sp_GetAvailableRoomsByDatesAndGuests
    @CheckIn DATE,
    @CheckOut DATE,
    @Guests INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.RoomNumber, r.Type, r.MaxGuests, r.BasePrice, r.Description,
           a.Name AS AccommodationName, s.Name AS SiteName
    FROM Rooms r
    INNER JOIN Accommodations a ON r.AccommodationId = a.Id
    INNER JOIN Sites s ON a.SiteId = s.Id
    WHERE r.IsActive = 1
      AND r.MaxGuests >= @Guests
      AND r.Id NOT IN (
          SELECT rd.RoomId
          FROM ReservationDetails rd
          INNER JOIN Reservations res ON rd.ReservationId = res.Id
          WHERE res.Status != 5
            AND res.CheckIn < @CheckOut
            AND res.CheckOut > @CheckIn
      )
    ORDER BY r.BasePrice;
END
GO

-- SP3: Consultar tarifas de una habitacion por temporada
CREATE OR ALTER PROCEDURE sp_GetRoomRates
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT rt.Id, rt.PricePerNight, rt.IsActive,
           s.Name AS SeasonName, s.StartDate, s.EndDate, s.PriceMultiplier,
           r.RoomNumber, r.BasePrice
    FROM Rates rt
    INNER JOIN Seasons s ON rt.SeasonId = s.Id
    INNER JOIN Rooms r ON rt.RoomId = r.Id
    WHERE rt.RoomId = @RoomId
    ORDER BY s.StartDate;
END
GO

-- SP4: Calcular tarifa total para una habitacion en un rango de fechas
CREATE OR ALTER PROCEDURE sp_CalculateTotalRate
    @RoomId INT,
    @CheckIn DATE,
    @CheckOut DATE,
    @TotalRate DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentDate DATE = @CheckIn;
    DECLARE @DailyRate DECIMAL(18,2);
    DECLARE @BasePrice DECIMAL(18,2);
    SET @TotalRate = 0;

    SELECT @BasePrice = BasePrice FROM Rooms WHERE Id = @RoomId;

    WHILE @CurrentDate < @CheckOut
    BEGIN
        SELECT TOP 1 @DailyRate = rt.PricePerNight
        FROM Rates rt
        INNER JOIN Seasons s ON rt.SeasonId = s.Id
        WHERE rt.RoomId = @RoomId
          AND rt.IsActive = 1
          AND s.IsActive = 1
          AND s.StartDate <= @CurrentDate
          AND s.EndDate >= @CurrentDate;

        IF @DailyRate IS NULL
            SET @DailyRate = @BasePrice;

        SET @TotalRate = @TotalRate + @DailyRate;
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
        SET @DailyRate = NULL;
    END
END
GO

-- SP5 (plus): Validar overbooking
CREATE OR ALTER PROCEDURE sp_ValidateOverbooking
    @RoomId INT,
    @CheckIn DATE,
    @CheckOut DATE,
    @ExcludeReservationId INT = NULL,
    @IsOverbooked BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM ReservationDetails rd
        INNER JOIN Reservations res ON rd.ReservationId = res.Id
        WHERE rd.RoomId = @RoomId
          AND res.Status != 5
          AND res.CheckIn < @CheckOut
          AND res.CheckOut > @CheckIn
          AND (@ExcludeReservationId IS NULL OR rd.ReservationId != @ExcludeReservationId)
    )
        SET @IsOverbooked = 1;
    ELSE
        SET @IsOverbooked = 0;
END
GO

-- =====================================================================
-- 5. Datos iniciales (catalogo)
-- =====================================================================
-- Para insertar/actualizar el catalogo de sedes, alojamientos, habitaciones
-- y tarifas, ejecutar el script database\SeedCatalog.sql (incluido en el
-- repositorio). Ese script es idempotente y se ejecuta automaticamente
-- al arrancar la aplicacion desde ReservasXYZ.Web.
-- Tambien puede invocarse manualmente con:
--   sqlcmd -S localhost -d ReservasXYZDb -E -i database\SeedCatalog.sql
-- =====================================================================

PRINT N'Database.sql ejecutado correctamente. Ejecuta SeedCatalog.sql para poblar el catalogo.';
