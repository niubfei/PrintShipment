using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using TPCBarcode.Common;

namespace TPCBarcode.LabelPrint
{
    public class LabelCreator
    {
        protected Dictionary<string, TPCPrintLabel> m_Labels = new Dictionary<string, TPCPrintLabel>();
    

        public bool LoadConfig(string config)
        {
            if (!File.Exists(config))
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(config);

                XmlNodeList nods = doc.SelectNodes("labels/label");
                foreach (XmlNode nod in nods)
                {
                    if (nod.Attributes["name"] != null)
                    {
                        TPCPrintLabel label = new TPCPrintLabel();
                        label.Name = nod.Attributes["name"].Value;
                        //读取margin
                        if (nod.Attributes["margin"] != null)
                        {
                            label.MarginWidth = Convert.ToSingle(nod.Attributes["margin"].Value);
                        }
                        //读取线宽
                        if (nod.Attributes["line-width"] != null)
                        {
                            label.LineWidth = Convert.ToSingle(nod.Attributes["line-width"].Value);
                        }

                        XmlNodeList nodItems = nod.SelectNodes("item");
                        foreach (XmlNode nodItem in nodItems)
                        {
                            LabelItem item = new LabelItem();
                            if (nodItem.Attributes["text"] != null)
                            {
                                item.Text = nodItem.Attributes["text"].Value;
                            }
                            if (nodItem.Attributes["state"] != null)
                            {
                                //item.State = (nodItem.Attributes["state"].Value.Equals("fixed")) ? TextState.state_fixed : TextState.state_dynamic;
                                switch (nodItem.Attributes["state"].Value)
                                {
                                    case "iconRoHS":
                                        item.State = TextState.state_iconRoHS;
                                        break;
                                    case "iconHF":
                                        item.State = TextState.state_iconHF;
                                        break;
                                    case "fixed":
                                        item.State = TextState.state_fixed;
                                        break;
                                    case "dynamic":
                                    default:
                                        item.State = TextState.state_dynamic;
                                        break;
                                }
                            }
                            if (nodItem.Attributes["image"] != null)
                            {
                                item.IsImage = (nodItem.Attributes["image"].Value.Equals("false")) ? false : true;
                            }
                            if (nodItem.Attributes["offset"] != null)
                            {
                                string[] val = nodItem.Attributes["offset"].Value.Split(',');
                                item.Offset = new PointF(Convert.ToSingle(val[0]), Convert.ToSingle(val[1]));
                            }
                            if (nodItem.Attributes["position"] != null)
                            {
                                string[] val = nodItem.Attributes["position"].Value.Split(',');
                                item.Position = new PointF(Convert.ToSingle(val[0]), Convert.ToSingle(val[1]));
                            }
                            if (nodItem.Attributes["size"] != null)
                            {
                                string[] val = nodItem.Attributes["size"].Value.Split(',');
                                item.Width = Convert.ToSingle(val[0]);
                                item.Height = Convert.ToSingle(val[1]);
                            }
                            if (nodItem.Attributes["border"] != null)
                            {
                                string[] val = nodItem.Attributes["border"].Value.Split('|');
                                item.Border = BorderStyle.border_none;
                                foreach (string v in val)
                                {
                                    if (v.Equals("left"))
                                        item.Border = item.Border | BorderStyle.border_left;
                                    else if (v.Equals("right"))
                                        item.Border = item.Border | BorderStyle.border_right;
                                    else if (v.Equals("top"))
                                        item.Border = item.Border | BorderStyle.border_top;
                                    else if (v.Equals("bottom"))
                                        item.Border = item.Border | BorderStyle.border_bottom;
                                }
                            }

                            if (item.IsImage)
                            {
                                //读入Code信息
                                XmlNode nodCode = nodItem.SelectSingleNode("code");
                                if (nodCode.InnerText.Equals("code39"))
                                {
                                    item.Barcode = BarcodeFactory.CreateBarcode(BarcodeType.Code_39);
                                }
                                else if (nodCode.InnerText.Equals("code128"))
                                {
                                    item.Barcode = BarcodeFactory.CreateBarcode(BarcodeType.Code_128);
                                }
                                else if (nodCode.InnerText.Equals("codeQR"))
                                {
                                    item.Barcode = BarcodeFactory.CreateBarcode(BarcodeType.Code_QR);
                                }
                                else if (nodCode.InnerText.Equals("codeQRMini"))
                                {
                                    item.Barcode = BarcodeFactory.CreateBarcode(BarcodeType.Code_QR_Mini);
                                }
                            }
                            else
                            {
                                //读入Font信息
                                XmlNode nodFont = nodItem.SelectSingleNode("font");
                                string face = nodFont.Attributes["face"].Value;
                                int size = Convert.ToInt32(nodFont.Attributes["size"].Value);

                                item.TextFont = new Font(face, size, FontStyle.Regular);
                            }
                            //读取条码Size
                            XmlNode nodSize = nodItem.SelectSingleNode("size");
                            if (nodSize != null)
                            {
                                string[] val = nodSize.InnerText.Split(',');
                                item.BarcodeWidth = Convert.ToSingle(val[0]);
                                item.BarcodeHeight = Convert.ToSingle(val[1]);
                            }

                            label.Items.Add(item);
                        }
                        m_Labels.Add(label.Name, label);
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public TPCPrintLabel GetPrintLabel(string name)
        {
            if (m_Labels.ContainsKey(name))
            {
                TPCPrintLabel template = m_Labels[name];
                return template.Clone();
            }
            else
            {
                return null;
            }
        }
    }
}
