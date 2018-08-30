using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InternetDataMine.Models.DataService
{
    public class ReportDataBLL
    {
        ReportDataDAL reportdall = new ReportDataDAL();

        string conn = "";
        public ReportDataBLL()
        {
            ReadConfig read = new ReadConfig();
            conn = read.GetSQLConnection();
        }

        /// <summary>
        /// 获取所有报表非固定格式报表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetReportList()
        {
            DataTable dt = reportdall.GetReportList();
            return dt;
        }
        /// <summary>
        /// 获取通讯状态统计报表信息
        /// </summary>
        /// <param name="MineCode"></param>
        /// <returns></returns>
        //public string GetMineBB(string MineCode)
        //{
        //    return reportdall.QueryInfo("  ReportName= '" + MineCode + "'");
        //}
        /// <summary>
        /// 获取所有报表
        /// </summary>
        /// <returns></returns>
        public DataTable GetFixedReport()
        {
            DataTable dt = reportdall.GetFixedReport();
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("SystemName");
                foreach (DataRow dr in dt.Rows)
                {//报表所属系统类型,0:固定格式报表;1:瓦斯监控;2:人员管理;3:瓦斯抽放;4:瓦斯监控包含抽放
                    int typeid = Convert.ToInt32(dr["SystemType"]);
                    string sysname = "";
                    if (typeid == 0)
                    {
                        sysname = "固定格式报表";
                    }
                    else if (typeid == 1)
                    {
                        sysname = "瓦斯监控报表";
                    }
                    else if (typeid == 2)
                    {
                        sysname = "人员管理报表";
                    }
                    else if (typeid == 3)
                    {
                        sysname = ":瓦斯抽放报表";
                    }
                    else if (typeid == 4)
                    {
                        sysname = "瓦斯监控及抽放报表";
                    }
                    dr["SystemName"] = sysname;
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取所有报表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllReportList()
        {
            DataTable dt = reportdall.GetAllReportList();
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("SystemName");
                foreach (DataRow dr in dt.Rows)
                {//报表所属系统类型,0:固定格式报表;1:瓦斯监控;2:人员管理;3:瓦斯抽放;4:瓦斯监控包含抽放
                    int typeid = Convert.ToInt32(dr["SystemType"]);
                    string sysname = "";
                    if (typeid == 0)
                    {
                        sysname = "固定格式报表";
                    }
                    else if (typeid == 1)
                    {
                        sysname = "瓦斯监控报表";
                    }
                    else if (typeid == 2)
                    {
                        sysname = "人员管理报表";
                    }
                    else if (typeid == 3)
                    {
                        sysname = ":瓦斯抽放报表";
                    }
                    else if (typeid == 4)
                    {
                        sysname = "瓦斯监控及抽放报表";
                    }
                    dr["SystemName"] = sysname;
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取指定报表类型信息
        /// </summary>
        /// <param name="reportname"></param>
        /// <returns></returns>
        public DataTable GetReportList(string reportname)
        {
            try
            {
                DataTable dt = reportdall.GetReportList(reportname);
                return dt;
            }
            catch { return null; }
        }


        /// <summary>
        /// 获取指定报表类型的数据
        /// </summary>
        /// <param name="reportname">报表名称</param>
        /// <returns></returns>
        public DataTable GetReportData(string reportname)
        {
            try
            {
                DataTable dt = reportdall.GetReportList(reportname);
                if (dt.Rows.Count > 0)
                {
                    string dis = dt.Rows[0]["DispName"].ToString();
                    dt = reportdall.GetReportData(dis);
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }

        /// <summary>
        /// 查询指定报表类型指定时间范围内的数据
        /// </summary>
        /// <param name="reportname">报表类型</param>
        /// <param name="begintime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns>
        public DataTable GetReportData(string reportname, DateTime begintime, DateTime endtime)
        {
            try
            {
                string where = "SubmitTime>='" + begintime + "' and SubmitTime<'" + endtime + "'";
                DataTable dt = reportdall.GetReportData(reportname, where);
                return dt;
            }
            catch { return null; }
        }
        /// <summary>
        /// 超员报表 2017-06-20新增
        /// </summary>
        /// <param name="MineCode"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public DataTable GetReportRYCYBB(string MineCode, DateTime BeginTime, DateTime EndTime, int flag)
        {
            string sql1 = "select mc.SimpleName , cy.MineCode,cy.AreaName,cy.AreaCode,max(cy.Number) Number, " +
                          "  max( isnull(cy.SUM,0) -isnull(cy.Number,0)) Counts,MAX(T1.InAreaTime) MaxTime, " +
                          "  cy.StartAlTime,cy.EndAlTime,DATEDIFF(s, cy.StartAlTime, cy.EndAlTime)Continuous_Tmp,CAST(NULL AS VARCHAR(50)) Continuous ,cast(null as varchar(5)) CYL" +

                          "  from ShineView_His.dbo.RYCYXZH  cy " +
                          "  left join ShineView_Data.dbo.MineConfig mc " +
                          "  on mc.MineCode = cy.MineCode  " +
                          "  left JOIN " +
                          "  ( " +
                          "  select * From (   " +
                          "   select minecode ,startaltime,endaltime,number,sum,InAreaTime,areacode ,row_number() over(partition by minecode ,areacode ,startaltime, endaltime order by minecode  desc) rn   " +
                          "   From  ShineView_His.dbo.RYCYXZH   " +
                          "  ) t where t.rn <=1 " +
                           "  ) AS T1 " +
                         "  on t1.minecode =cy.minecode and t1.startaltime=cy.startaltime and t1.endaltime =cy.endaltime and t1.areacode =cy.areacode and t1.number =cy.number where 1=" + flag;


            string sql2 = "  GROUP BY mc.SimpleName , cy.MineCode,cy.AreaName,cy.AreaCode,cy.Number,cy.StartAlTime,cy.EndAlTime,T1.InAreaTime";
           if (!string.IsNullOrEmpty(MineCode))
           {
               sql1 += " and cy.MineCode ='" + MineCode + "'";
           }
           sql1 += " and cy.startaltime>='" + BeginTime + "'";
           sql1 += " and cy.startaltime<='" + EndTime + "'";
           string whereData = sql1 + sql2;
           return SQLDataServer.ToDataTable(whereData, conn);
        
        }
        /// <summary>
        /// 超时报表 2017-06-20新增
        /// </summary>
        /// <param name="MineCode"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="flag"></param>
        /// <param name="TimeSpan"></param>
        /// <returns></returns>
        public DataTable GetReportRYCSBB(string MineCode, DateTime BeginTime, DateTime EndTime, int flag, string TimeSpan)
        {
            string sql1 = "select SimpleName ,MineCode,JobCardCode,Name,Position,Department,sum( Continuous)  Continuous_Tmp, " +
                              "  max( MaxTime) MaxTime_Tmp ,min(MinTime ) MinTime_Tmp,sum( Counts ) Counts,cast(null as varchar(50)) Continuous, " +
                              "  cast(null as varchar(50)) MaxTime,cast(null as varchar(50)) MinTime" +
                              "  from  " +
                              "  ( " +

                              "  select mc.SimpleName,cs.MineCode ,cs.JobCardCode ,cs.Name,cs.Position , cs.Department, " +
                              "  Sum(DATEDIFF(s, StartAlTime, EndAlTime)) Continuous,COUNT(cs.MineCode) Counts,max(DATEDIFF(s, StartAlTime, EndAlTime)) MaxTime, " +
                             "  min(DATEDIFF(s, StartAlTime, EndAlTime)) MinTime " +
                              "  from    ShineView_His.dbo.RYCSH cs  " +
                              "  left join ShineView_Data.dbo.MineConfig mc " +
                              "  on cs.MineCode =mc.MineCode  where 1="+flag;
            string sql2 = "  GROUP BY mc.SimpleName ,cs.MineCode ,JobCardCode,Name, Position,Department " +

                   "  union ALL " +

                   "  select mc.SimpleName,cs.MineCode ,cs.JobCardCode ,xx.Name,xx.Position , xx.Department, " +
                   "  Sum(DATEDIFF(s, StartAlTime, EndAlTime)) Continuous,COUNT(cs.MineCode) Counts,max(DATEDIFF(s, StartAlTime, EndAlTime)) MaxTime, " +
                   "  min(DATEDIFF(s, StartAlTime, EndAlTime)) MinTime " +
                   "  from    ShineView_Data.dbo.RYCS cs " +
                   "  left join ShineView_Data.dbo.MineConfig mc " +
                   "  on cs.MineCode =mc.MineCode  " +
                   "  left join ShineView_Data.dbo.RYXX xx " +
                   "  on xx.MineCode =cs.MineCode  and cs.JobCardCode=xx.JobCardCode  where EndAlTime   NOT  LIKE 'x%' AND  EndAlTime  NOT LIKE 'X%' and  1=" + flag;
          string sql3=   "  GROUP BY mc.SimpleName ,cs.MineCode ,cs.JobCardCode,xx.Name, xx.Position,xx.Department " +
                              "  ) as a GROUP BY SimpleName,MineCode,JobCardCode,Name,Position,Department";

          if (!string.IsNullOrEmpty(MineCode))
          {
              sql1 += " and mc.MineCode ='" + MineCode + "'";
              sql2 += " and mc.MineCode ='" + MineCode + "'";
          }
          if (!string.IsNullOrEmpty(TimeSpan))
          {
              sql1 += " and  DATEDIFF(s, StartAlTime, EndAlTime)>=" + TimeSpan;
              sql2 += " and  DATEDIFF(s, StartAlTime, EndAlTime)>=" + TimeSpan ;
          }
          sql1 += " and StartAlTime>='" + BeginTime + "'";
          sql2 += " and StartAlTime>='" + BeginTime + "'";
          sql1 += " and StartAlTime<='" + EndTime + "'";
          sql2 += " and StartAlTime<='" + EndTime + "'";
          string whereData = sql1 + sql2 + sql3;
          return SQLDataServer.ToDataTable(whereData, conn);
        }


        /// <summary>
        /// 获取指定条件下的指定类型的报表数据
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <returns></returns>
        public DataTable GetReportData(string reportable, string minecode, string DevType, DateTime begintime, DateTime endtime)
        {
            try
            {
                string where = "SubmitTime>='" + begintime + "' and SubmitTime<'" + endtime + "'";
                if (minecode != "" && minecode != null)
                {
                    where += " and 煤矿编号='" + minecode + "'";
                }
                if (DevType != null && DevType != "")
                {
                    where += " and 设备名='" + DevType + "'";
                }
                DataTable dt = reportdall.GetReportData(reportable, where);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 加载模拟量日、月馈电异常数据
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_KD(string minecode, string devname, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW)
        {
            //string type = "A";
            //int ts = 6;
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportMNLRKD || VIEW == TransJsonToTreeListModel.EnumDataType.ReportKGLRKD)
            //    ts = 8;
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportKGLRKD || VIEW == TransJsonToTreeListModel.EnumDataType.ReportKGLYKD)
            //    type = "D";

            string where = "SubmitTime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and SubmitTime<'" +
                           EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (minecode != "" && minecode != null)
            {
                where += " and 煤矿编号='" + minecode + "'";
            }
            if (devname != null && devname != "")
            {
                where += " and 设备名='" + devname + "'";
            }
            where += " and 累计次数 IS NOT NULL";

            string sql = string.Format(@"SELECT 煤矿编号, 煤矿名称,测点编号,设备名,安装位置,单位,断电区域,count(累计次数) 累计次数,[ShineView_Data].dbo.FunConvertTime(sum(累计时间)) 累计时间 FROM ShineView_His.dbo.Report_FeedBack
  WHERE {0}
  GROUP BY 煤矿编号, 煤矿名称,测点编号,设备名,安装位置,单位,断电区域", where);
            //            var sql =
            //                string.Format(
            //                    @"Select 煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,断电区域,单位,Convert(varchar({0}),SubmitTime,112) TheDate,Sum(累计时间) 累计时间,Sum(累计次数) 累计次数 From 
            //(Select * From ShineView_His.dbo.Report_FeedBack T1 Left Join [ShineView_Data].[dbo].[DeviceType] T2 On T1.设备名=T2.TypeName ) T100 Where {1}
            // Group By  煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,断电区域,单位,Convert(varchar({0}),SubmitTime,112)", ts, where);

            return SQLDataServer.ToDataTable(sql, conn);
        }

        /// <summary>
        /// 加载模拟量开关量日、月断电异常数据
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_DD(string minecode, string devname, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW)
        {
        
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportMNLRDD || VIEW == TransJsonToTreeListModel.EnumDataType.ReportKGLRDD)
            //    ts = 8;
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportKGLRDD || VIEW == TransJsonToTreeListModel.EnumDataType.ReportKGLYDD)
            //    type = "D";

            string where = "SubmitTime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and SubmitTime<'" +
                           EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (minecode != "" && minecode != null)
            {
                where += " and 煤矿编号='" + minecode + "'";
            }
            if (devname != null && devname != "")
            {
                where += " and 设备名='" + devname + "'";
            }
            where += " and 断电次数 is not null";

            string sql = string.Format(@"select 煤矿编号, 煤矿名称,测点编号,设备名,安装位置,单位, 断电值,复电值,max(最大值) 最大值 ,最大值时间,count(*) 断电次数,shineview_data.dbo.FunConvertTime (sum(累计时间)) 断电时间 
from ShineView_His.dbo.Report_Power where {0} group by 煤矿编号, 煤矿名称,测点编号,设备名,安装位置,单位, 断电值,复电值,最大值时间", where);

            //            var sql = string.Format(
            //                @"Select T.*,TT.本次最大值时间 From (
            //Select 煤矿编号 ,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,单位 ,断电范围 ,Sum(IsNull(断电次数, 0)) 断电次数 ,Sum(IsNull(累计时间, 0)) 累计时间 ,Max(IsNull(最大值, 0)) 最大值 ,Avg(IsNull(本次平均值, 0)) 平均值
            // ,Convert(varchar({0}),SubmitTime,112) TheDate From (
            //Select * From ShineView_His.dbo.Report_Power T1 Left Join [ShineView_Data].[dbo].[DeviceType] T2 On T1.设备名=T2.TypeName ) T100 Where {1}
            // Group By  煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,断电范围,单位,Convert(varchar({0}),SubmitTime,112)
            // ) T Left Join ShineView_His.dbo.Report_Power TT On T.最大值=TT.本次最大值", ts, where);

            return SQLDataServer.ToDataTable(sql, conn);
        }

        /// <summary>
        /// 加载模拟量日、月报警异常数据
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_BJ(string minecode, string devname, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW, string TimeSpan, string TypeKind)
        {
            #region 原先
            //            //int ts = 6;
//            //if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportMNLRKD)
//            //    ts = 8;
//            string where = "SubmitTime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and SubmitTime<'" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
//            if (minecode != "" && minecode != null)
//            {
//                where += " and 煤矿编号='" + minecode + "'";
//            }
//            if (devname != null && devname != "")
//            {
//                where += " and 设备名='" + devname + "'";
//            }
//            where += " and 报警次数 is not null";

//            //            var sql = string.Format(
//            //                @"Select T.*,TT.本次最大值时间 From (
//            //Select 煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,单位,Convert(varchar({0}),SubmitTime,112) TheDate,dbo.FunConvertTime(Sum(累计时间)) 累计时间,Sum(报警次数) 报警次数,Avg(本次平均值) 平均值,
//            //Max(本次最大值) 最大值
//            // From (Select * From ShineView_His.dbo.Report_Alarm T1 Left Join [ShineView_Data].[dbo].[DeviceType] T2 On T1.设备名=T2.TypeName ) T100 Where {1} And T100.Type='A'
//            //Group By  煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,单位,Convert(varchar({0}),SubmitTime,112)
//            // ) T Left Join ShineView_His.dbo.Report_Power TT On T.最大值=TT.本次最大值", ts, where);

//            string sql = string.Format(@"SELECT 煤矿名称,测点编号,设备名,安装位置,单位,报警值,解报值,报警次数,shineview_data.dbo.FunConvertTime(累计时间) 累计时间,最大值,最大值时间,SubmitTime FROM shineview_his.dbo.Report_Alarm 
// WHERE {0}
            // GROUP BY 煤矿名称,测点编号,设备名,安装位置,单位,报警值,解报值,报警次数,累计时间,最大值,最大值时间,SubmitTime", where);
            #endregion

            string sql = "select bj.MineCode , mc.SimpleName 煤矿名称,bj.SensorNum 测点编号,d.TypeName 设备名,bj.Place 安装位置,bj.PoliceDatetime 开始时间,bj.PoliceEndDatetime 结束时间," +
                       "  case d.Type when 'A' then '模拟量' when 'C' then '控制量' when 'D' THEN '开关量' when 'F' then '分站/设备'  when 'L' then '累积量' when 'U' then '抽象出的逻辑量' else '其他' end 量程类型," +
                       "  DATEDIFF(s, PoliceDatetime, PoliceEndDatetime) Continuous ,cast(null as VARCHAR(50)) 持续时间," +
                       "  case bj.PoliceType when 0 then '上限报警' when 1 then '下限报警' when 2 then '开关量报警'  else '其他' end 报警类型 " +
                       "  from ShineView_His.dbo.AQBJ bj " +
                       "  left JOIN MineConfig mc " +
                       "  on mc.MineCode=bj.MineCode " +
                       "  left join DeviceType d " +
                       "  on d.TypeCode = bj.Type where 1=1 ";
            if (!string.IsNullOrEmpty(minecode))
            {
                sql += " and bj.MineCode ='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(devname))
            {
                sql += " and d.TypeName ='" + devname + "'";
            }
            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                sql += " and PoliceDatetime>='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                sql += " and PoliceEndDatetime<='" + EndTime + "'";
            }
            if (!string.IsNullOrEmpty(TimeSpan))
            {
                sql += " and  DATEDIFF(s, PoliceDatetime, PoliceEndDatetime)>=" + TimeSpan ;
            }
            if (!string.IsNullOrEmpty(TypeKind))
            {
                sql += " and d.Type='" + TypeKind + "'";
            }

            return SQLDataServer.ToDataTable(sql, conn);
        }

        /// <summary>
        /// 加载日、月设备故障数据
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_SBGZ(string minecode, string devname, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW,string TimeSpan)
        {
               #region 原先
                    //之前就注释的
                //    int ts = 6;
                //    if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportSBGZR)
                //        ts = 8;

                         //之前的
                    //string where = "HitchDatetime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and HitchDatetime<'" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    //if (minecode != "" && minecode != null)
                    //{
                    //    where += " and 煤矿编号='" + minecode + "'";
                    //}
                    //if (devname != null && devname != "")
                    //{
                    //    where += " and 设备名='" + devname + "'";
                    //}

                    //where += "";
       
                        //            string sql = string.Format(
                        //@"SELECT gz.mineCode 煤矿编号,MC.SimpleName 煤矿名称, sensorNum 测点编号,DT.TypeName 设备名,place 安装位置,累计次数,shineview_data.dbo.FunConvertTime(累计时间) 累计时间  FROM 
                        //(SELECT mineCode,sensorNum,type,place,Count(*) 累计次数,Sum(DateDiff(SS,HitchDatetime,HitchEndDatetime)) 累计时间 FROM ShineView_His.dbo.AQGZ 
                        //WHERE {0}
                        //GROUP BY mineCode,sensorNum,type,place
                        //) GZ
                        //LEFT JOIN [ShineView_Data].[dbo].[DeviceType] DT ON GZ.Type=DT.TypeCode
                        //LEFT JOIN [ShineView_Data].[dbo].[MineConfig] MC ON GZ.mineCode=MC.MineCode", where);
          
                        //之前就注释的
                    //            var sql = 
                    //            string.Format(
                    //                @"Select 煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置,dbo.FunConvertTime(Sum(累计时间)) 累计时间,Count(*) 累计次数,Convert(varchar({0}),HitchDatetime,112) TheDate From (
                    //SELECT T1.MineCode 煤矿编号,T2.SimpleName 煤矿名称
                    //,SensorNum 测点编号
                    //,Place 安装位置
                    //,T3.TypeName 设备名,
                    //DateDiff(SS,HitchDatetime,HitchEndDatetime) 累计时间,HitchDatetime
                    //FROM ShineView_His.dbo.AQGZ T1 Left Join [ShineView_Data].[dbo].[MineConfig] T2 On T1.MineCode=T2.MineCode
                    //  Left Join [ShineView_Data].[dbo].[DeviceType] T3 On T1.Type=T3.TypeCode
                    //) T Where {1} Group By  煤矿编号,煤矿名称 ,测点编号 ,设备名 ,安装位置 ,Convert(varchar({0}),HitchDatetime,112)", ts, where);
               #endregion
            string sql = "select Row_Number() over (ORDER BY gz.MineCode asc) Tmp_ID, gz.MineCode  ,gz.SensorNum 测点编号,m.simplename 煤矿名称 ,gz.Place  安装位置,d.TypeName 设备名, " +
                   "   gz.HitchDateTime 开始时间,gz.HitchEndDatetime 结束时间,DATEDIFF(s, HitchDateTime, HitchEndDateTime) Continuous,RelevanceDepict 故障类型, " +
                   "   cast (null as varchar(50)) 持续时间, " +
                   "   case  gz.HitchDepict when 0 then '通信故障' when 1 then '设备故障' when 2 then '调试' else  '其他'  end as 故障原因  " +
                   "   from ShineView_His.dbo.AQGZ gz left join ShineView_Data.dbo.MineConfig m on m.minecode = gz.minecode  " +
                   "   left join ShineView_Data.dbo.DeviceType d  " +
                   "   on  gz.Type =d.TypeCode " +
                   "   where  1= 1";
            if (!string.IsNullOrEmpty(minecode))
            {
                sql += " and gz.MineCode ='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(devname))
            {
                sql += " and d.TypeName='" + devname + "'";
            }
            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                sql += " and HitchDateTime>='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                sql += " and HitchDateTime<='" + EndTime + "'";
            }
            if (!string.IsNullOrEmpty(TimeSpan))
            {
                sql += " and DATEDIFF(s, HitchDateTime, HitchEndDateTime) > =" + TimeSpan ;
            }
            return SQLDataServer.ToDataTable(sql, conn);
        }

        /// <summary>
        /// 日、月超时报表
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_RYCSBB(string minecode, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW)
        {
            int ts = 6;
            if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportRYCSBB_R)
                ts = 8;
            string where = "StartAlTime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and StartAlTime<'" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (minecode != "" && minecode != null)
            {
                where += " and MineCode='" + minecode + "'";
            }

            var sql = string.Format(
                @" SELECT [MineCode] ,[SimpleName],[Name],[Position],[Department],[InTime],[StartAlTime],[EndAlTime]
				  ,dbo.FunConvertTime(Datediff(SECOND,[StartAlTime],[EndAlTime])) continuoustime
              FROM ShineView_His.[dbo].[RYCSH] where 1=1 and {1}", ts, where);

            return SQLDataServer.ToDataTable(sql, conn);
        }

        /// <summary>
        /// 日、月上下井报表
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_RYSXJBB(string minecode, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW)
        {
            int ts = 6;
            if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportRYSXJBB_R || VIEW == TransJsonToTreeListModel.EnumDataType.ReportRYGBLDXJBB_R)
                ts = 8;
            var whereLD = " 1=1 ";
            if (VIEW == TransJsonToTreeListModel.EnumDataType.ReportRYGBLDXJBB_R ||
                VIEW == TransJsonToTreeListModel.EnumDataType.ReportRYGBLDXJBB_Y)
                whereLD = "工种或职务 like '%矿领导%'";
            string where = "SubmitTime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and SubmitTime<'" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "' And " + whereLD;
            if (minecode != "" && minecode != null)
            {
                where += " and 煤矿编号='" + minecode + "'";
            }

            var sql = string.Format(
                @"Select 煤矿编号,煤矿名称,姓名,标识卡,性别,工种或职务,部门,dbo.FunConvertTime(Sum(累计时间)) 累计时间,Sum(累计次数) 累计次数,Convert(varchar({0}),SubmitTime,112) TheDate From ShineView_His.dbo.Report_UpDown 
Where {1}
Group By 煤矿编号,煤矿名称,姓名,标识卡,性别,工种或职务,部门,Convert(varchar({0}),SubmitTime,112)", ts, where);

            return SQLDataServer.ToDataTable(sql, conn);
        }

        /// <summary>
        /// 班超时报表
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_RYCSBB_B(string minecode, DateTime BegingTime, DateTime EndTime)
        {
            string where = "InTime>='" + BegingTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and InTime<'" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (minecode != "" && minecode != null)
            {
                where += " and MineCode='" + minecode + "'";
            }

            var sql = string.Format(
                @"Select MineCode,SimpleName,SystemType,Department,Count(1) OverCount,[ShineView_Data].[dbo].[FunConvertTime](Sum(DateDiff(Second,StartAlTime,EndAlTime))) OverTime
,Convert(varchar(8),InTime,112) TheDate
 From ShineView_His.dbo.RYCSH Where {0}
Group BY MineCode,SimpleName,SystemType,Department,Convert(varchar(8),InTime,112)", where);

            return SQLDataServer.ToDataTable(sql, conn);
        }


        /// <summary>
        /// 通讯异常报表
        /// </summary>
        /// <param name="reportable">【必须】报表表名</param>
        /// <param name="minecode">煤矿编号，全部则为空</param>
        /// <param name="begintime">【必选】开始时间</param>
        /// <param name="endtime">【必选】结束时间</param>
        /// <param name="VIEW">区分日报表、月报表</param>
        /// <returns></returns>
        public DataTable GetReportData_RYTXYCBB(string minecode, DateTime BegingTime, DateTime EndTime)
        {

            var sql = string.Format(
                @"Select T1.*,T2.SimpleName,Case When TypeCode=1 Then '安全监控' When TypeCode=2 Then '人员管理' When TypeCode=3 Then '瓦斯抽放' Else  '安全监控+瓦斯抽放' End
 TypeName,[dbo].[FunConvertTime](Continuous) ContinuousTime From [ShineView_His].dbo.BadLog T1 Left Join [ShineView_Data].[dbo].[MineConfig] T2 On T1.MineCode=T2.MineCode
 Where T1.MineCode like '%{0}%' AND LastTime>='{1}' And LastTime<'{2}' ", minecode, BegingTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

            return SQLDataServer.ToDataTable(sql, conn);
        }


        /// <summary>
        /// 获取指定报表数据的列信息
        /// </summary>
        /// <param name="reportname">报表名称</param>
        /// <returns></returns>
        public DataTable GetReportColumns(string reportname)
        {
            try
            {
                DataTable dt = reportdall.GetReportList(reportname);
                string dis = dt.Rows[0]["DispName"].ToString();
                dt = reportdall.GetTableColumns(dis);
                return dt;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 获取指定表的列信息（不包含数据）
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public DataTable GetTableColumns(string tabname)
        {
            try
            {
                DataTable dt = reportdall.GetTableColumns(tabname);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定表中的字段名和字段类型
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <returns></returns>
        public DataTable GetColumns(string tabname)
        {
            try
            {
                DataTable dt = reportdall.GetReportList(tabname);
                string dis = dt.Rows[0]["DispName"].ToString();
                dt = reportdall.GetColumns(dis);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定表指定列的信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DataTable GetTableColumns(string tabname, string columns)
        {
            try
            {
                DataTable dt = reportdall.GetReportList(tabname);
                string dis = dt.Rows[0]["DispName"].ToString();
                dt = reportdall.GetTableColumns(dis, columns);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取报表类型(Report_ 为前缀)的表名
        /// </summary>
        /// <returns></returns>
        public DataTable GetReportTables()
        {
            DataTable dt = reportdall.GetReportTables();
            return dt;
        }

        /// <summary>
        /// 增加报表
        /// </summary>
        /// <param name="reportname">报表名称</param>
        /// <param name="tablename">报表对应的表名</param>
        /// <param name="systemtype">报表所属系统类型</param>
        /// <param name="remark">报表备注</param>
        /// <returns>true 成功,false 失败</returns>
        public bool InsertReport(string reportname, string tablename, int systemtype, string remark)
        {
            return reportdall.InsertReport(reportname, tablename, systemtype, remark);
        }

        /// <summary>
        /// 删除指定编号的报表
        /// </summary>
        /// <param name="ID">编号</param>
        /// <returns>true 成功,false 失败</returns>
        public bool DeleteReport(int ID)
        {
            bool state = reportdall.DeleteReport(ID);
            return state;
        }

        /// <summary>
        /// 删除指定表名的报表
        /// </summary>
        /// <param name="tablename">对应表名</param>
        /// <returns>true 成功,false 失败</returns>
        public bool DeleteReportTable(string tablename)
        {
            bool state = reportdall.DeleteReportTable(tablename);
            return state;
        }
    }
}
