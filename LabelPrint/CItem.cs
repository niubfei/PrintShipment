using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint
{
    public class CItem
    {
        protected string m_Name = "";
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        protected string m_PCode = "";
        public string PCode
        {
            get { return m_PCode; }
            set { m_PCode = value; }
        }

        protected string m_Code = "";
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        protected DateTime m_Date = DateTime.MinValue;
        public DateTime Date
        {
            get { return m_Date; }
            set { m_Date = value; }
        }

        protected int m_Status = 0;
        public int Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
    }
}
