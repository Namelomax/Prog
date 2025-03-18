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