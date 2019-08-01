using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TPCBarcode.Common;

namespace TPCBarcode.LabelPrint
{
    public enum BorderStyle
    { 
        border_none     = 0x00000000,
        border_left     = 0x00000001,
        border_right    = 0x00000002,
        border_top      = 0x00000004,
        border_bottom   = 0x00000008,
    }

    public enum TextState
    { 
        state_fixed = 0,
        state_dynamic = 1,
    }

    //绘制的最小单元，这里绘制的单位采用毫米
    public class LabelItem
    {
        /// <summary>
        /// Label单元的相对位置
        /// </summary>
        protected PointF m_Offset = new PointF(0.0f, 0.0f);
        public PointF Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        #region 属性
        /// <summary>
        /// 条码编码生成器
        /// </summary>
        protected IFBarcode m_Barcode = null;
        public IFBarcode Barcode
        {
            get { return m_Barcode; }
            set { m_Barcode = value; }
        }
        /// <summary>
        /// 边框线宽度
        /// </summary>
        protected float m_LineWidth = 0.2f;
        public float LineWidth
        {
            get { return m_LineWidth; }
            set { m_LineWidth = value; }
        }
        /// <summary>
        /// Label单元宽度
        /// </summary>
        protected float m_Width = 0.0f;
        public float Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        /// <summary>
        /// Label单元高度
        /// </summary>
        protected float m_Height = 0.0f;
        public float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        /// <summary>
        /// 条码（图像或文字）宽度
        /// </summary>
        protected float m_BarcodeWidth = 0.0f;
        public float BarcodeWidth
        {
            get { return m_BarcodeWidth; }
            set { m_BarcodeWidth = value; }
        }
        /// <summary>
        /// 条码（图像或文字）高度
        /// </summary>
        protected float m_BarcodeHeight = 0.0f;
        public float BarcodeHeight
        {
            get { return m_BarcodeHeight; }
            set { m_BarcodeHeight = value; }
        }
        /// <summary>
        /// 是否绘制条码图像
        /// </summary>
        protected bool m_IsImage = false;
        public bool IsImage
        {
            get { return m_IsImage; }
            set { m_IsImage = value; }
        }
        /// <summary>
        /// 条码内容
        /// </summary>
        protected string m_Text = "";
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
        /// <summary>
        /// 设定条码内容是否固定，如果是动态则由标签参数表决定其内容
        /// </summary>
        protected TextState m_State = TextState.state_fixed;
        public TextState State
        {
            get { return m_State; }
            set { m_State = value; }
        }
        /// <summary>
        /// 边界样式
        /// </summary>
        protected BorderStyle m_Border = 0x00;
        public BorderStyle Border
        {
            get { return m_Border; }
            set { m_Border = value; }
        }
        /// <summary>
        /// 条码（图像或文字）的绘制起始点
        /// </summary>
        protected PointF m_Position = new PointF(0.0f, 0.0f);
        public PointF Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        /// <summary>
        /// 条码文字的字体
        /// </summary>
        protected Font m_TextFont = null;
        public Font TextFont
        {
            get { return m_TextFont; }
            set { m_TextFont = value; }
        }

        #endregion

        //绘制基本单元
        public void DrawItem(Graphics g)
        {
            float BORDER_MARGIN = 0.0f;

            //设定坐标原点为相对位置原点
            g.TranslateTransform(m_Offset.X, m_Offset.Y);

            //绘制条码
            if (m_IsImage && m_Text.Trim().Length > 0)
            { 
                //是绘制条码图像
                m_Barcode.Width = m_BarcodeWidth;
                m_Barcode.Height = m_BarcodeHeight;
                Image imgBarcode = m_Barcode.Create(m_Text);

                g.DrawImage(imgBarcode, m_Position);
            }
            else
            { 
                //是绘制条码文字
                Brush brText = new SolidBrush(Color.Black);
                g.DrawString(m_Text, m_TextFont, brText, m_Position);
            }

            //绘制边框
            #region 绘制边框
            Pen pnBorder = new Pen(Color.Black, m_LineWidth);
            if ((m_Border & BorderStyle.border_left) == BorderStyle.border_left)
            {
                //绘制左边框
                g.DrawLine(pnBorder, new PointF(0.0f, 0.0f), new PointF(0.0f, m_Height));
            }
            if ((m_Border & BorderStyle.border_right) == BorderStyle.border_right)
            {
                //绘制右边框
                g.DrawLine(pnBorder, new PointF(m_Width - BORDER_MARGIN, 0.0f), new PointF(m_Width - BORDER_MARGIN, m_Height));
            }
            if ((m_Border & BorderStyle.border_top) == BorderStyle.border_top)
            {
                //绘制顶边框
                g.DrawLine(pnBorder, new PointF(0.0f, 0.0f), new PointF(m_Width, 0.0f));
            }
            if ((m_Border & BorderStyle.border_bottom) == BorderStyle.border_bottom)
            {
                //绘制底边框
                g.DrawLine(pnBorder, new PointF(0.0f, m_Height - BORDER_MARGIN), new PointF(m_Width, m_Height - BORDER_MARGIN));
            }
            #endregion

            //恢复坐标原点
            g.TranslateTransform(-m_Offset.X, -m_Offset.Y);
        }
    }
}
