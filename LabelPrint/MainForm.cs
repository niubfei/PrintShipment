using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LabelPrint.Model;
using TPCCommon.Common;
using TPCCommon.Database;
using OUT;

namespace LabelPrint
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Text = Text + "_" + Application.ProductVersion.ToString();
            cboLanguage.SelectedIndex = 0;
        }

        protected PackingForm m_PackingForm = new PackingForm();
        private LabelDatabaseHelper m_DatabseHelper = null;
        public LabelDatabaseHelper DatabaseHelper
        {
            get { return m_DatabseHelper; }
            set { m_DatabseHelper = value; }
        }

        #region 三种模式
        protected PackModel m_Pack = null;
        protected CartonModel m_Carton = null;
        protected PalletModel m_Pallet = null;
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            //初始化标签打印模块
            string config = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\label.xml";
            if (!LabelPrintGlobal.g_LabelCreator.LoadConfig(config))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("LABEL_PRINT_INIT_ERROR"), "ERROR", MessageBoxButtons.OK);
                Close();
                return;
            }

            //基础配置文件
            LabelPrintGlobal.g_ConfigFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config.xml";
            TPCResult<bool> rt = LabelPrintGlobal.g_Config.LoadConfig(LabelPrintGlobal.g_ConfigFile);
            if (rt.State == RESULT_STATE.NG)
            {
                MessageBox.Show(rt.Message);
                Close();
                return;
            }

            ///初始化三种模式
            #region 初始化三种模式
            m_Pack = new PackModel(m_PackingForm);
            m_Carton = new CartonModel(m_PackingForm);
            m_Pallet = new PalletModel(m_PackingForm);
            
            m_Pack.Database = m_DatabseHelper;
            m_Carton.Database = m_DatabseHelper;
            m_Pallet.Database = m_DatabseHelper;

            m_Pack.CodeKey = "BM004HK1-1";
            m_Carton.CodeKey = "BM004HK1-1";
            m_Pallet.CodeKey = "BM004HK1-1";
            #endregion

            m_PackingForm.Model = m_Pack;

            m_PackingForm.Show();
            InitLanguage();
            InitAuth();

            ShowMode(PACK_MODE.Pack);
        }

        protected void ShowMode(PACK_MODE mode)
        {
            try
            {
                TabPage page = tabWork.TabPages[(int)mode];
                page.Select();
                m_PackingForm.Parent = page;
                switch (mode)
                { 
                    case PACK_MODE.Pack:
                        m_PackingForm.Model = m_Pack;
                        break;
                    case PACK_MODE.Carton:
                        m_PackingForm.Model = m_Carton;
                        break;
                    case PACK_MODE.Pallet:
                        m_PackingForm.Model = m_Pallet;
                        break;
                }
                
                m_PackingForm.ShowMode();
                page.Controls.Add(m_PackingForm);
                m_PackingForm.Top = page.ClientRectangle.Top;
                m_PackingForm.Left = page.ClientRectangle.Left;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        protected void InitLanguage()
        {
            btUnpacking.Text = LanguageMapping.Instance.GetStaticMessage("BT_UNPACK", LabelPrintGlobal.g_Language);
            btDataBrowse.Text = LanguageMapping.Instance.GetStaticMessage("BT_SEARCH_DATA", LabelPrintGlobal.g_Language);
            btConfig.Text = LanguageMapping.Instance.GetStaticMessage("BT_CONFIG", LabelPrintGlobal.g_Language);
            btReprint.Text = LanguageMapping.Instance.GetStaticMessage("BT_REPRINT", LabelPrintGlobal.g_Language);

            tabWork.TabPages[0].Text = LanguageMapping.Instance.GetStaticMessage("PACK_NAME", LabelPrintGlobal.g_Language);
            tabWork.TabPages[1].Text = LanguageMapping.Instance.GetStaticMessage("CARTON_NAME", LabelPrintGlobal.g_Language);
            tabWork.TabPages[2].Text = LanguageMapping.Instance.GetStaticMessage("PALLET_NAME", LabelPrintGlobal.g_Language);

            if (m_PackingForm.Model != null)
            {
                m_PackingForm.Model.InitLanguage();
            }
        }

        protected void InitAuth()
        {
            btReprint.Enabled = Program.IsSupperUser();
            btConfig.Enabled = Program.IsSupperUser();
        }
        /// <summary>
        /// tab页切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabWork_SelectedIndexChanged(object sender, EventArgs e)
        {
            PACK_MODE mode = (PACK_MODE)tabWork.SelectedIndex;
            ShowMode(mode);
        }

        /// <summary>
        /// 配置设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btConfig_Click(object sender, EventArgs e)
        {
            //设置参数
            ConfigForm dlg = new ConfigForm();
            dlg.ShowDialog();
            //设置参数后，重新载入
            ShowMode((PACK_MODE)tabWork.SelectedIndex);
        }

        private void btUnpacking_Click(object sender, EventArgs e)
        {
            UnpackForm dlg = new UnpackForm();
            dlg.Database = DatabaseHelper;
            dlg.ShowDialog();
        }
        /// <summary>
        /// exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void btExit_Click(object sender, EventArgs e)
        {
            /*
            if (m_PackingForm.ItemsListView.Items.Count > 0 || m_PackingForm.PNoEdit.Text.Length > 0)
            {
                if (MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("EXIT_ERROR"), "CONFIRM", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
             * */
            Close();
        }

        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cboLanguage.SelectedIndex;
            string[] LANG_NAME = new string[] { "zh-cn", "jp", "en" };
            LabelPrintGlobal.g_Language = LANG_NAME[index];

            InitLanguage();
        }

        private void btReprint_Click(object sender, EventArgs e)
        {
            ReprintForm dlg = new ReprintForm();
            dlg.Database = DatabaseHelper;
            dlg.ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //检查是否有未完成工作,即工作对象条码是否存在 
            if (m_PackingForm.PNoEdit.Text.Length > 0)
            {
                if (MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("UNCOMPLETED_CLOSE_WARN"), "WARNNING", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

		private void button1_Click(object sender, EventArgs e)
		{
			LoginForm login = new LoginForm();
		
			Form_Invoice invoice = new Form_Invoice();
			invoice.logingname = login.LoginUser;
			invoice.Show();
		}
    }
}
