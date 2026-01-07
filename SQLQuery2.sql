create TABLE Orders(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber NVARCHAR(50) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

create TABLE Payments(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    Provider NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    TransactionId NVARCHAR(100),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);

create TABLE PaymentLogs(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequestJson NVARCHAR(MAX),
    ResponseJson NVARCHAR(MAX),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

create TABLE Users(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    NationalId NVARCHAR(14) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

create TABLE Logs(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Level NVARCHAR(50),
    Message NVARCHAR(MAX),
    Exception NVARCHAR(MAX),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

create TABLE UserSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId INT NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

alter table Orders
add UserId int not null;

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Users
FOREIGN KEY (UserId) REFERENCES Users(Id);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(250),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE SubCategories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(250),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_SubCategories_Categories
        FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SubCategoryId INT NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Products_SubCategories
        FOREIGN KEY (SubCategoryId) REFERENCES SubCategories(Id)
);

CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,

    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice AS (Quantity * UnitPrice) PERSISTED,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_OrderItems_Orders
        FOREIGN KEY (OrderId) REFERENCES Orders(Id),

    CONSTRAINT FK_OrderItems_Products
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

ALTER TABLE Orders
DROP COLUMN OrderNumber;

Select * From Logs;
Select * From Users;
Select * From Orders;
Select * From Payments;
Select * From Products;
Select * From Categories;
Select * From PaymentLogs;
Select * From UserSessions;
Select * From SubCategories;