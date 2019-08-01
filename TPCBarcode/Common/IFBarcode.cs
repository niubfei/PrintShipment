using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TPCBarcode.Common
{
    public abstract class IFBarcode
    {
        #region 属性
        protected float m_Height = 0.0F;
        public float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        protected float m_Width = 0.0F;
        public float Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        protected bool m_Is2DBarcode = false;
        public bool Is2DBarcode
        {
            get { return m_Is2DBarcode; }
            set { m_Is2DBarcode = value; }
        }
        #endregion

        abstract public string Encode(string text);

        abstract public Image Create(string text);

        /// <summary>
        /// 计算mm单位转pixel单位的barcode图像大小
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        protected Size CalcImageSize()
        {
            SizeF dpi = GDIAPI.GetDeviceDPI();
            return new Size((int)(dpi.Width * m_Width / 25.4f), (int)(dpi.Height * m_Height / 25.4f));
        }

        /// <summary>
        /// 创建条码图像
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected Image CreateBarcodeImage(string text)
        {
            //计算图像像素大小 
            Size szImage = CalcImageSize();

            //生成条码编码
            string code = Encode(text);

            //建立画布
            Bitmap bmp = new Bitmap(szImage.Width, szImage.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                ///设定白色背景
                g.Clear(Color.White);

                //计算基础线宽
                int line_width = szImage.Width / code.Length;
                if (line_width == 0)
                {
                    //确定最小像素线宽
                    line_width = 1;
                }
                if (line_width > 3)
                {
                    //确定最大像素线宽
                    line_width = 3;
                }

                ///设定画笔
                Pen pen = new Pen(Color.Black, line_width);
                pen.Alignment = PenAlignment.Right;

                //绘制一维条码
                int pos = 0;            ///条码编码位
                while (pos < code.Length)
                {
                    //编码中是 1， 则画一个基准单位的竖线
                    if (code[pos] == '1')
                    {
                        g.DrawLine(pen, new Point(pos * line_width, 0), new Point(pos * line_width, szImage.Height));
                    }

                    pos++;
                }//while
            }

            return bmp;
        }
    }
}
