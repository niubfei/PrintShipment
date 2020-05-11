using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrint
{
    public partial class Print2 : Form
    {
        string ID, QTY, QRcode;
        DataTable DT;
        public Print2(string ID,string QTY, string DateCode, string Lot)
        {
            InitializeComponent();
            //lblID.Text = ID;
            //lblAPN.Text = LabelPrintGlobal.g_Config.APN;
            //lblQTY.Text = QTY;
            //ShowQR_Code(QTY, DateCode,Lot, ID);

            this.ID = ID;
            this.QTY = QTY;
            QRcode = LabelPrintGlobal.g_Config.HH
               + "," + QTY
               + "," + LabelPrintGlobal.g_Config.Mfr
               + "," + DateCode
               + "," + Lot
               + "," + ID;
        }

        public void ImportDataTable(DataTable dt)
        {
            //dgv.DataSource = dt;
            DT = dt;
        }

        public bool BtnPrint_Click(System.Drawing.Printing.PrinterSettings setting)
        {
            try
            {
                printDocument1.PrinterSettings =  setting;
                printDocument1.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "打印标签", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }



        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            #region 打印tabPage1
            //Bitmap bitmap = new Bitmap(pnl.Width, pnl.Height);
            //pnl.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            //e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            #endregion



            //Font f = new Font("Verdana", 8f);
            Graphics g = e.Graphics;
            Font f = new Font("宋体", 9f);
            Brush b = new SolidBrush(Color.Black);
            g.DrawString("Pallet Detail", new Font("宋体", 30f), b, new Point(275, 10));
            g.DrawString(ID, new Font("宋体", 15f), b, new Point(320, 60));
            TPCBarcode.Common.Code.CodeQR_2cm pic = new TPCBarcode.Common.Code.CodeQR_2cm();
            pic.Width = pic.Height = 77;//2cm*2cm的二维码
            Image img = pic.Create(QRcode);
            g.DrawImage(img, new Point(650, 30));

            int x1 = 30, y2 = 150, xAdd2 = 170, yAdd2 = 20;
            g.DrawString("No.", f, b, new Point(x1 + xAdd2 * 0, y2));
            g.DrawString("箱号", f, b, new Point(x1 + xAdd2 * 1, y2));
            g.DrawString("料号", f, b, new Point(x1 + xAdd2 * 2, y2));
            g.DrawString("数量", f, b, new Point(x1 + xAdd2 * 3, y2));

            int i = 0;
            foreach (DataRow dr in DT.Rows )
            {
                i++;
                g.DrawString(dr[0].ToString(), f, b, new Point(x1 + xAdd2 * 0, y2 + yAdd2 * i));
                g.DrawString(dr[1].ToString(), f, b, new Point(x1 + xAdd2 * 1, y2 + yAdd2 * i));
                g.DrawString(dr[2].ToString(), f, b, new Point(x1 + xAdd2 * 2, y2 + yAdd2 * i));
                g.DrawString(dr[3].ToString(), f, b, new Point(x1 + xAdd2 * 3, y2 + yAdd2 * i));
            }
            #region 测试行数
            ////生产需要48行，测试写50行，前48行正常显示，49行显示不完整
            //for (int i=1;i<50;i++)
            //{
            //    g.DrawString(i.ToString(), f, b, new Point(x2 + xAdd2 * 0, y2 + yAdd2 * i));
            //    g.DrawString(i.ToString(), f, b, new Point(x2 + xAdd2 * 1, y2 + yAdd2 * i));
            //    g.DrawString(i.ToString(), f, b, new Point(x2 + xAdd2 * 2, y2 + yAdd2 * i));
            //    g.DrawString(i.ToString(), f, b, new Point(x2 + xAdd2 * 3, y2 + yAdd2 * i));
            //}
            #endregion

            int x2 = 70, y1 = 100, xAdd1 = 170, yAdd1 = 20;
            g.DrawString("Item", f, b, new Point(x2 + xAdd1 * 0, y1));
            g.DrawString("料号", f, b, new Point(x2 + xAdd1 * 1, y1));
            g.DrawString("大箱数量", f, b, new Point(x2 + xAdd1 * 2, y1));
            g.DrawString("QTY", f, b, new Point(x2 + xAdd1 * 3, y1));
            
            g.DrawString("1", f, b, new Point(x2 + xAdd1 * 0, y1+yAdd1));            
            g.DrawString(LabelPrintGlobal.g_Config.APN, f, b, new Point(x2 + xAdd1 * 1, y1 + yAdd1));
            g.DrawString(i.ToString(), f, b, new Point(x2 + xAdd1 * 2, y1 + yAdd1));
            g.DrawString(QTY, f, b, new Point(x2 + xAdd1 * 3, y1 + yAdd1));
        }

        private void Print2_Load(object sender, EventArgs e)
        {
            //1024分辨率最高窗体高度是1044
            //this.Height = 1208;
            //this.Width = 3000;
            //this.Height = dgv.Height = 2000;
            dgv.ClearSelection();
            dgv.Columns[0].Width = 150;
            dgv.Columns[1].Width = 150;
            dgv.Columns[2].Width = 150;
            dgv.Columns[3].Width = 150;
        }
    }
}
