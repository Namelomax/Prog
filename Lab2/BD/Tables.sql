CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY,
    ClientName NVARCHAR(100),
    OrderType NVARCHAR(50)
);

CREATE TABLE OrderDishes (
    OrderDishId INT PRIMARY KEY IDENTITY,
    OrderId INT,
    Dish NVARCHAR(100),
    Quantity INT,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

CREATE TABLE OrderOptions (
    OptionId INT PRIMARY KEY IDENTITY,
    OrderDishId INT,
    OptionName NVARCHAR(100),
    FOREIGN KEY (OrderDishId) REFERENCES OrderDishes(OrderDishId) ON DELETE CASCADE
);
