using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using ThoughtWorks.QRCode;
using ThoughtWorks.QRCode.Codec;

namespace TPCBarcode.Common.Code
{
    public class CodeQRMini : IFBarcode
    {
        public CodeQRMini()
        {
            m_Is2DBarcode = true;
        }

        //protected int m_Version = 1;        //QR Version
        //protected int m_ModuleQty = 0;      //QR 2D Image modules

        static int[] VERSION_TEXT_LENGTH = new int[]{0, 26, 44, 70, 100, 143, 176, 192, 242, 292, 346, 404, 466,
                                                    532, 581, 655, 733, 815, 901, 991, 1085, 1156, 1258, 1364,
                                                    1474, 1588, 1706, 1828, 1921, 2051, 2185, 2323, 2465, 2611,
                                                    2761, 2876, 3034, 3196, 3362, 3532, 3706, };

        public override string Encode(string text)
        {
            int ver = CalcVersion(text);
            if (ver == -1)
            {
                return "";
            }
            //m_Version = ver;
            //m_ModuleQty = (ver - 1) * 4 + 21;

            QRCodeEncoder coder = new QRCodeEncoder();
            coder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            coder.QRCodeScale = 1;
            coder.QRCodeVersion = ver;
            Bitmap origin = coder.Encode(text);

            int size = (ver - 1) * 4 + 21;          //计算2D条码模块数
            int pixel = origin.Width / size;       //计算每个模块的像素点

            StringBuilder ss = new StringBuilder();
            for (int x = 0; x < origin.Width; x += pixel)
            {
                if (ss.Length > 0)
                    ss.Append(",");

                for (int y = 0; y < origin.Height; y += pixel)
                {
                    if (origin.GetPixel(x, y).ToArgb() == Color.White.ToArgb())
                    {
                        ss.Append("0");
                    }
                    else
                    {
                        ss.Append("1");
                    }
                }
            }
            return ss.ToString();
        }

        protected int CalcVersion(string text)
        {
            for (int i = 0; i < VERSION_TEXT_LENGTH.Length; i++)
            {
                if (text.Length < VERSION_TEXT_LENGTH[i])
                {
                    return i + 1;
                }
            }
            return -1;
        }

        public override Image Create(string text)
        {
            QRCodeEncoder coder = new QRCodeEncoder();
            coder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            coder.QRCodeScale = 1;
            coder.QRCodeVersion = 2;
            return coder.Encode(text);
        }
    }
}
