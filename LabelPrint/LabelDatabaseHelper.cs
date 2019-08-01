using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TPCCommon.Common;
using TPCCommon.Database;
using LabelPrint.Model;

namespace LabelPrint
{
    /// <summary>
    /// 数据库处理模块
    /// </summary>
    public class LabelDatabaseHelper
    {
        public LabelDatabaseHelper(TPCDatabase database)
        {
            m_Database = database;
        }

        private TPCDatabase m_Database = null;
        public TPCDatabase Database
        {
            get { return m_Database; }
        }

        /// <summary>
        /// 登录系统
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns>登录用户的角色</returns>
        public TPCResult<string> Login(string user, string pass)
        {
            TPCResult<string> result = new TPCResult<string>();
            string sql = "select user_id, user_name, pass, u_role from t_user where user_id = @user";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@user", DbType.AnsiString, user));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.Message = dt.Message;
                result.State = RESULT_STATE.NG;
                return result;
            }
            //没有该用户
            if (dt.Value.Rows.Count == 0)
            {
                result.State = RESULT_STATE.NG;
                result.Message = string.Format("Not found user: {0}", user);
                return result;
            }

            string db_pass = dt.Value.Rows[0]["pass"].ToString();
            string db_role = dt.Value.Rows[0]["u_role"].ToString();
            if (!pass.Equals(db_pass))
            {
                result.Message = "Invalid password.";
                result.State = RESULT_STATE.NG;
                return result;
            }

            result.Value = db_role;

            return result;
        }

        /// <summary>
        /// 根据不同模式，关键词及日期，申请新的编号
        /// </summary>
        /// <param name="mode">模式：pack, carton, pallet</param>
        /// <param name="vendor">关键词</param>
        /// <param name="site_code"></param>
        /// <param name="date">日期</param>
        /// <param name="username"></param>
        /// <returns></returns>
        public TPCResult<string> ApplyNewCode(string mode, string vendor, string site_code, string date, int qty, string username)
        {
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@mode", DbType.AnsiString, mode));
            parameters.Add(new DbParameter("@vendor", DbType.AnsiString, vendor));
            parameters.Add(new DbParameter("@sitecode", DbType.AnsiString, site_code));
            parameters.Add(new DbParameter("@date", DbType.AnsiString, date));
            //日期参数要修改
            //TPCResult<string> result = Database.ExecuteSP("fn_apply_no", parameters);
            TPCResult<string> result = Database.ExecuteSP("fn_apply_no", parameters);
            if (result == null)
            {
                result = new TPCResult<string>();
                result.State = RESULT_STATE.NG;
                result.Message = "Execute function fn_apply_no failed.";
                return result;
            }
            if (result.State == RESULT_STATE.NG)
                return result;

            //成功生成ID，则需要加入管理表记录
            string sql = "insert into pnt_mng (pkg_id, pkg_type, act, pkg_date, pkg_qty, pkg_user, pkg_status, remark) "
                       + "values (@id, @type, @act, @date, @qty, @user, @status, @remark)";
            parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@id", DbType.AnsiString, result.Value));
            parameters.Add(new DbParameter("@type", DbType.AnsiString, mode));
            parameters.Add(new DbParameter("@act", DbType.AnsiString, "r"));
            parameters.Add(new DbParameter("@date", DbType.DateTime, DateTime.Now));
            parameters.Add(new DbParameter("@qty", DbType.Int32, qty));
            parameters.Add(new DbParameter("@user", DbType.AnsiString, username));
            parameters.Add(new DbParameter("@status", DbType.Int32, 0));
            parameters.Add(new DbParameter("@remark", DbType.AnsiString, ""));
            TPCResult<bool> ret = Database.Execute(sql, parameters);

            if (ret.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = ret.Message;
            }

            return result;
        }

        /// <summary>
        /// 检查tray code是否合法
        /// 这里主要用于重新捆包时，扫描已装tray编码
        /// 所以需要从t_module中查找该编号是否存在
        /// </summary>
        /// <param name="tray_no"></param>
        /// <param name="is_repack"></param>
        /// <returns></returns>
        public TPCResult<bool> CheckTrayNo(string tray_no, bool is_repack)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //1。在module表中是否存在
            string sql = "select count(tray_id) as qty from t_module where tray_id = @code";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, tray_no));

            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }
            int qty = Convert.ToInt32(dt.Value.Rows[0]["qty"]);
            if (qty == 0)
            {
                result.Value = false;
                return result;
            }
            //2。在pack表中不存在状态为0或1的
            sql = "select count(tray_id) as qty from pnt_pack where tray_id = @code and (status = 0 or status = 1)";
            dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            qty = Convert.ToInt32(dt.Value.Rows[0]["qty"]);
            if (is_repack)
                result.Value = (qty > 0);
            else
                result.Value = (qty == 0);

            return result;
        }

        /// <summary>
        /// 更新管理表信息
        /// </summary>
        /// <param name="mode">模式：pack, carton, pallet</param>
        /// <param name="id">箱id</param>
        /// <param name="user">用户</param>
        /// <param name="qty">装箱数量</param>
        /// <param name="action">操作:r=register; c=cancel</param>
        /// <param name="status">状态:0=BEGIN;1=COMPLETED;2=CANCELED</param>
        /// <returns></returns>
        public TPCResult<bool> SetManagerData(PACK_MODE mode, string id, string user, int qty, PACK_ACTION action, PACK_STATUS status)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //设定mode
            string[] MODE_NAME = new string[] { "pack", "carton", "pallet" };
            string mode_name = MODE_NAME[(int)mode];
            //设定act
            string[] ACTION_NAME = new string[] { "r", "c" };
            string act = ACTION_NAME[(int)action];

            string sql = "insert into pnt_mng (pkg_id, pkg_type, act, pkg_date, pkg_qty, pkg_user, pkg_status)"
                       + " values (@id, @type, @act, @date, @qty, @user, @status)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@id", DbType.AnsiString, id));
            parameters.Add(new DbParameter("@type", DbType.AnsiString, mode_name));
            parameters.Add(new DbParameter("@act", DbType.AnsiString, act));
            parameters.Add(new DbParameter("@date", DbType.DateTime, DateTime.Now));
            parameters.Add(new DbParameter("@qty", DbType.Int32, qty));
            parameters.Add(new DbParameter("@status", DbType.Int32, (int)status));
            parameters.Add(new DbParameter("@user", DbType.AnsiString, user));
            

            result = Database.Execute(sql, parameters);

            return result;
        }

        /// <summary>
        /// 通过数据库计算module数量
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public TPCResult<int> GetModuleCount(PACK_MODE mode, string code)
        {
            TPCResult<int> result = new TPCResult<int>();

            string sql = "";
            switch (mode)
            { 
                case PACK_MODE.Pack:
                    sql = "select count(m.module_id) as qty from t_module m, pnt_pack p" +
                        " where p.tray_id = m.tray_id and p.pack_id = @code "
                        + "and p.status = 1";
                    break;
                case PACK_MODE.Carton:
                    sql = "select count(m.module_id) as qty from t_module m, pnt_pack p, pnt_carton c "
                        + "where c.pack_id = p.pack_id and p.tray_id = m.tray_id and c.carton_id = @code "
                        + "and (p.status = 1 and c.status = 1)";
                    break;
                case PACK_MODE.Pallet:
                    sql = "select count(m.module_id) as qty from t_module m, pnt_pack p, pnt_carton c, pnt_pallet l "
                        + "where l.carton_id = c.carton_id and c.pack_id = p.pack_id and p.tray_id = m.tray_id and l.pallet_id = @code "
                        + "and (p.status = 1 and c.status = 1 and l.status = 1)";
                    break;
            }

            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, code));

            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            int qty = 0;
            if (!int.TryParse(dt.Value.Rows[0]["qty"].ToString(), out qty))
            {
                result.State = RESULT_STATE.NG;
                result.Message = "Get module count failed.";
                return result;
            }

            result.Value = qty;
            return result;
        }


        /// <summary>
        /// 通过数据库计算lot数量 修改时间20180518
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public TPCResult<List<KeyValuePair<string, string>>> GetLotCount(PACK_MODE mode, string code)
        {
            TPCResult<List<KeyValuePair<string, string>>> result = new TPCResult<List<KeyValuePair<string, string>>>();

            string sql = "";
            switch (mode)
            {
                case PACK_MODE.Pack:
                    sql = "select lot ,count(lot) as qty from t_module left join pnt_pack on t_module.tray_id = pnt_pack.tray_id "
                        + "where pack_id =  @code group by lot order by qty desc ,lot  limit 16";
                    break;
                case PACK_MODE.Carton:
                    sql = "select lot ,count(lot) as qty from t_module left join pnt_pack on t_module.tray_id = pnt_pack.tray_id "
                        + "left join pnt_carton on pnt_pack.pack_id = pnt_carton.pack_id "
                        + "where carton_id =  @code group by lot order by qty desc ,lot  limit 16";
                    break;
                case PACK_MODE.Pallet:
                    sql = "select lot ,count(lot) as qty from t_module left join pnt_pack on t_module.tray_id = pnt_pack.tray_id "
                        + "left join pnt_carton on pnt_pack.pack_id = pnt_carton.pack_id "
                        + "left join pnt_pallet on pnt_carton.carton_id = pnt_pallet.carton_id "
                        + "where pallet_id =  @code group by lot order by qty desc ,lot  limit 16";
                    break;
            }

            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, code));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);

            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            List<KeyValuePair<string, string>> lotCount = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < dt.Value.Rows.Count; i++)
            {
                int qty = 0;
                string lotName = "";
                string lotNum = "";
                lotName = dt.Value.Rows[i]["lot"].ToString();
                if (!int.TryParse(dt.Value.Rows[i]["qty"].ToString(), out qty))
                {
                    result.State = RESULT_STATE.NG;
                    result.Message = "Get module count failed.";
                    return result;
                }
                else
                {
                    lotNum = qty.ToString();
                    KeyValuePair<string, string> lotQty = new KeyValuePair<string, string>(lotName, lotNum);
                    lotCount.Add(lotQty);
                }
            }
            result.Value = lotCount;
            return result;
        }

        /// <summary>
        /// 通过数据库计算lot总数量
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public TPCResult<int> GetLotTotal(PACK_MODE mode, string code)
        {
            TPCResult<int> result = new TPCResult<int>();

            string sql = "";
            switch (mode)
            {

                case PACK_MODE.Pack:
                    sql = "select count(lot) as qty from t_module left join pnt_pack on t_module.tray_id = pnt_pack.tray_id "
                         + "where pack_id = @code ";
                    break;
                case PACK_MODE.Carton:
                    sql = "select count(lot) as qty from t_module left join pnt_pack on t_module.tray_id = pnt_pack.tray_id "
                        + "left join pnt_carton on pnt_pack.pack_id = pnt_carton.pack_id "
                        + "where carton_id =  @code ";
                    break;
                case PACK_MODE.Pallet:
                    sql = "select count(lot) as qty from t_module left join pnt_pack on t_module.tray_id = pnt_pack.tray_id "
                        + "left join pnt_carton on pnt_pack.pack_id = pnt_carton.pack_id "
                        + "left join pnt_pallet on pnt_carton.carton_id = pnt_pallet.carton_id "
                        + "where pallet_id =  @code ";
                    break;
            }

            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, code));

            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            int qty = 0;
            if (!int.TryParse(dt.Value.Rows[0]["qty"].ToString(), out qty))
            {
                result.State = RESULT_STATE.NG;
                result.Message = "Get module count failed.";
                return result;
            }

            result.Value = qty;
            return result;
        }

       
        #region 捆包操作
        /// <summary>
        /// 从未完成捆包的Pack中，按照Tray的条码获得状态为0（未完成）的Pack条码
        /// 为了方便增加status为1的也取出，但是不能重新打印标签，必须通过重新打印
        /// 画面打印
        /// </summary>
        /// <param name="tray_no"></param>
        /// <returns></returns>
        public TPCResult<string> GetPackNoByTrayNo(string tray_no)
        {
            TPCResult<string> result = new TPCResult<string>();

            string sql = "select pack_id from pnt_pack where tray_id = @tray and (status = 0 or status = 1)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@tray", DbType.AnsiString, tray_no));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);

            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            if (dt.Value.Rows.Count == 0)
            {
                result.State = RESULT_STATE.NG;
                result.Message = string.Format("Not found pack id by tray id:{0}", tray_no);
                return result;
            }

            result.Value = dt.Value.Rows[0]["pack_id"].ToString();
            return result;
        }

        /// <summary>
        /// 捆包时，扫描了Tray后，将记录写入数据库
        /// 状态为0（未完成）
        /// </summary>
        /// <param name="pack_no"></param>
        /// <param name="tray_no"></param>
        /// <param name="p_date"></param>
        /// <returns></returns>
        public TPCResult<bool> WritePackTray(string pack_no, string tray_no, DateTime p_date)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            string sql = "insert into pnt_pack values (@packid, @trayid, @pdate, 0)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@packid", DbType.AnsiString, pack_no));
            parameters.Add(new DbParameter("@trayid", DbType.AnsiString, tray_no));
            parameters.Add(new DbParameter("@pdate", DbType.DateTime, p_date));

            result = Database.Execute(sql, parameters);

            return result;
        }

        /// <summary>
        /// 检索以捆包的tray信息
        /// </summary>
        /// <param name="pack_no">Pack Code</param>
        /// <returns></returns>
        public TPCResult<List<CItem>> GetPackedItems(string pack_no)
        {
            TPCResult<List<CItem>> result = new TPCResult<List<CItem>>();

            string sql = "select pack_id, tray_id, p_date, status from pnt_pack where pack_id = @pack_no and (status = 0 or status = 1) order by tray_id";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@pack_no", DbType.AnsiString, pack_no));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            result.Value = new List<CItem>();
            foreach (DataRow row in dt.Value.Rows)
            {
                CItem item = new CItem();
                item.PCode = row["pack_id"].ToString();
                item.Code = row["tray_id"].ToString();
                item.Date = Convert.ToDateTime(row["p_date"]);
                item.Status = Convert.ToInt32(row["status"]);

                result.Value.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 检查pack code是否合法
        /// 这里主要用于重新装箱时，扫描已装pack编码
        /// 所以需要从pnt_pack中查找该编号是否存在
        /// </summary>
        /// <param name="pack_no">Pack编码</param>
        /// <returns></returns>
        public TPCResult<bool> CheckPackNo(string pack_no, bool is_repack)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //1. 检查在pack表中是否存在?且状态为1（已完成）
            string sql = "select count(pack_id) as qty from pnt_pack where pack_id = @code and status = 1";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, pack_no));

            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            int qty = Convert.ToInt32(dt.Value.Rows[0]["qty"]);
            if (qty == 0)
            {
                result.Value = false;
                return result;
            }
            //2. 检查在carton表中不存在状态为0或1的
            sql = "select count(pack_id) as qty from pnt_carton where pack_id = @code and (status = 0 or status = 1)";
            dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }
            qty = Convert.ToInt32(dt.Value.Rows[0]["qty"]);
            if (is_repack)
                result.Value = (qty > 0);
            else
                result.Value = (qty == 0);

            return result;
        }

        /// <summary>
        /// 完成捆包,将数据状态设置为完成
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public TPCResult<bool> CompletedPack(List<CItem> items)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            List<string> sqls = new List<string>();
            foreach (CItem item in items)
            {
                string sql = "update pnt_pack set status = 1 where pack_id = '" + item.PCode + "' and tray_id = '" + item.Code + "'";
                sqls.Add(sql);
            }
            result = Database.Execute(sqls);

            return result;
        }

        
        #endregion

        #region 装箱操作
        /// <summary>
        /// 从未完成装箱的Carton中，按照Pack的条码获得状态为0（未完成）的Carton条码
        /// 为了方便客户增加status为1的内容，只是不能打印
        /// </summary>
        /// <param name="tray_no"></param>
        /// <returns></returns>
        public TPCResult<string> GetCartonNoByPackNo(string pack_no)
        {
            TPCResult<string> result = new TPCResult<string>();

            string sql = "select carton_id from pnt_carton where pack_id = @code and (status = 0 or status = 1)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, pack_no));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);

            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            if (dt.Value.Rows.Count == 0)
            {
                result.State = RESULT_STATE.NG;
                result.Message = string.Format("Not found carton id by pack id:{0}", pack_no);
                return result;
            }

            result.Value = dt.Value.Rows[0]["carton_id"].ToString();
            return result;
        }

        /// <summary>
        /// 装箱时，扫描了Pack后，将记录写入数据库
        /// 状态为0（未完成）
        /// </summary>
        /// <param name="pack_no"></param>
        /// <param name="tray_no"></param>
        /// <param name="p_date"></param>
        /// <returns></returns>
        public TPCResult<bool> WriteCartonPack(string carton_no, string pack_no, DateTime p_date)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            string sql = "insert into pnt_carton values (@cartonid, @packid, @pdate, 0)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@cartonid", DbType.AnsiString, carton_no));
            parameters.Add(new DbParameter("@packid", DbType.AnsiString, pack_no));
            parameters.Add(new DbParameter("@pdate", DbType.DateTime, p_date));

            result = Database.Execute(sql, parameters);

            return result;
        }

        public TPCResult<System.Data.DataTable> CheckBin(string id)
        {
            StringBuilder sql = new StringBuilder();
            string mode = id.Substring(0, 1);
            switch (mode)
            {
                case "M":
                    sql.AppendLine("select distinct bin");
                    sql.AppendLine("from t_module");
                    sql.AppendLine("where tray_id = @id");
                    break;
                case "B":
                    sql.AppendLine("select distinct mm.bin");
                    sql.AppendLine("from pnt_pack as pp");
                    sql.AppendLine("inner join t_module as mm");
                    sql.AppendLine("on pp.tray_id = mm.tray_id");
                    sql.AppendLine("where pp.status=1");
                    sql.AppendLine("and pp.pack_id =@id");
                    break;
                case "H":
                    //左连接left join改成内连接inner join，防止空值
                    //< param name = "status" > 状态:0 = BEGIN; 1 = COMPLETED; 2 = CANCELED </ param >
                    //public TPCResult<bool> SetManagerData(PACK_MODE mode, string id, string user, int qty, PACK_ACTION action, PACK_STATUS status)
                    sql.AppendLine("select distinct bin");
                    sql.AppendLine("from pnt_carton as cc");
                    sql.AppendLine("inner join pnt_pack as pp");
                    sql.AppendLine("on cc.pack_id=pp.pack_id");
                    sql.AppendLine("inner join t_module as mm");
                    sql.AppendLine("on pp.tray_id=mm.tray_id");
                    sql.AppendLine("where cc.status=1");
                    sql.AppendLine("and pp.status=1");
                    sql.AppendLine("and cc.carton_id=@id");
                    break;
            }
            List<DbParameter> parameters = new List<DbParameter>();            
            parameters.Add(new DbParameter("@id", DbType.AnsiString, id));


            TPCResult<System.Data.DataTable> result = new TPCResult<System.Data.DataTable>();
            TPCResult<DataTable> dt = Database.Query(sql.ToString(), parameters);

            if (dt.State == RESULT_STATE.NG)
            {
                result.Message = dt.Message;
                result.State = RESULT_STATE.NG;
                return result;
            }
            //没有记录
            if (dt.Value.Rows.Count == 0)
            {
                result.State = RESULT_STATE.NG;
                result.Message = string.Format("Not found module bin: {0}", id);
                return result;
            }
            result.Value = dt.Value;
            return result;
        }

        /// <summary>
        /// 检索以装箱的pack信息
        /// </summary>
        /// <param name="pack_no">Carton Code</param>
        /// <returns></returns>
        public TPCResult<List<CItem>> GetCartonedItems(string carton_no)
        {
            TPCResult<List<CItem>> result = new TPCResult<List<CItem>>();

            string sql = "select carton_id, pack_id, p_date, status from pnt_carton where carton_id = @carton_no and (status = 0 or status = 1) order by pack_id";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@carton_no", DbType.AnsiString, carton_no));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            result.Value = new List<CItem>();
            foreach (DataRow row in dt.Value.Rows)
            {
                CItem item = new CItem();
                item.PCode = row["carton_id"].ToString();
                item.Code = row["pack_id"].ToString();
                item.Date = Convert.ToDateTime(row["p_date"]);
                item.Status = Convert.ToInt32(row["status"]);

                result.Value.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 检查carton code是否合法
        /// 这里主要用于重新装箱时，扫描已装carton编码
        /// 所以需要从pnt_carton中查找该编号是否存在
        /// </summary>
        /// <param name="pack_no">carton编码</param>
        /// <returns></returns>
        public TPCResult<bool> CheckCartonNo(string carton_no, bool is_repack)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //1. 检查在carton表中是否存在且状态为1（已完成）
            string sql = "select count(carton_id) as qty from pnt_carton where carton_id = @code and status = 1";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, carton_no));

            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            int qty = Convert.ToInt32(dt.Value.Rows[0]["qty"]);
            if (qty == 0)
            {
                result.Value = false;
                return result;
            }
            //2. 检查在pallet表中是否不存在且状态为0或1
            sql = "select count(carton_id) as qty from pnt_pallet where carton_id = @code and (status = 0 or status = 1)";
            dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            qty = Convert.ToInt32(dt.Value.Rows[0]["qty"]);
            if (is_repack)
                result.Value = (qty > 0);
            else 
                result.Value = (qty == 0);

            return result;
        }

        /// <summary>
        /// 完成装箱,将数据状态设置为完成
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public TPCResult<bool> CompletedCarton(List<CItem> items)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            List<string> sqls = new List<string>();
            foreach (CItem item in items)
            {
                string sql = "update pnt_carton set status = 1 where carton_id = '" + item.PCode + "' and pack_id = '" + item.Code + "'";
                sqls.Add(sql);
            }
            result = Database.Execute(sqls);

            return result;
        }
        #endregion

        #region 装盘操作
        /// <summary>
        /// 从未完成装箱的Pallet中，按照Carton的条码获得状态为0（未完成）的Pallet条码
        /// </summary>
        /// <param name="carton_no"></param>
        /// <returns></returns>
        public TPCResult<string> GetPalletNoByCartonNo(string carton_no)
        {
            TPCResult<string> result = new TPCResult<string>();

            string sql = "select pallet_id from pnt_pallet where carton_id = @code and (status = 0 or status = 1)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@code", DbType.AnsiString, carton_no));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);

            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            if (dt.Value.Rows.Count == 0)
            {
                result.State = RESULT_STATE.NG;
                result.Message = string.Format("Not found pallet id by carton id:{0}", carton_no);
                return result;
            }

            result.Value = dt.Value.Rows[0]["pallet_id"].ToString();
            return result;
        }

        /// <summary>
        /// 装盘时，扫描了Carton后，将记录写入数据库
        /// 状态为0（未完成）
        /// </summary>
        /// <param name="pallet_no"></param>
        /// <param name="carton_no"></param>
        /// <param name="p_date"></param>
        /// <returns></returns>
        public TPCResult<bool> WritePalletCarton(string pallet_no, string carton_no, DateTime p_date)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            string sql = "insert into pnt_pallet values (@palletid, @cartonid, @pdate, 0)";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@palletid", DbType.AnsiString, pallet_no));
            parameters.Add(new DbParameter("@cartonid", DbType.AnsiString, carton_no));
            parameters.Add(new DbParameter("@pdate", DbType.DateTime, p_date));

            result = Database.Execute(sql, parameters);

            return result;
        }

        /// <summary>
        /// 检索以装盘的carton信息
        /// </summary>
        /// <param name="pallet_no">pallet Code</param>
        /// <returns></returns>
        public TPCResult<List<CItem>> GetPalletedItems(string pallet_no)
        {
            TPCResult<List<CItem>> result = new TPCResult<List<CItem>>();

            string sql = "select pallet_id, carton_id, p_date, status from pnt_pallet where pallet_id = @pallet_no and (status = 0 or status = 1) order by carton_id";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@pallet_no", DbType.AnsiString, pallet_no));
            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            result.Value = new List<CItem>();
            foreach (DataRow row in dt.Value.Rows)
            {
                CItem item = new CItem();
                item.PCode = row["pallet_id"].ToString();
                item.Code = row["carton_id"].ToString();
                item.Date = Convert.ToDateTime(row["p_date"]);
                item.Status = Convert.ToInt32(row["status"]);

                result.Value.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 检查pallet code是否合法
        /// 这里主要用于重新装箱时，扫描已装pallet编码
        /// 所以需要从pnt_pallet中查找该编号是否存在
        /// </summary>
        /// <param name="pack_no">carton编码</param>
        /// <returns></returns>
        public TPCResult<bool> CheckPalletNo(string pallet_no, bool is_repack)
        {
            //这里预留接口，目前不需要实现，因为没有实际应用
            TPCResult<bool> result = new TPCResult<bool>();
            result.Value = true;
            return result;
        }

        /// <summary>
        /// 完成装盘,将数据状态设置为完成
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public TPCResult<bool> CompletedPallet(List<CItem> items)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            List<string> sqls = new List<string>();
            foreach (CItem item in items)
            {
                string sql = "update pnt_pallet set status = 1 where pallet_id = '" + item.PCode + "' and carton_id = '" + item.Code + "'";
                sqls.Add(sql);
            }
            result = Database.Execute(sqls);

            return result;
        }
        #endregion

        #region 拆包
        /// <summary>
        /// 拆包只针对已经完成的
        /// </summary>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public TPCResult<List<CItem>> GetItemsByPCode(string pcode)
        {
            TPCResult<List<CItem>> result = new TPCResult<List<CItem>>();
            string sql = "select name, pcode, ccode, p_date, status from vw_packing where pcode = @pcode "
                       + "and status = 1 "
                       + "order by ccode";
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@pcode", DbType.AnsiString, pcode));

            TPCResult<DataTable> dt = Database.Query(sql, parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            result.Value = new List<CItem>();
            foreach (DataRow row in dt.Value.Rows)
            {
                CItem item = new CItem();
                item.Name = row["name"].ToString();
                item.PCode = row["pcode"].ToString();
                item.Code = row["ccode"].ToString();
                item.Date = Convert.ToDateTime(row["p_date"]);
                item.Status = Convert.ToInt32(row["status"]);

                result.Value.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 单层拆包
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public TPCResult<bool> UnpackItemsBySingle(List<CItem> items, string username)
        {
            TPCResult<bool> result = new TPCResult<bool>();

            List<string> sqls = new List<string>();
            foreach (CItem item in items)
            {
                string pfield = "";
                string cfield = "";
                string name = GetTableFieldName(item.Name, ref pfield, ref cfield);

                string sql = "update " + name + " set status = ";
                if (item.Status == 2)
                    sql += "2";
                else
                    sql += "0";
                sql = sql + " where " + pfield + " = '" + item.PCode
                           + "' and " + cfield + " = '" + item.Code + "'";
                sqls.Add(sql);
            }
            //更新pnt_mng
            string pcode = items[0].PCode;
            string pack_name = items[0].Name;
            string mngSql = string.Format("insert into pnt_mng values ('{0}', '{1}', 'c', now(), {2}, '{3}', 2, '')", pcode, pack_name, items.Count, username);
            sqls.Add(mngSql);

            result = Database.Execute(sqls);

            return result;
        }

        /// <summary>
        /// 深度拆包，按照自选明细拆包掉用
        /// </summary>
        /// <param name="items"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public TPCResult<bool> UnpackItemByDeep(List<CItem> items, string username)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            foreach (CItem item in items)
            {
                if (item.Status != 2)
                    continue;
                string mode = GetModeFromTableName(item.Name);
                if (mode.Length == 0)
                {
                    result.State = RESULT_STATE.NG;
                    result.Message = "Unknown mode by canceled item.";
                    return result;
                }
                int qty = GetQuantityByMode(mode);
                if (qty == -1)
                {
                    result.State = RESULT_STATE.NG;
                    result.Message = string.Format("Not found quantity in this mode:{0}", mode);
                    return result;
                }

                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("@mode", DbType.AnsiString, mode));
                parameters.Add(new DbParameter("@code", DbType.AnsiString, item.Code));
                parameters.Add(new DbParameter("@qty", DbType.Int32, qty));
                parameters.Add(new DbParameter("@user", DbType.AnsiString, username));

                TPCResult<string> ret = Database.ExecuteSP("fn_deep_unpack", parameters);
                if (ret.State == RESULT_STATE.NG)
                {
                    result.Message = ret.Message;
                    result.State = RESULT_STATE.NG;
                    return result;
                }
            }
            return result;
        }
        /// <summary>
        /// 深度拆包，按照顶级项目全部拆包掉用
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="code"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public TPCResult<bool> UnpackItemByDeep(string mode, string code, string username)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            
            int qty = GetQuantityByMode(mode);
            if (qty == -1)
            {
                result.State = RESULT_STATE.NG;
                result.Message = string.Format("Not found quantity in this mode:{0}", mode);
                return result;
            }

            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@mode", DbType.AnsiString, mode));
            parameters.Add(new DbParameter("@code", DbType.AnsiString, code));
            parameters.Add(new DbParameter("@qty", DbType.Int32, qty));
            parameters.Add(new DbParameter("@user", DbType.AnsiString, username));

            TPCResult<string> ret = Database.ExecuteSP("fn_deep_unpack", parameters);
            if (ret.State == RESULT_STATE.NG)
            {
                result.Message = ret.Message;
                result.State = RESULT_STATE.NG;
                return result;
            }
            return result;
        }
        /// <summary>
        /// 通过表名获得拆包的模式：pack, carton, pallet
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetModeFromTableName(string name)
        {
            if (name.ToLower().Equals("pnt_pallet"))
                return "carton";
            else if (name.ToLower().Equals("pnt_carton"))
                return "pack";
            else
                return "";
        }
        /// <summary>
        /// 根据模式从配置信息中获取数量
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private int GetQuantityByMode(string mode)
        {
            if (mode.Equals("pack"))
                return LabelPrintGlobal.g_Config.PackTrays;
            else if (mode.Equals("carton"))
                return LabelPrintGlobal.g_Config.CartonPack;
            else if (mode.Equals("pallet"))
                return LabelPrintGlobal.g_Config.PalletCarton;
            else
                return -1;
        }

        /// <summary>
        /// 根据表名，获得结构父子的字段名称
        /// </summary>
        /// <param name="name">表名</param>
        /// <param name="pfield">父字段名称</param>
        /// <param name="cfield">子字段名称</param>
        /// <returns>表名</returns>
        protected string GetTableFieldName(string name, ref string pfield, ref string cfield)
        {
            pfield = "";
            cfield = "";

            string[][] TABLE_FIELD_MAPPING = new string[][]{ new string[]{"pnt_pack", "pack_id", "tray_id"},
                                                            new string[]{"pnt_carton", "carton_id", "pack_id"},
                                                            new string[]{"pnt_pallet", "pallet_id", "carton_id"}};
            foreach (string[] item in TABLE_FIELD_MAPPING)
            {
                if (item[0].Equals(name))
                {
                    pfield = item[1];
                    cfield = item[2];
                    return item[0];
                }
            }
            return "";
        }
        #endregion

        #region 富士康新标签
        public TPCResult<List<List<string>>> GetFXZZ_Data(string pcode)
        {
            StringBuilder sql=new StringBuilder();
            string mode = pcode.Substring(0, 1);
            switch (mode)
            {
                case "B":
                    sql.AppendLine("select sum(count)over(partition by 1),substring(lot,4,4),substring(lot,1,7)");
                    sql.AppendLine("from");
                    sql.AppendLine("(select lot,count(lot) from pnt_pack as aa");
                    sql.AppendLine("left join t_module as bb on aa.tray_id = bb.tray_id");
                    sql.AppendLine("where aa.pack_id = @pcode");
                    sql.AppendLine("and aa.status = '1'");
                    sql.AppendLine("group by lot)as tt");
                    sql.AppendLine("order by count desc limit 1");
                    break;
                case "H":
                    sql.AppendLine("select sum(count)over(partition by 1),substring(lot,4,4),substring(lot,1,7)");
                    sql.AppendLine("from");
                    sql.AppendLine("(select lot,count(lot) from pnt_carton as aa");
                    sql.AppendLine("left join pnt_pack as bb on aa.pack_id = bb.pack_id");
                    sql.AppendLine("left join t_module as cc on bb.tray_id = cc.tray_id");
                    sql.AppendLine("where aa.carton_id = @pcode");
                    sql.AppendLine("and aa.status = '1'");
                    sql.AppendLine("and bb.status = '1'");
                    sql.AppendLine("group by lot)as tt");
                    sql.AppendLine("order by count desc limit 1");
                    break;
                case "P":
                    sql.AppendLine("select sum(count)over(partition by 1),substring(lot,4,4),substring(lot,1,7)");
                    sql.AppendLine("from");
                    sql.AppendLine("(select lot,count(lot) from pnt_pallet as aa");
                    sql.AppendLine("left join pnt_carton as bb on aa.carton_id = bb.carton_id");
                    sql.AppendLine("left join pnt_pack as cc on bb.pack_id = cc.pack_id");
                    sql.AppendLine("left join t_module as dd on cc.tray_id = dd.tray_id");
                    sql.AppendLine("where aa.pallet_id = @pcode");
                    sql.AppendLine("and aa.status = '1'");
                    sql.AppendLine("and bb.status = '1'");
                    sql.AppendLine("and cc.status = '1'");
                    sql.AppendLine("group by lot)as tt");
                    sql.AppendLine("order by count desc limit 1");
                    break;
            }
            TPCResult<List<List<string>>> result = new TPCResult<List<List<string>>>();
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(new DbParameter("@pcode", DbType.AnsiString, pcode));

            TPCResult<DataTable> dt = Database.Query(sql.ToString(), parameters);
            if (dt.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = dt.Message;
                return result;
            }

            result.Value = new List<List<string>>();
            foreach (DataRow row in dt.Value.Rows)
            {
                List<string> item = new List<string>();
                item.Add(row[0].ToString());
                item.Add(row[1].ToString());
                item.Add(row[2].ToString());
                result.Value.Add(item);
            }

            return result;
        }
        #endregion
    }
}