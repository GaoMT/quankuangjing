﻿using InternetDataMine.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InternetDataMine.Controllers
{
    public class CenterController : Controller
    {
        //
        // GET: /Center/

        public ActionResult Index()
        {

            //TransJsonToTreeListModel t = new TransJsonToTreeListModel();
            //t.test();

            // PersonModel pm = new PersonModel();
            //DataTableCollection dtc =  pm.test();


            return View();
        }

        public ActionResult Portal()
        {
            return View();
        }
        public ActionResult ChildSysBarGraph(string ChildSys)
        {
            ViewBag.ChildSys = ChildSys;
            return View();
        }
        /// <summary>
        /// 为下拉控件的树形煤矿内容加载煤矿信息，按所在城市分组
        /// </summary>
        /// <param name="mineCode"></param>
        public void ReturnComboTreeData(string mineCode)
        {
            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.ReturnComboTreeMineInfo(mineCode));
            Response.End();
        }

        /// <summary>
        /// 为下拉控件绑定设备名称信息
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="Type">设备类型 A模拟量 D开关量 L累积量</param>
        public void ReturnComboboxForDevType(string mineCode,string Type)
        {
            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.ReturnDeviceType(mineCode, Type));
            Response.End();
         }

        /// <summary>
        /// 为下拉控件的表格加载设备信息，按煤矿名称分组。
        /// </summary>
        /// <param name="mineCode"></param>
        public void ReturnComboGridForSensorInfo(string mineCode,string SensorNameCode,string devType)
        {
            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.ReturnComboDeviceInfo(mineCode,SensorNameCode,devType));
            Response.End();
        }

        public void ReturnLDKZ(string MineCode, string page,string Type)
        {
         
            BaseInfoModel model = new BaseInfoModel();

            Response.Write(model.ReturnLDKZ(MineCode, page,Type));
            Response.End();
        }
   
        public void ReturnmMineTreeData(string mineCode)
        {
            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.ReturnTreeMineInfo(mineCode));
            Response.End();
        }

        public ActionResult AQJKRTData()
        {
            return View();
        }

        public ActionResult ShowAQJKRTData()
        {
            return View();
        }
        public ActionResult KSYLData()
        {
            return View();
        }
        public ActionResult HZSGData()
        {
            return View();
        }
        /// <summary>
        /// 通过前台传来的页面名称，加载参数
        /// </summary>
        /// <param name="PageName">UI选择的界面名称</param>
        /// <returns></returns>
        public ActionResult ShowData(string PageName, string UserAbility, string MineCode, string PageType, string Height,string SystemType,string Width)
        {
         
            LoadModel loadModel = new LoadModel();
            loadModel.Height = Height;
            loadModel.UserAbility = UserAbility;
            loadModel.UserMineCode = MineCode;
            loadModel.Width = (Convert.ToInt32(Width) - 200).ToString();
            if (string.IsNullOrEmpty(SystemType))
                loadModel.SystemType = 1;
            else
                loadModel.SystemType = Convert.ToInt32(SystemType);
            if (loadModel.SystemType == 1)
            {
                Session["SystemType"] = "1";//  Session 记录SystemType，以区分实时数据中 “测点编号” 显示
            }
            else if (loadModel.SystemType == 5)
            {
                Session["SystemType"] = "5";

            }
            else if (loadModel.SystemType == 7)
            {
                Session["SystemType"] = "7";

            }
            else
            {
                Session["SystemType"] = "0";
            }

            switch (loadModel.SystemType)
            {
                #region  安全监控
                //安全监控页面
                case 1:
                    switch (PageName)
                    {
                        #region 加载安全监控页面
                       
                        #region 历史报警
                        //加载历史报警页面
                        case "AQBJHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQBJHis";
                            loadModel.PageTitle = "历史报警信息";
                            break;
                        #endregion

                        #region 历史故障
                        //加载历史故障页面
                        case "AQGZHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQGZHis";
                            loadModel.PageTitle = "历史故障信息";
                            break;
                        #endregion

                        #region 历史馈电
                        //加载历史馈电页面
                        case "AQKDHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKDHis";
                            loadModel.PageTitle = "历史馈电信息";
                            break;
                        #endregion

                        #region 历史断电
                        //加载历史断电页面
                        case "AQDDHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQDDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQDDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQDDHis";
                            loadModel.PageTitle = "历史断电信息";
                            break;
                        #endregion

                        #region 实时馈电
                        //加载实时馈电页面
                        case "AQKD":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKD";
                            loadModel.PageTitle = "实时馈电信息";
                            break;
                        #endregion

                        #region 历史模拟量统计
                        //加载历史模拟量统计页面
                        case "AQMCHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMCHis.xml";
                            //loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data/AQMCHis.xml";
                            //loadModel.PreLoadData = "AQMCHis";
                            //loadModel.SystemType = 1;
                            //loadModel.PageTitle = "历史模拟量统计";
                            break;
                        #endregion

                        #region 实时报警页面
                        //加载实时报警页面
                        case "AQBJ":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJ_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQBJ";
                            loadModel.PageTitle = "实时报警信息";
                            break;
                        #endregion

                        #region 实时断电
                        //加载实时断电页面
                        case "AQDD":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQDD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQDD_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQDD";
                            loadModel.PageTitle = "实时断电信息";
                            break;
                        #endregion

                        #region 测点配置
                        //加载测点配置界面
                        case "PointSet":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "PointSet";
                            loadModel.PageTitle = "测点配置信息";
                            break;
                        #endregion

                        #region 历史模拟量统计
                        //加载历史模拟量统计页面
                        case "AQMT":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMT";
                            loadModel.PageTitle = "历史模拟量统计";
                            break;

                        case "AQMNL_1M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_1M";
                            loadModel.PageTitle = "历史数据";
                            break;

                        case "AQMNL_3M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_3M";
                            loadModel.PageTitle = "模拟量三分钟数据";
                            break;

                        case "AQMNL_5M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_5M";
                            loadModel.PageTitle = "模拟量五分钟数据";
                            break;

                        case "AQMNL_1D":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_1D";
                            loadModel.PageTitle = "模拟量日统计";
                            break;

                        case "AQMNL_30D":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_30D";
                            loadModel.PageTitle = "模拟量月统计";
                            break;

                        #endregion

                        #region 煤矿信息
                        //加载煤矿信息界面
                        case "MineInfo":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "MineInfoData";
                            loadModel.PageTitle = "煤矿信息";
                            break;
                        #endregion

                        #region 实时数据
                        //加载实时数据界面
                        case "RealData":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData";
                            loadModel.PageTitle = "实时数据";
                            break;
                        #endregion

                        #region 实时故障
                        //加载实时故障
                        case "AQGZ":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQGZ";
                            loadModel.PageTitle = "实时故障";
                            break;
                        #endregion

                        #region 煤矿传输状态
                        //加载煤矿传输状态
                        case "MineState":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_MineState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "MineState";
                            loadModel.PageTitle = "煤矿传输状态";
                            break;
                        #endregion

                        #region 子系统配置
                        //加载子系统配置
                        case "ChildSystemSet":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ChildSystemSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_ChildSystemSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "ChildSystemSet";
                            loadModel.PageTitle = "子系统配置信息";
                            break;
                        #endregion

                        #region 开关量状态变动
                        //加载开关量状态变动页面
                        case "AQKGL_Day":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKGL_Day";
                            loadModel.SystemType = 1;
                            loadModel.PageTitle = "每天开关量变动";
                            break;

                        case "AQKGL_Week":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKGL_Week";
                            loadModel.SystemType = 1;
                            loadModel.PageTitle = "七天开关量变动";
                            break;
                        #endregion

                        #endregion

                        #region 安全监控报表加载
                        case "MineStateHisBB":
                            loadModel.QueryBarPath = "../DataXML/rbar.xml";
                            loadModel.QueryDataPath = "../DataXML/rpque.xml";
                            loadModel.PreLoadData = "MineStateHisBB";
                            loadModel.PageType = "Report";
                            break;
                        case "Report":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report_Day.xml";
                            loadModel.PreLoadData = "Report";
                            loadModel.PageType = "Report";
                            loadModel.ReportName = "Report_Day";
                            loadModel.PageTitle = "模拟量日报表";//模拟量日报表
                            break;
                        case "ReportSBGZR":

                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay_BJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";//报表查询项
                            loadModel.QueryDataPath = "../DataXML/Report/Report_SBGZ.xml";//报表内容项
                            loadModel.PreLoadData = PageName;//
                            loadModel.ReportName = "AQGZ";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "设备故障日报表";//菜单的大标题
                            break;
                        case "ReportSBGZY":

                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay_BJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";//报表查询项
                            loadModel.QueryDataPath = "../DataXML/Report/Report_SBGZ_Y.xml";//报表内容项
                            loadModel.PreLoadData = PageName;//
                            loadModel.ReportName = "AQGZ";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "设备故障月报表";//菜单的大标题
                            break;
                        case "ReportMNLYKD":
                        case "ReportMNLRKD":
                        case "ReportKGLYKD":
                        case "ReportKGLRKD":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_KD.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_FeedBack";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = PageName.Contains("KGL") ? "开关量" : "模拟量" + "馈电日报表";
                            break;

                        case "ReportMNLYDD":
                        case "ReportMNLRDD":
                        case "ReportKGLYDD":
                        case "ReportKGLRDD":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_DD.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_Power";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = PageName.Contains("KGL") ? "开关量" : "模拟量" + "断电日报表";
                            break;


                        case "ReportMNLRBJ":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay_BJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_BJ.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_Alarm";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "报警日报表";
                            break;

                        case "ReportMNLYBJ":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay_BJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_BJY.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_Alarm";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "报警月报表";
                            break;

                        #endregion


                        case "PreAlarm":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PreAlarm";
                            loadModel.PageTitle = "实时预警";
                            break;
                        case "PreAlarm_His":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PreAlarm_His";
                            loadModel.PageTitle = "历史预警";
                            break;
                        case "PreAlarm_TG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PreAlarm_TG";
                            loadModel.PageTitle = "历史预警统计";
                            break;


                        case "TXZDHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TXZDHis";
                            loadModel.PageTitle = "历史通讯异常";
                            break;


                        case "TXZD":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TXZD";
                            loadModel.PageTitle = "实时通讯异常";
                            break;

                        case "HisTXState":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "HisTXState";
                            loadModel.PageTitle = "历史通讯故障";
                            break;
                        case "TXYCHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TXYCHis";
                            loadModel.PageTitle = "历史通讯异常";
                            break;
                        case "TXZDTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TXZDTG";
                            loadModel.PageTitle = "历史通讯异常统计";
                            break;

                        case "TXYCTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TXYCTG";
                            loadModel.PageTitle = "历史通讯异常统计";
                            break;
                        case "GZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GZTG";
                            loadModel.PageTitle = "传感器故障统计";
                            break;
                        case "BJTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "BJTG";
                            loadModel.PageTitle = "报警统计";
                            break;
                        case "MNLTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "MNLTG";
                            loadModel.PageTitle = "模拟量统计";
                            break;


                        #region 综合平台子系统
                        #region 通风
                        case "TFST":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFST";
                            loadModel.PageTitle = "实时数据";
                            break;


                        case "TFDX":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFDX";
                            loadModel.PageTitle = "实时断线";
                            break;


                        case "TFGZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFGZTG";
                            loadModel.PageTitle = "故障统计";
                            break;




                        case "TFMNLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFMNLHis";
                            loadModel.PageTitle = "模拟量历史数据";
                            break;



                        case "TFKGLHis":
                              loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFKGLHis";
                            loadModel.PageTitle = "开关量历史数据";
                            break;

                            //通风历史故障
                        case "TFGZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFGZHis";
                            loadModel.PageTitle = "历史故障";
                            break;

                        //通风实时故障
                        case "TFGZ":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFGZ";
                            loadModel.PageTitle = "实时故障";
                            break;
                        #endregion
                        #region 皮带

                        case "PDST":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDST";
                            loadModel.PageTitle = "实时数据";
                            break;
                      
                        case "PDGZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDGZTG";
                            loadModel.PageTitle = "故障统计";
                            break;

                        case "PDDX":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDDX";
                            loadModel.PageTitle = "实时断线";
                            break;


                        case "PDMNLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDMNLHis";
                            loadModel.PageTitle = "模拟量历史数据";
                            break;



                        case "PDKGLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDKGLHis";
                            loadModel.PageTitle = "开关量历史数据";
                            break;

                        //皮带历史故障
                        case "PDGZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDGZHis";
                            loadModel.PageTitle = "历史故障";
                            break;

                        //皮带实时故障
                        case "PDGZ":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "PDGZ";
                            loadModel.PageTitle = "实时故障";
                            break;

                        #endregion
                        #region 瓦斯

                        case "WSCF":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSCF";
                            loadModel.PageTitle = "实时数据";
                            break;

                        case "WSGZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSGZTG";
                            loadModel.PageTitle = "故障统计";
                            break;

                        case "WSDX":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSDX";
                            loadModel.PageTitle = "实时断线";
                            break;


                        case "WSMNLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSMNLHis";
                            loadModel.PageTitle = "模拟量历史数据";
                            break;



                        case "WSKGLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSKGLHis";
                            loadModel.PageTitle = "开关量历史数据";
                            break;

                        //皮带历史故障
                        case "WSGZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSGZHis";
                            loadModel.PageTitle = "历史故障";
                            break;

                        //皮带实时故障
                        case "WSGZ":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSGZ";
                            loadModel.PageTitle = "实时故障";
                            break;

                        #endregion
                        #region 供电

                        case "GDST":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDST";
                            loadModel.PageTitle = "实时数据";
                            break;

                        case "GDGZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDGZTG";
                            loadModel.PageTitle = "故障统计";
                            break;

                        case "GDDX":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDDX";
                            loadModel.PageTitle = "实时断线";
                            break;


                        case "GDMNLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDMNLHis";
                            loadModel.PageTitle = "模拟量历史数据";
                            break;



                        case "GDKGLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDKGLHis";
                            loadModel.PageTitle = "开关量历史数据";
                            break;

                        //皮带历史故障
                        case "GDGZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDGZHis";
                            loadModel.PageTitle = "历史故障";
                            break;

                        //皮带实时故障
                        case "GDGZ":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDGZ";
                            loadModel.PageTitle = "实时故障";
                            break;

                        #endregion
                        #region 压风

                        case "YFST":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFST";
                            loadModel.PageTitle = "实时数据";
                            break;

                        case "YFGZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFGZTG";
                            loadModel.PageTitle = "故障统计";
                            break;

                        case "YFDX":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFDX";
                            loadModel.PageTitle = "实时断线";
                            break;


                        case "YFMNLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFMNLHis";
                            loadModel.PageTitle = "模拟量历史数据";
                            break;



                        case "YFKGLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFKGLHis";
                            loadModel.PageTitle = "开关量历史数据";
                            break;

                        //皮带历史故障
                        case "YFGZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFGZHis";
                            loadModel.PageTitle = "历史故障";
                            break;

                        //皮带实时故障
                        case "YFGZ":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFGZ";
                            loadModel.PageTitle = "实时故障";
                            break;

                        #endregion
                        #region 水泵集控

                        case "SPST":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPST";
                            loadModel.PageTitle = "实时数据";
                            break;

                        case "SPGZTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPGZTG";
                            loadModel.PageTitle = "故障统计";
                            break;

                        case "SPDX":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPDX";
                            loadModel.PageTitle = "实时断线";
                            break;


                        case "SPMNLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPMNLHis";
                            loadModel.PageTitle = "模拟量历史数据";
                            break;



                        case "SPKGLHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPKGLHis";
                            loadModel.PageTitle = "开关量历史数据";
                            break;

                        //皮带历史故障
                        case "SPGZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPGZHis";
                            loadModel.PageTitle = "历史故障";
                            break;

                        //皮带实时故障
                        case "SPGZ":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPGZ";
                            loadModel.PageTitle = "实时故障";
                            break;



                        case "SPMNLFZHis":
                                loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "SPMNLFZHis";
                            loadModel.PageTitle = "模拟量分钟数据";
                            break;

                        case "YFMNLFZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "YFMNLFZHis";
                            loadModel.PageTitle = "模拟量分钟数据";
                            break;
                        case "TFMNLFZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "TFMNLFZHis";
                            loadModel.PageTitle = "模拟量分钟数据";
                            break;

                        case "WSMNLFZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSMNLFZHis";
                            loadModel.PageTitle = "模拟量分钟数据";
                            break;

                        case "GDMNLFZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "GDMNLFZHis";
                            loadModel.PageTitle = "模拟量分钟数据";
                            break;
                        case "PDMNLFZHis":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "WSMNLFZHis";
                            loadModel.PageTitle = "模拟量分钟数据";
                            break;


                        case "OPCState":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "OPCState";
                            loadModel.PageTitle = "OPC通讯状态";
                            break;


                        case "DTSRealData":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "DTSRealData";
                            loadModel.PageTitle = "实时数据";
                            break;
                        #endregion



                        #endregion

                        default:
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet.xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData";
                            break;
                    }
                    break;
                #endregion
                //人员定位页面
                case 2:
                    switch (PageName)
                    {
                        #region [人员管理页面加载]

                        case "RYXJTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "RYXJTG";
                            loadModel.PageTitle = "人员下井统计";
                            break;



                        case "RYBJTG":

                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "RYBJTG";
                            loadModel.PageTitle = "人员报警统计";
                            break;

                        //人员实时数据
                        case "RYSS":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "RYSS";
                            loadModel.PageTitle = "实时数据";
                            break;
                        case "XJSJBZTG":
                              loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "XJSJBZTG";
                            loadModel.PageTitle = "下井时间不足统计";
                            break;
                        case "RYSSTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "RYSSTG";
                            loadModel.PageTitle = "实时统计";
                            break;
                        case "XZQYRYTG":
                            loadModel.QueryBarPath = "";
                            loadModel.QueryDataPath = "";
                            loadModel.PreLoadData = "XZQYRYTG";
                            loadModel.PageTitle = "限制区域人员统计";
                            break;

                        case "RYPathInfo"://路线预设
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PathInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_PathInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RYPathInfo";
                            loadModel.PageTitle = "路线预设";
                            break;
                        // 实时通信状态
                        case "RealTXState":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RealTXState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealTXState";
                            loadModel.PageTitle = "实时通信状态";
                            break;
                        // 历史通信故障查询
                        case "HisTXState":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTXState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisTXState";
                            loadModel.PageTitle = "历史通信故障查询";
                            break;
                        // 模拟量 统计



                        #region 基本信息:人员管理分站信息、区域信息、人员基本信息
                        //加载读卡分站页面
                        case "RYStation":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RecordStation" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RYStation";
                            loadModel.PageTitle = "读卡分站信息";
                            break;
                        //加载人员区域界面
                        case "RYAreaInfo":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RYArea" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RYArea" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RYAreaInfo";
                            loadModel.PageTitle = "人员管理区域信息";
                            break;
                        //加载人员信息界面
                        case "RYXX":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RYXX";
                            loadModel.PageTitle = "人员信息";
                            break;
                        #endregion
                        #region 路线预设
                        //加载路线预设界面
                        case "PreRoute":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_PreRoute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "PreRoute";
                            loadModel.PageTitle = "路线预设";
                            break;
                        #endregion
                        #region 实时数据：实时井下人员信息
                        //加载实时井下人员界面
                        case "RealInPeople":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RealInPeople" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealInPeople";
                            loadModel.PageTitle = "实时井下人员";
                            break;
                        #endregion
                        #region 人员实时报警：实时超时报警、实时限制报警、实时超员报警、实时特种异常报警
                        //加载实时超时报警界面
                        case "RealCS":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_SSCS" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealCS";
                            loadModel.PageTitle = "实时超时信息";
                            break;
                        //加载实时限制报警界面
                        case "RealXZ":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_CYXZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealXZ";
                            loadModel.PageTitle = "实时限制信息";
                            break;
                        //加载实时超员报警界面
                        case "RealCY":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_CY" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_CY" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealCY";
                            loadModel.PageTitle = "实时超员信息";
                            break;
                        //加载实时特种异常报警界面
                        case "RealTZYC":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_TZYC" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealTZYC";
                            loadModel.PageTitle = "实时特种异常信息";
                            break;
                        #endregion
                        #region 历史数据：历史下井查询、历史轨迹、历史超时报警、历史超员报警等
                        //历史下井查询
                        case "HisTrack":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisTrack";
                            loadModel.PageTitle = "历史下井信息";
                            break;
                        //历史轨迹
                        case "HisTrackInfo":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTrackInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisTrackInfo";
                            loadModel.PageTitle = "历史轨迹信息";
                            break;
                        //区域分站人员查询
                        case "QYFZQuery":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_AreaOrStationPeopleQuery" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "QYFZQuery";
                            loadModel.PageTitle = "区域/分站人员信息";
                            break;
                        //历史超时报警查询
                        case "HisCS":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisCS" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisCS";
                            loadModel.PageTitle = "历史超时信息";
                            break;
                        //历史限制报警查询
                        case "HisXZ":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisXZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisXZ";
                            loadModel.PageTitle = "历史限制信息";
                            break;
                        //历史超员报警查询
                        case "HisCY":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisCY" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisCY";
                            loadModel.PageTitle = "历史超员信息";
                            break;
                        //历史特种异常报警查询
                        case "HisTZYC":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTZYC" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "HisTZYC";
                            loadModel.PageTitle = "历史特种异常信息";
                            break;
                        #endregion

                        #region 报表
                        case "ReportSBGZR":
                        case "ReportSBGZY":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_SBGZ.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "AQGZ";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "设备故障报表";
                            break;

                        //人员管理 超时报表
                        case "ReportRYCSBB_R":
                        case "ReportRYCSBB_Y":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYCSBB.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "ReportRYCSBB";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "超时报表";
                            break;
                        //人员管理 超时班报表
                        case "ReportRYCSBB_B":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYCSBB_B.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "ReportRYCSBB_B";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "超时报表";
                            break;


                        case "ReportRYCSBB":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo_New" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml"; ;
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYCSBB_New.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_RYCSBB_New";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "超时报表";
                            break;

                        case "ReportRYCYBB":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisBETime" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml"; ;
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYCYBB_New.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "ReportRYCYBB_New";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "超员报表";

                            break;
                        //人员管理 超员报表
                        case "ReportRYCYBB_R":
                        case "ReportRYCYBB_Y":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYCYBB.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "ReportRYCYBB";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "超员报表";
                            break;

                        //人员管理 上下井报表
                        case "ReportRYSXJBB_R":
                        case "ReportRYSXJBB_Y":
                        case "ReportRYGBLDXJBB_R":
                        case "ReportRYGBLDXJBB_Y":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYSXJBB.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "RYSXJBB";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "上下井报表";
                            break;

                        //人员管理 通讯异常报表
                        case "ReportRYTXYCBB":
                            loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_RYTXYC.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "ReportRYTXYCBB";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "通讯异常报表";
                            break;

                        #endregion
                        #endregion
                        default:
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet.xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData";
                            break;
                    }
                    break;
                //瓦斯抽放
                case 4:
                    break;
                //矿压 - 广播 -  程控
                case 5:
                    switch (PageName)
                    {
                        #region 加载安全监控页面
                        #region 历史报警
                        case "GBHZ":
                            loadModel.PreLoadData = "GBHZ";
                            loadModel.PageTitle = "话站信息";
                            break;
                        case "GBSS":
                           loadModel.PreLoadData = "GBSS";
                            loadModel.PageTitle = "实时话站";
                            break;
                        case "CKSS":
                            loadModel.PreLoadData = "CKSS";
                            loadModel.PageTitle = "实时程控";
                            break;
                        //加载历史报警页面
                        case "AQBJHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQBJHis_KY";
                            loadModel.PageTitle = "历史报警信息";
                            break;
                        #endregion
                        #region 历史故障
                        //加载历史故障页面
                        case "AQGZHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQGZHis_KY";
                            loadModel.PageTitle = "历史故障信息";
                            break;
                        #endregion

                        #region 历史馈电
                        //加载历史馈电页面
                        case "AQKDHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKDHis";
                            loadModel.PageTitle = "历史馈电信息";
                            break;
                        #endregion

                        #region 历史断电
                        //加载历史断电页面
                        case "AQDDHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQDDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQDDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQDDHis";
                            loadModel.PageTitle = "历史断电信息";
                            break;
                        #endregion

                        #region 实时馈电
                        //加载实时馈电页面
                        case "AQKD":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKD";
                            loadModel.PageTitle = "实时馈电信息";
                            break;
                        #endregion

                        #region 历史模拟量统计
                        //加载历史模拟量统计页面
                        case "AQMCHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMCHis.xml";
                            //loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data/AQMCHis.xml";
                            //loadModel.PreLoadData = "AQMCHis";
                            //loadModel.SystemType = 1;
                            //loadModel.PageTitle = "历史模拟量统计";
                            break;
                        #endregion

                        #region 实时报警页面
                        //加载实时报警页面
                        case "AQBJ":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJ_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQBJ_KY";
                            loadModel.PageTitle = "实时报警信息";
                            break;
                        #endregion

                        #region 实时断电
                        //加载实时断电页面
                        case "AQDD":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQDD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQDD_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQDD_KY";
                            loadModel.PageTitle = "实时断电信息";
                            break;
                        #endregion

                        #region 测点配置
                        //加载测点配置界面
                        case "PointSet":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "PointSet";
                            loadModel.PageTitle = "测点配置信息";
                            break;
                        #endregion

                        #region 历史模拟量统计
                        //加载历史模拟量统计页面
                        case "AQMT":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMT";
                            loadModel.PageTitle = "历史模拟量统计";
                            break;

                        case "AQMNL_1M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_1M";
                            loadModel.PageTitle = "模拟量一分钟数据";
                            break;

                        case "AQMNL_3M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_3M";
                            loadModel.PageTitle = "模拟量三分钟数据";
                            break;

                        case "AQMNL_5M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_5M";
                            loadModel.PageTitle = "模拟量五分钟数据";
                            break;

                        case "AQMNL_1D":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_1D";
                            loadModel.PageTitle = "模拟量日统计";
                            break;

                        case "AQMNL_30D":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_30D";
                            loadModel.PageTitle = "模拟量月统计";
                            break;

                        #endregion

                        #region 煤矿信息
                        //加载煤矿信息界面
                        case "MineInfo":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "MineInfoData";
                            loadModel.PageTitle = "煤矿信息";
                            break;
                        #endregion

                        #region 实时数据
                        //加载实时数据界面
                        case "RealData":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData_KY";
                            loadModel.PageTitle = "实时数据";
                            break;
                        #endregion

                        #region 实时故障
                        //加载实时故障
                        case "AQGZ":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQGZ_KY";
                            loadModel.PageTitle = "实时故障";
                            break;
                        #endregion

                        #region 煤矿传输状态
                        //加载煤矿传输状态
                        case "MineState":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_MineState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "MineState_KY";
                            loadModel.PageTitle = "煤矿传输状态";
                            break;
                        #endregion

                        #region 子系统配置
                        //加载子系统配置
                        case "ChildSystemSet":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ChildSystemSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_ChildSystemSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "ChildSystemSet";
                            loadModel.PageTitle = "子系统配置信息";
                            break;
                        #endregion

                        #region 开关量状态变动
                        //加载开关量状态变动页面
                        case "AQKGL_Day":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKGL_Day_KY";
                            loadModel.SystemType = 1;
                            loadModel.PageTitle = "每天开关量变动";
                            break;

                        case "AQKGL_Week":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKGL_Week";
                            loadModel.SystemType = 1;
                            loadModel.PageTitle = "七天开关量变动";
                            break;
                        #endregion

                        #endregion

                        #region 安全监控报表加载
                        case "MineStateHisBB":
                            loadModel.QueryBarPath = "../DataXML/rbar.xml";
                            loadModel.QueryDataPath = "../DataXML/rpque.xml";
                            loadModel.PreLoadData = "MineStateHisBB";
                            loadModel.PageType = "Report";
                            break;
                        case "Report":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report_Day.xml";
                            loadModel.PreLoadData = "Report";
                            loadModel.PageType = "Report";
                            loadModel.ReportName = "Report_Day";
                            loadModel.PageTitle = "报表";
                            break;

                        case "ReportMNLYKD":
                        case "ReportMNLRKD":
                        case "ReportKGLYKD":
                        case "ReportKGLRKD":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_KD.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_FeedBack";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = PageName.Contains("KGL") ? "开关量" : "模拟量" + "馈电报表";
                            break;

                        case "ReportMNLYDD":
                        case "ReportMNLRDD":
                        case "ReportKGLYDD":
                        case "ReportKGLRDD":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_DD.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_Power";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = PageName.Contains("KGL") ? "开关量" : "模拟量" + "断电报表";
                            break;

                        case "ReportMNLYBJ":
                        case "ReportMNLRBJ":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Report/Report_BJ.xml";
                            loadModel.PreLoadData = PageName;
                            loadModel.ReportName = "Report_Alarm";
                            loadModel.PageType = "Report";
                            loadModel.PageTitle = "模拟量报警报表";
                            break;
                        #endregion

                        default:
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet.xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData";
                            break;
                    }
                    break;


                case 7:
                    switch (PageName)
                    {
                        #region  监测
                        #region 实时数据
                        //加载实时数据界面
                        case "RealData":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData_HG";
                            loadModel.PageTitle = "JSG8井下自燃火灾束管监测系统实时监测数据";
                            break;
                        #endregion

                        #region 实时故障
                        //加载实时故障
                        case "AQGZ":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQGZ_HG";
                            loadModel.PageTitle = "实时故障";
                            break;
                        #endregion
                        #region 实时报警页面
                        //加载实时报警页面
                        case "AQBJ":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJ_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQBJ_HG";
                            loadModel.PageTitle = "实时报警信息";
                            break;
                        #endregion
                        #endregion
                        #region  查询

                        #region 历史报警
                        //加载历史报警页面
                        case "AQBJHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQBJHis_HG";
                            loadModel.PageTitle = "历史报警信息";
                            break;
                        #endregion
                        #region 历史故障
                        //加载历史故障页面
                        case "AQGZHis":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQGZHis_HG";
                            loadModel.PageTitle = "历史故障信息";
                            break;
                        #endregion


                        #region 开关量状态变动
                        //加载开关量状态变动页面
                        case "AQKGL_Day":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKGL_Day_HG";
                            loadModel.SystemType = 1;
                            loadModel.PageTitle = "每天开关量变动";
                            break;

                        case "AQKGL_Week":
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQKGL_Week_HG";
                            loadModel.SystemType = 1;
                            loadModel.PageTitle = "七天开关量变动";
                            break;
                        #endregion


                        #region 历史模拟量统计
                        //加载历史模拟量统计页面
                        case "AQMT":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMT";
                            loadModel.PageTitle = "历史模拟量统计";
                            break;

                        case "AQMNL_1M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_1M_HG";
                            loadModel.PageTitle = "模拟量一分钟数据";
                            break;

                        case "AQMNL_3M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_3M";
                            loadModel.PageTitle = "模拟量三分钟数据";
                            break;

                        case "AQMNL_5M":
                            //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_5M";
                            loadModel.PageTitle = "模拟量五分钟数据";
                            break;

                        case "AQMNL_1D":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_1D";
                            loadModel.PageTitle = "模拟量日统计";
                            break;

                        case "AQMNL_30D":
                            loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "AQMNL_30D";
                            loadModel.PageTitle = "模拟量月统计";
                            break;

                        #endregion

                        default:
                            loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet.xml";
                            loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                            loadModel.PreLoadData = "RealData";
                            break;
                        #endregion
                    }
                    break;

              
                case 11:
                    loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                    loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                    loadModel.PreLoadData = "RealData_WS";
                    loadModel.PageTitle = "实时数据";
                    break;

                case 999:
                    loadModel.PreLoadData = "RealData_JSG8";
                    loadModel.PageTitle = "JSG8井下自燃火灾束管监测系统人工采样监测报告";

                    break;

                default:
                    loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet.xml";
                    loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
                    loadModel.PreLoadData = "RealData";
                    break;
            }

            #region 【暂时不用】
            //switch (PageName)
            //{
            //    #region 加载安全监控页面
            //    #region 历史报警
            //    //加载历史报警页面
            //    case "AQBJHis":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQBJHis";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "历史报警信息";
            //        break;
            //    #endregion
            //    #region 历史故障
            //    //加载历史故障页面
            //    case "AQGZHis":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQGZHis";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "历史故障信息";
            //        break;
            //    #endregion
            //    #region 历史馈电
            //    //加载历史馈电页面
            //    case "AQKDHis":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQKDHis";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "历史馈电信息";
            //        break;
            //    #endregion
            //    #region 历史断电
            //    //加载历史断电页面
            //    case "AQDDHis":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQDDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQDDHis" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQDDHis";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "历史断电信息";
            //        break;
            //    #endregion
            //    #region 实时馈电
            //    //加载实时馈电页面
            //    case "AQKD":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQKD";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "实时馈电信息";
            //        break;
            //    #endregion
            //    #region 历史模拟量统计
            //    //加载历史模拟量统计页面
            //    case "AQMCHis":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMCHis.xml";
            //        //loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data/AQMCHis.xml";
            //        //loadModel.PreLoadData = "AQMCHis";
            //        //loadModel.SystemType = 1;
            //        //loadModel.PageTitle = "历史模拟量统计";
            //        break;
            //    #endregion
            //    #region 实时报警页面
            //    //加载实时报警页面
            //    case "AQBJ":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQBJ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQBJ_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQBJ";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "实时报警信息";
            //        break;
            //    #endregion
            //    #region 实时断电
            //    //加载实时断电页面
            //    case "AQDD":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQDD" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQDD_NotSwitching" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQDD";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "实时断电信息";
            //        break;
            //    #endregion
            //    #region 测点配置
            //    //加载测点配置界面
            //    case "PointSet":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "PointSet";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "测点配置信息";
            //        break;
            //    #endregion
            //    #region 历史模拟量统计
            //    //加载历史模拟量统计页面
            //    case "AQMT":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQMT";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "历史模拟量统计";
            //        break;

            //    case "AQMNL_1M":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQMNL_1M";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "模拟量一分钟数据";
            //        break;

            //    case "AQMNL_3M":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQMNL_3M";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "模拟量三分钟数据";
            //        break;

            //    case "AQMNL_5M":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQMNL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Minute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQMNL_5M";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "模拟量五分钟数据";
            //        break;

            //    case "AQMNL_1D":
            //        loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQMNL_1D";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "模拟量日统计";
            //        break;

            //    case "AQMNL_30D":
            //        loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQMNL_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQMNL_30D";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "模拟量月统计";
            //        break;

            //    #endregion
            //    #region 煤矿信息
            //    //加载煤矿信息界面
            //    case "MineInfo":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "MineInfoData";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "煤矿信息";
            //        break;
            //    #endregion
            //    #region 实时数据
            //    //加载实时数据界面
            //    case "RealData":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_RealData" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealData";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "实时数据";
            //        break;
            //    #endregion
            //    #region 实时故障
            //    //加载实时故障
            //    case "AQGZ":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQGZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQGZ";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "实时故障";
            //        break;
            //    #endregion
            //    #region 煤矿传输状态
            //    //加载煤矿传输状态
            //    case "MineState":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_MineState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "MineState";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "煤矿传输状态";
            //        break;
            //    #endregion
            //    #region 子系统配置
            //    //加载子系统配置
            //    case "ChildSystemSet":
            //        //loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ChildSystemSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_ChildSystemSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "ChildSystemSet";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "子系统配置信息";
            //        break;
            //    #endregion
            //    #endregion
            //    #region [人员管理页面加载]
            //    case "RYPathInfo"://路线预设
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PathInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_PathInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RYPathInfo";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "路线预设";
            //        break;
            //    // 实时通信状态
            //    case "RealTXState":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RealTXState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealTXState";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "实时通信状态";
            //        break;
            //    // 历史通信故障查询
            //    case "HisTXState":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTXState" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisTXState";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史通信故障查询";
            //        break;

            //    #region 基本信息:人员管理分站信息、区域信息、人员基本信息
            //    //加载读卡分站页面
            //    case "RYStation":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_MineInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RecordStation" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RYStation";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "读卡分站信息";
            //        break;
            //    //加载人员区域界面
            //    case "RYAreaInfo":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RYArea" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RYArea" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RYAreaInfo";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "人员管理区域信息";
            //        break;
            //    //加载人员信息界面
            //    case "RYXX":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RYXX";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "人员信息";
            //        break;
            //    #endregion
            //    #region 路线预设
            //    //加载路线预设界面
            //    case "PreRoute":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_PreRoute" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "PreRoute";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "路线预设";
            //        break;
            //    #endregion
            //    #region 实时数据：实时井下人员信息
            //    //加载实时井下人员界面
            //    case "RealInPeople":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_RealInPeople" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealInPeople";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "实时井下人员";
            //        break;
            //    #endregion
            //    #region 人员实时报警：实时超时报警、实时限制报警、实时超员报警、实时特种异常报警
            //    //加载实时超时报警界面
            //    case "RealCS":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_PersonInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_SSCS" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealCS";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "实时超时信息";
            //        break;
            //    //加载实时限制报警界面
            //    case "RealXZ":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_CYXZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealXZ";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "实时限制信息";
            //        break;
            //    //加载实时超员报警界面
            //    case "RealCY":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_CY" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_CY" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealCY";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "实时超员信息";
            //        break;
            //    //加载实时特种异常报警界面
            //    case "RealTZYC":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_RT" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_TZYC" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealTZYC";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "实时特种异常信息";
            //        break;
            //    #endregion
            //    #region 历史数据：历史下井查询、历史轨迹、历史超时报警、历史超员报警等
            //    //历史下井查询
            //    case "HisTrack":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisTrack";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史下井信息";
            //        break;
            //    //历史轨迹
            //    case "HisTrackInfo":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTrackInfo" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisTrackInfo";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史轨迹信息";
            //        break;
            //    //区域分站人员查询
            //    case "QYFZQuery":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_AreaOrStationPeopleQuery" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "QYFZQuery";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "区域/分站人员信息";
            //        break;
            //    //历史超时报警查询
            //    case "HisCS":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisCS" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisCS";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史超时信息";
            //        break;
            //    //历史限制报警查询
            //    case "HisXZ":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisXZ" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisXZ";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史限制信息";
            //        break;
            //    //历史超员报警查询
            //    case "HisCY":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisCY" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisCY";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史超员信息";
            //        break;
            //    //历史特种异常报警查询
            //    case "HisTZYC":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_His" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Person/Person_Data_HisTZYC" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "HisTZYC";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "历史特种异常信息";
            //        break;
            //    #endregion

            //    #region 开关量状态变动
            //    //加载开关量状态变动页面
            //    case "AQKGL_Day":
            //        loadModel.QueryBarPath = "../DataXML/Safe/safe_Bar_Day" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQKGL_Day";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "每天开关量变动";
            //        break;

            //    case "AQKGL_Week":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_AQKGL" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "AQKGL_Week";
            //        loadModel.SystemType = 1;
            //        loadModel.PageTitle = "七天开关量变动";
            //        break;
            //    #endregion

            //    #region 报表
            //    case "Report":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Report_Day.xml";
            //        loadModel.PreLoadData = "Report";
            //        loadModel.PageType = "Report";
            //        loadModel.ReportName = "Report_Day";
            //        loadModel.SystemType = 2;
            //        loadModel.PageTitle = "报表";
            //        break;

            //    case "ReportMNLYKD":
            //    case "ReportMNLRKD":
            //    case "ReportKGLYKD":
            //    case "ReportKGLRKD":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_KD.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "Report_FeedBack";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = PageName.Contains("KGL") ? "开关量" : "模拟量" + "馈电报表";
            //        break;

            //    case "ReportMNLYDD":
            //    case "ReportMNLRDD":
            //    case "ReportKGLYDD":
            //    case "ReportKGLRDD":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_DD.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "Report_Power";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = PageName.Contains("KGL") ? "开关量" : "模拟量" + "断电报表";
            //        break;

            //    case "ReportMNLYBJ":
            //    case "ReportMNLRBJ":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_BJ.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "Report_Alarm";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "模拟量报警报表";
            //        break;

            //    case "ReportSBGZR":
            //    case "ReportSBGZY":
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_ReportDay" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_SBGZ.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "AQGZ";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "设备故障报表";
            //        break;

            //    //人员管理 超时报表
            //    case "ReportRYCSBB_R":
            //    case "ReportRYCSBB_Y":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_RYCSBB.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "ReportRYCSBB";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "超时报表";
            //        break;
            //    //人员管理 超时班报表
            //    case "ReportRYCSBB_B":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_RYCSBB_B.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "ReportRYCSBB_B";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "超时报表";
            //        break;
            //    //人员管理 超员报表
            //    case "ReportRYCYBB_R":
            //    case "ReportRYCYBB_Y":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_RYCYBB.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "ReportRYCYBB";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "超员报表";
            //        break;

            //    //人员管理 上下井报表
            //    case "ReportRYSXJBB_R":
            //    case "ReportRYSXJBB_Y":
            //    case "ReportRYGBLDXJBB_R":
            //    case "ReportRYGBLDXJBB_Y":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrackInfo.xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_RYSXJBB.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "RYSXJBB";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "上下井报表";
            //        break;

            //    //人员管理 通讯异常报表
            //    case "ReportRYTXYCBB":
            //        loadModel.QueryBarPath = "../DataXML/Person/Person_Bar_HisTrack" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.QueryDataPath = "../DataXML/Report/Report_RYTXYC.xml";
            //        loadModel.PreLoadData = PageName;
            //        loadModel.ReportName = "ReportRYTXYCBB";
            //        loadModel.PageType = "Report";
            //        loadModel.PageTitle = "通讯异常报表";
            //        break;

            //    #endregion
            //    #endregion
            //    #region 安全监控报表加载
            //    case "MineStateHisBB":
            //        loadModel.QueryBarPath = "../DataXML/rbar.xml";
            //        loadModel.QueryDataPath = "../DataXML/rpque.xml";
            //        loadModel.PreLoadData = "MineStateHisBB";
            //        loadModel.PageType = "Report";
            //        loadModel.SystemType = 1;
            //        break;
            //    #endregion
            //    default:
            //        loadModel.QueryBarPath = "../DataXML/Safe/Safe_Bar_PointSet.xml";
            //        loadModel.QueryDataPath = "../DataXML/Safe/Safe_Data_PointSet" + (string.IsNullOrEmpty(MineCode) ? "" : "_Mine") + ".xml";
            //        loadModel.PreLoadData = "RealData";
            //        loadModel.SystemType = 1;
            //        break;
            //}
            #endregion
      
            return View(loadModel);
        }

        public ActionResult Test(string PageName)
        {
            TestModel Test = new TestModel();
            Test.List_Object = new List<ToolBar>();

            #region --初始化ToolBar控件信息--
            ToolBar MineName = new ToolBar();
            MineName.ControlName = "MineName";
            MineName.GridCol = "MineName|煤矿名称,MineCode|煤矿编号";
            MineName.JsonURL = "../TransJson/TransJsonToTreeList?DataType=MineTest";
            MineName.idFiled = "MineCode";
            MineName.TextFiled = "MineName";
            ToolBar SensorName = new ToolBar();
            SensorName.ControlName = "SensorName";
            SensorName.GridCol = "MineName|煤矿名称,MineCode|煤矿编号";
            SensorName.JsonURL = "../TransJson/TransJsonToTreeList?DataType=MineTest";
            MineName.idFiled = "MineCode";
            MineName.TextFiled = "MineName";
            ToolBar SensorType = new ToolBar();
            SensorType.ControlName = "SensorType";
            SensorType.GridCol = "MineName|煤矿名称,MineCode|煤矿编号";
            SensorType.JsonURL = "../TransJson/TransJsonToTreeList?DataType=MineTest";
            MineName.idFiled = "MineCode";
            MineName.TextFiled = "MineName";
            ToolBar StationName = new ToolBar();
            StationName.ControlName = "StationName";
            StationName.GridCol = "MineName|煤矿名称,MineCode|煤矿编号";
            StationName.JsonURL = "../TransJson/TransJsonToTreeList?DataType=MineTest";
            MineName.idFiled = "MineCode";
            MineName.TextFiled = "MineName";
            ToolBar PersonName = new ToolBar();
            PersonName.ControlName = "PersonName";
            PersonName.GridCol = "MineName|煤矿名称,MineCode|煤矿编号";
            PersonName.JsonURL = "../TransJson/TransJsonToTreeList?DataType=MineTest";
            MineName.idFiled = "MineCode";
            MineName.TextFiled = "MineName";
            #endregion

            Test.teststring = "<div id=\"div\">煤矿名称：</div>";

            switch (PageName)
            {
                case "RealData":
                    Test.DataCol = "SimpleName|煤矿名称,SensorNum|测点编号,Place|安装地点";
                    Test.List_Object.Add(MineName);
                    Test.List_Object.Add(SensorName);
                    Test.List_Object.Add(SensorType);
                    break;
                case "RealIn":
                    Test.DataCol = "SimpleName|煤矿名称,StationName|分站名称,PersonName|姓名";
                    Test.List_Object.Add(MineName);
                    Test.List_Object.Add(StationName);
                    Test.List_Object.Add(PersonName);
                    break;
                default: break;
            }
            return View(Test);
        }


    

        /// <summary>
        /// 加载登录信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public string Login(string userName, string passWord)
        {
            LoginModel loginModel = new LoginModel();
            loginModel.UserName = userName;
            loginModel.PassWord = passWord;
          
            return loginModel.Login();
        }

        public ActionResult ShowTest()
        {
            return View();
        }

        public ActionResult dataReader()
        {
            return View();
        }

        public ActionResult UserManager()
        {
            TestModel tm = new TestModel();
            return View(tm);
        }

        public ActionResult Video()
        {
            return View();
        }


        public void LoadChildTypeName(string mineCode, string Type,string SysCode)
        {
            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.LoadChildTypeName(mineCode, Type,SysCode));
            Response.End();
        }

        public void LoadChildSensor(string mineCode, string SensorNameCode, string devType, string SysCode)
        {
            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.LoadChildSensor(mineCode, SensorNameCode,SysCode));
            Response.End();
        }

        public ActionResult ShowUserAQSS( )
        {
            return View();
        }

    }
}