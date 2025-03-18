CREATE PROCEDURE GetOrders
AS
BEGIN
    SELECT 
        o.OrderId, 
        o.ClientName, 
        o.Dish, 
        o.OrderType, 
        STRING_AGG(op.OptionName, ', ') AS Options
    FROM Orders o
    LEFT JOIN OrderOptions op ON o.OrderId = op.OrderId
    GROUP BY o.OrderId, o.ClientName, o.Dish, o.OrderType;
END;
