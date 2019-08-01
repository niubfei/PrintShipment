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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        protected string m_LoginUser = "";
        public string LoginUser
        {
            get { return m_LoginUser; }
            set { m_LoginUser = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            InitLanguage();
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
			m_LoginUser = txtUserName.Text;
            if (!CheckLogin())
            {
                return;
            }
			
			DialogResult = DialogResult.OK;
            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            m_LoginUser = "";
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected bool CheckLogin()
        {
            string name = txtUserName.Text;
            string passwd = txtPasswd.Text;

            if (name.Length == 0)
            {
                MessageBox.Show("Please input user name.", "ERROR", MessageBoxButtons.OK);
                return false;
            }

            TPCResult<string> ret = Program.DataHelper.Login(name, passwd);
            if (ret.State == RESULT_STATE.NG)
            {
                MessageBox.Show(ret.Message, "ERROR", MessageBoxButtons.OK);
                return false;
            }

            Program.LoginUser = name;
            Program.UserRole = ret.Value;
		
			return true;
        }

        protected void InitLanguage()
        { 
            //TODO
        }
    }
}
