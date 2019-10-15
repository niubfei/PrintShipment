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
            sales_date.Enabled = sales_cbx.Checked;
            dtpSales_new.Enabled = chkSales.Checked;
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
            string sqlTime = sales_cbx.Checked ? "'" + sales_date.Value.ToString("yyyy-MM-dd hh:mm:ss") + "'" : "now()";
            string sqlStr = string.Format("insert into invoice(pallet_id,sales_date,invoice_code)"
                + " values('{0}',{1},'{2}')", kb1, sqlTime, n1);

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

        private void Sales_cbx_CheckedChanged(object sender, EventArgs e)
        {
            sales_date.Enabled = sales_cbx.Checked;
        }

        private void ChkSales_CheckedChanged(object sender, EventArgs e)
        {
            dtpSales_new.Enabled = chkSales.Checked;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            string pallet_id = txtPallet_old.Text;
            string invoice_new = txtInvoice_new.Text;
            string sqlTime = chkSales.Checked ? "'" + dtpSales_new.Value.ToString("yyyy-MM-dd hh:mm:ss") + "'" : "now()";
            string errMsg = "";

            if (pallet_id == "" || invoice_new == "")
            {
                MessageBox.Show("带*号的是必填项");
                return;
            }


//            string sqlStr = string.Format(
//@"UPDATE invoice
//SET pallet_id = '{0}',invoice_code = '{1}',sales_date={2}
//WHERE pallet_id = '{3}'
//AND invoice_code = '{4}'",
//pallet_new, invoice_new, sqlTime, pallet_old, invoice_old);

            string sqlStr = string.Format(
@"UPDATE invoice
SET invoice_code = '{0}',sales_date={1}
WHERE pallet_id = '{2}'",
invoice_new, sqlTime, pallet_id);

            int i = db.ExecuteNoneQuery(sqlStr, ref errMsg);

            if (i > 0)
            {
                MessageBox.Show("修改成功！");
            }
            else
            {
                if(errMsg!="")
                    MessageBox.Show(errMsg);
            }
        }
    }
}
