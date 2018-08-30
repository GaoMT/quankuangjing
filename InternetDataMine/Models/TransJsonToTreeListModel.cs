﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Newtonsoft.Json.Converters;
using InternetDataMine.Controllers;
using System.Globalization;
using System.Runtime.InteropServices;
namespace InternetDataMine.Models
{
    public class TransJsonToTreeListModel
    {
        #region [变量]
        InternetDataMine.Models.DataService.DataBLL _BLL_Data = new DataService.DataBLL();
        InternetDataMine.Models.DataService.ReportDataBLL _BLL_Report = new DataService.ReportDataBLL();
        string TreeListType = string.Empty;
        string MineCode = string.Empty;
        string SensorNum = string.Empty;
        string DevType = string.Empty;
        string DropListType = string.Empty;
        string Position = string.Empty;
        string Name = string.Empty;
        string ReportName = string.Empty;
        DateTime BeginTime = new DateTime();
        DateTime EndTime = new DateTime();
        string TypeName = string.Empty;
        int SystemType;
        int StartRow;
        int flag;
        int Rows;
        string DropName = string.Empty;
        string m_deviceKind = null;
        string Timespan = string.Empty;
        string TypeKind = string.Empty;
        string Job = string.Empty;
        string Department = string.Empty;
        #endregion

        //public TransJsonToTreeListModel(string Type)
        //{
        //    TreeListType = Type;            
        //}
        public TransJsonToTreeListModel(string SystemType, string DataType, string MineCode, string SensorNum, string DevType, string DropListName, string ReportName, int startRows, int rows, string StartTime, string EndTime, string TypeName, string DropName, string TimeSpan,string TypeKind, string Job,string Department  ,string p_position = null)
        {

            #region 初始化
            TreeListType = DataType;
            this.MineCode = MineCode;
            this.SensorNum = SensorNum;
            this.DevType = DevType;
            this.m_deviceKind = p_position;
            this.Timespan = TimeSpan=="NaN"?null:TimeSpan;
            this.TypeKind = TypeKind;
            this.Job = Job;
            this.Department = Department;
            int flagb = 0;
            int flage = 0;
            #endregion
            if (StartTime != "" && StartTime != null)
            {

             
//DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
//dtFormat.ShortDatePattern = "yyyy/MM/dd";
               
this.BeginTime = Convert.ToDateTime(StartTime);
//this.BeginTime = DateTime.SpecifyKind(BeginTime, DateTimeKind.Local);
//log log = new log();
//log.WriteTextLog("a1:" + StartTime + ";" + BeginTime.Kind.ToString(), DateTime.Now);
//log.WriteTextLog( "a:"+BeginTime, DateTime.Now);
            }
            else
            {
                flagb = 1;
                this.BeginTime = DateTime.Now;
            }
            if (EndTime != "" && EndTime != null)
            {
               
                this.EndTime = Convert.ToDateTime(EndTime).AddDays(1).AddSeconds(-1);
            }
            else
            {
                flage = 1;
                this.EndTime = DateTime.Now;
            }
            if (flagb == 1 && flage == 1)
            {
                flag = 2;
            }
            else flag = 1;
            this.DropName = DropName;
            this.TypeName = DevType;
            this.DropListType = DropListName;
            this.ReportName = ReportName;
            this.StartRow = startRows;
            this.Rows = rows;
            this.Position = p_position;
            if (SystemType != null && SystemType!="null")
            { this.SystemType = int.Parse(SystemType); }
        }



        public string GetDataJson
        {
            get
            {
                  if (Rows != 0)
                {
                    _BLL_Data.PageIndex = StartRow / Rows;
                    _BLL_Data.PageSize = Rows;
                }
                else
                {
                    _BLL_Data.PageIndex = 0;
                    _BLL_Data.PageSize = 100;
                }
                
             
              
                DataTable Result = null;
                DataTableCollection Results;
                EnumDataType VIEW = (EnumDataType)Enum.Parse(typeof(EnumDataType), TreeListType);//字符串转化为枚举
                Results = null;
       
                #region 枚举方式
                switch (VIEW)
                {
                    case EnumDataType.ShowAQJKRTData:
                        Result = new DataTable();
                        return "";

                    #region [加载查询bar数据]
                    case EnumDataType.Filter:
                        if (SystemType == 1)
                        {
                            switch (DropName)
                            {
                                case "DevType": Result = _BLL_Data.GetDevTypeList(MineCode,TypeKind); break;
                                case "SensorNum": Result = _BLL_Data.GetRealDataForAQList(MineCode, DevType, SystemType.ToString()); break;
                            }

                        }
                        else
                        {
                            switch (DropListType)
                            {
                                case "DevType": _BLL_Data.GetRealDataForAQList(MineCode, DevType, SystemType.ToString()); break;
                                case "FZ": Result = _BLL_Data.GetRYFZList(MineCode); break;
                                case "QY": Result = _BLL_Data.GetRYQYList(MineCode); break;
                                case "Card": Result = _BLL_Data.GetRYXXList(MineCode, Position, Name); break;
                                case "Name": Result = _BLL_Data.GetRYXXList(MineCode, Position, Name); break;
                            }
                        }
                        break;
                    #endregion

                    #region [安全监控数据查询]
                    //获取所有煤矿信息  
                    case EnumDataType.MineName: Result = _BLL_Data.MineList(); break;
                    //获取测点编号 
                    case EnumDataType.Sensor: Results = _BLL_Data.GetRealDataForAQ(MineCode); break;
                    //获取所有设备名 _______________________----------------------------------------------------------原来TypeKind 均为  m_deviceKind---------
                    case EnumDataType.DevType: Result =  _BLL_Data.DeviceList(TypeKind); break;
                    //获取实时数据
                    case EnumDataType.RealData: Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "1"); break;
                    //加载煤矿信息内容 
                    case EnumDataType.Mine: Result = _BLL_Data.MineList(); break;
                    //加载实时故障
                    case EnumDataType.AQGZ: Results = _BLL_Data.GetRealAQGZ(MineCode, DevType, "1", SensorNum); break;
                    //获取测点配置信息  
                    case EnumDataType.PointSet: Results = _BLL_Data.GetDeviceInfo(MineCode, DevType, SensorNum); break;
                    //获取煤矿信息 
                    case EnumDataType.MineInfoData: Result = _BLL_Data.MineList(MineCode); break;
                    //获取煤矿传输状态 
                    case EnumDataType.MineState: Result = _BLL_Data.GetBadLog(MineCode, SystemType); break;
                    //获取子系统配置信息 
                    case EnumDataType.ChildSystemSet:
                        Result = _BLL_Data.GetChildSystemSet(MineCode); break;
                    //获取实时报警信息 
                    case EnumDataType.AQBJ: Results = _BLL_Data.GetRealAQBJ(MineCode, TypeName, "1",SensorNum); break;
                    //历史开关量统计查询
                    case EnumDataType.AQMCHis: Results = _BLL_Data.GetHisAQLT(MineCode, DevType, BeginTime, EndTime); break;
                    //历史报警信息 
                    case EnumDataType.AQBJHis: Results = _BLL_Data.GetHisAQBJ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "1"); break;
                    //历史断电信息
                    case EnumDataType.AQDDHis: Results = _BLL_Data.GetHisAQDD(MineCode, TypeName, SensorNum, BeginTime, EndTime, "1"); break;
                    //历史馈电异常信息
                    case EnumDataType.AQKDHis: Results = _BLL_Data.GetHisAQYC(MineCode, DevType, SensorNum, BeginTime, EndTime); break;


                    #region 2015-2-3[修改记录]
                    //模拟量统计数据
                    case EnumDataType.AQMT: Results = _BLL_Data.GetMinutesData(MineCode, TypeName, BeginTime, EndTime); break;
                    //历史故障信息
                    case EnumDataType.AQGZHis: Results = _BLL_Data.GetHisAQGZ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "1"); break;
                    //测点类型下拉
                    case EnumDataType.PointType: Result = _BLL_Data.DeviceType(); break;
                    //实时断电信息
                    case EnumDataType.AQDD: Results = _BLL_Data.GetRealAQDD(MineCode, TypeName, "1"); break;
                    //实时馈电信息
                    case EnumDataType.AQKD: Results = _BLL_Data.GetRealAQYC(MineCode, TypeName, SensorNum); break;
                    //历史曲线
                    case EnumDataType.HistLine:
                        Results = _BLL_Data.GetMnlMinute_Curve(MineCode, DevType, SensorNum, BeginTime, EndTime); break;
                    #endregion

                    //模拟量统计数据
                    case EnumDataType.AQMNL_1M:
                    case EnumDataType.AQMNL_3M:
                    case EnumDataType.AQMNL_5M:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, BeginTime, EndTime, VIEW, "1"); break;
                    case EnumDataType.AQMNL_1D:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, DateTime.Parse(BeginTime.ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(BeginTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00")), VIEW, "1"); break;
                    case EnumDataType.AQMNL_30D:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, DateTime.Parse(BeginTime.ToString("yyyy-MM-01 00:00:00")), DateTime.Parse(BeginTime.AddMonths(1).ToString("yyyy-MM-01 00:00:00")), VIEW, "1"); break;

                    //开关量统计数据——郁森
                    case EnumDataType.AQKGL_Day: Results = _BLL_Data.GetAQKGLData_Day(MineCode, TypeName, BeginTime, BeginTime.AddDays(1)); break;
                    case EnumDataType.AQKGL_Week: Results = _BLL_Data.GetAQKGLData_Week(MineCode, TypeName, BeginTime, BeginTime.AddDays(7)); break;



                        //量程类型:
                    case EnumDataType.TypeKind:
                        Result = _BLL_Data.GetTypeKind(MineCode); break;
                    #endregion

                    #region [人员管理数据查询]
                    #region 基本信息
                    case EnumDataType.RatedNumber: Result = _BLL_Data.GetRYRatedNumber(MineCode); break;
                    //获取分站信息
                    case EnumDataType.RYStation: Results = _BLL_Data.GetRYFZ(MineCode, SensorNum); break;
                    //获取人员区域信息
                    case EnumDataType.RYAreaInfo: Results = _BLL_Data.GetRYQY(MineCode, SensorNum, DevType,TypeKind); break;
                    //获取人员信息
                    case EnumDataType.RYXX: Results = _BLL_Data.GetRYXX(MineCode, SensorNum, DevType); break;
                    //路线预设
                    case EnumDataType.RYPathInfo: Results = _BLL_Data.GetPathInfo(MineCode, SensorNum, DevType); break;
                    case EnumDataType.Duty:
                        Result = _BLL_Data.GetRYDuty(MineCode);
                        break;
                    case EnumDataType.Department:
                        Result = _BLL_Data.GetRYDepartment(MineCode);
                        break;
                    #endregion
                    #region 路线预设
                    case EnumDataType.PreRoute: Results = _BLL_Data.GetRYLXYS(MineCode, Position, Name); break;
                    #endregion
                    #region 实时数据
                    case EnumDataType.RealInPeople: Results = _BLL_Data.GetRealDataForRY(MineCode, SensorNum, DevType); break;
                    //实时通信状态
                    case EnumDataType.RealTXState: Results = _BLL_Data.GetRealTXState(MineCode); break;
                    #endregion
                    #region 实时报警
                    //实时超时报警
                    case EnumDataType.RealCS: Results = _BLL_Data.GetRYCS(MineCode, SensorNum, DevType, Position); break;
                    //实时限制报警
                    case EnumDataType.RealXZ: Results = _BLL_Data.GetRYXZ(MineCode, SensorNum, DevType); break;
                    //实时超员报警
                    case EnumDataType.RealCY: Results = _BLL_Data.GetRYCY(MineCode, SensorNum, DevType); break;
                    //实时超员报警
                    case EnumDataType.RealTZYC: Results = _BLL_Data.GetRYTZYC(MineCode, SensorNum, DevType); break;
                    #endregion
                    #region 历史数据
                    //获取历史上下井信息
                    case EnumDataType.HisTrack: Results = _BLL_Data.GetTrack(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //获取历史轨迹信息
                    case EnumDataType.HisTrackInfo: Results = _BLL_Data.GetTrackInfo(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //区域分站人员历史信息
                    case EnumDataType.QYFZQuery:
                        //Results = _BLL_Data.GetAreaStationPerson(MineCode, Name, BeginTime, EndTime); 
                        break;
                    //历史超时信息
                    case EnumDataType.HisCS: Results = _BLL_Data.GetHisCS(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史限制信息
                    case EnumDataType.HisXZ: Results = _BLL_Data.GetHisXZ(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史超员限制
                    case EnumDataType.HisCY: Results = _BLL_Data.GetHisCY(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史特种异常
                    case EnumDataType.HisTZYC: Results = _BLL_Data.GetHisTZYC(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史通信故障查询
                    case EnumDataType.HisTXState: Results = _BLL_Data.GetHisTXState(MineCode, BeginTime, EndTime); break;
                    #endregion
                    #region 报表分析
                    case EnumDataType.ReportRYCYBB:
                            Result = _BLL_Report.GetReportRYCYBB(MineCode , BeginTime, EndTime, flag); break;
                    case EnumDataType.ReportRYCSBB:
                        Result = _BLL_Report.GetReportRYCSBB(MineCode , BeginTime, EndTime, flag, Timespan); break;
                    case EnumDataType.Report: Result = _BLL_Report.GetReportData(ReportName, MineCode, DevType, BeginTime, BeginTime.AddDays(1)); break;

                    case EnumDataType.ReportMNLRKD:
                    case EnumDataType.ReportKGLRKD:
                        Result = _BLL_Report.GetReportData_KD(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW);
                        break;
                    case EnumDataType.ReportMNLYKD:
                    case EnumDataType.ReportKGLYKD:
                        Result = _BLL_Report.GetReportData_KD(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW);
                        break;

                    case EnumDataType.ReportMNLRDD:

                    case EnumDataType.ReportKGLRDD:
                        Result = _BLL_Report.GetReportData_DD(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW);
                        break;
                    case EnumDataType.ReportMNLYDD:
                    case EnumDataType.ReportKGLYDD:
                        Result = _BLL_Report.GetReportData_DD(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW);
                        break;

                    case EnumDataType.ReportMNLRBJ:
                        Result = _BLL_Report.GetReportData_BJ(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW,Timespan,TypeKind);
                        break;
                    case EnumDataType.ReportMNLYBJ:
                        Result = _BLL_Report.GetReportData_BJ(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW, Timespan, TypeKind);
                        break;

                    case EnumDataType.ReportSBGZR:
                        Result = _BLL_Report.GetReportData_SBGZ(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW, Timespan);
                        break;
                    case EnumDataType.ReportSBGZY:
                        Result = _BLL_Report.GetReportData_SBGZ(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW, Timespan);
                        break;

                    case EnumDataType.ReportRYCSBB_R:
                        Result = _BLL_Report.GetReportData_RYCSBB(MineCode, BeginTime, EndTime, VIEW);
                        break;
                    case EnumDataType.ReportRYCSBB_Y:
                        Result = _BLL_Report.GetReportData_RYCSBB(MineCode, BeginTime, EndTime, VIEW);
                        break;

                    case EnumDataType.ReportRYCSBB_B:
                        Result = _BLL_Report.GetReportData_RYCSBB_B(MineCode, BeginTime, EndTime);
                        break;

                    case EnumDataType.ReportRYCYBB_R:
                    case EnumDataType.ReportRYCYBB_Y:
                        Result = _BLL_Report.GetReportData_RYCSBB(MineCode, BeginTime, EndTime, VIEW);
                        break;

                    case EnumDataType.ReportRYSXJBB_R:
                    case EnumDataType.ReportRYSXJBB_Y:
                    case EnumDataType.ReportRYGBLDXJBB_R:
                    case EnumDataType.ReportRYGBLDXJBB_Y:
                        Result = _BLL_Report.GetReportData_RYSXJBB(MineCode, BeginTime, EndTime, VIEW);
                        break;
                    case EnumDataType.ReportRYTXYCBB:
                        Result = _BLL_Report.GetReportData_RYTXYCBB(MineCode, BeginTime, EndTime);
                        break;

                    #endregion

                    #endregion

                    #region 报表查询
                    ////获取通讯异常统计报表信息
                    //case EnumDataType.MineStateHisBB:
                    //    string json = _ReportBLL_Data.GetMineBB(MineCode);
                    //    return json;  
                    #endregion

                    #region [ 公告 ]

                    case EnumDataType.NoticeList:     // 公告列表
                        Result = _BLL_Data.NoticeList();
                        return JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }).Replace("shine998", "<br/>");
                    case EnumDataType.NoticeDelete:
                        return _BLL_Data.NoticeDelete(MineCode).ToString();

                    #endregion


                    #region [ 网络硬盘 ]

                    case EnumDataType.DiskTree://目录树
                        return CallTreeGridDataFormat(_BLL_Data.NetDiskTree(MineCode));
                    case EnumDataType.FileList://文件列表
                        IsoDateTimeConverter timeConverter1 = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                        DataTable dt = _BLL_Data.NetDiskFileList(MineCode);
                        return "{\"total\":" + dt.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter1) + "}";
                    case EnumDataType.RemoveFiles:// 删除文件
                        return _BLL_Data.RemoveFiles(MineCode).ToString();
                    case EnumDataType.RemoveDisk:// 删除目录
                        return _BLL_Data.RemoveDisk(MineCode).ToString();
                    case EnumDataType.DiskReName:// 新增子目录，重命名
                        return _BLL_Data.DiskReName(MineCode, SensorNum, DropName, ReportName, Position).ToString();
                    case EnumDataType.DiskViewUsers:
                        Result = _BLL_Data.DiskViewUsers();
                        IsoDateTimeConverter timeConverter2 = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                        return JsonConvert.SerializeObject(Result, Formatting.Indented, timeConverter2);
                    case EnumDataType.DiskSaveUsers:
                        return _BLL_Data.DiskSaveUsers(MineCode, SensorNum, DropName).ToString();

                    #endregion

                    // 预警

                    #region [ 预警 ]

                    case EnumDataType.WarnMineName:
                        Result = _BLL_Data.MineList();
                        DataRow dr = Result.NewRow();
                        //dr["MineCode"] = -1;
                        dr["SimpleName"] = "全部";
                        Result.Rows.InsertAt(dr, 0);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter()) +
                               "}";
                    case EnumDataType.WarnDevType: Result = m_deviceKind == null ? _BLL_Data.DeviceList() : _BLL_Data.DeviceList(m_deviceKind);
                        DataRow dr1 = Result.NewRow();
                        dr1["TypeName"] = "全部";
                        Result.Rows.InsertAt(dr1, 0);

                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter()) +
                               "}";
                    case EnumDataType.WarnList:
                        Result = _BLL_Data.WarmList(MineCode, SensorNum, DropName);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }) + "}";
                    case EnumDataType.WarnSave:
                        return _BLL_Data.WarmSave(MineCode, SensorNum, DropName, ReportName).ToString();

                    case EnumDataType.WarnToHis:
                        return _BLL_Data.WarmToHis(MineCode).ToString();

                    case EnumDataType.WarnAlarmHis:
                        Result = _BLL_Data.WarnAlarmHis(MineCode, SensorNum, DropName, ReportName, Position);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }) + "}";

                    case EnumDataType.WarnAlarmTotal:
                        Result = _BLL_Data.WarnAlarmTotal(MineCode, SensorNum, ReportName, Position);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }) + "}";

                    #endregion

                    #region 【矿压】
                    case EnumDataType.RealData_KY:
                        Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "5");
                        break;
                    //加载实时故障
                    case EnumDataType.AQGZ_KY:
                        Results = _BLL_Data.GetRealAQGZ(MineCode, DevType, "5", SensorNum);
                        break;
                    case EnumDataType.AQGZHis_KY: Results = _BLL_Data.GetHisAQGZ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "5");
                        break;
                    case EnumDataType.AQBJ_KY: Results = _BLL_Data.GetRealAQBJ(MineCode, TypeName, "5",SensorNum);
                        break;
                    case EnumDataType.AQMNL_1M_KY:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, BeginTime, EndTime, VIEW, "5");
                        break;
                    case EnumDataType.AQBJHis_KY: Results = _BLL_Data.GetHisAQBJ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "5");
                        break;


                    #endregion


                    #region 【火灾束管】
                    case EnumDataType.RealData_HG:
                        Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "7");
                        break;
                    //加载实时故障
                    case EnumDataType.AQGZ_HG:
                        Results = _BLL_Data.GetRealAQGZ(MineCode, DevType, "7", SensorNum);
                        break;
                    case EnumDataType.AQGZHis_HG: Results = _BLL_Data.GetHisAQGZ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "7");
                        break;
                    case EnumDataType.AQBJ_HG: Results = _BLL_Data.GetRealAQBJ(MineCode, TypeName, "7",SensorNum);
                        break;
                    case EnumDataType.AQMNL_1M_HG:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, BeginTime, EndTime, VIEW, "7");
                        break;
                    case EnumDataType.AQBJHis_HG: Results = _BLL_Data.GetHisAQBJ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "7");
                        break;


                    #endregion


                    #region 瓦斯抽放
                    case EnumDataType.RealData_WS:
                        Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "11");
                        break;

                    #endregion


               
                  
                    //返回空值 
                    default: Result = new DataTable(); break;
                }
                #endregion

                string json = "";
                int TotalRows = 0;
                //在对DATATABLE进行序列化的时候，规范日期格式
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                if (Result != null)
                {

                    if (VIEW == EnumDataType.ReportSBGZR || VIEW == EnumDataType.ReportSBGZY || VIEW == EnumDataType.ReportMNLYBJ || VIEW == EnumDataType.ReportMNLRBJ)
                        {
                            for (int i = 0; i < Result.Rows.Count; i++)
                            {
                                Result.Rows[i]["持续时间"] = ReturnTSFM(Result.Rows[i]["Continuous"].ToString());
                            }

                        }

                    if (VIEW == EnumDataType.ReportRYCSBB )
                    {
                        for (int i = 0; i < Result.Rows.Count; i++)
                        {
                            Result.Rows[i]["Continuous"] = ReturnTSFM(Result.Rows[i]["Continuous_Tmp"].ToString());
                            Result.Rows[i]["MaxTime"] = ReturnTSFM(Result.Rows[i]["MaxTime_Tmp"].ToString());
                            Result.Rows[i]["MinTime"] = ReturnTSFM(Result.Rows[i]["MinTime_Tmp"].ToString());
                        }
                    
                    }
                    if (VIEW == EnumDataType.ReportRYCYBB)
                    {
                        for (int i = 0; i < Result.Rows.Count; i++)
                        {
                            Result.Rows[i]["Continuous"] = ReturnTSFM(Result.Rows[i]["Continuous_Tmp"].ToString());
                            Result.Rows[i]["CYL"] = Convert.ToInt32(Convert.ToDouble(Result.Rows[i]["Counts"].ToString()) / Convert.ToDouble(Result.Rows[i]["Number"].ToString()) * 100).ToString();
                        }

                    }
                    
                TotalRows = Result.Rows.Count;
                    json = JsonConvert.SerializeObject(Result, Formatting.Indented, timeConverter);

                }
                else
                {
                    if (Results != null)
                    {
                        TotalRows = int.Parse(Results[0].Rows[0][0].ToString());
                        json = JsonConvert.SerializeObject(Results[1], Formatting.Indented, timeConverter);

                    
                    }
                }

                //if (Result.Rows.Count == 0 && Results != null && Results.Count > 1)
                //{
                //    Result = Results[1];
                //    TotalRows = int.Parse(Results[0].Rows[0][0].ToString());
                //}
                //datagrid的格式是 total:  rows
                //return "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";
                 return "{\"totalrows\": " + TotalRows + ",\"data\":" + json + "}";   //这是supcan中的
                //return JsonConvert.SerializeObject(Result);
            }
        }
        /// <summary>
        /// 获取Data Json
        /// </summary>
        /// <returns></returns>
        public string GetDataJsonDG
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["Export"].ToString() == "y")
                {
                    _BLL_Data.PageIndex = 0;
                    _BLL_Data.PageSize = Int32.MaxValue;

                }
                else { 
                  if (Rows != 0)
                {
                    _BLL_Data.PageIndex = StartRow / Rows;
                    _BLL_Data.PageSize = Rows;
                }
                else
                {
                    _BLL_Data.PageIndex = 0;
                    _BLL_Data.PageSize = 20;
                }
                
                }

                DataTable Result = null;
                DataTableCollection Results;
                EnumDataType VIEW = (EnumDataType)Enum.Parse(typeof(EnumDataType), TreeListType);//字符串转化为枚举
                Results = null;
                #region 枚举方式
           
                switch (VIEW)
                {
                    case EnumDataType.ShowAQJKRTData:
                        Result = new DataTable();
                        return "";

                    #region [加载查询bar数据]
                    case EnumDataType.Filter:
                        if (SystemType == 1)
                        {
                            switch (DropName)
                            {
                                case "DevType": Result = _BLL_Data.GetDevTypeList(MineCode,TypeKind); break;
                                case "SensorNum": Result = _BLL_Data.GetRealDataForAQList(MineCode, DevType, SystemType.ToString()); break;
                            }

                        }
                        else
                        {
                            switch (DropListType)
                            {
                                case "DevType": _BLL_Data.GetRealDataForAQList(MineCode, DevType, SystemType.ToString()); break;
                                case "FZ": Result = _BLL_Data.GetRYFZList(MineCode); break;
                                case "QY": Result = _BLL_Data.GetRYQYList(MineCode); break;
                                case "Card": Result = _BLL_Data.GetRYXXList(MineCode, Position, Name); break;
                                case "Name": Result = _BLL_Data.GetRYXXList(MineCode, Position, Name); break;
                            }
                        }
                        break;
                    #endregion

                    #region [安全监控数据查询]
                        //量程类型
                    case EnumDataType.TypeKind:
                        Result = _BLL_Data.GetTypeKind(MineCode); break;
                    //获取所有煤矿信息  
                    case EnumDataType.MineName: Result = _BLL_Data.MineList(); break;
                    //获取测点编号 
                    case EnumDataType.Sensor: Results = _BLL_Data.GetRealDataForAQ(MineCode); break;
                    //获取所有设备名
                    case EnumDataType.DevType: Result = _BLL_Data.DeviceList(m_deviceKind); break;
                    //获取实时数据
                    case EnumDataType.RealData: Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "","1"); break;
                    //加载煤矿信息内容 
                    case EnumDataType.Mine: Result = _BLL_Data.MineList(); break;
                    //加载实时故障
                    case EnumDataType.AQGZ: Results = _BLL_Data.GetRealAQGZ(MineCode, DevType, "1", SensorNum); break;
                    //获取测点配置信息  
                    case EnumDataType.PointSet: Results = _BLL_Data.GetDeviceInfo(MineCode, DevType, SensorNum); break;
                    //获取煤矿信息 
                    case EnumDataType.MineInfoData: Result = _BLL_Data.MineList(MineCode); break;
                    //获取煤矿传输状态 
                    case EnumDataType.MineState: Result = _BLL_Data.GetBadLog(MineCode, SystemType); break;
                    //获取子系统配置信息 
                    case EnumDataType.ChildSystemSet:
                        Result = _BLL_Data.GetChildSystemSet(MineCode); break;
                    //获取实时报警信息 
                    case EnumDataType.AQBJ: Results = _BLL_Data.GetRealAQBJ(MineCode, TypeName,"1",SensorNum); break;
                    //历史开关量统计查询
                    case EnumDataType.AQMCHis: Results = _BLL_Data.GetHisAQLT(MineCode, DevType, BeginTime, EndTime); break;
                    //历史报警信息 
                    case EnumDataType.AQBJHis: Results = _BLL_Data.GetHisAQBJ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "1"); break;
                    //历史断电信息
                    case EnumDataType.AQDDHis: Results = _BLL_Data.GetHisAQDD(MineCode, TypeName, SensorNum, BeginTime, EndTime,"1"); break;
                    //历史馈电异常信息
                    case EnumDataType.AQKDHis: Results = _BLL_Data.GetHisAQYC(MineCode, DevType, SensorNum, BeginTime, EndTime); break;


                    #region 2015-2-3[修改记录]
                    //模拟量统计数据
                    case EnumDataType.AQMT: Results = _BLL_Data.GetMinutesData(MineCode, TypeName, BeginTime, EndTime); break;
                    //历史故障信息
                    case EnumDataType.AQGZHis: Results = _BLL_Data.GetHisAQGZ(MineCode, TypeName, SensorNum, BeginTime, EndTime,"1"); break;
                    //测点类型下拉
                    case EnumDataType.PointType: Result = _BLL_Data.DeviceType(); break;
                    //实时断电信息
                    case EnumDataType.AQDD: Results = _BLL_Data.GetRealAQDD(MineCode, TypeName,"1"); break;
                    //实时馈电信息
                    case EnumDataType.AQKD: Results = _BLL_Data.GetRealAQYC(MineCode, TypeName, SensorNum); break;
                    //历史曲线
                    case EnumDataType.HistLine:
                        Results = _BLL_Data.GetMnlMinute_Curve(MineCode, DevType, SensorNum, BeginTime, EndTime); break;
                    #endregion

                    //模拟量统计数据
                    case EnumDataType.AQMNL_1M:
                    case EnumDataType.AQMNL_3M:
                    case EnumDataType.AQMNL_5M:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, BeginTime, EndTime, VIEW,"1"); break;
                    case EnumDataType.AQMNL_1D:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, DateTime.Parse(BeginTime.ToString("yyyy-MM-dd 00:00:00")), DateTime.Parse(BeginTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00")), VIEW,"1"); break;
                    case EnumDataType.AQMNL_30D:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, DateTime.Parse(BeginTime.ToString("yyyy-MM-01 00:00:00")), DateTime.Parse(BeginTime.AddMonths(1).ToString("yyyy-MM-01 00:00:00")), VIEW,"1"); break;

                    //开关量统计数据——郁森
                    case EnumDataType.AQKGL_Day: Results = _BLL_Data.GetAQKGLData_Day(MineCode, TypeName, BeginTime, BeginTime.AddDays(1)); break;
                    case EnumDataType.AQKGL_Week: Results = _BLL_Data.GetAQKGLData_Week(MineCode, TypeName, BeginTime, BeginTime.AddDays(7)); break;


                        // 预警管理
                   
                    case EnumDataType.PreAlarm:
                        Results = _BLL_Data.GetData_PreAlarm(MineCode,SensorNum,TypeName);break;
                    case EnumDataType.PreAlarm_His:
                        Results = _BLL_Data.GetData_PreAlarm_His(MineCode,SensorNum,TypeName,BeginTime,EndTime); break;
                    case EnumDataType.PreAlarm_TG:
                         Results = _BLL_Data.GetData_PreAlarm_TG(MineCode,SensorNum,TypeName,BeginTime,EndTime,flag);break;



                        //通讯状态
                        //历史通讯中断
                    case EnumDataType.TXZDHis:
                         Results = _BLL_Data.GetData_TXZDHis(MineCode,BeginTime,EndTime);break;

                    //实时通讯中断
                    case EnumDataType.TXZD:
                        Results = _BLL_Data.GetData_TXZD(MineCode); break;
                    //历史通讯异常
                    case EnumDataType.TXYCHis:
                        Results = _BLL_Data.GetData_TXYCHis(MineCode, BeginTime, EndTime); break;


                    //历史通讯中断统计
                    case EnumDataType.TXZDTG:
                        Results = _BLL_Data.GetData_TXZDTG(MineCode, BeginTime, EndTime,flag); break;
                      //通讯异常统计
                    case EnumDataType.TXYCTG:
                        Results = _BLL_Data.GetData_TXYCTG(MineCode, BeginTime, EndTime, flag); break;


                        // 故障统计
                    case EnumDataType.GZTG:
                        Results = _BLL_Data.GetData_GZTG(MineCode,SensorNum,TypeName, BeginTime, EndTime, flag); break;
                        //报警统计：
                    case EnumDataType.BJTG:
                        Results=_BLL_Data.GetData_BJTG(MineCode ,SensorNum,TypeName, BeginTime,EndTime,flag,TypeKind); break;
                        // 模拟量统计
                    case EnumDataType.MNLTG:
                          Results=_BLL_Data.GetData_MNLTG(MineCode ,SensorNum,TypeName, BeginTime,EndTime,flag); break;
                      #endregion
                    #region [人员管理数据查询]
                    #region 基本信息
                   
        
               
                    case EnumDataType.RatedNumber: Result = _BLL_Data.GetRYRatedNumber(MineCode); break;
                    //获取分站信息
                    case EnumDataType.RYStation: Results = _BLL_Data.GetRYFZ(MineCode, SensorNum); break;
                    //获取人员区域信息
                    case EnumDataType.RYAreaInfo: Results = _BLL_Data.GetRYQY(MineCode, SensorNum, DevType,TypeKind); break;
                    //获取人员信息
                    case EnumDataType.RYXX: Results = _BLL_Data.GetRYXX(MineCode, SensorNum, DevType); break;
                    //路线预设
                    case EnumDataType.RYPathInfo: Results = _BLL_Data.GetPathInfo(MineCode, SensorNum, DevType); break;
                    case EnumDataType.Duty:
                        Result = _BLL_Data.GetRYDuty(MineCode);
                        break;
                    case EnumDataType.Department:
                        Result = _BLL_Data.GetRYDepartment(MineCode);
                        break;
                    #endregion
                    #region 路线预设
                    case EnumDataType.PreRoute: Results = _BLL_Data.GetRYLXYS(MineCode, Position, Name); break;
                    #endregion
                    #region 实时数据

                    case EnumDataType.RYSSTG:
                     Results = _BLL_Data.GetRYSSTG(MineCode); break;
                    case EnumDataType.RYSS: Results = _BLL_Data.GetRYSS(MineCode, SensorNum, DevType, Job, Department); break;
                    case EnumDataType.RealInPeople: Results = _BLL_Data.GetRealDataForRY(MineCode, SensorNum, DevType); break;
                    //实时通信状态
                    case EnumDataType.RealTXState: Results = _BLL_Data.GetRealTXState(MineCode); break;
                    #endregion
                    #region 实时报警
                    //实时超时报警
                    case EnumDataType.RealCS: Results = _BLL_Data.GetRYCS(MineCode, SensorNum, DevType, Position); break;
                    //实时限制报警
                    case EnumDataType.RealXZ: Results = _BLL_Data.GetRYXZ(MineCode, SensorNum, DevType); break;
                    //实时超员报警
                    case EnumDataType.RealCY: Results = _BLL_Data.GetRYCY(MineCode, SensorNum, DevType); break;
                    //实时超员报警
                    case EnumDataType.RealTZYC: Results = _BLL_Data.GetRYTZYC(MineCode, SensorNum, DevType); break;
                    #endregion
                    #region 历史数据

                    case EnumDataType.RYXJTG:
                        Results = _BLL_Data.GetData_XJRYTG(MineCode, DevType, Job, Department, BeginTime, EndTime, flag);break;

                        //限制区域人员统计

                    case EnumDataType.XZQYRYTG:
                        Results=_BLL_Data.GetXZQYRYTG(MineCode, SensorNum,BeginTime,EndTime,flag); break;
                    case EnumDataType.RYBJTG:
                        Results = _BLL_Data.GetRYBJTG(MineCode, BeginTime, EndTime, flag);break;

                    //获取历史上下井信息
                    case EnumDataType.HisTrack: Results = _BLL_Data.GetTrack(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //获取历史轨迹信息
                    case EnumDataType.HisTrackInfo: Results = _BLL_Data.GetTrackInfo(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //区域分站人员历史信息
                    case EnumDataType.QYFZQuery:
                        //Results = _BLL_Data.GetAreaStationPerson(MineCode, Name, BeginTime, EndTime); 
                        break;
                    //历史超时信息
                    case EnumDataType.HisCS: Results = _BLL_Data.GetHisCS(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史限制信息
                    case EnumDataType.HisXZ: Results = _BLL_Data.GetHisXZ(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史超员限制
                    case EnumDataType.HisCY: Results = _BLL_Data.GetHisCY(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史特种异常
                    case EnumDataType.HisTZYC: Results = _BLL_Data.GetHisTZYC(MineCode, SensorNum, DevType, BeginTime, EndTime); break;
                    //历史通信故障查询
                    case EnumDataType.HisTXState: Results = _BLL_Data.GetHisTXState(MineCode, BeginTime, EndTime); break;


                    case EnumDataType.XJSJBZTG: Results = _BLL_Data.GetXJSJBZTG(MineCode, DevType, Job, Department, BeginTime, EndTime, Timespan,flag); break;
                    #endregion
                    #region 报表分析
                    case EnumDataType.Report: Result = _BLL_Report.GetReportData(ReportName, MineCode, DevType, BeginTime, BeginTime.AddDays(1)); break;

                    case EnumDataType.ReportMNLRKD:
                    case EnumDataType.ReportKGLRKD:
                        Result = _BLL_Report.GetReportData_KD(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW);
                        break;
                    case EnumDataType.ReportMNLYKD:
                    case EnumDataType.ReportKGLYKD:
                        Result = _BLL_Report.GetReportData_KD(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW);
                        break;

                    case EnumDataType.ReportMNLRDD:

                    case EnumDataType.ReportKGLRDD:
                        Result = _BLL_Report.GetReportData_DD(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW);
                        break;
                    case EnumDataType.ReportMNLYDD:
                    case EnumDataType.ReportKGLYDD:
                        Result = _BLL_Report.GetReportData_DD(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW);
                        break;

                    case EnumDataType.ReportMNLRBJ:
                        Result = _BLL_Report.GetReportData_BJ(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW, Timespan,TypeKind);
                        break;
                    case EnumDataType.ReportMNLYBJ:
                        Result = _BLL_Report.GetReportData_BJ(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW, Timespan, TypeKind);
                        break;

                    case EnumDataType.ReportSBGZR:
                        Result = _BLL_Report.GetReportData_SBGZ(MineCode, DevType, BeginTime, BeginTime.AddDays(1), VIEW, Timespan);
                        break;
                    case EnumDataType.ReportSBGZY:
                        Result = _BLL_Report.GetReportData_SBGZ(MineCode, DevType, BeginTime, BeginTime.AddMonths(1), VIEW, Timespan);
                        break;

                    case EnumDataType.ReportRYCSBB_R:
                        Result = _BLL_Report.GetReportData_RYCSBB(MineCode, BeginTime, EndTime, VIEW);
                        break;
                    case EnumDataType.ReportRYCSBB_Y:
                        Result = _BLL_Report.GetReportData_RYCSBB(MineCode, BeginTime, EndTime, VIEW);
                        break;

                    case EnumDataType.ReportRYCSBB_B:
                        Result = _BLL_Report.GetReportData_RYCSBB_B(MineCode, BeginTime, EndTime);
                        break;

                    case EnumDataType.ReportRYCYBB_R:
                    case EnumDataType.ReportRYCYBB_Y:
                        Result = _BLL_Report.GetReportData_RYCSBB(MineCode, BeginTime, EndTime, VIEW);
                        break;

                    case EnumDataType.ReportRYSXJBB_R:
                    case EnumDataType.ReportRYSXJBB_Y:
                    case EnumDataType.ReportRYGBLDXJBB_R:
                    case EnumDataType.ReportRYGBLDXJBB_Y:
                        Result = _BLL_Report.GetReportData_RYSXJBB(MineCode, BeginTime, EndTime, VIEW);
                        break;
                    case EnumDataType.ReportRYTXYCBB:
                        Result = _BLL_Report.GetReportData_RYTXYCBB(MineCode, BeginTime, EndTime);
                        break;

                    #endregion

                    #endregion

                    #region 报表查询
                    ////获取通讯异常统计报表信息
                    //case EnumDataType.MineStateHisBB:
                    //    string json = _ReportBLL_Data.GetMineBB(MineCode);
                    //    return json;  
                    #endregion

                    #region [ 公告 ]

                    case EnumDataType.NoticeList:     // 公告列表
                        Result = _BLL_Data.NoticeList();
                        return JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }).Replace("shine998", "<br/>");
                    case EnumDataType.NoticeDelete:
                        return _BLL_Data.NoticeDelete(MineCode).ToString();

                    #endregion


                    #region [ 网络硬盘 ]

                    case EnumDataType.DiskTree://目录树
                        return CallTreeGridDataFormat(_BLL_Data.NetDiskTree(MineCode));
                    case EnumDataType.FileList://文件列表
                        IsoDateTimeConverter timeConverter1 = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                        DataTable dt = _BLL_Data.NetDiskFileList(MineCode);
                        return "{\"total\":" + dt.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter1) + "}";
                    case EnumDataType.RemoveFiles:// 删除文件
                        return _BLL_Data.RemoveFiles(MineCode).ToString();
                    case EnumDataType.RemoveDisk:// 删除目录
                        return _BLL_Data.RemoveDisk(MineCode).ToString();
                    case EnumDataType.DiskReName:// 新增子目录，重命名
                        return _BLL_Data.DiskReName(MineCode, SensorNum, DropName, ReportName, Position).ToString();
                    case EnumDataType.DiskViewUsers:
                        Result = _BLL_Data.DiskViewUsers();
                        IsoDateTimeConverter timeConverter2 = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                        return JsonConvert.SerializeObject(Result, Formatting.Indented, timeConverter2);
                    case EnumDataType.DiskSaveUsers:
                        return _BLL_Data.DiskSaveUsers(MineCode, SensorNum, DropName).ToString();

                    #endregion

                    // 预警

                    #region [ 预警 ]

                    case EnumDataType.WarnMineName:
                        Result = _BLL_Data.MineList();
                        DataRow dr = Result.NewRow();
                        //dr["MineCode"] = -1;
                        dr["SimpleName"] = "全部";
                        Result.Rows.InsertAt(dr, 0);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter()) +
                               "}";
                    case EnumDataType.WarnDevType: Result = m_deviceKind == null ? _BLL_Data.DeviceList() : _BLL_Data.DeviceList(m_deviceKind);
                        DataRow dr1 = Result.NewRow();
                        dr1["TypeName"] = "全部";
                        Result.Rows.InsertAt(dr1, 0);

                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter()) +
                               "}";
                    case EnumDataType.WarnList:
                        Result = _BLL_Data.WarmList(MineCode, SensorNum, DropName);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }) + "}";
                    case EnumDataType.WarnSave:
                        return _BLL_Data.WarmSave(MineCode, SensorNum, DropName, ReportName).ToString();

                    case EnumDataType.WarnToHis:
                        return _BLL_Data.WarmToHis(MineCode).ToString();

                    case EnumDataType.WarnAlarmHis:
                        Result = _BLL_Data.WarnAlarmHis(MineCode, SensorNum, DropName, ReportName, Position);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }) + "}";

                    case EnumDataType.WarnAlarmTotal:
                        Result = _BLL_Data.WarnAlarmTotal(MineCode, SensorNum, ReportName, Position);
                        return "{\"total\":" + Result.Rows.Count.ToString() + ",\"rows\":" +
                               JsonConvert.SerializeObject(Result, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }) + "}";

                    #endregion

                    #region 【矿压】
                    case EnumDataType.RealData_KY:
                        Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "5"); 
                        break;
                    //加载实时故障
                   case EnumDataType.AQGZ_KY:
                        Results = _BLL_Data.GetRealAQGZ(MineCode, DevType, "5", SensorNum);
                        break;
                   case EnumDataType.AQGZHis_KY: Results = _BLL_Data.GetHisAQGZ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "5"); 
                        break;
                   case EnumDataType.AQBJ_KY: Results = _BLL_Data.GetRealAQBJ(MineCode, TypeName, "5",SensorNum); 
                        break;
                   case EnumDataType.AQMNL_1M_KY:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, BeginTime, EndTime, VIEW, "5"); 
                        break;
                   case EnumDataType.AQBJHis_KY: Results = _BLL_Data.GetHisAQBJ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "5"); 
                        break;

                    
                    #endregion

                   #region 瓦斯抽放
                   case EnumDataType.RealData_WS:
                        Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "11");
                        break;

                   #endregion


                   #region JSG8火灾束管
                   case EnumDataType.RealData_JSG8:
                        Results = _BLL_Data.GetRealDataForJSG8(BeginTime,EndTime);
                        break;

                   #endregion
                   #region 【火灾束管】
                   case EnumDataType.RealData_HG:
                        Results = _BLL_Data.GetRealDataForAQ(MineCode, DevType, SensorNum, "", "7");
                        break;
                   //加载实时故障
                   case EnumDataType.AQGZ_HG:
                        Results = _BLL_Data.GetRealAQGZ(MineCode, DevType, "7", SensorNum);
                        break;
                   case EnumDataType.AQGZHis_HG: Results = _BLL_Data.GetHisAQGZ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "7");
                        break;
                   case EnumDataType.AQBJ_HG: Results = _BLL_Data.GetRealAQBJ(MineCode, TypeName, "7",SensorNum);
                        break;
                   case EnumDataType.AQMNL_1M_HG:
                        Results = _BLL_Data.GetData_AQMNL(MineCode, TypeName, SensorNum, BeginTime, EndTime, VIEW, "7");
                        break;
                   case EnumDataType.AQBJHis_HG: Results = _BLL_Data.GetHisAQBJ(MineCode, TypeName, SensorNum, BeginTime, EndTime, "7");
                        break;


                   #endregion


                   #region
                    case EnumDataType.GBHZ:Results = _BLL_Data.GetGBHZ(MineCode);
                        break;
                    case EnumDataType.GBSS: Results = _BLL_Data.GetGBSS(MineCode); break;
                        //GetCKSS
                         case EnumDataType.CKSS: Results = _BLL_Data.GetCKSS(MineCode);
                        break;
                   #endregion


                        
                         #region  六个子系统
                        //设备类型
                    case   EnumDataType.ChildSysTypeKind:
                        Result = _BLL_Data.LoadChildSysTypeKind(MineCode);
                        break;

                        //通风系统
                         case EnumDataType.TFST:
                        case EnumDataType.PDST:
                        case EnumDataType.WSCF:
                        case EnumDataType.GDST:
                        case EnumDataType.YFST:
                        case EnumDataType.SPST:
                        Results = _BLL_Data.GetRealDataForTFST(TypeKind, TypeName, SensorNum,VIEW.ToString());
                        break;
                        //历史模拟量
                    case EnumDataType.TFMNLHis:
                    case EnumDataType.PDMNLHis:
                    case EnumDataType.YFMNLHis:
                    case EnumDataType.GDMNLHis:
                    case EnumDataType.SPMNLHis:
                    case EnumDataType.WSMNLHis:
                          Results = _BLL_Data.GetRealDataForTFSTHisMNL(TypeName, SensorNum,BeginTime,EndTime,VIEW.ToString(),flag);
                        break;
                        //TFKGLHis
                        // 历史开关量
                    case EnumDataType.TFKGLHis:
                    case EnumDataType.PDKGLHis:

                    case EnumDataType.YFKGLHis:
                    case EnumDataType.GDKGLHis:
                    case EnumDataType.SPKGLHis:
                    case EnumDataType.WSKGLHis:
                        Results = _BLL_Data.GetRealDataForTFSTHisKGL(TypeName, SensorNum, BeginTime, EndTime,VIEW.ToString(),flag);
                        break;
                        //历史故障
                    case EnumDataType.TFGZHis:
                    case EnumDataType.PDGZHis:
                    case EnumDataType.SPGZHis:
                    case EnumDataType.WSGZHis:
                    case EnumDataType.YFGZHis:
                    case EnumDataType.GDGZHis:
                         Results = _BLL_Data.GetRealDataForTFSTHisGZ(TypeKind,TypeName, SensorNum, BeginTime, EndTime,VIEW.ToString());
                        break;
                        // 实时故障
                    case EnumDataType.TFGZ:
                    case EnumDataType.PDGZ:
                    case EnumDataType.YFGZ:
                    case EnumDataType.GDGZ:
                    case EnumDataType.WSGZ:
                    case EnumDataType.SPGZ:
                        Results = _BLL_Data.GetRealDataForTFSTGZ(TypeKind, TypeName, SensorNum, VIEW.ToString());
                        break;

                        //实时断线
                    case EnumDataType.PDDX:
                    case EnumDataType.TFDX:
                    case EnumDataType.GDDX:
                    case EnumDataType.YFDX:
                    case EnumDataType.SPDX:
                    case EnumDataType.WSDX:
                        Results = _BLL_Data.GetRealDataForTFSTDX(TypeKind, TypeName, SensorNum, VIEW.ToString());
                        break;
                        //故障统计
                    case EnumDataType.TFGZTG:
                    case EnumDataType.PDGZTG:
                    case EnumDataType.YFGZTG:
                    case EnumDataType.GDGZTG:
                    case EnumDataType.SPGZTG:
                    case EnumDataType.WSGZTG:
                        Results = _BLL_Data.GetRealDataForTFGZTG(TypeKind, TypeName, SensorNum, BeginTime, EndTime, VIEW.ToString(),flag);
                        break;

                    case  EnumDataType.TFMNLFZHis:
                    case EnumDataType.YFMNLFZHis:
                    case EnumDataType.GDMNLFZHis:
                    case EnumDataType.WSMNLFZHis:
                    case EnumDataType.SPMNLFZHis:
                    case EnumDataType.PDMNLFZHis:
                           Results = _BLL_Data.GetRealDataForTFSTHisFZMNL(TypeName, SensorNum,BeginTime,EndTime,VIEW.ToString(),flag);
                        break;
                        //OPC通讯状态
                    case EnumDataType.OPCState:
                         Results= _BLL_Data.GetOPCSatate(SensorNum);
                        break;
                    case EnumDataType.DTSRealData:
                        Results = _BLL_Data.GetRealDataForDTSRealData(DevType, SensorNum);
                        break;

                         #endregion
                   //返回空值 
                    default: Result = new DataTable(); break;



                }
                #endregion
              
                string json = "";
                int TotalRows = 0;
                //在对DATATABLE进行序列化的时候，规范日期格式
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                if(Result!=null)
                {
                    TotalRows = Result.Rows.Count;
                    json = JsonConvert.SerializeObject(Result, Formatting.Indented, timeConverter);
               
                }
                else
                {
                    if (Results != null && Results.Count==2)
                    {
                        TotalRows = int.Parse(Results[0].Rows[0][0].ToString());


                        #region 预警、历史预警 时间处理
                        if (VIEW == EnumDataType.PreAlarm || VIEW == EnumDataType.PreAlarm_His)
                        {
                            for (int i = 0; i <Results[1].Rows.Count; i++)
                            {
                                Results[1].Rows[i]["Continuous"] = ReturnTSFM(Results[1].Rows[i]["LastTime"].ToString());
                                
                            }

                        }
                        #endregion
                        #region 通讯状态时间处理
                        if (VIEW == EnumDataType.TXZDHis)
                        {
                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {

                                if (!string.IsNullOrEmpty(Results[1].Rows[i]["Continuous"].ToString()))

                                {
                                    
                                    TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(Results[1].Rows[i]["Continuous"]));
                                    Results[1].Rows[i]["LastTime"] = ts.Days + "天" + ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";

                                    DateTime dt = Convert.ToDateTime(Results[1].Rows[i]["StartTime"].ToString()) + ts;
                                    Results[1].Rows[i]["EndTime"] = dt;


                                }
                                else
                                {
                                    Results[1].Rows[i]["Continuous"] = DBNull.Value;
                                }
                                
                            }

                        }

                        #endregion
                        #region 历史通讯异常事  时间处理
                        if (VIEW == EnumDataType.TXYCHis)
                        {

                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {
                                if (!string.IsNullOrEmpty(Results[1].Rows[i]["BeginTime"].ToString()))
                                {
                                     if (!string.IsNullOrEmpty(Results[1].Rows[i]["EndTime"].ToString()))
                                     {
                                         TimeSpan ts = Convert.ToDateTime(Results[1].Rows[i]["EndTime"])-Convert.ToDateTime(Results[1].Rows[i]["BeginTime"]) ;
                                         Results[1].Rows[i]["Continuous"] = ts.Days + "天" + ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                                     }
                                
                                }

                            }

                        }
                        #endregion
                        #region 通讯中断统计、通讯异常统计 时间处理
                        if ( VIEW == EnumDataType.TXYCTG)
                        {

                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {

                                Results[1].Rows[i]["LongestTime"] = ReturnTSFM(Results[1].Rows[i]["LongestTimes"].ToString());
                                Results[1].Rows[i]["ShortestTime"] = ReturnTSFM(Results[1].Rows[i]["ShortestTimes"].ToString());
                                Results[1].Rows[i]["SumTime"] = ReturnTSFM(Results[1].Rows[i]["SumTimes"].ToString());

                            }

                        }
                        #endregion
                        #region 报警统计 时间处理
                        if (VIEW==EnumDataType.BJTG)
                        {
                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {
                                Results[1].Rows[i]["Continuous"] = ReturnTSFM(Results[1].Rows[i]["Continuous_Tmp"].ToString());
                                Results[1].Rows[i]["MaxTime"] = ReturnTSFM(Results[1].Rows[i]["MaxTime_Tmp"].ToString());
                                Results[1].Rows[i]["MinTime"] = ReturnTSFM(Results[1].Rows[i]["MinTime_Tmp"].ToString());
                                Results[1].Rows[i]["AvgTime"] = ReturnTSFM(Results[1].Rows[i]["AvgTime_Tmp"].ToString());
                            }
                        }
                        #endregion


                        #region 人员管理 实时数据 时间处理
                        if (VIEW == EnumDataType.RYSS)
                        {
                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {
                                Results[1].Rows[i]["InMainTime"] = ReturnTSFM(Results[1].Rows[i]["InTimes"].ToString());
                                Results[1].Rows[i]["InStTime"] = ReturnTSFM(Results[1].Rows[i]["InStTimes"].ToString());
                            }
                        }
                        #endregion 


                        #region 人员下井统计
                        if (VIEW == EnumDataType.RYXJTG)
                        {
                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {
                                Results[1].Rows[i]["Continuous"] = ReturnTSFM(Results[1].Rows[i]["累计时间"].ToString());
                            }
                        }
                        
                        #endregion 
                        #region  下井时间不足统计
                        if (VIEW == EnumDataType.XJSJBZTG)
                        {
                            for (int i = 0; i < Results[1].Rows.Count; i++)
                            {
                                Results[1].Rows[i]["Ori"] = ReturnTSFM((Convert.ToInt32(Results[1].Rows[i]["Ori"]) * 60).ToString());
                                Results[1].Rows[i]["Total"] = ReturnTSFM((Convert.ToInt32(Results[1].Rows[i]["Total"]) * 60).ToString());
                                Results[1].Rows[i]["Before"] = ReturnTSFM((Convert.ToInt32(Results[1].Rows[i]["Before"]) * 60).ToString());
                            }

                        }
                        #endregion 
                    
                        json = JsonConvert.SerializeObject(Results[1], Formatting.Indented, timeConverter);
                    }
                    else return "{\"total\": " + "0" + ",\"rows\":" + "[]" + "}";
                }
             
                //if (Result.Rows.Count == 0 && Results != null && Results.Count > 1)
                //{
                //    Result = Results[1];
                //    TotalRows = int.Parse(Results[0].Rows[0][0].ToString());
                //}
              //datagrid的格式是 total:  rows
                return "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";
                // return "{\"totalrows\": " + TotalRows + ",\"data\":" + json + "}";   //这是datagrid中的
                //return JsonConvert.SerializeObject(Result);
            }
        }
        //将 时间长度（持续时间）转化为“天时分秒”格式
        public string ReturnTSFM(string Continuous)
        {
            if (!string.IsNullOrEmpty(Continuous))
            {
                TimeSpan ts = new TimeSpan(0,0,Convert.ToInt32(Continuous));
                return ts.Days + "天" + ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
            }
            else return "0天0时0分0秒";
        
        }

        #region test
        public string treetest()
        {
            DataTable dt = new DataTable();
            dt.TableName = "Parent";
            dt.Columns.Add("id");
            dt.Columns.Add("text");
            dt.Columns.Add("state");



            for (int i = 0; i < 10; i++)
            {
                DataRow dr0 = dt.NewRow();
                dr0[0] = "1";
                dr0[1] = "a";
                dr0[2] = "closed";
                dt.Rows.Add(dr0);
            }

            //DataRow dr0 = dt.NewRow();
            //dr0[0] = "1";
            //dr0[1] = "a";
            //dt.Rows.Add(dr0);
            //DataRow dr1 = dt.NewRow();
            //dr1[0] = "2";
            //dr1[1] = "b";
            //dt.Rows.Add(dr1);
            //StringBuilder  json = new StringBuilder();
            //json.Append("[");
            //foreach (DataRow dr in dt.Rows)
            //{

            //    json.Append("{\"id\":" + dr["id"].ToString());
            //    json.Append(",\"text\":\"" + dr["name"].ToString() + "\"");
            //    json.Append(",\"state\":\"closed\"");

            //    DataTable dtChildren = new DataTable();
            //    dtChildren.Columns.Add("id");
            //    dtChildren.Columns.Add("name");
            //    DataRow dr2=dtChildren.NewRow();
            //    dr2[0] = 3;
            //    dr2[1] = "Son";
            //    dtChildren.Rows.Add(dr2);
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        json.Append(",\"children\":[");
            //        json.Append(DataTable2Json(dtChildren, Convert.ToInt32(dr["id"])));
            //        json.Append("]");
            //    }
            //    json.Append("},");

            //}
            //if (dt.Rows.Count > 0)
            //{
            //    json.Remove(json.Length - 1, 1);
            //}
            //json.Append("]");
            //return json.ToString();  

            return JsonConvert.SerializeObject(dt);
        }

        public string DataTable2Json(DataTable dt, int a)
        {
            StringBuilder json = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                json.Append("{\"id\":" + dr["id"].ToString());
                json.Append(",\"text\":\"" + dr["name"].ToString() + "\"");
                json.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                json.Remove(json.Length - 1, 1);
            }
            return json.ToString();
        }
        #endregion

        #region MineTest
        public DataTable minetest()
        {
            DataTable Result = new DataTable();
            Result.Columns.Add("MineCode");
            Result.Columns.Add("MineName");
            for (int i = 0; i < 10; i++)
            {
                DataRow dr = Result.NewRow();
                dr[0] = (10000 + i).ToString();
                dr[1] = "测试煤矿" + i.ToString();
                Result.Rows.Add(dr);
            }
            return Result;
        }
        #endregion

        #region [ 树 ]

        private string CallTreeGridDataFormat(DataTable dt)
        {
            string json = string.Format("[{0}]", TreeGridDataFormat(dt, dt.Select("PDiskID =0")));
            return json;
        }

        private string TreeGridDataFormat(DataTable dt, DataRow[] drs)
        {
            string toJosn = "";
            for (int i = 0; i < drs.Length; i++)
            {
                if (i != 0) toJosn += ",";
                toJosn += "{";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j != 0) toJosn += ",";

                    string jsonFormat = "\"{0}\":\"{1}\"";
                    if (dt.Columns[j].DataType == typeof(int) && !(dt.Rows[i][j] is DBNull))
                    {
                        jsonFormat = "\"{0}\":{1}";
                    }
                    toJosn += string.Format(jsonFormat, dt.Columns[j].ColumnName, drs[i][j].ToString());
                }

                // 调用下一层
                #region [class2]

                string parentMenuID = drs[i]["Disk_ID"].ToString();
                string josnChird = TreeGridDataFormat(dt, dt.Select("PDiskID='" + parentMenuID + "'"));
                toJosn += string.Format(" ,\"children\": [{0}]", josnChird);

                #endregion

                toJosn += "}";
            }


            return toJosn;
        }

        #endregion


        #region 枚举对象
        public enum EnumDataType
        {
            #region 公告管理
            NoticeList,     // 公告列表
            NoticeDelete,   // 公告删除
            #endregion

            #region 预警管理
            WarnMineName,   // 预警煤矿名称
            WarnDevType,    // 预警设备名称
            WarnList,       // 预警列表
            WarnSave,       // 保存预警信息
            WarnToHis,      // 预警放入历史
            WarnAlarmHis,   // 历史预警
            WarnAlarmTotal, // 预警统计
            #endregion

            #region 网络硬盘
            DiskSaveUsers,   //保存权限
            DiskViewUsers,  //查看权限
            DiskReName, //重命名
            RemoveDisk, //删除当前目录
            RemoveFiles,    //删除文件
            DiskTree,   //网络硬盘树
            FileList,   //文件列表
            #endregion

            #region[人员]
            XJSJBZTG,//下井时间不足
            RYXJTG,//人员下井统计
            ReportRYCYBB,//  超员报表
            ReportRYCSBB,//超时报表
            XZQYRYTG,//限制区域人员统计
            RYSSTG,//实时统计
            RYSS,//人员实时
            RYBJTG,//人员报警统计
            RYStation,
            AQDD,
            RYAreaInfo,
            RYXX,
            PreRoute,
            RealInPeople,
            RealCS,
            RealCY,
            RealXZ,
            RealTZYC,
            RYPathInfo,
            RealTXState,
            HisTXState,
            HisTrack,
            HisTrackInfo,
            QYFZQuery,
            HisCS,
            HisXZ,
            HisCY,
            HisTZYC,
            Report,
            RatedNumber,
            Duty,
            Department,
            #endregion
            #region 安全监控
            Filter,
            MineName,
            Sensor,
            DevType,
            RealData,
            Mine,
            Aqss,
            AQGZ,
            PointSet,
            MineInfoData,
            MineState,
            MineStateHis,
            ChildSystemSet,
            AQKD,
            AQBJ,
            AQMT,
            AQMCHis,
            AQBJHis,
            AQDDHis,
            AQKDHis,
            AQGZHis,
            TypeKind,//量程类型
            BJTG,//报警统计
            GZTG,//故障统计
            AQKGL_Day,//每天开关量
            AQKGL_Week,//七天开关量
            AQMNL_1M,//模拟量每分钟数据显示
            AQMNL_3M,//模拟量3分钟数据显示
            AQMNL_5M,//模拟量5分钟数据显示
            AQMNL_1D,//模拟量每天数据显示
            AQMNL_30D,//模拟量每月数据显示

            ReportMNLYKD,//模拟量月馈电异常
            ReportMNLRKD,//模拟量日馈电异常

            ReportMNLRDD,//模拟量日断电异常
            ReportMNLYDD,//模拟量月断电异常

            ReportMNLRBJ,//模拟量日报警异常
            ReportMNLYBJ,//模拟量日报警异常

            ReportKGLYKD,//开关量月馈电异常
            ReportKGLRKD,//开关量日馈电异常

            ReportKGLRDD,//开关量日断电异常
            ReportKGLYDD,//开关量月断电异常

            ReportSBGZR,//设备故障日报表
            ReportSBGZY,//设备故障月报表

            ReportRYCSBB_R,//人员管理 日超时报表
            ReportRYCSBB_Y,//人员管理 月超时报表
            ReportRYCSBB_B,//人员管理 班超时报表

            ReportRYCYBB_R,//人员管理 日超员报表
            ReportRYCYBB_Y,//人员管理 月超员报表

            ReportRYTXYCBB,//人员管理 通讯异常报表
            ReportRYSXJBB_R,//人员管理 日上下井报表
            ReportRYSXJBB_Y,//人员管理 月上下井报表
            ReportRYSXJBB_B,//人员管理 班上下井报表
            ReportRYGBLDXJBB_R,//人员管理 跟班领导下井报表
            ReportRYGBLDXJBB_Y,//人员管理 跟班领导下井报表
            ReportRYGBLDXJBB_B,//人员管理 跟班领导下井报表


            PreAlarm, //预警
            PreAlarm_His,
            PreAlarm_TG,//预警统计

            TXZD,//通讯状态
            TXZDHis,// 历史通讯中断
            TXYCHis,//历史通讯异常
            TXZDTG,//历史 通讯中断统计
            TXYCTG,//历史通讯异常统计
            MNLTG,//模拟量统计
            #endregion
            #region 安全监控(报表)
            MineStateHisBB,
            #endregion

            HistLine,
            PointType,
            TreeTest,
            MineTest,
            ShowAQJKRTData,
            #region 【矿压】
            RealData_KY,
            AQGZ_KY,
            AQBJ_KY,
            AQMNL_1M_KY,
            AQGZHis_KY,
           AQBJHis_KY,
            #endregion
            #region 【火灾束管】
            RealData_HG,
            AQGZ_HG,
            AQBJ_HG,
            AQMNL_1M_HG,
            AQGZHis_HG,
           AQBJHis_HG,
            #endregion

            GBHZ,
            GBSS,
            CKSS,

            DTSRealData,
            TFMNLHis,//通风系统  模拟量历史数据
            TFST,//通风系统
            TFKGLHis,//通风系统 开关量历史数据
            TFGZHis,//通风系统 历史故障表
            TFGZTG,//通风故障统计
            TFGZ,//通风实时故障
            TFDX,//皮带运输

            PDGZ,//皮带实时故障
            PDST,//皮带运输
            PDGZTG,//皮带故障统计
            PDDX,//皮带断线
            PDMNLHis,//皮带模拟量
            PDKGLHis,//皮带开关量
            PDGZHis,// 历史故障表

            GDGZ,//皮带实时故障
            GDST,//皮带运输
            GDGZTG,//皮带故障统计
            GDDX,//皮带断线
            GDMNLHis,//皮带模拟量
            GDKGLHis,//皮带开关量
            GDGZHis,// 历史故障表

            WSGZ,//皮带实时故障
            WSCF,//皮带运输
            WSGZTG,//皮带故障统计
            WSDX,//皮带断线
            WSMNLHis,//皮带模拟量
            WSKGLHis,//皮带开关量
            WSGZHis,// 历史故障表

            YFGZ,//皮带实时故障
            YFST,//皮带运输
            YFGZTG,//皮带故障统计
            YFDX,//皮带断线
            YFMNLHis,//皮带模拟量
            YFKGLHis,//皮带开关量
            YFGZHis,// 历史故障表

            SPGZ,//皮带实时故障
            SPST,//皮带运输
            SPGZTG,//皮带故障统计
            SPDX,//皮带断线
            SPMNLHis,//皮带模拟量
            SPKGLHis,//皮带开关量
            SPGZHis,// 历史故障表

            OPCState,//OPC状态
            YFMNLFZHis,//模拟量
            TFMNLFZHis,//模拟量
            GDMNLFZHis,//模拟量
            WSMNLFZHis,//模拟量
            SPMNLFZHis,//模拟量
            PDMNLFZHis,//模拟量
            ChildSysTypeKind,
            #region 瓦斯抽放
             RealData_WS,
            #endregion

            RealData_JSG8
        }
        #endregion
    }
}