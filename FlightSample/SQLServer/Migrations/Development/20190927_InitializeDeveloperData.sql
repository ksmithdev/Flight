INSERT INTO dbo.Employees (EmployeeNumber, FirstName, LastName)
VALUES (N'E000001', N'Test', N'McTest'),
	(N'E000002', N'Test', N'McTest Jr');
INSERT INTO dbo.Projects (ProjectNumber, ProjectTypeId, ProjectManagerNumber)
VALUES (N'P00001', 1, N'E000001'),
	(N'P00002', 1, N'E000001'),
	(N'T00001', 2, N'E000002');
GO