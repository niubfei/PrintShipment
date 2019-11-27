using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using TPCBarcode.Common;

namespace TPCBarcode.LabelPrint
{
    public class TPCPrintLabel
    {
        protected string m_Name = "";
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        protected float m_MarginWidth = 5.0f;
        public float MarginWidth
        {
            get { return m_MarginWidth; }
            set { m_MarginWidth = value; }
        }

        protected float m_LineWidth = 0.2f;
        public float LineWidth
        {
            get { return m_LineWidth; }
            set { m_LineWidth = value; }
        }

        protected List<LabelItem> m_Items = new List<LabelItem>();
        public List<LabelItem> Items
        {
            get { return m_Items; }
        }

        public TPCPrintLabel Clone()
        {
            TPCPrintLabel clone = new TPCPrintLabel();
            clone.m_Name = m_Name;
            clone.m_LineWidth = m_LineWidth;
            clone.m_MarginWidth = m_MarginWidth;
            clone.Items.Clear();
            foreach (LabelItem item in m_Items)
            {
                LabelItem new_item = new LabelItem();
                #region 设定属性
                new_item.Barcode = item.Barcode;
                new_item.BarcodeHeight = item.BarcodeHeight;
                new_item.BarcodeWidth = item.BarcodeWidth;
                new_item.Border = item.Border;
                new_item.Height = item.Height;
                new_item.IsImage = item.IsImage;
                new_item.LineWidth = item.LineWidth;
                new_item.Offset = item.Offset;
                new_item.Position = item.Position;
                new_item.State = item.State;
                new_item.Text = item.Text;
                new_item.TextFont = item.TextFont;
                new_item.Width = item.Width;
                #endregion
                clone.Items.Add(new_item);
            }
            return clone;
        }

        protected void PrintLabel(Graphics g)
        {
            g.PageUnit = GraphicsUnit.Millimeter;
            g.SmoothingMode = SmoothingMode.HighQuality;
            foreach (LabelItem item in m_Items)
            {
                item.LineWidth = m_LineWidth;
                item.DrawItem(g);
            }
        }

        protected void SetDynamicParameters(List<string> parameters)
        {
            foreach (LabelItem item in m_Items)
            {
                if (item.State == TextState.state_dynamic)
                {
                    MatchCollection matches = Regex.Matches(item.Text, @"\$\$\d+");
                    //foreach (Match match in matches)
                    //{
                    //    int index = 0;
                    //    //如果$1是A，要是字符串$1$10$12,会变成AA0A2
                    //    if (int.TryParse(match.Value.Replace("$$", ""), out index))
                    //    {
                    //        string param = parameters[index - 1];
                    //        item.Text = item.Text.Replace(match.Value, param);
                    //    }
                    //}

                    int[] intArray = new int[matches.Count];                    
                    for (int i = 0; i < matches.Count; i++)
                    {
                        intArray[i] =Convert.ToInt32(matches[i].Groups[0].Value.Replace("$$", ""));
                    }
                    Array.Sort(intArray);
                    Array.Reverse(intArray);

                    foreach (int intStr in intArray)
                    {
                        string param = parameters[intStr - 1];
                        item.Text = item.Text.Replace("$$"+intStr, param);
                    }
                }
            }
        }

        protected PrintDocument m_Printer = null;


        public void Print(PrinterSettings setting, List<string> parameters)
        {
            if (m_Printer == null)
            {
                m_Printer = new PrintDocument();
                m_Printer.PrintPage += this.OnPrint;
            }
            if (setting != null)
            {
                m_Printer.PrinterSettings = setting;
            }

            if (m_Printer.PrinterSettings.IsValid)
            {
                SetDynamicParameters(parameters);
                m_Printer.Print();
            }
        }

        private void OnPrint(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(m_MarginWidth, m_MarginWidth);
            //Image imgBarcode = Image.FromFile(@"C:\Users\yyx19\Desktop\测试图片\RoHS.png");
            //g.DrawImage(imgBarcode, new Point(150, 13));
            //imgBarcode = Image.FromFile(@"C:\Users\yyx19\Desktop\测试图片\HF.png");
            //g.DrawImage(imgBarcode, new Point(150, 66));

            PrintLabel(g);
            ////使用Rectangle定义椭圆的边界，位置在（30,30）宽150，高70
            //Rectangle Rec = new Rectangle(50, 20, 30, 14);
            ////使用DrawEllipse绘制椭圆
            //g.DrawEllipse(new Pen(Color.Black), Rec);

            g.TranslateTransform(-m_MarginWidth, -m_MarginWidth);
            e.HasMorePages = false;
        }
    }
}
