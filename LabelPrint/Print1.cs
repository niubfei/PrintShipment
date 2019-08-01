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
    public partial class Print1 : Form
    {
        public Print1(string PKG_ID, string QTY, string DateCode, string LotNo )
        {
            InitializeComponent();
            lblC_PN.Text = LabelPrintGlobal.g_Config.HH;
            lblQTY.Text = QTY;
            lblMfr_PN.Text = LabelPrintGlobal.g_Config.Mfr;
            lblDateCode.Text = DateCode;
            lblLotNo.Text = LotNo;
            lblPKG_ID.Text = PKG_ID;
            ShowQR_Code();
        }

        private void ShowQR_Code()
        {
            //if (lblC_PN.Text == "" || lblQTY.Text == "" || lblMfr_PN.Text == "" || lblDateCode.Text == "" || lblLotNo.Text == "" || lblPKG_ID.Text == "")
            //{ MessageBox.Show("所有信息不能为空"); return; }

            picC_PN.Image = BarcodeHelper.Barcode(lblC_PN.Text, picC_PN.Height);
            picQTY.Image = BarcodeHelper.Barcode(lblQTY.Text, picQTY.Height);
            picMfr_PN.Image = BarcodeHelper.Barcode(lblMfr_PN.Text, picMfr_PN.Height);
            picDateCode.Image = BarcodeHelper.Barcode(lblDateCode.Text, picDateCode.Height);
            picLotNo.Image = BarcodeHelper.Barcode(lblLotNo.Text, picLotNo.Height);
            picPKG_ID.Image = BarcodeHelper.Barcode(lblPKG_ID.Text, picPKG_ID.Height);
            string QRcode =lblC_PN.Text;
            QRcode += "," + lblQTY.Text;
            QRcode += "," + lblMfr_PN.Text;
            QRcode += "," + lblDateCode.Text;
            QRcode += "," + lblLotNo.Text;
            QRcode += "," + lblPKG_ID.Text;
            picQRcode.Image = BarcodeHelper.QRcode(QRcode, picQRcode.Width, picQRcode.Height);
        }

        public bool BtnPrint_Click(System.Drawing.Printing.PrinterSettings setting,string mode)
        {
            //if (printDialog1.ShowDialog() == DialogResult.OK)
            //{
            try
            {
                printDocument1.PrinterSettings = setting;

                switch (mode)
                {
                    case "B":
                        printDocument1.Print();
                        break;
                    case "H":
                        printDocument1.Print();
                        printDocument1.Print();
                        break;
                    case "P":
                        printDocument1.Print();
                        break;
                }
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "打印标签", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            //}
            return false;
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            #region 打印tabPage1
            Bitmap bitmap = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            #endregion
        }
    }
}
