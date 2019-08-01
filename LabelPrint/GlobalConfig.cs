using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TPCCommon.Common;
using TPCBarcode.LabelPrint;

namespace LabelPrint
{
    public enum ITEM_STATUS
    { 
        BEGIN = 0,
        COMPLETED = 1,
        CANCELED = 2,
    }

    public static class LabelPrintGlobal
    {
        /// <summary>
        /// 多语言设置
        /// </summary>
        public static string g_Language = "zh-cn";
        /// <summary>
        /// 多语言配置文件
        /// </summary>
        public static string g_LanguageConfig = "";

        /// <summary>
        /// 基础配置文件
        /// </summary>
        public static string g_ConfigFile = "";

        /// <summary>
        /// 基础配置信息
        /// </summary>
        public static ConfigSetting g_Config = new ConfigSetting();

        /// <summary>
        /// 获取多语言警告文字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ShowWarningMessage(string key)
        {
            return LanguageMapping.Instance.GetWarnningMessage(g_LanguageConfig, key,  g_Language);
        }

        /// <summary>
        /// 标签打印模块
        /// </summary>
        public static LabelCreator g_LabelCreator = new LabelCreator();

        /// <summary>
        /// 选取打印的打印机名称
        /// 如果是默认打印机，则为空
        /// </summary>
        public static string g_PrintName = ""; //"Microsoft XPS Document Writer";     //这里作为调试测试用
    }
}
