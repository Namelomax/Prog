CREATE PROCEDUre AddOrder
    @ClientName NVARCHAR(100),
    @Dish NVARCHAR(100),
    @OrderType NVARCHAR(50),
    @Options NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OrderId INT;

    -- Добавляем заказ и получаем его ID
    INSERT INTO Orders (ClientName, Dish, OrderType)
    VALUES (@ClientName, @Dish, @OrderType);

    SET @OrderId = SCOPE_IDENTITY();

    -- Разбиваем строку с опциями и добавляем в OrderOptions
    DECLARE @Option NVARCHAR(100);
    DECLARE @Pos INT;

    WHILE LEN(@Options) > 0
    BEGIN
        SET @Pos = CHARINDEX(',', @Options);
        IF @Pos = 0
        BEGIN
            SET @Option = @Options;
            SET @Options = '';
        END
        ELSE
        BEGIN
            SET @Option = LEFT(@Options, @Pos - 1);
            SET @Options = RIGHT(@Options, LEN(@Options) - @Pos);
        END

        INSERT INTO OrderOptions (OrderId, OptionName)
        VALUES (@OrderId, @Option);
    END;
END;