do $$
declare
    cardDocumentID uuid;
	refBaseUniversalID uuid;
    refStatesID uuid;
	business_trip_info text;
	main_info text;
	items text;
	state_names text;
	sql_text text;
begin
    -- Id карточки "Документ"
    select "CardTypeID" into cardDocumentID
    from dvsys_carddefs where "Alias" = 'CardDocument';
    -- Id карточки "Конструктор справочников"
    select "CardTypeID" into refBaseUniversalID
    from dvsys_carddefs where "Alias" = 'RefBaseUniversal';
	-- Id карточки "Конструктор состояний"
    select "CardTypeID" into refStatesID
    from dvsys_carddefs where "Alias" = 'RefStates';

	-- Таблица секции с информацией о командировке
    select format('dvtable_{%s}', "SectionTypeID") into business_trip_info
    from dvsys_sectiondefs where "Alias" = 'Командировка' and "CardTypeID" = cardDocumentID;
	-- Таблица секции с основной информацией
    select format('dvtable_{%s}', "SectionTypeID") into main_info
    from dvsys_sectiondefs where "Alias" = 'MainInfo' and "CardTypeID" = cardDocumentID;
	-- Таблица секции со строками справочника
    select format('dvtable_{%s}', "SectionTypeID") into items
    from dvsys_sectiondefs where "Alias" = 'Items' and "CardTypeID" = refBaseUniversalID;
	-- Таблица секции с именами состояний
    select format('dvtable_{%s}', "SectionTypeID") into state_names
    from dvsys_sectiondefs where "Alias" = 'StateNames' and "CardTypeID" = refStatesID;

	sql_text := format('
create or replace function dd_business_trip_history(val_travelerId uuid)
returns table (
    "FromDate" timestamp,
    "City" text,
    "Reason" text,
    "State" text
) as $function$
declare
begin
	return query
	select bti."fromTravelDate", i."Name", bti."reasonTravel", sn."Name"
	from %I bti
		join %I mi on bti."InstanceID" = mi."InstanceID"
		join %I i on bti."city" = i."RowID" 
		join %I sn on mi."State" = sn."ParentRowID" 
	where sn."LocaleID" = 1049 and bti."traveler" = val_travelerId;
end;
$function$ language plpgsql;
	', business_trip_info, main_info, items, state_names);

	EXECUTE sql_text;
end $$;