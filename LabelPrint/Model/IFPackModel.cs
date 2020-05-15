using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing.Printing;
using TPCCommon.Common;
using TPCCommon.Database;


namespace LabelPrint.Model
{
    public enum PACK_MODE
    {
        Pack = 0,
        Carton = 1,
        Pallet = 2,
    }

    public enum PACK_ACTION
    { 
        Register = 0,
        Cancel = 1,
    }

    public enum PACK_STATUS
    { 
        Begin = 0,
        Completed = 1,
        Canceled = 2,
    }

    public abstract class IFPackModel
    {
        #region  属性
        protected PackingForm m_Parent = null;
        public PackingForm Parent
        {
            get { return m_Parent; }
        }

        //当前模式：pack，carton，pallet
        protected PACK_MODE m_Mode = PACK_MODE.Pack;
        public PACK_MODE Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        /// <summary>
        /// 数据库模块
        /// </summary>
        protected LabelDatabaseHelper m_Database = null;
        public LabelDatabaseHelper Database
        {
            get { return m_Database; }
            set { m_Database = value; }
        }

        /// <summary>
        /// 申请编码时的关键词
        /// </summary>
        protected string m_CodeKey = "";
        public string CodeKey
        {
            get { return m_CodeKey; }
            set { m_CodeKey = value; }
        }

        #endregion

        #region PackingForm的控制 方法
        /// <summary>
        /// 初始化各个控件
        /// </summary>
        protected void InitCtrl()
        {
            //默认新捆包状态
            Parent.PackingRadioButton.Checked = true;
            Parent.PNoEdit.ReadOnly = true;

            //设定当天为默认捆包日
            Parent.DatePicker.Value = DateTime.Today;

            Parent.PNoEdit.Text = "";
            Parent.CNoEdit.Text = "";
            Parent.QTYEdit.Text = "";

            //Quantity
            Parent.QTYEdit.Text = "0";

            ClearItems();
        }
        protected string GetCodeDate_old()
        {
            DateTime day = Parent.DatePicker.Value;
            string year = string.Format("{0:yyyy}", day).Substring(3);
            GregorianCalendar gc = new GregorianCalendar();
            string week = string.Format("{0:d2}", gc.GetWeekOfYear(day, CalendarWeekRule.FirstDay, DayOfWeek.Sunday));
            return string.Format("{0}{1}{2}", year, week, (int)day.DayOfWeek + 1);
        }

        /// <summary>
        /// 计算编码中日期的值
        /// </summary>
        /// <returns></returns>
        protected string GetCodeDate( int type)
        {
            if (type == 1)
            {
                DateTime day = Parent.DatePicker.Value;
                string year = string.Format("{0:yyyy}", day).Substring(3);
                GregorianCalendar gc = new GregorianCalendar();
                string week = string.Format("{0:d2}", gc.GetWeekOfYear(day, CalendarWeekRule.FirstDay, DayOfWeek.Sunday));
                return string.Format("{0}{1}{2}", year, week, (int)day.DayOfWeek + 1);
            }
            else if (type == 2)
            {
                string CodeDate = DateTime.Today.ToString("yyyyMdd");
                if (CodeDate.Length == 8)
                {
                    string key = CodeDate.Substring(4, 2);
                    switch (key)
                    {
                        case "10":
                            CodeDate = DateTime.Today.ToString("yyyyAdd");
                            break;
                        case "11":
                            CodeDate = DateTime.Today.ToString("yyyyBdd");
                            break;
                        case "12":
                            CodeDate = DateTime.Today.ToString("yyyyCdd");
                            break;
                    }
                }
                return CodeDate;
            }
            else if (type == 3)
            {
                DateTime CodeDate = DateTime.Today;
                string year = CodeDate.ToString("yyyy");
                GregorianCalendar gc = new GregorianCalendar();
                string week = string.Format("{0:d2}", gc.GetWeekOfYear(CodeDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday));
                return string.Format("{0}W{1}", year, week);
            }
            else
                return "";
        }

        /// <summary>
        /// 编码中日期设定修改日期20180518
        /// </summary>
        /// <returns></returns>
        protected string GetDate()
        {
            DateTime day = Parent.DatePicker.Value;
            return string.Format("{0:yyyy/MM/dd}", day);
        }

        /// <summary>
        /// 清空子项列表
        /// </summary>
        protected void ClearItems()
        {
            Parent.ItemsListView.Tag = null;
            Parent.ItemsListView.Items.Clear();
        }

        /// <summary>
        /// 增加子项到列表中
        /// </summary>
        /// <param name="cno"></param>
        /// <param name="date"></param>
        protected void AddItem(string cno, DateTime date)
        {
            ListViewItem item = Parent.ItemsListView.Items.Add(string.Format("{0}", Parent.ItemsListView.Items.Count + 1));
            item.SubItems.Add(cno);
            item.SubItems.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", date));

            int total = int.Parse(Parent.QTYTotalEdit.Text);

            Parent.PrintButton.Enabled = (Parent.ItemsListView.Items.Count == total);
            //修正时间20180517
            Parent.PrintButtonNew.Enabled = (Parent.ItemsListView.Items.Count == total);
            //end
            Parent.PrintButton3.Enabled = (Parent.ItemsListView.Items.Count == total);
        }

        /// <summary>
        /// 批量增加子项
        /// </summary>
        /// <param name="items"></param>
        protected void AddItem(List<CItem> items)
        {
            Parent.ItemsListView.Tag = items;
            foreach (CItem item in items)
            {
                AddItem(item.Code, item.Date);
            }
        }

        /// <summary>
        /// 捆包状态
        /// </summary>
        public void PackingOn()
        {
            InitCtrl();
            Parent.PackingRadioButton.Checked = true;
            Parent.RepackingRadioButton.Checked = false;
            Parent.PNoEdit.ReadOnly = true;
        }

        /// <summary>
        /// 重新捆包状态
        /// </summary>
        public void RepackingOn()
        {
            InitCtrl();
            Parent.PackingRadioButton.Checked = false;
            Parent.RepackingRadioButton.Checked = true;
            Parent.PNoEdit.ReadOnly = false;
        }

        /// <summary>
        /// 检查数量是否达到规定
        /// </summary>
        /// <returns></returns>
        protected bool CheckQuantityIsFull()
        {
            int total = Convert.ToInt32(Parent.QTYTotalEdit.Text);
            if (Parent.ItemsListView.Items.Count == total)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获得打包的项目列表
        /// </summary>
        /// <returns></returns>
        protected List<CItem> GetPackingItems()
        {
            List<CItem> result = new List<CItem>();
            foreach (ListViewItem it in Parent.ItemsListView.Items)
            {
                CItem item = new CItem();
                item.PCode = Parent.PNoEdit.Text;
                item.Code = it.SubItems[1].Text;
                result.Add(item);
            }
            return result;
        }
      

        /// <summary>
        /// 选取打印机
        /// </summary>
        /// <returns></returns>
        protected PrinterSettings GetPrinterSetting()
        {
            PrintDialog dlg = new PrintDialog();
            try
            {                
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.PrinterSettings;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                dlg.Dispose();
            }
        }

        /// <summary>
        /// 从窗口获得标签数据
        /// </summary>
        /// <returns></returns>
        protected PrintLabelData GetLabelData(int type)
        {
            PrintLabelData data = new PrintLabelData();
            data.PCode = Parent.PNoEdit.Text;
            foreach (ListViewItem item in Parent.ItemsListView.Items)
            {
                data.CCode.Add(item.SubItems[1].Text);
            }
            data.DataCode = GetCodeDate(type);
            //修改日期20180518
            data.Date = GetDate();
            data.Quantity = Convert.ToInt32(Parent.QTYEdit.Text);
            data.Total = Convert.ToInt32(Parent.QTYTotalEdit.Text);
            return data;
        }

        /// <summary>
        /// 构建打印标签参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected List<string> MakePrintParameters(PACK_MODE mode, PrintLabelData data)
        {
            List<string> parameters = new List<string>();
            //pegatron P/N 1
            parameters.Add(LabelPrintGlobal.g_Config.Pegatoron_PN);
            //APN 2
            parameters.Add(LabelPrintGlobal.g_Config.APN);
            //REV 3
            parameters.Add(LabelPrintGlobal.g_Config.REV);
            //Config 4
            parameters.Add(LabelPrintGlobal.g_Config.Config);
            //DESC 5
            parameters.Add(LabelPrintGlobal.g_Config.DESC);
            //date 6
            parameters.Add(data.DataCode);
            //L/C 7
            parameters.Add(LabelPrintGlobal.g_Config.lc);
            //Quantity 8
            TPCResult<int> qty = Database.GetModuleCount(mode, data.PCode);
            if (qty.State == RESULT_STATE.NG)
            {
                parameters.Add("ERROR QTY");
            }
            else
            {
                parameters.Add(qty.Value.ToString());
            }
            //Batch 9
            parameters.Add(LabelPrintGlobal.g_Config.batch);
            //Pack No 10
            parameters.Add(data.PCode);
            //stage 11
            parameters.Add(LabelPrintGlobal.g_Config.Stage);
            //2D barcode 12
            string barcode = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                            data.PCode,                                     //pack No
                                            LabelPrintGlobal.g_Config.Pegatoron_PN,         //PEGA P/N
                                            LabelPrintGlobal.g_Config.APN,                  //APN
                                            LabelPrintGlobal.g_Config.batch,                //Batch
                                            qty.Value.ToString(),                           //Quantity
                                            data.DataCode,                                  //Date
                                            LabelPrintGlobal.g_Config.lc);                  //L/C
            //Packの場合、特別処理(Erase Pack id at QR code)、2017/11/14（呉）
            int idx_first = barcode.IndexOf(",");
            int len_barcode = barcode.Length;
            switch (mode)
            {
                case PACK_MODE.Pack:
                    barcode = barcode.Substring(idx_first + 1, len_barcode - idx_first - 1);
                    break;
            }
            parameters.Add(barcode);
            //修改日期20180518
            //13
            parameters.Add(LabelPrintGlobal.g_Config.Vendor_PN);
            //14
            parameters.Add(data.Date);
            //15-46
            KeyValuePair<string, string>[] lotCountArray = new KeyValuePair<string, string>[16];
            TPCResult<List<KeyValuePair<string, string>>> lotCountList = Database.GetLotCount(mode, data.PCode);
            int lotSum = 0;
            if (lotCountList.State == RESULT_STATE.NG)
            {
                for (int i = 0; i < 20; i++)
                {
                    parameters.Add("ERROR");
                }
            }
            else
            {
                Array.Copy(lotCountList.Value.ToArray(), lotCountArray, lotCountList.Value.ToArray().Length);
                for (int i = 0; i < lotCountArray.Length; i++)
                {
                    parameters.Add(lotCountArray[i].Key);
                    parameters.Add(lotCountArray[i].Value);
                    lotSum += Convert.ToInt32(lotCountArray[i].Value);
                }
            }
            //other  47, total 48
            if (lotSum == 0)
            {
                parameters.Add("ERROR");
                parameters.Add("ERROR");
            }
            else
            {
                TPCResult<int> lotTotal = Database.GetLotTotal(mode, data.PCode);
                if (lotTotal.State == RESULT_STATE.NG)
                {
                    parameters.Add("ERROR");
                    parameters.Add("ERROR");
                }
                else
                {
                    int other  = lotTotal.Value - lotSum;
                    parameters.Add(other.ToString());
                    parameters.Add(lotTotal.Value.ToString());
                }
            }
            lotSum = 0;
            //49
            parameters.Add(LabelPrintGlobal.g_Config.Mfr);
            //end
            return parameters;
            
        }
        #endregion

        #region 共通方法
        /// <summary>
        /// 显示不同模式下的窗口 title
        /// </summary>
        public abstract void InitLanguage();
        /// <summary>
        /// 申请新编号
        /// </summary>
        public abstract TPCResult<bool> ApplyNewCode();

        /// <summary>
        /// 子编号编辑框，收到扫描条码事件
        /// 增加子项到列表中，并保存到数据库
        /// </summary>
        /// <param name="code"></param>
        public abstract TPCResult<bool> ScanCCode(string code);

        public abstract TPCResult<System.Data.DataTable> CheckBin( string code);

        /// <summary>
        /// 父编号编辑框，收到扫描条码后事件
        /// 逻辑上认为扫描的条码为子项，需要从数据库中检索出父项条码
        /// 并显示到父编号编辑框中，同时设定只读属性，切换状态
        /// </summary>
        /// <param name="code"></param>
        public abstract TPCResult<bool> ScanPCode(string code);

        /// <summary>
        /// 打印标签
        /// </summary>
        public abstract TPCResult<bool> PrintLabel1();
        /// <summary>
        /// 修改日期20180517
        /// </summary>
        public abstract TPCResult<bool> PrintLabel2();
        #endregion

        /// <summary>
        /// 新客户20190617
        /// </summary>
        /// <returns></returns>
        public abstract TPCResult<bool> PrintLabel3();
    }
}
