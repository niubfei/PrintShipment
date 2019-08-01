using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TPCCommon.Common
{
    /// <summary>
    /// 多语言处理
    /// </summary>
    public class LanguageMapping
    {
        private LanguageMapping() { }

        #region 单例模式
        private static LanguageMapping m_Instance = new LanguageMapping();
        public static LanguageMapping Instance
        {
            get { return m_Instance; }
        }
        #endregion

        private Dictionary<string, Dictionary<string, string>> m_Mapping = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 初始化静态语言信息
        /// </summary>
        /// <param name="mapping_file"></param>
        /// <returns></returns>
        public bool LoadstaticLanguage(string mapping_file)
        {
            if (!File.Exists(mapping_file))
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Not found language mapping file.");
                return false;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(mapping_file);

                XmlNodeList nodes = doc.SelectNodes("language/static/item");
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["key"] == null)
                    {
                        TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_WARNNING, "Not found key in language mapping file.");
                        continue;
                    }

                    Dictionary<string, string> item = null;
                    //检查是否载入相同key
                    string key = node.Attributes["key"].Value;
                    if (m_Mapping.ContainsKey(key))
                    {
                        TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_WARNNING, string.Format("Duplicate Key:{0} in language mapping file.", key));
                        continue;
                    }
                    else
                    {
                        item = new Dictionary<string,string>();
                        m_Mapping.Add(key, item);
                    }

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name.Equals("key"))
                            continue;

                        //检查是否包含该语言说明
                        string lang = attr.Name;
                        if (item.ContainsKey(lang))
                        {
                            TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_WARNNING, string.Format("Duplicate language:{0} in Key {1}.", lang, key));
                            continue;
                        }

                        item.Add(lang, attr.Value);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                return false;
            }
        }

        /// <summary>
        /// 从多语言配置文件中获得固定的语言自串
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="lang">语言标记</param>
        /// <returns></returns>
        public string GetStaticMessage(string key, string lang)
        {
            if (m_Mapping.ContainsKey(key))
            {
                Dictionary<string, string> item = m_Mapping[key];
                if (item.ContainsKey(lang))
                {
                    return item[lang];
                }
            }
            TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, string.Format("Not found language:{0} message by key:{1}", lang, key));
            return "";
        }

        /// <summary>
        /// 动态从多语言配置文件中读取语言字串
        /// </summary>
        /// <param name="language_file">多语言配置文件</param>
        /// <param name="key">键值</param>
        /// <param name="lang">语言标记</param>
        /// <returns></returns>
        public string GetDynamicMessage(string language_file, string key, string lang)
        {
            if (!File.Exists(language_file))
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Not found language mapping file.");
                return "";
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(language_file);

                //判断key的节点是否存在
                string xpath = string.Format("language/dynamic/item[@key={0}]", key);
                XmlNode node = doc.SelectSingleNode(xpath);
                if (node == null)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, string.Format("Not found language:{0} message by key:{1}", lang, key));
                    return "";
                }
                //判断该语言标记是否存在
                if (node.Attributes[lang] == null)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, string.Format("Not found language:{0} message by key:{1}", lang, key));
                    return "";
                }

                return node.Attributes[lang].Value;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                return "";
            }
        }

        /// <summary>
        /// 动态从多语言配置文件中读取警告信息语言字串
        /// </summary>
        /// <param name="language_file">多语言配置文件</param>
        /// <param name="key">键值</param>
        /// <param name="lang">语言标记</param>
        /// <returns></returns>
        public string GetWarnningMessage(string language_file, string key, string lang)
        {
            if (!File.Exists(language_file))
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, "Not found language mapping file.");
                return "";
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(language_file);

                //判断key的节点是否存在
                string xpath = string.Format("language/warnning/item[@key='{0}']", key);
                XmlNode node = doc.SelectSingleNode(xpath);
                if (node == null)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, string.Format("Not found language:{0} message by key:{1}", lang, key));
                    return "";
                }
                //判断该语言标记是否存在
                if (node.Attributes[lang] == null)
                {
                    TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, string.Format("Not found language:{0} message by key:{1}", lang, key));
                    return "";
                }

                return node.Attributes[lang].Value;
            }
            catch (Exception e)
            {
                TPCLogger.Instance.Write(LOGGER_TYPE.LOGGER_ERROR, e.Message);
                return "";
            }
        }
    }
}
