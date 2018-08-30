﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using InternetDataMine.Models;
using System.Reflection;
using InternetDataMine.Controllers;

namespace InternetDataMine.Models.DataService
{
    public class DataBLL
    {
        private int pageIndex;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        private int pageSize;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        public byte[] MyFileContent { get; set; }


        #region [ 公告]

        /// <summary>
        /// 公告列表
        /// </summary>
        /// <returns></returns>
        public DataTable NoticeList()
        {
            string sql = string.Format(@"Select * From [dbo].[NoticeManage]  A 
                    Left Join [FileManage] B on a.Notice_ID = b.Disk_ID order by CreateTime desc");
            return dal.ReturnData(sql);
        }



        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool NoticeDelete(string ids)
        {
            string sql = string.Format(@"delete [dbo].[NoticeManage] Where [Notice_ID] in ({0})
                                delete [dbo].[FileManage] Where Disk_ID in ({0})", ids);
            return dal.ExcuteSql(sql);
        }

        #endregion


        #region [ 网络硬盘 ]

        #region [ 网络硬盘树 ]

        /// <summary>
        /// 网络硬盘树
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable NetDiskTree(string username)
        {
            string sql =
                string.Format(
                    @"Select * From [dbo].[DiskManage] Where (isnull(ViewUsers,',')+CreateUser+',' like '%,{0},%' or ViewUsers like '%全部%' or  ManageUsers like '%{0}%')  order by PDiskID,OrderBy",
                    username);
            return dal.ReturnData(sql);
        }

        #endregion

        #region [ 网络硬盘文件列表 ]

        /// <summary>
        /// 网络硬盘文件列表
        /// </summary>
        /// <returns></returns>
        public DataTable NetDiskFileList(string diskId)
        {
            string sql = string.Format(@"Select * From [dbo].[FileManage] Where Disk_ID='{0}' order by OrderBy", diskId);
            return dal.ReturnData(sql);
        }

        #endregion

        #region [ 删除文件 ]

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool RemoveFiles(string ids)
        {
            string sql = string.Format("delete from FileManage where File_ID in (" + ids + ")");
            return dal.ExcuteSql(sql);
        }
        #endregion

        #region [ 删除目录 ]
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveDisk(string id)
        {
            string sql = string.Format(@"delete [dbo].[FileManage] Where Disk_ID in (
select Disk_ID from [dbo].[DiskManage] Where Disk_ID ='{0}' Or PDiskID ='{0}')
delete [dbo].[DiskManage] Where Disk_ID ='{0}' Or PDiskID ='{0}'", id);
            return dal.ExcuteSql(sql);
        }
        #endregion

        #region [ 重命名 ]

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="diskName">名称</param>
        /// <param name="pid">父ID</param>
        /// <param name="userame">当前用户名</param>
        /// <param name="orderby">排序字段</param>
        /// <returns></returns>
        public bool DiskReName(string id, string diskName, string pid, string userame, string orderby)
        {
            string sql = string.Format("Update [DiskManage] Set DiskName ='{1}',Orderby={2} Where Disk_ID='{0}'", id, diskName, orderby);
            if (id == "")
            {
                sql = string.Format("Insert [DiskManage](Disk_ID,pdiskid,diskName,CreateUser,OrderBy) Values('{4}','{0}','{1}','{2}',{3})", pid, diskName, userame, orderby, Guid.NewGuid());
            }
            return dal.ExcuteSql(sql);
        }

        #endregion

        /// <summary>
        /// 查看权限
        /// </summary>
        /// <returns></returns>
        public DataTable DiskViewUsers()
        {
            string sql = string.Format(@"Select UserID, UserName From [dbo].[UsersInfo]");
            return dal.ReturnData(sql);
        }

        #region [ 保存权限 ]

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="diskid">编号</param>
        /// <param name="ViewUsers">查看权限</param>
        /// <param name="ManageUsers">管理权限</param>
        /// <returns></returns>
        public bool DiskSaveUsers(string diskid, string ViewUsers, string ManageUsers)
        {
            ManageUsers = string.IsNullOrEmpty(ManageUsers) ? "" : ("," + ManageUsers + ",");
            ViewUsers = string.IsNullOrEmpty(ViewUsers) ? "" : ("," + ViewUsers + ",");
            string sql = string.Format(
                "Update [DiskManage] Set ManageUsers='{0}',ViewUsers='{1}' Where Disk_ID ='{2}'", ManageUsers, ViewUsers,
                diskid);
            return dal.ExcuteSql(sql);
        }
        #endregion

        #endregion

        #region [ 预警 ]

        #region [ 预警列表 ]

        /// <summary>
        /// 预警列表
        /// </summary>
        /// <param name="MineCode"></param>
        /// <param name="SensorNum"></param>
        /// <param name="DropName"></param>
        /// <returns></returns>
        public DataTable WarmList(string MineCode, string SensorNum, string DropName)
        {
            string where = " wa.level<>5 ";
            if (MineCode != "" && MineCode != "全部")
            {
                //wa.MineCode ='123456789012345' and wa.sensorNum ='001A012' and wa.[level]=1
                where += string.Format(" and wa.MineCode ='{0}'", MineCode);
            }

            if (SensorNum != "" && SensorNum != "全部")
            {
                where += string.Format(" and wa.sensorNum ='{0}'", SensorNum);
            }

            if (DropName != "" && DropName != "全部" && DropName != "-1")
            {
                where += string.Format(" and aqss.ValueState ='{0}'", DropName);
            }

            string sql = @" select wa.ID,chulresult,chulway,jlpers,mc.MineCode,mc.SimpleName,TypeName,AQMC.Place,wa.level,case when AQSS.ShowValue is null then '0.0' else AQSS.ShowValue end as ShowValue,case when wa.MaxValue is null then 0.0 else  wa.MaxValue end as  PoliceMaxValue,
              case when wa.MinValue is null then 0.0 else wa.MinValue end as PoliceMin,wa.[Datetime],dt.unit, case wa.ValueState when 0 then '正常' When 1 Then '报警' when 2 then '复电' 
                When 4 Then '断电报警' When 8 Then '故障报警' When 16 Then '馈电异常' Else '工作异常' end as valueState,wa.starttime,
                wa.overtime, dbo.FunConvertTime(wa.lasttime) lasttimeLength, case wa.ChulStatus when 0 then '未处理' else '正在处理' end ChulState
                from warnalarm wa 
                left join AQSS on wa.MineCode=aqss.MineCode and wa.SensorNum=aqss.SensorNum 
                left join (
                select MineCode,SensorNum,Place,[Type] from aqmc
                union all
                select MineCode,SensorNum,Place,[Type] from AQKC
                ) aqmc on wa.MineCode=aqmc.MineCode and wa.SensorNum=AQMC.SensorNum 
                left join DeviceType dt on AQMC.Type=dt.TypeCode 
                left join MineConfig mc on wa.MineCode=mc.MineCode Where  " + where;
            return dal.ReturnData(sql);
        }

        #endregion

        #region [ 填写预警信息 ]

        /// <summary>
        /// 填写预警信息
        /// </summary>
        /// <param name="chulway"></param>
        /// <param name="chulresult"></param>
        /// <param name="jlpers"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool WarmSave(string chulway, string chulresult, string jlpers, string id)
        {
            string sql =
                string.Format(
                    "Update WarnAlarm Set chulway ='{0}',chulresult='{1}',jlpers='{2}',jltime =GETDATE(),ChulStatus=1 Where id ={3}",
                    chulway, chulresult, jlpers, id);
            return dal.ExcuteSql(sql);
        }

        #endregion

        #region [ 放入历史 ]

        /// <summary>
        /// 放入历史
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public bool WarmToHis(string id)
        {
            string sql = string.Format(@"Insert shineview_his.dbo.[WarnAlarm_His]
                    select mc.MineCode,AQMC.Place,TypeName,AQSS.ShowValue,Aqss.PoliceMaxValue,
                    AQSS.PoliceMin,dt.unit, case aqss.ValueState when 0 then '正常' When 1 Then '报警' when 2 then '复电' 
                    When 4 Then '断电报警' When 8 Then '故障报警' When 16 Then '馈电异常' Else '工作异常' end as valueState,wa.starttime,
                    wa.overtime,wa.lasttime,wa.level,wa.SensorNum,mc.SimpleName,'',fenxi,chulway,chulresult,jlpers,jltime,wa.[Datetime]
                    from warnalarm wa 
                    left join AQSS on wa.MineCode=aqss.MineCode and wa.SensorNum=aqss.SensorNum 
                    left join (
                    select MineCode,SensorNum,Place,[Type] from aqmc
                    union all
                    select MineCode,SensorNum,Place,[Type] from AQKC
                    ) aqmc on wa.MineCode=aqmc.MineCode and wa.SensorNum=AQMC.SensorNum 
                    left join DeviceType dt on AQMC.Type=dt.TypeCode 
                    left join MineConfig mc on wa.MineCode=mc.MineCode Where wa.id ={0} delete [warnalarm] Where id ={0}", id);
            return dal.ExcuteSql(sql);
        }

        #endregion

        #region [ 历史预警 ]

        /// <summary>
        /// 历史预警
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="sensorNum">设备编号</param>
        /// <param name="alarmType">报警类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public DataTable WarnAlarmHis(string mineCode, string sensorNum, string alarmType, string startTime, string endTime)
        {
            string[,] strarr = new string[5, 5]
                {
                    {"MineCode", "=", mineCode, "string", "1"},
                    {"SensorNum", "=", sensorNum, "string", "1"},
                    {"status", "=", alarmType, "string", "1"},
                    {"jltime", ">=", startTime, "string", "1"},
                    {"jltime", "<", endTime, "string", "1"}
                };

            string strWhere = GetSelectString(strarr);
            string sql =
                string.Format(@"
                        select mineName,SensorNum,place,type,Value,unit,status,starttime,overtime,dbo.FunConvertTime(lasttime) 
                        lasttime,level,fenxi,chulway,chulresult,jlpers,jltime,Datetime from shineview_his.[dbo].[WarnAlarm_His] Where {0}", strWhere);
            return dal.ReturnData(sql);
        }

        #endregion

        #region [ 预警统计 ]

        /// <summary>
        /// 预警统计
        /// </summary>
        /// <param name="MineCode">煤矿编号</param>
        /// <param name="SensorNum">设备编号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public DataTable WarnAlarmTotal(string MineCode, string SensorNum, string startTime, string endTime)
        {
            string[,] strarr = new string[4, 5]
                {
                    {"MineCode", "=", MineCode, "string", "1"},
                    {"SensorNum", "=", SensorNum, "string", "1"},
                    {"starttime", ">=", startTime, "string", "1"},
                    {"starttime", "<=", endTime, "string", "1"}
                };

            string strWhere = GetSelectString(strarr);
            string sql =
                string.Format(
                    @"Select MineCode,mineName,sum(case when [status]='故障' then 1 else 0 end) gz,sum(case when [status]='报警' then 1 else 0 end) bj,
                    sum(case when [status]='断电' then 1 else 0 end) dd,sum(case when [status]='馈电异常' then 1 else 0 end) kd,sum(case when [status]='工作异常' then 1 else 0 end) gzyc
                    From shineview_his.[dbo].[WarnAlarm_His] Where {0} group by MineCode,mineName",
                    strWhere);
            return dal.ReturnData(sql);
        }

        #endregion

        #endregion

        #region[公共部分]
        DataDAL dal = new DataDAL();

        public DataTable MineList()
        {
            DataTable dt = new DataTable();
            dt = dal.GetMineList("1=1");
            return dt;
        }
        public DataTable GetTypeKind(string mineCode )
        {

            string sql = "select DISTINCT Type, case  Type  when  'A' then '模拟量' when  'C'  then '控制量' when 'D' then '开关量'  when 'F' then '分站/设备' when  'L' then '累积量' when 'U' then '抽象出的逻辑量'  else  '其他' end  TypeName from ( " +
                       "    select distinct  d.Type ,d.TypeName from  aqmc c  " +
                       "    left JOIN DeviceType d " +
                       "    on c.Type=d.typecode   where 1=1 ";
             string  sql2=    "    union  " +
                       "    select distinct  d.Type ,d.TypeName  from  AQKC c  " +
                       "    left JOIN DeviceType d " +
                       "    on c.Type=d.typecode where 1=1 ";
          string sql3="  ) as a ";
          if (!string.IsNullOrEmpty(mineCode))
          {
              sql += " and c.MineCode ='" + mineCode + "'";
              sql2 += " and c.MineCode ='" + mineCode + "'";
          }
          sql = sql + sql2 + sql3;
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询所有煤矿列表
        /// </summary>
        /// <returns></returns>
        public DataTable MineList(string minecode)
        {
            DataTable dt = dal.GetMineList(string.IsNullOrEmpty(minecode) ? "1=1" : string.Format("MineCode={0}", minecode));

            return dt;
        }
        /// <summary>
        /// 首页左侧树，
        /// StateCode（状态编号）:
        /// 0正常，
        /// 1.网络中断：无数据上传。
        /// 2.传输异常：客户端上传打开，未生成数据。
        /// 3、通讯中断：网络不稳定。
        /// 4、网络故障：网络不通。
        /// 5、数据延时：解析中实时数据与本机时间不一致
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public DataTable GetMineTreeInfo(string Type)
        {
        
            string sql = " with a as  " +
                     " (select m.id ,m.minecode,m.simplename,a.aqjkstate,b.ryglstate ,c.ksylstate,d.hzsgstate,MiningBureau "+
                     " from MineConfig m "+
                     " left join  (select MineCode,StateCode aqjkstate from SystemConfig where TypeCode=1 and IsEnabled=1) as a on a.minecode=m.id "+
                     " left join (select MineCode,StateCode ryglstate from SystemConfig where TypeCode=2 and IsEnabled=1) as b on m.id=b.MineCode  "+
                     " LEFT JOIN  (select MineCode,StateCode ksylstate from SystemConfig where TypeCode=5 and IsEnabled=1) as c on c.MineCode=m.id  "+
                     " LEFT JOIN  (select MineCode,StateCode hzsgstate from SystemConfig where TypeCode=7 and IsEnabled=1) as d on d.MineCode=m.id) "+

                     " select * from (select newid() rowid,mc.SimpleName,mc.MineCode,mc.City,mc.MiningBureau,"+
                     " case   a.aqjkstate  when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-' end as AQJKState, " +
                     " case   a.RYGLState when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-'  end as RYGLState ," +
                     " case   a.ksylState  when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-'  end as ksylState," +
                     " case   a.hzsgState when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-'  end as hzsgState " +


                     // "   a.aqjkstate  AQJKState, " +
                     //"    a.RYGLState  RYGLState ," +
                     //"    a.ksylState  ksylState," +
                     //"    a.hzsgState  hzsgState " +



                     " from MineConfig mc "+
                     " left join a on mc.ID=a.id ) as mytable "+
                    " where  1=1  ORDER BY "+Type+",minecode";
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 测试MODE
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public DataTableCollection ReturnTables(string Where, string WhereCount, string OrderColumns, string DB)
        {

            DataTableCollection dtc;
            SqlParameter[] sqls = new SqlParameter[5];
            sqls[0] = new SqlParameter("@pageNum", PageIndex);//查询页索引
            sqls[1] = new SqlParameter("@pageSize", PageSize);//查询页大小
            sqls[2] = new SqlParameter("@orderColum", OrderColumns);//排序列名
            sqls[3] = new SqlParameter("@sql_Count", WhereCount);//查询总行数主表
            sqls[4] = new SqlParameter("@sql", Where);//分页查询联合表

             dtc = dal.ReturnDTS_ExcutePro(sqls, DB);
            return dtc;
        }

        /// <summary>
        /// 煤矿列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMineInfo(string mineCode)
        {
            //string sql = "  select  newid() rowid, m.City,m.MineCode,m.SimpleName,s.TypeCode,s.StateCode from MineConfig m left join SystemConfig s on m.ID=s.MineCode ORDER BY m.city,m.MineCode";
            //string sql = string.Format("with a as " +
            //                            "(" +
            //                            "select a.MineCode,AQJK,RYGL,ksyl,hzsg,a.aqjkstate,b.ryglstate ,c.ksylstate,d.hzsgstate  from " +
            //                            "(select MineCode,Name AQJK,StateCode aqjkstate from SystemConfig where TypeCode=1) as a " +
            //                            "left join " +
            //                            "(select MineCode,name RYGL,StateCode ryglstate from SystemConfig where TypeCode=2) as b " +
            //                            "on a.MineCode=b.MineCode  LEFT JOIN  " +
            //                             "(select MineCode,name ksyl,StateCode ksylstate from SystemConfig where TypeCode=5) as c " +
            //                             "on c.MineCode=b.MineCode LEFT JOIN " +
            //                             " (select MineCode,name hzsg,StateCode hzsgstate from SystemConfig where TypeCode=7) as d on d.MineCode=b.MineCode" +
            //                            ")" +
            //                            "select * from (select newid() rowid,mc.SimpleName,mc.MineCode,mc.City,a.AQJK,case a.aqjkstate when 0 then '正常' when 1 then '通讯中断' else '传输异常' end as AQJKState," +
            //                            "a.RYGL,case a.RYGLState when 0 then '正常' when 1 then '通讯中断' else '传输异常' end as RYGLState ," +
            //                            "a.ksyl,case a.ksylState when 0 then '正常' when 1 then '通讯中断' else '传输异常' end as ksylState," +
            //                            "a.hzsg,case a.hzsgState when 0 then '正常' when 1 then '通讯中断' else '传输异常'end as hzsgState " +
            //                            "from MineConfig mc left join a on mc.ID=a.MineCode ) as mytable where minecode like '%" + mineCode + "%' order by City,MineCode "
            //                        );

            string sql = string.Format("with a as " +
                     "(select m.id ,m.minecode,m.simplename ,AQJK,RYGL,ksyl,hzsg,a.aqjkstate,b.ryglstate ,c.ksylstate,d.hzsgstate " +
                      " from MineConfig m " +
                      "left join  (select MineCode,Name AQJK,StateCode aqjkstate from SystemConfig where TypeCode=1 and IsEnabled=1) as a on a.minecode=m.id " +
                     " left join (select MineCode,name RYGL,StateCode ryglstate from SystemConfig where TypeCode=2 and IsEnabled=1) as b on m.id=b.MineCode  " +
                     " LEFT JOIN  (select MineCode,name ksyl,StateCode ksylstate from SystemConfig where TypeCode=5 and IsEnabled=1) as c on c.MineCode=m.id  " +
                     " LEFT JOIN  (select MineCode,name hzsg,StateCode hzsgstate from SystemConfig where TypeCode=7 and IsEnabled=1) as d on d.MineCode=m.id) " +

                     " select * from (select newid() rowid,mc.SimpleName,mc.MineCode,mc.City,a.AQJK," +
                    " case   a.aqjkstate  when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-' end as AQJKState,a.RYGL ," +
                     " case   a.RYGLState when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-'  end as RYGLState ,a.KSYL," +
                     " case   a.ksylState  when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-'  end as ksylState,a.HZSG,"+
                     " case   a.hzsgState when  0 then '正常' when 1 then '网络中断'  when 2 then  '传输异常' when 3 then '通讯中断'  when 4 then '网络故障' when 5 then '数据延时' else '-'  end as hzsgState " +
                     " from MineConfig mc " +
                     " left join a on mc.ID=a.id ) as mytable " +
                     " where  1=1 ");
            if (!string.IsNullOrEmpty(mineCode))
            {
                sql += "  and minecode ='" + mineCode + "' ";
            }
            string sql2 = "   order by City,MineCode";
            sql += sql2;
            return dal.ReturnData(sql);
        }


        /// <summary>
        /// 设备类型
        /// </summary>
        /// <returns></returns>
        public DataTable DeviceType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TypeName");
            dt.Columns.Add("TypeCode");
            DataRow dr0 = dt.NewRow();
            dr0[0] = "模拟量";
            dr0[1] = "A";
            dt.Rows.Add(dr0);
            DataRow dr1 = dt.NewRow();
            dr1[0] = "开关量";
            dr1[1] = "D";
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2[0] = "累计量";
            dr2[1] = "L";
            dt.Rows.Add(dr2);
            return dt;
        }

        /// <summary>
        /// 获取所有设备名、单位、设备代码、类型
        /// </summary>
        /// <returns></returns>
        public DataTable DeviceList()
        {
            DataTable dt = new DataTable();
            dt = dal.GetDeviceList("1=1");
            return dt;
        }


        /// <summary>
        /// 获取指定设备类型的设备
        /// </summary>
        /// <param name="typecode">设备类型编码 A-模拟量,C-控制量,D-开关量,L-累计量</param>
        /// <returns></returns>
        public DataTable DeviceList(string typecode)
        {
            string where ="";
            if (!string.IsNullOrEmpty(typecode))
            {
                where = "  Type='" + typecode + "'";
            }
            else where = " 1=1 ";
            DataTable dt = new DataTable();
            dt = dal.GetDeviceList(where);
            return dt;
        }

        /// <summary>
        /// 查询所有煤矿列表，用于下拉选择
        /// </summary>
        /// <returns></returns>
        public string GetMineItems()
        {
            string items = "";
            DataTable dt = new DataTable();
            dt = dal.GetMineList("1=1");
            if (dt.Rows.Count > 0)
            {
                items = "<items><item key=>全部</item>";
                foreach (DataRow dr in dt.Rows)
                {
                    items += "<item key=\"" + dr[1].ToString() + "\">" + dr[1].ToString() + "</item>";
                }
                items += "</items>";
            }
            return items;
        }


        /// <summary>
        /// 传输异常信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="systemtype">1 安全监控，2 人员管理，3 瓦斯抽放，4安全监控+瓦斯抽放</param>
        /// <returns></returns>
        public DataTable GetBadLog(string minecode, int systemtype)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(minecode))
            {
                where = string.Format("M.MineCode='{0}'", minecode);
            }

            return dal.GetRtBadLog(where);
        }

        /// <summary>
        /// 子系统配置信息
        /// </summary>
        /// <param name="minecode"></param>
        /// <returns></returns>
        public DataTable GetChildSystemSet(string minecode)
        {
            string sql =
                string.Format(
                    @"Select a.MineCode,SimpleName,SystemMACode,Name,case IsEnabled when 0 then '不起用' when 1 then '启用' end IsEnabled,
                    Supplier,Maintainer,a.Phone,IP,case StateCode when 0 then '正常' when 1 then '通讯中断' when 2 then '传输异常' end StateCode From [dbo].[MineConfig] A
                    Left Join [SystemConfig] B on a.ID = b.MineCode Where a.MineCode like '%{0}%'", minecode);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 获取所有实时报警信息
        /// </summary>
        /// <returns>返回查询结果</returns>
        public DataTable GetAllAlarmInfo(string mineCode, string AccountID)
        {
            string queryString = string.Empty;

            if (!string.IsNullOrEmpty(mineCode))
            {
                queryString = string.Format(" Where MineCode Like '%{0}%'", mineCode);
            }

            string sql = string.Format(@"With U As(
	                Select * From (
		                -- 人员管理报警信息
		                Select '超时报警' AlarmType, MineCode,'人员管理系统' AlarmGroup,'人员管理系统' SensorName From RYCS 
		                Union All 
		                Select '特种人员工作异常报警' AlarmType, MineCode,'人员管理系统' AlarmGroup,'人员管理系统' SensorName From RYTZYC
		                Union All 
		                Select '超员报警' AlarmType, MineCode,'人员管理系统' AlarmGroup,'人员管理系统' SensorName From RYCYXZ 
		                Union All
		                -- 安全监控报警信息
		                Select case A.ValueState When 1 Then '报警' When 4 Then '断电报警' When 8 Then '故障报警' When 16 Then '馈电异常报警' Else '工作异常报警' End AlarmType,
			                A.MineCode, '安全监控系统' as AlarmGroup, dt.TypeName SensorName From (Select MineCode,ValueState,[type] from Aqss where ValueState!=0) As A 
			                Left Join [dbo].[DeviceType] dt on A.[type]=dt.TypeCode Left Join MineConfig mc on A.MineCode=mc.MineCode
	                ) As U 
                )
                Select * From
                (
                        Select * From
                        (
	                        --通讯状态报警信息
	                        Select SimpleName, Case [StateCode] When 1 Then '通讯中断报警' When 2 Then '传输异常报警' End AlarmType, M.MineCode, 
                            case typecode when 1 then '安全监控系统' when 2 then '人员管理系统' end as AlarmGroup, case typecode when 1 then '安全监控系统' when 2 then '人员管理系统' end as  SensorName
		                        From (Select * From SystemConfig S Where s.isenabled=1 and  StateCode <> 0) S
		                        Left Join MineConfig M On S.MineCode = M.ID 
	                        Union All
	                        -- 人员管理及安全监控报警信息
	                        Select SimpleName, AlarmType, M.MineCode, U.AlarmGroup, U.SensorName From U Left Join MineConfig M On U.MineCode = M.MineCode 
		                        --Where U.MineCode Not In (Select M.MineCode From SystemConfig S Left Join MineConfig M On S.MineCode = M.ID Where s.isenabled=1 and  StateCode <> 0)
                        ) As U {0}
                    ) As U
                Where SimpleName Not In (Select LinkData From dbo.AlarmSet Where AlarmGroup = 0 And Disabled = 1 and AccountID='{1}')
	                And AlarmGroup Not In (Select LinkData From dbo.AlarmSet Where AlarmGroup = 1 And Disabled = 1 and AccountID='{1}')
	                And AlarmType Not In (Select LinkData From dbo.AlarmSet Where AlarmGroup = 2 And Disabled = 1 and AccountID='{1}')
	                And SensorName Not In (Select LinkData From dbo.AlarmSet Where AlarmGroup = 3 And Disabled = 1 and AccountID='{1}')
                Order By SimpleName, AlarmGroup", queryString, AccountID);

            return dal.ReturnData(sql);
        }

        #endregion

        #region[安全监控部分]
        /// <summary>
        /// 安监实时数据
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRealDataForAQ(string minecode)
        {
            DataTable dt = new DataTable();
            //string where = "select * from aqss where";
            //if (minecode != "" && minecode != null)
            //{
            //    where = "g.Minecode='" + minecode + "'";
            //}
            //else
            //{
            //    where = "1=1";
            //}
            //dt = dal.GetRealDataForAQ(where);

            string where = @"Select  distinct SensorNum,Place,A.[Type],TypeName,Unit from [dbo].[AQSS] A
                        Left Join [DeviceType] D on A.Type = D.TypeCode Where";
            if (minecode != "" && minecode != null)
            {
                where += " A.MineCode ='" + minecode + "'";
            }
            else
            {
                where += " 1=1";
            }
            int StstemType = Convert.ToInt32(System.Web.HttpContext.Current.Session["SystemType"]);
            if (StstemType != 0)
            {
                where += "and SystemType=" + StstemType;
            }
            DataTableCollection dtc = ReturnTables(where, where, "SensorNum", "Data");
            return dtc;
        }
        /// <summary>
        /// 安监实时数据
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTable GetRealDataForAQList(string minecode, string Devtype, string systemType)
        {
            DataTable dt = new DataTable();
            string where = " 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and g.Minecode='" + minecode + "'";
            }
            if (Devtype != null && Devtype != "")
            {
                where += " and  o.Type = '" + Devtype + "'";
            }
            if (systemType != null && systemType != "")
            {
                where += " and  o.SystemType = '" + systemType + "'";
            }
            dt = dal.GetRealDataForAQ(where);
            return dt;
        }

        public DataTable GetDevType(string MineCode, string systemType)
        {
            string sql = string.Format("select * from DeviceType where typecode in (select type from AQMC where MineCode='" + MineCode + "' and SystemType='" + systemType + "' union select type from AQKC where MineCode='" + MineCode + "' and SystemType='" + systemType + "' )order by TypeCode");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 安监实时数据
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="devtype">设备类型 A 模拟量，D 开关量，L 累计量</param>
        /// <returns></returns>
        public DataTableCollection GetRealDataForAQ(string minecode, string devname, string devtype)
        {
            DataTable dt = new DataTable();
            string where = "";
            string wherecount = "select * from AQSS where ";
            if (minecode != "" && minecode != null)
            {
                minecode = "g.Minecode='" + minecode + "'";
                if (where == "")
                {
                    where = minecode;
                }
                else
                {
                    where += " and " + minecode;
                }
                wherecount += " MineCode = '" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                devname = "d.TypeCode='" + devname + "'";
                if (where == "")
                {
                    where = devname;
                }
                else
                {
                    where += " and " + devname;
                }
                wherecount += "and TypeCode = '" + devname + "'";
            }
            if (devtype != "" && devtype != null)
            {
                devtype = "type='" + devtype + "'";
                if (where == "")
                {
                    where = devtype;
                }
                else
                {
                    where += " and " + devtype;
                }
                wherecount += " and type='" + devtype + "'";
            }
            if (where == "")
            {
                where = "1=1";
                wherecount = "1=1";
            }
            //dt = dal.GetRealDataForAQ(where);
            string wheredata = @"select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,SensorNum,
                    d.TypeName,d.Unit,Place,ShowValue, case when  (select top 1 [StateCode] from systemconfig where  isenabled=1 and  MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=1) = 0 then  (case ValueState when 0 then '正常'  when 1 then '报警' when 2 then '复电' when 4 
                    then '断电' when 8 then '故障' when 16 then '馈电异常' when 64 then '分站故障' else '工作异常' end)  when (select top 1 [StateCode] from systemconfig where  isenabled=1 and  MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=1) = 1 then  '通讯中断' else '传输异常' end  as ValueState  ,
                    Datetime from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode 
                    where " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 安监实时数据
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="sensornum">测点编号</param>
        /// <param name="devtype">设备类型 A 模拟量，D 开关量，L 累计量</param>
        /// <returns></returns>
        public DataTableCollection GetRealDataForAQ(string minecode, string devname, string sensornum, string devtype, string systemType)
        {

            DataTable dt = new DataTable();
            string TypeCode = systemType;
            string where = "";
            string wherecount = "select [Datetime] from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                wherecount += " and g.MineCode = '" + minecode + "'";
                minecode = " g.Minecode='" + minecode + "'";
                if (where == "")
                {
                    where = minecode;
                }
                else
                {
                    where += " and " + minecode;
                }
            }
            if (devname != "" && devname != null)
            {
                wherecount += " and d.Typecode = '" + devname + "'";
                devname = "d.Typecode='" + devname + "'";
                if (where == "")
                {
                    where = devname;
                }
                else
                {
                    where += " and " + devname;
                }
            }
            if (sensornum != "" && sensornum != null)
            {
                wherecount += " and o.Sensornum in ('" + sensornum.Replace(",", "','") + "')";
                sensornum = "o.SensorNum in ('" + sensornum.Replace(",", "','") + "')";
                if (where == "")
                {
                    where = sensornum;
                }
                else
                {
                    where += " and " + sensornum;
                }
            }
            if (devtype != "" && devtype != null)
            {
                //wherecount += " and type = '" + devtype + "'";
                devtype = "o.type='" + devtype + "'";
                if (where == "")
                {
                    where = devtype;
                }
                else
                {
                    where += " and " + devtype;
                }
            }
            if (systemType != null && systemType != "")
            {
                systemType = "o.systemType='" + systemType + "'";
                if (where == "")
                {
                    where = systemType;
                }
                else
                {
                    where += " and " + systemType;
                }
            }
            if (where == "")
            {
                where = " 1=1 ";
            }
            //dt = dal.GetRealDataForAQ(where);
            //return dt;


//            string wheredata = @"select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,o.SensorNum,g.MineCode ,o.Type,
//d.TypeName,d.Unit,o.Place,case when  (select top 1 [StateCode] from systemconfig where isenabled=1 and MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=" + TypeCode
//+ ") = 0 then  (case ValueState  when 8 then '故障'  when 32 then '工作异常'  when 64 then '分站故障' else ShowValue end) else '通讯中断' end as ShowValue,"
//+ "case when  (select top 1 [StateCode] from systemconfig  where  isenabled=1 and  MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=" + TypeCode
//+ ") = 0 then  (case ValueState when 0 then '正常'  when 1 then '报警' when 2 then '复电' when 4 then '断电' when 8 then '故障' when 16 then '馈电异常' when 64 then '分站故障' else '工作异常' end)  when (select top 1 [StateCode] from systemconfig where isenabled=1 and  MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=" + TypeCode
//+ ") = 1 then  '通讯中断' else '传输异常' end  as ValueState  ,Datetime,  isnull(AlarmHigh,0) AlarmHigh,isnull(AlarmLower,0) AlarmLower,isnull(OutPowerHigh,0) OutPowerHigh,isnull(OutPowerLower,0) OutPowerLower,isnull(InPowerHigh,0) InPowerHigh,isnull(InPowerLower,0)  InPowerLower  from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode  LEFT JOIN aqmc c on o.minecode = c.minecode and  o.sensornum=c.sensornum  left join systemconfig s on s.minecode =g.id    and s.typecode = o.systemtype  where " + where + "";

            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,o.SensorNum,g.MineCode ,o.Type, " +
             " d.TypeName,d.Unit,o.Place, " +
             " case (select top 1 [StateCode] from systemconfig where MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=" + TypeCode + ") " +
             "  when  0 then  (case ValueState  when 8 then '故障'  when 32 then '工作异常'  when 64 then '分站故障' else ShowValue end) " +
             " when 1 then  '网络中断'  when 2 then '传输异常' when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时'  end as ShowValue," +
             " case (select top 1 [StateCode] from systemconfig where MineCode in (select id from MineConfig where MineCode=o.MineCode) and TypeCode=" + TypeCode + ")  " +
             " when  0  then dbo.dectobin(ValueState) " +
             " when 1 then  '网络中断'  when 2 then '传输异常' when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时' end  as ValueState  ," +
             " Datetime,  isnull(AlarmHigh,0) AlarmHigh,isnull(AlarmLower,0) AlarmLower,isnull(OutPowerHigh,0) OutPowerHigh,isnull(OutPowerLower,0) OutPowerLower,isnull(InPowerHigh,0) InPowerHigh,isnull(InPowerLower,0) " +
             "  InPowerLower  from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode  " +
              " LEFT JOIN aqmc c on o.minecode = c.minecode and  o.sensornum=c.sensornum   where " + where + "";
            return ReturnTables(wheredata, wheredata, "TmpID", "Data");
        }

        public DataTableCollection GetRealDataForDTSRealData(string DeviceID, string ChannelID)
        {

            string sql = "select row_number() over (order by DeviceID,ChannelID) Tmp, DeviceID, ChannelID,RecordTime,temperaturedata,Meter TmpID from DTSRawData where 1=1 ";
            if (!string.IsNullOrEmpty(DeviceID))
            {
                sql += " and DeviceID='" + DeviceID + "'";
            
            }
            if (!string.IsNullOrEmpty(ChannelID))
            {
                sql += " and ChannelID='" + ChannelID + "'";

            }
            return ReturnTables(sql, sql, "Tmp", "Data");

        }

        public DataTableCollection GetSensorAll(string MineCode)
        {
            string sql = " select  Row_Number() over (order by SensorNum asc) as  TmpID ,SensorNum,Place,TypeName,Type from ( select  SensorNum , Place, TypeName ,d.Type from aqmc  m " +
                       "   left join  DeviceType d " +
                       "  on d.TypeCode = m.Type    where minecode ='" + MineCode + "'" +
                       "  UNION ALL" +
                       "   select SensorNum , Place, TypeName,d.Type  from aqkc  m" +
                       "   left join  DeviceType d " +
                       "   on d.TypeCode = m.Type    where minecode ='" + MineCode + "'";
            if (string.IsNullOrEmpty(MineCode))
            {
                sql += " and 1=2";
            }

            sql += " ) as A";
            return ReturnTables(sql, sql, "TmpID", "Data");
        }



        public DataTableCollection GetSensorAll()
        {
            string sql = " select  Row_Number() over (order by SensorNum asc) as  TmpID ,SensorNum,Place,TypeName,Type from ( select  SensorNum , Place, TypeName ,d.Type from aqmc  m    left join  DeviceType d   on d.TypeCode = m.Type    UNION ALL   select SensorNum , Place, TypeName,d.Type  from aqkc  m   left join  DeviceType d    on d.TypeCode = m.Type   ) as A";
            return ReturnTables(sql, sql, "TmpID", "Data");
        }


        public DataTableCollection GetVInfo(string SysCode)
        {
            string sql = "select row_number() over (order by VarName asc) as TmpID, SysCode,VarName SensorNum,EquipName TypeName,EquipPlace Place, " +
                       " case  EquipType when 1 then 'A' when  2 then 'D' when 3 then 'L' when 4 then 'C'" +
                       " end  Type " +
                       " from VariableFixInfo  where SysCode='" + SysCode+"'";
            return ReturnTables(sql, sql, "TmpID", "Data");
        }


        public  DataTableCollection GetRYFZAll(string MineCode)
        {
            string sql = " select Row_Number() over (order by StationCode asc) as  TmpID , StationCode , StationName,Place from RYFZ where MineCode ='" + MineCode + "'";
            if (string.IsNullOrEmpty(MineCode))
            {
                sql += " and 1=2";
            }

         
            return ReturnTables(sql, sql, "TmpID", "Data");
        
        }



        public DataTableCollection GetRYBZKHAll(string MineCode)
        {
            string sql = " select Row_Number() over (order by JobCardCode asc) as  TmpID , JobCardCode , Name,Position,Department from RYXX where MineCode ='" + MineCode + "'";
            if (string.IsNullOrEmpty(MineCode))
            {
                sql += " and 1=2";
            }


            return ReturnTables(sql, sql, "TmpID", "Data");

        }


        public DataTableCollection GetHZBHAll(string MineCode)
        {
            string sql = " select Row_Number() over (order by SensorNum asc) as  TmpID , SensorNum , Place,IPAddress from GBHZ where MineCode ='" + MineCode + "'";
            if (string.IsNullOrEmpty(MineCode))
            {
                sql += " and 1=2";
            }


            return ReturnTables(sql, sql, "TmpID", "Data");

        }
        public DataTableCollection GetCKBHAll(string MineCode)
        {
            string sql = " select Row_Number() over (order by SensorNum asc) as  TmpID , SensorNum , Place,IPAddress from CKHZ where MineCode ='" + MineCode + "'";
            if (string.IsNullOrEmpty(MineCode))
            {
                sql += " and 1=2";
            }


            return ReturnTables(sql, sql, "TmpID", "Data");

        }

        public DataTableCollection GetLedTable()
        {
            string sql = "select row_number() over (order by LEDIP  ) TmpID, LedIP,LedText,LedAddresss,SGIP,SGText,"+
"substring(LedShowConfig,2,len(LedShowConfig)-1)  LedShowConfig ,"+

"substring(LedShowConfig,1,1)  ConfigCount, IsSQL,   isnull(cast(SGPort as varchar ),'') SGPort  ," +
"case IsSQL when 1 then SGCondition  when 0 then '定时触发' end as SGSQL from LedShowTable";

            return ReturnTables(sql, sql, "TmpID", "Data");
        }
        public DataTableCollection GetLDKZ(string MineCode)
        {
            string sql = " select Row_Number() over (order by l.MineCode asc) as  TmpID ,SimpleName,l.MineCode ,SensorNums,s.Place ,d.TypeName,StationCodes,JobCardCodes," +
           " GBCodes, GBContent,IsRYHJKZ,IsGBKZ,CKCodes,CKContent,IsCKKZ from LDKZ l left join MineConfig m on  l.MineCode = m.MineCode " +
           " left JOIN  aqss s" +
           " on s.MineCode =l.MineCode and s.SensorNum= l.SensorNums" +
           " left join  deviceType d " +
           " on d.TypeCode = s.Type  where 1=1";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql += " and l.MineCode='" + MineCode + "'";
            }
            return ReturnTables(sql, sql, "TmpID", "Data");
        }
        /// <summary>
        /// 根据煤矿编号和设备类型、设备名，加载测点列表
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="SensorName">测点名称编号：如甲烷编号101003、一氧化碳编号103001、温度编号102001</param>
        /// <param name="devtype">设备类别 A,D,L分别表示模拟量、开关量、累积量</param>
        /// <returns></returns>
        public DataTableCollection GetSensorInfo(string minecode, string SensorNameCode, string devtype)
        {
            string where = " 1=1 ";
            string TableName = string.Empty;
            string wheredata = string.Empty;
            if (minecode != "" && minecode != null)
            {
                where += " and  g.MineCode = '" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(SensorNameCode))
            {
                where += " and  dt.typeCode = '" + SensorNameCode + "'";
            }
            if (devtype == "D")
            {
                TableName = "AQKC";
                wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,m.sensorNum,dt.TypeName,Place,'' Unit,'' AlarmLower,'' AlarmHigh,m.ZeroMeaning as '0态含义'," +
                    "(case m.ZeroISpolice when 0 then '否' else '是' end) as '0态报警',(case m.ZeroIsPower when 0 then '否' else '是' end) as '0态断电',OneMeaning as '1态含义',"
                    + " (case m.OneISpolice when 0 then '否' else '是' end) as '1态报警',(case m.OneIsPower when 0 then '否' else '是' end) as '1态断电',TwoMeaning as '2态含义',"
                    + " (case m.TwoISpolice when 0 then '否' else '是' end) as '2态报警',(case m.TwoIsPower when 0 then '否' else '是' end) as '2态断电',SensorTime from AQKC m "
                    + " left join DeviceType dt on m.Type=dt.TypeCode left join MineConfig g on m.MineCode=g.MineCode where " + where + "";
            }
            else
            {
                TableName = "AQMC";
                wheredata = " select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,o.SensorNum,dt.TypeName,dt.Unit,o.Place,o.Range,"
                    + " o.AlarmLower,o.AlarmHigh,o.AlarmLowerRemove,o.AlarmHighRemove,o.OutPowerLower,o.OutPowerHigh,o.InPowerLower,o.InPowerHigh,"
                    + " o.SensorTime from AQMC o left join DeviceType dt on o.Type=dt.TypeCode left join MineConfig g on o.MineCode=g.MineCode "
                    + " where " + where + "";
            }
            string wherecount = "select g.* from " + TableName + " as g left join DeviceType dt on g.Type=dt.TypeCode   where " + where;
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }


        #region 图表曲线
        /// <summary>
        /// 根据煤矿编号和测点，起始时间查查询曲线数据
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="SensorCodes">测点编号集合</param>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns>查询的数据集</returns>
        /// substring(convert(nvarchar(30),StatisticalTime,120),0,16)+'0:00'在 sql+下面   group by 和order by 后面
        public DataTableCollection ReturnCurverDatas(string mineCode, string SensorCodes, string BeginTime, string EndTime)
        {

            string[] sensorCodes = SensorCodes.Split(new char[] { ',' });
            string sql = "declare @BeginID bigint,@EndID bigint " +
                    "select @EndID =max(ID),@BeginID = min(ID) from ShineView_His.dbo.AQMT where StatisticalTime>='" + BeginTime + "' and StatisticalTime<'" + EndTime + "' ";
            foreach (string mysensorCode in sensorCodes)
            {
                sql += " select  max(StatisticaMaxValue) maxValue,min(StatisticaMinValue) minValue,AVG(StatisticaAvg) avgValue," +
                    " StatisticalTime statisticTime,Place,sensorNum from (select * from ShineView_His.dbo.AQMT where ID>=@BeginID and ID<@EndID) as xx where MineCode='" + mineCode + "' and sensorNum='" + mysensorCode + "'" +
                    " and StatisticalTime>='" + BeginTime + "' and StatisticalTime<'" + EndTime + "'" +
                    "group by StatisticalTime,sensorNum,Place order by StatisticalTime";
            }
            return dal.ReturnDs(sql).Tables;
        }


        public DataTableCollection ReturnSwitchDatas(string mineCode, string SensorCodes, string BeginTime, string EndTime)
        {
            string[] sensorCodes = SensorCodes.Split(new char[] { ',' });
            string sql = "";
            foreach (string mysensorCode in sensorCodes)
            {
                //不明白这个sql为什么要把id减1 为什么要查个state为0再查为空 再减一下时间算作次数
                //sql += " select xxx.SensorNum,xxx.Place,sum(datediff(second,xxx.StateDatetime,yyy.StateDatetime)) times,xxx.[state] from " +
                //    "(select * from shineview_his.[dbo].[AQKD] where MineCode='" + mineCode + "' and SensorNum='" + mysensorCode + "' and StateDatetime>'" + BeginTime + "' " +
                //        "and StateDatetime<'" + EndTime + "' and state=0) as xxx left join (select * from shineview_his.[dbo].[AQKD] where MineCode='" + mineCode + "' and SensorNum='" + mysensorCode + "' " +
                //        "and StateDatetime>'" + BeginTime + "' and StateDatetime<'" + EndTime + "') as yyy on xxx.ID=(yyy.ID-1) where yyy.StateDatetime is not null group  by xxx.SensorNum,xxx.Place,xxx.state " +
                //    " union all " +
                //   " select xxx.SensorNum,xxx.Place,sum(datediff(second,xxx.StateDatetime,yyy.StateDatetime)) times,xxx.[state] from " +
                //    "(select * from shineview_his.[dbo].[AQKD] where MineCode='" + mineCode + "' and SensorNum='" + mysensorCode + "' and StateDatetime>'" + BeginTime + "' " +
                //        "and StateDatetime<'" + EndTime + "' and state=1) as xxx left join (select * from shineview_his.[dbo].[AQKD] where MineCode='" + mineCode + "' and SensorNum='" + mysensorCode + "' " +
                //        "and StateDatetime>'" + BeginTime + "' and StateDatetime<'" + EndTime + "') as yyy on xxx.ID=(yyy.ID-1) where yyy.StateDatetime is not null group  by xxx.SensorNum,xxx.Place,xxx.state " +
                //    " union all " +
                //   " select xxx.SensorNum,xxx.Place,sum(datediff(second,xxx.StateDatetime,yyy.StateDatetime)) times,xxx.[state] from " +
                //    "(select * from shineview_his.[dbo].[AQKD] where MineCode='" + mineCode + "' and SensorNum='" + mysensorCode + "' and StateDatetime>'" + BeginTime + "' " +
                //        "and StateDatetime<'" + EndTime + "' and state=2) as xxx left join (select * from shineview_his.[dbo].[AQKD] where MineCode='" + mineCode + "' and SensorNum='" + mysensorCode + "' " +
                //        "and StateDatetime>'" + BeginTime + "' and StateDatetime<'" + EndTime + "') as yyy on xxx.ID=(yyy.ID-1) where yyy.StateDatetime is not null group  by xxx.SensorNum,xxx.Place,xxx.state ";


                // select xxx.SensorNum,xxx.Place,count(xxx.ID) times,xxx.[state] 
                //from 
                //(select * from shineview_his.[dbo].[AQKD] 
                //where MineCode='34060000001' and SensorNum='036D025' and StateDatetime>'2017-03-01' and StateDatetime<'2017-03-02' and state=0) 
                //as xxx  group  by xxx.SensorNum,xxx.Place,xxx.state 
                //union all 

                // select xxx.SensorNum,xxx.Place,count(xxx.ID) times,xxx.[state] 
                //from 
                //(select * from shineview_his.[dbo].[AQKD] 
                //where MineCode='34060000001' and SensorNum='036D025' and StateDatetime>'2017-03-01' and StateDatetime<'2017-03-02' and state=1) 
                //as xxx  group  by xxx.SensorNum,xxx.Place,xxx.state 
                //union all 
                // select xxx.SensorNum,xxx.Place,count(xxx.ID) times,xxx.[state] 
                //from 
                //(select * from shineview_his.[dbo].[AQKD] 
                //where MineCode='34060000001' and SensorNum='036D025' and StateDatetime>'2017-03-01' and StateDatetime<'2017-03-02' and state=2) 
                //as xxx  group  by xxx.SensorNum,xxx.Place,xxx.state 

                sql += string.Format("select xxx.SensorNum,xxx.Place,count(xxx.ID) times ,xxx.[state] from (select * from shineview_his.[dbo].[AQKD]  where MineCode='{0}' and SensorNum='{1}' and StateDatetime>'{2}' and StateDatetime<'{3}' and state=0) as xxx group  by xxx.SensorNum,xxx.Place,xxx.state ", mineCode, mysensorCode, BeginTime, EndTime);
                sql += string.Format("union all select xxx.SensorNum,xxx.Place,count(xxx.ID) times ,xxx.[state] from (select * from shineview_his.[dbo].[AQKD]  where MineCode='{0}' and SensorNum='{1}' and StateDatetime>'{2}' and StateDatetime<'{3}' and state=1 ) as xxx group  by xxx.SensorNum,xxx.Place,xxx.state ", mineCode, mysensorCode, BeginTime, EndTime);
                sql += string.Format("union all select xxx.SensorNum,xxx.Place,count(xxx.ID) times ,xxx.[state] from (select * from shineview_his.[dbo].[AQKD]  where MineCode='{0}' and SensorNum='{1}'  and StateDatetime>'{2}' and StateDatetime<'{3}' and state=2 ) as xxx group  by xxx.SensorNum,xxx.Place,xxx.state ", mineCode, mysensorCode, BeginTime, EndTime);
            }
            return dal.ReturnDs(sql).Tables;
        }
        public DataTableCollection RtCurver(string mineCode, string SensorCodes ,DateTime time , string timeRound)
        {
            string[] sensorCodes = SensorCodes.Split(new char[] { ',' });
            string where = "";
            for (int i=0;i<sensorCodes.Count();i++)
            {
                if (i == 0)
                    where += "'" + sensorCodes[i].ToString() + "'";
                else
                    where += " or SensorNum='" + sensorCodes[i].ToString() + "'";
                if (i == sensorCodes.Count() - 1)
                {
                    where += " )";
                }
            }


            string sql = string.Format("Select  DateAdd(ss,  dtm * 10, '2017-1-1') dtm2, * From " +
            "( Select DateDiff(ss, '2017-1-1', [DateTime]) / 10 dtm, * From AQSSCurve Where MineCode = '{0}' And  ( SensorNum =  {1}" +
"  ) T  Where dtm > (Select  DateDiff(ss, '2017-1-1', '{2}') / 10 - 6 * 10)  " +
" And [Datetime] > '{2}' " +
 " Order By [DateTime]   ", mineCode, where, time);

            return dal.ReturnDs(sql).Tables;
        }
        #endregion

        /// <summary>
        /// 设备配置信息
        /// </summary>
        /// <param name="minecode">煤矿编码</param>
        /// <param name="devtype">【必选】A 模拟量，D 开关量，L 累计量</param>
        /// <returns></returns>
        public DataTableCollection GetDeviceInfo(string minecode, string devtype, string SensorNum)
        {
            string where = "";
            string TableName = string.Empty;
            string wheredata = string.Empty;
            if (minecode != "" && minecode != null)
            {
                where = "g.MineCode = '" + minecode + "'";
            }
            else
            {
                where = "1=1";
            }
            if (!string.IsNullOrEmpty(SensorNum))
            {
                where += string.Format(" and SensorNum  in ('{0}') ", SensorNum.Replace(",", "','"));
            }

            if (!string.IsNullOrEmpty(devtype) )
            {
                where += " and  o.Type='"+devtype+"'";
            }
            if (devtype == "D")
            {
                TableName = "AQKC";
                wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,m.sensorNum,d.TypeName,m.ZeroMeaning as '0态含义'," +
                    "(case m.ZeroISpolice when 0 then '否' else '是' end) as '0态报警',(case m.ZeroIsPower when 0 then '否' else '是' end) as '0态断电',OneMeaning as '1态含义',"
                    + " (case m.OneISpolice when 0 then '否' else '是' end) as '1态报警',(case m.OneIsPower when 0 then '否' else '是' end) as '1态断电',TwoMeaning as '2态含义',"
                    + " (case m.TwoISpolice when 0 then '否' else '是' end) as '2态报警',(case m.TwoIsPower when 0 then '否' else '是' end) as '2态断电',SensorTime from AQKC m "
                    + " left join DeviceType d on m.Type=d.TypeCode left join MineConfig g on m.MineCode=g.MineCode where " + where + "";
            }
            else
            {
                TableName = "AQMC";
                wheredata = " select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,o.SensorNum,d.TypeName,d.Unit,o.Place,o.Range,"
                    + " o.AlarmLower,o.AlarmHigh,o.AlarmLowerRemove,o.AlarmHighRemove,o.OutPowerLower,o.OutPowerHigh,o.InPowerLower,o.InPowerHigh,"
                    + " o.SensorTime from AQMC o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode "
                    + " where " + where + "";
            }
            string wherecount = "select * from " + TableName + " as g where " + where;
            return ReturnTables(wheredata, wheredata, "TmpID", "Data");
        }
        /// <summary>
        /// 实时报警
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRealAQBJ(string minecode, string devname, string systemType, string SensorNum)
        {
            string where = "";
            string wherecount = "select * from AQSS  where SUBSTRING(dbo.dectobin(ValueState), 16, 1)=1 ";
            if (minecode != "" && minecode != null)
            {
                where = "g.Minecode='" + minecode + "' and ";
                wherecount += " and MineCode = '" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                where += "o.type = '" + devname + "' and ";
                wherecount += " and type = '" + devname + "'";
            }

            if (systemType != null && systemType != "")
            {
                where += " o.systemType='" + systemType + "'  ";
                wherecount += " and systemType = '" + systemType + "' ";
            }
            if (!string.IsNullOrEmpty(SensorNum))
            {
                string[] SensorCodes = SensorNum.Split(new char[] { ',' });
                for (int i = 0; i < SensorCodes.Count(); i++)
                {
                    if (i == 0)
                    {
                        where += " and  (o.SensorNum='" + SensorCodes[i] + "'";
                        wherecount += " and (SensorNum='" + SensorCodes[i] + "'";
                    }
                    else
                    {
                        where += " or o.SensorNum='" + SensorCodes[i] + "'";
                        wherecount += " or SensorNum='" + SensorCodes[i] + "'";
                    }
                    if (i == (SensorCodes.Count() - 1))
                    {
                        where += ")";
                        wherecount += ")";
                    }
                }

            }

            where += " and SUBSTRING(dbo.dectobin(ValueState), 16, 1)=1 ";
            //return dal.GetRealAQBJ(where);            
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,o.DateTime, g.SimpleName,o.SensorNum,d.TypeName,d.Unit,o.Place,ShowValue,isnull(mc.AlarmLower,0) AlarmLower, isnull(mc.AlarmHigh,0) AlarmHigh ,isnull(mc.OutPowerHigh,0) OutPowerHigh,isnull(mc.OutPowerLower,0) OutPowerLower ,isnull(mc.InPowerLower,0) InPowerLower ,isnull(mc.InPowerHigh,0) InPowerHigh ,'报警' as ValueState,PoliceDatetime,PoliceMaxValue,PowerMaxDatetime,powerMin,PowerMinDatetime,PowerAvg,[dbo].[FunConvertTime](datediff(second, PoliceDatetime,getdate())) as continuoustime  from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode  left join systemconfig sc on sc.minecode=g.id and sc.typecode =o.systemtype left join aqmc mc on  o.minecode =mc.minecode and o.SensorNum=mc.sensornum  where " + where;


            return ReturnTables(wheredata, wherecount, "TmpID", "Data");


        }

        /// <summary>
        /// 实时断电
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRealAQDD(string minecode, string devtype, string systemType)
        {
            string where = "";
            string wherecoutn = "select * from AQSS where SUBSTRING(dbo.dectobin(ValueState), 14, 1)=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " g.Minecode='" + minecode + "' and ";
                wherecoutn += " and MineCode = '" + minecode + "'";
            }
            if (systemType != null && systemType != "")
            {
                where += " o.systemType='" + systemType + "' and ";
                wherecoutn += " and systemType = '" + systemType + "' ";
            }
            if (devtype != "" && devtype != null)
            {
                where += " o.type = '" + devtype + "' and";
                wherecoutn += " and type = '" + devtype + "'";
            }
            where += " SUBSTRING(dbo.dectobin(ValueState), 14, 1)=1";
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,SensorNum,d.TypeName,d.Unit,Place,ShowValue,'断电' as ValueState,PowerDateTime,PowerMax,PowerMaxDatetime,powerMin,PowerMinDatetime,PowerAvg,dbo.FunConvertTime(datediff(second, PowerDateTime,getdate())) as continuoustime from AQSS o  left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode    left join systemconfig s on s.minecode =g.id    and s.typecode = o.systemtype  where " + where + "";
            return ReturnTables(wheredata, wherecoutn, "TmpID", "Data");
        }

        /// <summary>
        /// 实时故障
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRealAQGZ(string minecode, string DevType, string SystemType, string sensorNum)
        {
            string where = "";
            string wherecount = "select * from AQSS where SUBSTRING(dbo.dectobin(ValueState), 13, 1)=1  or   SUBSTRING(dbo.dectobin(ValueState), 10, 1)=1  ";
            if (minecode != "" && minecode != null)
            {
                where += "g.Minecode='" + minecode + "' and ";
                wherecount += " and MineCode = '" + minecode + "' ";
            }
            if (DevType != null && DevType != "")
            {
                where += " o.type='" + DevType + "' and ";
                wherecount += " and type = '" + DevType + "' ";
            }
            if (SystemType != null && SystemType != "")
            {
                where += " o.systemType='" + SystemType + "' and ";
                wherecount += " and systemType = '" + SystemType + "' ";
            }

            if (sensorNum != "" && sensorNum != null)
            {
                where += " o.sensorNum in ('" + sensorNum.Replace(",", "','") + "') and ";
                wherecount += " and sensorNum in ('" + sensorNum.Replace(",", "','") + "')";
            }

            where += "  SUBSTRING(dbo.dectobin(o.ValueState), 13, 1)=1  or   SUBSTRING(dbo.dectobin(o.ValueState), 10, 1)=1 ";
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,o.SensorNum,d.TypeName,d.Unit,o.Place,cast ('故障' as varchar(50)) ShowValue,isnull(mc.AlarmLower,0) AlarmLower, isnull(mc.AlarmHigh,0) AlarmHigh ,isnull(mc.OutPowerHigh,0) OutPowerHigh,isnull(mc.OutPowerLower,0) OutPowerLower ,isnull(mc.InPowerLower,0) InPowerLower ,isnull(mc.InPowerHigh,0) InPowerHigh ,'故障' as ValueState,PoliceDateTime as Datetime,dbo.FunConvertTime(datediff(second, PoliceDateTime,getdate())) as continuoustime from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode left join aqmc mc on  o.minecode =mc.minecode and o.SensorNum=mc.sensornum    left join systemconfig s on s.minecode =g.id    and s.typecode = o.systemtype  where " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 实时馈电异常
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRealAQYC(string minecode, string devtype, string sensorNum)
        {
            string where = "";
            string wherecount = "select * from AQSS where  SUBSTRING(dbo.dectobin(ValueState), 12, 1)=1  ";
            if (minecode != "" && minecode != null)
            {
                where += "g.Minecode='" + minecode + "' and ";
                wherecount += " and MineCode = '" + minecode + "'";
            }
            if (devtype != "" && devtype != null)
            {
                where += "o.type = '" + devtype + "' and ";
                wherecount += "and type = '" + devtype + "'";
            }
            if (sensorNum != "" && sensorNum != null)
            {
                where += " sensorNum in ('" + sensorNum.Replace(",", "','") + "') and ";
                wherecount += " and sensorNum in ('" + sensorNum.Replace(",", "','") + "')";
            }

            where += " SUBSTRING(dbo.dectobin(ValueState), 12, 1)=1";
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,g.SimpleName,SensorNum,d.TypeName,d.Unit,Place,ShowValue,'馈电异常' as ValueState,AbnormalDateTime,PowerDateTime,PowerMax,PowerMaxDatetime,powerMin,PowerMinDatetime,PowerAvg,dbo.FunConvertTime(datediff(second, AbnormalDateTime,getdate())) as continuoustime from AQSS o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where  " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }


        /// <summary>
        /// 分钟数据显示
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        /// 2015-2-3修改：m 改成 o
        public DataTableCollection GetMinutesData(string minecode, string devname, DateTime BegingTime, DateTime EndTime)
        {
            string where = "";
            string wherecount = "select * from [ShineView_His].[dbo].AQMT where 1=1 ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            if (minecode != "" && minecode != null)
            {
                where = "o.Minecode='" + minecode + "' and ";
                wherecount += " and Minecode = '" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                where += "o.Type='" + devname + "' and ";
                wherecount += "  and Type = '" + devname + "'";
            }
            where += "o.StatisticalTime>='" + BegingTime + "' and o.StatisticalTime<='" + EndTime + "'";
            wherecount += " and StatisticalTime>='" + BegingTime + "' and StatisticalTime<='" + EndTime + "'";
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,sensorNum,d.TypeName,d.Unit,Place,StatisticaMaxValue,StatisticaMaxDatetime,StatisticaMinValue,StatisticaMinDatetime,StatisticaAvg,StatisticalTime from [ShineView_His].[dbo].AQMT o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 查询开关量变动
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetAQKDData(string minecode, string devname, DateTime BegingTime, DateTime EndTime)
        {
            string where = "";
            string wherecount = "select * from [ShineView_His].[dbo].AQKD where ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            if (minecode != "" && minecode != null)
            {
                where = "k.Minecode='" + minecode + "' and ";
                wherecount += " MineCode='" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                where += "TypeName='" + devname + "' and ";
                wherecount += "and TypeName='" + devname + "'";
            }
            where += "k.statedatetime>='" + BegingTime + "' and k.statedatetime<='" + EndTime + "'";
            wherecount += " and statedatetime>'" + BegingTime + "' and statedatetime<'" + EndTime + "'";
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,SensorNum,d.TypeName,d.Unit,Place,state,(case isPolice when 0 then '正常' else '报警' end) as alarmstate,(case IsPower when 0 then '正常' else '断电' end) as powerstate,StateDatetime from [ShineView_His].[dbo].AQKD o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 历史报警查询
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        /// 2015-2-4 修改记录：添加o.
        public DataTableCollection GetHisAQBJ(string minecode, string devname, string sensorNum, DateTime BegingTime, DateTime EndTime, string systemType)
        {
            string where = "";
            string wherecount = "select * from [ShineView_His].[dbo].AQBJ where 1=1 ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }

            if (minecode != "" && minecode != null)
            {
                where += "Minecode='" + minecode + "' and ";
                wherecount += " and  MineCode='" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                where += " type='" + devname + "' and ";
                wherecount += " and type='" + devname + "'";
            }
            if (sensorNum != "" && sensorNum != null)
            {
                where += " sensorNum in ('" + sensorNum.Replace(",", "','") + "') and ";
                wherecount += " and sensorNum in ('" + sensorNum.Replace(",", "','") + "')";
            }
            if (systemType != "" && systemType != null)
            {
                where += " systemType='" + systemType + "' and ";
                wherecount += " and systemType='" + systemType + "'";
            }
            where += "PoliceDatetime>='" + BegingTime + "' and PoliceDatetime<='" + EndTime + "'  and PoliceEndDatetime is not null";
            wherecount += " and PoliceDatetime>='" + BegingTime + "' and PoliceDatetime<='" + EndTime + "' and PoliceEndDatetime is not null";
            //string wheredata = "select * from [ShineView_His].[dbo].AQBJ";
            string wheredata = string.Format(@"
            select ROW_NUMBER() over (order by getdate() asc) as tmpID,mc.SimpleName,hisBJ.SensorNum,dt.TypeName,hisbj.Place,hisbj.alarmtype,hisbj.PoliceDatetime,
            hisbj.PoliceEndDatetime,hisbj.continuoustime, isnull(ka.AlarmValue,'-')AlarmValue ,dt.Unit,hisbj.MaxValue,hisbj.MaxTime,hisbj.MinValue,hisbj.MinTime,hisbj.[avg],
            hisbj.RelevanceDepict,hisbj.measures,hisbj.alarmwhy
             from
            (
            select mineCode,SensorNum,place,
            (case PoliceType
            when 0 then '上限报警' when 1 then '下限报警' else '开关量报警' end) as alarmtype,[Type],
            [PoliceDatetime],[PoliceEndDatetime],
            [ShineView_Data].dbo.[FunConvertTime](datediff(second,[PoliceDatetime],[PoliceEndDatetime])) as continuoustime,
            [MaxValue],[MaxTime],[MinValue],[MinTime],[avg],[RelevanceDepict],[measures],
            (case [PoliceDepict]
             when 0 then '放炮' 
             when 1 then '停电'
             when 2 then '割煤'
             when 3 then '采空区来压'
             when 4 then '调试'
             when 5 then '通风'
             when 6 then '发火'
             when 7 then '断线断电'
             else '其他' 
             end) as alarmwhy
             from [ShineView_His].dbo.AQBJ where {0}
             ) as hisBJ
            left join
            (select * from
            (select minecode, [SensorNum],
                  (case 
	              when [ZeroISpolice]=1 then ZeroMeaning
	              when [OneISpolice]=1 then [OneMeaning]
	              when [TwoISpolice]=1 then [TwoMeaning]
	              end )as AlarmValue
	              from
             [ShineView_Data].dbo.AQKC) as KAlarm  where KAlarm.AlarmValue is not null) as KA
              on KA.MineCode=hisbj.MineCode and ka.SensorNum=hisbj.SensorNum
              left join ShineView_Data.dbo.MineConfig mc on hisbj.MineCode=mc.MineCode
              left join ShineView_Data.dbo.DeviceType dt on hisbj.[Type]=dt.TypeCode
            ", where);

            // "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,SensorNum,d.TypeName,d.Unit,Place,(case PoliceType when 0 then '上限报警' when 1 then '下限报警' else '开关量报警' end) as alarmtype,PoliceDatetime,PoliceEndDatetime,[dbo].[FunConvertTime](datediff(second, PoliceDatetime,PoliceEndDatetime)) as continuoustime,MaxValue,MaxTime,MinValue,MinTime,avg,RelevanceDepict,measures,(case PoliceDepict when 0 then '放炮' when 1 then '停电' when 2 then '割煤' when 3 then '采空区来压' when 4 then '调试' when 5 then '通风' when 6 then '发火' when 7 then '断线断电' else '其他' end) as alarmwhy from [ShineView_His].[dbo].AQBJ o left join ShineView_Data.dbo.DeviceType d on o.Type=d.TypeCode left join ShineView_Data.dbo.MineConfig g on o.MineCode=g.MineCode where " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }


        public DataTableCollection GetGBHZ(string MineCode)
        {
            
            string sql = " select row_number() over (order by g.MineCode asc) TmpID,g.MineCode ,m.SimpleName,SensorNum,Place,IPAddress,Time from GBHZ g left join MineConfig m on m.MineCOde = g.MineCode";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql += " where g.MineCode ='" + MineCode + "'";
            }
            return ReturnTables(sql, sql, "TmpID", "Data");
        }

        public DataTableCollection GetGBSS(string MineCode)
        {
            //select  s.MineCode,s.SensorNum,h.Place,IPAddress,s.State,s.Time from gbss  s left JOIN gbhz h on h.minecode =s.MineCode and h.sensorNum=s.sensornum
            string sql = " SELECT row_number() over (order by g.MineCode asc) TmpID,g.MineCode,m.SimpleName,g.SensorNum,h.Place ,h.IPAddress," +
                           " case State when 0 then '未知' when 1 then '正常' when 2 then '故障' when 3 then '广播' end State,g.Time from gbss g "+
                           " left join  GBHZ h on g.minecode = h.minecode and g.sensornum=h.sensornum "+
                           " left join MineConfig m on m.minecode = g.minecode";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql += " where g.MineCode ='" + MineCode + "'";
            }
           return ReturnTables(sql, sql, "TmpID", "Data");
        }
        public DataTableCollection GetCKSS(string MineCode)
        {
            //select  s.MineCode,s.SensorNum,h.Place,IPAddress,s.State,s.Time from gbss  s left JOIN gbhz h on h.minecode =s.MineCode and h.sensorNum=s.sensornum
            string sql = " SELECT row_number() over (order by g.MineCode asc) TmpID,g.MineCode,m.SimpleName,g.SensorNum,h.Place ,h.IPAddress," +
                           " case State when 0 then '未知' when 1 then '正常' when 2 then '故障' when 3 then '广播' end State,g.Time from CKss g " +
                           " left join  CKHZ h on g.minecode = h.minecode and g.sensornum=h.sensornum " +
                           " left join MineConfig m on m.minecode = g.minecode";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql += " where g.MineCode ='" + MineCode + "'";
            }
            return ReturnTables(sql, sql, "TmpID", "Data");
        }

        public DataTable LoadChildSysTypeKind(string MineCode)
        {
            string sql = "select DISTINCT   cast(EquipType as VARCHAR(20))   Type,case EquipType when 0 then '模拟量' when 1 then '开关量' when 2 then '累积量' " +
                         " end as TypeName from TF_Equipment";
            return dal.ReturnData(sql);
        }

        public DataTableCollection GetRealDataForTFST(string TypeKind, string TypeName, string SensorNum,string VIEW)
        {
            string SysCode = "";

            SysCode = VIEW.Substring(0, 2);
          
            string where = "";
            if (!string.IsNullOrEmpty(TypeKind))
            {
                where += " and te.EquipType ='" + TypeKind + "'";
            }
            if (!string.IsNullOrEmpty(TypeName))
            {
                where += " and te.EquipName='" + TypeName + "'";
            
            }


            if (!string.IsNullOrEmpty(SensorNum))
            {
                where += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
             
            }

            string sql = "  select row_number() over (order by  tr.PointID  )TmpID,  tp.PointName SensorName,tp.EquipPlace Place,Unit ,"+
                       " te.EquipName TypeName ,"+
                       " case te.EquipType when 0 then '模拟量'  when 1 then '开关量' when 2 then '累积量' end Type, "+
                       " case VarStatus when 0 then ( "+
                       " case  te.EquipType when 0 then   cast(VarValue as  VARCHAR)  "+
                       " when 1 then "+
                       " (case  cast(VarValue as int) when 0 then KaiGuan_0_Show when 1 then KaiGuan_1_Show  when 2 then  KaiGuan_2_Show end) "+
                       " end ) when 1 then '故障' when 2 then 'OPC断线'  end   [Value] ,"+
                       " UpdateTime," +
                       " case VarStatus when 0 then '正常' when 1 then '故障' when 2 then 'OPC断线' end ValueState "+
                       " from  ["+SysCode+"_RealValues] tr  "+
                       " left join " + SysCode + "_Points tp on tr.PointID=tp.PointID  " +
                       " left join " + SysCode + "_Equipment te on  te.ID= tp.EquipID where 1=1 " + where;


            string sqlCount = "select tp.PointName " + " from  [" + SysCode + "_RealValues] tr  " +
                       " left join " + SysCode + "_Points tp on tr.PointID=tp.PointID  " +
                       " left join " + SysCode + "_Equipment te on  te.ID= tp.EquipID where 1=1 " + where;
            return ReturnTables(sql, sqlCount, "TmpID", "data");
           

        }


        public DataTableCollection GetRealDataForTFSTHisMNL(string TypeName, string SensorNum, DateTime BeginTime, DateTime EndTime,string VIEW,int flag)
        {
            if (flag == 2)
            {
                string sql2 = "select 1 as TmpID  from aqss where 1=2";
                return ReturnTables(sql2, sql2, "TmpID", "Data");
            }
            string Table = "";
            Table = VIEW.Substring(0, 2);
            //string sql = "select row_number() over (ORDER BY PointID) TmpID,  * ,case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  from ShineView_His.dbo." + Table + "_HistorySimulateValues  where Time>'" + BeginTime + "' and Time<'" + EndTime + "'";
            //string sqlCount = "select PointID  from ShineView_His.dbo." + Table + "_HistorySimulateValues  where Time>'" + BeginTime + "' and Time<'" + EndTime + "'";
           
            
            //if (!string.IsNullOrEmpty(TypeName))
            //{
            //    sql += " and  EquipName ='" + TypeName + "'";
            //    sqlCount += " and  EquipName ='" + TypeName + "'"; 
            //}
            ////if (!string.IsNullOrEmpty(SensorNum))
            ////{
            ////    sql += " and PointID  ='" + SensorNum + "'";
            ////}


            //if (!string.IsNullOrEmpty(SensorNum))
            //{
            //    sql += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
            //    sqlCount += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
            //}
            string sql = "  select PointID,  PointName,EquipName ,EquipPlace,Time,VarValue, Unit ," +
                          "  case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
                          "  from ShineView_His.dbo." + Table + "_HistorySimulateValues_";
            string where = " where  1=1 ";

            if (!string.IsNullOrEmpty(TypeName))
            {
                where += " and  EquipName ='" + TypeName + "'";
            }
          


            if (!string.IsNullOrEmpty(SensorNum))
            {
                where += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
             
            }

            string Select = "";

            string Condition = "select row_number() over (order by PointID) TmpID,PointName,EquipName ,EquipPlace,Time,VarValue,ValueState, Unit from(";
            int BeginYear = BeginTime.Year;
            int BeginMonth = BeginTime.Month;
            int BeginDay = BeginTime.Day;
            int EndMonth = EndTime.Month;
            int EndDay = EndTime.Day;
            int EndYear = EndTime.Year;
         
            if (BeginMonth == EndMonth)
            {
                for (int i = BeginDay; i < EndDay + 1; i++)
                {
                    string mounth = "";
                    string day = "";

                    if (BeginTime.Month < 10)
                    {
                        mounth = "0" + BeginTime.Month;
                    }
                    else {
                        mounth = BeginTime.Month.ToString();
                    }

                    if (i < 10)
                    {
                        day = "0" + i;
                    }
                    else
                    {
                        day = i.ToString();
                    }

                    DateTime dt = new DateTime(BeginTime.Year, BeginTime.Month, i);
                    Select += sql + dt.Year + "_" + mounth + "_" + day + where + " and  [Time]>'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "' and [Time]<'" + dt.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    if (i != EndDay)
                    {
                        Select += " union all ";
                    }
                }

            }
            Condition = Condition + Select + ") as a";
            //string sql = "select row_number() over (ORDER BY PointID) TmpID, PointName,EquipName ,EquipPlace,Time,VarValue,ValueState, Unit from (" +

            //              "  select PointID,  PointName,EquipName ,EquipPlace,Time,VarValue, Unit ," +
            //              "  case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
            //              "  from ShineView_His.dbo.GD_HistorySimulateValues_2018_03_17" +

            //                " where Time>'2018-03-17 00:00:00' and Time<'2018-03-17 23:59:59'" +

            //                 " union all  " +

            //                "  select PointID, PointName,EquipName ,EquipPlace,Time,VarValue, Unit," +
            //                 " case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
            //                "  from ShineView_His.dbo.GD_HistorySimulateValues_2018_03_21" +

            //                "  where Time>'2018-03-21 00:00:00' and Time<'2018-03-21 23:59:59'" +

            //                "  union all  " +

            //                "  select PointID, PointName,EquipName ,EquipPlace,Time,VarValue, Unit," +
            //                "  case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
            //                "  from ShineView_His.dbo.GD_HistorySimulateValues_2018_03_19" +

            //                "  where Time>'2018-03-19 00:00:00' and Time<'2018-03-19 23:59:59'" +

            //               "   union all  " +

            //                "  select PointID, PointName,EquipName ,EquipPlace,Time,VarValue, Unit ," +
            //                "  case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
            //               "   from ShineView_His.dbo.GD_HistorySimulateValues_2018_03_20" +

            //               "   where Time>'2018-03-20 00:00:00' and Time<'2018-03-20 23:59:59'" +

            //               "   union all  " +

            //                "  select PointID,  PointName,EquipName ,EquipPlace,Time,VarValue, Unit ," +
            //                "  case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
            //                "  from ShineView_His.dbo.GD_HistorySimulateValues_2018_03_18" +

            //               "   where Time>'2018-03-18 00:00:00' and Time<'2018-03-18 23:59:59'" +


            //               "   ) as a ";
            return ReturnTables(Condition, Condition, "TmpID", "Data");
        
        }
        //GetRealDataForTFSTHisKGL



        public DataTableCollection GetOPCSatate(string SensorNum)
        {
            string sql = "select row_number() over (order by ChildSysCode asc) TmpID, * from ChildSysConfig  where 1=1 and   TablePrefix IS not null";
            if (!string.IsNullOrEmpty(SensorNum))
            {

                sql += " and ChildSysCode ='" + SensorNum + "'";
            }
            return ReturnTables(sql, sql, "TmpID", "data");
        
        }
        public DataTableCollection GetRealDataForTFSTHisFZMNL(string TypeName, string SensorNum, DateTime BeginTime, DateTime EndTime, string VIEW, int flag)
        {
            if (flag == 2)
            {
                string sql2 = "select 1 as TmpID  from aqss where 1=2";
                return ReturnTables(sql2, sql2, "TmpID", "Data");
            }
            string Table = "";
            Table = VIEW.Substring(0, 2);
            string sql = "select row_number() over (ORDER BY PointID) TmpID,  *  from ShineView_His.dbo." + Table + "_HistorySimulateValuesCount  where StartTime>'" + BeginTime + "' and StartTime<'" + EndTime + "'";
            string sqlCount = "select PointID  from ShineView_His.dbo." + Table + "_HistorySimulateValuesCount where StartTime>'" + BeginTime + "' and StartTime<'" + EndTime + "'";


            if (!string.IsNullOrEmpty(TypeName))
            {
                sql += " and  EquipName ='" + TypeName + "'";
                sqlCount += " and  EquipName ='" + TypeName + "'"; 
            }
            //if (!string.IsNullOrEmpty(SensorNum))
            //{
            //    sql += " and PointID  ='" + SensorNum + "'";
            //}


            if (!string.IsNullOrEmpty(SensorNum))
            {
                sql += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
                sqlCount += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
            }
        
            return ReturnTables(sql, sqlCount, "TmpID", "Data");

        }
        //GetRealDataForTFSTHisKGL



        public DataTableCollection GetRealDataForTFSTHisKGL(string TypeName, string SensorNum, DateTime BeginTime, DateTime EndTime,string VIEW,int flag)
        {
            string Table = "";
            Table = VIEW.Substring(0, 2);
            if (flag == 2)
            {
                string sql2 = "select 1 as TmpID  from aqss where 1=2";
                return ReturnTables(sql2, sql2, "TmpID", "Data");
            }
         //TF_HistorySimulateValuesCount
            string sql = "  select PointID,  PointName,EquipName ,EquipPlace,Time,VarValue, Unit ," +
                          "  case  Status when 0 then  '正常' when 1 then '故障'  when 2 then 'OPC断线' end ValueState  " +
                          "  from ShineView_His.dbo." + Table + "_HistorySwitchValues_";
            string where = " where  1=1 ";

            if (!string.IsNullOrEmpty(TypeName))
            {
                where += " and  EquipName ='" + TypeName + "'";
            }



            if (!string.IsNullOrEmpty(SensorNum))
            {
                where += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";

            }

            string Select = "";

            string Condition = "select row_number() over (order by PointID) TmpID,PointName,EquipName ,EquipPlace,Time,VarValue,ValueState, Unit from(";
            int BeginYear = BeginTime.Year;
            int BeginMonth = BeginTime.Month;
            int BeginDay = BeginTime.Day;
            int EndMonth = EndTime.Month;
            int EndDay = EndTime.Day;
            int EndYear = EndTime.Year;

            if (BeginMonth == EndMonth)
            {
                for (int i = BeginDay; i < EndDay + 1; i++)
                {
                    string mounth = "";
                    string day = "";

                    if (BeginTime.Month < 10)
                    {
                        mounth = "0" + BeginTime.Month;
                    }
                    else
                    {
                        mounth = BeginTime.Month.ToString();
                    }

                    if (i < 10)
                    {
                        day = "0" + i;
                    }
                    else
                    {
                        day = i.ToString();
                    }

                    DateTime dt = new DateTime(BeginTime.Year, BeginTime.Month, i);
                    Select += sql + dt.Year + "_" + mounth + "_" + day + where + " and  [Time]>'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "' and [Time]<'" + dt.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    if (i != EndDay)
                    {
                        Select += " union all ";
                    }
                }

            }
            Condition = Condition + Select + ") as a";
            //string sql = "select row_number() over (ORDER BY PointID) TmpID,  *   from ShineView_His.dbo."+Table+"_HistorySwitchValues  where StartTime>'" + BeginTime + "' and StartTime<'" + EndTime + "'";
            //string sqlCount = "select PointID  from ShineView_His.dbo." + Table + "_HistorySwitchValues  where StartTime>'" + BeginTime + "' and StartTime<'" + EndTime + "'";
            
            //if (!string.IsNullOrEmpty(TypeName))
            //{
            //    sql += " and  EquipName ='" + TypeName + "'";
            //    sqlCount += " and  EquipName ='" + TypeName + "'";
            //}

            //if (!string.IsNullOrEmpty(SensorNum))
            //{
            //    sql += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
            //    sqlCount += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";

            //}

            return ReturnTables(Condition, Condition, "TmpID", "Data");

        }
        public DataTableCollection GetRealDataForTFSTHisGZ(string TypeKind, string TypeName, string SensorNum, DateTime BeginTime, DateTime EndTime,string VIEW)
        {
            string Table = "";
            Table = VIEW.Substring(0, 2);
            string sql = "select row_number() over (ORDER BY PointID) TmpID,  * ,case EquipType when 0 then '模拟量' when 1 then '开关量' when 2 then '累积量' end Type  from ShineView_His.dbo." + Table + "_HistoryErrorValues  where StartTime>'" + BeginTime + "' and StartTime<'" + EndTime + "'";
            string sqlCount = "select  PointID from ShineView_His.dbo." + Table + "_HistoryErrorValues  where StartTime>'" + BeginTime + "' and StartTime<'" + EndTime + "'";
           
            if (!string.IsNullOrEmpty(TypeName))
            {
                sql += " and  EquipName ='" + TypeName + "'";
                sqlCount += " and  EquipName ='" + TypeName + "'";
            }
            if (!string.IsNullOrEmpty(TypeKind))
            {
                sql += " and   EquipType ='" + TypeKind+ "'";
                sqlCount += " and   EquipType ='" + TypeKind + "'";

            }
            if (!string.IsNullOrEmpty(SensorNum))
            {
                sql += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";
                sqlCount += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";

            }

            return ReturnTables(sql, sqlCount, "TmpID", "Data");

        }


        public DataTableCollection GetRealDataForTFSTGZ(string TypeKind, string TypeName, string SensorNum, string VIEW)
        {

            string SysCode = "";
            SysCode = VIEW.Substring(0, 2);
            string where = "";
            if (!string.IsNullOrEmpty(TypeKind))
            {
                where += " and te.EquipType ='" + TypeKind + "'";
            }
            if (!string.IsNullOrEmpty(TypeName))
            {
                where += " and te.EquipName='" + TypeName + "'";

            }


            if (!string.IsNullOrEmpty(SensorNum))
            {
                where += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";

            }

            string sql = "  select row_number() over (order by  tr.PointID  )TmpID,  tp.PointName SensorName,tp.EquipPlace Place,Unit ," +
                       " te.EquipName TypeName ," +
                       " case te.EquipType when 0 then '模拟量'  when 1 then '开关量' when 2 then '累积量' end Type, " +
                       " case VarStatus when 0 then ( " +
                       " case  te.EquipType when 0 then   cast(VarValue as  VARCHAR)  " +
                       " when 1 then " +
                       " (case  cast(VarValue as int) when 0 then KaiGuan_0_Show when 1 then KaiGuan_1_Show  when 2 then  KaiGuan_2_Show end) " +
                       " end ) when 1 then '故障' when 2 then 'OPC断线'  end   [Value] ," +
                       " UpdateTime," +
                       " case VarStatus when 0 then '正常' when 1 then '故障' when 2 then 'OPC断线' end ValueState " +
                       " from  [" + SysCode + "_RealValues] tr  " +
                       " left join " + SysCode + "_Points tp on tr.PointID=tp.PointID  " +
                       " left join " + SysCode + "_Equipment te on  te.ID= tp.EquipID where  VarStatus=1 " + where;


            string sqlCount = "select tp.PointName " + " from  [" + SysCode + "_RealValues] tr  " +
                       " left join " + SysCode + "_Points tp on tr.PointID=tp.PointID  " +
                       " left join " + SysCode + "_Equipment te on  te.ID= tp.EquipID where VarStatus=1 " + where;
            return ReturnTables(sql, sqlCount, "TmpID", "data");
           
        }

        //GetRealDataForTFSTHisGZ(TypeKind,TypeName, SensorNum, BeginTime, EndTime);
        public DataTableCollection GetRealDataForTFSTDX(string TypeKind, string TypeName, string SensorNum, string VIEW)
        {

            string SysCode = "";
            SysCode = VIEW.Substring(0, 2);
            string where = "";
            if (!string.IsNullOrEmpty(TypeKind))
            {
                where += " and te.EquipType ='" + TypeKind + "'";
            }
            if (!string.IsNullOrEmpty(TypeName))
            {
                where += " and te.EquipName='" + TypeName + "'";

            }


            if (!string.IsNullOrEmpty(SensorNum))
            {
                where += " and   PointName in ('" + SensorNum.Replace(",", "','") + "') ";

            }

            string sql = "  select row_number() over (order by  tr.PointID  )TmpID,  tp.PointName SensorName,tp.EquipPlace Place,Unit ," +
                       " te.EquipName TypeName ," +
                       " case te.EquipType when 0 then '模拟量'  when 1 then '开关量' when 2 then '累积量' end Type, " +
                       " case VarStatus when 0 then ( " +
                       " case  te.EquipType when 0 then   cast(VarValue as  VARCHAR)  " +
                       " when 1 then " +
                       " (case  cast(VarValue as int) when 0 then KaiGuan_0_Show when 1 then KaiGuan_1_Show  when 2 then  KaiGuan_2_Show end) " +
                       " end ) when 1 then '故障' when 2 then 'OPC断线'  end   [Value] ," +
                       " UpdateTime," +
                       " case VarStatus when 0 then '正常' when 1 then '故障' when 2 then 'OPC断线' end ValueState " +
                       " from  [" + SysCode + "_RealValues] tr  " +
                       " left join " + SysCode + "_Points tp on tr.PointID=tp.PointID  " +
                       " left join " + SysCode + "_Equipment te on  te.ID= tp.EquipID where  VarStatus=2 " + where;


            string sqlCount = "select tp.PointName " + " from  [" + SysCode + "_RealValues] tr  " +
                       " left join " + SysCode + "_Points tp on tr.PointID=tp.PointID  " +
                       " left join " + SysCode + "_Equipment te on  te.ID= tp.EquipID where VarStatus=2 " + where;
            return ReturnTables(sql, sqlCount, "TmpID", "data");

        }
        //GetRealDataForTFGZTG

        public DataTableCollection GetRealDataForTFGZTG(string TypeKind, string TypeName, string SensorNum,DateTime BeginTime ,DateTime EndTime, string View,int flag)
        {
            string table = "";
            table = View.Substring(0, 2);
            //string sql = "select row_number() over (order by tp.PointID) TmpID, tp.PointID,tp.PointName,te.EquipName,te.Unit,case te.EquipType when 0 then '模拟量' when 1 then '开关量' " +
            //           " when 2 then '累积量'  end  Type ,tp.EquipPlace  ,Count(tp.PointID) Counts  " +

            //           " from ShineView_Data.dbo." + table + "_Points tp" +
            //           " left join ShineView_Data.dbo." + table + "_Equipment te on tp.EquipID= te.ID" +
            //           " left join ShineView_His.dbo." + table + "_HistoryErrorValues the on tp.PointID =the.PointID " +
            //             " where  StartTime>'"+BeginTime+"' and StartTime<'"+EndTime+"'";

            string sql="select   row_number() over (ORDER BY tp.PointID) TmpID, tp.PointID,tp.PointName,te.EquipName,te.Unit,case te.EquipType when 0 then '模拟量' when 1 then '开关量' "+
                       " when 2 then '累积量'  end  Type ,tp.EquipPlace  , isnull(Cunnts,0) Counts " +

                       " from ShineView_Data.dbo."+table+"_Points tp "+
                       " left join ShineView_Data.dbo."+table+"_Equipment te on tp.EquipID= te.ID  "+

                       " left join ( select PointID,Count(PointID) Cunnts from ShineView_His.dbo."+table+"_HistoryErrorValues  where StartTime >='"+BeginTime +"' and StartTime<='"+ EndTime+"'  GROUP BY PointID ) AS A "+
                       " on A.PointID= tp.PointID where  1="+ flag;
            if (!string.IsNullOrEmpty(TypeKind))
            { 
                sql+=" and te.EquipType ="+TypeKind;
            
            }

            if (!string.IsNullOrEmpty(TypeName))
            {
                sql += " and te.EquipName ='" + TypeName+"'";
            }
            if (!string.IsNullOrEmpty(SensorNum))
            {
                sql += " and   tp.PointName in ('" + SensorNum.Replace(",", "','") + "') ";

            }
            return ReturnTables(sql,sql,"TmpID","His");
        }
        public DataTableCollection GetRealDataForJSG8(DateTime BegingTime, DateTime EndTime)
        {
            string table = "His_Beamtube_";
            if (BegingTime == null)
            {
                table += DateTime.Now.Year;

            }
            else
                table += BegingTime.Year;
            //table += "His_Beamtube_2017";
            BegingTime = BegingTime.Date;
            //BegingTime = BegingTime.Date.AddMonths(-3);
            string begintime = BegingTime.ToString("yyyy-M-d HH:mm:ss");
            EndTime = BegingTime.Date.AddHours(24).AddSeconds(-1);
            string endtime = EndTime.ToString("yyyy-M-d HH:mm:ss");
            string isexit = string.Format("(select * from syscolumns where name='BTHis_DID' and  id=object_id('{0}'))", table);
            DataTable dt = new DataTable();
            dt = dal.ReturnData(isexit);

            if (dt.Rows.Count > 0)
            {
                string where = " select  *  from   " + table + " where DetectTime>= '" + begintime + "'  and  DetectTime<=  '" + endtime + "'";

                // 点击了查询 显示某一天
                return ReturnTables(where, where, "BTHis_DID", "Data");





            }
            else return null;




            //return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }


        /// <summary>
        /// 获取预警（实时）
        /// </summary>
        /// <param name="minecode"></param>
        /// <returns></returns>
        public DataTableCollection GetData_PreAlarm(string minecode, string SensorNum, string devtype)
        {
            //0:正常;1:报警;2:复电;4:断电;8:故障;16:馈电异常;32:工作异常
            string whereData = "select  Row_Number() over (order by w.ID desc) as TmpID,  w.ID,w.flag,w.MineCode,case w.SensorNum  when 'JKXT1'  then '安全监控' when 'JKXT2' then '人员管理' else w.SensorNum end SensorNum,w.StartTime,w.OverTime," +
                " w.MaxValue," +
                "w.MinValue,w.LastTime,w.DateTime,w.fenxi,w.chulway,w.chulresult,w.jlpers,w.ChulStatus,d.TypeName,s.ShowValue," +
" w.jltime,c.Place,m.SimpleName,c.Unit,s.Type,  cast(null as varchar(50)) Continuous,( case w.level when 1 then '红色' when 2 then '橙色' when 3 then '黄色' when 4 then '蓝色' else  '' end )as Class ," +
"(case w.chulStatus  when 1  then '报警' when 2 then '复电' when 4 then '断电' when 8 then '故障' when 16 then '馈电异常' when 32 then '工作异常'   when  41 then '网络中断' when 42 then '传输异常' when 43 then '通讯中断' when 44 then '网络故障' when 45 then '数据延时' end) as State " +
" from warnalarm  w LEFT JOIN aqss s on w.MineCode=s.MineCode  and w.SensorNum=s.SensorNum  " +
" LEFT JOIN MineConfig m on w.MineCode=m.MineCode  " +
" LEFT JOIN aqmc c on w.MineCode=c.minecode and w.SensorNum=c.sensornum" +
"  LEFT JOIN DeviceType d on  s.type= d.TypeCode  where 1=1  and w.level!=5 ";
            string whereCount = "select w.MineCode  as count  from warnalarm  w LEFT JOIN aqss s on w.MineCode=s.MineCode  and w.SensorNum=s.SensorNum  " +
                " LEFT JOIN MineConfig m on w.MineCode=m.MineCode " +
                "  LEFT JOIN aqmc c on w.MineCode=c.minecode and w.SensorNum=c.sensornum " +
                " LEFT JOIN DeviceType d on  s.type= d.TypeCode  where 1=1 and w.level!=5 ";
            if (!string.IsNullOrEmpty(minecode))
            {
                whereData += " and w.minecode='" + minecode + "'";
                whereCount += " and w.minecode='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(SensorNum))
            {
                string[] SensorCodes = SensorNum.Split(new char[] { ',' });
                for (int i = 0; i < SensorCodes.Count(); i++)
                {
                    if (i == 0)
                    {
                        whereData += " and  (w.SensorNum='" + SensorCodes[i] + "'";
                        whereCount += " and (w.SensorNum='" + SensorCodes[i] + "'";
                    }
                    else
                    {
                        whereData += " or w.SensorNum='" + SensorCodes[i] + "'";
                        whereCount += " or w.SensorNum='" + SensorCodes[i] + "'";
                    }
                    if (i == (SensorCodes.Count() - 1))
                    {
                        whereData += ")";
                        whereCount += ")";
                    }
                
                }
            }
            if (!string.IsNullOrEmpty(devtype))
            {
                whereData += " and s.Type='" + devtype + "'";
                whereCount += " and s.Type='" + devtype + "'";
            }

            return ReturnTables(whereData, whereData, "TmpID", "Data");
        }
        public DataTableCollection GetData_PreAlarm_His(string minecode, string SensorNum, string devtype, DateTime BegingTime, DateTime EndTime)
        {
            //0:正常;1:报警;2:复电;4:断电;8:故障;16:馈电异常;32:工作异常
            string whereData = "select  Row_Number() over (order by getdate() asc) as TmpID,( case w.level when 1 then '红色' when 2 then '橙色' when 3 then '黄色' when 4 then '蓝色'  when 6 then '' end )as Class, w.ID,w.MineCode,w.place,w.type,w.value,w.MaxValue,w.MineValue,w.unit,w.LastTime, cast(null as varchar(50)) Continuous,case  w.SensorNum when 'JKXT1' then '安全监控' when 'JKXT2' then '人员管理' else w.SensorNum end SensorNum,w.StartTime,w.OverTime," +

                "w.jltime,w.datetime,w.fenxi,w.chulway,w.chulresult,w.jlpers,w.minename as SimpleName,d.TypeName,s.ShowValue," +

"(case w.status  when 1  then '报警' when 2 then '复电' when 4 then '断电' when 8 then '故障' when 16 then '馈电异常' when 32 then '工作异常'  when 41 then '网络中断' when 42 then '传输异常' when 43 then '通讯中断' when 44 then '网络故障' when 45 then '数据延时' end) as State " +
" from ShineView_His.dbo.warnalarm_His  w LEFT JOIN ShineView_Data.dbo.aqss s on w.MineCode=s.MineCode  and w.SensorNum=s.SensorNum  " +
" LEFT JOIN ShineView_Data.dbo.MineConfig m on w.MineCode=m.MineCode  " +
" LEFT JOIN ShineView_Data.dbo.aqmc c on w.MineCode=c.minecode and w.SensorNum=c.sensornum" +
"  LEFT JOIN ShineView_Data.dbo.DeviceType d on  s.type= d.TypeCode  where 1=1 ";
            string whereCount = "select w.MineCode  as count from ShineView_His.dbo.warnalarm_His  w LEFT JOIN ShineView_Data.dbo.aqss s on w.MineCode=s.MineCode  and w.SensorNum=s.SensorNum    " +
              " LEFT JOIN ShineView_Data.dbo.MineConfig m on w.MineCode=m.MineCode  " +
" LEFT JOIN ShineView_Data.dbo.aqmc c on w.MineCode=c.minecode and w.SensorNum=c.sensornum" +
"  LEFT JOIN ShineView_Data.dbo.DeviceType d on  s.type= d.TypeCode  where 1=1 ";
            if (!string.IsNullOrEmpty(minecode))
            {
                whereData += " and w.minecode='" + minecode + "'";

                whereCount += " and w.minecode='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(SensorNum))
            {
                whereData += " and w.SensorNum='" + SensorNum + "'";
                whereCount += " and w.minecode='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(devtype))
            {
                whereData += " and s.Type='" + devtype + "'";
                whereCount += " and s.Type='" + devtype + "'";
            }
            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                whereData += " and w.starttime>='" + BegingTime + "'";
                whereCount += " and w.starttime>='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                whereData += " and w.starttime<='" + EndTime + "'";
                whereCount += " and w.starttime<='" + EndTime + "'";
            }

            return ReturnTables(whereData, whereCount, "TmpID", "His");
        }
        /// <summary>
        /// 预警统计
        /// </summary>
        /// <param name="minecode">煤矿名称</param>
        /// <param name="SensorNum"></param>
        /// <param name="devtype"></param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetData_PreAlarm_TG(string minecode, string SensorNum, string devtype, DateTime BegingTime, DateTime EndTime,int flag)
        {
            //0:正常;1:报警;2:复电;4:断电;8:故障;16:馈电异常;32:工作异常
            string wherecount = " SELECT  Row_Number() over (order by getdate() asc) as TmpID from MineConfig T1 " +
" left JOIN (select minecode, COUNT(status) BJCount from   [ShineView_His].[dbo].WarnAlarm_his T2   where status=1  and T2.level!=5 ";
            string sql1 = " SELECT  Row_Number() over (order by getdate() asc) as TmpID,  T1.MineCode,T1.SimpleName ,BJCount ,DDCount,GZCount,GZYCCount,KDYCCount,AQJK_TXZDCount,RYGL_TXZDCount from MineConfig T1 " +
 " left JOIN (select minecode, COUNT(status) BJCount from   [ShineView_His].[dbo].WarnAlarm_his T2   where status=1  and T2.level!=5 ";
            string sql2 = " group by minecode, status) as T2  on t1.minecode=T2.minecode " +

 " left JOIN (select minecode, COUNT(status) DDCount from   [ShineView_His].[dbo].WarnAlarm_his T2   where status=4 and T2.level!=5 ";
            string sql3 = " group by minecode, status) as T3  on t1.minecode=T3.minecode" +

" left JOIN (select minecode, COUNT(status) GZCount from   [ShineView_His].[dbo].WarnAlarm_his T2   where status=8  and T2.level!=5 ";
            string sql4 = " group by minecode, status) as T4  on t1.minecode=T4.minecode " +

" left JOIN (select minecode, COUNT(status) KDYCCount from   [ShineView_His].[dbo].WarnAlarm_his T2   where status=16 and T2.level!=5 ";
            string sql5 = " group by minecode, status) as T5  on t1.minecode=T5.minecode" +

"  left JOIN (select minecode, COUNT(status) GZYCCount from   [ShineView_His].[dbo].WarnAlarm_his T2   where status=32 and T2.level!=5  ";
            string sql6 = " group by minecode, status) as T6  on t1.minecode=T6.minecode " +
                "left JOIN (select  minecode, COUNT(status) AQJK_TXZDCount from   [ShineView_His].[dbo].WarnAlarm_his T2  where  T2.level=6   and T2.SensorNum='JKXT1'    ";
            string sql7 = " group by minecode) as T7  on t1.minecode=T7.minecode "+
                " left JOIN  (select  minecode, COUNT(status) RYGL_TXZDCount "+
                " from   [ShineView_His].[dbo].WarnAlarm_his T2  where T2.level=6   and T2.SensorNum='JKXT2'     ";

           
          
            string sql10 = " group by minecode)as T10  on t1.minecode=T10.minecode where 1="+flag;
            if (!string.IsNullOrEmpty(minecode))
            {
                sql10 += " and T1.MineCode='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(BegingTime.ToShortDateString()))
            {
                sql1 += " and  starttime>='" + BegingTime + "'";
                sql2 += " and starttime>='" + BegingTime + "'";
                sql3 += " and  starttime>='" + BegingTime + "'";
                sql4 += " and starttime>='" + BegingTime + "'";
                sql5 += " and  starttime>='" + BegingTime + "'";
                sql6 += " and starttime>='" + BegingTime + "'";
                sql7 += " and starttime>='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToShortDateString()))
            {
                sql1 += " and  starttime<='" + EndTime + "'";
                sql2 += " and starttime<='" + EndTime + "'";
                sql3 += " and  starttime<='" + EndTime + "'";
                sql4 += " and starttime<='" + EndTime + "'";
                sql5 += " and  starttime<='" + EndTime + "'";
                sql6 += " and starttime<='" + EndTime + "'";
                sql7 += " and starttime<='" + EndTime + "'";
            }
            string wheredate = sql1 + sql2 + sql3 + sql4 + sql5 + sql6 + sql7 +sql10;
            wherecount = wherecount + sql2 + sql3 + sql4 + sql5 + sql6 + sql7  + sql10;


            return ReturnTables(wheredate, wherecount, "MineCode", "His");
        }


        /// <summary>
        /// 历史通讯中断
        /// </summary>
        /// <param name="minecode">煤矿名称</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetData_TXZDHis(string minecode, DateTime BegingTime, DateTime EndTime)
        {
            // 去的时间是Badlog的Continuous  
            string wheredata = " select  Row_Number() over (order by  b.LastTime asc) as TmpID, " +
" (case b.TypeCode when 1 then '安全监控' when 2 then '人员管理' when 5 then '矿山压力' when 7 then '火灾束管' else '其他系统' end ) as JKXT, "+
" case b.stateCode  when 1 then '网络中断' when 2 then '传输异常'  when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时' else '其他异常' end StateCode, "+
" b.MineCode,m.SimpleName,b.LastTime StartTime ,b.Continuous,b.LastTime EndTime,  cast ( null as VARCHAR(50)) LastTime "+
" from ShineView_His.dbo.BadLog b left join MineConfig m on b.MineCode=m.MineCode "+
" left join SystemConfig sc on sc.MineCode=m.id and sc.TypeCode=b.typecode "+
" where 1=1   and sc.IsEnabled=1";

            if (!string.IsNullOrEmpty(minecode))
            {
                wheredata += "  and b.MineCode='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                wheredata += "  and b.LastTime>='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                wheredata += "  and b.LastTime<='" + EndTime + "'";
            }
            return ReturnTables(wheredata, wheredata, "TmpID", "His");
        }






        public DataTableCollection GetData_TXYCHis(string minecode, DateTime BegingTime, DateTime EndTime)
        {

            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID, b.MineCode , b.BeginTime,b.EndTime,m.SimpleName,cast(NULL as  varchar(50)) Continuous," +
                "(case b.typecode when 1 then '安全监控' when 2 then '人员管理'  when 5 then '矿山压力' when 7 then '火灾束管' else '其他系统' end) as JKXT " +
                "from  ShineView_His.dbo.BadCreate b  " +
                "left join ShineView_Data.dbo.MineConfig m on b.MineCode=m.minecode where 1=1  ";
            if (!string.IsNullOrEmpty(minecode))
            {
                wheredata += "  and b.MineCode='" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                wheredata += "  and b.BeginTime>='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                wheredata += "  and b.BeginTime<='" + EndTime + "'";
            }
            return ReturnTables(wheredata, wheredata, "TmpID", "His");
        }

        public DataTableCollection GetData_TXZD(string minecode)
        {

//            string sql = string.Format("select  Row_Number() over (order by getdate() asc) as TmpID,mc.SimpleName,mc.MineCode,mc.City,a.AQJK, a.AQJKTime,a.RYGLTime,a.KSYLTime,a.HZSGTime," +
//"case when  a.aqjkstate = 0 then '正常' when a.aqjkstate =1 then '通讯中断'  when a.aqjkstate is null then  '-' else '传输异常' end as AQJKState,a.RYGL,  " +
//"case when  a.RYGLState = 0 then '正常'  when  a.RYGLState = 1 then '通讯中断' when  a.RYGLState is null then  '-' else '传输异常' end as RYGLState ,a.ksyl, " +
//"case when  a.ksylState = 0 then '正常' when  a.ksylState = 1 then '通讯中断' when  a.ksylState  is null then  '-' else '传输异常' end as ksylState,a.hzsg, " +
//"case when  a.hzsgState = 0 then '正常' when  a.hzsgState = 1 then '通讯中断' when  a.hzsgState IS null then  '-'  else '传输异常' end as hzsgState  " +
//"from MineConfig mc  " +
//" left join  " +
//"(select Row_Number() over (order by getdate() asc) as TmpID,m.id ,m.minecode,m.simplename ,AQJK,RYGL,ksyl,hzsg,a.aqjkstate ,b.ryglstate  ,c.ksylstate ,d.hzsgstate ,a.UpdateTime AQJKTime,b.UpdateTime RYGLTime,c.UpdateTime KSYLTime,d.UpdateTime HZSGTime from MineConfig m  " +
//"left join  (select MineCode,Name AQJK,StateCode aqjkstate,UpdateTime from SystemConfig where TypeCode=1 and IsEnabled=1) as a on a.minecode=m.id   " +
//"left join (select MineCode,name RYGL,StateCode ryglstate ,UpdateTime from SystemConfig where TypeCode=2 and IsEnabled=1) as b on m.id=b.MineCode   " +

//" LEFT JOIN  (select MineCode,name ksyl,StateCode ksylstate ,UpdateTime from SystemConfig where TypeCode=5 and IsEnabled=1) as c on c.MineCode=m.id    " +
//"LEFT JOIN  (select MineCode,name hzsg,StateCode hzsgstate,UpdateTime from SystemConfig where TypeCode=7 and IsEnabled=1) as d on d.MineCode=m.id)  as a  on mc.ID=a.id " +
//"  where  1=1   ");
            //string sql = "";
            string sql = "select  Row_Number() over (order by getdate() asc) as TmpID,  m.MineCode , m.SimpleName , case s.TypeCode when 1 then '安全监控'  when '2' then '人员管理' when '5' then '矿山压力' when '7' then '火灾束管' else '其他系统' end as  Type ," +
                            " case s.StateCode  when 1 then '网络中断'  when '0' then '正常'  when  2 then '传输异常' when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时' else '异常状态' end ValueState,s.UpdateTime from SystemConfig  s  " +
                            " LEFT JOIN  MineConfig m on m.id=s.MineCode " +
                            " where s.StateCode!=0 and s.isenabled =1 ";
            if (!string.IsNullOrEmpty(minecode))
            {
                sql += "  and m.minecode ='" + minecode + "' ";
            }




            return ReturnTables(sql, sql, "MineCode", "His");
        }



        public DataTableCollection GetData_TXZDTG(string minecode, DateTime BegingTime, DateTime EndTime, int  flag)
        {
            string sql1="select  Row_Number() over (order by m.MineCode,s.TypeCode asc) as TmpID,m.MineCode,SimpleName  , " +
                            " case s.typecode   when 1 then '安全监控' when 2 then '人员管理' when 5 then '矿山压力' when 7 then '火灾束管' END JKXT ,"+
                            " IsNull(WLZDCount,0) WLZDCount,IsNull(CSYCCount,0) CSYCCount,IsNull(TXZDCount,0) TXZDCount,IsNull(WLGZCount,0) WLGZCount,IsNull(SJYSCount,0) SJYSCount"+
                            " from ShineView_Data.dbo.SystemConfig s "+
                            " left join ShineView_Data.dbo.MineConfig m on m.ID = s.MineCode "+
                            " Left  Join "+
                            " ( select MineCode , TypeCode ,  count(StateCode) WLZDCount  from ShineView_His.dbo.BadLog WHERE  StateCode=1 ";
            string sql2=" GROUP BY MineCode ,TypeCode,StateCode) as  a " +
                             " on a.MineCode=m.MineCode and a.TypeCode=s.TypeCode" +
                             " Left  Join " +
                             " ( select MineCode , TypeCode ,  count(StateCode) CSYCCount  from ShineView_His.dbo.BadLog WHERE  StateCode=2  ";
            string sql3=" GROUP BY MineCode ,TypeCode,StateCode) as  b " +
                              " on b.MineCode=m.MineCode and b.TypeCode=s.TypeCode" +
                              " Left  Join " +
                              " ( select MineCode , TypeCode ,  count(StateCode) TXZDCount  from ShineView_His.dbo.BadLog WHERE  StateCode=3 ";
            string sql4=" GROUP BY MineCode ,TypeCode,StateCode) as  c " +
                               " on c.MineCode=m.MineCode and c.TypeCode=s.TypeCode" +
                               " Left  Join " +
                               " ( select MineCode , TypeCode ,  count(StateCode) WLGZCount  from ShineView_His.dbo.BadLog WHERE  StateCode=4 ";
            string sql5=" GROUP BY MineCode ,TypeCode,StateCode) as  d " +
                                " on d.MineCode=m.MineCode and d.TypeCode=s.TypeCode" +
                                " Left  Join " +
                                "( select MineCode , TypeCode ,  count(StateCode) SJYSCount  from ShineView_His.dbo.BadLog WHERE  StateCode=5 ";
            string sql6=" GROUP BY MineCode ,TypeCode,StateCode) as  e " +
                                 " on e.MineCode=m.MineCode and e.TypeCode=s.TypeCode" +
                                 " where s.IsEnabled=1  and 1=" + flag;



            if (!string.IsNullOrEmpty(minecode))
            {
                sql6 += " and m.MineCode ='" + minecode + "'";
            }

            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                sql1 += " and LastTime>='" + BegingTime + "'";
                sql2 += " and LastTime >='" + BegingTime + "'";
                sql3 += " and LastTime >='" + BegingTime + "'";
                sql4 += " and LastTime >='" + BegingTime + "'";
                sql5 += " and LastTime >='" + BegingTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                sql1 += " and LastTime <='" + EndTime + "'";
                sql2 += " and LastTime <='" + EndTime + "'";
                sql3 += " and LastTime <='" + EndTime + "'";
                sql4 += " and LastTime <='" + EndTime + "'";
                sql5 += " and LastTime <='" + EndTime + "'";
            }


            string wheredata = sql1 + sql2 + sql3 + sql4 + sql5 + sql6;
            //string wheredata = "select  m.MineCode ,m.SimpleName,case  s.TypeCode  when 1 then '安全监控' when 2 then '人员管理' end TypeCode ," +
            //        " isnull(WLZDCount,0) WLZDCount  ,isnull(CSYCCount ,0) CSYCCount ,isnull(TXZDCount,0) TXZDCount ,ISNULL(WLGZCount, 0) WLGZCount ,ISNULL(SJYSCount, 0)  SJYSCount " +
            //        "  from ShineView_Data.dbo.MineConfig  m " +
            //        " left join ShineView_Data.dbo.SystemConfig s on m.id=s.minecode  " +
            //        " left join " +
            //        " (select minecode ,count(statecode) WLZDCount ,TypeCode from ShineView_His.dbo.BadLog where statecode=1   ";
            //string sql1 = " GROUP BY minecode,TypeCode , statecode ) as T1 on  m.minecode = T1.minecode and s.TypeCode=T1.TypeCode " +
            //       " left join " +
            //       " (select minecode ,count(statecode) CSYCCount ,TypeCode from ShineView_His.dbo.BadLog where  statecode=2";
            //string sql2 = "  GROUP BY minecode,TypeCode , statecode ) as T2   on  m.minecode = T2.minecode and s.TypeCode=T2.TypeCode " +
            //                    " LEFT JOIN " +
            //                    " (select minecode ,count(statecode) TXZDCount ,TypeCode from ShineView_His.dbo.BadLog where  statecode=3  ";
            //string sql3 = "  GROUP BY minecode,TypeCode , statecode ) as T3 on  m.minecode = T3.minecode and s.TypeCode=T3.TypeCode " +
            //      " LEFT JOIN " +
            //      " (select minecode ,count(statecode) WLGZCount ,TypeCode from ShineView_His.dbo.BadLog where  statecode=4";
            //string sql4 = "  GROUP BY minecode,TypeCode , statecode ) as T4  on  m.minecode = T4.minecode and s.TypeCode=T4.TypeCode " +
            //        " LEFT JOIN " +
            //        " (select minecode ,count(statecode) SJYSCount ,TypeCode from ShineView_His.dbo.BadLog where  statecode=5 ";
            //string sql5 = "  GROUP BY minecode,TypeCode , statecode ) as T5 on  m.minecode = T5.minecode and s.TypeCode=T5.TypeCode  where 1=" + flag;


            //if (!string.IsNullOrEmpty(BegingTime.ToString()))
            //{
            //    wheredata += " and LastTime >='" + BegingTime + "'";
            //    sql1 += " and LastTime >='" + BegingTime + "'";
            //    sql2 += " and LastTime >='" + BegingTime + "'";
            //    sql3 += " and LastTime >='" + BegingTime + "'";
            //    sql4 += " and LastTime >='" + BegingTime + "'";

            //}


            //if (!string.IsNullOrEmpty(EndTime.ToString()))
            //{
            //    wheredata += " and LastTime <='" + EndTime + "'";
            //    sql1 += " and LastTime <='" + EndTime + "'";
            //    sql2 += " and LastTime <='" + EndTime + "'";
            //    sql3 += " and LastTime <='" + EndTime + "'";
            //    sql4 += " and LastTime <='" + EndTime + "'";

            //}

            //wheredata = wheredata + sql1 + sql2 + sql3 + sql4 + sql5;
           return ReturnTables(wheredata, wheredata, "simplename", "His");
        }


        public DataTableCollection GetData_GZTG(string MineCode,string SensorNum,string TypeName, DateTime BegingTime, DateTime EndTime, int flag)
        {
            string whereCount = "SELECT MineCode,SensorNum,Type FROM( select mc.MineCode ,mc.SensorNum,mc.Place,mc.Type from ShineView_Data.dbo.aqmc mc  " +
                                " union  all " +
                                " select  kc.MineCode ,kc.SensorNum,kc.Place ,kc.Type from ShineView_Data.dbo.AQKC kc ) AS A  where 1=" + flag;
            string sql1 = " select row_number() over (order by t.SensorNum asc) TmpID,SimpleName, T.MineCode ,T.SensorNum,T.Place ,TypeName,ISNULL(Time0_4, 0)Time0_4,isnull(Time4_8,0) Time4_8, isnull( Time8_12,0) Time8_12, isnull(Time12_16,0) Time12_16 ,isnull( Time16_20,0) Time16_20,isnull(Time20_24,0) Time20_24,  ISNULL(Total,0) Total   from (" +
           " select mc.MineCode ,mc.SensorNum,mc.Place,mc.Type from ShineView_Data.dbo.aqmc mc " +
                           " union  all "+
                           " select  kc.MineCode ,kc.SensorNum,kc.Place ,kc.Type from ShineView_Data.dbo.AQKC kc ) as T"+
                           " left join ShineView_Data.dbo.DeviceType dt on T.Type = dt.TypeCode LEFT JOIN MineConfig m on T.MineCode = m.MineCode "+
                           " left JOIN  "+
                       " (select MineCode,SensorNum,Place ,Type,Count(DATENAME(hour, HitchDatetime)) Time0_4 "+
                       "  from ShineView_His.dbo.AQGZ  "+
                       "  where  DATENAME(hour, HitchDatetime)>=0 and DATENAME(hour, HitchDatetime)<4 ";
            string sql2= "   group by MineCode ,SensorNum,Place ,Type )  AS T0_4 on T0_4.MineCode = T.MineCode and T0_4.SensorNum=T.SensorNum  "+
                       " left join  "+
                       "  (select MineCode,SensorNum,Place ,Type,Count(DATENAME(hour, HitchDatetime)) Time4_8  from ShineView_His.dbo.AQGZ   "+
                       " where  DATENAME(hour, HitchDatetime)>=4 and DATENAME(hour, HitchDatetime)<8     ";
            string sql3= " group by MineCode ,SensorNum,Place ,Type )  AS   T4_8 on T4_8.MineCode =T.MineCode and T4_8.SensorNum=T.SensorNum "+
                       "   left join "+
                       "    (select MineCode,SensorNum,Place ,Type,Count(DATENAME(hour, HitchDatetime)) Time8_12  from ShineView_His.dbo.AQGZ  "+
                       "  where  DATENAME(hour, HitchDatetime)>=8 and DATENAME(hour, HitchDatetime)<12    ";
            string sql4 = "   group by MineCode ,SensorNum,Place ,Type )  AS   T8_12  on T8_12.MineCode = T.MineCode and T8_12.SensorNum=T.SensorNum  " +
                       " left join     " +
                       "  (select MineCode,SensorNum,Place ,Type,Count(DATENAME(hour, HitchDatetime)) Time12_16  from ShineView_His.dbo.AQGZ  " +
                       "    where  DATENAME(hour, HitchDatetime)>=12 and DATENAME(hour, HitchDatetime)<16    ";
                  string sql5=     "   group by MineCode ,SensorNum,Place ,Type )  AS   T12_16 on T12_16.MineCode = T.MineCode and T12_16.SensorNum=T.SensorNum  "+
                       "  left join "+
                       "   (select MineCode,SensorNum,Place ,Type,Count(DATENAME(hour, HitchDatetime)) Time16_20 from ShineView_His.dbo.AQGZ  "+
                        " where  DATENAME(hour, HitchDatetime)>=16 and DATENAME(hour, HitchDatetime)<20    ";
            string sql6 = " group by MineCode ,SensorNum,Place ,Type )  AS   T16_20   on T16_20.MineCode = T.MineCode and T16_20.SensorNum=T.SensorNum  " +
                         "   left join    (select MineCode,SensorNum,Place ,Type,Count(DATENAME(hour, HitchDatetime)) Time20_24 from ShineView_His.dbo.AQGZ " +
                         "   where  DATENAME(hour, HitchDatetime)>=20 and DATENAME(hour, HitchDatetime)<24    ";
            string sql7= "  group by MineCode ,SensorNum,Place ,Type )  AS   T20_24 on T20_24.MineCode = T.MineCode and T20_24.SensorNum=T.SensorNum  "+
                       " left JOIN  (select MineCode ,SensorNum, count(SensorNum) Total from ShineView_His.dbo.AQGZ   "+
                       "  where 1=1 ";
            string sql9 = " group by MineCode ,SensorNum )  " +
                      " as T_total " +
                       " on T_total.MineCode = T.MineCode and T.SensorNum =   T_total.SensorNum  where 1="+flag;

            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {

                sql1 += " and  HitchDatetime >='" + BegingTime + "'";
                sql2 += " and  HitchDatetime >='" + BegingTime + "'";
                sql3 += " and  HitchDatetime >='" + BegingTime + "'";
                sql4 += " and  HitchDatetime >='" + BegingTime + "'";
                sql5 += " and  HitchDatetime >='" + BegingTime + "'";
                sql6 += " and  HitchDatetime >='" + BegingTime + "'";
                sql7 += " and  HitchDatetime >='" + BegingTime + "'";
                //whereCount += " and  HitchDatetime >='" + BegingTime + "'";
                //sql7 += " and  HitchDatetime >='" + BegingTime + "'";
            }


            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {

                sql1 += " and  HitchDatetime <='" + EndTime + "'";
                sql2 += " and  HitchDatetime <='" + EndTime + "'";
                sql3 += " and  HitchDatetime <='" + EndTime + "'";
                sql4 += " and  HitchDatetime <='" + EndTime + "'";
                sql5 += " and  HitchDatetime <='" + EndTime + "'";
                sql6 += " and  HitchDatetime <='" + EndTime + "'";
                sql7 += " and  HitchDatetime <='" + EndTime + "'";
                //whereCount += " and  HitchDatetime <='" + EndTime + "'";
                //sql7 += " and  HitchDatetime <='" + EndTime + "'";
            }

            if (!string.IsNullOrEmpty(MineCode))
            {
                sql9 += " and T.MineCode ='" + MineCode + "'";
                //sql8 += " and g.MineCode ='" + MineCode + "'";
                whereCount += " and MineCode ='" + MineCode + "'";
            }


            if (!string.IsNullOrEmpty(SensorNum))
            {
                string[] sensorCodes = SensorNum.Split(new char[] { ',' });
                for (int i = 0; i < sensorCodes.Count(); i++)
                {
                    if (i == 0)
                    {
                        sql9 += " and (T.SensorNum ='" + sensorCodes[i] + "'";

                        whereCount += " and ( SensorNum ='" + sensorCodes[i] + "'";
                    }

                    else
                    {
                        sql9 += " or T.SensorNum ='" + sensorCodes[i] + "'";

                        whereCount += " or  SensorNum ='" + sensorCodes[i] + "'";

                    }
                    if (i == (sensorCodes.Count() - 1))
                    {
                        sql9 += ")";
                        whereCount += ")";
                    }


                }
            }

            if (!string.IsNullOrEmpty(TypeName))
            {
                sql9 += " and   T.Type ='" + TypeName + "'";

                whereCount += " and   Type ='" + TypeName + "'";

            }
           
              string whereData = sql1 + sql2 + sql3 + sql4 + sql5 + sql6 + sql7 + sql9;
            return ReturnTables(whereData, whereCount, "TmpID", "His");
        }


        public DataTableCollection GetData_MNLTG(string MineCode, string SensorNum, string TypeName, DateTime BeginTime, DateTime EndTime, int flag)
        {

            string whereDate = "select  Row_Number() over(  ORDER BY m.MineCode,m.SensorNum asc ) as TmpID,SimpleName, m.MineCode ,m.SensorNum,m.Type,d.TypeName,m.Place , " +
                               "isnull( m.AlarmHigh,0) AlarmHigh ,isnull(m.AlarmLower,0) AlarmLower,isnull(m.OutPowerHigh,0) OutPowerHigh,isnull(m.OutPowerLower,0) OutPowerLower," +
                               " isnull(max(t.StatisticaMaxValue),0) MaxValue,isnull(min(t.StatisticaMinValue),0) MinValue,isnull(AVG(t.StatisticaAvg),0) AvgValue " +
                               " from ShineView_Data.dbo.AQMC m " +
                               " left join(select *  from ShineView_His.dbo.aqmt where 1=1  ";
            string  sql1="   ) as t " +
                                " on t.MineCode = m.MineCode and t.SensorNum=m.SensorNum " +
                                " left  join ShineView_Data.dbo.DeviceType d " +
                                " on  d.TypeCode = m.Type " +
                                " left join MineConfig c " +
                                " on c.minecode =m.minecode  " +
                                " left join systemConfig sc " +
                                " on sc.minecode = c.ID " +
                                " where 1=" + flag + " and isenabled =1  and sc.typecode =1 ";
            string sql = " GROUP BY m.MineCode ,m.SensorNum,m.Type,d.TypeName,m.Place,AlarmHigh,AlarmLower,OutPowerHigh,OutPowerLower,SimpleName";
            string whereCount = "select Distinct m.MineCode ,m.SensorNum from ShineView_Data.dbo.AQMC m left join ShineView_His.dbo.AQMT g on g.MineCode = m.MineCode and g.SensorNum=m.SensorNum  where 1=" + flag;

               if (!string.IsNullOrEmpty(MineCode))
               {
                   sql1 += " and m.MineCode ='" + MineCode + "'";
                   whereCount += " and m.MineCode ='" + MineCode + "'";
               }
               if (!string.IsNullOrEmpty(TypeName))
               {
                   sql1 += " and m.Type='" + TypeName + "'";
                   whereCount += " and m.Type='" + TypeName + "'";
               }
               if (!string.IsNullOrEmpty(SensorNum))
               { 
                    string[] SensorCodes = SensorNum.Split(new  char[] {','});
                    for (int i = 0; i < SensorCodes.Count(); i++)
                    {
                        if (i == 0)
                        {
                            sql1 += " and (m.SensorNum='" + SensorCodes[i] + "'";
                            whereCount += " and (m.SensorNum='" + SensorCodes[i] + "'";
                        }
                        else
                        {
                            sql1 += " or m.SensorNum='" + SensorCodes[i] + "'";
                            whereCount += " or m.SensorNum='" + SensorCodes[i] + "'";
                        }
                        if (i==(SensorCodes.Count()-1))
                        {
                            sql1 += " )";
                            whereCount += " )";
                        }
                    }
               }
               whereDate += "and StatisticalTime>='" + BeginTime + "'";

               whereDate += "and StatisticalTime<='" + EndTime + "'";

               whereDate = whereDate + sql1 + sql;

              return ReturnTables(whereDate, whereCount, "TmpID", "His");
        }
        public DataTableCollection GetData_BJTG(string MineCode, string SensorNum, string TypeName, DateTime BeginTime, DateTime EndTime, int flag,string TypeKind)
        {
            string whereCount = "select  DISTINCT  b.MineCode,b.sensornum ,b.Type from ShineView_His.dbo.aqbj b RIGHT JOIN ShineView_Data.dbo.MineConfig m on m.minecode = b.minecode  "+
                       " left join ShineView_Data.dbo.systemConfig s on s.minecode =m.id  "+
                       " left join ShineView_Data.dbo.DeviceType d on d.TypeCode = b.Type"+
                       " where s.isenabled=1 and s.typecode =1 and 1="+flag;
            string sql1 = "select Row_Number() over (order by b.MineCode asc) as TmpID, b.MineCode ,m.SimpleName ,b.SensorNum,count(b.SensorNum) Counts,b.Place,d.TypeName ," +
                       " (case d.Type when 'A' then '模拟量'  when  'C' then '控制量' when 'D' then '开关量' when 'F' then  '分站/设备' when 'L' then '累计量' when 'U' then '抽象出的逻辑量' end)  Type," +
                       " Sum(DATEDIFF(s, PoliceDateTime, PoliceEndDateTime)) Continuous_Tmp ," +
                       " Max(DATEDIFF(s, PoliceDateTime, PoliceEndDateTime)) MaxTime_Tmp," +
                       " Min(DATEDIFF(s, PoliceDateTime, PoliceEndDateTime)) MinTime_Tmp," +
                       " AVG((DATEDIFF(s, PoliceDateTime, PoliceEndDateTime))) AvgTime_Tmp," +
                       " cast(null as varchar(50))  Continuous," +
                       " cast(null as varchar(50))  MaxTime," +
                       " cast(null as varchar(50))  MinTime," +
                       " cast(null as varchar(50)) AvgTime," +
                       " MAX(MaxValue) MaxValue,min(MinValue) MinValue " +
                       " from ShineView_His.dbo.AQBJ  b " +
                       " left join  ShineView_Data.dbo.MineConfig m " +
                       " on m.MineCode = b.MineCode " +
                       " left join  ShineView_Data.dbo.DeviceType  d  " +
                       " on b.Type=d.TypeCode LEFT JOIN ShineView_Data.dbo.SystemConfig s "+
                        " on s.minecode =m.id  "+
                        " where s.isenabled=1 and s.typecode =1 and 1=" + flag;
            string sql2 = " group by  b.MineCode ,m.SimpleName ,b.SensorNum,b.Place,d.TypeName ,d.Type ";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql1 += "  and b.MineCode='" + MineCode + "'";
                whereCount += "  and b.MineCode='" + MineCode + "'";
            }
            #region SensorNum
            if (!string.IsNullOrEmpty(SensorNum))
            {
                string[] sensorCodes = SensorNum.Split(new char[] { ',' });
                for (int i = 0; i < sensorCodes.Count(); i++)
                {
                    if (i == 0)
                    {
                        sql1 += " and (b.SensorNum='" + sensorCodes[i] + "'";
                        whereCount += "and (b.SensorNum='" + sensorCodes[i] + "'";
                    }

                    else
                    {
                        sql1 += " or b.SensorNum='" + sensorCodes[i] + "'";
                        whereCount += " or b.SensorNum='" + sensorCodes[i] + "'";

                    }
                    if (i == (sensorCodes.Count() - 1))
                    {
                        sql1 += ")";
                        whereCount += ")";
                    }
                }

            }
            #endregion

            if (!string.IsNullOrEmpty(TypeName))
            {
                sql1 += "and b.Type='" + TypeName + "'";
                whereCount += " and b.Type='" + TypeName + "'";
            }
            if (!string.IsNullOrEmpty(BeginTime.ToString()))
            {
                sql1 += " and b.PoliceDatetime>='" + BeginTime + "'";
                whereCount += " and b.PoliceDatetime >='" + BeginTime + "'";
            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                sql1 += " and b.PoliceEndDateTime<='" + EndTime + "'";
                whereCount += " and b.PoliceEndDateTime<='" + EndTime + "'";
            }
            if (!string.IsNullOrEmpty(TypeKind))
            {
                sql1 += " and d.Type='" + TypeKind + "'";
                whereCount += " and d.Type='" + TypeKind + "'";
            }


            string whereDate = sql1 + sql2;


            return ReturnTables(whereDate, whereCount, "TmpID", "His");
        }
        /// <summary>
        /// 历史通讯异常统计
        /// </summary>
        /// <param name="minecode"></param>
        /// <param name="BegingTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public DataTableCollection GetData_TXYCTG(string minecode, DateTime BegingTime, DateTime EndTime, int flag)
        {
            string wheredata = "select  Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,s.MineCode , isnull(Counts,0) Counts,isnull(LongestTimes,0) LongestTimes,isnull(ShortestTimes,0) ShortestTimes,isnull(SumTimes,0) SumTimes,LongestTime,ShortestTime,SumTime ,  " +
"  case s.typecode   when 1 then '安全监控' when 2 then '人员管理' when 5 then '矿山压力' when 7 then '火灾束管' END JKXT   " +
"  from ShineView_Data.dbo.SystemConfig s    left join ShineView_Data.dbo.MineConfig m on m.id = s.minecode  " +
" left join (select minecode ,count(typecode) Counts, typecode,max(DATEDIFF(second , BeginTime, EndTime) ) LongestTimes,min(DATEDIFF(second , BeginTime, EndTime) ) ShortestTimes,sum(DATEDIFF(second , BeginTime, EndTime) ) SumTimes ,     " +
" cast(NULL as  varchar(50)) LongestTime,  cast(NULL as  varchar(50)) ShortestTime,  cast(NULL as  varchar(50)) SumTime  " +
" FROM    ShineView_His.dbo.BadCreate b   where 1=1    ";


            if (!string.IsNullOrEmpty(BegingTime.ToString()))
            {
                wheredata += " and BeginTime>='" + BegingTime + "'";

            }
            if (!string.IsNullOrEmpty(EndTime.ToString()))
            {
                wheredata += " and BeginTime<='" + EndTime + "'";

            }
            wheredata += "GROUP BY minecode ,typecode) as a on m.minecode =a.minecode  and s.typecode = a.typecode  where isenabled =1 and  1=" + flag;
            if (!string.IsNullOrEmpty(minecode))
            {
                wheredata += " and m.minecode ='" + minecode+"'";
            }

            return ReturnTables(wheredata, wheredata, "TmpID", "His");
        }

        //GetData_TXZDHis
        /// <summary>
        /// 历史断电查询
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        /// 2015-2-4 修改记录：添加o.
        public DataTableCollection GetHisAQDD(string minecode, string devname, string sensorNum, DateTime BegingTime, DateTime EndTime, string systemType)
        {
            string where = "";
            string wherecount = "select * from ShineView_His.dbo.AQDD where 1=1 ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            if (minecode != "" && minecode != null)
            {
                where = "Minecode='" + minecode + "' and ";
                wherecount += " and MineCode='" + minecode + "' ";
            }
            if (devname != "" && devname != null)
            {
                where += " Type='" + devname + "' and ";
                wherecount += "and Type='" + devname + "'";
            }

            if (systemType != "" && systemType != null)
            {
                where += " SystemType='" + systemType + "' and";
                wherecount += " and SystemType='" + systemType + "'";
            }

            if (sensorNum != "" && sensorNum != null)
            {
                where += " sensorNum in ('" + sensorNum.Replace(",", "','") + "') and ";
                wherecount += " and sensorNum in ('" + sensorNum.Replace(",", "','") + "')";
            }

            where += " PowerDatetime>='" + BegingTime + "' and PowerDatetime<='" + EndTime + "' and PowerEndDatetime IS NOT NULL";
            wherecount += " and PowerDatetime>='" + BegingTime + "' and PowerDatetime<='" + EndTime + "' and PowerEndDatetime IS NOT NULL";
            // string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,SensorNum,d.TypeName,d.Unit,Place,(case PowerType when 0 then '上限断电' when 1 then '下限断电' else '开关量断电' end) as powertype,PowerDatetime,PowerEndDatetime,[dbo].[FunConvertTime](datediff(second, PowerDatetime,PowerEndDatetime)) as continuoustime,Max,MaxTime,Min,MinTime,Avg,RelevanceDepict,Measures,(case PowerDepit when 0 then '放炮' when 1 then '停电' when 2 then '割煤' when 3 then '采空区来压' when 4 then '调试' when 5 then '通风' when 6 then '发火' when 7 then '断线断电' else '其他' end) as powerwhy from ShineView_His.dbo.AQDD o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where " + where + "";

            string wheredata = string.Format(@"
            SELECT Row_Number() over (order by getdate() asc) as TmpID,H.MineCode,M.SimpleName,H.SensorNum,H.Type,d.TypeName,d.Unit,
            Place,PowerDatetime , PowerEndDatetime, continuoustime,[max],[maxTime],powertype, powerwhy,Measures FROM 
            (SELECT MineCode,Sensornum,place,[type],PowerDatetime,PowerEndDatetime,
            shineview_data.[dbo].[FunConvertTime](datediff(second, PowerDatetime,PowerEndDatetime)) as continuoustime,
            [max],[maxTime],(case PowerType when 0 then '上限断电' when 1 then '下限断电' else '开关量断电' end) as powertype,
            (case PowerDepit when 0 then '放炮' when 1 then '停电' when 2 then '割煤' when 3 then '采空区来压' when 4 then '调试' when 5 then '通风' when 6 then '发火' when 7 then '断线断电' else '其他' end) as powerwhy,
            Measures 
            FROM shineview_his.dbo.aqdd  WHERE {0}) AS H 
            LEFT JOIN shineview_data.dbo.DeviceType D ON H.type=d.typecode 
            left join shineview_data.dbo.MineConfig M on M.MineCode=H.MineCode  ", where);
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 历史故障查询
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        /// 2015-2-3 修改：添加o.
        /// 2016-7-8 修改，添加 system Type;
        public DataTableCollection GetHisAQGZ(string minecode, string devname, string sensorNum, DateTime BegingTime, DateTime EndTime, string systemType)
        {
            string where = "";
            string wherecount = "select * from ShineView_His.dbo.AQGZ where 1=1 ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            if (minecode != "" && minecode != null)
            {
                where = "Minecode='" + minecode + "' and ";
                wherecount += " and MineCode='" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                where += " type='" + devname + "' and ";
                wherecount += " and type='" + devname + "' ";
            }
            if (systemType != "" && systemType != null)
            {
                where += " SystemType='" + systemType + "' and";
                wherecount += " and SystemType='" + systemType + "'";
            }

            if (sensorNum != "" && sensorNum != null)
            {
                where += " sensorNum in ('" + sensorNum.Replace(",", "','") + "') and ";
                wherecount += " and sensorNum in ('" + sensorNum.Replace(",", "','") + "')";
            }



            where += " HitchDatetime>='" + BegingTime + "' and HitchDatetime<='" + EndTime + "'";
            wherecount += "and HitchDatetime>='" + BegingTime + "' and HitchDatetime<='" + EndTime + "' and HitchEndDatetime  is not null";
            // string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,SensorNum,d.TypeName,d.Unit,Place,HitchDatetime,HitchEndDatetime,[dbo].[FunConvertTime](datediff(second, HitchDatetime,HitchEndDatetime)) as continuoustime,RelevanceDepict,measures,(case HitchDepict when 0 then '通信故障' when 1 then '设备故障' when 2 then '调试' else '其他' end) as badwhy from ShineView_His.dbo.AQGZ o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where " + where + " ";
            string wheredata = string.Format(@"
                 SELECT   Row_Number() over (order by getdate() asc) as TmpID,H.MineCode,M.SimpleName,SensorNum,H.[Type],D.typeName,place,D.unit, RelevanceDepict,
                 HitchDatetime,HitchEndDatetime,continuoustime,badwhy,measures FROM
                 (SELECT MineCode, SensorNum, Place, RelevanceDepict,Type, HitchDatetime, HitchEndDatetime,shineView_data.[dbo].[FunConvertTime]
                (datediff(second, HitchDatetime,HitchEndDatetime)) as continuoustime,
                (case HitchDepict when 0 then '通信故障' when 1 then '设备故障' when 2 then '调试' else '其他' end) as badwhy,
                measures
                  FROM shineView_His.dbo.AQGZ WHERE {0} ) H
                  LEFT JOIN Shineview_Data.dbo.DeviceType D on H.Type=d.TypeCode 
                  left join Shineview_Data.dbo.MineConfig M on H.MineCode=M.MineCode 
                ", where);
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 历史馈电异常查询
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        /// 2015-2-5 修改记录：添加o.
        public DataTableCollection GetHisAQYC(string minecode, string devname, string sensorNum, DateTime BegingTime, DateTime EndTime)
        {
            string where = "";
            string wherecount = "select * from ShineView_His.dbo.AQYC where 1=1 ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            if (minecode != "" && minecode != null)
            {
                where = "Minecode='" + minecode + "' and ";
                wherecount += " and MineCode='" + minecode + "'";
            }
            if (devname != "" && devname != null)
            {
                where += "type='" + devname + "' and ";
                wherecount += "and type='" + devname + "'";
            }
            if (sensorNum != "" && sensorNum != null)
            {
                where += " sensorNum in ('" + sensorNum.Replace(",", "','") + "') and ";
                wherecount += " and sensorNum in ('" + sensorNum.Replace(",", "','") + "')";
            }
            where += "AbnormalDateTime>='" + BegingTime + "' and AbnormalDateTime<='" + EndTime + "'  and AbnormalEndDateTime IS NOT NULL";
            wherecount += " and AbnormalDateTime>='" + BegingTime + "' and AbnormalDateTime<='" + EndTime + "' and AbnormalEndDateTime IS NOT NULL";
            // string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,SensorNum,d.TypeName,d.Unit,Place,AbnormalDateTime,AbnormalEndDateTime,[dbo].[FunConvertTime](datediff(second, AbnormalDateTime,AbnormalEndDateTime)) as continuoustime,DataValue,AlarmDateTime,PowerDatetime,PowerArea,AbnormalStatus,AbnormalDepict,measures from ShineView_His.dbo.AQYC o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where " + where + " ";
            string wheredata = string.Format(@"
                SELECT Row_Number() over (order by getdate() asc) as TmpID,H.MineCode,H.SensorNum,H.Type,M.SimpleName,D.TypeName,H.place,
                AbnormalDateTime,AbnormalEndDateTime,continuoustime,AbnormalStatus,AbnormalDepict,measures
                 FROM 
                (
                SELECT MineCode, SensorNum, Type,place, AbnormalDateTime, AbnormalEndDateTime, 
                shineview_data.[dbo].[FunConvertTime](datediff(second, AbnormalDateTime,AbnormalEndDateTime)) as continuoustime,
                 AbnormalStatus, AbnormalDepict, measures
                FROM ShineView_His.dbo.AQYC WHERE {0} ) AS H

                 left join shineview_data.[dbo].DeviceType D on H.Type=D.TypeCode 
                left join shineview_data.[dbo].MineConfig M on H.MineCode=M.MineCode 
                ", where);
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 累计量统计查询
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        public DataTableCollection GetHisAQLT(string minecode, string devname, DateTime BegingTime, DateTime EndTime)
        {
            string where = "";
            string wherecount = "select * from [ShineView_His].[dbo].AQLT where ";
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            if (minecode != "" && minecode != null)
            {
                where = "Minecode='" + minecode + "' and ";
                wherecount += " MineCode='" + minecode + "' ";
            }
            if (devname != "" && devname != null)
            {
                where += " TypeName='" + devname + "' and ";
                wherecount += "and TypeName='" + devname + "'";
            }
            where += "EndTime>='" + BegingTime + "' and EndTime<='" + EndTime + "'";
            wherecount += " and EndTime>='" + BegingTime + "' and EndTime<='" + EndTime + "'";
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,SensorNum,d.TypeName,d.Unit,Place,StatTime,EndTime,Value,Interval from ShineView_His.dbo.AQLT o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode where " + where + "";
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 加载设备类型
        /// </summary>
        /// <param name="MineCode"></param>
        /// <returns></returns>
        public DataTable GetDevTypeList(string MineCode,string TypeKind)
        {
            string where = " and   1=1 ";
            if (!string.IsNullOrEmpty(TypeKind))
            {
                where = " and  Type='" + TypeKind + "'";
            }
            string sql = "select * from DeviceType where typecode in(select Type from AQMC where minecode='" + MineCode + "' union select Type from AQKC where minecode='" + MineCode + "')"+where+" order by typecode";
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 统计每天开关量状态
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="devname">设备名称</param>
        /// <param name="BegingTime">【必选】开始时间</param>
        /// <param name="EndTime">【必选】结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetAQKGLData_Day(string minecode, string devname, DateTime BegingTime, DateTime EndTime)
        {
            DataTableCollection dtc;
            SqlParameter[] sqls = new SqlParameter[6];
            if (string.IsNullOrEmpty(minecode))
            {
                minecode = "";
            }
            if (string.IsNullOrEmpty(devname))
            {
                devname = "";
            }
            sqls[0] = new SqlParameter("@pageNum", PageIndex);//查询页索引
            sqls[1] = new SqlParameter("@pageSize", PageSize);//查询页大小
            sqls[2] = new SqlParameter("@MineCode", minecode);
            sqls[3] = new SqlParameter("@DevName", devname);
            sqls[4] = new SqlParameter("@BegingTime", BegingTime);
            sqls[5] = new SqlParameter("@EndTime", EndTime);

            dtc = dal.ReturnDTS_ExcutePro_KGLDay(sqls);
            return dtc;
        }

        public DataTableCollection GetAQKGLData_Week(string minecode, string devname, DateTime BegingTime, DateTime EndTime)
        {
            DataTableCollection dtc;
            SqlParameter[] sqls = new SqlParameter[6];
            if (string.IsNullOrEmpty(minecode))
            {
                minecode = "";
            }
            if (string.IsNullOrEmpty(devname))
            {
                devname = "";
            }
            sqls[0] = new SqlParameter("@pageNum", PageIndex);//查询页索引
            sqls[1] = new SqlParameter("@pageSize", PageSize);//查询页大小
            sqls[2] = new SqlParameter("@MineCode", minecode);
            sqls[3] = new SqlParameter("@DevName", devname);
            sqls[4] = new SqlParameter("@BegingTime", BegingTime);
            sqls[5] = new SqlParameter("@EndTime", EndTime);

            dtc = dal.ReturnDTS_ExcutePro_KGLWeek(sqls);
            return dtc;
        }

        /// <summary>
        /// 模拟量历史统计分钟及日 （张大臣 0815）
        /// </summary>
        /// <param name="minecode"></param>
        /// <param name="devname"></param>
        /// <param name="SensorNum"></param>
        /// <param name="BegingTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="VIEW"></param>
        /// <returns></returns>
        public DataTableCollection GetData_AQMNL(string minecode, string devname, string SensorNum, DateTime BegingTime, DateTime EndTime, TransJsonToTreeListModel.EnumDataType VIEW, string systemType)
        {
            string where = "";
            string wheredata = "";
            string wherecount = "";
            string sql = "";

            switch (VIEW)
            {
                case TransJsonToTreeListModel.EnumDataType.AQMNL_1M:
                    {
                        where = "";
                        //wherecount = "select * from shineview_His.[dbo].[Report_Day]  where 1=1 ";
                        if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
                        {
                            return null;
                        }
                        if (minecode != "" && minecode != null)
                        {
                            where = "minecode='" + minecode + "' and ";

                        }
                        if (devname != "" && devname != null)
                        {
                            where += "Type='" + devname + "' and ";

                        }
                        if (SensorNum != "" && SensorNum != null)
                        {
                            where += " sensornum in ('" + SensorNum.Replace(",", "','") + "') and ";
                        }
                        if (systemType != "" && systemType != null)
                        {
                            systemType += "systemType='" + systemType + "' and ";
                        }
                        where += "StatisticalTime>='" + BegingTime + "' and StatisticalTime<='" + EndTime + "'";
                        wherecount = string.Format(@"select count(1) from ShineView_His.dbo.AQMT where {0} ", where);
                        wheredata = string.Format(@"select mc.SimpleName,t.*,dt.TypeName from (select ID,MineCode,sensorNum,[Type],StatisticaMaxValue MaxVal,Place,StatisticaMaxDatetime,
                                                    StatisticaMinValue MinVal,StatisticaMinDatetime,StatisticaAvg AvgVal,StatisticalTime DectTime 
                                                    from ShineView_His.dbo.AQMT where {0} 
                                                    order by ID 
                                                    offset {1} rows fetch next {2} rows only) as t 
                                                    left join MineConfig mc on t.MineCode=mc.MineCode 
                                                    left join DeviceType dt on t.Type=dt.TypeCode
                                                    ", where, pageIndex * pageSize, pageSize);

                        //return dal.ReturnDs(wheredata).Tables;



                        // string sql = @"select * from (select ROW_NUMBER() over (order by id) as rowID, * from ShineView_His.dbo.AQMT where StatisticalTime>='2015-08-10' and StatisticalTime<'2015-08-14') as xx order by  rowID offset  "+pageIndex*pageSize+" row fetch next "+pageSize+" rows only";"+pageIndex*pageSize+" "+pageSize+"

                        //
                        //select * from (select ROW_NUMBER() over (order by id) as rowID, * from ShineView_His.dbo.AQMT where StatisticalTime>='2015-08-10' and StatisticalTime<'2015-08-14') as xx order by  rowID offset  60000 row fetch next 300 rows only

                        sql = wherecount + " " + wheredata;
                        break;
                    }
                case TransJsonToTreeListModel.EnumDataType.AQMNL_3M:
                    {

                        break;
                    }
                case TransJsonToTreeListModel.EnumDataType.AQMNL_5M:
                    {

                        break;
                    }

                case TransJsonToTreeListModel.EnumDataType.AQMNL_1D:
                    {
                        where = "";
                        //wherecount = "select * from shineview_His.[dbo].[Report_Day]  where 1=1 ";
                        if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
                        {
                            return null;
                        }
                        if (minecode != "" && minecode != null)
                        {
                            where = "煤矿编号='" + minecode + "' and ";

                        }
                        if (devname != "" && devname != null)
                        {
                            where += "设备名 in (select typeName from devicetype where typecode= '" + devname + "') and ";

                        }
                        if (SensorNum != "" && SensorNum != null)
                        {
                            where += " 测点编号 in ('" + SensorNum.Replace(",", "','") + "') and ";

                        }
                        where += "SubmitTime>='" + BegingTime + "' and SubmitTime<='" + EndTime + "' and 最大值时间 is not null ";
                        wherecount = string.Format(@"select count(1) from shineview_His.[dbo].[Report_Day] where {0}", where);
                        wheredata = string.Format(@"select D.TypeCode,* from (select 煤矿编号,煤矿名称,测点编号,设备名,安装位置,最大值,最大值时间,报警值,断电值,平均值,SubmitTime" +
                                    " from shineview_His.[dbo].[Report_Day] where {0} order by SubmitTime offset " + (pageIndex * pageSize).ToString() + " row fetch next " + pageSize.ToString() + " rows only) as R " +
                                    "left join ShineView_data.dbo.DeviceType D on R.设备名=D.TypeName ", where);
                        sql = wherecount + " " + wheredata;
                        break;
                    }
                case TransJsonToTreeListModel.EnumDataType.AQMNL_30D:
                    {
                        break;
                    }

            }

            return dal.ReturnDs(sql).Tables;

            //return ReturnTables(wheredata, wherecount, "TmpID", "His"); ;

            #region [魏源码]

            //int ts = 1;
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.AQMNL_3M)
            //    ts = 3;
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.AQMNL_5M)
            //    ts = 5;
            //if (VIEW == TransJsonToTreeListModel.EnumDataType.AQMNL_30D)
            //    ts = 30;




            //if (VIEW == TransJsonToTreeListModel.EnumDataType.AQMNL_1D ||
            //    VIEW == TransJsonToTreeListModel.EnumDataType.AQMNL_30D)
            //{
            //    SqlParameter[] sqls = new SqlParameter[7];
            //    sqls[0] = new SqlParameter("@pageNum", PageIndex);//查询页索引
            //    sqls[1] = new SqlParameter("@pageSize", PageSize);//查询页大小
            //    sqls[2] = new SqlParameter("@MineCode", minecode);
            //    sqls[3] = new SqlParameter("@DevName", devname);
            //    sqls[4] = new SqlParameter("@BegingTime", BegingTime);
            //    sqls[5] = new SqlParameter("@EndTime", EndTime);
            //    sqls[6] = new SqlParameter("@TimeSpan", ts);
            //    dtc = dal.ReturnDTS_ExcutePro_MNLDay(sqls);
            //}
            //else
            //{
            //    SqlParameter[] sqls = new SqlParameter[8];
            //    sqls[0] = new SqlParameter("@pageNum", PageIndex);//查询页索引
            //    sqls[1] = new SqlParameter("@pageSize", PageSize);//查询页大小
            //    sqls[2] = new SqlParameter("@MineCode", minecode);
            //    sqls[3] = new SqlParameter("@DevName", devname);
            //    sqls[4] = new SqlParameter("@BegingTime", BegingTime);
            //    sqls[5] = new SqlParameter("@EndTime", EndTime);
            //    sqls[6] = new SqlParameter("@TimeSpan", ts);
            //    sqls[7] = new SqlParameter("SensorNum", SensorNum);
            //    dtc = dal.ReturnDTS_ExcutePro_MNLMinute(sqls);
            //}
            #endregion

        }


        #endregion

        #region[人员管理部分]

        #region [ GetSelectString ]

        /// <summary>
        /// 获取查询字符串
        /// </summary>
        /// <param name="strTest">例如，{"BlockAddress", "=", inputBlockID.Value, "string","1"},最后一个“1”表示精确查询，‘0’表示模糊查询</param>
        /// <returns></returns>
        public string GetSelectString(string[,] strTest)
        {
            string strNewSql = string.Empty;
            bool blnFirst = true;
            for (int i = 0; i < strTest.GetUpperBound(0) + 1; i++)
            {
                if (!string.IsNullOrEmpty(strTest[i, 2].Trim()))
                {
                    bool bSelectExact = strTest[i, 4].Equals("1");
                    if (strTest[i, 3].Trim() == "string")
                    {
                        if (bSelectExact)
                        {
                            //精确
                            strTest[i, 2] = "'" + strTest[i, 2].Trim() + "'";
                        }
                        else
                        {
                            //模糊
                            strTest[i, 2] = "'%" + strTest[i, 2].Trim() + "%'";
                            strTest[i, 1] = "like";
                        }
                    }

                    if (strTest[i, 3].Trim() == "datetime")
                    {
                        strTest[i, 2] = "'" + strTest[i, 2].Trim() + "'";
                    }


                    if (blnFirst)
                    {
                        strNewSql = strTest[i, 0].Trim() + " " + strTest[i, 1].Trim() + " " + strTest[i, 2].Trim();
                        blnFirst = false;
                    }
                    else
                    {
                        strNewSql += " and " + strTest[i, 0].Trim() + " " + strTest[i, 1].Trim() + " " + strTest[i, 2].Trim();
                    }
                }
            }
            return strNewSql;
        }

        #endregion

        /// <summary>
        /// 路线预设
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="cardcode">标识卡</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public DataTableCollection GetPathInfo(string minecode, string cardcode, string name)
        {

            minecode = string.IsNullOrEmpty(minecode) ? "" : minecode;
            cardcode = string.IsNullOrEmpty(cardcode) ? "" : cardcode;
            name = string.IsNullOrEmpty(name) ? "" : name;
            string strWhere = "";
            string[,] strarr = new string[3, 5]
                {
                    {"o.MineCode", "=", minecode, "string", "1"},
                    {"JobCardCode", "=", cardcode, "string", "1"},
                    {"Name", "=", name, "string", "0"},
                };

            strWhere = GetSelectString(strarr);
            strWhere = @"select Row_Number() over (order by o.Time asc) as TmpID,o.MineCode,SimpleName,o.JobCardCode,o.Name,Position,StationsList,o.Time from RYLXYS o
Left Join MineConfig g on o.MineCode=g.MineCode where " + (strWhere.Equals("") ? "1=1" : strWhere);

            string wherecount = "";
            string[,] strarrCount = new string[3, 5]
                {
                    {"MineCode", "=", minecode, "string", "1"},
                    {"JobCardCode", "=", cardcode, "string", "1"},
                    {"Name", "=", name, "string", "0"},
                };
            wherecount = GetSelectString(strarrCount);
            wherecount = "select * from RYLXYS o where  " + (wherecount.Equals("") ? "1=1" : wherecount);


            return ReturnTables(strWhere, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 下井时间不足统计
        /// </summary>
        /// <param name="MineCode">煤矿编号</param>
        /// <param name="Name">姓名</param>
        /// <param name="Job">职务</param>
        /// <param name="Department">部门</param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTableCollection GetXJSJBZTG(string MineCode, string Name, string Job, string Department, DateTime BeginTime, DateTime EndTime, string Timespan,int flag)
        {

            string sql1 = "   select  Row_Number() over (order by A.MineCode  asc) as TmpID, m.SimpleName,A.MineCode ,A.JobCardCode,A.[Position],A.Department,A.Name,cast( A.Ori as VARCHAR(50)) Ori ,cast(A.Total as VARCHAR(50)) Total ,cast(A.Before as VARCHAR(50)) Before  from (select MineCode ,SystemType,JobCardCode ,Name " +

                    " ,Position,Department, sum(WorkingHours) Ori ,sum(Duration) Total,(sum(WorkingHours)-sum(Duration)) Before "+
                    " from   ShineView_His.dbo.RYKQ  where 1="+flag;
      string sql2 = "  GROUP BY MineCode,SystemType,JobCardCode,Name " +

                    ",Department,Position  )  AS A " +
                    " left join  ShineView_Data.dbo.MineConfig m " +
                    " on m.MineCode = A.MineCode " +
                    " left JOIN ShineView_Data.dbo.SystemConfig sc " +
                    " on sc.MineCode = m.ID and sc.TypeCode  =A.SystemType " +
                    " where sc.IsEnabled=1";
      if (!string.IsNullOrEmpty(MineCode))
      {
          sql1 += " and MineCode ='" + MineCode + "'";
      
      }
      if (!string.IsNullOrEmpty(Name))
      {
          sql1 += " and Name like '%" + Name + "%'";
      }
      if (!string.IsNullOrEmpty(Job))
      {
          sql1 += " and Position ='" + Job + "'";
      }
      if (!string.IsNullOrEmpty(Department))
      {
          sql1 += " and Department='" + Department + "'";
      }

      if (!string.IsNullOrEmpty(BeginTime.ToString()))
      {
          sql1 += " and WorkingDate>='" + BeginTime + "'";
      }
      if (!string.IsNullOrEmpty(EndTime.ToString()))
      {
          sql1 += " and WorkingDate<='" + EndTime + "'";
      }
      if (!string.IsNullOrEmpty(Timespan))
      {
          sql2 += " and A.before>=" + Convert.ToInt32(Timespan);
      }
      else {
          sql2 += " and A.before>0";
      }
      string whereData = sql1 + sql2;
      return ReturnTables(whereData, whereData, "TmpID", "His");
        }
            
            
            /// <summary>
        /// 实时通信状态
        /// </summary>
        /// <param name="minecode"></param>
        /// <returns></returns>
        public DataTableCollection GetRealTXState(string minecode)
        {
            string where = " where B.isenabled=1 ";
            //string wherecount = "select * from MineConfig where ";
            if (!string.IsNullOrEmpty(minecode))
            {
                where += " and  o.Minecode='" + minecode + "' ";
                //wherecount += " minecode = '" + minecode + "'";
            }
            else
            {
                //wherecount += " 1=1 ";
            }

            string wheredata = @"select Row_Number() over (order by StateCode asc) as TmpID,o.MineCode,O.SimpleName,B.Name  systemName,case B.TypeCode when 1 then  '安全监控系统' when 2 then '人员管理系统' when 3 then '瓦斯抽放' else  '安全监控/瓦斯抽放' end as SyatemType,City,B.Phone,Case StateCode When 0 Then '正常' When 1 Then '通讯中断' When 2 Then '传输异常' End as StateCode,b.UpDateTime [TimeOut],Case StateCode When 0 Then '' else [dbo].[FunConvertTime](datediff(second,b.UpdateTime,getdate())) End as  ContinueTime from SystemConfig B Left Join MineConfig o   on o.ID = B.MineCode  " + where;
            string wherecount = @"select Row_Number() over (order by StateCode asc) as TmpID,o.MineCode,O.SimpleName,B.Name  systemName,case B.TypeCode when 1 then  '安全监控系统' when 2 then '人员管理系统' when 3 then '瓦斯抽放' else  '安全监控/瓦斯抽放' end as SyatemType,City,B.Phone,Case StateCode When 0 Then '正常' When 1 Then '通讯中断' When 2 Then '传输异常' End as StateCode,b.UpDateTime [TimeOut],Case StateCode When 0 Then '' else [dbo].[FunConvertTime](datediff(second,b.UpdateTime,getdate())) End as  ContinueTime from SystemConfig B  Left Join MineConfig o   on o.ID = B.MineCode " + where;
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 历史通信故障信息
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetHisTXState(string mineCode, DateTime BegingTime, DateTime EndTime)
        {
            string where = "";
            string wherecount = "";
            string[,] strarr = new string[3, 5]
                {
                    {"o.MineCode", "=", mineCode, "string", "1"},
                    {"LastTime", ">=", BegingTime.ToString(), "string", "1"},
                    {"LastTime", "<=", EndTime.ToString(), "string", "1"},
                };
            where = GetSelectString(strarr);

            string[,] strarrCount = new string[3, 5]
                {
                    {"MineCode", "=", mineCode, "string", "1"},
                    {"LastTime", ">=", BegingTime.ToString(), "string", "1"},
                    {"LastTime", "<=", EndTime.ToString(), "string", "1"},
                };

            wherecount = GetSelectString(strarrCount);
            wherecount = "select * from ShineView_His.dbo.BadLog where " + (wherecount.Equals("") ? "1=1" : wherecount);

            string wheredata = @"select Row_Number() over (order by LastTime asc) as TmpID,o.MineCode,SimpleName,case TypeCode when 1 then '安全监控' when 2 then '人员管理' when 3 then '瓦斯抽放' else '安全监控/瓦斯抽放' end as SystemTypeName,Case StateCode When 0 Then '正常' When 1 Then '通讯中断' When 2 Then '传输异常' End StateCode,LastTime StartTime,dateadd(second,timeout,LastTime) EndTime,dbo.FunConvertTime(Continuous) Continuous  from ShineView_His.dbo.BadLog B
Left Join  MineConfig o on o.MineCode = b.MineCode where " + (where.Equals("") ? "1=1" : where);

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }


        /// <summary>
        /// 历史通信故障信息统计
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetHisTXStateTJ(string mineCode, DateTime BegingTime, DateTime EndTime)
        {
            //string where = "";
            string wherecount = "";
            //string[,] strarr = new string[3, 5]
            //    {
            //        {"o.MineCode", "=", mineCode, "string", "1"},
            //        {"LastTime", ">=", BegingTime.ToString(), "string", "1"},
            //        {"LastTime", "<=", EndTime.ToString(), "string", "1"},
            //    };
            //where = GetSelectString(strarr);

            string[,] strarrCount = new string[3, 5]
                {
                    {"MineCode", "=", mineCode, "string", "1"},
                    {"LastTime", ">=", BegingTime.ToString(), "string", "1"},
                    {"LastTime", "<=", EndTime.ToString(), "string", "1"},
                };

            wherecount = GetSelectString(strarrCount);
            wherecount = "select SimpleName,badtimes,d.MineCode,starttime,endtime," +
                "case TypeCode when 1 then '安全监控' when 2 then '人员管理' when 3 then '瓦斯抽放' else '安全监控/瓦斯抽放' end as SystemTypeName," +
                "timecount from (select count(typecode) badtimes,MineCode,min(LastTime) starttime,max(dateadd(second,timeout,LastTime)) endtime," +
                "TypeCode,dbo.FunConvertTime(sum(Continuous)) timecount from shineview_his.dbo.BadLog " +
                (wherecount.Equals("") ? "1=1" : wherecount) +
                " group by MineCode,TypeCode) as dleft join MineConfig mc on d.MineCode=mc.MineCode where SimpleName is not null order by MineCode";

            // wherecount = "select * from ShineView_His.dbo.BadLog where " + (wherecount.Equals("") ? "1=1" : wherecount);

            //            string wheredata = @"select Row_Number() over (order by LastTime asc) as TmpID,o.MineCode,SimpleName,case TypeCode when 1 then '安全监控' when 2 then '人员管理' when 3 then '瓦斯抽放' else '安全监控/瓦斯抽放' end as SystemTypeName,Case StateCode When 0 Then '正常' When 1 Then '通讯中断' When 2 Then '传输异常' End StateCode,LastTime StartTime,dateadd(second,timeout,LastTime) EndTime,dbo.FunConvertTime(Continuous) Continuous  from ShineView_His.dbo.BadLog B
            //Left Join  MineConfig o on o.MineCode = b.MineCode where " + (where.Equals("") ? "1=1" : where);
            string wheredata = "select Row_Number() over (order by starttime asc) as TmpID, SimpleName,badtimes,d.MineCode,starttime,endtime," +
                "case TypeCode when 1 then '安全监控' when 2 then '人员管理' when 3 then '瓦斯抽放' else '安全监控/瓦斯抽放' end as SystemTypeName," +
                "timecount from (select count(typecode) badtimes,MineCode,min(LastTime) starttime,max(dateadd(second,timeout,LastTime)) endtime," +
                "TypeCode,dbo.FunConvertTime(sum(Continuous)) timecount from shineview_his.dbo.BadLog " +
                (wherecount.Equals("") ? "1=1" : wherecount) +
                " group by MineCode,TypeCode) as dleft join MineConfig mc on d.MineCode=mc.MineCode where SimpleName is not null order by MineCode";
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }



        public DataTableCollection GetRYSSTG(string MineCode)
        {
            string whereData = "select  Row_Number() over (order by  mc.MineCode asc) as TmpID, mc.MineCode ,mc.SimpleName,isnull(T1.AllPeople,0) AllPeople,isnull( InMinePeople,0) InMinePeople , " +
                              "  isnull(InMineLeader,0) InMineLeader,isnull(RYCY,0) RYCY,isnull(RYCS,0) RYCS,isnull(ZDQY,0) ZDQY "+
                              "   from MineConfig mc "+
                              "  left join SystemConfig sc "+
                              "  on sc.MineCode = mc.ID "+
                              "  left join "+
                              "  (select count(1) AllPeople,MineCode from RYXX  where SystemType=2 GROUP BY (MineCode) ) T1 "+
                              "  ON T1.MineCode =mc.MineCode  "+
                              "  left join  "+
                              "  (select count(1) InMinePeople ,ss.MineCode from RYSS   ss "+
                              "  LEFT JOIN RYXX  xx "+
                              "  ON   xx.MineCode =ss.MineCode and xx.JobCardCode=ss.JobCardCode "+
                              "   where ss.SystemType=2  and  InOutType=1 GROUP BY ss.MineCode ) T2 "+
                              "  on T2.MineCode=mc.MineCode "+
                              "  left join  "+
                              "  (select count(1) InMineLeader ,ss.MineCode from RYSS ss "+
                              "   left JOIN  RYXX xx   "+
                              "  on xx.MineCode = ss.MineCode  and ss.JobCardCode=xx.JobCardCode "+
                              "   where ss.SystemType=2  and  ss.InOutType=1 and ( xx.[Position] like '%矿长%'  or xx.[Position] like '%领导%'  )  GROUP BY ss.MineCode ) T3 "+
                              "  on T3.MineCode=mc.MineCode "+
                              "  left join  "+
                              "  (select count(1) RYCS ,cs.MineCode from RYCS cs  "+
                              "  left join ryxx xx "+
                              "  on xx.MineCode =cs.MineCode and xx.JobCardCode=cs.JobCardCode  where cs.SystemType=2   and (EndAlTime like 'x%' or EndAlTime like 'X%')  GROUP BY cs.MineCode ) T4 " +
                              "  on T4.MineCode=mc.MineCode "+
                              "  left join  "+
                               " (select count(1) RYCY ,cy.MineCode from RYCYXZ  cy "+
                               " LEFT JOIN   ryxx xx "+
                               " on xx.MineCode=cy.MineCode and cy.JobCardCode=xx.JobCardCode where cy.SystemType=2   and (EndAlTime like 'x%' or EndAlTime like 'X%')  GROUP BY cy.MineCode ) T5 " +
                               " on T5.MineCode=mc.MineCode "+
                               " left join  "+
                               " (select count(1) ZDQY ,ss.MineCode from RYSS ss "+
                               "  left JOIN  RYXX xx   "+
                               " on xx.MineCode = ss.MineCode  and ss.JobCardCode=xx.JobCardCode "+
                               " LEFT JOIN RYQY qy  "+
                               " on qy.MineCode = ss.MineCode and qy.AreaCode=ss.AreaCode "+
                                " where ss.SystemType=2  and qy.AreaType like '%重点区域%'   GROUP BY ss.MineCode ) T6 "+
                                " on T6.MineCode=mc.MineCode "+
                                " where sc.isenabled=1 and sc.TypeCode=2 ";
                 string whereCount ="select  mc.MineCode ,mc.SimpleName  from MineConfig mc"+
                               " left join SystemConfig sc"+
                               " on sc.MineCode = mc.ID where sc.isenabled=1 and sc.TypeCode=2 " ;

                    if (!string.IsNullOrEmpty(MineCode))
                    {
                        whereCount += " and mc.MineCode ='" + MineCode + "'";
                        whereData += " and mc.MineCode='" + MineCode + "'";
                    }
        return ReturnTables(whereData, whereCount, "TmpID", "Data");
        }



        /// <summary>
        /// 人员实时数据
        /// </summary>
        /// <param name="MineCode"></param>
        /// <param name="AreaCode"></param>
        /// <param name="Department"></param>
        /// <param name="Name"></param>
        /// <param name="Job"></param>
        /// <returns></returns>
        public DataTableCollection GetRYSS(string MineCode, string AreaCode, string Name, string Job, string Department)
        {
            string whereData = "select Row_Number() over (order by ss.MineCode asc) as TmpID, ss.MineCode ,mc.SimpleName,ss.JobCardCode ,ss.StationCode,ss.InOutType,xx.Name,xx.[Position], xx.Department, " +
                         "    fz.Place  Place , "+
                         "   case ss.InOutType when 0 then '-'  else ss.InTime end InTime, "+
                         "   case ss.InOutType when 1 then  DATEDIFF(s, ss.InTime, GETDATE()) else '-' end InTimes, "+
                         "   case ss.InOutType when 1 then  ss.InAreaTime else ss.InNowStTime end  InNowStTime, "+
                         "    DATEDIFF(s, ss.InNowStTime, GETDATE())   InStTimes, "+
                         "   cast(null as varchar(50)) InMainTime, "+
                         "   cast(null as varchar(50) ) InStTime "+
                         "   from RYSS  ss "+
                         "   left join  MineConfig mc "+
                         "   on mc.MineCode=ss.MineCode "+
                         "   left join RYXX xx "+
                         "   on ss.MineCode =xx.MineCode and ss.JobCardCode=xx.JobCardCode "+
                         "   left join RYFZ fz  " +
                         "   on fz.MineCode =ss.MineCode and fz.StationCode=ss.StationCode WHERE 1=1";
            if (!string.IsNullOrEmpty(MineCode))
            {
                whereData += " AND ss.MineCode ='" + MineCode + "'";

            }
            if (!string.IsNullOrEmpty(AreaCode))
            {
                whereData += " AND ss.AreaCode ='" + AreaCode + "'";

            }
            if (!string.IsNullOrEmpty(Department))
            {
                whereData += " AND xx.Department ='" + Department + "'";

            }
            if (!string.IsNullOrEmpty(Name))
            {
                whereData += " AND xx.Name LIKE'%" + Name + "%'";

            }
            if (!string.IsNullOrEmpty(Job))
            {
                whereData += " AND xx.Position ='" + Job + "'";
            }
            return ReturnTables(whereData, whereData, "TmpID", "Data");
        }

       
        /// <summary>
        /// 人员实时数据
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRealDataForRY(string minecode, string AreaCode, string StationCode)
        {
            DataTable dt = new DataTable();
            string where = " where s.InOutType=1 ";
            //string wherecount = "select * from RYSS where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and s.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            if (AreaCode != "" && AreaCode != null)
            {
                where += " and q.AreaCode = '" + AreaCode + "'";
                //wherecount += " and AreaCode = '" + AreaCode + "'";
            }
            if (StationCode != "" && StationCode != null)
            {
                where += "and f.StationCode = '" + StationCode + "'";
                //wherecount += "and StationCode = '" + StationCode + "'";
            }
            //dt = dal.GetRealDataForRY(where);
            string wheredata = "select  Row_Number() over (order by getdate() asc) as TmpID,s.MineCode,SimpleName ,City ,s.JobCardCode,dbo.FunConvertTime(datediff(second, InTime,getdate())) as continuoustime,case  when x.Name is null then '' else x.Name end as Name,case when Position is null then '' else Position end as Position , case when Department is null then '' else Department end as Department,s.JobAddress ,s.InTime ,q.AreaName ,s.InAreaTime ,f.StationName , f.Place ,s.InNowStTime  from RYSS s left join RYQY q ON s.MineCode = q.MineCode and s.AreaCode=q.AreaCode left join RYFZ f ON s.MineCode = f.MineCode and s.StationCode=f.StationCode left join RYXX x ON s.MineCode = x.MineCode and s.JobCardCode=x.JobCardCode left join MineConfig m ON s.MineCode = m.MineCode " + where;
            string wherecount = wheredata;
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 区域信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRYQY(string minecode, string p_areaCode, string p_ratedWorks,string TypeKind)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYQY where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            if (p_areaCode != "" && p_areaCode != null)
            {
                where += " and o.AreaCode = '" + p_areaCode + "'";
                //wherecount += " and AreaCode = '" + id + "'";
            }
            if (!string.IsNullOrEmpty(p_ratedWorks))
            {
                where += " and o.Workers = '" + p_ratedWorks + "'";
            }
            if (!string.IsNullOrEmpty(TypeKind))
            {
                where += " and o.AreaType='限制区域'";
            }
            //dt = dal.GetRYQY(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,AreaCode,City ,AreaType ,o.Workers ,AreaName ,Time  from RYQY o left join MineConfig m on o.MineCode=m.MineCode  " + where;
            string wherecount = "select * from RYQY o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }


        public DataTable GetJob(string MineCode)
        {
            string sql = "select distinct  MineCode,Position Job from RYXX where SystemType=2 ";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql += " and MineCode ='" + MineCode + "'";
            }
            return dal.ReturnData(sql);
        }


        /// <summary>
        /// 区域额定人数
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTable GetRYRatedNumber(string minecode)
        {
            //DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYQY where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            //dt = dal.GetRYQY(where);
            string wheredata = "select distinct o.Workers  from RYQY o left join MineConfig m on o.MineCode=m.MineCode  " + where;
            string wherecount = "select distinct o.Workers from RYQY o " + where;

            DataTable worksTable = ReturnTables(wheredata, wherecount, "Workers", "Data")[1];

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("TmpID"));
            dt.Columns.Add(new DataColumn("Workers"));
            for (int i = 0; i < worksTable.Rows.Count; i++)
            {
                var row = dt.NewRow();
                row[0] = i;
                row[1] = worksTable.Rows[i][0];
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 职务
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTable GetRYDuty(string minecode)
        {
            //DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYQY where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            //dt = dal.GetRYQY(where);
            string wheredata = "select distinct o.[Position] Duty ,SimpleName from [RYXX] o left join MineConfig m on o.MineCode=m.MineCode  " + where;
            return dal.ReturnData(wheredata);
        }

        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTable GetRYDepartment(string minecode)
        {
            //DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYQY where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            //dt = dal.GetRYQY(where);
            string wheredata = "select distinct o.[Department] ,SimpleName from [RYXX] o left join MineConfig m on o.MineCode=m.MineCode  " + where;
            return dal.ReturnData(wheredata);
        }

        /// <summary>
        /// 区域信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTable GetRYQYList(string minecode)
        {
            DataTable dt = new DataTable();
            string where = "";
            if (minecode != "" && minecode != null)
            {
                where = "o.Minecode='" + minecode + "'";
            }
            else
            {
                where = "1=1";
            }
            dt = dal.GetRYQY(where);
            return dt;
        }

        /// <summary>
        /// 分站信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRYFZ(string minecode, string StationCode)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYFZ where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            if (StationCode != "" && StationCode != null)
            {
                where += " and o.StationCode = '" + StationCode + "'";
                //wherecount += " and StationCode = '" + StationCode + "'";
            }
            //dt = dal.GetRYFZ(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID, o.MineCode, SimpleName ,City ,StationCode,StationName ,Place ,Time  from RYFZ o left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = "select * from RYFZ o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 分站信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTable GetRYFZList(string minecode)
        {
            DataTable dt = new DataTable();
            string where = "";
            if (minecode != "" && minecode != null)
            {
                where = "o.Minecode='" + minecode + "'";
            }
            else
            {
                where = "1=1";
            }
            dt = dal.GetRYFZ(where);
            return dt;
        }

        /// <summary>
        /// 人员信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="position">工种或职务</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public DataTableCollection GetRYXX(string minecode, string position, string name)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYXX where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                //wherecount += " and minecode = '" + minecode + "' ";
                where += " and o.Minecode='" + minecode + "'";
            }
            if (position != "" && position != null)
            {
                //wherecount += " and jobCardCode = '" + position + "' ";
                where += " and o.jobCardCode  like '%" + position + "%'";
            }
            if (name != "" && name != null)
            {
                //wherecount += " and name = '" + name + "' ";
                where += " and o.name like '%" + name + "%'";
            }
            //dt = dal.GetRYXX(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,City ,JobCardCode,Name ,Gender,Position ,JobAddress ,Birthday,Blood ,Record ,Marriage ,o.Phone ,WorkCardID ,DateOver ,Time  from RYXX o left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = "select * from RYXX o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 人员信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="position">工种或职务</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public DataTable GetRYXXList(string minecode, string position, string name)
        {
            DataTable dt = new DataTable();
            string where = " 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
            }
            if (position != "" && position != null)
            {
                where += " and o.Position='" + position + "'";
            }
            if (name != "" && name != null)
            {
                where += " and o.JobCardCode='" + name + "'";
            }
            dt = dal.GetRYXX(where);
            return dt;
        }

        /// <summary>
        /// 获取指定煤矿下的工种或职务
        /// </summary>
        /// <param name="minecode">【必选】煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetPosition(string minecode)
        {
            DataTable dt = new DataTable();
            string where = "";
            if (minecode == "" || minecode == null)
            {
                return null;
            }
            else
            {
                where = "o.Minecode='" + minecode + "'";
            }
            //dt = dal.GetRYPositions(where);
            string wheredata = "select distinct Position,Position as tmp from RYXX where " + where;
            return ReturnTables(wheredata, wheredata, "Position", "Data");
        }

        /// <summary>
        /// 获取指定煤矿下的人员姓名
        /// </summary>
        /// <param name="minecode">【必选】煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetNames(string minecode)
        {
            DataTable dt = new DataTable();
            string where = "";
            if (minecode == "" || minecode == null)
            {
                return null;
            }
            else
            {
                where = "o.Minecode='" + minecode + "'";
            }
            //dt = dal.GetRYNames(where);
            string wheredata = "select distinct Name as tmp from RYXX o where " + where;
            return ReturnTables(wheredata, wheredata, "tmp", "Data");
        }

        /// <summary>
        /// 人员路线预设
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="position">工种或职务</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public DataTableCollection GetRYLXYS(string minecode, string position, string name)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYXX where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "' ";
            }
            if (position != "" && position != null)
            {
                where += " and o.Position='" + position + "'";
                //wherecount += "and Position = '" + position + "'";
            }
            if (name != "" && name != null)
            {
                where += " and o.JobCardCode='" + name + "'";
                //wherecount += " and name = '" + name + "'";
            }
            //dt = dal.GetRYLXYS(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,o.Name ,o.Position ,JobAddress ,StationsList ,o.Time  from RYLXYS o left join RYXX x on x.MineCode=o.MineCode and o.JobCardCode=x.JobCardCode left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = "select * from RYXX o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 实时超时信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRYCS(string minecode, string Name, string p_duty, string p_department)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1";
            //string wherecount = "select * from RYCS where 1=1";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += " and Minecode = '" + minecode + "'";
            }
            if (Name != "" && Name != null)
            {
                where += " and o.JobCardCode like '%" + Name + "%' ";
                //wherecount = " and Name = '" + Name + "' ";
            }
            if (!string.IsNullOrEmpty(p_duty))
            {
                where += " and x.Name like '%" + p_duty + "%' ";
                //wherecount = " and Name = '" + Name + "' ";
            }
            if (!string.IsNullOrEmpty(p_department))
            {
                where += " and x.Department = '" + p_department + "' ";
                //wherecount = " and Name = '" + Name + "' ";
            }
            //if (where == "1=1")
            //{
            where += " and (EndAlTime like 'x%' or EndAlTime like 'X%')";
            //wherecount += " and (EndAlTime like 'x%' or EndAlTime like 'X%')";
            //}
            //dt = dal.GetRYCS(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,Name ,Position ,x.JobCardCode,Department ,o.JobAddress ,InTime ,q.AreaName ,InAreaTime ,f.Place ,InNowStTime ,StartAlTime ,dbo.FunConvertTime(datediff(second, StartAlTime,getdate())) as continuoustime from RYCS o left join RYXX x on x.MineCode=o.MineCode and o.JobCardCode=x.JobCardCode left join RYFZ f on o.MineCode=f.MineCode and o.StationCode=f.StationCode left join RYQY q on o.MineCode=q.MineCode and o.AreaCode=q.AreaCode left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = wheredata;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 历史超时信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="p_areaCode">工种或职务</param>
        /// <param name="p_stationCode">姓名</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetHisCS(string minecode, string p_areaCode, string p_stationCode, DateTime BegingTime, DateTime EndTime)
        {
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
           
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from ShineView_His.dbo.RYCSH where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += "and minecode = '" + minecode + "' ";
            }
            if (!string.IsNullOrEmpty(p_areaCode))
            {
                where += " and o.AreaCode='" + p_areaCode + "' ";
                //wherecount += " and position = '" + position + "' ";
            }
            if (!string.IsNullOrEmpty(p_stationCode))
            {
                where += " and o.StationCode='" + p_stationCode + "' ";
                //wherecount += " and name = '" + name + "' ";
            }
            where += " and EndAlTime>='" + BegingTime + "' and EndAlTime<='" + EndTime + "'";
            //wherecount += " and EndAlTime>='" + BegingTime + "' and EndAlTime<='" + EndTime + "'";
            //dt = dal.GetHisRYCS(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,o.SimpleName ,o.Name ,o.Position ,o.Department ,o.InTime ,o.StartAlTime,o.EndAlTime,[dbo].[FunConvertTime](datediff(second, StartAlTime,EndAlTime)) as continuoustime,AreaName ,InAreaTime ,StationName ,InNowStTime  from ShineView_His.dbo.RYCSH o  " + where;
            string wherecount = "select * from ShineView_His.dbo.RYCSH o " + where;
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 实时超员信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRYCY(string minecode, string jobCardCode, string name)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYCYXZ where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += "and  minecode ='" + minecode + "' ";
            }
            //if (!string.IsNullOrEmpty(p_areaCode))
            //{
            //    where += " and o.AreaCode='" + p_areaCode + "' ";
            //}

            if (jobCardCode != "" && jobCardCode != null)
            {
                where += " and o.JobCardCode like '%" + jobCardCode + "%' ";
                //wherecount = " and Name = '" + Name + "' ";
            }
            if (!string.IsNullOrEmpty(name))
            {
                where += " and x.Name like '%" + name + "%' ";
                //wherecount = " and Name = '" + Name + "' ";
            }

            //else
            //{
            where += " and (EndAlTime like 'x%' or EndAlTime like 'X%')  and o.type not like'%限制%' ";
            //wherecount += " and (Type like '超员%' or Type like '%超员%' or Type like '%超员') and (EndAlTime like 'x%' or EndAlTime like 'X%')";
            //}
            //dt = dal.GetRYCYXZ(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,Name ,Position ,Department ,o.JobCardCode,o.Type ,InTime ,q.AreaName ,InAreaTime ,f.Place ,InNowStTime ,StartAlTime ,dbo.FunConvertTime(datediff(second, StartAlTime,getdate())) as continuoustime from RYCYXZ o left join RYXX x on x.MineCode=o.MineCode and o.JobCardCode=x.JobCardCode left join RYFZ f on o.MineCode=f.MineCode and o.StationCode=f.StationCode left join RYQY q on o.MineCode=q.MineCode and o.AreaCode=q.AreaCode left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = wheredata;// "select * from RYCYXZ o " + where;
            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 历史超员信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="p_areaCode">工种或职务</param>
        /// <param name="p_stationCode">姓名</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetHisCY(string minecode, string p_areaCode, string p_stationCode, DateTime BegingTime, DateTime EndTime)
        {
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from ShineView_His.dbo.RYCYXZH where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += " and Minecode = '" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(p_areaCode))
            {
                where += " and o.AreaCode='" + p_areaCode + "' ";
                //wherecount += " and position = '" + position + "' ";
            }
            if ( !string.IsNullOrEmpty(p_stationCode))
            {
                where += " and o.StationCode='" + p_stationCode + "' ";
                //wherecount += " and name = '" + name + "' ";
            }
            where += " and EndAlTime>='" + BegingTime + "' and EndAlTime<='" + EndTime + "'  and o.type not like'%限制%' ";
            //wherecount += " and EndAlTime>='" + BegingTime + "' and EndAlTime<='" + EndTime + "' and (Type like '超员%' or Type like '%超员%' or Type like '%超员')";
            //dt = dal.GetHisRYCYXZ(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,o.SimpleName ,o.Name ,o.Position ,o.Department ,o.type,o.InTime ,o.StartAlTime,o.EndAlTime,[dbo].[FunConvertTime](datediff(second, StartAlTime,EndAlTime)) as continuoustime  from ShineView_His.dbo.RYCYXZH o " + where;
            string wherecount = "select * from ShineView_His.dbo.RYCYXZH o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 实时限制信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>
        public DataTableCollection GetRYXZ(string minecode, string p_areaCode, string p_stationCode)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYCYXZ where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += " and minecode = '" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(p_areaCode))
            {
                where += " and o.JobCardCode like '%" + p_areaCode + "%' ";
            }
            if (!string.IsNullOrEmpty(p_stationCode))
            {
                where += " and   Name like '%" + p_stationCode + "%' ";
            }
            //else
            //{
            where += " and (EndAlTime like 'x%' or EndAlTime like 'X%') and o.type like '%限制%' ";
            //wherecount += " and (Type like '限制%' or Type like '%限制%' or Type like '%限制') and (EndAlTime like 'x%' or EndAlTime like 'X%')";
            //}
            //dt = dal.GetRYCYXZ(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,o.Type,Name ,Position ,Department ,o.JobCardCode,o.JobAddress ,InTime ,q.AreaName ,InAreaTime ,f.Place ,InNowStTime ,StartAlTime ,dbo.FunConvertTime(datediff(second, StartAlTime,getdate())) as continuoustime from RYCYXZ o left join RYXX x on x.MineCode=o.MineCode and o.JobCardCode=x.JobCardCode left join RYFZ f on o.MineCode=f.MineCode and o.StationCode=f.StationCode left join RYQY q on o.MineCode=q.MineCode and o.AreaCode=q.AreaCode left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = "select * from RYCYXZ o " + where;

            return ReturnTables(wheredata, wheredata, "TmpID", "Data");
        }

        /// <summary>
        /// 历史限制信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="p_areaCode">工种或职务</param>
        /// <param name="p_stationCode">姓名</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetHisXZ(string minecode, string p_areaCode, string p_stationCode, DateTime BegingTime, DateTime EndTime)
        {
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from ShineView_His.dbo.RYCYXZH where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += " and minecode = '" + minecode + "' ";
            }
            if (!string.IsNullOrEmpty(p_areaCode))
            {
                where += " and o.AreaCode='" + p_areaCode + "' ";
                //wherecount += " and position = '" + position + "' ";
            }
            if (!string.IsNullOrEmpty(p_stationCode))
            {
                where += " and o.StationCode='" + p_stationCode + "' ";
                //wherecount += " and name = '" + name + "' ";
            }
            where += " and EndAlTime>='" + BegingTime + "' and EndAlTime<='" + EndTime + "' and o.type like'%限制%' ";
            //wherecount += " and EndAlTime>='" + BegingTime + "' and EndAlTime<='" + EndTime + "' and (Type like '限制%' or Type like '%限制%' or Type like '%限制')";
            //dt = dal.GetHisRYCYXZ(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,o.SimpleName ,o.Name ,o.Position ,o.Department,o.Type ,o.InTime ,o.StartAlTime,o.EndAlTime,[dbo].[FunConvertTime](datediff(second, StartAlTime,EndAlTime)) as continuoustime,AreaName ,InAreaTime ,StationName ,InNowStTime   from ShineView_His.dbo.RYCYXZH  o " + where;
            string wherecount = "select * from ShineView_His.dbo.RYCYXZH o " + where;

            return ReturnTables(wheredata, wheredata, "TmpID", "His");
        }

        /// <summary>
        /// 人员管理 报警统计
        /// </summary>
        /// <param name="MineCode"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public DataTableCollection GetRYBJTG(string MineCode,DateTime BeginTime,DateTime EndTime ,int flag)
        {
            string whereCount = "select SimpleName from  MineConfig mc left join SystemConfig sc  on mc.ID = sc.MineCode where sc.IsEnabled=1 and sc.TypeCode=2 and 1="+flag;
            string sql1 = "select Row_Number() over (order by mc.MineCode  asc) as TmpID,mc.MineCode,mc.SimpleName,isnull(CY,0) CY,ISNULL(XZ,0) XZ,ISNULL(CS,0) CS,ISNULL(TZ,0) TZ " +
                      "  from  MineConfig mc "+
                      "  left join SystemConfig sc "+
                      "  on mc.ID = sc.MineCode  "+
                      "  left join  "+
                      "  (SELECT SUM(CY) CY ,minecode FROM  "+
                      "  (select count(1) CY,MineCode  from ShineView_His.dbo.RYCYXZH   where Type !='限制区域有人' ";
            string sql2="  GROUP BY MineCode "+
                       " union all select count(1) CY,MineCode  from RYCYXZ   where Type !='限制区域有人'  and  ( EndAlTime not like  'x%'  and   EndAlTime  not like  'X%') ";
            string sql3=" GROUP BY MineCode "+
                      "  ) T1 group by MineCode ) T1 "+
                      "  on T1.MineCode = mc.MineCode "+
                      "  left join  "+
                      "  (SELECT SUM(XZ) XZ ,minecode FROM  "+
                      "  (select count(1) XZ,MineCode  from ShineView_His.dbo.RYCYXZH   where Type ='限制区域有人'  ";
            string sql4=" GROUP BY MineCode "+
                      "  union all select count(1) XZ,MineCode  from RYCYXZ   where Type ='限制区域有人' and  ( EndAlTime not like  'x%'  and   EndAlTime  not like  'X%')  ";
            string sql5=" GROUP BY MineCode "+
                     "   ) T1 group by MineCode ) T2 "+
                     "   on T2.MineCode = mc.MineCode "+
                      "  left join  "+
                      "  (SELECT SUM(CS) CS ,minecode FROM  "+
                      "  (select count(1) CS,MineCode  from ShineView_His.dbo.RYCSH where  1=1  ";
            string sql6=" GROUP BY MineCode "+
                       " union all select count(1) CS,MineCode  from RYCS   where 1=1  and  ( EndAlTime not like  'x%'  and   EndAlTime  not like  'X%') ";
            string  sql7="  GROUP BY MineCode "+
                        " ) T1 group by MineCode ) T3 "+
                        " on T3.MineCode = mc.MineCode "+
                        " left join  "+
                        " (SELECT SUM(TZ) TZ ,minecode FROM  "+
                        " (select count(1) TZ,MineCode  from ShineView_His.dbo.RYTZYCH   where  1=1 ";
            string sql8 = "GROUP BY MineCode ";
            string sql10=" "+
                        ") T1 group by MineCode ) T4 "+
                        "on T4.MineCode = mc.MineCode " +
                       " where sc.IsEnabled=1 and sc.TypeCode=2 and 1="+flag;
            if (!string.IsNullOrEmpty(MineCode))
            {
                whereCount += " and mc.MineCode ='" + MineCode + "'";
                sql10 += " and mc.MineCode='" + MineCode + "'";
            
            }
        
                sql1 += " and StartAlTime>='" + BeginTime + "'";
                sql1 += " and StartAlTime<='" + EndTime + "'";
                sql2 += " and StartAlTime>='" + BeginTime + "'";
                sql2 += " and StartAlTime<='" + EndTime + "'";
                sql3 += " and StartAlTime>='" + BeginTime + "'";
                sql3 += " and StartAlTime<='" + EndTime + "'";
                sql4 += " and StartAlTime>='" + BeginTime + "'";
                sql4 += " and StartAlTime<='" + EndTime + "'";
                sql5 += " and StartAlTime>='" + BeginTime + "'";
                sql5 += " and StartAlTime<='" + EndTime + "'";
                sql6 += " and StartAlTime>='" + BeginTime + "'";
                sql6 += " and StartAlTime<='" + EndTime + "'";
                sql7 += " and OrigTime>='" + BeginTime + "'";
                sql7 += " and OrigTime<='" + EndTime + "'";
            string whereData = sql1 + sql2 + sql3 + sql4 + sql5 + sql6 + sql7 + sql8 + sql10;
            return ReturnTables(whereData, whereCount, "TmpID", "His");
        }

        /// <summary>
        /// 实时特种异常信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <returns></returns>                 
        public DataTableCollection GetRYTZYC(string minecode, string p_area, string p_station)
        {
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from RYTZYC where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "'";
                //wherecount += " and MineCode = '" + minecode + "'";
            }
            if (!string.IsNullOrEmpty(p_area))
            {
                where += " and o.AreaCode='" + p_area + "'";
            }
            if (!string.IsNullOrEmpty(p_station))
            {
                where += " and o.StationCode='" + p_station + "'";
            }
            //dt = dal.GetRYTZYC(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,Name ,Position ,Department ,o.JobAddress ,InTime ,q.AreaName ,InAreaTime ,f.Place ,InNowStTime ,OrigAddress,OrigTime,State from RYTZYC o left join RYXX x on x.MineCode=o.MineCode and o.JobCardCode=x.JobCardCode left join RYFZ f on o.MineCode=f.MineCode and o.StationCode=f.StationCode left join RYQY q on o.MineCode=q.MineCode and o.AreaCode=q.AreaCode left join MineConfig m on o.MineCode=m.MineCode " + where;
            string wherecount = "select * from RYTZYC o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

       /// <summary>
       /// 人员下井统计
       /// </summary>
       /// <param name="MineCode"></param>
       /// <param name="name"></param>
       /// <param name="Job"></param>
       /// <param name="Department"></param>
       /// <param name="BeginTime"></param>
       /// <param name="EndTime"></param>
       /// <param name="flag"></param>
       /// <returns></returns>
        public DataTableCollection GetData_XJRYTG(string MineCode ,string name,string Job ,string Department,DateTime BeginTime,DateTime EndTime ,int flag)
        {

            string sql1 = "select  Row_Number() over (order by xx.MineCode asc) as TmpID, mc.SimpleName  ,Name, JobCardCode ,Position,Department," +
                       " isnull(sum(累计次数),0) 累计次数,isnull(sum(累计时间),0) 累计时间 ,cast(null as varchar(50)) Continuous  from ryxx  xx" +
                       " left JOIN MineConfig mc" +
                       " on xx.minecode = mc.minecode " +
                       " left join (select  DISTINCT *   from   ShineView_His.dbo.Report_UpDown  )  rud " +
                       " on XX.mINECODE=rud.煤矿编号 and JobCardCode=标识卡 ";
            string sql2 = " where 1=" + flag;
            string sql3 = " group by   xx.MineCode,mc.SimpleName ,JobCardCode,Name,Position,Department";
              if (!string.IsNullOrEmpty(MineCode))
              {
                  sql2 += " and XX.MINECODE='" + MineCode + "'";
              }
              if (!string.IsNullOrEmpty(name))
              {
                  name = name.Replace("'", "\"");
                  sql2 += " and Name like '%" + name + "%'";
              }
              if (!string.IsNullOrEmpty(Job))
              {
                  sql2 += " and Position='" + Job + "'";
              }
              if (!string.IsNullOrEmpty(Department))
              {
                  sql2 += " and Department ='" + Department + "'";
              }
              sql1 += " and SubmitTime>='" + BeginTime + "'";
              sql1 += " and SubmitTime<='" + EndTime + "' ";
              string strwhere = sql1 + sql2+sql3;
              return ReturnTables(strwhere, strwhere, "TmpID", "His");
        }

       public  DataTableCollection GetXZQYRYTG(string MineCode ,string SensorNum,DateTime BeginTime,DateTime EndTime,int  flag)
       {
           string sql1 = "select  Row_Number() over (order by cy.MineCode  asc) as TmpID,cy.MineCode ,mc.SimpleName ,cy.AreaName,cy.Number,max(cy.Sum) Sum,StartAlTime,EndAlTime  " +
                      "  from ShineView_His.dbo.RYCYXZH cy  "+
                      "  left join ShineView_Data.dbo.MineConfig mc  "+
                      "  on mc.minecode = cy.MineCode  "+
                      "  where  cy.Type like '限制区域%' and  1="+flag;
           string sql2=    "  GROUP BY cy.MineCode ,mc.SimpleName ,cy.AreaName,cy.Number,StartAlTime,EndAlTime  "+
                      "  UNION ALL  "+
                      "  select  Row_Number() over (order by cy.MineCode  asc) as TmpID, cy.MineCode ,mc.SimpleName ,qy.AreaName,cy.Number,max(cy.Sum) Sum,StartAlTime,EndAlTime  " +
                      "  from ShineView_Data.dbo.RYCYXZ cy  "+
                      "  left join ShineView_Data.dbo.MineConfig mc  "+
                      "  on mc.minecode = cy.MineCode   "+
                      "  left join ShineView_Data.dbo.RYQY qy  "+
                      "  on qy.MineCode = cy.MineCode and qy.AreaCode =qy.AreaCode   "+
                      "  where  cy.Type like '限制区域%' and  ( EndAlTime not like  'x%'  and   EndAlTime  not like  'X%') and 1="+flag;
           string sql3 = "  GROUP BY cy.MineCode ,mc.SimpleName ,qy.AreaName,cy.Number,StartAlTime,EndAlTime ";
           if (!string.IsNullOrEmpty(MineCode))
           {
               sql1 += " and cy.MineCode ='" + MineCode + "'";
               sql2 += " and cy.MineCode ='" + MineCode + "'";
           }
           if (!string.IsNullOrEmpty(SensorNum))
           {
               sql1 += " and cy.AreaCode='" + SensorNum + "'";
               sql2 += " and cy.MineCode ='" + MineCode + "'";
           }
           sql1 += " and  StartAlTime>='" + BeginTime + "'";
           sql1 += " and StartAlTime<='" + EndTime + "'";

           sql2 += " and  StartAlTime>='" + BeginTime + "'";
           sql2 += " and StartAlTime<='" + EndTime + "'";
           string whereData = sql1 + sql2+sql3;
           return ReturnTables(whereData, whereData, "TmpID", "His");
       }


        /// <summary>
        /// 历史特种异常信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="p_areaCode">工种或职务</param>
        /// <param name="p_stationCode">姓名</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public DataTableCollection GetHisTZYC(string minecode, string p_areaCode, string p_stationCode, DateTime BegingTime, DateTime EndTime)
        {
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            DataTable dt = new DataTable();
            string where = " where 1=1 ";
            //string wherecount = "select * from ShineView_His.dbo.RYTZYCH where 1=1 ";
            if (minecode != "" && minecode != null)
            {
                where += " and o.Minecode='" + minecode + "' ";
                //wherecount += " and minecode = '" + minecode + "' ";
            }
            if (!string.IsNullOrEmpty(p_areaCode))
            {
                where += " and o.AreaCode='" + p_areaCode + "' ";
                //wherecount += " and position = '" + position + "' ";
            }
            if (!string.IsNullOrEmpty(p_stationCode))
            {
                where += " and o.StationCode='" + p_stationCode + "' ";
                //wherecount += " and name = '" + name + "' ";
            }
            where += " and RealTime>='" + BegingTime + "' and RealTime<='" + EndTime + "'";
            //wherecount += " and OrigTime>='" + BegingTime + "' and OrigTime<='" + EndTime + "'";
            //dt = dal.GetHisRYTZYC(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,o.SimpleName ,o.JobCardCode, o.Name ,o.Position ,o.Department ,o.InTime ,o.AreaName ,o.InAreaTime ,o.StationName ,o.InNowStTime ,o.OrigAddress,o.OrigTime,o.RealTime,o.State from ShineView_His.dbo.RYTZYCH o  " + where;
            string wherecount = "select * from ShineView_His.dbo.RYTZYCH o " + where;

            return ReturnTables(wheredata, wherecount, "TmpID", "Data");
        }

        /// <summary>
        /// 获取实时下井人员轨迹
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="JobCardCode">标示卡号</param>
        /// <returns></returns>
        public DataTable GetRTTrack(string mineCode, string JobCardCode)
        {
            string where = " MineCode = '" + mineCode + "' and JobCardCode='" + JobCardCode + "'";
            string sql = string.Format(@"
                declare @MineCode varchar(50),@JobCardCode varchar(50),@Name varchar(50),@Position varchar(50),@Department varchar(50),@InTime varchar(50),@OutTime varchar(50),@Place varchar(200),@StationList varchar(max)
	            declare my_cursor_rttrack cursor for select  ryss.MineCode,ryss.JobCardCode,case when Name is null then '' else Name end as Name, case when Position is null then '' else Position end as Position, case when Department is null then '' else Department end as Department,convert(varchar(20),InTime,120),convert(varchar(20),OutTime,120),StationList from (select * from RYSS where {0} ) as ryss left join RYXX on ryss.MineCode=RYXX.MineCode and ryss.JobCardCode=RYXX.JobCardCode 
	            declare @sql varchar(max) 
	            set @sql=''
	            open my_cursor_rttrack
	            fetch next from my_cursor_rttrack into @MineCode,@JobCardCode,@Name,@Position,@Department,@InTime,@OutTime,@StationList
	            while @@FETCH_STATUS=0
	            begin
	            set @sql+='select JobCardCode,Name,Position,Department,InTime,OutTime,Place,inStationTime from (select '+char(39)+@MineCode+char(39)+' MineCode,'+char(39)+@JobCardCode+char(39)+' JobCardCode,'+char(39)+@Name+char(39)+' Name,'+char(39)+@Position+char(39)+' Position,'+char(39)+@Department+char(39)+' Department,'+char(39)+@InTime+char(39)+' InTime,'+char(39)+@OutTime+char(39)+' OutTime,substring(short_str,0,charindex(''&'',short_str,0)) stationAddress,substring(short_str,charindex(''&'',short_str,0)+1,len(short_str)) inStationTime  from [dbo].F_SQLSERVER_SPLIT('+char(39)+@StationList+char(39)+','','')) x left join RYFZ on x.MineCode=RYFZ.MineCode and x.stationAddress=RYFZ.StationCode ';
	            set @sql+=' union all '
	            fetch next from my_cursor_rttrack into  @MineCode,@JobCardCode,@Name,@Position,@Department,@InTime,@OutTime,@StationList
	            end
	            close my_cursor_rttrack
	            DEALLOCATE my_cursor_rttrack
	            if(len(@sql)>8)
	            begin
	            set @sql = SUBSTRING(@sql,0,len(@sql)-8)
	            end
	            print @sql
	            exec('select * from ('+@sql+') as mytable')
                ", where);
            return dal.ReturnData(sql);

        }

        /// <summary>
        /// 查询上下井信息
        /// </summary>
        /// <param name="minecode">煤矿编号</param>
        /// <param name="p_areaCode">工种或职务</param>
        /// <param name="p_stationCode">姓名</param>
        /// <param name="BegingTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        /// 2015-2-4 修改记录：删除o.
        public DataTableCollection GetTrack(string minecode, string p_areaCode, string p_stationCode, DateTime BegingTime, DateTime EndTime)
        {
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }
            //DataTable dt = new DataTable();
            string where = " where 1=1 ";
            if (minecode != null && minecode != "")
            {
                where += " and Minecode='" + minecode + "' ";
            }
            //if (!string.IsNullOrEmpty(p_areaCode))
            //{
            //    where += " and o.AreaCode='" + p_areaCode + "' ";
            //    //wherecount += " and position = '" + position + "' ";
            //}
            //if (p_stationCode != "")
            //{
            //    where += " and o.StationCode='" + p_stationCode + "' ";
            //    //wherecount += " and name = '" + name + "' ";
            //}
            where += " and OutTime>='" + BegingTime + "' and OutTime<='" + EndTime + "'";
            //dt = dal.GetTrack(where);
            string wheredata = "select Row_Number() over (order by getdate() asc) as TmpID,SimpleName ,Name ,Position ,Department ,InTime ,OutTime ,[dbo].[FunConvertTime](datediff(second, InTime,OutTime)) as continuoustime,StationList from ShineView_His.dbo.RYHisTrack o " + where;
            string wherecount = "select * from ShineView_His.dbo.RYHisTrack o " + where;
            return ReturnTables(wheredata, wherecount, "TmpID", "His");
        }

        /// <summary>
        /// 显示人员轨迹
        /// </summary>
        /// <param name="minecode">【必选】煤矿编号</param>
        /// <param name="parlist">轨迹集合</param>
        /// <returns>分站名称、安装位置、进入时间</returns>
        public DataTableCollection GetTrackInfo(string minecode, string jobcard, string name, DateTime BegingTime, DateTime EndTime)
        {
            if (BegingTime.ToString() == "" || BegingTime == null || EndTime.ToString() == "" || EndTime == null)
            {
                return null;
            }

            string where = " where 1=1 ";
            if (minecode != null && minecode != "")
            {
                where += " and A.Minecode='" + minecode + "' ";
            }
            if (jobcard != null && jobcard != "")
            {
                where += " and JobCardCode like '%" + jobcard + "%' ";
            }
            if (name != null && name != "")
            {
                where += " and Name like '%" + name + "%' ";
            }

            where += " and InTime>='" + BegingTime + "' and InTime<='" + EndTime + "'";

            //string sql = string.Format(@"
            //        declare @MineCode varchar(50),@SimpleName varchar(50),@JobCardCode varchar(50),@Name varchar(50),@Position varchar(50),@Department varchar(50),@InTime varchar(50),@OutTime varchar(50),@StationList varchar(max)
            //        declare my_cursor_histrack cursor for select  MineCode,SimpleName,JobCardCode,case when Name is null then '' else Name end as Name, case when Position is null then '' else Position end as Position, case when Department is null then '' else Department end as Department,convert(varchar(20),InTime,120),convert(varchar(20),OutTime,120),StationList from ShineView_His.dbo.RYHisTrack {0}
            //        declare @sql varchar(max)
            //        set @sql=''
            //        open my_cursor_histrack
            //        fetch next from my_cursor_histrack into @MineCode,@SimpleName,@JobCardCode,@Name,@Position,@Department,@InTime,@OutTime,@StationList
            //        while @@FETCH_STATUS=0
            //        begin
            //        set @sql+='select x.MineCode,x.SimpleName,JobCardCode,Name,Position,Department,InTime,OutTime,ryfz.Place,inStationTime from (select '+char(39)+@MineCode+char(39)+' MineCode,'+char(39)+@SimpleName+char(39)+' SimpleName,'+char(39)+@JobCardCode+char(39)+' JobCardCode,'+char(39)+@Name+char(39)+' Name,'+char(39)+@Position+char(39)+' Position,'+char(39)+@Department+char(39)+' Department,'+char(39)+@InTime+char(39)+' InTime,'+char(39)+@OutTime+char(39)+' OutTime,substring(short_str,0,charindex(''&'',short_str,0)) stationAddress,substring(short_str,charindex(''&'',short_str,0)+1,len(short_str)) inStationTime  from [dbo].F_SQLSERVER_SPLIT('+char(39)+@StationList+char(39)+','','')) x left join ryfz on x.MineCode = ryfz.MineCode and x.stationAddress=ryfz.StationCode';
            //        set @sql+=' union all '
            //        fetch next from my_cursor_histrack into @MineCode,@SimpleName,@JobCardCode,@Name,@Position,@Department,@InTime,@OutTime,@StationList
            //        end
            //        close my_cursor_histrack
            //        DEALLOCATE my_cursor_histrack
            //        if(len(@sql)>8)
            //        begin
            //        set @sql = SUBSTRING(@sql,0,len(@sql)-8)
            //        end
            //        print @sql
            //        exec('select count(1) as totalRow from ('+@sql+') as mytable')
            //        exec('select * from ('+@sql+') as mytable order by inStationTime offset {1} row fetch next {2} rows only')
            //        ", where, pageIndex * pageSize, pageSize);
            //return dal.ReturnDs(sql).Tables;
            string sql = string.Format(@"select Row_Number() over (order by getdate() asc) as TmpID,A.*,B.StationCode,B.InoutTime,StationName,Place from ShineView_His.dbo.RYHisTrack A
Left Join  (Select * From ShineView_His.dbo.Split_TraceStrings()) B on A.ID = B.ID
Left Join RYFZ C on A.MineCode = c.MineCode and B.StationCode =C.StationCode {0}", where);
            return ReturnTables(sql, sql, "TmpID", "His");
        }

        #endregion

        #region [用户和权限]
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="MyUser"></param>
        /// <returns></returns>
        public bool AddUser(UserAndPower.UserModel MyUser)
        {
            string sql =
                string.Format(
                    "insert into UsersInfo(UserID,GroupID,UserName,PassWord,Remark) Values('{0}','{1}','{2}','{3}','{4}')"
                    , MyUser.UserID, MyUser.GroupID, MyUser.UserName, MyUser.PassWord, MyUser.Remark);
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public DataTable QueryUser(string UserID)
        {
            string sql = string.Format(@"Select U.*,r.RoleName,G.GroupName From [dbo].[UsersInfo] U
Left Join [UsersGroupInfo] G on U.GroupID = G.GroupID
Left Join RolesInfo R on g.RoleID = r.RoleID where 1=1 ");
            if (!string.IsNullOrEmpty(UserID))
            {
                sql += " and u.UserID='" + UserID + "'";
            }
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool DelUser(string UserID)
        {
            string sql = string.Format("delete from UsersInfo where UserID in (" + UserID + ")");
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="MyUser"></param>
        /// <returns></returns>
        public bool EditUser(UserAndPower.UserModel MyUser)
        {
            bool Result = false;
            if (MyUser.UserID != "")
            {
                string sql =
                    string.Format(
                        "Update [UsersInfo] Set GroupID ='{0}',UserName ='{1}',[PassWord] ='{2}',Remark='{3}' Where UserID ='{4}'"
                        , MyUser.GroupID, MyUser.UserName, MyUser.PassWord, MyUser.Remark, MyUser.UserID);

                Result = dal.ExcuteSql(sql);
            }
            return Result;
        }
        /// <summary>
        /// 增加用户组
        /// </summary>
        /// <param name="MyUserGroup"></param>
        /// <returns></returns>
        public bool AddUserGroup(UserAndPower.UserGroupModel MyUserGroup)
        {
            string Sql = string.Format("insert into UsersGroupInfo (GroupID,");
            string values = string.Format(" values(newid(),");
            if (!string.IsNullOrEmpty(MyUserGroup.RoleID) && MyUserGroup.RoleID != "null")
            {
                Sql += "RoleID,";
                values += "Convert(uniqueidentifier,'" + MyUserGroup.RoleID + "'),";
            }
            Sql += "MineCode,GroupName,GroupDescription,Remark)";
            Sql += values + "'" + MyUserGroup.MineCode + "','" + MyUserGroup.GroupName + "','" + MyUserGroup.GroupDescription + "','" + MyUserGroup.Remark + "')";
            return dal.ExcuteSql(Sql);
        }
        /// <summary>
        /// 查询用户组信息
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        public DataTable QueryUserGroup(string UserGroupID)
        {
            string sql = string.Format(@"Select G.MineCode, G.GroupID, G.RoleID,ri.RoleName, M.SimpleName, G.GroupName, G.GroupDescription, G.Remark, G.GroupID
	From UsersGroupInfo G Left Join MineConfig M On G.MineCode = M.MineCode left join RolesInfo ri on G.roleID=ri.roleID Where 1=1 ");
            if (!string.IsNullOrEmpty(UserGroupID) && UserGroupID != "null")
            {
                sql += "And G.GroupID='" + UserGroupID + "'";
            }
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 修改用户组信息
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public bool AlterUserGroup(UserAndPower.UserGroupModel MyUserGroupModel)
        {
            bool Result = true;
            string sql = string.Format("update UsersGroupInfo set ");
            if (MyUserGroupModel.RoleID != null && MyUserGroupModel.RoleID != "" && MyUserGroupModel.RoleID != "null")
            {
                sql += "RoleID=Convert(uniqueidentifier,'" + MyUserGroupModel.RoleID + "'),";
            }
            sql += "MineCode='" + MyUserGroupModel.MineCode + "',GroupName='" + MyUserGroupModel.GroupName + "',GroupDescription='" + MyUserGroupModel.GroupDescription + "',Remark='" + MyUserGroupModel.Remark + "' where GroupID='" + MyUserGroupModel.GroupID + "'";
            Result = Result & dal.ExcuteSql(sql);
            return Result;
        }
        /// <summary>
        /// 删除用户组信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DelUserGroup(string UserGroupID)
        {
            bool Result = true;
            string sql = string.Format("delete from UsersGroupInfo where GroupID in (" + UserGroupID + ")");
            Result = Result & dal.ExcuteSql(sql);
            return Result;
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="MyRole"></param>
        /// <returns></returns>
        public bool AddRole(UserAndPower.RoleModel MyRole)
        {
            string sql = string.Format(@"If (Select COUNT(1) From RolesInfo Where RoleName = '{0}') = 0
insert into RolesInfo(RoleID,RoleName,RoleDescription,Remark)values(newid(),'{0}','{1}','{2}')",
                MyRole.RoleName, MyRole.RoleDescription, MyRole.Remark);
            return dal.ExecSql(sql) > 0;
        }
        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <returns></returns>
        public DataTable QueryRole(string RoleID)
        {
            string sql = string.Format("select * from RolesInfo where 1=1 ");
            if (RoleID != null && RoleID != "")
            {
                sql += " and RoleID='" + RoleID + "'";
            }
            return dal.ReturnData(sql);
        }

        public DataTable QueryRoleToCombobox(string RoleID)
        {
            string sql = string.Format("select RoleID,RoleName from RolesInfo where 1=1 ");
            if (RoleID != null && RoleID != "")
            {
                sql += " and RoleID='" + RoleID + "'";
            }
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="MyRoleModel"></param>
        /// <returns></returns>
        public bool AlterRole(UserAndPower.RoleModel MyRoleModel)
        {
            string sql = string.Format(@"If (Select COUNT(1) From RolesInfo Where RoleName = '{0}' And RoleID<>'{3}') = 0
                update RolesInfo set RoleName='{0}',RoleDescription='{1}',Remark='{2}' where RoleID='{3}'",
                MyRoleModel.RoleName, MyRoleModel.RoleDescription, MyRoleModel.Remark, MyRoleModel.RoleID);
            return dal.ExecSql(sql) > 0;
        }
        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="Roles"></param>
        /// <returns></returns>
        public bool DelRole(string RoleID)
        {
            bool Result = true;
            string sql = string.Format("delete from RolesInfo where RoleID in (" + RoleID + ")  delete from RolesPowerInfo where RoleID in (" + RoleID + ") ");
            Result = Result & dal.ExcuteSql(sql);
            return Result;
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="MyMenu"></param>
        /// <returns></returns>
        public bool AddMenu(UserAndPower.MenusModel MyMenu)
        {
            string sql1 = string.Format("insert into MenusInfo (MenuID");
            string sql2 = string.Format(" values(newid()");
            if (MyMenu.ParentMenuID != "")
            {
                sql1 += ",MenuParentID";
                sql2 += ",Convert(uniqueidentifier,'" + MyMenu.ParentMenuID + "')";
            }
            string sql = string.Format("{0},ClassNO,MenuName,MenuDescription,Remark,MenuIndex,MenuPath) {1},{2},'{3}','{4}','{5}',{6},'{7}')", sql1,
                                       sql2, MyMenu.ClassNO, MyMenu.MenuName, MyMenu.MenuDescription, MyMenu.Remark, MyMenu.MenuIndex, MyMenu.MenuPath);
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 查询菜单信息
        /// </summary>
        /// <param name="MenuID"></param>
        /// <returns></returns>
        public DataTable QueryMenu(string MenuID)
        {
            string sql = string.Format("select * from MenusInfo where 1=1 ");
            if (MenuID != null && MenuID != "")
            {
                sql += " and MenuID = '" + MenuID + "'";
            }
            sql += " order by ClassNO,MenuParentID,MenuIndex ";
            return dal.ReturnData(sql);
        }

        public DataTable QueryMenuCombox(string ClassNO)
        {
            string sql = string.Format("select * from MenusInfo where 1=1 ");
            if (ClassNO != null && ClassNO != "")
            {
                sql += " and ClassNO = " + (int.Parse(ClassNO) - 1) + "";
            }
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 修改菜单项
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public bool AlterMenu(UserAndPower.MenusModel Menu)
        {
            bool Result = true;
            string sql = string.Format("update MenusInfo set ");
            if (Menu.ClassNO != 0)
            {
                sql += "ClassNO='" + Menu.ClassNO + "',";
            }
            if (string.IsNullOrEmpty(Menu.ParentMenuID))
            {
                sql +=
              string.Format(
                  "MenuName='{0}',MenuPath='{1}',MenuDescription='{2}',Remark='{3}',MenuIndex={5} where MenuID='{4}'",
                  Menu.MenuName, Menu.MenuPath, Menu.MenuDescription, Menu.Remark, Menu.MenuID, Menu.MenuIndex);
            }
            else
            {
                sql +=
              string.Format(
                  "MenuName='{0}',MenuPath='{1}',MenuParentID=Convert(uniqueidentifier,{2}),MenuDescription='{3}',Remark='{4}',MenuIndex={6} where MenuID='{5}'",
                  Menu.MenuName, Menu.MenuPath, string.IsNullOrEmpty(Menu.ParentMenuID) ? null : "'" + Menu.ParentMenuID + "'", Menu.MenuDescription, Menu.Remark, Menu.MenuID, Menu.MenuIndex);
            }

            Result = Result & dal.ExcuteSql(sql);
            return Result;
        }
        /// <summary>
        /// 删除菜单项
        /// </summary>
        /// <param name="MenuIDs"></param>
        /// <returns></returns>
        public bool DelMenus(string MenuID)
        {
            bool Result = true;
            string sql = string.Format("delete from MenusInfo where MenuID = '" + MenuID + "'");
            Result = Result & dal.ExcuteSql(sql);
            return Result;
        }
        /// <summary>
        /// 根据菜单等级查询菜单信息
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public DataTable QueryMenus(int i)
        {
            string sql = string.Format("select * from MenusInfo where ClassNO=" + i + " order by MenuIndex ");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 根据父菜单ID查询菜单信息
        /// </summary>
        /// <param name="MenuParentID"></param>
        /// <returns></returns>
        public DataTable QueryMenusByMenuParentID(string MenuParentID)
        {
            string sql = string.Format("select * from MenusInfo where MenuParentID='" + MenuParentID + "' order by MenuIndex ");
            return dal.ReturnData(sql);
        }

        public DataTable MenuIndexToMenuID(int MenuIndex)
        {
            string sql = string.Format("select MenuID from MenusInfo where MenuIndex = " + MenuIndex + " order by MenuIndex ");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="MenuID"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public bool AddRolePower(string RoleID, string MenuID, string Remark)
        {
            string sql = string.Format("insert into RolesPowerInfo values(newid(),Convert(uniqueidentifier,'" + RoleID + "'),Convert(uniqueidentifier,'" + MenuID + "'),'" + Remark + "')");
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public bool DelRolePower(string RoleID)
        {
            string sql = string.Format("delete from RolesPowerInfo where RoleID='" + RoleID + "'");
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 根据角色ID查询权限
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public DataTable QueryPowers(string RoleID)
        {
            string sql = string.Format("select * from RolesPowerInfo where RoleID = '" + RoleID + "'");
            return dal.ReturnData(sql);
        }

        public string GetUpdateSql(string TableName, UserAndPower.BaseModel BaseModel)
        {
            string updateinfo = string.Empty;
            Type type = BaseModel.GetType();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                string value = pi.GetValue(BaseModel, null).ToString(); ;
                if (value != null && value != "")
                {
                    updateinfo = "" + pi.Name + "='" + value + "',";
                }
            }
            updateinfo = updateinfo.Remove(updateinfo.Length - 1);
            string Result = "update " + TableName + " set ";
            return Result;
        }

        public DataTable QueryMineCode()
        {
            string sql = "select * from MineConfig";
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="BaseModel"></param>
        /// <returns></returns>
        //public string GetSql(string TableName,UserAndPower.BaseModel BaseModel)
        //{
        //    string columns = string.Empty;
        //    string values = string.Empty;
        //    Type type = BaseModel.GetType();
        //    foreach (PropertyInfo pi in type.GetProperties())
        //    {
        //        columns += pi.Name + ",";
        //        values += "'" + pi.GetValue(BaseModel, null) + "',";
        //    }
        //    columns = columns.Remove(columns.Length - 1);
        //    values = values.Remove(values.Length - 1);
        //    string Result = "insert into " + TableName + "(" + columns + ") values(" + values + ")";
        //    return Result;
        //}
        #endregion

        #region 企业性质管理

        #region 下拉框数据
        /// <summary>
        /// 查询职务信息
        /// </summary>
        /// <returns></returns>
        public DataTable QueryPost()
        {
            string sql = string.Format("select * from EIDrop_Post");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询身体状况信息
        /// </summary>
        /// <returns></returns>
        public DataTable QueryHealth()
        {
            string sql = string.Format("select * from EIDrop_Health");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询从事工种信息
        /// </summary>
        /// <returns></returns>
        public DataTable Queryworktype()
        {
            string sql = string.Format("select * from EIDrop_Typework");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询设备类型
        /// </summary>
        /// <returns></returns>
        public DataTable Queryeqname()
        {
            string sql = string.Format("select * from EIDrop_DevType");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询设备用途
        /// </summary>
        /// <returns></returns>
        public DataTable QueryUseType()
        {
            string sql = string.Format("select * from EIDrop_UseType");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询企业图纸档案 图纸类别
        /// </summary>
        /// <returns></returns>
        public DataTable Querydrawingstype()
        {
            string sql = string.Format("select * from EIDrop_QYTZtype");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询相关资料文件类型
        /// </summary>
        /// <returns></returns>
        public DataTable QueryXGZLType()
        {
            string sql = string.Format("select * from EIDrop_XGZLType");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询企业证照附件类型信息
        /// </summary>
        /// <returns></returns>
        public DataTable Queryattachmenttype()
        {
            string sql = string.Format("select * from EIDrop_QYZZ");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询经济类型信息
        /// </summary>
        /// <returns></returns>
        public DataTable Querycol_type()
        {
            string sql = string.Format("select * from EIDrop_Economics");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询质量标准化等级
        /// </summary>
        /// <returns></returns>
        public DataTable Querystandard_grade()
        {
            string sql = string.Format("select * from EIDrop_Quality");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询矿井矿井状态
        /// </summary>
        /// <returns></returns>
        public DataTable Querycol_status()
        {
            string sql = string.Format("select * from EIDrop_MineState");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询矿井类型
        /// </summary>
        /// <returns></returns>
        public DataTable Querymine_type()
        {
            string sql = string.Format("select * from EIDrop_MineType");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询主要煤种
        /// </summary>
        /// <returns></returns>
        public DataTable Querycoal_type()
        {
            string sql = string.Format("select * from EIDrop_MainMine");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询开拓方式
        /// </summary>
        /// <returns></returns>
        public DataTable Querytunnel_mode()
        {
            string sql = string.Format("select * from EIDrop_Tunnelmode");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询采煤工艺
        /// </summary>
        /// <returns></returns>
        public DataTable Querycoal_mine()
        {
            string sql = string.Format("select * from EIDrop_CoalMine");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询运输方式
        /// </summary>
        /// <returns></returns>
        public DataTable Querymode_of_ship()
        {
            string sql = string.Format("select * from EIDrop_ModeofShip");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询供电方式
        /// </summary>
        /// <returns></returns>
        public DataTable Querypower_type()
        {
            string sql = string.Format("select * from EIDrop_PowerType");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询瓦斯等级
        /// </summary>
        /// <returns></returns>
        public DataTable Querygas_level()
        {
            string sql = string.Format("select * from EIDrop_GasLevel");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 煤层自燃等级
        /// </summary>
        /// <returns></returns>
        public DataTable Queryself_iginte()
        {
            string sql = string.Format("select * from EIDrop_SelfLevel");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询煤层顶底板岩性
        /// </summary>
        /// <returns></returns>
        public DataTable Queryrock_behavio()
        {
            string sql = string.Format("select * from EIDrop_Rockbehavio");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询开拓巷道支护方式
        /// </summary>
        /// <returns></returns>
        public DataTable Querylw_shoring_type()
        {
            string sql = string.Format("select * from EIDrop_Lwshoringtype");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询采面支护方式
        /// </summary>
        /// <returns></returns>
        public DataTable Queryme_shoring_type()
        {
            string sql = string.Format("select * from EIDrop_Meshoringtype");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 查询事故类型
        /// </summary>
        /// <returns></returns>
        public DataTable Queryaccident_type()
        {
            string sql = string.Format("select * from EIDrop_AccidentType");
            return dal.ReturnData(sql);
        }
        #endregion

        #region 企业基本信息

        /// <summary>
        /// 查询煤矿基本信息【煤矿编号、煤矿名称】
        /// </summary>
        /// <returns></returns>
        public DataTable QuereyJBXXSimple()
        {
            string sql = string.Format("select id,minecode,simplename,city,x,y from mineconfig");
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 查询基本信息
        /// </summary>
        /// <returns></returns>
        public DataTable QueryJBXX(string MineCode)
        {
            string sql = "";
            if (MineCode != "")
            {
                sql = string.Format("select id,colliery_no,col_name,areano,col_type,lawyer from EItable_JBXX where colliery_no ='" + MineCode + "'");
            }
            else
            {
                sql = string.Format("select id,colliery_no,col_name,areano,col_type,lawyer from EItable_JBXX");
            }
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 根据ID，查询企业信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable QueryJBXXByID(string ID)
        {
            string sql = string.Format("select * from EItable_JBXX where id=" + ID + "");
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 添加基本信息
        /// </summary>
        /// <param name="Prop"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public bool AddJBXX(string[] Prop, string[] Values)
        {
            bool Result = false;
            if (Prop.Length == Values.Length)
            {
                string sql = string.Format("if not exists(select 1 from EItable_JBXX where colliery_no='" + Values[0] + "') begin insert into EItable_JBXX ");
                string sqlprop = string.Empty;
                string sqlvalue = string.Empty;
                for (int i = 0; i < Prop.Length; i++)
                {
                    sqlprop += Prop[i] + ",";
                    sqlvalue += "'" + Values[i] + "',";
                }
                if (sqlprop.Length > 2) sqlprop = sqlprop.Remove(sqlprop.Length - 1);
                if (sqlvalue.Length > 2) sqlvalue = sqlvalue.Remove(sqlvalue.Length - 1);
                sql = sql + " (" + sqlprop + ") values(" + sqlvalue + ") end ";
                Result = dal.ExcuteSql(sql);
            }
            return Result;
        }
        /// <summary>
        /// 修改基本信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Prop"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public bool AlterJBXX(string ID, string[] Prop, string[] Values)
        {
            bool Result = false;
            if (Prop.Length == Values.Length)
            {
                string sql = string.Format("update EItable_JBXX set ");
                for (int i = 0; i < Prop.Length; i++)
                {
                    sql += Prop[i] + "='" + Values[i] + "',";
                }
                sql = sql.Remove(sql.Length - 1);
                sql += " where id=" + ID + "";
                Result = dal.ExcuteSql(sql);
            }
            return Result;
        }
        /// <summary>
        /// 删除企业基本信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DelJBXX(string ID)
        {
            string sql = string.Format("delete from EItable_JBXX where id in (select " + ID + ")");
            return dal.ExcuteSql(sql);
        }
        #endregion

        #region 相关资料
        /// <summary>
        /// 查询相关资料信息
        /// </summary>
        /// <returns></returns>
        public DataTable QueryXGZL(string minecode)
        {
            string sql = string.Format(@"Select * from (
        Select [id],[colliery_no],[filename],[notes],[attachmenttype] from EItable_XGZL Where colliery_no  like '%{0}%'
        ) A Left Join EIDrop_XGZLType B on b.Code = a.attachmenttype 
        left join (select MineCode,simpleName from MineConfig) as mc on A.colliery_no=mc.MineCode", minecode);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 下载企业证照信息
        /// </summary>
        /// <param name="rowid">自增列</param>
        /// <returns></returns>
        public DataTable DownloadXGZL(string rowid)
        {
            string sql = string.Format("select filename,filecontent from EItable_XGZL where id=" + rowid);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 添加相关资料数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="FileContent"></param>
        /// <returns></returns>
        public bool AddXGZL(string[] Values, byte[] FileContent)
        {
            string sql = string.Format("insert into EItable_XGZL(colliery_no,filename,notes,attachmenttype,filecontent)"
                + " values ('" + Values[0] + "','" + Values[1] + "','" + Values[2] + "','" + Values[3] + "',@filecontent)");
            return dal.InsertFileSql(sql, FileContent);
        }
        /// <summary>
        /// 修改相关资料数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="FileContent"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool AlterXGZL(string[] Values, byte[] FileContent, string ID)
        {
            string sql = string.Format("update EItable_XGZL set colliery_no='" + Values[0] + "',notes='" + Values[2] + "',"
                + "attachmenttype='" + Values[3] + "'");
            if (Values[1] != null && Values[1] != "")
            {
                sql += string.Format(",filename='" + Values[1] + "',fileContent=@filecontent");
            }
            sql += " where id=" + ID + "";
            return dal.InsertFileSql(sql, FileContent);
        }
        /// <summary>
        /// 删除相关资料数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DelXGZL(string ids)
        {
            bool Result = true;
            string sql = string.Format("delete from EItable_XGZL where id in (" + ids + ")");
            Result = Result & dal.ExcuteSql(sql);
            return Result;
        }
        #endregion

        #region 煤矿设备管理
        /// <summary>
        /// 查询煤矿设备数据
        /// </summary>
        /// <returns></returns>
        public DataTable QueryMineDev(string minecode)
        {
            string sql = string.Format("select * from EItable_MineDev Where colliery_no like '%{0}%'", minecode);
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 添加煤矿设备数据
        /// </summary>
        /// <param name="Props"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public bool AddMineDev(string[] Props, string[] Values)
        {
            string sql = string.Format("insert into EItable_MineDev  ");
            string sql1 = string.Empty;
            string sql2 = string.Empty;
            if (Props.Length == Values.Length)
            {
                for (int i = 0; i < Props.Length; i++)
                {
                    sql1 += Props[i] + ",";
                    sql2 += "'" + Values[i] + "',";
                }
                sql1 = sql1.Remove(sql1.Length - 1);
                sql2 = sql2.Remove(sql2.Length - 1);
            }
            sql = sql + "(" + sql1 + ") values (" + sql2 + ")";
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 修改煤矿设备数据
        /// </summary>
        /// <param name="Props"></param>
        /// <param name="Values"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool AlterMineDev(string[] Props, string[] Values, string ID)
        {
            string sql = string.Format("update EItable_MineDev set ");
            if (Props.Length == Values.Length)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    if (Values[i] != null && Values[i] != "")
                    {
                        sql += Props[i] + "='" + Values[i] + "',";
                    }
                }
                sql = sql.Remove(sql.Length - 1);
                sql += " where ID=" + ID + "";
            }
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 删除煤矿设备数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DelMineDev(string ids)
        {
            bool result = true;
            string sql = string.Format("delete from EItable_MineDev where id in (" + ids + ")");
            result = result & dal.ExcuteSql(sql);
            return result;
        }
        #endregion

        #region 安全管理机构
        /// <summary>
        /// 查询安全管理机构数据
        /// </summary>
        /// <returns></returns>
        public DataTable QueryAQJG(string minecode)
        {
            string sql = string.Format("select * from EItable_GLJG Where colliery_no like '%{0}%'", minecode);
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 添加管理机构数据
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public bool AddAQJG(string[] Values)
        {
            string sql = string.Format("insert into EItable_GLJG(colliery_no,dep_name,dep_alias,lead,telephone,change_no) values("
                + "'" + Values[0] + "','" + Values[1] + "','" + Values[2] + "','" + Values[3] + "','" + Values[4] + "','" + Values[5] + "')");
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 修改管理机构数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool AlterAQJG(string[] Values, string ID)
        {
            string sql = string.Format("update EItable_GLJG set colliery_no='" + Values[0] + "',dep_name='" + Values[1] + "',"
                + "dep_alias='" + Values[2] + "',lead='" + Values[3] + "',telephone='" + Values[4] + "',change_no='" + Values[5] + "'"
                + " where id=" + ID + "");
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 删除管理机构数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DelAQJG(string ID)
        {
            string sql = string.Format("delete from EItable_GLJG where ID  in(" + ID + ")");
            return dal.ExcuteSql(sql);
        }
        #endregion

        #region 持证人员
        /// <summary>
        /// 查询持证人员信息
        /// </summary>
        /// <returns></returns>
        public DataTable QueryCZRY(string minecode)
        {
            string sql = string.Format("select * from EItable_CZRY Where colliery_no like '%{0}%'", minecode);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 添加持证人员信息
        /// </summary>
        /// <param name="Props"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public bool AddCZRY(string[] Props, string[] Values)
        {
            string sql = string.Format("insert into EItable_CZRY  ");
            string sql1 = string.Empty;
            string sql2 = string.Empty;
            if (Props.Length == Values.Length)
            {
                for (int i = 0; i < Props.Length; i++)
                {
                    sql1 += Props[i] + ",";
                    sql2 += "'" + Values[i] + "',";
                }
                sql1 = sql1.Remove(sql1.Length - 1);
                sql2 = sql2.Remove(sql2.Length - 1);
            }
            sql = sql + "(" + sql1 + ") values (" + sql2 + ")";
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 修改持证人员信息
        /// </summary>
        /// <param name="Props"></param>
        /// <param name="Values"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool AlterCZRY(string[] Props, string[] Values, string ID)
        {
            string sql = string.Format("update EItable_CZRY set ");
            if (Props.Length == Values.Length)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    if (Values[i] != null && Values[i] != "")
                    {
                        sql += Props[i] + "='" + Values[i] + "',";
                    }
                }
                sql = sql.Remove(sql.Length - 1);
                sql += " where ID=" + ID + "";
            }
            return dal.ExcuteSql(sql);
        }
        /// <summary>
        /// 删除持证人员信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DelCZRY(string ids)
        {
            bool result = true;
            string sql = string.Format("delete from EItable_CZRY where id in (" + ids + ")");
            result = result & dal.ExcuteSql(sql);
            return result;
        }
        #endregion

        #region 企业证照
        /// <summary>
        /// 查询企业证照信息
        /// </summary>
        /// <returns></returns>
        public DataTable QueryQYZZ(string minecode)
        {
            string sql = string.Format("select * from (select * from EItable_QYZZ Where colliery_no like '%{0}%') as A " +
                " left join (select MineCode,simpleName from MineConfig) as mc on A.colliery_no=mc.MineCode ", minecode);
            return dal.ReturnData(sql);
        }
        /// <summary>
        /// 添加企业证照数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public bool AddQYZZ(string[] Values, byte[] fileContent)
        {
            string sql = string.Format("insert into EItable_QYZZ (colliery_no,filename,notes,attachmenttype,filecontent)"
                + " values('" + Values[0] + "','" + Values[1] + "','" + Values[2] + "','" + Values[3] + "',"
                + "@filecontent)");
            return dal.InsertFileSql(sql, fileContent);
            //return dal.InsertFileSql(sql, MyFileContent);
        }

        /// <summary>
        /// 下载企业证照信息
        /// </summary>
        /// <param name="rowid">自增列</param>
        /// <returns></returns>
        public DataTable DownloadQYZZ(string rowid)
        {
            string sql = string.Format("select filename,filecontent from EItable_QYZZ where id=" + rowid);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 删除企业证照信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DelQYZZ(string[] ids)
        {
            bool Result = true;
            for (int i = 0; i < ids.Count(); i++)
            {
                if (ids[i] == null || ids[i] == "") continue;
                string sql = string.Format("delete from EItable_QYZZ where id in (" + ids[i] + ")");
                Result = Result & dal.ExcuteSql(sql);
            }
            return Result;
        }
        /// <summary>
        /// 修改企业证照数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="FileContent"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AlterQYZZ(string[] Values, byte[] FileContent, string id)
        {
            bool AlterFile = false;
            string sql = string.Format("update EItable_QYZZ set colliery_no='" + Values[0] + "'");
            if (Values[1] != null && Values[1] != "")
            {
                AlterFile = true;
                sql += ",filename='" + Values[1] + "',filecontent=@filecontent";
            }
            sql += ",notes='" + Values[2] + "',attachmenttype='" + Values[3] + "' where id=" + id + "";
            if (AlterFile)
            {
                return dal.InsertFileSql(sql, FileContent);
            }
            else
            {
                return dal.ExcuteSql(sql);
            }
        }
        #endregion

        #region 图纸档案
        /// <summary>
        /// 查询企业图纸档案数据
        /// </summary>
        /// <returns></returns>
        public DataTable QueryQYTZ(string minecode)
        {
            string sql = string.Format(@"Select * from (Select id,colliery_no,upload_month,drawings_type,explain," +
                "valid_date_begin,valid_date_end ,filename from EItable_QYTZ Where colliery_no liKe '%{0}%' ) A Left Join " +
                "EIDrop_QYTZtype  B on b.Code = a.drawings_type left join (select minecode,simpleName from mineconfig) as mc on a.colliery_no=mc.minecode", minecode);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 下载企业图纸上传文件
        /// </summary>
        /// <param name="rowid">自增列</param>
        /// <returns></returns>
        public DataTable DownloadQYTZ(string rowid)
        {
            string sql = string.Format("select filename,filecontent from EItable_QYTZ where id=" + rowid);
            return dal.ReturnData(sql);
        }


        /// <summary>
        /// 添加企业图纸档案数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="FileContent"></param>
        /// <returns></returns>
        public bool AddQYTZ(string[] Values, byte[] FileContent)
        {
            string sql = string.Format("insert into EItable_QYTZ (colliery_no,upload_month,drawings_type,explain,valid_date_begin,valid_date_end,filename,filecontent)"
                + " values('" + Values[0] + "','" + Values[1] + "','" + Values[2] + "','" + Values[3] + "','" + Values[4] + "','" + Values[5] + "','" + Values[6] + "',"
                + "@filecontent)");
            return dal.InsertFileSql(sql, FileContent);
        }
        /// <summary>
        /// 修改企业图纸档案数据
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="FileContent"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool AlterQYTZ(string[] Values, byte[] FileContent, string ID)
        {
            bool AlterFile = false;
            string sql = string.Format("update EItable_QYTZ set colliery_no = '" + Values[0] + "',upload_month='" + Values[1] + "',drawings_type='" + Values[2] + "'"
                + ",explain = '" + Values[3] + "',valid_date_begin='" + Values[4] + "',Valid_date_end='" + Values[5] + "'");
            if (Values[6] != null && Values[6] != "")
            {
                AlterFile = true;
                sql += ",filename='" + Values[6] + "',fileContent = @filecontent";
            }
            sql += " where id=" + ID + "";
            if (AlterFile)
            {
                return dal.InsertFileSql(sql, FileContent);
            }
            else
            {
                return dal.ExcuteSql(sql);
            }
        }
        /// <summary>
        /// 删除企业图纸档案数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DelQYTZ(string[] ids)
        {
            bool Result = true;
            for (int i = 0; i < ids.Count(); i++)
            {
                if (ids[i] == null || ids[i] == "") continue;
                string sql = string.Format("delete from EItable_QYTZ where id in (" + ids[i] + ")");
                Result = Result & dal.ExcuteSql(sql);
            }
            return Result;
        }
        #endregion

        #endregion

        #region 登录
        /// <summary>
        /// 检验登录用户是否存在
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public DataTable getUserInfo_IsExists(string UserName, string PassWord)
        {
            string sql = string.Format("select * from usersInfo where userName='{0}'", UserName);
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 检验登录用户名和密码是否存在
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public DataTable getUserInfo_IsPassword(string UserName, string PassWord)
        {
            string sql = string.Format("select ui.UserID,UserName,MineCode from UsersInfo ui left join UsersGroupInfo ugi on ui.GroupID=ugi.GroupID  where [userName]='{0}' and [PassWord]='{1}'", UserName, PassWord);
            return dal.ReturnData(sql);

        }
        /// <summary>
        /// 加载登录用户权限菜单
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public string getUserMenu(string UserName, string MineCode)
        {
            return dal.ReturnData(string.Format("exec system_LoadUserMenu '{0}','{1}'", UserName, MineCode)).Rows[0][0].ToString();
        }
        #endregion

        #region 隐患排查管理
        /// <summary>
        /// 隐患排查-插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTouble_Check_Insert(TroubleCheckModel model)
        {
            string sql = string.Format("INSERT INTO [dbo].[HiddenTrouble_Check]([CheckCategory],[MineCode],[TroubleClass],[TroubleCategory],[CheckDept]," +
               "[Rummager],[CheckDate],[HiddenResponsibilityDept],[TroubleContent],[Remark])  VALUES({0},'{1}',{2},{3},'{4}','{5}','{6}','{7}','{8}','{9}')",
               model.CheckCategory, model.MineCode, model.TroubleClass, model.TroubleCategory, model.CheckDept, model.Rummager, model.CheckDate, model.HiddenResponsibilityDept, model.TroubleContent, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 隐患排查-查询
        /// </summary> 
        /// <param name="Condition"></param>
        /// <returns></returns>
        public DataTable HiddenTouble_Check_Query(string Condition)
        {
            string sql = string.Empty;
            if (Condition != "")
            {
                sql =
                    string.Format(@" select [RowID],[CheckCategory],a.[MineCode],simplename,col_name,choose([TroubleClass],'A级','B级','C级') TroubleClass ,choose([TroubleCategory],'顶板','运输','机电','通风','瓦斯','煤尘','放炮','火灾','水害','其它') [TroubleCategory],[CheckDept],[Rummager],[CheckDate],[HiddenResponsibilityDept],[TroubleContent],a.[Remark] 
 from HiddenTrouble_Check a 
 left  join EItable_JBXX b on a.MineCode=b.colliery_no 
 Left Join [MineConfig] m on a.MineCode = m.MineCode where {0} order by CheckDate", Condition);
            }
            else
            {
                sql = "select [RowID],[CheckCategory],[MineCode],col_name,choose([TroubleClass],'A级','B级','C级') TroubleClass ,choose([TroubleCategory],'顶板','运输','机电','通风','瓦斯','煤尘','放炮','火灾','水害','其它') [TroubleCategory],[CheckDept],[Rummager],[CheckDate],[HiddenResponsibilityDept],[TroubleContent],[Remark] from HiddenTrouble_Check a left join EItable_JBXX b on a.MineCode=b.colliery_no order by CheckDate";
            }

            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 隐患排查-更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTouble_Check_Update(TroubleCheckModel model)
        {
            string sql = string.Format("Update [HiddenTrouble_Check] set [CheckCategory]={1},[MineCode]='{2}',[TroubleClass]={3},[TroubleCategory]={4},[CheckDept]='{5}'," +
               "[Rummager]='{6}',[CheckDate]='{7}',[HiddenResponsibilityDept]='{8}',[TroubleContent]='{9}',[Remark]='{10}' where RowID={0}",
              model.RowID, model.CheckCategory, model.MineCode, model.TroubleClass, model.TroubleCategory, model.CheckDept, model.Rummager, model.CheckDate, model.HiddenResponsibilityDept, model.TroubleContent, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 隐患排查-删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTouble_Check_Delete(string Condition)
        {
            string sql = string.Format("delete from [HiddenTrouble_Check] where {0}", Condition);
            return dal.ExcuteSql(sql);
        }


        /// <summary>
        /// 隐患处理信息-插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTrouble_Process_Insert(TroubleProcessModel model)
        {
            string sql = string.Format("INSERT INTO [dbo].[HiddenTrouble_Process]([TroubleID],[ProcessCategory],[TroubleProcessContent],[TroubleProcessDate],[TroubleProcesser],[TroubleProcessCompleteDate],[Remark])" +
                "VALUES({0}, {1},'{2}','{3}','{4}','{5}','{6}')", model.TroubleID, model.ProcessCategory, model.TroubleProcessContent, model.TroubleProcessDate, model.TroubleProcesser, model.TroubleProcessCompleteDate, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 隐患处理信息-更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTrouble_Process_Update(TroubleProcessModel model)
        {
            string sql = string.Format("update [dbo].[HiddenTrouble_Process] set [TroubleID]={1},[ProcessCategory]={2},[TroubleProcessContent]='{3}',[TroubleProcessDate]='{4}',[TroubleProcesser]='{5}',[TroubleProcessCompleteDate]='{6}',[Remark]='{7}' " +
                  " where RowID={0}", model.RowID, model.TroubleID, model.ProcessCategory, model.TroubleProcessContent, model.TroubleProcessDate, model.TroubleProcesser, model.TroubleProcessCompleteDate, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 隐患处理信息-删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTrouble_Process_Delete(TroubleProcessModel model)
        {
            string sql = string.Format("delete from [dbo].[HiddenTrouble_Process] where RowID={0}", model.RowID);
            return dal.ExcuteSql(sql);
        }

        #endregion

        #region [ 隐患 ]

        public DataTable HiddenTrouble_CheckProcess_Query(string Condition)
        {
            string sql = @"select RowID, CheckCategory, MineCode,simplename,choose([TroubleClass],'A级','B级','C级') TroubleClass,choose([TroubleCategory],'顶板','运输','机电','通风','瓦斯','煤尘','放炮','火灾','水害','其它') TroubleCategory, CheckDept, Rummager, CheckDate, HiddenResponsibilityDept,
TroubleContent, Remark,RowID1,TroubleID,ProcessCategory, TroubleProcessContent, TroubleProcessDate, TroubleProcesser, TroubleProcessCompleteDate,Remark1,case   when TroubleID is null then   '未处理' else '已处理' end as ProcessStatus from 
(select htc.*,htp.*,simplename from [dbo].[HiddenTrouble_Check] htc 
left join 
(select RowID as RowID1, TroubleID, ProcessCategory, TroubleProcessContent, TroubleProcessDate, TroubleProcesser, TroubleProcessCompleteDate, Remark as Remark1 from [dbo].[HiddenTrouble_Process]) as  htp on htc.RowID=htp.TroubleID
 Left Join [MineConfig] m on htc.MineCode = m.MineCode) as x";
            if (Condition != "")
            {
                sql = sql + " where " + Condition;
            }
            return dal.ReturnData(sql);
        }


        /// <summary>
        /// 隐患处理-查询
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public DataTable HiddenTrouble_Process_Query(string Condition)
        {
            string sql = string.Empty;
            if (Condition != "")
            {
                sql = "select * from HiddenTrouble_Process where  " + Condition;
            }
            else
            {
                sql = "select * from HiddenTrouble_Process";
            }
            return dal.ReturnData(sql);
        }


        /// <summary>
        /// 隐患复查信息-插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTrouble_Recheck_Insert(TroubleRecheckModel model)
        {
            string sql = string.Format("INSERT INTO [dbo].[HiddenTrouble_ReCheck](TroubleID,RecheckCategory,TroubleProcessID,TroubleRecheckDate,TroubleRechecker,TroubleRecheckResult,IsPass,Remark)" +
                "VALUES({0}, {1},{2},'{3}','{4}','{5}',{6},'{7}')", model.TroubleID, model.RecheckCategory, model.TroubleProcessID, model.TroubleRecheckDate, model.TroubleRechecker, model.TroubleRecheckResult, model.IsPass, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 隐患复查信息-更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTrouble_Recheck_Update(TroubleRecheckModel model)
        {
            string sql = string.Format("update [dbo].[HiddenTrouble_ReCheck] set [TroubleID]={1},[RecheckCategory]={2},[TroubleProcessID]={3},[TroubleRecheckDate]='{4}',[TroubleRechecker]='{5}',[TroubleRecheckResult]='{6}',IsPass={7},[Remark]='{8}'" +
                  " where RowID={0}", model.RowID, model.TroubleID, model.RecheckCategory, model.TroubleID, model.TroubleRecheckDate, model.TroubleRechecker, model.TroubleRecheckResult, model.IsPass, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 隐患复查信息-删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HiddenTrouble_Recheck_Delete(TroubleRecheckModel model)
        {
            string sql = string.Format("delete from [dbo].[HiddenTrouble_ReCheck] where RowID={0}", model.RowID);
            return dal.ExcuteSql(sql);
        }



        /// <summary>
        /// 隐患复查-查询
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public DataTable HiddenTrouble_Recheck_Query(string Condition)
        {

            string sql = "select * from (select a.RowID,a.CheckCategory,a.MineCode,mc.SimpleName, choose(a.[TroubleClass],'A级','B级','C级') TroubleClass," +
"choose(a.[TroubleCategory],'顶板','运输','机电','通风','瓦斯','煤尘','放炮','火灾','水害','其它') [TroubleCategory]," +
"a.CheckDate,a.CheckDept,a.Rummager,a.TroubleContent,a.HiddenResponsibilityDept,b.RowID processID,b.TroubleProcessDate,b.TroubleProcesser," +
"b.TroubleProcessContent,b.TroubleProcessCompleteDate,case   when b.TroubleID is null then   '未处理' else '已处理' end as ProcessStatus,c.RowID recheckID," +
"case  when c.TroubleID is NULL then '未复查' else '已复查' end recheckFlag ,Convert(varchar(10),c.TroubleRecheckDate,120) TroubleRecheckDate," +
"c.TroubleRechecker,c.TroubleRecheckResult,c.IsPass,case when c.IsPass=1 then '隐患已消除' when c.IsPass=0 then '隐患未消除' " +
"else '未复查' end as recheckResult ,c.Remark from HiddenTrouble_Check A " +
" left join HiddenTrouble_Process B on a.RowID=b.TroubleID " +
" left join HiddenTrouble_ReCheck C on a.RowID=c.TroubleID" +
 " left join mineconfig mc on a.MineCode=mc.MineCode) as tx ";
            if (Condition != "")
            {
                sql += " where  " + Condition;
            }

            return dal.ReturnData(sql);
        }

        #endregion

        #region 交接班管理
        /// <summary>
        ///  交接班记录查询
        /// </summ
        /// ary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public DataTable WorkShift_Query(string Condition)
        {
            string sql = string.Empty;
            if (Condition != "")
            {
                sql = string.Format("select wi.*,mc.simpleName from WorkShiftInfo wi left join mineconfig mc on wi.MineCode=mc.MineCode  where {0}", Condition);
            }
            else
            {
                sql = "select * from WorkShiftInfo";
            }
            return dal.ReturnData(sql);
        }

        /// <summary>
        /// 添加交接班记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool WorkShift_Insert(WorkShiftModel model)
        {
            string sql = string.Format("INSERT INTO [dbo].[WorkShiftInfo]([MineCode],[Dept],[PreWorkTime],[PreWorkContent],[PreWorkShift],[PreWorkPersonName],[NextWorkTime]" +
          " ,[NextWorkContent],[NextWorkShift],[NextWorkPersonName],[Remark],[WorkShiftCategory])VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11})", model.MineCode, model.Dept, model.PreWorkTime, model.PreWorkContent, model.PreWorkShift, model.PreWorkPersonName, model.NextWorkTime,
          model.NextWorkContent, model.NextWorkShift, model.NextWorkPersonName, model.Remark, model.WorkShiftCategory);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 更新交接班记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool WorkShift_Update(WorkShiftModel model)
        {
            string sql = string.Format("Update [dbo].[WorkShiftInfo] set [MineCode]='{1}',[Dept]='{2}',[PreWorkTime]='{3}',[PreWorkContent]='{4}',[PreWorkShift]='{5}',[PreWorkPersonName]='{6}',[NextWorkTime]='{7}'" +
        " ,[NextWorkContent]='{8}',[NextWorkShift]='{9}',[NextWorkPersonName]='{10}',[Remark]='{11}' where RowID={0}", model.RowID, model.MineCode, model.Dept, model.PreWorkTime, model.PreWorkContent, model.PreWorkShift, model.PreWorkPersonName, model.NextWorkTime,
        model.NextWorkContent, model.NextWorkShift, model.NextWorkPersonName, model.Remark);
            return dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 删除交接班记录
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public bool WorkShift_Delete(string Condition)
        {
            string sql = string.Format("delete from  [dbo].[WorkShiftInfo] where {0}", Condition);
            return dal.ExcuteSql(sql);
        }

        #endregion


        public DataTableCollection GetMnlMinute_Curve(string minecode, string typeCode, string sensorNames,
            DateTime BegingTime, DateTime EndTime)
        {
            int ts = 1;

            DataTableCollection dtc;

            var sqls = new SqlParameter[8];
            sqls[0] = new SqlParameter("@pageNum", PageIndex);  //查询页索引
            sqls[1] = new SqlParameter("@pageSize", PageSize);  //查询页大小
            sqls[2] = new SqlParameter("@MineCode", minecode);
            sqls[3] = new SqlParameter("@TypeCode", typeCode);
            sqls[4] = new SqlParameter("@SensorNums", sensorNames);
            sqls[4] = new SqlParameter("@BegingTime", BegingTime);
            sqls[5] = new SqlParameter("@EndTime", EndTime);
            sqls[6] = new SqlParameter("@TimeSpan", ts);

            dtc = dal.GetMnlMinute_Curve(sqls);

            return dtc;
        }


        public DataTable GetRouteByUserID(string Name, string StartTime, string EndTime, string con,string FileID)
        {

            string selectstring = "Shine_HistoryInOutStation_QueryView_ZZHA";
            SqlParameter[] parameters = new SqlParameter[] {
																new SqlParameter("@strTableName", SqlDbType.VarChar, 50),
																new SqlParameter("@intBlock", SqlDbType.Int),
																new SqlParameter("@strName", SqlDbType.VarChar, 20),
																new SqlParameter("@intUserType", SqlDbType.Int),
																new SqlParameter("@strStartDateTime", SqlDbType.VarChar, 50),
																new SqlParameter("@strEndDateTime", SqlDbType.VarChar, 50),
                                                                new SqlParameter("@FileID", SqlDbType.Int)};
            parameters[0].Value = "Shen_HisInOutStationHeadInfo_zdc";
            parameters[1].Value = Name;
            parameters[2].Value = "";
            parameters[3].Value = 0;
            parameters[4].Value = StartTime;
            parameters[5].Value = EndTime;
            parameters[6].Value = FileID;
            try
            {
                return SQLDataServer.ProcedureDataSet(selectstring, parameters, con).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            //try
            //{
            //    DateTime startTime = Convert.ToDateTime(StartTime);
            //    string yearMounth = startTime.Year.ToString() + startTime.Month.ToString();
            //    string sql = "select  StationAddress , StationHeadAddress,StationHeadPlace ,DeptName,UserName,UserNo ,InStationHeadTime,OutStationHeadTime,ContinueTime " +
            //               "from His_InOutStationHead_" + yearMounth + " where UserNo='" + Name + "'";

            //    sql += " and InStationHeadTime>'" + StartTime + "' and OutStationHeadTime<'" + EndTime + "'";
            //    return dal.ReturnData(sql, con);
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}

        }

        public DataTable GetReoutePointByID(string ID, bool IsDesc, string FileID,string con)
        {
            string sql = sql = string.Format("select x,y from G_DPoint where pointid='{0}' and FileID={1} order by [id] ", ID, FileID);
            if (IsDesc)
            {
                sql += " desc";
            }
            else
            {
                sql += " asc";
            }
            return dal.ReturnData(sql,con);

        }

        public List<string> GetRouteInfoByEmpID(string name, string StartTime, string EndTime, string con, string FileID)
        {
            DataTable dt = GetRouteByUserID(name, StartTime, EndTime, con, FileID);
            string strMessage = "";
            string empname = "";
            List<string> list = new List<string>();
            if (dt!=null && dt.Rows.Count > 0)
            {
                strMessage = dt.Rows[0]["BlockID"].ToString() + "-" + dt.Rows[0]["UserName"].ToString()+":";
      ;
                if (dt.Rows[0][12].ToString() == "")
                {
                    empname = dt.Rows[0]["UserName"].ToString();
                }
                else
                {
                    empname = dt.Rows[0][12].ToString() + ":" + dt.Rows[0][2].ToString();
                }
                string routepoint = "";
                int LastIndex=0;
                 bool isdesc = false;
                 string address = "";
                string StationPoint="";
                string date="";
                for (int i=0;i<dt.Rows.Count;i++)
                {
                  string nowname=dt.Rows[i]["StationPlace"].ToString();
                  if (dt.Rows[LastIndex]["StationPlace"].ToString() != dt.Rows[i]["StationPlace"].ToString() && !string.IsNullOrEmpty(dt.Rows[i]["StationPlace"].ToString()))
                  {
                      string towid;
                      float xy1 = float.Parse(dt.Rows[LastIndex]["StationAddress"].ToString() + "." + dt.Rows[LastIndex]["StationHeadAddress"].ToString());
                      float xy2= float.Parse(dt.Rows[i]["StationAddress"].ToString() + "." + dt.Rows[i]["StationHeadAddress"].ToString());
                      LastIndex=i;
                      if (xy1>xy2)
                      {
                          towid=xy2.ToString("F1")+","+xy1.ToString("F1");
                          isdesc=true;
                      }
                      else 
                      {
                      towid=xy1.ToString("F1")+","+xy2.ToString("F1");
                      isdesc=false;
                      }
                      DataTable dt_Point=GetReoutePointByID(towid,isdesc,FileID,con);
                      if (dt_Point.Rows.Count > 0)
                      {
                          for (int j = 0; j < dt_Point.Rows.Count; j++)
                          {
                              routepoint += dt_Point.Rows[j]["x"].ToString() + "," + dt_Point.Rows[j]["y"].ToString() + "|";
                          }
                      }
                      else
                      {
                          // 128 中个若无路径 则添加  
                          //string strPath = "";
                          //if (isdesc)
                          //{ 
                          //strPath=GetWayByStaIDT(xy2.ToString("F1"),xy1.ToString("F1"),FileID,con)
                          //}
                      }
                      if (!string.IsNullOrEmpty(dt.Rows[i]["StationPlace"].ToString()))
                      {
                          address += dt.Rows[i]["StationPlace"].ToString() + "|";
                          StationPoint += dt.Rows[i]["StationX"].ToString() + "," + dt.Rows[i]["StationY"].ToString() + "|";
                          date += dt.Rows[i]["InStationTime"].ToString() + "|" + dt.Rows[i]["OutStationTime"].ToString() + "|";
                      }

                  }

                }//for
                
                if (!string.IsNullOrEmpty(routepoint))
                {
                    list.Add(routepoint.Remove(routepoint.Length-1));//x,y坐标
                    list.Add(date.Remove(date.Length-1));//进出分站时间
                    list.Add(address.Remove(address.Length-1));//分站名
                    list.Add(StationPoint.Remove(StationPoint.Length-1));//一个分站到另一个分站
                    list.Add(empname);//部门+姓名
                }
                return list;


            }//if
            strMessage = ":";
            return list;
         
        }

        #region 日志管理
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="processLogModel">日志对象</param>
        public void ProcesslogInfo_Insert(ProcessLogInfoModel processLogModel)
        {
            string sql = string.Format(@"INSERT INTO [dbo].[ProcessLogInfo]
           ([ProrcessUserName]
           ,[ProcessContent]
          )
     VALUES
           ('{0}'
           ,'{1}'
          
         )", processLogModel.ProcessName, processLogModel.ProcessContent);

            dal.ExcuteSql(sql);
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="BeginTime">查询开始时间</param>
        /// <param name="EndTime">查询结束时间</param>
        /// <returns></returns>
        public DataTable ProcessLogInfo_Query(string BeginTime, string EndTime, string ProcessUserName)
        {
            string sql = string.Format(@"SELECT [ProrcessUserName],[ProcessContent],[ProcessDateTime],[Remark] FROM 
                    [dbo].[ProcessLogInfo] where ProcessDateTime>='{0}' and ProcessDateTime<'{1}' and ProrcessUserName like '%{2}%' order by ProcessID 
                     ", BeginTime, DateTime.Parse(EndTime).AddDays(1).ToString("yyyy-MM-dd"), ProcessUserName);

            return dal.ReturnData(sql);
        }

        #endregion


    }
}