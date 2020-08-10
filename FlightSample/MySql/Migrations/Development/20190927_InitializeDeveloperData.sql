INSERT INTO Employees (EmployeeNumber, FirstName, LastName)
VALUES ('E000001', 'Test', 'McTest'),
	('E000002', 'Test', 'McTest Jr');

INSERT INTO Projects (ProjectNumber, ProjectTypeId, ProjectManagerNumber)
VALUES ('P00001', 1, 'E000001'),
	('P00002', 1, 'E000001'),
	('T00001', 2, 'E000002');