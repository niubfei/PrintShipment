using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace TPCCommon.Common
{
    /// <summary>
    /// 日志类型:系统，错误，警告，信息，调试
    /// </summary>
    public enum LOGGER_TYPE
    { 
        LOGGER_SYSTEM = 0,
        LOGGER_ERROR = 1,
        LOGGER_WARNNING = 2,
        LOGGER_INFORMATION = 3,
        LOGGER_DEBUG = 4,
    }

    /// <summary>
    /// 日志模块
    /// </summary>
    public class TPCLogger
    {
        private TPCLogger() { }

        #region 单例模式
        private static TPCLogger m_Instance = new TPCLogger();
        public static TPCLogger Instance
        {
            get { return m_Instance; }
        }
        #endregion

        #region 线程同步处理
        private ManualResetEvent m_SyncoEvent = new ManualResetEvent(true);
        private void Wait()
        {
            m_SyncoEvent.WaitOne();
            m_SyncoEvent.Reset();
        }

        private void Free()
        {
            m_SyncoEvent.Set();
        }
        #endregion

        #region 日志文件根目录
        protected string m_RootFolder = "";
        public string RootFolder
        {
            get { return m_RootFolder; }
            set { m_RootFolder = value; }
        }
        #endregion

        #region 写入日志的级别
        /// <summary>
        /// 日志输出按照级别处理，小于等于指定级别的日志才会被记录
        /// </summary>
        protected LOGGER_TYPE m_Level = LOGGER_TYPE.LOGGER_DEBUG;
        public LOGGER_TYPE Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        #endregion

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="state">日志类型</param>
        /// <param name="msg">日志描述</param>
        public void Write(LOGGER_TYPE state, string msg)
        {
            //检查日志级别，这里只写入指定级别以下的日志
            if (state > m_Level)
                return;

            StreamWriter sw = null;
            //多线程同步等待
            Wait();
            try
            {
                //打开日志文件
                sw = GetLoggerStream();
                if (sw == null)
                    return;

                string[] STATE_CAPTION = new string[] { "SYS", "ERR", "WRN", "INF", "DBG" };

                //写入
                string logger = string.Format("[{0}] {1:yyyy-MM-dd HH:mm:ss} === {2}", STATE_CAPTION[(int)state], DateTime.Now, msg);
                sw.WriteLine(logger);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                Free();
            }
        }

        /// <summary>
        /// 生成文件写入流
        /// </summary>
        /// <returns></returns>
        protected StreamWriter GetLoggerStream()
        {
            StreamWriter sw = null;
            string filename = string.Format("{0:yyyyMMdd}.log", DateTime.Today);
            try
            {
                sw = new StreamWriter(string.Format("{0}\\{1}", m_RootFolder, filename), true);
                return sw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }
        }
    }
}
