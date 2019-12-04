
using Npgsql;
using System;
using System.Configuration;
using System.Data;

using System.Xml.Linq;
using System.Linq;

namespace OUT
{
	public class DBHelp
	{
        //static string DbConnectstring = ConfigurationManager.AppSettings["conn"].ToString();

        //共用一个xml配置文件        
        static string DbConnectstring = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "database.xml")
            .Descendants("database").FirstOrDefault().Attribute("connection").Value;
        NpgsqlConnection con;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStr"></param>
        public DataTable ExecuteDataTable(string sqlStr, ref string errMsg)
		{
			DataTable dt = new DataTable();
			using (con = new NpgsqlConnection(DbConnectstring))
			{
				try
				{
					NpgsqlDataAdapter da = new NpgsqlDataAdapter(sqlStr, con);
					da.Fill(dt);
					return dt;
				}
				catch (Exception ex)
				{

					errMsg = ex.Message;
					return dt;
				}
			}
		}



		public int ExecuteNoneQuery(string sqlStr, ref string errMsg)
		{
			var i = -1;
			using (con = new NpgsqlConnection(DbConnectstring))
			{
				try
				{
					con.Open();
					NpgsqlCommand cmd = new NpgsqlCommand(sqlStr, con);
					i = cmd.ExecuteNonQuery();
					return i;
				}
				catch (Exception ex)
				{

					errMsg = ex.Message;
					return i;
				}



				//NpgsqlTransaction tran;
				//NpgsqlCommand cmd = null;
				//cmd.Connection = con;
				//tran = cmd.Transaction;
				//try
				//{
				//    //cmd.CommandText = "insert into print_num values(" + maxNum.ToString() + ",'" + date.Rows[0]["date"] + "')";
				//    //cmd.ExecuteNonQuery();

				//    cmd = new NpgsqlCommand("insert into print_num values(" + maxNum.ToString() + ",'" + date.Rows[0]["date"] + "')", con);
				//    cmd.ExecuteNonQuery();

				//    tran.Commit();
				//}
				//catch
				//{
				//    tran.Rollback();
				//}
			}


		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string getServerDate(int type)
		{
			string errMsg = "";
			string sqlStrServerDate = "";

			DBHelp db = new DBHelp();

			// 取服务器时间
			if (type == 1)
			{
				sqlStrServerDate = "select to_char(current_date,'yyyyMMdd') as currentDate";
			}
			else
			{
				sqlStrServerDate = "select to_char(current_date,'yyyy-MM-dd') as currentDate";

			}

			DataTable date = ExecuteDataTable(sqlStrServerDate, ref errMsg);

			return date.Rows[0]["currentDate"].ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string getCurrentDayNo(string date, string ItemType)
		{
			string errMsg = "";
			// 取序号
			string sqlStr = "select * from fn_apply_no('" + ItemType + "','" + date + "')";
			DataTable dt = ExecuteDataTable(sqlStr, ref errMsg);

			return dt.Rows[0]["fn_apply_no"].ToString();
		}
	}
}
