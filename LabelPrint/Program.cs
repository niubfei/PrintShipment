using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using TPCCommon.Common;
using TPCCommon.Database;

namespace LabelPrint
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //设定日志的根目录
            TPCLogger.Instance.RootFolder = string.Format("{0}\\log", Application.StartupPath);
            if (!Directory.Exists(TPCLogger.Instance.RootFolder))
            {
                Directory.CreateDirectory(TPCLogger.Instance.RootFolder);
            }
            //语言设定
            LabelPrintGlobal.g_Language = ConfigurationManager.AppSettings["language"];

            if (!InitLanguageMapping())
            {
                MessageBox.Show("Initialize language failed.", "ERROR", MessageBoxButtons.OK);
                return;
            }
            //load database oonfig
            string config = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\database.xml";
            if (!DatabaseFactory.GetInstance().LoadConfig(config))
            {
                MessageBox.Show("Initialize database failed.", "ERROR", MessageBoxButtons.OK);
                return;
            }
            //数据库初始化
            m_DataHelper = new LabelDatabaseHelper(DatabaseFactory.GetInstance().CreateDatabase("NIDEC"));
            if (m_DataHelper.Database == null)
            {
                MessageBox.Show("Initialize database failed.", "ERROR", MessageBoxButtons.OK);
                return;
            }
            //DO TEST
            //TestProcOnMultiThread(m_DataHelper);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            //Print2 print2 = new Print2("id", "A", "a", "_", "1", "1 a_A");
            //Application.Run(print2);
            //return;

            LoginForm login = new LoginForm();
            DialogResult rt = login.ShowDialog();
            if (rt == DialogResult.Cancel)
            {
                return;
            }

            MainForm main = new MainForm();
            main.DatabaseHelper = m_DataHelper;
            Application.Run(main);
        }

        /// <summary>
        /// 初始化多语言配置
        /// </summary>
        /// <returns></returns>
        private static bool InitLanguageMapping()
        {
            string config = string.Format("{0}\\language.xml", Application.StartupPath);
            if (!LanguageMapping.Instance.LoadstaticLanguage(config))
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Not found language mapping file.");
                return false;
            }
            LabelPrintGlobal.g_LanguageConfig = config;
            return true;
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        private static string m_LoginUser = "";
        public static string LoginUser
        {
            get { return m_LoginUser; }
            set { m_LoginUser = value; }
        }
        /// <summary>
        /// 用户角色，用于设定重新打印按钮的权限
        /// </summary>
        private static string m_UserRole = "";
        public static string UserRole
        {
            get { return m_UserRole; }
            set { m_UserRole = value; }
        }

        //用用户角色来定义用户权限
        public static bool IsSupperUser()
        {
            return m_UserRole.ToLower().Equals("super");
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private static LabelDatabaseHelper m_DataHelper = null;
        public static LabelDatabaseHelper DataHelper
        {
            get { return m_DataHelper; }
        }

        #region 多线程同步访问PROC测试
        /*
        private static void TestProcOnMultiThread(LabelDatabaseHelper database)
        {
            for (int i = 0; i < 20; i++)
            {
                Thread th = new Thread(WorkingTest);
                th.Start(database);
            }
        }

        private static void WorkingTest(object param)
        {
            LabelDatabaseHelper database = param as LabelDatabaseHelper;

            for (int i = 0; i < 100; i++)
            {
                TPCResult<string> result = database.ApplyNewCode("pack", "BM004HK1", "1", "7265", 13, "magic.zhu");
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_DEBUG, result.Value);
                Thread.Sleep(100);
            }

        }
         * */
        #endregion
    }
}
