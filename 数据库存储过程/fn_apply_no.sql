-- FUNCTION: public.fn_apply_no(text, text, text, text)

-- DROP FUNCTION public.fn_apply_no(text, text, text, text);

CREATE OR REPLACE FUNCTION public.fn_apply_no(
	a_mode text,
	a_vendor text,
	a_sitecode text,
	a_date text)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
declare
    s_words text;
    v_idx integer;
    v_rec Record;
    v_prefix char;
    v_serial integer;
    v_len integer;
    v_serial_no text;
    v_ret text;

--BEGIN TRANSACTION ISOLATION LEVEL REPEATABLE READ;  

BEGIN 
    --设定编号前缀字符 
    if a_mode = 'pack' then
        v_prefix := 'B';
    elseif a_mode = 'carton' then
        v_prefix := 'H';
    elseif a_mode = 'pallet' then
        v_prefix := 'P';
    else
        raise notice 'Error Mode';
        return '';
    end if;
    
    --建立顺序字母表 
    s_words := '123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
 
    --构建新编码
    select * into v_rec from t_apply_no where ap_mode = a_mode and ap_vendor = a_vendor and ap_sitecode = a_sitecode and ap_date = a_date;
    if v_rec is null then
        v_ret := v_prefix || a_vendor || a_date || a_sitecode || '0001';
        insert into t_apply_no values (a_mode, a_vendor, a_sitecode, a_date, 1, now());
    else
        v_serial := v_rec.ap_serial + 1;

        -- 计算顺番
        v_serial_no := '';
        while v_serial >= 36 loop
            v_idx := v_serial % 36;
            v_serial := v_serial / 36;

            if v_idx = 0 then
                v_serial_no := '0' || v_serial_no;
            else
                v_serial_no := substring(s_words, v_idx, 1) || v_serial_no;
            end if;
        end loop;
        v_serial_no := substring(s_words, v_serial, 1) || v_serial_no;

        --合成顺番
        v_len := 4 - length(v_serial_no);
        v_serial_no := substring('0000', 0, v_len+1) || v_serial_no;
        v_serial_no := a_sitecode || v_serial_no;
        v_ret = v_prefix || a_vendor || a_date || v_serial_no;
        update t_apply_no set ap_serial = v_rec.ap_serial + 1, ap_lasttime = now() where ap_mode = a_mode and ap_vendor = a_vendor and ap_sitecode = a_sitecode and ap_date = a_date;
    end if;
    return v_ret;
END;
$BODY$;

ALTER FUNCTION public.fn_apply_no(text, text, text, text)
    OWNER TO pqm;
