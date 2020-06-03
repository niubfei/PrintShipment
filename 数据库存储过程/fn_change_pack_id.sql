-- FUNCTION: public.fn_change_pack_id(text, text, text, text, text)

-- DROP FUNCTION public.fn_change_pack_id(text, text, text, text, text);

CREATE OR REPLACE FUNCTION public.fn_change_pack_id(
	a_old_pack_id text,
	a_user text,
	a_vendor text,
	a_sitecode text,
	a_date text)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
DECLARE
	v_rows RECORD;
	v_array TEXT[];
	v_new_pack_id TEXT;

BEGIN
	--该pack状态是否已包装(status=1)且存在
	SELECT ARRAY_AGG(tray_id) INTO v_array
	FROM pnt_pack
	WHERE pack_id=a_old_pack_id
	AND status=1
	GROUP BY pnt_pack.pack_id;
	IF v_array IS NULL THEN
		--RAISE NOTICE '该pack号没有装包，请输入已装包的';
		--RETURN 'ERROR';
		RAISE EXCEPTION '该pack号没有装包，请输入已装包的';
	END IF;

	--该pack没装箱(status=1)
	SELECT COUNT(pack_id)=0 AS not_in_carton INTO v_rows
	FROM pnt_carton
	WHERE pack_id=a_old_pack_id
	AND status=1;
	IF v_rows.not_in_carton IS FALSE THEN
		RAISE EXCEPTION '该pack号已装箱，请输入没装箱的';
	END IF;

	--增加拆除操作记录，和该pack所有tray记录状态改为拆除(status=2)
	INSERT INTO pnt_mng (pkg_id,pkg_type,act,pkg_date,pkg_qty,pkg_user,pkg_status,remark)
	VALUES(a_old_pack_id,'pack','c',NOW(),ARRAY_LENGTH(v_array,1),a_user,2,'change pack id');

	UPDATE pnt_pack
	SET p_date = now(),status=2
	WHERE pack_id = a_old_pack_id
	AND status=1;

	--获取新ID
	SELECT fn_apply_no('pack',a_vendor,a_sitecode,a_date) INTO v_new_pack_id;

	--增加装包操作记录，和将之前改为拆除状态的tray号与新pack号插入表中且状态是1已包装
	INSERT INTO pnt_mng (pkg_id,pkg_type,act,pkg_date,pkg_qty,pkg_user,pkg_status,remark)
	VALUES(v_new_pack_id,'pack','r',NOW(),ARRAY_LENGTH(v_array,1),a_user,1,NULL);

	FOR i IN 1..ARRAY_LENGTH(v_array,1) LOOP
		INSERT INTO pnt_pack (pack_id,tray_id,p_date,status)
		VALUES(v_new_pack_id,v_array[i],NOW(),1);
	END LOOP;
--EXCEPTION
	--WHEN OTHERS THEN
	--ROLLBACK;
--COMMIT;
	RETURN v_new_pack_id;
END
$BODY$;

ALTER FUNCTION public.fn_change_pack_id(text, text, text, text, text)
    OWNER TO pqm;
