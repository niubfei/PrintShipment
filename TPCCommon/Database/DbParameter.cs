/// <summary>
/// 数据库参数说明
/// </summary>
/// <remarks>
/// 数据库参数说明
/// Copyright(C) 2017-，Technopro China
/// </remarks>
/// 
/// 创建日期: 2017-03-09
/// 版   本: 0.0.01
/// 作   者: Magic.Zhu
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TPCCommon.Database
{
    /// <summary>
    /// 数据库参数说明
    /// </summary>
    public class DbParameter
    {
        public DbParameter(string name, DbType type, object value)
        {
            m_ParamName = name;
            m_DbType = type;
            m_Value = value;
        }

        /// <summary>
        /// 参数值
        /// </summary>
        protected object m_Value = null;
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        /// <summary>
        /// 参数类型
        /// </summary>
        protected DbType m_DbType = DbType.AnsiString;
        public DbType DbType
        {
            get { return m_DbType; }
            set { m_DbType = value; }
        }

        /// <summary>
        /// 参数命名
        /// </summary>
        protected string m_ParamName = "";
        public string ParamName
        {
            get { return m_ParamName; }
            set { m_ParamName = value; }
        }

        /// <summary>
        /// 是否可以为空
        /// </summary>
        protected bool m_IsNullable = false;
        public bool IsNullable
        {
            get { return m_IsNullable; }
            set { m_IsNullable = value; }
        }
    }
}
