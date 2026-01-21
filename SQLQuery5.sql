Select * From Logs;
Select * From Users;
Select * From Orders;
Select * From Payments;
Select * From Products;
Select * From Categories;
Select * From PaymentLogs;
Select * From UserSessions;
Select * From SubCategories;

CREATE TABLE BannedIps (
    Id INT PRIMARY KEY IDENTITY,
    IpAddress NVARCHAR(50),
    BannedUntil DATETIME,
    Reason NVARCHAR(255)
);

Drop TABLE BannedIps;

CREATE TABLE BannedIps (
    Id INT IDENTITY PRIMARY KEY,
    IpAddress NVARCHAR(50) NOT NULL,
    BannedUntil DATETIME NOT NULL,
    Reason NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETUTCDATE()
);

Select * From BannedIps;

CREATE TABLE LoginLockouts (
    Id INT IDENTITY PRIMARY KEY,
    KeyValue NVARCHAR(255),
    FailedAttempts INT,
    LockedUntil DATETIME NULL
);

Select * From LoginLockouts;

Drop TABLE LoginLockouts;

CREATE TABLE LoginLockouts (
    Id INT IDENTITY PRIMARY KEY,
    KeyValue NVARCHAR(255) UNIQUE,
    FailedAttempts INT NOT NULL,
    LockedUntil DATETIME NULL
);

Select * From LoginLockouts;