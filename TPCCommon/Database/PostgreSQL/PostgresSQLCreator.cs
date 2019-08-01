using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

namespace TPCCommon.Database.PostgreSQL
{
    public class PostgresSQLCreator : IFDatabaseCreator
    {
        protected string m_Name = "";
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        protected DatabaseCategory m_Category = DatabaseCategory.SQLServer;
        public DatabaseCategory Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }

        protected string m_ConnectionString = "";
        public string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        public IDbConnection CreateConnection()
        {
            IDbConnection connect = new NpgsqlConnection();
            connect.ConnectionString = m_ConnectionString;

            return connect;
        }

        public IDbDataAdapter CreateDataAdapter()
        {
            IDbDataAdapter da = new NpgsqlDataAdapter();
            return da;
        }
    }
}
