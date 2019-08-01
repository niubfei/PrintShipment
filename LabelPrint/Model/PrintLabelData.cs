using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Model
{
    public class PrintLabelData
    {
        #region 属性
        protected string m_PCode = "";
        public string PCode
        {
            get { return m_PCode; }
            set { m_PCode = value; }
        }

        protected List<string> m_CCode = new List<string>();
        public List<string> CCode
        {
            get { return m_CCode; }
        }

        protected string m_DataCode = "";
        public string DataCode
        {
            get { return m_DataCode; }
            set { m_DataCode = value; }
        }
        //修改日期20180518
        protected string m_Date = "";
        public string Date
        {
            get { return m_Date; }
            set { m_Date = value; }
        }

        protected int m_Quantity = 0;
        public int Quantity
        {
            get { return m_Quantity; }
            set { m_Quantity = value; }
        }

        protected int m_Total = 0;
        public int Total
        {
            get { return m_Total; }
            set { m_Total = value; }
        }
        #endregion
    }
}
