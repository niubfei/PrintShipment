using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Xml;
using TPCCommon.Common;

namespace LabelPrint
{
    public partial class NewFXZZConfig : Form
    {
        public NewFXZZConfig()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            TPCResult<bool> result = LabelPrintGlobal.g_Config.LoadConfig(LabelPrintGlobal.g_ConfigFile);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                Close();
                return;
            }

            SetData();
        }        

        private void BtOK_Click(object sender, EventArgs e)
        {
            if (txtC.Text == "" || txtMfr.Text == "")
            {
                MessageBox.Show("参数不能为空","保存参数",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            GetData();
            TPCResult<bool> result = LabelPrintGlobal.g_Config.SaveConfig(LabelPrintGlobal.g_ConfigFile);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                return;
            }

            Close();
        }
        protected void SetData()
        {
            txtC.Text = LabelPrintGlobal.g_Config.HH;
            txtMfr.Text = LabelPrintGlobal.g_Config.Mfr;
        }
        protected void GetData()
        {
            LabelPrintGlobal.g_Config.HH = txtC.Text;
            LabelPrintGlobal.g_Config.Mfr = txtMfr.Text;
        }
    }
}
