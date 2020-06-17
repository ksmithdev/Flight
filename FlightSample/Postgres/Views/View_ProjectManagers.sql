DROP VIEW IF EXISTS project_managers;

CREATE VIEW project_managers
AS
SELECT p.project_number,
    pt.display_name AS project_type,
	p.description,
    e.employee_number,
    e.first_name,
    e.last_name
FROM projects p
INNER JOIN project_types pt ON pt.project_type_id=p.project_type_id
INNER JOIN employees e ON e.employee_number=p.project_manager_number;
