/// <summary>
/// 数据库连接
/// </summary>
/// <remarks>
/// 数据库连接.包含于数据库的操作方法。
/// Copyright(C) 2017-，Technopro China
/// </remarks>
/// 
/// 创建日期: 2017-03-09
/// 版   本: 0.0.01
/// 作   者: Magic.Zhu
/// 
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TPCCommon.Common;

namespace TPCCommon.Database
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    /// <remarks>
    /// 数据库连接.包含于数据库的操作方法。
    /// </remarks>
    public class TPCDatabase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="creator">数据库连接创建器</param>
        public TPCDatabase(IFDatabaseCreator creator) 
        {
            m_Creator = creator;
            m_Connection = m_Creator.CreateConnection();
        }

        /// <summary>
        /// 数据库连接创建器
        /// </summary>
        protected IFDatabaseCreator m_Creator = null;
        public IFDatabaseCreator Creator
        {
            get { return m_Creator; }
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private IDbConnection m_Connection = null;

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <returns>是否成功打开</returns>
        public bool Open()
        {
            try
            {
                //检查数据库连接是否为空
                if (m_Connection == null)
                {
                    //为空则通过创建器创建一个
                    m_Connection = m_Creator.CreateConnection();
                }
                //打开连接
                m_Connection.Open();
                //检查连接状态
                if (m_Connection.State == ConnectionState.Open)
                {
                    //成功
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, "Open database OK.");
                    return true;
                }
                else
                {
                    //失败
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, "Open database NG.");
                    return false;
                }
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            try
            {
                m_Connection.Close();
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>事务对象</returns>
        public IDbTransaction BeginTrans()
        {
            IDbTransaction ts = null;
            try
            {
                ts = m_Connection.BeginTransaction();
            }
            catch (Exception e)
            {
                ts = null;
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
            }
            return ts;
        }

        /// <summary>
        /// 创建数据库命令
        /// </summary>
        /// <returns>数据库命令</returns>
        public IDbCommand CreateCommand()
        {
            IDbCommand cmd = null;
            try
            {
                cmd = m_Connection.CreateCommand();
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
            }
            return cmd;
        }

        /// <summary>
        /// 创建数据库命令
        /// </summary>
        /// <param name="sql">数据库命令SQL文</param>
        /// <returns>数据库命令</returns>
        public IDbCommand CreateCommand(string sql)
        {
            IDbCommand cmd = CreateCommand();
            if (cmd != null)
            {
                cmd.CommandText = sql;
            }
            return cmd;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <returns>执行成功与否</returns>
        public TPCResult<bool> Execute(string sql)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return result;
            }
            try
            {
                //构建数据库命令
                IDbCommand cmd = CreateCommand(sql);
                if (cmd == null)
                {
                    result.State = RESULT_STATE.NG;
                    result.Message = "Create SQL Command Failed.";
                    return result;
                }

                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute SQL:{0}", sql));
                //执行SQL
                cmd.ExecuteNonQuery();
                result.Value = true;
                return result;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>执行成功与否</returns>
        public TPCResult<bool> Execute(string sql, List<DbParameter> parameters)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return result;
            }
            try
            {
                //构造数据库指令
                IDbCommand cmd = CreateCommand(sql);
                if (cmd == null)
                {
                    result.State = RESULT_STATE.NG;
                    result.Message = "Create SQL Command Failed.";
                    return result;
                }

                //构造数据库指令执行的参数列表
                StringBuilder param_logger = new StringBuilder();
                foreach (DbParameter param in parameters)
                {
                    IDbDataParameter dp = cmd.CreateParameter();
                    dp.DbType = param.DbType;
                    dp.ParameterName = param.ParamName;
                    dp.Value = param.Value;

                    if (param_logger.Length > 0)
                    {
                        param_logger.Append(",");
                    }
                    param_logger.Append(string.Format("{{ParamName:{0}, DbType:{1}, Value:{2}. }}", param.ParamName, param.DbType.ToString(), (param.Value == null) ? "NULL" : param.Value.ToString()));
                    cmd.Parameters.Add(dp);
                }

                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute SQL:{0} Params:{1}", sql, param_logger.ToString()));
                //执行SQL
                cmd.ExecuteNonQuery();
                result.Value = true;
                return result;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 在一个事务下批量执行带参数的SQL语句
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public TPCResult<bool> Execute(List<KeyValuePair<string, List<DbParameter>>> sqls)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            IDbTransaction ts = null;
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return result;
            }
            try
            {
                ts = BeginTrans();
                foreach (KeyValuePair<string, List<DbParameter>> item in sqls)
                {
                    //构造数据库指令
                    IDbCommand cmd = CreateCommand(item.Key);
                    if (cmd == null)
                    {
                        result.State = RESULT_STATE.NG;
                        result.Message = "Create SQL Command Failed.";
                        return result;
                    }
                    cmd.Transaction = ts;

                    //构造数据库指令执行的参数列表
                    StringBuilder param_logger = new StringBuilder();
                    foreach (DbParameter param in item.Value)
                    {
                        IDbDataParameter dp = cmd.CreateParameter();
                        dp.DbType = param.DbType;
                        dp.ParameterName = param.ParamName;
                        dp.Value = param.Value;

                        if (param_logger.Length > 0)
                        {
                            param_logger.Append(",");
                        }
                        param_logger.Append(string.Format("{{ParamName:{0}, DbType:{1}, Value:{2}. }}", param.ParamName, param.DbType.ToString(), (param.Value == null) ? "NULL" : param.Value.ToString()));
                        cmd.Parameters.Add(dp);
                    }

                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute SQL:{0} Params:{1}", item.Key, param_logger.ToString()));
                    //执行SQL
                    cmd.ExecuteNonQuery();
                }
                ts.Commit();
                result.Value = true;
                return result;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;

                if (ts != null)
                {
                    ts.Rollback();
                }
                return result;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 执行一组SQL语句
        /// </summary>
        /// <param name="sqls">待执行的SQL语句列表</param>
        /// <returns></returns>
        public TPCResult<bool> Execute(List<string> sqls)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return result;
            }
            //开启事务
            IDbTransaction ts = BeginTrans();
            try
            {
                StringBuilder sql_logger = new StringBuilder();
                //遍历SQL语句列表，构建每条语句的数据库指令
                foreach (string sql in sqls)
                {
                    IDbCommand cmd = CreateCommand(sql);
                    if (cmd == null)
                    {
                        result.State = RESULT_STATE.NG;
                        result.Message = "Create SQL Command Failed.";
                        return result;
                    }
                    cmd.Transaction = ts;
                    if (sql_logger.Length > 0)
                    {
                        sql_logger.Append(",");
                    }
                    sql_logger.Append(string.Format("[{0}]", sql));
                    cmd.ExecuteNonQuery();
                }

                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute SQLS : {0}", sql_logger.ToString()));
                //完成事务
                ts.Commit();
                result.Value = true;
                return result;
            }
            catch (Exception e)
            {
                ts.Rollback();
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 通过SQL语句查询数据库内容
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>查询的数据结果列表</returns>
        public TPCResult<DataTable> Query(string sql)
        {
            TPCResult<DataTable> result = new TPCResult<DataTable>();
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return result;
            }

            try
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute SQL:{0}", sql));
                //创建SQL指令
                IDbCommand cmd = CreateCommand(sql);
                //创建数据适配器
                IDbDataAdapter da = m_Creator.CreateDataAdapter();
                da.SelectCommand = cmd;
                //执行查询并将结果填入DataSet
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables.Count == 0)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Execute query failed.");
                    result.State = RESULT_STATE.NG;
                    result.Message = "Execute query failed.";
                    return result;
                }

                result.Value = ds.Tables[0];

                return result;
                
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 通过SQL语句查询数据库内容
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>查询的数据结果列表</returns>
        public TPCResult<DataTable> Query(string sql, List<DbParameter> parameters)
        {
            TPCResult<DataTable> result = new TPCResult<DataTable>();
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return null;
            }

            try
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute SQL:{0}", sql));
                //创建SQL指令
                IDbCommand cmd = CreateCommand(sql);
                //构建数据库指令参数列表
                StringBuilder param_logger = new StringBuilder();
                foreach (DbParameter param in parameters)
                {
                    IDbDataParameter dp = cmd.CreateParameter();
                    dp.ParameterName = param.ParamName;
                    dp.DbType = param.DbType;
                    dp.Value = param.Value;
                    cmd.Parameters.Add(dp);
                    if (param_logger.Length > 0)
                    {
                        param_logger.Append(",");
                    }
                    param_logger.Append(string.Format("{{ParamName:{0}, DbType:{1}, Value:{2}. }}", param.ParamName, param.DbType.ToString(), (param.Value == null) ? "NULL" : param.Value.ToString()));
                }
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("\t\tParameters:{0}", param_logger.ToString()));

                //创建数据适配器
                IDbDataAdapter da = m_Creator.CreateDataAdapter();
                da.SelectCommand = cmd;
                //执行查询并将结果填入DataSet
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables.Count == 0)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Execute query failed.");
                    result.State = RESULT_STATE.NG;
                    result.Message = "Execute query failed.";
                    return result;
                }


                result.Value = ds.Tables[0];
                return result;

            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 执行存储过程并获得结果
        /// </summary>
        /// <param name="name">Postgres 函数名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>存储过程结果</returns>
        public TPCResult<string> ExecuteSP(string name, List<DbParameter> parameters)
        {
            TPCResult<string> result = new TPCResult<string>();
            //打开连接
            if (!Open())
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Open Database Failed.");
                result.State = RESULT_STATE.NG;
                result.Message = "Open Database Failed.";
                return null;
            }

            try
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Execute Function:{0}", name));
                //创建SQL指令
                IDbCommand cmd = CreateCommand(name);
                cmd.CommandType = CommandType.StoredProcedure;
                //构建数据库指令参数列表
                StringBuilder param_logger = new StringBuilder();
                foreach (DbParameter param in parameters)
                {
                    IDbDataParameter dp = cmd.CreateParameter();
                    dp.ParameterName = param.ParamName;
                    dp.DbType = param.DbType;
                    dp.Value = param.Value;
                    cmd.Parameters.Add(dp);
                    if (param_logger.Length > 0)
                    {
                        param_logger.Append(",");
                    }
                    param_logger.Append(string.Format("{{ParamName:{0}, DbType:{1}, Value:{2}. }}", param.ParamName, param.DbType.ToString(), (param.Value == null) ? "NULL" : param.Value.ToString()));
                }
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("\t\tParameters:{0}", param_logger.ToString()));

                IDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result.Value = dr.IsDBNull(0) ? "" : dr.GetValue(0).ToString();
                }

                return result;

            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
            finally
            {
                Close();
            }
        }
    }
}
