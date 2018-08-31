using System;
using System.Web.Mvc;
using InternetDataMine.Models;
using InternetDataMine.Models.DataService;
using Newtonsoft.Json;
using InternetDataMine.Models.Config;
using System.Data;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace InternetDataMine.Controllers
{
    public class RealDataController : Controller
    {
        DataDAL dal = new DataDAL();
        private readonly MineConfigModel _mineCfgModel = new MineConfigModel();
        /// <summary>
        /// 动态加载菜单
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="mineCode"></param>
        /// <returns></returns>
        public ActionResult Index(string userName,string mineCode)
        {
            return View();
        }

        public ActionResult MainPage(string userName, string mineCode)
        {
            return View();
        }

        public void GetAlarmPageData(string mineCode)
        {
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "No-Cache");

            string queryString = string.Empty;

            if (!string.IsNullOrEmpty(mineCode) && mineCode.ToLower() != "null")
            {
                queryString = string.Format(" Where MineCode Like '%{0}%'", mineCode);
            }
            else
            {
                queryString = "";
            }
            //在安全监控的sql中增加SystemType来区分安全监控、矿压、火管，人员未改
            string sql = string.Format(@"select * from (Select mc.MineCode,mc.SimpleName,case sc.TypeCode when 1 then '安全监控系统' when 2 then '人员管理系统' when 3 then '瓦斯抽放系统' when 5 then '广播系统' when 7 then '火灾束管系统' else '安全监控/瓦斯抽放系统' end as systemName,
case sc.StateCode  when 1 then '网络中断' when 2 then '传输异常' when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时' end as communicationState 
  from SystemConfig sc left join MineConfig mc on sc.MineCode=mc.ID where sc.StateCode<>0 ) as A {0}

/*安全监控报警*/
select * from (select mc.MineCode,mc.SimpleName MineName,dt.TypeName deviceName,aqss.Place,
dbo.dectobin(aqss.ValueState) as AlarmType,
ShowValue,aqss.PoliceMaxValue,AQSS.SystemType, aqss.PoliceMaxDatetime,aqss.PowerMax,aqss.PowerMaxDatetime,aqss.PowerDateTime,aqss.PoliceDateTime,[dbo].[FunConvertTime](datediff(second, PoliceDatetime,getdate())) as PoliceContinuoustime,
[dbo].[FunConvertTime](datediff(second, PowerDateTime,getdate())) as PowerContinuoustime,[dbo].[FunConvertTime](datediff(second, PowerDateTime,getdate())) as AbnormalContinuoustime,
aqss.AbnormalDateTime
from (select * from aqss where ValueState<>0) as aqss  
left join DeviceType dt on AQSS.Type=dt.TypeCode 
left join MineConfig mc on aqss.MineCode=mc.MineCode ) as B {0}  Order by AlarmType   

/*人员管理超时报警*/
select * from (select mc.MineCode,mc.SimpleName,rycs.JobCardCode,ryxx.Name,RYXX.Position Post,RYXX.Department Dept,rycs.InTime InMineTime,rycs.StartAlTime,
dbo.FunConvertTime(DateDiff(second,RYCS.StartAlTime,GETDATE())) OverTimeLength 
from RYCS 
left join MineConfig mc on rycs.MineCode=mc.MineCode 
left join RYXX on RYCS.JobCardCode=RYXX.JobCardCode and RYCS.MineCode=RYXX.MineCode ) as C {0}

/*人员管理超员报警*/
select * from (select mc.MineCode,cy.JobCardCode,mc.SimpleName,xx.Name,xx.Position Post,xx.Department Dept,cy.Number LimiteNumber,cy.[Sum] RealNumber
,cy.[Type] AlarmType,cy.InTime InMineTime,cy.StartAlTime,dbo.FunConvertTime(DateDiff(second,cy.StartAlTime,GETDATE())) OverTimeLength  
from RYCYXZ cy 
left join RYXX xx on cy.MineCode=xx.MineCode and cy.JobCardCode=xx.JobCardCode 
left join MineConfig mc on cy.MineCode=mc.MineCode) as D {0}

/*人员管理特种工作异常报警*/
select * from (select mc.MineCode,mc.SimpleName,xx.Name,xx.Position Post,xx.Department Dept,tz.InTime InMineTime,tz.OrigAddress PlanReachPlace,tz.OrigTime PlanReachTime,tz.RealTime RealReachTime,tz.[State] NowState 
from RYTZYC tz 
left join RYXX xx on tz.MineCode=xx.MineCode and tz.JobCardCode=xx.JobCardCode 
left join MineConfig mc on tz.MineCode=mc.MineCode ) as E {0}
/*基站报警*/
select * from (select R.MineCode ,SimpleName,StationCode , StationName, Place, format(Time,'yyyy-MM-dd HH:mm:ss')as Time,[dbo].[FunConvertTime](datediff(second, Time,getdate())) Continuous,
case StationState when 1 then '停用' when 2 then '检修'  end State from RYFZ  R
left join MineConfig mc on r.minecode =mc.minecode  where StationState!=0
) as F  {0}
", queryString);

            var dal = new DataDAL();
            var dt = dal.ReturnDs(sql);
            var data = JsonConvert.SerializeObject(dt);

            Response.Write(data);
            Response.End();
        }

        /// <summary>
        /// 保存/提交 预警处理
        /// </summary>
        /// <param name="ID">该条预警的id</param>
        /// <param name="Recorder">记录人</param>
        /// <param name="Measure">处理措施</param>
        /// <param name="DealResult">处理结果</param>
        /// <param name="state">保存或提交，保存还存在原来表中，提交转存历史表</param>
        /// <returns></returns>
        public ActionResult SavePre( string Recorder, string Measure, string DealResult, string ID ,string  flag,string Operate)
        {
            // flag 为报警是否结束 0为结束1为未结束

            //将输入文字中的单引号变为双引号，防止插入失败
            if (!string.IsNullOrEmpty(Recorder))
            {
            Recorder=    Recorder.Replace("'", "\"");
            }
            if (!string.IsNullOrEmpty(Measure))
            {
              Measure=  Measure.Replace("'", "\"");
                //Measure.Replace("'", "\"");
            }
            if (!string.IsNullOrEmpty(DealResult))
            {
             DealResult=   DealResult.Replace("'", "\"");
            }
           
                string sql = string.Format(@"update  warnalarm  set jlpers='{0}',chulway='{1}',chulresult='{2}',jltime='{3}' where ID={4} ", Recorder, Measure, DealResult, DateTime.Now, ID);
                bool IsUpdate=  _mineCfgModel.Exec(sql);
                if (!IsUpdate)
                {
                    return Json(new { data = "保存失败，请检查数据库连接！" }, JsonRequestBehavior.AllowGet);
                }
                else {
                    if (flag == "1")
                    {
                        return Json(new { data = "保存成功！" }, JsonRequestBehavior.AllowGet);

                    }
                    else {
                        if (Operate == "submit")
                        {
                            string sql_Select = "select w.minecode,w.sensornum,w.starttime,w.overtime,w.maxvalue,w.minvalue,w.lasttime,w.level,w.chulway,w.chulresult,w.jlpers,w.jltime ,w.chulStatus,m.simplename,m.city , c.place,c.unit,c.type from warnalarm w" +
                              " left join mineconfig m on w.minecode=m.minecode " +
                              " left join aqmc c on w.minecode=c.minecode and w.sensornum=c.sensornum  where w.id=" + ID;

                            var dal = new DataDAL();
                            var dt = dal.ReturnData(sql_Select);
                            string sql_Insert = string.Format("insert into shineview_his.dbo.[WarnAlarm_His] (MineCode,sensornum,starttime,overtime,maxvalue,minevalue,lasttime,level,chulway,chulresult,jlpers,jltime,status,mineName,city,place,unit,type,datetime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')", dt.Rows[0][0], dt.Rows[0][1], dt.Rows[0][2], dt.Rows[0][3], dt.Rows[0][4], dt.Rows[0][5], dt.Rows[0][6], dt.Rows[0][7], dt.Rows[0][8], dt.Rows[0][9], dt.Rows[0][10], dt.Rows[0][11], dt.Rows[0][12], dt.Rows[0][13], dt.Rows[0][14], dt.Rows[0][15], dt.Rows[0][16], dt.Rows[0][17], DateTime.Now);
                            bool IsInsert = dal.ExcuteSql(sql_Insert);

                            if (IsInsert)
                            {
                                string sql_Delete = string.Format("delete from WarnAlarm where id={0}", ID);
                                bool IsDelete = dal.ExcuteSql(sql_Delete);
                                if (IsDelete)
                                {
                                    return Json(new { data = "提交成功！" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { data = "删除数据失败，请检查数据库连接！" }, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {

                                return Json(new { data = "插入历史预警表失败，请检查数据库连接。" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {
                            return Json(new { data = "保存成功！该报警已结束，请及时提交。" }, JsonRequestBehavior.AllowGet);
                        
                        }
                      
                    
                    }

                
                }
               
         
        }
        /// <summary>
        /// 加载所有煤矿报警统计信息
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        public void GetAllAlarmCount(string mineCode)
        {
         
            try
            {
                Response.Buffer = true;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
                Response.Expires = 0;
                Response.CacheControl = "no-cache";
                Response.AddHeader("Pragma", "No-Cache");

                string queryString = string.Empty;

                if (!string.IsNullOrEmpty(mineCode) && mineCode.ToLower() != "null")
                {
                    queryString = string.Format(" Where MineCode Like '%{0}%'", mineCode);
                }
                else
                {
                    queryString = "";
                }

                string sql = string.Format(@"With U As(
	Select * From (
        -- 基站报警
        select '分站停用' AlarmType, MineCode,'人员管理系统' AlarmGroup,Place SensorName From  RYFZ where StationState=1

        union ALL
        select '分站检修' AlarmType, MineCode,'人员管理系统' AlarmGroup,Place SensorName From  RYFZ where StationState=2
        union all
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
Select COUNT(1) As AlarmCount From
(
	--通讯状态报警信息
	Select SimpleName, Case [StateCode] When 1 Then '通讯中断报警' When 2 Then '传输异常报警' End AlarmType, M.MineCode, Name AlarmGroup, '' SensorName
		From (
       Select * From SystemConfig S Where StateCode <> 0) S
		Left Join MineConfig M On S.MineCode = M.ID 
	Union All
	-- 人员管理及安全监控报警信息
	Select SimpleName, AlarmType, M.MineCode, U.AlarmGroup, U.SensorName From U Left Join MineConfig M On U.MineCode = M.MineCode 
		--Where U.MineCode Not In (Select M.MineCode From SystemConfig S Left Join MineConfig M On S.MineCode = M.ID Where StateCode <> 0)
) As U {0}
", queryString);

                var dal = new DataDAL();
                var dt = dal.ReturnData(sql);
                var data = JsonConvert.SerializeObject(dt);

                Response.Write(data);
                Response.End();
            }
            catch (Exception)
            {
                

                
            }
         
        }

        ///获取当前下井人数
        public ActionResult LoadXJRS(string MineCode)
        {
            string sql = "select isnull(count(1),0)  Counts from ryss where inouttype=1 ";
            if (!string.IsNullOrEmpty(MineCode))
            {
                sql += " and MineCode ='" + MineCode + "'";
            }
            try{


                var dal = new DataDAL();
                var dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {
                    return Json(new { data = dt.Rows[0][0].ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
                }
            
            }
            catch(Exception e)
            {
                return Json(new { data = "Error!" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 加载所有煤矿报警信息
        /// </summary>
        /// <param name="MineCode">煤矿编号</param>
        public void GetAllAlarmInfo(string MineCode, string AccountID)
        {
           // try
           // {
                Response.Buffer = true;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
                Response.Expires = 0;
                Response.CacheControl = "no-cache";
                Response.AddHeader("Pragma", "No-Cache");

                if (MineCode == null)
                {
                    MineCode = "";
                }
                RealDataModel realDataModel = new RealDataModel();
                string AlarmMessage = realDataModel.GetAllAlarmInfo(MineCode, AccountID);
                Response.Write(AlarmMessage);
                Response.End();
           // }
            //catch
           // {
                
           // }
        }

        public void GetMineName()
        {
            RealDataModel realDataModel = new RealDataModel();
            string AlarmMessage = realDataModel.GetMineName();
            Response.Write(AlarmMessage);
            Response.End();
        }

        /// <summary>
        /// 根据帐户 Id 获取报警设置状态
        /// </summary>
        /// <param name="accountId"></param>
        public void GetAlarmSetState(string accountId)
        {
            try
            {
                Response.Buffer = true;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
                Response.Expires = 0;
                Response.CacheControl = "no-cache";
                Response.AddHeader("Pragma", "No-Cache");

                string sql =
                    string.Format(@"Select AlarmType, T.AlarmGroup, IsNull([Disabled], 1) [Disabled] From dbo.AlarmTypeInfo T
	Left Join 
	(Select * From dbo.AlarmSet Where AccountId = '{0}') S
	On T.AlarmType = S.LinkData", accountId);

                var dal = new DataDAL();
                var dt = dal.ReturnData(sql);
                var data = JsonConvert.SerializeObject(dt);

                Response.Write(data);
                Response.End();
            }
            catch (Exception)
            {

            }
           
        }

        public void SetAlarmSetItem(string accountId, string alarmGroup, string disabled, string linkData)
        {
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "No-Cache");
            string sql;

            if (alarmGroup.Equals("99"))
            {
                sql = string.Format(@"Delete From [ShineView_Data].[dbo].[AlarmSet] Where [AccountId] = '{0}' And [AlarmGroup] = {1} And [LinkData] = '{2}' ", accountId, alarmGroup, linkData);
                sql += string.Format(@"INSERT INTO [ShineView_Data].[dbo].[AlarmSet]([AccountId],[AlarmGroup],[Disabled],[LinkData])VALUES('{0}', {1},{2},'{3}') ", accountId, alarmGroup, disabled, linkData);
            }
            else
            {
                sql = string.Format(@"Delete From [ShineView_Data].[dbo].[AlarmSet] Where [AccountId] = '{0}' And [AlarmGroup] = {1} And [LinkData] = '{2}' ", accountId, alarmGroup, linkData);
                //event before is  false ;  event have been is  true;
                //if (disabled.Equals("false")) //
                {
                    sql += string.Format(@"INSERT INTO [ShineView_Data].[dbo].[AlarmSet]([AccountId],[AlarmGroup],[Disabled],[LinkData])VALUES('{0}', {1},{2},'{3}') ", accountId, alarmGroup, disabled.Equals("true") ? 1 : 0, linkData);
                }
            }

            var dal = new DataDAL();

            if (dal.ExcuteSql(sql))
            {
                if (!disabled.Equals("true"))
                {
                    Response.Write(string.Format("{0} 更新成功, 状态为报警", linkData));
                }
                else
                {
                    Response.Write(string.Format("{0} 更新成功, 状态为不报警", linkData));
                }
            }
            else
            {
                if (!disabled.Equals("true"))
                {
                    Response.Write(string.Format("{0} 更新失败, 状态为报警", linkData));
                }
                else
                {
                    Response.Write(string.Format("{0} 更新失败, 状态为不报警", linkData));
                }
            }

            Response.End();
        }


        public  class MenuInfo
        {
            public string MenuName { get; set; }
              public string MenuPath { get; set; }
              public List<MenuInfo> subMenu { get; set; }
        
        }


        public T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        public void ReturnMenus(string userName, string mineCode)
        {
            
            // LoginModel loginModel = new LoginModel();
            //DataTable dt=new DataTable();
            //loginModel.LoadMenuDynamic(userName, mineCode);
            //  Response.Write(loginModel.ReturnMenus);
            //Response.End();
           
       
            //{"MenuName":"导航","MenuPath":"../DH/DH?test=test&UserAbility=0&MineCode=34060000001","subMenu":[]}

           // string xx = loginModel.GetWeather("http://api.map.baidu.com/telematics/v3/weather?location=兴义&output=json&ak=1Be18hYdbDVirh7pTk6UdEst", "utf-8")
            try
            {
                LoginModel loginModel = new LoginModel();
                loginModel.LoadMenuDynamic(userName, mineCode);
                DataTable dt = new DataTable();
                string json = loginModel.ReturnMenus;
                //List<MenuInfo> menuList = Deserialize<List<MenuInfo>>(json);
                //string sql = "select  ct.ChildSysCode ,ct.ChildSysName, ChildSysUrl   from ChildSysConfig  cs left join ChildSysTypeInfo  ct on  cs.ChildSysCode = ct.ChildSysCode  ";
                //List<MenuInfo> childSysList = new List<MenuInfo>();
                //try
                //{
                //    dt = dal.ReturnData(sql);
                   

                //}
                //catch (Exception)
                //{
                //    dt = null;
                //}
               
                //    for (int i = 0; i < menuList.Count; i++)
                //    {
                //        if (menuList[i].MenuPath.IndexOf("RYSS") > 0)
                //        {
                //            if (dt != null)
                //            {
                //                for (int j = 0; j < dt.Rows.Count; j++)
                //                {
                //                    MenuInfo mi = new MenuInfo();
                //                    mi.MenuName = dt.Rows[j]["ChildSysName"].ToString();
                //                    //../DH/ChildSys?SysCode=YFST
                //                    mi.MenuPath = "../DH/ChildSys?SysCode=" + dt.Rows[j]["ChildSysCode"].ToString();
                //                    mi.subMenu = null;
                //                    menuList.Insert(i+1, mi);
                //                }
                //            }
                //        }
                //    }
                //json = JsonConvert.SerializeObject(menuList, Formatting.Indented);
                Response.Write(json);
                Response.End();
            }
            catch (Exception e)
            {
                Response.Write("<script language='javascript'>alert('获取菜单失败！错误没人："+e.Message+"');</script>");
                Response.End();
            }
          
        }


        public ActionResult RealDataTest()
        {
            RealDataModel rdModel = new RealDataModel();
            return View(rdModel);
        }


        public ActionResult AQGZ()
        {
            RealDataModel rdModel = new RealDataModel();
            return View(rdModel);

        }




        public void ReturnMineList(string Type)
        {
            Response.Buffer = true;
             Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "No-Cache");

            BaseInfoModel model = new BaseInfoModel();
            Response.Write(model.ReturnTreeMine(Type));
            Response.End();


        }
    }
}
