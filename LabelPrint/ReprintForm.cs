using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;
using LabelPrint.Model;
using TPCCommon.Common;
using TPCCommon.Database;
using TPCBarcode.LabelPrint;

namespace LabelPrint
{
    public partial class ReprintForm : Form
    {
        public ReprintForm()
        {
            InitializeComponent();            
        }

        protected override void OnLoad(EventArgs e)
        {
            InitLanguage();
        }

        public void InitLanguage()
        {
            lbScan.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_CODE", LabelPrintGlobal.g_Language);
            lbDate.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_DATE", LabelPrintGlobal.g_Language);
            lbQty.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_QTY", LabelPrintGlobal.g_Language);

            string[] columns_header = new string[] { "REPRINT_ITEM_NO", "REPRINT_ITEM_CODE", "REPRINT_ITEM_DATE" };
            for (int i = 0; i < lstItems.Columns.Count; i++)
            {
                ColumnHeader col = lstItems.Columns[i];
                string key = columns_header[i];
                col.Text = LanguageMapping.Instance.GetStaticMessage(key, LabelPrintGlobal.g_Language);
            }

            btReprint.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_PRINT", LabelPrintGlobal.g_Language);
            btReprintNew.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_PRINT_NEW", LabelPrintGlobal.g_Language);
            btReprint3.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_PRINT3", LabelPrintGlobal.g_Language);
            btClose.Text = LanguageMapping.Instance.GetStaticMessage("REPRINT_BT_CLOSE", LabelPrintGlobal.g_Language);
        }

        protected LabelDatabaseHelper m_Database = null;
        public LabelDatabaseHelper Database
        {
            get { return m_Database; }
            set { m_Database = value; }
        }

        protected PACK_MODE m_Mode = PACK_MODE.Pack;
        public PACK_MODE Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                //从数据库综合视图vw_packing中读取条码的有效性和所属模式
                string code = txtCode.Text;
                TPCResult<List<CItem>> items = Database.GetItemsByPCode(code);
                if (items.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(items.Message);
                    return;
                }
                FillItems(items.Value);
            }
        }

        protected void FillItems(List<CItem> items)
        {
            lstItems.Items.Clear();
            lstItems.Tag = items;
            foreach (CItem item in items)
            {
                ListViewItem it = lstItems.Items.Add(string.Format("{0}", lstItems.Items.Count + 1));
                it.Tag = item;
                it.SubItems.Add(item.Code);
                it.SubItems.Add(item.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            //设定数量
            txtQty.Text = string.Format("{0}", lstItems.Items.Count);
            if (items.Count > 0)
            {
                if (items[0].Name.Equals("pnt_pack"))
                {
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PackTrays);
                    m_Mode = PACK_MODE.Pack;
                }
                else if (items[0].Name.Equals("pnt_carton"))
                {
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.CartonPack);
                    m_Mode = PACK_MODE.Carton;
                }
                else if (items[0].Name.Equals("pnt_pallet"))
                {
                    txtTotal.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PalletCarton);
                    m_Mode = PACK_MODE.Pallet;
                }
            }

        }

        protected string GetCodeDate()
        {
            DateTime day = dtPicker.Value;
            string year = string.Format("{0:yyyy}", day).Substring(3);
            GregorianCalendar gc = new GregorianCalendar();
            string week = string.Format("{0:d2}", gc.GetWeekOfYear(day, CalendarWeekRule.FirstDay, DayOfWeek.Sunday));

            return string.Format("{0}{1}{2}", year, week, (int)day.DayOfWeek + 1);
        }

        protected string GetDate()
        {
            DateTime day = dtPicker.Value;
            return string.Format("{0:yyyy/MM/dd}", day);
        }

        protected List<string> MakePrintParameters(PACK_MODE mode, PrintLabelData data)
        {
            List<string> parameters = new List<string>();
            //pegatron P/N
            parameters.Add(LabelPrintGlobal.g_Config.Pegatoron_PN);
            //APN
            parameters.Add(LabelPrintGlobal.g_Config.APN);
            //REV
            parameters.Add(LabelPrintGlobal.g_Config.REV);
            //Config
            parameters.Add(LabelPrintGlobal.g_Config.Config);
            //DESC
            parameters.Add(LabelPrintGlobal.g_Config.DESC);
            //date
            parameters.Add(data.DataCode);
            //L/C
            parameters.Add(LabelPrintGlobal.g_Config.lc);
            //Quantity
            parameters.Add(data.Quantity.ToString());
            //Batch
            parameters.Add(LabelPrintGlobal.g_Config.batch);
            //Pack No
            parameters.Add(data.PCode);
            //stage
            parameters.Add(LabelPrintGlobal.g_Config.Stage);
            //2D barcode
            string barcode = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                            data.PCode,                                     //pack No
                                            LabelPrintGlobal.g_Config.Pegatoron_PN,         //PEGA P/N
                                            LabelPrintGlobal.g_Config.APN,                  //APN
                                            LabelPrintGlobal.g_Config.batch,                //Batch
                                            data.Quantity.ToString(),                       //Quantity
                                            data.DataCode,                                  //Date
                                            LabelPrintGlobal.g_Config.lc);                  //L/C
            //Packの場合、特別処理(Erase Pack id at QR code)、2017/11/14（呉）
            int idx_first=barcode.IndexOf(",");
            int len_barcode=barcode.Length;
            switch (mode)
            {   
                case PACK_MODE.Pack:
                    barcode = barcode.Substring(idx_first+1,len_barcode-idx_first-1);
                    break;
            }
            parameters.Add(barcode);
            //修改日期20180518
            //13
            parameters.Add(LabelPrintGlobal.g_Config.Vendor_PN);
            //14
            parameters.Add(data.Date);
            KeyValuePair<string, string>[] lotCountArray = new KeyValuePair<string, string>[16];
            TPCResult<List<KeyValuePair<string, string>>> lotCountList = Database.GetLotCount(mode, data.PCode);
            int lotSum = 0;
            if (lotCountList.State == RESULT_STATE.NG)
            {
                for (int i = 0; i < 8; i++)
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
                    int other = lotTotal.Value - lotSum;
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

        private void btReprint_Click(object sender, EventArgs e)
        {
            reprint1();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lstItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            StringBuilder ss = new StringBuilder();
            for (int i = 0; i < lstItems.SelectedItems.Count; i++)
            {
                ListViewItem item = lstItems.SelectedItems[i];
                if (i > 0)
                {
                    ss.Append("\r\n");
                }

                ss.Append(string.Format("{0},{1},{2}", item.Text, item.SubItems[1].Text, item.SubItems[2].Text));
            }

            Clipboard.SetText(ss.ToString());
        }
        //修改日期20180518
        private void btReprintNew_Click(object sender, EventArgs e)
        {
            reprint2();
        }
        private void BtReprint3_Click(object sender, EventArgs e)
        {
            reprint3();
        }
        private void reprint1()
        {
            if (lstItems.Items.Count < Convert.ToInt32(txtTotal.Text))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NOT_FULL_QUANTITY_ERROR"), "ERROR", MessageBoxButtons.OK);
                return;
            }

            if (!"123456789ABC".Contains(txtCode.Text.Substring(10, 1)))
            {
                MessageBox.Show("该ID格式错误", "ID格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;

            }
            PrinterSettings setting = dlg.PrinterSettings;

            PrintLabelData data = new PrintLabelData();
            data.PCode = txtCode.Text;
            data.DataCode = GetCodeDate();
            data.Total = Convert.ToInt32(txtTotal.Text);
            data.Date = GetDate();
            TPCResult<int> result = Database.GetModuleCount(m_Mode, txtCode.Text);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                return;
            }

            data.Quantity = result.Value;

            string label_nameFrist = "";
            string label_nameSecond = "";
            switch (m_Mode)
            {
                case PACK_MODE.Pack:
                    label_nameFrist = "pack";
                    label_nameSecond = "pack_pega";
                    break;
                case PACK_MODE.Carton:
                    label_nameFrist = "carton";
                    label_nameSecond = "carton_pega";
                    break;
                case PACK_MODE.Pallet:
                    label_nameFrist = "pallet";
                    label_nameSecond = "pallet_pega";
                    break;
            }
            //打印第一页信息
            TPCPrintLabel labelFrist = LabelPrintGlobal.g_LabelCreator.GetPrintLabel(label_nameFrist);
            List<string> parametersFrist = MakePrintParameters(m_Mode, data);
            
            //打印第二页信息
            TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel(label_nameSecond);
            List<string> parametersSecond = MakePrintParameters(m_Mode, data);            

            switch (m_Mode)
            {
                case PACK_MODE.Pack:
                    labelFrist.Print(setting, parametersFrist);
                    labelSecond.Print(setting, parametersSecond);
                    break;
                case PACK_MODE.Carton:
                    labelFrist.Print(setting, parametersFrist);
                    //labelFrist.Print(setting, parametersFrist);
                    //labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    break;
                case PACK_MODE.Pallet:
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    break;
            }



            //这里需要写入pnt_mng表
            TPCResult<bool> ret = Database.SetManagerData(m_Mode, data.PCode, Program.LoginUser,
                                            Convert.ToInt32(data.Total), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);
            if (ret.State == RESULT_STATE.NG)
            {
                MessageBox.Show(ret.Message);
                return;
            }
        }
        private void reprint2()
        {
            if (lstItems.Items.Count < Convert.ToInt32(txtTotal.Text))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NOT_FULL_QUANTITY_ERROR"), "ERROR", MessageBoxButtons.OK);
                return;
            }

            if (txtCode.Text.Substring(10, 1) != "W")
            {
                MessageBox.Show("该ID格式错误", "ID格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            PrinterSettings setting = dlg.PrinterSettings;

            PrintLabelData data = new PrintLabelData();
            data.PCode = txtCode.Text;
            data.DataCode = GetCodeDate();
            data.Date = GetDate();
            data.Total = Convert.ToInt32(txtTotal.Text);
            TPCResult<int> result = Database.GetModuleCount(m_Mode, txtCode.Text);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                return;
            }

            data.Quantity = result.Value;

            string label_nameFrist = "";
            string label_nameSecond = "fxzz_additional";
            switch (m_Mode)
            {
                case PACK_MODE.Pack:
                    label_nameFrist = "pack_fxzz";
                    break;
                case PACK_MODE.Carton:
                    label_nameFrist = "carton_fxzz";
                    break;
                case PACK_MODE.Pallet:
                    label_nameFrist = "pallet_fxzz";                    
                    break;
            }
            //打印第一页信息
            TPCPrintLabel labelFrist = LabelPrintGlobal.g_LabelCreator.GetPrintLabel(label_nameFrist);
            List<string> parametersFrist = MakePrintParameters(m_Mode, data);

            //打印第二页信息
            TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel(label_nameSecond);
            List<string> parametersSecond = MakePrintParameters(m_Mode, data);

            #region 修改打印参数
            //富士康"Date Code"
            parametersSecond[5] = changeDateFormat(parametersSecond[14].Substring(3, 4));
            //富士康"Lot No"固定为NA+YY（年）WW（周）
            parametersSecond[14] = "NA" + parametersSecond[14].Substring(0, parametersSecond[14].Length - 1);
            #endregion

            switch (m_Mode)
            {
                case PACK_MODE.Pack:
                    labelFrist.Print(setting, parametersFrist);

                    dlg = new PrintDialog();
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    setting = dlg.PrinterSettings;
                    labelSecond.Print(setting, parametersSecond);
                    break;
                case PACK_MODE.Carton:
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);

                    dlg = new PrintDialog();
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    setting = dlg.PrinterSettings;
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    break;
                case PACK_MODE.Pallet:
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);

                    dlg = new PrintDialog();
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    setting = dlg.PrinterSettings;
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);

                    #region 富士康卡板A4纸张
                    //设置新纸张大小
                    dlg = new PrintDialog();
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    setting = dlg.PrinterSettings;

                    Print2 print2 = new Print2(txtCode.Text, parametersSecond[7], parametersSecond[5], parametersSecond[14]);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("No.");
                    dt.Columns.Add("箱号");
                    dt.Columns.Add("料号");
                    dt.Columns.Add("数量");
                    int i = 0;
                    //每行写上被包括的子标签数量等
                    TPCResult<List<List<string>>> items = null;
                    foreach (ListViewItem var in lstItems.Items)
                    {
                        string id = var.SubItems[1].Text;
                        if (!queryInfo(id,ref items))
                            return;
                        if (items.Value.Count == 0)
                            continue;
                        DataRow dr = dt.NewRow();
                        dr["No."] = (++i).ToString();
                        dr["箱号"] = id;
                        dr["料号"] = LabelPrintGlobal.g_Config.APN;
                        dr["数量"] = items.Value[0][0].ToString();
                        dt.Rows.Add(dr);
                    }

                   

                    print2.ImportDataTable(dt);

                    if (print2.BtnPrint_Click(setting))
                    {
                        print2.Dispose();
                        return;
                    }
                    print2.Dispose();
                    #endregion
                    break;
            }

            //这里需要写入pnt_mng表
            TPCResult<bool> ret = Database.SetManagerData(m_Mode, data.PCode, Program.LoginUser,
                                            Convert.ToInt32(data.Total), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);
            if (ret.State == RESULT_STATE.NG)
            {
                MessageBox.Show(ret.Message);
                return;
            }
        }

        private void reprint3()
        {
            if (lstItems.Items.Count < Convert.ToInt32(txtTotal.Text))
            {
                MessageBox.Show(LabelPrintGlobal.ShowWarningMessage("NOT_FULL_QUANTITY_ERROR"), "ERROR", MessageBoxButtons.OK);
                return;
            }

            if (!"123456789ABC".Contains(txtCode.Text.Substring(10, 1)))
            {
                MessageBox.Show("该ID格式错误", "ID格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;

            }
            PrinterSettings setting = dlg.PrinterSettings;

            PrintLabelData data = new PrintLabelData();
            data.PCode = txtCode.Text;
            data.DataCode = GetCodeDate();
            data.Total = Convert.ToInt32(txtTotal.Text);
            data.Date = GetDate();
            TPCResult<int> result = Database.GetModuleCount(m_Mode, txtCode.Text);
            if (result.State == RESULT_STATE.NG)
            {
                MessageBox.Show(result.Message);
                return;
            }

            data.Quantity = result.Value;

            string label_nameFrist = "";
            string label_nameSecond = "";
            switch (m_Mode)
            {
                case PACK_MODE.Pack:
                    label_nameFrist = "pack_fxzz";
                    label_nameSecond = "pack_wks";
                    break;
                case PACK_MODE.Carton:
                    label_nameFrist = "carton_fxzz";
                    label_nameSecond = "carton_wks";
                    break;
                case PACK_MODE.Pallet:
                    label_nameFrist = "pallet_fxzz";
                    label_nameSecond = "pallet_wks";
                    break;
            }
            //打印第一页信息
            TPCPrintLabel labelFrist = LabelPrintGlobal.g_LabelCreator.GetPrintLabel(label_nameFrist);
            List<string> parametersFrist = MakePrintParameters(m_Mode, data);
            
            //打印第二页信息
            TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel(label_nameSecond);
            List<string> parametersSecond = MakePrintParameters(m_Mode, data);
           
            switch (m_Mode)
            {
                case PACK_MODE.Pack:
                    labelFrist.Print(setting, parametersFrist);
                    labelSecond.Print(setting, parametersSecond);
                    break;
                case PACK_MODE.Carton:
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    break;
                case PACK_MODE.Pallet:
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelFrist.Print(setting, parametersFrist);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);
                    labelSecond.Print(setting, parametersSecond);

                    #region 富士康卡板A4纸张
                    //设置新纸张大小
                    dlg = new PrintDialog();
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    setting = dlg.PrinterSettings;

                    #region 修改打印参数
                    //富士康"Date Code"
                    parametersSecond[5] = changeDateFormat(parametersSecond[14].Substring(3, 4));
                    //富士康"Lot No"固定为NA+YY（年）WW（周）
                    parametersSecond[14] = parametersSecond[14].Substring(0, parametersSecond[14].Length - 1);
                    #endregion
                    Print2 print2 = new Print2(txtCode.Text, parametersSecond[7], parametersSecond[5], parametersSecond[14]);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("No.");
                    dt.Columns.Add("箱号");
                    dt.Columns.Add("料号");
                    dt.Columns.Add("数量");
                    int i = 0;
                    //每行写上被包括的子标签数量等
                    TPCResult<List<List<string>>> items = null;
                    foreach (ListViewItem var in lstItems.Items)
                    {
                        string id = var.SubItems[1].Text;
                        if (!queryInfo(id,ref items))
                            return;
                        if (items.Value.Count == 0)
                            continue;
                        DataRow dr = dt.NewRow();
                        dr["No."] = (++i).ToString();
                        dr["箱号"] = id;
                        dr["料号"] = LabelPrintGlobal.g_Config.APN;
                        dr["数量"] = items.Value[0][0].ToString();
                        dt.Rows.Add(dr);
                    }



                    print2.ImportDataTable(dt);

                    if (print2.BtnPrint_Click(setting))
                    {
                        print2.Dispose();
                        return;
                    }
                    print2.Dispose();
                    #endregion
                    break;
            }


            //这里需要写入pnt_mng表
            TPCResult<bool> ret = Database.SetManagerData(m_Mode, data.PCode, Program.LoginUser,
                                            Convert.ToInt32(data.Total), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);
            if (ret.State == RESULT_STATE.NG)
            {
                MessageBox.Show(ret.Message);
                return;
            }
        }

        private void txtChangePackID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (txtChangePackID.Text.Length != 18)
                {
                    MessageBox.Show("该包ID不是18位","格式错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                bool isFXZZ = txtChangePackID.Text.Substring(10, 1) == "W";
                int type = isFXZZ ? 2 : 3;
                TPCResult<string> result = Database.ChangePackID(txtChangePackID.Text, Program.LoginUser, LabelPrintGlobal.g_Config.Vendor, LabelPrintGlobal.g_Config.SiteCode, GetCodeDate(type));
                if (result.State == RESULT_STATE.NG)
                {
                    MessageBox.Show(result.Message);
                    return;
                }
                txtCode.Text = result.Value.ToString();
                txtCode_KeyDown(sender, e);
            }
        }

        string GetCodeDate(int type)
        {
            if (type == 2)
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

        ///将时间格式9013改成2019-01-03
        string changeDateFormat(string dateCode)
        {
            string outputTime = "";
            string[] code = { dateCode.Substring(0, 1), dateCode.Substring(1, 2), dateCode.Substring(3, 1) };
            #region 确定年月日
            //确定年
            string today = DateTime.Today.ToString("yyyy");
            for (int i = 0; i < 10; i++)
            {
                if (today.Substring(3, 1).Equals(code[0]))
                {
                    outputTime = today;
                }
                else
                {
                    today = (Convert.ToInt16(today) - 1).ToString();
                }
            }
            //确定月份和日
            DateTime dtTemp = Convert.ToDateTime(outputTime + "-01-01");
            GregorianCalendar gc = new GregorianCalendar();
            for (int i = 0; i < 365; i++)
            {
                int weekOfYear = gc.GetWeekOfYear(dtTemp, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int dayOfWeek = (int)dtTemp.DayOfWeek + 1;
                if (weekOfYear == Convert.ToInt16(code[1]) && dayOfWeek == Convert.ToInt16(code[2]))
                {
                    outputTime = dtTemp.ToString("yyyy-MM-dd");
                    break;
                }
                else { dtTemp = dtTemp.AddDays(1); }
            }
            #endregion
            return outputTime;
        }

        bool queryInfo(string ID, ref TPCResult<List<List<string>>> items)
        {
            items = null;
            //m_Mode = PACK_MODE.Pack;
            items = Database.GetFXZZ_Data(ID);
            if (items.State == RESULT_STATE.NG)
            {
                MessageBox.Show(items.Message);
                return false;
            }
            return true;
        }
    }
}
