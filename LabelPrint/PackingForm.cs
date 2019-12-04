using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TPCCommon.Common;
using TPCCommon.Database;
using LabelPrint.Model;

namespace LabelPrint
{
    public partial class PackingForm : Form
    {
        public PackingForm()
        {
            InitializeComponent();
            TopLevel = false;
        }

        protected IFPackModel m_Model = null;
        public IFPackModel Model
        {
            get { return m_Model; }
            set { m_Model = value; }
        }

        public void ShowMode()
        {
            m_Model.InitLanguage();
            txtCNo.ReadOnly = true;
        }

        #region 开放公共控件属性
        public Label PNoLabel
        {
            get { return lbPNo; }
        }

        public TextBox PNoEdit
        {
            get { return txtPNo; }
        }

        public Label CNoLabel
        {
            get { return lbCNo; }
        }

        public TextBox CNoEdit
        {
            get { return txtCNo; }
        }

        public Button ApplyButton
        {
            get { return btApply; }
        }

        public Label PackDateLabel
        {
            get { return lbPackDate; }
        }

        public Label QTYLabel
        {
            get { return lbQty; }
        }

        public TextBox QTYEdit
        {
            get { return txtQty; }
        }

        public TextBox QTYTotalEdit
        {
            get { return txtTotal; }
        }

        public ListView ItemsListView
        {
            get { return lstItems; }
        }

        public Button PrintButton
        {
            get { return btPrintLabel; }
        }
        //修正时间20180517
        public Button PrintButtonNew
        {
            get { return btPrintLabelNew; }
        }
        //end

        public RadioButton PackingRadioButton
        {
            get { return rdPacking; }
        }

        public RadioButton RepackingRadioButton
        {
            get { return rdRepacking; }
        }

        public DateTimePicker DatePicker
        {
            get { return dtPackDate; }
        }
        #endregion

        protected bool CanPacking()
        {
            //如果前面还有装箱，则先检测是否打印过标签
            if (lstItems.Items.Count > 0 || txtPNo.Text.Length > 0)
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("APPLY_ID_ERROR"), "ERROR", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 申请新编号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btApply_Click(object sender, EventArgs e)
        {
            //如果前面还有装箱，则先检测是否打印过标签
            if (!CanPacking())
                return;

            //预约新号
            TPCResult<bool> result = m_Model.ApplyNewCode();
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message, "ERROR", MessageBoxButtons.OK);
                return;
            }

            //只有在父No有值时，才能扫描子No
            txtCNo.ReadOnly = false;
            txtCNo.Focus();

            //防止重复提交
            btApply.Enabled = false;
            txtQty.Text = "0";
        }

        /// <summary>
        /// 新装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdPacking_Click(object sender, EventArgs e)
        {
            m_Model.PackingOn();
            txtPNo.ReadOnly = true;
            txtCNo.ReadOnly = true;
        }

        /// <summary>
        /// 重装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdRepacking_Click(object sender, EventArgs e)
        {
            //如果前面还有装箱，则先检测是否打印过标签
            if (lstItems.Items.Count > 0 || txtPNo.Text.Length > 0)
            {
                if (MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("REPACK_ERROR"), "CONFIRM", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    rdPacking.Checked = true;
                    return;
                }
            }

            m_Model.RepackingOn();

            //只有在父No有值时，才能扫描子No
            txtCNo.ReadOnly = true;
        }

        /// <summary>
        /// 子编号编辑框，收到扫描条码事件
        /// 增加子项到列表中，并保存到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                string code = CNoEdit.Text;
                //这里增加限制，如果父项条码没有预约，则不允许扫描
                if (txtPNo.Text.Length == 0)
                {
                    MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NO_PARENT_SCAN_ERROR"), "WARNNING", MessageBoxButtons.OK);
                    return;
                }

                //回车键，这里默认条码扫描后自带回车键
                TPCResult<bool> result = m_Model.ScanCCode(code);
                if (result.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(result.Message);
                    return;
                }
                CNoEdit.Text = "";

                //只有至少一个子项输入才能再次预约
                btApply.Enabled = true;

                //开始扫描子项后,不允许修改总数
                //txtTotal.Enabled = false;
            }
        }

        /// <summary>
        /// 父编号编辑框，收到扫描条码后事件
        /// 逻辑上认为扫描的条码为子项，需要从数据库中检索出父项条码
        /// 并显示到父编号编辑框中，同时设定只读属性，切换状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                //这里认为扫描焦点在父编码编辑框
                string code = txtPNo.Text;
                //回车键，这里默认条码扫描后自带回车键
                TPCResult<bool> result = m_Model.ScanPCode(code);
                if (result.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(result.Message, "ERROR", MessageBoxButtons.OK);
                    return;
                }
                //只有在父No有值时，才能扫描子No
                txtCNo.ReadOnly = false;
                txtCNo.Focus();
            }
        }

        /// <summary>
        /// 打印标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPrintLabel_Click(object sender, EventArgs e)
        {
            if (lstItems.Items.Count < Convert.ToInt32(txtTotal.Text))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NOT_FULL_QUANTITY_ERROR"), "ERROR", MessageBoxButtons.OK);
                return;
            }

            //检查明细的状态，如果为1则已经打印，不能在这里打印了
            if (lstItems.Tag != null)
            {
                List<CItem> items = lstItems.Tag as List<CItem>;
                if (items != null)
                {
                    foreach (CItem item in items)
                    {
                        if (item.Status == 1)
                        {
                            MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("REPACK_NOT_PRINT"), "ERROR", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
            }
            //修改日期20180521
            //打印新标签PEGA 2页
            m_Model.PrintLabel();
            //标签打印结束，清空明细列表
            lstItems.Items.Clear();
            //清空父ID信息
            txtPNo.Text = "";

            txtCNo.ReadOnly = true;

            //打印后允许修改总数
            //txtTotal.Enabled = true;
        }

        /// <summary>
        /// 可以编辑装包总数，当编辑结束，焦点移开时
        /// 校验数值是否正确
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTotal_Leave(object sender, EventArgs e)
        {
            int total = 0;
            bool is_error = false;
            if (!int.TryParse(txtTotal.Text, out total))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("TOTAL_NUMBER_ERROR"), "ERROR", MessageBoxButtons.OK);
                is_error = true;
            }

            if (total < lstItems.Items.Count)
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("TOTAL_NUMBER_LESS_THEN_DETAIL"), "ERROR", MessageBoxButtons.OK);
                is_error = true;
            }

            if (is_error)
            {
                switch (m_Model.Mode)
                {
                    case PACK_MODE.Pack:
                        txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PackTrays);
                        break;
                    case PACK_MODE.Carton:
                        txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.CartonPack);
                        break;
                    case PACK_MODE.Pallet:
                        txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PalletCarton);
                        break;
                }
            }

            //设定打印按钮有效状态
            btPrintLabel.Enabled = (total == lstItems.Items.Count);
            //设定打印按钮的有效状态 修正时间20180517
            btPrintLabelNew.Enabled = (total == lstItems.Items.Count);
            //end
        }


        /// <summary>
        /// 拷贝明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstItems_KeyPress(object sender, KeyPressEventArgs e)
        {
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
        /// <summary>
        /// 打印标签 修改日期20180517
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPrintLabelNew_Click(object sender, EventArgs e)
        {

            if (lstItems.Items.Count < Convert.ToInt32(txtTotal.Text))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NOT_FULL_QUANTITY_ERROR"), "ERROR", MessageBoxButtons.OK);
                return;
            }

            //检查明细的状态，如果为1则已经打印，不能在这里打印了
            if (lstItems.Tag != null)
            {
                List<CItem> items = lstItems.Tag as List<CItem>;
                if (items != null)
                {
                    foreach (CItem item in items)
                    {
                        if (item.Status == 1)
                        {
                            MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("REPACK_NOT_PRINT"), "ERROR", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
            }
            //修改日期20180521
            //打印标签FXZZ
            m_Model.PrintLabelNew();
            //标签打印结束，清空明细列表
            lstItems.Items.Clear();
            //清空父ID信息
            txtPNo.Text = "";

            txtCNo.ReadOnly = true;

            //打印后允许修改总数
            //txtTotal.Enabled = true;
        }
    }
}
