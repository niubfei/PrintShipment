﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using TPCCommon.Common;
using TPCCommon.Database;
using TPCBarcode.LabelPrint;

using System.Globalization;

namespace LabelPrint.Model
{
    public class CartonModel : IFPackModel
    {
        public CartonModel(PackingForm parent)
        {
            m_Parent = parent;
        }

        public override void InitLanguage()
        {
            Parent.PNoLabel.Text = LanguageMapping.Instance.GetStaticMessage("CARTON_PNO", LabelPrintGlobal.g_Language);
            Parent.CNoLabel.Text = LanguageMapping.Instance.GetStaticMessage("CARTON_CNO", LabelPrintGlobal.g_Language);
            Parent.PackDateLabel.Text = LanguageMapping.Instance.GetStaticMessage("CARTON_PACK_DATE", LabelPrintGlobal.g_Language);
            Parent.QTYLabel.Text = LanguageMapping.Instance.GetStaticMessage("CARTON_PACK_QTY", LabelPrintGlobal.g_Language);
            Parent.ItemsListView.Columns[0].Text = LanguageMapping.Instance.GetStaticMessage("ITEM_COL_NO", LabelPrintGlobal.g_Language);
            Parent.ItemsListView.Columns[1].Text = LanguageMapping.Instance.GetStaticMessage("ITEM_COL_BARCODE", LabelPrintGlobal.g_Language);
            Parent.ItemsListView.Columns[2].Text = LanguageMapping.Instance.GetStaticMessage("ITEM_COL_TIME", LabelPrintGlobal.g_Language);
            Parent.PrintButton.Text = LanguageMapping.Instance.GetStaticMessage("BT_PRINT", LabelPrintGlobal.g_Language);
            //修正时间20180517
            Parent.PrintButtonNew.Text = LanguageMapping.Instance.GetStaticMessage("BT_PRINT_NEW", LabelPrintGlobal.g_Language);
            //end
            Parent.PrintButton3.Text = LanguageMapping.Instance.GetStaticMessage("BT_PRINT_3", LabelPrintGlobal.g_Language);
            InitCtrl();
            Parent.QTYTotalEdit.Text = string.Format("{0}", LabelPrintGlobal.g_Config.CartonPack);
        }

        public override TPCResult<bool> ApplyNewCode()
        {
            TPCResult<bool> result = new TPCResult<bool>();
            //预约新号
            int qty = Convert.ToInt32(Parent.QTYTotalEdit.Text);
            int type = Parent.PFXZZFormat.Checked ? 3 : 2;
            TPCResult<string> code = Database.ApplyNewCode("carton", LabelPrintGlobal.g_Config.Vendor, LabelPrintGlobal.g_Config.SiteCode, GetCodeDate(type), qty, Program.LoginUser);

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
                result.Message = LanguageMapping.Instance.GetWarnningMessage(LabelPrintGlobal.g_LanguageConfig, "PACK_CODE_ERROR", LabelPrintGlobal.g_Language);
                return result;
            }
            //将子项加入数据库中
            string carton_no = Parent.PNoEdit.Text;
            DateTime time = DateTime.Now;
            result = Database.WriteCartonPack(carton_no, code, time);
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
                result.Message = LanguageMapping.Instance.GetWarnningMessage(LabelPrintGlobal.g_LanguageConfig, "PACK_CODE_ERROR", LabelPrintGlobal.g_Language);
                return result;
            }

            TPCResult<string> carton_no = Database.GetCartonNoByPackNo(code);
            if (carton_no.State == RESULT_STATE.NG)
            {
                result.State = RESULT_STATE.NG;
                result.Message = carton_no.Message;
                return result;
            }

            //从数据库中读取已装的Tray
            TPCResult<List<CItem>> packs = Database.GetCartonedItems(carton_no.Value);
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
            Parent.PNoEdit.Text = carton_no.Value;
            return result;
        }

        /// <summary>
        /// 打印标签
        /// </summary>
        public override TPCResult<bool> PrintLabel1()
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
            result = Database.CompletedCarton(items);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            //打印第一页信息
            TPCPrintLabel labelFristPage = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("carton");
            List<string> parametersFristPage = MakePrintParameters(PACK_MODE.Carton, GetLabelData(1));
            //labelFristPage.Print(setting, parametersFristPage);
            labelFristPage.Print(setting, parametersFristPage);
            //打印第二页信息
            TPCPrintLabel labelSecondPage = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("carton_pega");
            List<string> parametersSecondPage = MakePrintParameters(PACK_MODE.Carton, GetLabelData(2));
            //labelSecondPage.Print(setting, parametersSecondPage);
            labelSecondPage.Print(setting, parametersSecondPage);

            //这里需要写入pnt_mng表
            result = Database.SetManagerData(PACK_MODE.Carton, Parent.PNoEdit.Text, Program.LoginUser,
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
            result = Database.CompletedCarton(items);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            //打印第一页信息
            TPCPrintLabel labelFristPage = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("carton_fxzz");
            List<string> parametersFristPage = MakePrintParameters(PACK_MODE.Carton, GetLabelData(2));
            labelFristPage.Print(setting, parametersFristPage);
            labelFristPage.Print(setting, parametersFristPage);
            //选择性打印第二页信息
            setting = GetPrinterSetting();
            if (setting != null)
            {
                TPCPrintLabel labelSecond = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("fxzz_additional");
                List<string> parametersSecond = MakePrintParameters(PACK_MODE.Carton, GetLabelData(2));
                #region 修改打印参数
                #region dateCode            
                //将时间格式9013改成2019-01-03            
                string outputTime = "";
                string dateCode = parametersSecond[14].Substring(3, 4);
                string[] code = { dateCode.Substring(0, 1), dateCode.Substring(1, 2), dateCode.Substring(3, 1) };

                //确定年
                string today = DateTime.Today.ToString("yyyy");
                for (int i = 0; i < 10; i++)
                {
                    if (today.Substring(3, 1).Equals(code[0]))
                    {
                        outputTime = today;
                        break;
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

                parametersSecond[5] = outputTime;
                #endregion
                #region 富士康"Lot No"改为NA+lot号
                parametersSecond[14] = "NA" + parametersSecond[14].Substring(0, parametersSecond[14].Length - 1);
                #endregion
                #endregion
                labelSecond.Print(setting, parametersSecond);
                labelSecond.Print(setting, parametersSecond);
            }

            //这里需要写入pnt_mng表
            result = Database.SetManagerData(PACK_MODE.Carton, Parent.PNoEdit.Text, Program.LoginUser,
                                            Convert.ToInt32(Parent.QTYEdit.Text), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);

            return result;
        }

        public override TPCResult<bool> PrintLabel3()
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
            result = Database.CompletedCarton(items);
            if (result.State == RESULT_STATE.NG)
            {
                return result;
            }
            //打印第一页信息
            TPCPrintLabel labelFristPage = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("carton_fxzz");
            List<string> parametersFristPage = MakePrintParameters(PACK_MODE.Carton, GetLabelData(2));
            labelFristPage.Print(setting, parametersFristPage);
            labelFristPage.Print(setting, parametersFristPage);
            //打印第二页信息
            TPCPrintLabel labelSecondPage = LabelPrintGlobal.g_LabelCreator.GetPrintLabel("carton_wks");
            List<string> parametersSecondPage = MakePrintParameters(PACK_MODE.Carton, GetLabelData(2));
            labelSecondPage.Print(setting, parametersSecondPage);
            labelSecondPage.Print(setting, parametersSecondPage);

            //这里需要写入pnt_mng表
            result = Database.SetManagerData(PACK_MODE.Carton, Parent.PNoEdit.Text, Program.LoginUser,
                                            Convert.ToInt32(Parent.QTYEdit.Text), PACK_ACTION.Register,
                                            PACK_STATUS.Completed);

            return result;
        }

        /// <summary>
        /// 检查Pack编码是否合法
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckCNo(string code, bool is_repack)
        {
            TPCResult<bool> result = Database.CheckPackNo(code, is_repack);
            if (result.State == RESULT_STATE.NG)
            {
                return false;
            }
            return result.Value;
        }
    }
}
