using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TPCCommon.Common;

namespace LabelPrint
{
    public partial class UnpackForm : Form
    {
        public UnpackForm()
        {
            InitializeComponent();
        }

        protected LabelDatabaseHelper m_Database = null;
        public LabelDatabaseHelper Database
        {
            get { return m_Database; }
            set { m_Database = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            InitLanguage();
            InitAuth();

            txtQty.Text = "0";
            txtTotal.Text = "0";
        }

        protected string m_CCode = "";

        private void txtUnpackCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                //获得扫描的编码
                string code = txtUnpackCode.Text;
                //从视图中查找
                TPCResult<List<CItem>> items = Database.GetItemsByPCode(code);
                if (items.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(items.Message);
                    return;
                }

                FillItems(items.Value);
                is_all_cancel = false;
            }
        }

        private void txtUnpackItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                //获得扫描的编码
                m_CCode = txtUnpackItemCode.Text;
                txtUnpackItemCode.Text = "";

                //从列表中找到对应的item，并将其设置为Canceled
                foreach (ListViewItem it in lstUnpackItems.Items)
                {
                    CItem item = it.Tag as CItem;
                    if (item != null && item.Code.Equals(m_CCode))
                    {
                        it.UseItemStyleForSubItems = false;
                        item.Status = (int)ITEM_STATUS.CANCELED;
                        it.SubItems[3].Text = "CANCELED";
                        it.SubItems[3].ForeColor = Color.Red;
                        it.SubItems[3].BackColor = Color.Yellow;
                    }
                }
                m_CCode = "";
            }
        }

        protected void FillItems(List<CItem> items)
        {
            lstUnpackItems.Items.Clear();
            lstUnpackItems.Tag = items;
            foreach (CItem item in items)
            {
                ListViewItem it = lstUnpackItems.Items.Add(string.Format("{0}", lstUnpackItems.Items.Count + 1));
                it.Tag = item;
                it.SubItems.Add(item.Code);
                it.SubItems.Add(item.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                it.SubItems.Add(item.Status == (int)ITEM_STATUS.COMPLETED ? "COMPLETED" : "CANCELD");
                if (item.Status == (int)ITEM_STATUS.CANCELED)
                {
                    it.UseItemStyleForSubItems = false;
                    it.SubItems[3].ForeColor = Color.Red;
                    it.SubItems[3].BackColor = Color.Yellow;
                }
            }
            btAll.Enabled = true;
            //设定数量
            txtQty.Text = string.Format("{0}", lstUnpackItems.Items.Count);
            if (items.Count > 0)
            { 
                if (items[0].Name.Equals("pnt_pack"))
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PackTrays);
                else if (items[0].Name.Equals("pnt_carton"))
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.CartonPack);
                else if (items[0].Name.Equals("pnt_pallet"))
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PalletCarton);
            }
            
        }

        protected bool IsDeepCancel()
        {
            return rdoDeepCancel.Checked;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool is_all_cancel = false;
        private void btAll_Click(object sender, EventArgs e)
        {
            List<CItem> items = lstUnpackItems.Tag as List<CItem>;
            if (items != null)
            {
                foreach (CItem item in items)
                {
                    item.Status = (int)ITEM_STATUS.CANCELED;
                }
                FillItems(items);
            }
            is_all_cancel = true;
        }

        /// <summary>
        /// 通过表名获得拆包的模式：pack, carton, pallet
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetModeFromTableName(string name)
        {
            if (name.ToLower().Equals("png_pack"))
                return "pack";
            else if (name.ToLower().Equals("pnt_carton"))
                return "carton";
            else if (name.ToLower().Equals("pnt_pallet"))
                return "pallet";
            else
                return "";
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (!CheckUnpack())
                return;

            if (IsDeepCancel())
            {
                if (MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("DEEP_CANCELED_WARNNING"), "Warnning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            List<CItem> items = lstUnpackItems.Tag as List<CItem>;
            if (items != null)
            {
                string name = items[0].Name;
                string mode = GetModeFromTableName(name);
                if (IsDeepCancel())
                {
                    TPCResult<bool> result = null;
                    //深度拆包 
                    if (is_all_cancel)
                    {
                        result = Database.UnpackItemByDeep(mode, txtUnpackCode.Text, Program.LoginUser);
                    }
                    else
                    {
                        result = Database.UnpackItemByDeep(items, Program.LoginUser);
                    }
                    if (result.State == RESULT_STATE.NG)
                    {
                        MessageBox.Show(result.Message);
                        return;
                    }
                }
                else
                {
                    //单层拆包
                    TPCResult<bool> result = Database.UnpackItemsBySingle(items, Program.LoginUser);
                    if (result.State == RESULT_STATE.NG)
                    {
                        MessageBox.Show(result.Message);
                        return;
                    }
                }
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("COMPLETED_UNPACK_INFO"));
                Close();
            }
            else
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NO_ITEM_UNPACK_ERROR"));
            }
        }

        protected void InitLanguage()
        {
            lbUnpackCode.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_CODE", LabelPrintGlobal.g_Language);
            lbUnpackItemCode.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_CHILD_CODE", LabelPrintGlobal.g_Language);
            lbQty.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_QTY", LabelPrintGlobal.g_Language);
            rdoSingle.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_SINGLE", LabelPrintGlobal.g_Language);
            rdoDeepCancel.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_DEEP", LabelPrintGlobal.g_Language);

            string[] columns_header = new string[] { "UNPACK_ITEM_NO", "UNPACK_ITEM_CODE", "UNPACK_ITEM_DATE", "UNPACK_ITEM_STATUS" };
            for (int i = 0; i < lstUnpackItems.Columns.Count; i++)
            {
                ColumnHeader col = lstUnpackItems.Columns[i];
                string key = columns_header[i];
                col.Text = LanguageMapping.Instance.GetStaticMessage(key, LabelPrintGlobal.g_Language);
            }

            btAll.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_BT_ALL", LabelPrintGlobal.g_Language);
            btOK.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_BT_COMMIT", LabelPrintGlobal.g_Language);
            btClose.Text = LanguageMapping.Instance.GetStaticMessage("UNPACK_BT_CLOSE", LabelPrintGlobal.g_Language);
        }

        protected void InitAuth()
        {
            rdoDeepCancel.Enabled = Program.IsSupperUser();
            rdoSingle.Enabled = Program.IsSupperUser();
        }

        private void lstUnpackItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            StringBuilder ss = new StringBuilder();
            for (int i = 0; i < lstUnpackItems.SelectedItems.Count; i++)
            {
                ListViewItem item = lstUnpackItems.SelectedItems[i];
                if (i > 0)
                {
                    ss.Append("\r\n");
                }

                ss.Append(string.Format("{0},{1},{2}", item.Text, item.SubItems[1].Text, item.SubItems[2].Text));
            }

            Clipboard.SetText(ss.ToString());
        }

        //如果没有选择任何拆包的明细，则不允许确定
        protected bool CheckUnpack()
        {
            List<CItem> items = lstUnpackItems.Tag as List<CItem>;
            if (items != null)
            {
                bool has_cancel = false;
                foreach (CItem item in items)
                {
                    if (item.Status == 2)
                    {
                        has_cancel = true;
                        break;
                    }
                }
                if (!has_cancel)
                {
                    MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("UNPACK_NO_ITEM_ERROR"));
                }
                return has_cancel;
            }
            else
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NO_ITEM_UNPACK_ERROR"));
                return false;
            }
        }
    }
}
