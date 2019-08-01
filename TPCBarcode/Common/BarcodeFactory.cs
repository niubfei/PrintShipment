using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPCBarcode.Common
{
    public enum BarcodeType { Code_39, Code_128, Code_QR, Code_QR_Mini};
    public class BarcodeFactory
    {
        private static Code.Code39 m_Code39 = new Code.Code39();
        private static Code.Code128 m_Code128 = new Code.Code128();
        private static Code.CodeQR m_CodeQR = new Code.CodeQR();
        private static Code.CodeQRMini m_CodeQRMini = new Code.CodeQRMini();

        public static IFBarcode CreateBarcode(BarcodeType type)
        {
            IFBarcode barcode = null;
            switch (type)
            { 
                case BarcodeType.Code_39:
                    barcode = m_Code39;
                    break;
                case BarcodeType.Code_128:
                    barcode = m_Code128;
                    break;
                case BarcodeType.Code_QR:
                    barcode = m_CodeQR;
                    break;
                case BarcodeType.Code_QR_Mini:
                    barcode = m_CodeQRMini;
                    break;
            }

            return barcode;
        }
    }
}
