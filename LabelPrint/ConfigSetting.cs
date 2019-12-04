using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TPCCommon.Common;

namespace LabelPrint
{
    public class ConfigSetting
    {
        #region 设定值
        protected string m_Pegatoron_PN = "";
        public string Pegatoron_PN
        {
            get { return m_Pegatoron_PN; }
            set { m_Pegatoron_PN = value; }
        }

        protected string m_APN = "";
        public string APN
        {
            get { return m_APN; }
            set { m_APN = value; }
        }

        protected string m_REV = "";
        public string REV
        {
            get { return m_REV; }
            set { m_REV = value; }
        }

        protected string m_Config = "";
        public string Config
        {
            get { return m_Config; }
            set { m_Config = value; }
        }

        protected string m_DESC = "";
        public string DESC
        {
            get { return m_DESC; }
            set { m_DESC = value; }
        }

        protected string m_Stage = "";
        public string Stage
        {
            get { return m_Stage; }
            set { m_Stage = value; }
        }

        protected string m_Vendor = "BM004HK1";
        public string Vendor
        {
            get { return m_Vendor; }
            set { m_Vendor = value; }
        }

        protected string m_SiteCode = "1";
        public string SiteCode
        {
            get { return m_SiteCode; }
            set { m_SiteCode = value; }
        }

        protected string m_lc = "";
        public string lc
        {
            get { return m_lc; }
            set { m_lc = value; }
        }

        protected string m_batch = "";
        public string batch
        {
            get { return m_batch; }
            set { m_batch = value; }
        }

        protected string m_Vendor_PN = "";
        public string Vendor_PN
        {
            get { return m_Vendor_PN; }
            set { m_Vendor_PN = value; }
        }

        #endregion

        #region 单位换算
        protected int m_PackTrays = 13;
        public int PackTrays
        {
            get { return m_PackTrays; }
            set { m_PackTrays = value; }
        }

        protected int m_CartonPack = 2;
        public int CartonPack
        {
            get { return m_CartonPack; }
            set { m_CartonPack = value; }
        }

        protected int m_PalletCarton = 48;
        public int PalletCarton
        {
            get { return m_PalletCarton; }
            set { m_PalletCarton = value; }
        }
        #endregion


        public TPCResult<bool> LoadConfig(string config)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            if (!File.Exists(config))
            {
                result.State = RESULT_STATE.NG;
                result.Message = LanguageMapping.Instance.GetWarnningMessage(LabelPrintGlobal.g_LanguageConfig, "CONFIG_FILE_ERROR", LabelPrintGlobal.g_Language);
                return result;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(config);

                #region Constant Parameter
                //PEGATORON P/N
                XmlNode nod = doc.SelectSingleNode("configuration/constant/pegatoron_pn");
                if (nod != null)
                {
                    m_Pegatoron_PN = nod.InnerText;
                }

                //APN
                nod = doc.SelectSingleNode("configuration/constant/apn");
                if (nod != null)
                {
                    m_APN = nod.InnerText;
                }

                //REV
                nod = doc.SelectSingleNode("configuration/constant/rev");
                if (nod != null)
                {
                    m_REV = nod.InnerText;
                }

                //CONFIG
                nod = doc.SelectSingleNode("configuration/constant/config");
                if (nod != null)
                {
                    m_Config = nod.InnerText;
                }

                //DESC
                nod = doc.SelectSingleNode("configuration/constant/desc");
                if (nod != null)
                {
                    m_DESC = nod.InnerText;
                }

                //Stage
                nod = doc.SelectSingleNode("configuration/constant/stage");
                if (nod != null)
                {
                    m_Stage = nod.InnerText;
                }

                //Vendor
                nod = doc.SelectSingleNode("configuration/constant/vendor");
                if (nod != null)
                {
                    m_Vendor = nod.InnerText;
                }

                //Site Code
                nod = doc.SelectSingleNode("configuration/constant/sitecode");
                if (nod != null)
                {
                    m_SiteCode = nod.InnerText;
                }

                //LC
                nod = doc.SelectSingleNode("configuration/constant/lc");
                if (nod != null)
                {
                    m_lc = nod.InnerText;
                }

                //Batch
                nod = doc.SelectSingleNode("configuration/constant/batch");
                if (nod != null)
                {
                    m_batch = nod.InnerText;
                }

                //Vendor P/N 修改日期20180518
                nod = doc.SelectSingleNode("configuration/constant/vendor_pn");
                if (nod != null)
                {
                    m_Vendor_PN = nod.InnerText;
                }
                #endregion

                #region Unit Conersion

                //Pack - Tray
                nod = doc.SelectSingleNode("configuration/conversion/pack-tray");
                if (nod != null)
                {
                    m_PackTrays = Convert.ToInt32(nod.InnerText);
                }

                //Carton - Pack
                nod = doc.SelectSingleNode("configuration/conversion/carton-pack");
                if (nod != null)
                {
                    m_CartonPack = Convert.ToInt32(nod.InnerText);
                }

                //Pallet - Carton
                nod = doc.SelectSingleNode("configuration/conversion/pallet-carton");
                if (nod != null)
                {
                    m_PalletCarton = Convert.ToInt32(nod.InnerText);
                }

                #endregion

                result.Value = true;
                return result;
            }
            catch(Exception e)
            {
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
        }

        public TPCResult<bool> SaveConfig(string config)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            try
            {
                XmlDocument doc = new XmlDocument();
                if (File.Exists(config))
                {
                    doc.Load(config);
                }
                else
                { 
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
                    XmlElement root = doc.CreateElement("configuration");
                    doc.AppendChild(root);
                    root.AppendChild(doc.CreateElement("constant"));
                    root.AppendChild(doc.CreateElement("conversion"));
                }
                
                #region Constant Parameter
                //PEGATORON P/N
                XmlNode nodConstant = doc.SelectSingleNode("configuration/constant");
                XmlNode nod = nodConstant.SelectSingleNode("pegatoron_pn");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("pegatoron_pn"));
                }
                nod.InnerText = m_Pegatoron_PN;

                //APN
                nod = nodConstant.SelectSingleNode("apn");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("apn"));
                }
                nod.InnerText = m_APN;

                //REV
                nod = nodConstant.SelectSingleNode("rev");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("rev"));
                }
                nod.InnerText = m_REV;

                //CONFIG
                nod = nodConstant.SelectSingleNode("config");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("config"));
                }
                nod.InnerText = m_Config;
                
                //DESC
                nod = nodConstant.SelectSingleNode("desc");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("desc"));
                }
                nod.InnerText = m_DESC;

                //Stage
                nod = nodConstant.SelectSingleNode("stage");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("stage"));
                }
                nod.InnerText = m_Stage;

                //Vendor
                nod = nodConstant.SelectSingleNode("vendor");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("vendor"));
                }
                nod.InnerText = m_Vendor;

                //Site Code
                nod = nodConstant.SelectSingleNode("sitecode");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("sitecode"));
                }
                nod.InnerText = m_SiteCode;

                //LC
                nod = nodConstant.SelectSingleNode("lc");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("lc"));
                }
                nod.InnerText = m_lc;

                //Batch
                nod = nodConstant.SelectSingleNode("batch");
                if (nod == null)
                {
                    nod = nodConstant.AppendChild(doc.CreateElement("batch"));
                }
                nod.InnerText = m_batch;
                #endregion

                #region Unit Conersion
                XmlNode nodConversion = doc.SelectSingleNode("configuration/conversion");
                //Pack - Tray
                nod = nodConversion.SelectSingleNode("pack-tray");
                if (nod == null)
                {
                    nod = nodConversion.AppendChild(doc.CreateElement("pack-tray"));
                }
                nod.InnerText = string.Format("{0}", m_PackTrays);

                //Carton - Pack
                nod = nodConversion.SelectSingleNode("carton-pack");
                if (nod == null)
                {
                    nod = nodConversion.AppendChild(doc.CreateElement("carton-pack"));
                }
                nod.InnerText = string.Format("{0}", m_CartonPack);

                //Pallet - Carton
                nod = nodConversion.SelectSingleNode("pallet-carton");
                if (nod == null)
                {
                    nod = nodConversion.AppendChild(doc.CreateElement("pallet-carton"));
                }
                nod.InnerText = string.Format("{0}", m_PalletCarton);
                
                #endregion

                doc.Save(LabelPrintGlobal.g_ConfigFile);
                result.Value = true;
                return result;
            }
            catch (Exception e)
            {
                result.State = RESULT_STATE.NG;
                result.Message = e.Message;
                return result;
            }
        }
    }
}
