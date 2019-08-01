using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OUT
{
	public partial class Form_Invoice : Form
	{
		
		public Form_Invoice()
		{
			InitializeComponent();
			
		}
		public string logingname { get; set; }

		DBHelp db = new DBHelp();
		private void btn_search_Click(object sender, EventArgs e)
		{
			string errMsg = "";
			string kb2 = this.kaban2.Text.ToString().Trim();
			string n2 = this.no2.Text.ToString().Trim();

			string sqlStr = " select a.pallet_id,a.p_date,b.invoice_code"
							+ " ,to_char(b.sales_date, 'yyyy-MM-dd HH24:MI:SS') sales_date from"
							+ " (select pallet_id, max(to_char(p_date, 'yyyy-MM-dd HH24:MI:SS')) p_date from pnt_pallet"
							+ " where status = '1'";
			if (dateShift_cbx.Checked == true)

			{
				string sd = start_date.Value.ToString("yyyy-MM-dd 00:00:00");
				string ed = end_date.Value.ToString("yyyy-MM-dd 23:59:59");
				sqlStr += " and p_date >= '" + sd + "'and  p_date <= '" + ed + "'";
			}

			if (kb2 != "") sqlStr += " and pallet_id like '%" + kb2 + "%'";


			sqlStr += " group by pallet_id) as a"
							+ " left join invoice as b on a.pallet_id = b.pallet_id";
			if (n2 != "") sqlStr += " where b.invoice_code like '%" + n2 + "%'";
			sqlStr += " order by a.p_date desc";
	

			DataTable dt2 = db.ExecuteDataTable(sqlStr, ref errMsg);
			DataTable dt = new DataTable();
			DataRow newRow = null;

			
			dt.Columns.Add("序号");
			//dt.Columns.Add("用户");
			dt.Columns.Add("卡板号");
			
			dt.Columns.Add("登记时间");
			dt.Columns.Add("发票编号");
			dt.Columns.Add("出货时间");

			for (int i = 0; i < dt2.Rows.Count; i++)
			{
			    newRow = dt.NewRow();
				newRow["序号"] = i + 1;
				//newRow["用户"] = dt2.Rows[i]["user_id"].ToString();
				newRow["卡板号"] = dt2.Rows[i]["pallet_id"].ToString();
				newRow["出货时间"] = dt2.Rows[i]["sales_date"].ToString();
				newRow["登记时间"] = dt2.Rows[i]["p_date"].ToString();
				newRow["发票编号"] = dt2.Rows[i]["invoice_code"].ToString();
				dt.Rows.Add(newRow);
			}
			
			dataGridView1.DataSource = dt;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


		}

		private void button1_Click(object sender, EventArgs e)
		{
			string kb1 = this.kaban1.Text.ToString().Trim();
			string n1 = this.no1.Text.ToString().Trim() ;
			
			string errMsg = "";
			
			if (kb1 == "" || n1 == "") 
			{
				MessageBox.Show("带*号的是必填项");
				return;
			}
			//string sql = "select * from t_user where user_id = '" + userId + "'";
			//if (db.ExecuteNoneQuery(sql, ref errMsg) < 0)
			//{
			//	MessageBox.Show("");
			//	return;
			//}

			string sqlStr = "insert into invoice(pallet_id,sales_date,invoice_code)"
							+ " values('"+ kb1 + "',now(),'" + n1 + "')";
			int i = db.ExecuteNoneQuery(sqlStr, ref errMsg);

			if (i > 0)
			{
				MessageBox.Show("出货成功！");
			}
			else
			{
				MessageBox.Show(errMsg);
			}
		}
	}
}
