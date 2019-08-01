/// <summary>
/// 数据库工厂
/// </summary>
/// <remarks>
/// 通过数据库配置文件，配置数据库连接，并建立对应数据库连接实例
/// Copyright(C) 2017-，Technopro China
/// </remarks>
/// 
/// 创建日期: 2017-03-09
/// 版   本: 0.0.01
/// 作   者: Magic.Zhu
/// 
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using TPCCommon.Common;
using TPCCommon.Database.PostgreSQL;

namespace TPCCommon.Database
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    /// <remarks>
    /// 通过数据库配置文件，配置数据库连接，并建立对应数据库连接实例
    /// </remarks>
    public class DatabaseFactory
    {
        /// <summary>
        /// 单例模式，禁止外部实例化
        /// </summary>
        private DatabaseFactory() { }

        /// <summary>
        /// 单例的静态实例
        /// </summary>
        private static DatabaseFactory m_Instance = new DatabaseFactory();
        /// <summary>
        /// 获得单例实例
        /// </summary>
        /// <returns>数据库工厂实例</returns>
        public static DatabaseFactory GetInstance()
        {
            return m_Instance;
        }

        /// <summary>
        /// 数据库配置中的数据库连接创建器映射表
        /// </summary>
        private Dictionary<string, IFDatabaseCreator> m_Databases = new Dictionary<string, IFDatabaseCreator>();
        /// <summary>
        /// 将数据库连接创建器加入映射表
        /// </summary>
        /// <param name="creator">数据库连接创建器</param>
        public void AddDatabaseCreatory(IFDatabaseCreator creator)
        {
            //检查映射表中是否已经存在该命名的连接
            if (!m_Databases.ContainsKey(creator.Name))
            {
                m_Databases.Add(creator.Name, creator);
            }
        }
        /// <summary>
        /// 从映射表中移除命名数据库连接创建器
        /// </summary>
        /// <param name="name">数据库连接名称</param>
        public void RemoveDatabaseCreatory(string name)
        {
            m_Databases.Remove(name);
        }

        /// <summary>
        /// 已经注册的数据库连接数量
        /// </summary>
        public int Count
        {
            get { return m_Databases.Count; }
        }

        /// <summary>
        /// 数据库连接创建器
        /// </summary>
        /// <param name="name">数据库连接名称</param>
        /// <returns>数据库连接创建器</returns>
        public IFDatabaseCreator this[string name]
        {
            get { return m_Databases[name]; }
        }
        /// <summary>
        /// 数据库连接创建器
        /// </summary>
        /// <param name="index">数据库连接创建器索引</param>
        /// <returns>数据库连接创建器</returns>
        public IFDatabaseCreator this[int index]
        {
            get 
            {
                IList<IFDatabaseCreator> ary = m_Databases.Values.ToList();
                return ary[index];
            }
        }

        /// <summary>
        /// 通过数据库配置文件初始化数据库工厂
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <returns>是否成功初始化</returns>
        public bool LoadConfig(string config)
        {
            //检查数据库配置文件是否存在
            if (!File.Exists(config))
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Not found database config file.");
                return false;
            }

            try
            {
                //读取XML配置文件
                XmlDocument doc = new XmlDocument();
                doc.Load(config);
                //读取系统默认数据库连接配置
                XmlNode nodeSystem = doc.SelectSingleNode("databases/system_database");
                if (nodeSystem == null || nodeSystem.Attributes["name"] == null)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Not set system database.");
                    return false;
                }
                m_SystemDatabaseName = nodeSystem.Attributes["name"].Value;

                //遍历全部数据库连接配置
                XmlNodeList nodes = doc.SelectNodes("databases/database");
                m_Instance.m_Databases.Clear();
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["name"] == null || node.Attributes["connection"] == null || node.Attributes["category"] == null)
                    {
                        TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Database config file is not match format.");
                        return false;
                    }
                    string name = node.Attributes["name"].Value;
                    string connection = node.Attributes["connection"].Value;
                    string category = node.Attributes["category"].Value;

                    IFDatabaseCreator creator = null;
                    //按照不同的数据库类型生成不同的数据库连接创建器
                    if (category.ToLower().Equals("sqlserver"))
                    {
                    }
                    else if (category.ToLower().Equals("postgresql"))
                    {
                        creator = new PostgresSQLCreator();
                        creator.Category = DatabaseCategory.PostgresSQL;
                    }
                    else if (category.ToLower().Equals("mysql"))
                    { }
                    else if (category.ToLower().Equals("oracle"))
                    { }
                    else if (category.ToLower().Equals("oledb"))
                    { }
                    else
                    {
                        TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_WARNNING, "Unknow category database.");
                        continue;
                    }

                    if (creator != null)
                    {
                        creator.Name = name;
                        creator.ConnectionString = connection;
                        AddDatabaseCreatory(creator);
                        TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_INFORMATION, string.Format("Create database {0} category:{1}", name, category));
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                return false;
            }
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="name">数据库名称</param>
        /// <returns>数据库连接</returns>
        public TPCDatabase CreateDatabase(string name)
        {
            TPCDatabase database = null;
            if (m_Databases.ContainsKey(name))
            {
                database = new TPCDatabase(this[name]);
            }
            else
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, string.Format("Not found database creator by name {0}", name));
            }
            return database;
        }

        /// <summary>
        /// 系统默认数据库连接
        /// </summary>
        protected string m_SystemDatabaseName = "";
        public TPCDatabase CreateSystemDatabase()
        {
            return CreateDatabase(m_SystemDatabaseName);
        }
    }
}
