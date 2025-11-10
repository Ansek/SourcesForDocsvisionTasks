create view dd_employee_company
as
select employees."RowID" as "EmployeeId", units."Name" as "Company" 
from "dvtable_{7473f07f-11ed-4762-9f1e-7ff10808ddd1}" units
	join "dvtable_{dbc8ae9d-c1d2-4d5e-978b-339d22b32482}" employees
		on units."SystemTreeID"::text = subltree(employees."ParentSectionTreeKey", 1, 2)::text
where units."Type" = 0;