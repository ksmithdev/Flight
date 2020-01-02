CREATE TABLE Employees (
	EmployeeNumber TEXT NOT NULL,
	FirstName TEXT NOT NULL,
	LastName TEXT NOT NULL,
	CONSTRAINT PK_Employees PRIMARY KEY (EmployeeNumber)
);

CREATE TABLE ProjectTypes (
	ProjectTypeId INT NOT NULL,
	DisplayName TEXT NOT NULL,
	CONSTRAINT PK_ProjectTypes PRIMARY KEY (ProjectTypeId)
);

CREATE TABLE Projects (
	ProjectNumber TEXT NOT NULL,
	ProjectTypeId INT NOT NULL,
	ProjectManagerNumber TEXT NOT NULL,
	CONSTRAINT PK_Projects PRIMARY KEY (ProjectNumber),
	CONSTRAINT FK_Projects_ProjectTypes FOREIGN KEY (ProjectTypeId) REFERENCES ProjectTypes(ProjectTypeId),
	CONSTRAINT FK_Projects_Employees FOREIGN KEY (ProjectManagerNumber) REFERENCES Employees(EmployeeNumber)
);