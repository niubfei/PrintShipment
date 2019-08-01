using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using TPCCommon.Database;
using TPCCommon.Common;
using TPCBarcode.LabelPrint;
using System.Windows.Forms;

namespace LabelPrint.Model
{
    public class PalletModel : IFPackModel
    {
        public PalletModel(PackingForm parent)
        {
            m_Parent = parent;
        }

        public override void InitLanguage()
        {
            Parent.PNoLabel.Text = LanguageMapping.Instance.GetStaticMessage("PALLET_PNO", LabelPrintGlobal.g_Language);
            Parent.CNoLabel.Text = LanguageMapping.Instance.GetStaticMessage("PALLET_CNO", LabelPrintGlobal.g_Language);
            Parent.PackDateLabel.Text = LanguageMapping.Instance.GetStaticMessage("PALLET_PACK_DATE", LabelPrintGlobal.g_Language);
            Parent.QTYLabel.Text = LanguageMapping.Instance.GetStaticMessage("PALLET_PACK_QTY", LabelPrintGlobal.g_Language);
            Parent.ItemsListView.Columns[0].Text = LanguageMapping.Instance.GetStaticMessage("ITEM_COL_NO", LabelPrintGlobal.g_Language);
            Parent.ItemsListView.Columns[1].Text = LanguageMapping.Instance.GetStaticMessage("ITEM_COL_BARCODE", LabelPrintGlobal.g_Language);
            Parent.ItemsListView.Columns[2].Text = LanguageMapping.Instance.GetStaticMessage("ITEM_COL_TIME", LabelPrintGlobal.g_Language);
            Parent.PrintButton.Text = LanguageMapping.Instance.GetStaticMessage("BT_PRINT", LabelPrintGlobal.g_Language);
            //修正时间20180517
            Parent.PrintButtonNew.Text = LanguageMapping.Instance.GetStaticMessage("BT_PRINT_NEW", LabelPrintGlobal.g_Language);
            //end
            Parent.PrintButton3.Text = LanguageMapping.Instance.GetStaticMessage("BT_PRINT_3", LabelPrintGlobal.g_Language);
            InitCtrl();
            Parent.QTYTotalEdit.Text = string.Format("{0}", LabelPrintGlobal.g_Config.PalletCarton);
        }

        public override TPCResult<bool> ApplyNewCode()
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //预约新号
            int qty = Convert.ToInt32(Parent.QTYTotalEdit.Text);
            TPCResult<string> code = Database.ApplyNewCode("pallet", LabelPrintGlobal.g_Config.Vendor, LabelPrintGlobal.g_Config.SiteCode, GetCodeDate(), qty, Program.LoginUser);

            if (code.State == RESULT_STATE.NG)
            {
                result.Message = code.Message;
                result.State = code.State;
                return result;
            }

            //设置PNO
            Parent.PNoEdit.Text = code.Value;
            //设置子编码输入框的焦点，等待扫描
            Parent.CNoEdit.Focus();

            return result;
        }

        public override TPCResult<bool> ScanCCode(string code)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            if (CheckQuantityIsFull())
            {
                result.Message = LabelPrintGlobal.ShowWarningMessage("QUANTITY_FULL_ERROR");
                result.State = RESULT_STATE.NG;
                return result;
            }

            if (!CheckCNo(code, false))
            {
                result.State = RESULT_STATE.NG;
                result.Message = LanguageMapping.Instance.GetWarnningMessage(LabelPrintGlobal.g_LanguageConfig, "CARTON_CODE_ERROR", LabelPrintGlobal.g_Language);
                return result;
            }
            //将子项加入数据库中
            string pallet_no = Parent.PNoEdit.Text;
            DateTime time = DateTime.Now;
            result = Database.WritePalletCarton(pallet_no, code, time);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }

            //将子项加入列表中
            AddItem(code, time);

            //计数器更新
            Parent.QTYEdit.Text = string.Format("{0}", Parent.ItemsListView.Items.Count);

            return result;
        }

        public override TPCResult<System.Data.DataTable> CheckBin( string code)
        {
            TPCResult<System.Data.DataTable> result = new TPCResult<System.Data.DataTable>();
            result = Database.CheckBin(code);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            return result;
        }

        public override TPCResult<bool> ScanPCode(string code)
        {
            TPCResult<bool> result = new TPCResult<bool>();
            if (!CheckCNo(code, true))
            {
                result.State = RESULT_STATE.NG;
                result.Message = LanguageMapping.Instance.GetWarnningMessage(LabelPrintGlobal.g_LanguageConfig, "CARTON_CODE_ERROR", LabelPrintGlobal.g_Language);
                return result;
            }

            TPCResult<string> pallet_no = Database.GetPalletNoByCartonNo(code);
            if (pallet_no.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = pallet_no.Message;
                return result;
            }

            //从数据库中读取已装的Tray
            TPCResult<List<CItem>> packs = Database.GetPalletedItems(pallet_no.Value);
            if (packs.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = packs.Message;
                return result;
            }
            //刷新
            PackingOn();


            AddItem(packs.Value);
            Parent.QTYEdit.Text = string.Format("{0}", Parent.ItemsListView.Items.Count);

            //填充PNO
            Parent.PNoEdit.Text = pallet_no.Value;
            return result;
        }

        /// <summary>
        /// 打印标签
        /// </summary>
        public override TPCResult<bool> PrintLabel1()
        {
            TPCResult<bool> result = null;
            List<CItem> items = null;
            
            PrinterSettings setting = GetPrinterSetting();
            if (setting == null)
            {
                return new TPCResult<bool>();
            }

            items = GetPackingItems();
            result = Database.CompletedPallet(items);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            //打印第一页信息
            TPCPrintLabel labelFrist = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("pallet");
            List<string> parametersFrist = MakePrintParameters(PACK_MODE.Pallet, GetLabelData());
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);

            //打印第二页信息
            TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("pallet_pega");
            List<string> parametersSecond = MakePrintParameters(PACK_MODE.Pallet, GetLabelData());
            labelSecond.Print(setting, parametersSecond);
            labelSecond.Print(setting, parametersSecond);
            labelSecond.Print(setting, parametersSecond);
            labelSecond.Print(setting, parametersSecond);

            //这里需要写入pnt_mng表
            result = Database.SetManagerData(PACK_MODE.Pallet, Parent.PNoEdit.Text, Program.LoginUser,
                                            Convert.ToInt32(Parent.QTYEdit.Text), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);

            return result;
        }
        /// <summary>
        /// 修改日期20180517
        /// </summary>
        public override TPCResult<bool> PrintLabel2()
        {

            TPCResult<bool> result = null;
            List<CItem> items = null;

            result = new TPCResult<bool>();
            PrinterSettings setting = GetPrinterSetting();
            if (setting == null)
            {
                return result;
            }

            items = GetPackingItems();
            //写数据库文件
            result = Database.CompletedPallet(items);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            //打印第一页信息
            TPCPrintLabel labelFrist = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("pallet_fxzz");
            List<string> parametersFrist = MakePrintParameters(PACK_MODE.Pallet, GetLabelData());
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            //打印第二页信息
            //TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("pallet_fxzz");
            //List<string> parametersSecond = MakePrintParameters(PACK_MODE.Pallet, GetLabelData());
            //labelSecond.Print(setting, parametersSecond);

            //这里需要写入pnt_mng表
            result = Database.SetManagerData(PACK_MODE.Pallet, Parent.PNoEdit.Text, Program.LoginUser,
                                            Convert.ToInt32(Parent.QTYEdit.Text), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);

            return result;
        }


        public override TPCResult<bool> PrintLabel3()
        {
            TPCResult<bool> result = null;
            List<CItem> items = null;

            PrinterSettings setting = GetPrinterSetting();
            if (setting == null)
            {
                return new TPCResult<bool>();
            }

            items = GetPackingItems();
            result = Database.CompletedPallet(items);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            //打印第一页信息
            TPCPrintLabel labelFrist = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("pallet_fxzz");
            List<string> parametersFrist = MakePrintParameters(PACK_MODE.Pallet, GetLabelData());
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            labelFrist.Print(setting, parametersFrist);
            //打印第二页信息
            TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("pallet_wks");
            List<string> parametersSecond = MakePrintParameters(PACK_MODE.Pallet, GetLabelData());
            labelSecond.Print(setting, parametersSecond);
            labelSecond.Print(setting, parametersSecond);
            labelSecond.Print(setting, parametersSecond);
            labelSecond.Print(setting, parametersSecond);

            //这里需要写入pnt_mng表
            result = Database.SetManagerData(PACK_MODE.Pallet, Parent.PNoEdit.Text, Program.LoginUser,
                                            Convert.ToInt32(Parent.QTYEdit.Text), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);

            return result;
        }

        /// <summary>
        /// 检查Carton编码是否合法
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckCNo(string code, bool is_repack)
        {
            TPCResult<bool> result = Database.CheckCartonNo(code, is_repack);
            if (result.State == RESULT_STATE.NG)
            {
                return false;
            }
            return result.Value;
        }
    }
}
