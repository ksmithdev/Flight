INSERT INTO public.employees (employee_number, first_name, last_name)
VALUES ('E000001', 'Test', 'McTest'),
	('E000002', 'Test', 'McTest Jr');
INSERT INTO public.projects (project_number, project_type_id, project_manager_number)
VALUES ('P00001', 1, 'E000001'),
	('P00002', 1, 'E000001'),
	('T00001', 2, 'E000002');