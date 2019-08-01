using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;
using LabelPrint.Model;
using TPCCommon.Common;
using TPCCommon.Database;
using TPCBarcode.LabelPrint;

namespace LabelPrint
{
    public partial class NewFXZZ : Form
    {
        public NewFXZZ()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            InitLanguage();
        }

        public void InitLanguage()
        {
            lbScan.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_CODE", LabelPrintGlobal.g_Language);
            //lbDate.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_DATE", LabelPrintGlobal.g_Language);
            lbQty.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_QTY", LabelPrintGlobal.g_Language);

            string[] columns_header = new string[] { "REPRINT_ITEM_NO", "REPRINT_ITEM_CODE", "REPRINT_ITEM_DATE" };
            for (int i = 0; i < lstItems.Columns.Count; i++)
            {
                ColumnHeader col = lstItems.Columns[i];
                string key = columns_header[i];
                col.Text = LanguageMapping.Instance.GetStaticMessage(key, LabelPrintGlobal.g_Language);
            }

            //btReprint.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_PRINT", LabelPrintGlobal.g_Language);
            btReprintNew.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_PRINT_NEW", LabelPrintGlobal.g_Language);
            btClose.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_CLOSE", LabelPrintGlobal.g_Language);
        }

        protected LabelDatabaseHelper m_Database = null;
        public LabelDatabaseHelper Database
        {
            get { return m_Database; }
            set { m_Database = value; }
        }

        protected PACK_MODE m_Mode = PACK_MODE.Pack;
        //public PACK_MODE Mode
        //{
        //    get { return m_Mode; }
        //    set { m_Mode = value; }
        //}

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (txtCode.Text.Length != 18)
                {
                    MessageBox.Show("号码长度错误", "包、箱、板标签号码:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string mode = txtCode.Text.Substring(0, 1);
                if (mode != "B" && mode != "H" && mode != "P")
                {
                    MessageBox.Show("标签号码格式错误", "包、箱、板标签号码:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //从数据库综合视图vw_packing中读取条码的有效性和所属模式
                lstItems.Items.Clear();
                TPCResult<List<CItem>> items = Database.GetItemsByPCode(txtCode.Text);
                if (items.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(items.Message);
                    return;
                }
                FillItems(items.Value);
            }
        }

        protected void FillItems(List<CItem> items)
        {
            //lstItems.Tag = items;
            foreach (CItem item in items)
            {
                ListViewItem it = lstItems.Items.Add(string.Format("{0}", lstItems.Items.Count + 1));
                //it.Tag = item;
                it.SubItems.Add(item.Code);
                it.SubItems.Add(item.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            //设定数量
            txtQty.Text = string.Format("{0}", lstItems.Items.Count);
            if (items.Count > 0)
            {
                if (items[0].Name.Equals("pnt_pack"))
                {
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PackTrays);
                    m_Mode = PACK_MODE.Pack;
                }
                else if (items[0].Name.Equals("pnt_carton"))
                {
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.CartonPack);
                    m_Mode = PACK_MODE.Carton;
                }
                else if (items[0].Name.Equals("pnt_pallet"))
                {
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PalletCarton);
                    m_Mode = PACK_MODE.Pallet;
                }
            }
        }


        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lstItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (lstItems.SelectedItems.Count == 0)
                return;
            StringBuilder ss = new StringBuilder();
            for (int i = 0; i < lstItems.SelectedItems.Count; i++)
            {
                ListViewItem item = lstItems.SelectedItems[i];
                if (i > 0)
                {
                    ss.Append("\r\n");
                }

                ss.Append(string.Format("{0},{1},{2}", item.Text, item.SubItems[1].Text, item.SubItems[2].Text));
            }

            Clipboard.SetText(ss.ToString());
        }

        private void btReprintNew_Click(object sender, EventArgs e)
        {
            if (lstItems.Items.Count == 0)
            {
                MessageBox.Show("No data,please check.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            reprintNew();
        }

        private void reprintNew()
        {
            //实际装包数<总数时，弹窗退出
            //if (lstItems.Items.Count < Convert.ToInt32(txtTotal.Text)) //txtTotal.Text可能是""，会出问题
            //{
            //    MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NOT_FULL_QUANTITY_ERROR"), "ERROR", MessageBoxButtons.OK);
            //    return;
            //}

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            PrinterSettings setting = dlg.PrinterSettings;


            //TPCResult<int> result = Database.GetModuleCount(m_Mode, txtCode.Text);
            //if (result.State == RESULT_STATE.NG)
            //{
            //    MessageBox.Show(result.Message);
            //    return;
            //}

            TPCResult<List<List<string>>> items = null;
            if (!queryInfo(txtCode.Text))
                return;

            bool queryInfo(string ID)
            {
                items = null;
                items = Database.GetFXZZ_Data(ID);
                if (items.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(items.Message);
                    return false;
                }
                return true;
            }

            #region
            //items.Value[0][1].ToString()            
            //将时间格式9013改成2019-01-03
            //items.Value[0][1] = "5015";
            string outputTime = "";
            string dateCode = items.Value[0][1].ToString();
            if (dateCode.Length != 4)
                return;
            string[] code = { dateCode.Substring(0, 1), dateCode.Substring(1, 2), dateCode.Substring(3, 1) };

            #region 确定年
            string today = DateTime.Today.ToString("yyyy");
            for (int i = 0; i < 10; i++)
            {
                if (today.Substring(3, 1).Equals(code[0]))
                {
                    outputTime = today;
                }
                else
                {
                    today = (Convert.ToInt16(today) - 1).ToString();
                }
            }
            #endregion
            #region 确定月份和日
            DateTime dtTemp = Convert.ToDateTime(outputTime + "-01-01");
            GregorianCalendar gc = new GregorianCalendar();
            for (int i = 0; i < 365; i++)
            {
                int weekOfYear = gc.GetWeekOfYear(dtTemp, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int dayOfWeek = (int)dtTemp.DayOfWeek + 1;
                if (weekOfYear == Convert.ToInt16(code[1]) && dayOfWeek == Convert.ToInt16(code[2]))
                {
                    outputTime = dtTemp.ToString("yyyy-MM-dd");
                    break;
                }
                else { dtTemp = dtTemp.AddDays(1); }
            }
            #endregion
            #endregion

            Print1 print1 = new Print1(txtCode.Text, items.Value[0][0].ToString(), outputTime, items.Value[0][2].ToString());
            print1.Show();//预览，不显示的话，全是空白
            string mode = txtCode.Text.Substring(0, 1);
            if (print1.BtnPrint_Click(setting, mode))
            {
                print1.Dispose();
                return;
            }
            print1.Dispose();
            //print1.BtnPrint_Click(setting);
            //print1.Dispose();

            
            if (mode == "P")
            {
                //设置新纸张大小
                dlg = new PrintDialog();
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                setting = dlg.PrinterSettings;



                Print2 print2 = new Print2(txtCode.Text, items.Value[0][0].ToString(), outputTime, items.Value[0][2].ToString());

                DataTable dt = new DataTable();
                dt.Columns.Add("No.");
                dt.Columns.Add("箱号");
                dt.Columns.Add("料号");
                dt.Columns.Add("数量");
                int i = 0;
                //每行写上被包括的子标签数量等
                foreach (ListViewItem var in lstItems.Items)
                {
                    string id = var.SubItems[1].Text;
                    if (!queryInfo(id))
                        return;
                    DataRow dr = dt.NewRow();
                    dr["No."] = (++i).ToString();
                    dr["箱号"] = id;
                    dr["料号"] = LabelPrintGlobal.g_Config.APN;
                    dr["数量"] = items.Value[0][0].ToString();
                    dt.Rows.Add(dr);
                }
                print2.ImportDataTable(dt);

                if (print2.BtnPrint_Click(setting))
                {
                    print2.Dispose();
                    return;
                }
                print2.Dispose();
            }
            //MessageBox.Show("插入管理表insert into pnt_mng");
            //这里需要写入pnt_mng表

            TPCResult<bool> ret = Database.SetManagerData(m_Mode, txtCode.Text, Program.LoginUser,
                                            Convert.ToInt32(txtTotal.Text), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);
            if (ret.State == RESULT_STATE.NG)
            {
                MessageBox.Show(ret.Message);
                return;
            }
        }

        private void 参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFXZZConfig fm = new NewFXZZConfig();
            //fm.StartPosition = FormStartPosition.CenterParent;
            fm.ShowDialog();
        }
    }
}
