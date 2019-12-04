using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TPCCommon.Common;

namespace LabelPrint
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            InitLanguage();

            TPCResult<bool> result = LabelPrintGlobal.g_Config.LoadConfig(LabelPrintGlobal.g_ConfigFile);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                Close();
                return;
            }

            SetData();
        }

        protected void SetData()
        {
            txtPega.Text = LabelPrintGlobal.g_Config.Pegatoron_PN;
            txtAPN.Text = LabelPrintGlobal.g_Config.APN;
            txtREV.Text = LabelPrintGlobal.g_Config.REV;
            txtConfig.Text = LabelPrintGlobal.g_Config.Config;
            txtDesc.Text = LabelPrintGlobal.g_Config.DESC;
            txtStage.Text = LabelPrintGlobal.g_Config.Stage;
            txtVendor.Text = LabelPrintGlobal.g_Config.Vendor;
            txtSiteCode.Text = LabelPrintGlobal.g_Config.SiteCode;
            txtLc.Text = LabelPrintGlobal.g_Config.lc;
            txtBacth.Text = LabelPrintGlobal.g_Config.batch;
            txtVendorPN.Text = LabelPrintGlobal.g_Config.Vendor_PN;

            txtQtyTrays.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PackTrays);
            txtQTYPacks.Text = string.Format("{0}", LabelPrintGlobal.g_Config.CartonPack);
            txtQTYCartons.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PalletCarton);
        }

        protected void GetData()
        {
            LabelPrintGlobal.g_Config.Pegatoron_PN =  txtPega.Text;
            LabelPrintGlobal.g_Config.APN = txtAPN.Text;
            LabelPrintGlobal.g_Config.REV = txtREV.Text;
            LabelPrintGlobal.g_Config.Config = txtConfig.Text;
            LabelPrintGlobal.g_Config.DESC = txtDesc.Text;
            LabelPrintGlobal.g_Config.Stage = txtStage.Text;
            LabelPrintGlobal.g_Config.Vendor = txtVendor.Text;
            LabelPrintGlobal.g_Config.SiteCode = txtSiteCode.Text;
            LabelPrintGlobal.g_Config.lc = txtLc.Text;
            LabelPrintGlobal.g_Config.batch = txtBacth.Text;
            LabelPrintGlobal.g_Config.Vendor_PN = txtVendorPN.Text;

            LabelPrintGlobal.g_Config.PackTrays = Convert.ToInt32(txtQtyTrays.Text);
            LabelPrintGlobal.g_Config.CartonPack = Convert.ToInt32(txtQTYPacks.Text);
            LabelPrintGlobal.g_Config.PalletCarton = Convert.ToInt32(txtQTYCartons.Text);
        }

        protected bool CheckInput()
        {
            //TODO
            return true;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            GetData();
            TPCResult<bool> result = LabelPrintGlobal.g_Config.SaveConfig(LabelPrintGlobal.g_ConfigFile);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                return;
            }

            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected void InitLanguage()
        { 
            //TODO
        }

    }
}
