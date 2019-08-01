using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPCCommon.Common
{
    public enum RESULT_STATE
    { 
        NG = 0,
        OK = 1,
    }

    public class TPCResult<T>
    {
        public TPCResult()
        { }

        public TPCResult(T value)
        {
            m_Value = value;
        }

        #region 属性
        protected RESULT_STATE m_State = RESULT_STATE.OK;
        public RESULT_STATE State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        protected string m_Message = "";
        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

        protected T m_Value = default(T);
        public T Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion
    }
}
