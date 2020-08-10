CREATE TABLE Employees (
	EmployeeNumber VARCHAR(15) NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	CONSTRAINT PK_Employees PRIMARY KEY (EmployeeNumber)
);

CREATE TABLE ProjectTypes (
	ProjectTypeId INT NOT NULL,
	DisplayName VARCHAR(15) NOT NULL,
	CONSTRAINT PK_ProjectTypes PRIMARY KEY (ProjectTypeId)
);

CREATE TABLE Projects (
	ProjectNumber VARCHAR(15) NOT NULL,
	ProjectTypeId INT NOT NULL,
	ProjectManagerNumber VARCHAR(15) NOT NULL,
	CONSTRAINT PK_Projects PRIMARY KEY (ProjectNumber),
	CONSTRAINT FK_Projects_ProjectTypes FOREIGN KEY (ProjectTypeId) REFERENCES ProjectTypes(ProjectTypeId),
	CONSTRAINT FK_Projects_Employees FOREIGN KEY (ProjectManagerNumber) REFERENCES Employees(EmployeeNumber)
);

INSERT INTO ProjectTypes (ProjectTypeId, DisplayName) VALUES (1, 'Standard');
INSERT INTO ProjectTypes (ProjectTypeId, DisplayName) VALUES (2, 'T&M');