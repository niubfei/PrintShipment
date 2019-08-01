/// <summary>
/// 数据库连接创建器接口
/// </summary>
/// <remarks>
/// 数据库连接创建器接口
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
    /// 数据库连接创建器类型
    /// </summary>
    public enum DatabaseCategory { SQLServer, MySQL, PostgresSQL, OLEDB, Oracle }
    /// <summary>
    /// 数据库连接创建器接口
    /// </summary>
    public interface IFDatabaseCreator
    {
        string Name { get; set; }
        DatabaseCategory Category { get; set; }
        string ConnectionString { get; set; }
        IDbConnection CreateConnection();
        IDbDataAdapter CreateDataAdapter();
    }
}
