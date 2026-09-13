CREATE TABLE Products (Id uniqueidentifier NOT NULL CONSTRAINT PK_Products PRIMARY KEY, Name nvarchar(200) NOT NULL, Description nvarchar(max) NOT NULL, Price decimal(18,2) NOT NULL, Stock int NOT NULL, IsActive bit NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT 1);
CREATE INDEX IX_Products_IsActive_Name ON Products(IsActive, Name);
CREATE TABLE Carts (Id uniqueidentifier NOT NULL CONSTRAINT PK_Carts PRIMARY KEY, UserId uniqueidentifier NOT NULL CONSTRAINT UQ_Carts_UserId UNIQUE);
CREATE TABLE CartItems (Id uniqueidentifier NOT NULL CONSTRAINT PK_CartItems PRIMARY KEY, CartId uniqueidentifier NOT NULL, ProductId uniqueidentifier NOT NULL, Quantity int NOT NULL, UnitPrice decimal(18,2) NOT NULL, CONSTRAINT FK_CartItems_Carts FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE, CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id));
CREATE INDEX IX_CartItems_CartId_ProductId ON CartItems(CartId, ProductId);
CREATE TABLE Orders (Id uniqueidentifier NOT NULL CONSTRAINT PK_Orders PRIMARY KEY, UserId uniqueidentifier NOT NULL, CreatedUtc datetime2 NOT NULL, Status int NOT NULL, Total decimal(18,2) NOT NULL);
CREATE INDEX IX_Orders_UserId_CreatedUtc ON Orders(UserId, CreatedUtc DESC);
CREATE TABLE OrderItems (Id uniqueidentifier NOT NULL CONSTRAINT PK_OrderItems PRIMARY KEY, OrderId uniqueidentifier NOT NULL, ProductId uniqueidentifier NOT NULL, Quantity int NOT NULL, UnitPrice decimal(18,2) NOT NULL, CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE, CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id));
GO
