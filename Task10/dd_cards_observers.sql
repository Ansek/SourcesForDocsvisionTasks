create or replace function "dd_cards_observers"(val_cardids uuid[])
returns table("CardID" uuid, "Value" uuid, "Type" integer)
as $function$
begin
	return query
	select
	info."InstanceID" as "CardID", observer."ObserverID" as "Value", 13 as "Type"
	from "dvtable_{ce5f1dd4-ef59-43ae-ae39-2e186605caba}" as info
		join dd_cities_observers observer on info."city" = observer."CityID"
	where info."InstanceID" = ANY(val_cardids);
end;
$function$
language plpgsql;