-- =============================================
-- SEED CATALOG - ReservasXYZ
-- Datos iniciales del catalogo (Seasons, Sites, Accommodations, Rooms, Rates).
-- Idempotente: se puede ejecutar varias veces sin duplicar.
-- =============================================

SET NOCOUNT ON;

-- ---------- Seasons ----------
IF NOT EXISTS (SELECT 1 FROM Seasons WHERE Name = N'Baja Temporada 2026')
    INSERT INTO Seasons (Name, StartDate, EndDate, PriceMultiplier, IsActive)
    VALUES (N'Baja Temporada 2026', '2026-01-01', '2026-05-31', 1.00, 1);

IF NOT EXISTS (SELECT 1 FROM Seasons WHERE Name = N'Alta Temporada 2026')
    INSERT INTO Seasons (Name, StartDate, EndDate, PriceMultiplier, IsActive)
    VALUES (N'Alta Temporada 2026', '2026-06-01', '2026-08-31', 1.00, 1);

IF NOT EXISTS (SELECT 1 FROM Seasons WHERE Name = N'Tarifa Especial 2026')
    INSERT INTO Seasons (Name, StartDate, EndDate, PriceMultiplier, IsActive)
    VALUES (N'Tarifa Especial 2026', '2026-09-01', '2026-11-30', 1.00, 1);

-- Asegura multiplicadores correctos si las temporadas ya existian.
-- Los Rates almacenan el precio FINAL por temporada (segun PDF FODUN), por lo que
-- el multiplicador de Season debe ser 1.0 para evitar duplicar el ajuste.
UPDATE Seasons SET PriceMultiplier = 1.00 WHERE Name = N'Tarifa Especial 2026' AND PriceMultiplier <> 1.00;
UPDATE Seasons SET PriceMultiplier = 1.00 WHERE Name = N'Alta Temporada 2026'   AND PriceMultiplier <> 1.00;

-- ---------- Sites ----------
DECLARE @Sites TABLE (
    Name NVARCHAR(200),
    City NVARCHAR(100),
    Address NVARCHAR(500),
    Description NVARCHAR(MAX),
    ImageUrl NVARCHAR(500)
);

INSERT INTO @Sites VALUES
(N'Villeta', N'Villeta', N'Por definir', N'Sede recreativa con ocho habitaciones, cada una con cama doble, camarote, bano, nevera, televisor y terraza cubierta. Capacidad total hasta 32 personas.', N'/images/sites/villeta.jpg'),
(N'El Placer - Fusagasuga', N'Fusagasuga', N'Por definir', N'Sede recreativa con alojamientos y bloque de cabanas. Capacidad total hasta 34 personas.', N'/images/sites/fusagasuga.jpg'),
(N'Gonzalo Morante - Chinchina', N'Chinchina', N'Por definir', N'Sede recreativa con alojamientos y cabanas. Capacidad total hasta 30 personas.', N'/images/sites/chinchina.jpg'),
(N'Tablones - Palmira', N'Palmira', N'Por definir', N'Sede recreativa con cuatro alojamientos. Capacidad total hasta 24 personas.', N'/images/sites/palmira.jpg'),
(N'Manguruma - Santa Fe de Antioquia', N'Santa Fe de Antioquia', N'Por definir', N'Sede recreativa con alojamientos tradicionales y bloque nuevo. Capacidad total hasta 46 personas.', N'/images/sites/santa-fe-de-antioquia.jpg'),
(N'Federman - Bogota', N'Bogota', N'Por definir', N'Sede con zona humeda, gimnasio, billar, salas sociales y cuatro habitaciones para alojamiento.', N'/images/sites/bogota.jpg'),
(N'Suramericana - Medellin', N'Medellin', N'Por definir', N'Apartamento Suramericana con cinco habitaciones individuales.', N'/images/sites/medellin.jpg'),
(N'El Rodadero - Santa Marta', N'Santa Marta', N'Por definir', N'Apartamentos en El Rodadero con sala comedor, cocina, banos, habitaciones y parqueadero.', N'/images/sites/santa-marta.jpg');

INSERT INTO Sites (Name, Address, City, Country, Description, ImageUrl, IsActive, CreatedAt)
SELECT s.Name, s.Address, s.City, N'Colombia', s.Description, s.ImageUrl, 1, SYSUTCDATETIME()
FROM @Sites s
WHERE NOT EXISTS (SELECT 1 FROM Sites x WHERE x.Name = s.Name);

-- ---------- Accommodations ----------
DECLARE @Acc TABLE (
    SiteName NVARCHAR(200),
    Name NVARCHAR(200),
    Type NVARCHAR(50),
    Description NVARCHAR(MAX),
    TotalRooms INT
);

INSERT INTO @Acc VALUES
(N'Villeta', N'Habitaciones Villeta', N'Habitaciones', N'Habitaciones Villeta', 8),
(N'El Placer - Fusagasuga', N'Alojamientos El Placer', N'Alojamientos', N'Cuatro alojamientos familiares independientes con bano y televisor.', 4),
(N'El Placer - Fusagasuga', N'Bloque de Cabanas', N'Cabanas', N'Bloque de Cabanas', 4),
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'Alojamientos', N'Seis alojamientos entre cabanas y unidades familiares.', 6),
(N'Tablones - Palmira', N'Alojamientos Tablones', N'Alojamientos', N'Cuatro alojamientos familiares con cocineta y bano privado.', 4),
(N'Manguruma - Santa Fe de Antioquia', N'Alojamientos Manguruma', N'Alojamientos', N'Tres alojamientos familiares con terraza y bano privado.', 3),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'Cabanas', N'Bloque Nuevo', 8),
(N'Federman - Bogota', N'Habitaciones Federman', N'Habitaciones', N'Habitaciones Federman', 4),
(N'Suramericana - Medellin', N'Habitaciones Suramericana', N'Habitaciones', N'Cinco habitaciones reservables dentro del apartamento Suramericana.', 5),
(N'El Rodadero - Santa Marta', N'Apartamentos El Rodadero', N'Apartamentos', N'Tres apartamentos completos. Cada uno se reserva como una sola unidad.', 3);

INSERT INTO Accommodations (SiteId, Name, Description, Type, TotalRooms, IsActive, CreatedAt)
SELECT s.Id, a.Name, a.Description, a.Type, a.TotalRooms, 1, SYSUTCDATETIME()
FROM @Acc a
INNER JOIN Sites s ON s.Name = a.SiteName
WHERE NOT EXISTS (
    SELECT 1 FROM Accommodations x WHERE x.SiteId = s.Id AND x.Name = a.Name
);

-- ---------- Rooms ----------
-- RoomType enum: Single=1, Double=2, Twin=3, Suite=4, Deluxe=5, Family=6, Cabin=7, Apartment=8
DECLARE @Rooms TABLE (
    SiteName NVARCHAR(200),
    AccommodationName NVARCHAR(200),
    RoomNumber NVARCHAR(20),
    Type INT,
    MaxGuests INT,
    BasePrice DECIMAL(18,2),
    Description NVARCHAR(MAX),
    Amenities NVARCHAR(MAX)
);

INSERT INTO @Rooms VALUES
-- Villeta
(N'Villeta', N'Habitaciones Villeta', N'V01', 6, 4, 70000, N'Unidad 1 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V02', 6, 4, 70000, N'Unidad 2 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V03', 6, 4, 70000, N'Unidad 3 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V04', 6, 4, 70000, N'Unidad 4 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V05', 6, 4, 70000, N'Unidad 5 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V06', 6, 4, 70000, N'Unidad 6 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V07', 6, 4, 70000, N'Unidad 7 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
(N'Villeta', N'Habitaciones Villeta', N'V08', 6, 4, 70000, N'Unidad 8 de Habitaciones Villeta.', N'Televisor, bano, hospedaje familiar'),
-- El Placer
(N'El Placer - Fusagasuga', N'Alojamientos El Placer', N'1', 6, 4, 90000, N'Dos habitaciones, bano y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Alojamientos El Placer', N'2', 6, 6, 90000, N'Dos habitaciones, bano y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Alojamientos El Placer', N'3', 6, 4, 70000, N'Una habitacion con cama doble, dos camas sencillas, bano y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Alojamientos El Placer', N'4', 6, 4, 90000, N'Dos habitaciones, bano y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Bloque de Cabanas', N'F-C01', 7, 4, 90000, N'Unidad 1 de Bloque de Cabanas.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Bloque de Cabanas', N'F-C02', 7, 4, 90000, N'Unidad 2 de Bloque de Cabanas.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Bloque de Cabanas', N'F-C03', 7, 4, 90000, N'Unidad 3 de Bloque de Cabanas.', N'Televisor, bano, hospedaje familiar'),
(N'El Placer - Fusagasuga', N'Bloque de Cabanas', N'F-C04', 7, 4, 90000, N'Unidad 4 de Bloque de Cabanas.', N'Televisor, bano, hospedaje familiar'),
-- Gonzalo Morante
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'1', 6, 7, 90000, N'Cocineta, bano, televisor y dos habitaciones.', N'Televisor, bano, hospedaje familiar'),
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'2', 6, 7, 90000, N'Cocineta, bano, televisor y dos habitaciones.', N'Televisor, bano, hospedaje familiar'),
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'3-A', 7, 6, 90000, N'Cabana tipo A: cocineta, dos banos, sala comedor, televisor y dos habitaciones.', N'Televisor, bano, hospedaje familiar'),
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'4', 2, 3, 70000, N'Cocineta, bano, televisor y una habitacion.', N'Televisor, bano, hospedaje familiar'),
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'5-B', 7, 3, 70000, N'Cabana tipo B: cocineta, bano, sala con sofa, televisor y una habitacion.', N'Televisor, bano, hospedaje familiar'),
(N'Gonzalo Morante - Chinchina', N'Alojamientos Gonzalo Morante', N'6-B', 7, 3, 70000, N'Cabana tipo B: cocineta, bano, sala con sofa, televisor y una habitacion.', N'Televisor, bano, hospedaje familiar'),
-- Tablones
(N'Tablones - Palmira', N'Alojamientos Tablones', N'1', 6, 4, 70000, N'Una habitacion con cama doble y camarote, televisor, bano y cocineta.', N'Televisor, bano, hospedaje familiar'),
(N'Tablones - Palmira', N'Alojamientos Tablones', N'2', 6, 4, 70000, N'Una habitacion con cama doble y camarote, televisor, bano y cocineta.', N'Televisor, bano, hospedaje familiar'),
(N'Tablones - Palmira', N'Alojamientos Tablones', N'3', 6, 8, 90000, N'Dos habitaciones, sala de estar, televisor, bano y cocineta.', N'Televisor, bano, hospedaje familiar'),
(N'Tablones - Palmira', N'Alojamientos Tablones', N'4', 6, 8, 90000, N'Dos habitaciones, sala de estar, televisor, bano y cocineta.', N'Televisor, bano, hospedaje familiar'),
-- Manguruma
(N'Manguruma - Santa Fe de Antioquia', N'Alojamientos Manguruma', N'1', 6, 4, 70000, N'Cama doble, camarote, bano, terraza y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Alojamientos Manguruma', N'2', 6, 5, 70000, N'Cama doble, camarote, sofa cama, bano, terraza y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Alojamientos Manguruma', N'3', 6, 5, 70000, N'Cama doble, camarote, sofa cama, bano, terraza y televisor.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N01', 7, 4, 90000, N'Unidad 1 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N02', 7, 4, 90000, N'Unidad 2 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N03', 7, 4, 90000, N'Unidad 3 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N04', 7, 4, 90000, N'Unidad 4 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N05', 7, 4, 90000, N'Unidad 5 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N06', 7, 4, 90000, N'Unidad 6 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N07', 7, 4, 90000, N'Unidad 7 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
(N'Manguruma - Santa Fe de Antioquia', N'Bloque Nuevo', N'M-N08', 7, 4, 90000, N'Unidad 8 de Bloque Nuevo.', N'Televisor, bano, hospedaje familiar'),
-- Federman
(N'Federman - Bogota', N'Habitaciones Federman', N'B01', 2, 2, 70000, N'Unidad 1 de Habitaciones Federman.', N'Televisor, bano, hospedaje familiar'),
(N'Federman - Bogota', N'Habitaciones Federman', N'B02', 2, 2, 70000, N'Unidad 2 de Habitaciones Federman.', N'Televisor, bano, hospedaje familiar'),
(N'Federman - Bogota', N'Habitaciones Federman', N'B03', 2, 2, 70000, N'Unidad 3 de Habitaciones Federman.', N'Televisor, bano, hospedaje familiar'),
(N'Federman - Bogota', N'Habitaciones Federman', N'B04', 2, 2, 70000, N'Unidad 4 de Habitaciones Federman.', N'Televisor, bano, hospedaje familiar'),
-- Suramericana
(N'Suramericana - Medellin', N'Habitaciones Suramericana', N'1', 3, 2, 63000, N'Dos camas sencillas y bano privado.', N'Televisor, bano, hospedaje familiar'),
(N'Suramericana - Medellin', N'Habitaciones Suramericana', N'2', 3, 2, 63000, N'Dos camas sencillas.', N'Televisor, bano, hospedaje familiar'),
(N'Suramericana - Medellin', N'Habitaciones Suramericana', N'3', 3, 2, 63000, N'Dos camas sencillas.', N'Televisor, bano, hospedaje familiar'),
(N'Suramericana - Medellin', N'Habitaciones Suramericana', N'4', 3, 2, 63000, N'Dos camas sencillas.', N'Televisor, bano, hospedaje familiar'),
(N'Suramericana - Medellin', N'Habitaciones Suramericana', N'5', 1, 1, 63000, N'Una cama sencilla y bano privado.', N'Televisor, bano, hospedaje familiar'),
-- El Rodadero
(N'El Rodadero - Santa Marta', N'Apartamentos El Rodadero', N'202', 8, 8, 103000, N'Tres habitaciones, dos banos y parqueadero. Capacidad maxima 8 personas.', N'Televisor, bano, hospedaje familiar'),
(N'El Rodadero - Santa Marta', N'Apartamentos El Rodadero', N'301', 8, 6, 89000, N'Dos habitaciones, un bano y parqueadero. Capacidad maxima 6 personas.', N'Televisor, bano, hospedaje familiar'),
(N'El Rodadero - Santa Marta', N'Apartamentos El Rodadero', N'401', 8, 6, 89000, N'Dos habitaciones, un bano y parqueadero. Capacidad maxima 6 personas.', N'Televisor, bano, hospedaje familiar');

INSERT INTO Rooms (AccommodationId, RoomNumber, Type, MaxGuests, BasePrice, Description, Amenities, IsActive, CreatedAt)
SELECT a.Id, r.RoomNumber, r.Type, r.MaxGuests, r.BasePrice, r.Description, r.Amenities, 1, SYSUTCDATETIME()
FROM @Rooms r
INNER JOIN Sites s ON s.Name = r.SiteName
INNER JOIN Accommodations a ON a.SiteId = s.Id AND a.Name = r.AccommodationName
WHERE NOT EXISTS (
    SELECT 1 FROM Rooms x WHERE x.AccommodationId = a.Id AND x.RoomNumber = r.RoomNumber
);

-- ---------- Rates (precio por habitacion x temporada x tipo de tarifa) ----------
-- Motor FODUN: BaseGuests + ExtraPersonPrice + Kind (0=Standard, 1=SpecialWeekday Lun-Jue)
-- Reglas:
--   Suramericana (Medellin), por persona/noche:
--     hab 5: $63.000 fijo (1 pax)
--     resto: BaseGuests=1, $63.000 base, extra $12.000 (2 pax = 75.000)
--   El Rodadero (Santa Marta), por apartamento entero (sin persona adicional):
--     Ap 202 (cap 8): Baja $103.000 / Alta $143.000
--     Ap 301/401 (cap 6): Baja $89.000 / Alta $124.000
--   Sedes recreativas (Villeta, El Placer, Manguruma, Gonzalo Morante, Tablones):
--     Alojamiento 1 hab (BasePrice 70000): Standard 70.000 base 4 pax, extra 16.000;
--                                          Especial Lun-Jue 27.000 base 4 pax, extra 11.000
--     Alojamiento 2 hab (BasePrice 90000): Standard 90.000 base 4 pax, extra 16.000;
--                                          Especial Lun-Jue 37.000 base 4 pax, extra 11.000

DECLARE @LowId  INT = (SELECT Id FROM Seasons WHERE Name = N'Baja Temporada 2026');
DECLARE @HighId INT = (SELECT Id FROM Seasons WHERE Name = N'Alta Temporada 2026');
DECLARE @SpecId INT = (SELECT Id FROM Seasons WHERE Name = N'Tarifa Especial 2026');

-- Limpia Rates existentes para garantizar consistencia con reglas FODUN.
-- Seguro porque ReservationDetails no referencia Rates (referencia RoomId).
DELETE FROM Rates;

;WITH RoomData AS (
    SELECT
        r.Id AS RoomId,
        s.Name AS SiteName,
        r.RoomNumber,
        r.BasePrice
    FROM Rooms r
    INNER JOIN Accommodations a ON r.AccommodationId = a.Id
    INNER JOIN Sites s ON a.SiteId = s.Id
),
RatesComputed AS (
    SELECT
        RoomId, SiteName, RoomNumber, BasePrice,
        CASE
            WHEN SiteName = N'Suramericana - Medellin' THEN 1
            WHEN SiteName = N'El Rodadero - Santa Marta' AND RoomNumber = N'202' THEN 8
            WHEN SiteName = N'El Rodadero - Santa Marta' THEN 6
            ELSE 4
        END AS BaseGuests,
        CASE
            WHEN SiteName = N'Suramericana - Medellin' AND RoomNumber = N'5' THEN 0.00
            WHEN SiteName = N'Suramericana - Medellin' THEN 12000.00
            WHEN SiteName = N'El Rodadero - Santa Marta' THEN 0.00
            ELSE 16000.00
        END AS ExtraStd,
        -- Precio Standard temporada Baja
        CASE
            WHEN SiteName = N'Suramericana - Medellin' THEN 63000.00
            WHEN SiteName = N'El Rodadero - Santa Marta' AND RoomNumber = N'202' THEN 103000.00
            WHEN SiteName = N'El Rodadero - Santa Marta' THEN 89000.00
            ELSE BasePrice
        END AS PriceLow,
        -- Precio Standard temporada Alta
        CASE
            WHEN SiteName = N'Suramericana - Medellin' AND RoomNumber = N'5' THEN 63000.00
            WHEN SiteName = N'Suramericana - Medellin' THEN 63000.00
            WHEN SiteName = N'El Rodadero - Santa Marta' AND RoomNumber = N'202' THEN 143000.00
            WHEN SiteName = N'El Rodadero - Santa Marta' THEN 124000.00
            ELSE BasePrice
        END AS PriceHigh
    FROM RoomData
)
INSERT INTO Rates (RoomId, SeasonId, PricePerNight, BaseGuests, ExtraPersonPrice, Kind, IsActive)
SELECT RoomId, SeasonId, Price, BaseGuests, ExtraPrice, Kind, 1
FROM (
    -- Standard - Baja
    SELECT RoomId, @LowId AS SeasonId, PriceLow AS Price, BaseGuests, ExtraStd AS ExtraPrice, 0 AS Kind
    FROM RatesComputed
    UNION ALL
    -- Standard - Alta
    SELECT RoomId, @HighId AS SeasonId, PriceHigh AS Price, BaseGuests, ExtraStd AS ExtraPrice, 0 AS Kind
    FROM RatesComputed
    UNION ALL
    -- Standard - Especial (igual al Baja para Medellin/Santa Marta; sirve de fallback en sedes recreativas)
    SELECT RoomId, @SpecId AS SeasonId, PriceLow AS Price, BaseGuests, ExtraStd AS ExtraPrice, 0 AS Kind
    FROM RatesComputed
    UNION ALL
    -- SpecialWeekday - Especial (solo sedes recreativas: 1hab=27000 / 2hab=37000)
    SELECT RoomId, @SpecId AS SeasonId,
           CASE WHEN BasePrice = 90000.00 THEN 37000.00 ELSE 27000.00 END AS Price,
           4 AS BaseGuests, 11000.00 AS ExtraPrice, 1 AS Kind
    FROM RatesComputed
    WHERE SiteName NOT IN (N'Suramericana - Medellin', N'El Rodadero - Santa Marta')
) src
WHERE NOT EXISTS (
    SELECT 1 FROM Rates x
    WHERE x.RoomId = src.RoomId AND x.SeasonId = src.SeasonId AND x.Kind = src.Kind
);

PRINT N'SeedCatalog.sql ejecutado correctamente.';
