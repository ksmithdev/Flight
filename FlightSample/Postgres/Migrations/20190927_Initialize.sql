CREATE TABLE public.employees (
	employee_number VARCHAR(15) NOT NULL,
	first_name VARCHAR(50) NOT NULL,
	last_name VARCHAR(50) NOT NULL,
	CONSTRAINT pk_employees PRIMARY KEY (employee_number)
);

CREATE TABLE public.project_types (
	project_type_id INT NOT NULL,
	display_name VARCHAR(15) NOT NULL,
	CONSTRAINT pk_projecttypes PRIMARY KEY (project_type_id)
);

CREATE TABLE public.projects (
	project_number VARCHAR(15) NOT NULL,
	project_type_id INT NOT NULL,
	project_manager_number VARCHAR(15) NOT NULL,
	CONSTRAINT pk_projects PRIMARY KEY (project_number),
	CONSTRAINT fk_projects_project_types FOREIGN KEY (project_type_id) REFERENCES project_types(project_type_id),
	CONSTRAINT fk_projects_employees FOREIGN KEY (project_manager_number) REFERENCES employees(employee_number)
);

INSERT INTO project_types (project_type_id, display_name) VALUES (1, 'Standard');
INSERT INTO project_types (project_type_id, display_name) VALUES (2, 'T&M');