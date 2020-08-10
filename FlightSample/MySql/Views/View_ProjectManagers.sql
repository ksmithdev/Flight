DROP VIEW IF EXISTS ProjectManagers;

CREATE VIEW ProjectManagers
AS
SELECT p.ProjectNumber,
    pt.DisplayName AS ProjectType,
	p.Description,
    e.EmployeeNumber,
    e.FirstName,
    e.LastName
FROM Projects p
INNER JOIN ProjectTypes pt ON pt.ProjectTypeId=p.ProjectTypeId
INNER JOIN Employees e ON e.EmployeeNumber=p.ProjectManagerNumber;