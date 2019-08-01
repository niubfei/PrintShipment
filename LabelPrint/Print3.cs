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
    public partial class Print3 : Form
    {
        public Print3(string C_PN, string QTY, string Mfr_PN, string DateCode, string LotNo, string PKG_ID)
        {
            InitializeComponent();
            txtC_PN.Text = C_PN;
            txtQTY.Text = QTY;
            txtMfr_PN.Text = Mfr_PN;
            txtDateCode.Text = DateCode;
            txtLotNo.Text = LotNo;
            txtPKG_ID.Text = PKG_ID;
            ShowQR_Code();
        }

        private void ShowQR_Code()
        {
            if (txtC_PN.Text == "" || txtQTY.Text == "" || txtMfr_PN.Text == "" || txtDateCode.Text == "" || txtLotNo.Text == "" || txtPKG_ID.Text == "")
            { MessageBox.Show("所有信息不能为空"); return; }

            picC_PN.Image = BarcodeHelper.Barcode(txtC_PN.Text, picC_PN.Height);
            picQTY.Image = BarcodeHelper.Barcode(txtQTY.Text, picQTY.Height);
            picMfr_PN.Image = BarcodeHelper.Barcode(txtMfr_PN.Text, picMfr_PN.Height);
            picDateCode.Image = BarcodeHelper.Barcode(txtDateCode.Text, picDateCode.Height);
            picLotNo.Image = BarcodeHelper.Barcode(txtLotNo.Text, picLotNo.Height);
            picPKG_ID.Image = BarcodeHelper.Barcode(txtPKG_ID.Text, picPKG_ID.Height);
            string QRcode = "P" + txtC_PN.Text;
            QRcode += ",Q" + txtQTY.Text;
            QRcode += ",M" + txtMfr_PN.Text;
            QRcode += ",D" + txtDateCode.Text;
            QRcode += ",L" + txtLotNo.Text;
            QRcode += ",S" + txtPKG_ID.Text;
            picQRcode.Image = BarcodeHelper.QRcode(QRcode, picQRcode.Width, picQRcode.Height);
        }

        public void BtnPrint_Click(System.Drawing.Printing.PrinterSettings setting)
        {
            //if (printDialog1.ShowDialog() == DialogResult.OK)
            //{
            try
            {
                printDocument1.PrinterSettings = setting;
                printDocument1.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "打印标签", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //}
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            TxtBorderStyle(BorderStyle.None);
            #region 打印tabPage1
            Bitmap bitmap = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            #endregion
            TxtBorderStyle(BorderStyle.Fixed3D);
        }
        void TxtBorderStyle(BorderStyle borderStyle)
        {
            txtC_PN.BorderStyle = txtQTY.BorderStyle = txtMfr_PN.BorderStyle = txtDateCode.BorderStyle = txtLotNo.BorderStyle = txtPKG_ID.BorderStyle = borderStyle;
        }
    }
}
