-- FUNCTION: public.fn_deep_unpack(text, text, integer, text)

-- DROP FUNCTION public.fn_deep_unpack(text, text, integer, text);

CREATE OR REPLACE FUNCTION public.fn_deep_unpack(
	a_mode text,
	a_code text,
	a_qty integer,
	a_user text)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
declare
    v_rec Record;
    v_ret text;
BEGIN 
    raise notice 'mode:%  code:%', a_mode, a_code;
    v_ret = 'OK';
    if a_mode = 'pallet' then
        -- 1. cancel pallet
        update pnt_pallet set status = 2 where pallet_id = a_code;
        update pnt_carton set status = 2 where carton_id in (select carton_id from pnt_pallet where pallet_id = a_code);
        update pnt_pack set status = 2 where pack_id in (select pack_id from pnt_carton where carton_id in (select carton_id from pnt_pallet where pallet_id = a_code));
    elseif a_mode = 'carton' then
        -- 2. cancel carton
        update pnt_pallet set status = 2 where carton_id = a_code;
        update pnt_carton set status = 2 where carton_id = a_code;
        update pnt_pack set status = 2 where pack_id in (select pack_id from pnt_carton where carton_id = a_code);
    elseif a_mmode = 'pack' then
        -- 3. cancel pack
        update pnt_carton set status = 2 where pack_id = a_code;
        update pnt_pack set status = 2 where pack_id = a_code;
    else
        -- 4. unknow mode
        v_ret := 'NG';
        return v_ret;
    end if;

    --5.insert manager table
    insert into pnt_mng (pkg_id, pkg_type, act, pkg_date, pkg_qty, pkg_user, pkg_status, remark) values (a_code, a_mode, 'c', now(), a_qty, a_user, 2, 'deep cancel');
    
    return v_ret;
END;
$BODY$;

ALTER FUNCTION public.fn_deep_unpack(text, text, integer, text)
    OWNER TO pqm;
