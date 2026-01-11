Select * From Users;
Select * From UserSessions;

Select * From Logs;

Select * From Payments;
Select * From PaymentLogs;

Select * From Orders;
Select * From OrderItems;

Select * From Products;

Select * From Categories;
Select * From SubCategories;

Delete From OrderItems Where Id = 1;

ALTER TABLE Logs
ADD TraceId NVARCHAR(100) NULL;