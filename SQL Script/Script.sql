CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    StockCount INT NOT NULL,
    Region NVARCHAR(50) NOT NULL
);

CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Region NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);


CREATE TABLE OrderItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL,

    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE ProcessedOrders (
    OrderId UNIQUEIDENTIFIER PRIMARY KEY,
    ProcessedAt DATETIME NOT NULL DEFAULT GETDATE()
);


-- Insert Products
INSERT INTO Products (Id, Name, StockCount, Region)
VALUES
('11111111-1111-1111-1111-111111111111', 'Wireless Mouse', 150, 'North'),
('22222222-2222-2222-2222-222222222222', 'Mechanical Keyboard', 8, 'South'),
('33333333-3333-3333-3333-333333333333', 'Laptop', 5, 'East'),
('44444444-4444-4444-4444-444444444444', 'Monitor', 80, 'West'),
('55555555-5555-5555-5555-555555555555', 'USB-C Cable', 200, 'North');


-- Insert Orders
INSERT INTO Orders (Id, Region, CreatedAt)
VALUES
('aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'North', GETDATE()),
('aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 'South', GETDATE());


-- Insert OrderItems for Order 1 (North)
INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity)
VALUES
('bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 'aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '11111111-1111-1111-1111-111111111111', 2), -- Wireless Mouse
('bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '55555555-5555-5555-5555-555555555555', 3); -- USB-C Cable

-- Insert OrderItems for Order 2 (South)
INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity)
VALUES
('bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 'aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2', '22222222-2222-2222-2222-222222222222', 1); -- Mechanical Keyboard




select * from Products
select * from Orders
select * from OrderItems 


/*
truncate table OrderItems
truncate table Orders
truncate table Products
*/


