PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT * FROM Projects;

DROP TABLE Projects;

CREATE TABLE Projects (
	ProjectNumber TEXT NOT NULL,
	ProjectTypeId INT NOT NULL,
	ProjectManagerNumber TEXT NOT NULL,
    Description TEXT CONSTRAINT DF_Projects_Description DEFAULT (''),
	CONSTRAINT PK_Projects PRIMARY KEY (ProjectNumber),
	CONSTRAINT FK_Projects_ProjectTypes FOREIGN KEY (ProjectTypeId) REFERENCES ProjectTypes(ProjectTypeId),
	CONSTRAINT FK_Projects_Employees FOREIGN KEY (ProjectManagerNumber) REFERENCES Employees(EmployeeNumber)
);

INSERT INTO Projects (ProjectNumber, ProjectTypeId, ProjectManagerNumber)
    SELECT ProjectNumber,
        ProjectTypeId,
        ProjectManagerNumber
    FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

PRAGMA foreign_keys = 1;