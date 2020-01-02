CREATE TABLE Employees (
	EmployeeNumber NVARCHAR(15) NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	CONSTRAINT PK_Employees PRIMARY KEY (EmployeeNumber)
);
GO

CREATE TABLE ProjectTypes (
	ProjectTypeId INT NOT NULL,
	DisplayName NVARCHAR(15) NOT NULL,
	CONSTRAINT PK_ProjectTypes PRIMARY KEY (ProjectTypeId)
);
GO

CREATE TABLE Projects (
	ProjectNumber NVARCHAR(15) NOT NULL,
	ProjectTypeId INT NOT NULL,
	ProjectManagerNumber NVARCHAR(15) NOT NULL,
	CONSTRAINT PK_Projects PRIMARY KEY (ProjectNumber),
	CONSTRAINT FK_Projects_ProjectTypes FOREIGN KEY (ProjectTypeId) REFERENCES ProjectTypes(ProjectTypeId),
	CONSTRAINT FK_Projects_Employees FOREIGN KEY (ProjectManagerNumber) REFERENCES Employees(EmployeeNumber)
);
GO

INSERT INTO ProjectTypes (ProjectTypeId, DisplayName) VALUES (1, N'Standard');
INSERT INTO ProjectTypes (ProjectTypeId, DisplayName) VALUES (2, N'T&M');
GO