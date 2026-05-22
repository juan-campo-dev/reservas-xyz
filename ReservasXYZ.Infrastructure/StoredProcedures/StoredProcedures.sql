-- =============================================
-- STORED PROCEDURES - ReservasXYZ
-- SQL Server
-- =============================================

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

-- SP2: Buscar habitaciones disponibles por fechas + número de personas
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

-- SP3: Consultar tarifas de una habitación por temporada
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

-- SP4: Calcular tarifa total para una habitación en un rango de fechas
-- Motor de tarifas FODUN: aplica BaseGuests + ExtraPersonPrice, tarifa especial Lun-Jue
-- y multiplicador de temporada por noche.
CREATE OR ALTER PROCEDURE sp_CalculateTotalRate
    @RoomId INT,
    @CheckIn DATE,
    @CheckOut DATE,
    @TotalGuests INT = 1,
    @TotalRate DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 1; -- Lunes = 1

    DECLARE @CurrentDate    DATE = @CheckIn;
    DECLARE @BasePrice      DECIMAL(18,2);
    DECLARE @DailyBase      DECIMAL(18,2);
    DECLARE @ExtraPerPerson DECIMAL(18,2);
    DECLARE @BaseGuests     INT;
    DECLARE @Multiplier     DECIMAL(5,2);
    DECLARE @PreferSpecial  BIT;
    DECLARE @ExtraGuests    INT;
    SET @TotalRate = 0;

    IF @TotalGuests IS NULL OR @TotalGuests < 1 SET @TotalGuests = 1;

    SELECT @BasePrice = BasePrice FROM Rooms WHERE Id = @RoomId;

    WHILE @CurrentDate < @CheckOut
    BEGIN
        SET @DailyBase      = NULL;
        SET @ExtraPerPerson = NULL;
        SET @BaseGuests     = NULL;
        SET @Multiplier     = NULL;

        -- Tarifa especial Lun-Jue solo aplica si NO es alta temporada (PriceMultiplier <= 1).
        SET @PreferSpecial = CASE
            WHEN DATEPART(WEEKDAY, @CurrentDate) BETWEEN 1 AND 4 THEN 1
            ELSE 0
        END;

        SELECT TOP 1
            @DailyBase      = rt.PricePerNight,
            @ExtraPerPerson = rt.ExtraPersonPrice,
            @BaseGuests     = rt.BaseGuests,
            @Multiplier     = s.PriceMultiplier
        FROM Rates rt
        INNER JOIN Seasons s ON rt.SeasonId = s.Id
        WHERE rt.RoomId = @RoomId
          AND rt.IsActive = 1
          AND s.IsActive = 1
          AND s.StartDate <= @CurrentDate
          AND s.EndDate   >= @CurrentDate
          AND (
                (@PreferSpecial = 1 AND rt.Kind = 1 AND ISNULL(s.PriceMultiplier, 1) <= 1)
                OR rt.Kind = 0
              )
        ORDER BY
            CASE
                WHEN @PreferSpecial = 1 AND rt.Kind = 1 AND ISNULL(s.PriceMultiplier, 1) <= 1 THEN 0
                ELSE 1
            END,
            s.PriceMultiplier DESC;

        IF @DailyBase IS NULL
        BEGIN
            SET @DailyBase      = @BasePrice;
            SET @ExtraPerPerson = 0;
            SET @BaseGuests     = 1;
            SET @Multiplier     = 1.0;
        END

        IF @Multiplier IS NULL OR @Multiplier = 0 SET @Multiplier = 1.0;
        IF @BaseGuests IS NULL OR @BaseGuests < 1 SET @BaseGuests = 1;
        IF @ExtraPerPerson IS NULL SET @ExtraPerPerson = 0;

        SET @ExtraGuests = @TotalGuests - @BaseGuests;
        IF @ExtraGuests < 0 SET @ExtraGuests = 0;

        -- Las tarifas SpecialWeekday tienen precio final ya rebajado: no aplica multiplicador de temporada.
        DECLARE @DailyTotal DECIMAL(18,2);
        IF @PreferSpecial = 1 AND EXISTS (
            SELECT 1 FROM Rates rt2 INNER JOIN Seasons s2 ON rt2.SeasonId = s2.Id
            WHERE rt2.RoomId = @RoomId AND rt2.Kind = 1 AND rt2.IsActive = 1
              AND s2.IsActive = 1 AND s2.StartDate <= @CurrentDate AND s2.EndDate >= @CurrentDate
              AND ISNULL(s2.PriceMultiplier, 1) <= 1
              AND rt2.PricePerNight = @DailyBase
        )
            SET @DailyTotal = @DailyBase + (@ExtraGuests * @ExtraPerPerson);
        ELSE
            SET @DailyTotal = (@DailyBase * @Multiplier) + (@ExtraGuests * @ExtraPerPerson);

        SET @TotalRate = @TotalRate + @DailyTotal;

        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END
END
GO

-- SP5: Validar overbooking (verificar si una habitación tiene reserva en las fechas)
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
          AND res.Status != 5 -- Not Cancelled
          AND res.CheckIn < @CheckOut
          AND res.CheckOut > @CheckIn
          AND (@ExcludeReservationId IS NULL OR rd.ReservationId != @ExcludeReservationId)
    )
        SET @IsOverbooked = 1;
    ELSE
        SET @IsOverbooked = 0;
END
GO

-- SP6: Consultar tarifas por criterios (sitio, temporada, alojamiento, personas)
-- Cumple requisito FODUN: 'tarifas segun sitio, temporada, numero de personas y alojamiento'
CREATE OR ALTER PROCEDURE sp_GetRatesByCriteria
    @SiteId INT = NULL,
    @AccommodationId INT = NULL,
    @SeasonId INT = NULL,
    @Guests INT = NULL,
    @CheckIn DATE = NULL,
    @CheckOut DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rt.Id            AS RateId,
        rt.PricePerNight,
        rt.BaseGuests,
        rt.ExtraPersonPrice,
        rt.Kind          AS RateKind,
        rt.IsActive      AS RateIsActive,
        r.Id             AS RoomId,
        r.RoomNumber,
        r.Type           AS RoomType,
        r.MaxGuests,
        r.BasePrice,
        a.Id             AS AccommodationId,
        a.Name           AS AccommodationName,
        s.Id             AS SiteId,
        s.Name           AS SiteName,
        se.Id            AS SeasonId,
        se.Name          AS SeasonName,
        se.StartDate,
        se.EndDate,
        se.PriceMultiplier,
        CAST(
            (rt.PricePerNight * ISNULL(NULLIF(se.PriceMultiplier, 0), 1.0))
            + (CASE WHEN @Guests IS NULL OR @Guests <= rt.BaseGuests THEN 0
                    ELSE (@Guests - rt.BaseGuests) * rt.ExtraPersonPrice END)
        AS DECIMAL(18,2)) AS EffectivePricePerNight
    FROM Rates rt
    INNER JOIN Rooms          r  ON rt.RoomId   = r.Id
    INNER JOIN Accommodations a  ON r.AccommodationId = a.Id
    INNER JOIN Sites          s  ON a.SiteId    = s.Id
    INNER JOIN Seasons        se ON rt.SeasonId = se.Id
    WHERE rt.IsActive = 1
      AND r.IsActive = 1
      AND a.IsActive = 1
      AND s.IsActive = 1
      AND se.IsActive = 1
      AND (@SiteId          IS NULL OR s.Id  = @SiteId)
      AND (@AccommodationId IS NULL OR a.Id  = @AccommodationId)
      AND (@SeasonId        IS NULL OR se.Id = @SeasonId)
      AND (@Guests          IS NULL OR r.MaxGuests >= @Guests)
      AND (@CheckIn         IS NULL OR @CheckOut IS NULL
           OR (se.StartDate <= @CheckOut AND se.EndDate >= @CheckIn))
    ORDER BY s.Name, a.Name, r.RoomNumber, se.StartDate;
END
GO
