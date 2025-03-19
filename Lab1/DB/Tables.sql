CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY,
    ClientName NVARCHAR(100),
    OrderType NVARCHAR(50)
);

CREATE TABLE OrderOptions (
    OptionId INT PRIMARY KEY IDENTITY,
    OrderDishId INT,
    OptionName NVARCHAR(100),
    FOREIGN KEY (OrderDishId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

CREATE table OrderDishes(
    OrderDishId INT PRIMARY KEY IDENTITY,
    Quantity INT,
    Dish NVARCHAR(100),
    OrderId INT,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

-- Таблица для всех доступных блюд
CREATE TABLE AvailableDishes (
    AvailableDishID INT PRIMARY KEY IDENTITY(1,1), -- Уникальный ID с автоинкрементом
    Name NVARCHAR(100) NOT NULL,                   -- Название блюда
    Price int NOT NULL                  -- Цена блюда
);

-- Таблица для всех доступных опций
CREATE TABLE AvailableOptions (
    AvailableOptionID INT PRIMARY KEY IDENTITY(1,1), -- Уникальный ID с автоинкрементом
    Name NVARCHAR(100) NOT NULL,                     -- Название опции
    Price int NOT NULL                    -- Цена опции
);

SELECT 
    d.Name AS DishName,
    o.Name AS OptionName,
    COUNT(*) AS OptionCount
FROM 
    OrderDishes od
JOIN AvailableDishes d ON od.AvailableDishID = d.AvailableDishID
JOIN OrderOptions oo ON oo.OrderDishID = od.OrderDishID
JOIN AvailableOptions o ON oo.AvailableOptionID = o.AvailableOptionID
GROUP BY 
    d.Name, o.Name
ORDER BY 
    d.Name, OptionCount DESC;
