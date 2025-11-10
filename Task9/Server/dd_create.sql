create table if not exists dd_cities_observers (
    "CityID"    uuid not null,
    "ObserverID" uuid not null,
    primary key ("CityID", "ObserverID"),
    foreign key ("CityID")
    	references "dvtable_{1b1a44fb-1fb1-4876-83aa-95ad38907e24}"("RowID") -- Города
		on delete cascade,
    foreign key ("ObserverID")
    	references "dvtable_{dbc8ae9d-c1d2-4d5e-978b-339d22b32482}"("RowID") -- employees
    	on delete cascade
);

create or replace procedure dd_link_cities_to_observer(cities json, observerID uuid)
language plpgsql
as $$
declare
    city text;
	cityID uuid;
	typeID uuid default 'b56b2216-7704-4142-b44b-28b39ca1303a'::uuid; -- Города
begin
    for city in select * from json_array_elements_text(cities)
	loop
		select "RowID" into cityID
		from "dvtable_{1b1a44fb-1fb1-4876-83aa-95ad38907e24}" items
		where "ParentRowID" = typeID and "Name" = city;
	
		if cityID is null then -- items
			insert into "dvtable_{1b1a44fb-1fb1-4876-83aa-95ad38907e24}" ("Name", "ParentRowID", "SDID")
			values(city, typeID, '1e49c422-dd61-440f-a4f2-699a69d52873'::uuid)
			returning "RowID" into cityID;
		end if;

		insert into dd_cities_observers("CityID", "ObserverID")
		values(cityID, observerID);
    end loop;
end; $$;

create or replace procedure dd_add_observers(observers json, departmentID uuid)
language plpgsql
as $$
declare
    item json;
	fio text[];
	positionID uuid;
	observerID uuid;
begin
    for item in select * from json_array_elements(observers)
	loop
		select "RowID" into positionID
		from "dvtable_{cfdfe60a-21a8-4010-84e9-9d2df348508c}" positions
		where "Name" = item->>'Position';
	
		if positionID is null then -- positions
			insert into "dvtable_{cfdfe60a-21a8-4010-84e9-9d2df348508c}" ("Name")
			values(item->>'Position')
			returning "RowID" into positionID;
		end if;

		fio := string_to_array(item->>'FIO', ' ');
		insert into "dvtable_{dbc8ae9d-c1d2-4d5e-978b-339d22b32482}" -- employees
		("ParentRowID", "LastName", "FirstName", "MiddleName", "Position", "SDID")
		values(departmentID, fio[1], fio[2], fio[3], positionID,
			'27915402-a83c-4778-a9f0-b2459cd56ae5'::uuid)
		returning "RowID" into observerID;

		call dd_link_cities_to_observer(item->'Cities', observerID);
    end loop;
end; $$;

create or replace procedure dd_add_observer_departments(
	departments json,
	parentID uuid default uuid_nil()
) language plpgsql
as $$
declare
    item json;
	departmentID uuid;
begin
    for item in select * from json_array_elements(departments)
    loop
		select "RowID" into departmentID
		from "dvtable_{7473f07f-11ed-4762-9f1e-7ff10808ddd1}" units
		where "ParentTreeRowID" = parentID and "Name" = item->>'Name';

		if departmentID is null then -- units
			insert into "dvtable_{7473f07f-11ed-4762-9f1e-7ff10808ddd1}" ("Name", "ParentTreeRowID", "Type", "SDID")
			values(item->>'Name', parentID, 1, '27915402-a83c-4778-a9f0-b2459cd56ae5'::uuid)
			returning "RowID" into departmentID;
		end if;

		call dd_add_observers(item->'Observers', departmentID);
		call dd_add_observer_departments(item->'Subdepartments', departmentID);
    end loop;
end; $$;