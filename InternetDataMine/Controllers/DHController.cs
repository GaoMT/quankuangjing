﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using InternetDataMine.Models.DataService;
using System.Drawing;
using InternetDataMine.Controllers;
using System.Drawing.Imaging;
using System.IO;
namespace InternetDataMine.Controllers
{
    public class DHController : Controller
    {


        // GET: /DH/
        DataDAL dal = new DataDAL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DH()
        {
            return View();
        }
        public ActionResult Navigation()
        {
            string ChildSys = "";
            string ChildName = "";
            string sql = "select ChildSysCode,ChildSysName from ChildSysConfig";
            try
            {
                DataTable dt = dal.ReturnData(sql);
                if (dt != null)
                {

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ChildSys += dt.Rows[i][0] + ",";
                            ChildName += dt.Rows[i][1] + ",";
                        }
                    }
                }
                ViewBag.ListName = ChildName;
                ViewBag.list = ChildSys;
            }
            catch (Exception)
            {
                ViewBag.list = "";
                ViewBag.ListName = "";
            
            }
            return View();
        }
        public ActionResult DHConfig(string SysCode)
        {
            ViewBag.Message = SysCode;
            return View();
        }
        public ActionResult ChildSysControl(string SysCode)
        {
            ViewBag.ChildSys = SysCode;
            return View();
        
        }

        public ActionResult RealData(string SysCode)
        {
            ViewBag.Message = SysCode;
            return View();
        }
        public ActionResult ChildSys(string SysCode)
        {
            ViewBag.Message = SysCode;
            string SysUrl = "";
            string sql = "select ChildSysUrl from ChildSysConfig where ChildSysCode ='" + SysCode + "'";
            try
            {
                SysUrl = dal.ReturnData(sql).Rows[0][0].ToString();
                ViewBag.Url = SysUrl;
            }
            catch (Exception e)
            {
                ViewBag.Url = "";
            }
            return View();
        }

        public ActionResult ChildSysConfig()
        {
            return View();
        }
        public ActionResult LoadGraph(string SysCode)
        {
            string sys = "";

            if (string.IsNullOrEmpty(SysCode))
            {
                sys = "DH";
            }
            else
            {
                sys = SysCode;
            }
            string sql = "select  ImgSrc from GraphConfig where SysCode='" + sys + "'";
            try
            {

                DataTable dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {

                    return Json(new { data = dt.Rows[0][0].ToString() }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        

      
        }


        public void LoadChildSys()
        {
            string sql = "SELECT ChildSysCode id ,ChildSysName from ChildSysTypeInfo ";
            try
            {
                DataTable dt = dal.ReturnData(sql);

                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                Response.Write(json);
                Response.Flush();
                Response.End();

            }
            catch (Exception e)
            {
                Response.Write("");
                Response.Flush();
                Response.End();
            }
        
        }

        public ActionResult SaveChildonfig(string SysCode, string SysName, string SysUrl,string DHLoad)
        {
            try
            {
                string sql = string.Format("if exists (select *  from ChildSysConfig where ChildSysCode = '{0}')" +
                                              " begin" +
                                              "    update ChildSysConfig" +
                                              "    set ChildSysName = '{1}' ,ChildSysUrl='{2}',DHLoad="+DHLoad +
                                              "    where ChildSysCode = '{0}'" +
                                              " end" +
                                           " else" +
                                             "  begin" +
                                              "    insert into ChildSysConfig(ChildSysCode,ChildSysName,ChildSysUrl,DHLoad)  values ('{0}','{1}','{2}',{3})" +
                                              " end ", SysCode, SysName, SysUrl, DHLoad);
                bool success = dal.ExcuteSql(sql);
                if (success)
                {
                    return Json(new { data = "保存成功" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "保存失败，请检查数据是否合法" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            { 
             return Json(new { data = "保存失败"+e.Message }, JsonRequestBehavior.AllowGet);
            
            }
           
        }


        public void LoadChildSysConfig()
        {
            string sql = "select row_number() over (order by ChildSysCode asc) as TmpID ,ChildSysCode,ChildSysName,ChildSysUrl,case DHLoad when 0 then '模拟图' when 1 then '系统详情页' end  DHLoad from  ChildSysConfig";
            try
            {
                DataTable dt = dal.ReturnData(sql);
                int total = dt.Rows.Count;
                string json= JsonConvert.SerializeObject(dt, Formatting.Indented);
               json= "{\"total\": " + total + ",\"rows\":" + json + "}";
                Response.Write(json);
                 Response.Flush();
                Response.End();

            }
            catch (Exception e)
            { 
                Response.Write("");
                 Response.Flush();
                Response.End();
            
            }
        
        }


        public ActionResult DeleteSysConfig(string Condition)
        {

            if (!string.IsNullOrEmpty(Condition))
            {
                string[] SysCode = Condition.Split(new char[] { ',' });
                string sql= "delete from ChildSysConfig where ChildSysCode  in (";
                string conn="";
                for (int i = 0; i < SysCode.Length; i++)
                {
                    conn += string.Format("'{0}',", SysCode[i]);
                }
                sql += conn.Substring(0, conn.Length-1) + ") delete from GifConfig where  SysCode in ( ";
                sql += conn.Substring(0, conn.Length - 1) + ") delete from GraphConfig where  SysCode in ( ";
                sql += conn.Substring(0, conn.Length - 1) + ")";
                bool success = dal.ExcuteSql(sql);
                if (success)
                {
                    return Json(new { data = "删除成功" }, JsonRequestBehavior.AllowGet);

                }
                else {
                    return Json(new { data = "删除失败" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { data = "获取子系统编号失败" }, JsonRequestBehavior.AllowGet);
            }
        }


        public void LoadSysList()
        {

            string sql = "select 'DH'  as id , '导航' as ChildSysName  union all  select ChildSysCode id ,ChildSysName from ChildSysConfig";
            try
            {
                DataTable dt = dal.ReturnData(sql);
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                Response.Write(json);
                Response.Flush();
                Response.End();
            }
            catch (Exception e)
            {
                Response.Write("");
                Response.Flush();
                Response.End();
            }
        }
        //读取input文件内容
        private byte[] GetFileContent()
        {

            HttpFileCollectionBase files = Request.Files;//这里只能用<input type="file" />才能有效果,因为服务器控件是HttpInputFile类型
            string msg = string.Empty;
            string error = string.Empty;
            if (files.Count > 0)
            {
                if (files[0].FileName == "")
                {
                    return null;
                }
                else
                {
                    byte[] Buffer = new byte[files[0].ContentLength];
                    files[0].InputStream.Read(Buffer, 0, Buffer.Length);
                    return Buffer;
                }
            }
            else
            {
                return null;
            }
        }

      
        
        
        /// <summary>
        /// 加载导航中的第一个表格
        /// </summary>
        /// <returns></returns>
        public string ShowMC()
        {
            string sql = "select Unit,g.id  ImgID,g.Tag,g.[Value],s.Place,ShowValue from aqss s left join MineConfig m on s.MineCode = m.MineCode  left join GifConfig g on s.SensorNum=g.SensorNum  left join aqmc mc on mc.sensornum=s.SensorNum where s.place LIKE '%煤仓%' ";

            try {
               
                DataTable dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {
                    int TotalRows = dt.Rows.Count;
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    string json = JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
                    string ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";
                    return ReturnString;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                return null;

            }
        
        }

      
        
        
        /// <summary>
        /// 加载导航中的第二个表格
        /// </summary>
        /// <returns></returns>
        public string ShowZAX()
        {
            string sql = "select Unit,g.id  ImgID,g.Tag,g.[Value], s.Place,ShowValue from aqss s left join MineConfig m on s.MineCode = m.MineCode left join GifConfig g on s.SensorNum=g.SensorNum left join aqmc mc on mc.sensornum=s.SensorNum   where s.place LIKE '%主暗斜%'";

            try
            {

                DataTable dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {
                    int TotalRows = dt.Rows.Count;
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    string json = JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
                    string ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";
                    return ReturnString;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;

            }

        }

        // 所有gif动画关联的测点的实时数据



        public string ShowGifTable(string SysCode)
        {
            
            string sql = "";
            string Sys = (string.IsNullOrEmpty(SysCode)) ? "DH" : SysCode;
            if (Sys == "DH")
            {
                sql = "select s.SensorNum, isnull(d.Unit,'')Unit , imagesrc,g.Angle,g.ID,g.Tag,g.Value,s.Value CurrentValue,s.ShowValue,s.Place,d.TypeName,sc.StateCode,s.ValueState," +
                        " case d.Type when 'A' then '模拟量'  when 'D' then '开关量' when 'C' then '控制量' when 'L' then '累积量'  end Type" +
                        " from aqss s  "+
                        " left join DeviceType d on d.TypeCode=s.Type "+
                        " left join GifConfig g  on g.SensorNum= s.SensorNum left join SystemConfig sc on sc.TypeCode= 1 " +
                        " where s.SensorNum in (select SensorNum from GifConfig)";

            }
            else
            {
                string TableName = "";
                TableName = SysCode.Substring(0, 2);
                sql = "select g.ID,g.SensorNum,ISNULL(Unit,'') Unit,g.Angle,g.Value,g.Tag,imagesrc," +
                       "VarValue CurrentValue," +

                       " case EquipType when 0 then cast(VarValue as VARCHAR) when 1 then "+
                       " (case VarValue when 0 then  '开' when 1 then '关' when 2 then KaiGuan_2_Show end )end  ShowValue,"+
                       " EquipPlace Place,"+
                       " case EquipType when 0 then '模拟量' when 1 then '开关量' when 2 then '控制量' when 3 then '累积量' end Type,"+
                       " EquipName TypeName,VarStatus StateCode"+
                       " from   [" + TableName + "_RealValues] tr" +
                       " LEFT JOIN " + TableName + "_Points tp on tp.PointID= tr.PointID " +
                       " LEFT JOIN " + TableName + "_Equipment te  on  te.id=tp.EquipID " +
                       " left JOIN GifConfig g on g.SensorNum=tp.PointName "+
                       " where g.SensorNum in "+
                       " (select SensorNum from GifConfig where SysCode='"+SysCode+"') ";
            }
        
            try
            {

                DataTable dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {
                    int TotalRows = dt.Rows.Count;
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    string json = JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
                    string ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";
                    return ReturnString;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;

            }
        
        }
      
        
        
        public ActionResult ShowPeople()
        {
            string sql = "select  Row_Number() over (order by  mc.MineCode asc) as TmpID, mc.MineCode ,mc.SimpleName,isnull(T1.AllPeople,0) AllPeople,isnull( InMinePeople,0) InMinePeople , " +
                                   "  isnull(InMineLeader,0) InMineLeader,isnull(RYCY,0) RYCY,isnull(RYCS,0) RYCS,isnull(ZDQY,0) ZDQY " +
                                   "   from MineConfig mc " +
                                   "  left join SystemConfig sc " +
                                   "  on sc.MineCode = mc.ID " +
                                   "  left join " +
                                   "  (select count(1) AllPeople,MineCode from RYXX  where SystemType=2 GROUP BY (MineCode) ) T1 " +
                                   "  ON T1.MineCode =mc.MineCode  " +
                                   "  left join  " +
                                   "  (select count(1) InMinePeople ,ss.MineCode from RYSS   ss " +
                                   "  LEFT JOIN RYXX  xx " +
                                   "  ON   xx.MineCode =ss.MineCode and xx.JobCardCode=ss.JobCardCode " +
                                   "   where ss.SystemType=2  and  InOutType=1 GROUP BY ss.MineCode ) T2 " +
                                   "  on T2.MineCode=mc.MineCode " +
                                   "  left join  " +
                                   "  (select count(1) InMineLeader ,ss.MineCode from RYSS ss " +
                                   "   left JOIN  RYXX xx   " +
                                   "  on xx.MineCode = ss.MineCode  and ss.JobCardCode=xx.JobCardCode " +
                                   "   where ss.SystemType=2  and  ss.InOutType=1 and ( xx.[Position] like '%矿长%'  or xx.[Position] like '%领导%'  )  GROUP BY ss.MineCode ) T3 " +
                                   "  on T3.MineCode=mc.MineCode " +
                                   "  left join  " +
                                   "  (select count(1) RYCS ,cs.MineCode from RYCS cs  " +
                                   "  left join ryxx xx " +
                                   "  on xx.MineCode =cs.MineCode and xx.JobCardCode=cs.JobCardCode  where cs.SystemType=2   and (EndAlTime like 'x%' or EndAlTime like 'X%')  GROUP BY cs.MineCode ) T4 " +
                                   "  on T4.MineCode=mc.MineCode " +
                                   "  left join  " +
                                    " (select count(1) RYCY ,cy.MineCode from RYCYXZ  cy " +
                                    " LEFT JOIN   ryxx xx " +
                                    " on xx.MineCode=cy.MineCode and cy.JobCardCode=xx.JobCardCode where cy.SystemType=2   and (EndAlTime like 'x%' or EndAlTime like 'X%')  GROUP BY cy.MineCode ) T5 " +
                                    " on T5.MineCode=mc.MineCode " +
                                    " left join  " +
                                    " (select count(1) ZDQY ,ss.MineCode from RYSS ss " +
                                    "  left JOIN  RYXX xx   " +
                                    " on xx.MineCode = ss.MineCode  and ss.JobCardCode=xx.JobCardCode " +
                                    " LEFT JOIN RYQY qy  " +
                                    " on qy.MineCode = ss.MineCode and qy.AreaCode=ss.AreaCode " +
                                     " where ss.SystemType=2  and qy.AreaType like '%重点区域%'   GROUP BY ss.MineCode ) T6 " +
                                     " on T6.MineCode=mc.MineCode " +
                                     " where sc.isenabled=1 and sc.TypeCode=2 ";
            try
            {

                DataTable dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {
                    string json = "当前在册人数：" + dt.Rows[0]["AllPeople"] + "人，下井人数：" + dt.Rows[0]["InMineLeader"] + "人，下井领导数：" + dt.Rows[0]["InMineLeader"] + "人";
                    string bj = "超时人数：" + dt.Rows[0]["RYCS"] + "人，超员人数：" + dt.Rows[0]["RYCY"] + "人";
                    return Json(new { data=json ,bj=bj}, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    string json = "当前在册人数：0人，下井人数：0人，下井领导数：0人";
                    string bj = "<br>超时人数：0人，超员人数：0人";
                    return Json(new { data = json,bj=bj }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception e)
            {

                return null;
            }
          
        }


        public ActionResult LoadGif(string SysCode)
        {
            string Sys = "";
            string sql = "";
            if (string.IsNullOrEmpty(SysCode))
            {       
                Sys="DH";
            }
            else 
            {
                Sys = SysCode;
            
            }
            if (Sys == "DH")
            {
                sql = "SELECT   g.[ID] as DivID,g.SensorNum,isnull(d.Unit,'') Unit,s.Place ,sc.StateCode, g.Tag,g.Angle," +
                       " case d.Type when 'A' then '模拟量' when 'D' then '开关量' when 'L' then '累积量' when 'C' then '控制量' end Type, d.Type  as Types,"+
                       " TypeName,d.TypeCode,s.[ShowValue],s.value,g.value as  setValue,[Index],g.[Name],[ImageSrc],[RateW],[RateH] ,[RateL],[RateT]  "+
                       " FROM [ShineView_Data].[dbo].[GifConfig] g  "+
                       " left join aqss s on s.SensorNum=g.SensorNum  "+
                       " left join DeviceType d on d.TypeCode= s.Type "+
                       " left join SystemConfig sc on sc.TypeCode=1  "+
                        " where  SysCode ='"+ Sys + "' order by  [Index] asc";
                                    }
            else
            {
                string TableName = "";
                TableName = SysCode.Substring(0, 2);
                sql = "select g.[ID] as DivID, g.[Value] setValue,g.Tag,g.SensorNum,"+
                     
                       " isnull(case EquipType when 0 then  cast(VarValue as VARCHAR) when 1 THEN  (case  VarValue when 0 then KaiGuan_0_Show when 1 then KaiGuan_1_Show when 2 then KaiGuan_2_Show end) end ,'') as [ShowValue]," +
                       " g.Angle,isnull(cast(VarValue as VARCHAR ),'') [Value],isnull(Unit,'')Unit ,EquipPlace Place,[Index],[Name]," +
                       " [ImageSrc],[RateW],[RateH] ,[RateL],[RateT] ,EquipName TypeName, EquipName TypeCode," +
                       " VarStatus StateCode,"+
                       " case EquipType  when 0 then '模拟量' when 1 then '开关量' when 2 then '控制量' when 3 then '累积量' end as  Type , "+
                       " case EquipType when 0 then 'A' when 1 then 'D' when 2 then 'C' when 3 then 'L' end as  Types " +
                        " FROM [ShineView_Data].[dbo].[GifConfig] g"+
                        " left join  "+TableName+"_Points tp on  tp.PointName = g.SensorNum"+
                        " left join  "+TableName+"_Equipment  te on  te.ID= tp.EquipID "+
                        " left join  ["+TableName+"_RealValues] tr on tr.PointID=tp.PointID  where  g.SysCode ='" + Sys + "'";
            }
            try
            {
                DataTable dt = dal.ReturnData(sql);

                //if (dt != null && dt.Rows.Count > 0)
                //{

                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        if (dt.Rows[i]["Tag"].ToString() == "6")
                //        {
                //        string[] Sensor = dt.Rows[i]["SensorNum"].ToString().Split(new char[] { '|' });
                //            for (int j = 0; j < Sensor.Length - 1; j++)
                //            { 
                //             if(Sensor.)
                            
                //            }

                //        }
                //    }
                //}
                    
              string json=  JsonConvert.SerializeObject(dt, Formatting.Indented);
              return Json(new { data = json }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception e)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        
        }


        public ActionResult SaveConfig(string IDs, string SensorNums, string Values, string Tags, string Indexs, string ImgSrcs, string RateWs, string RateHs, string RateLs, string RateTs, string SysCode, string IsEdits, string IsChangeDT, string DT, string Tip, string Angles)
        {
            try
            {
                List<string> sql = new List<string>();
                if (Tip == "0")
                { 
                    string[] ID = IDs.Split(new char[] { ',' });
                    string[] RateL = RateLs.Split(new char[] { ',' });
                    string[] RateT = RateTs.Split(new char[] { ',' });
                    string[] IsEdit = IsEdits.Split(new char[] { ',' });
                    string[] RateW = RateWs.Split(new char[] { ',' });
                    string[] RateH = RateHs.Split(new char[] { ',' });
                    string[] Index = Indexs.Split(new char[] { ',' });

                    string[] SensorNum = SensorNums.Split(new char[] { ',' });
                    string[] Value = Values.Split(new char[] { ',' });
                    string[] Tag = Tags.Split(new char[] { ',' });
                    string[] ImgSrc = ImgSrcs.Split(new char[] { ',' });
                    string[] Angle = Angles.Split(new char[] { ',' });
              
                    for (int i = 0; i < ID.Length; i++)
                    {
                       // 新增
                        if (ID[i].IndexOf("New") >= 0)
                        {
                            //,SensorNum,[Value],Tag,ImgSrc
                            string TempSql = "insert into GifConfig (Name,[Index],RateW,RateH,RateL,RateT,SysCode,Angle ";
                            string TempValue = ") values ('" + ID[i] + "','" + Index[i] + "','" + RateW[i] + "','" + RateH[i] + "','" + RateL[i] + "','" + RateT[i] + "','" + SysCode + "','" + Angle[i] + "'";
                            if (SensorNum[i] != "---")
                            {
                                TempSql += ",SensorNum";
                                TempValue += ",'" + SensorNum[i] + "'";
                            }
                            if (Value[i] != "---")
                            {
                                TempSql += ",[Value]";
                                TempValue += ",'" + Value[i] + "'";
                            }
                            if (Tag[i] != "---")
                            {
                                TempSql += ",[Tag]";
                                TempValue += ",'" + Tag[i] + "'";
                            }
                            if (ImgSrc[i] != "---"  && ImgSrc[i]!="")
                            {
                                TempSql += ",[ImageSrc]";
                                TempValue += ",'../" + ImgSrc[i].Substring(ImgSrc[i].IndexOf("Content"), ImgSrc[i].Length - ImgSrc[i].IndexOf("Content")) + "'";
                            }
                            sql.Add(TempSql + TempValue + ")");

                        }
                        else
                        {
                            //修改
                            string ChangeSQL = "";
                     
                                ChangeSQL += string.Format(" RateL='{0}', RateT='{1}',", RateL[i], RateT[i]);
                                ChangeSQL += string.Format(" Value='{0}',", Value[i]);
                                ChangeSQL += string.Format(" RateW='{0}', RateH='{1}',", RateW[i], RateH[i]);
                                ChangeSQL += string.Format(" Tag='{0}',", Tag[i]);
                                ChangeSQL += string.Format(" SensorNum='{0}',", SensorNum[i]);
                                ChangeSQL += string.Format(" Angle='{0}',", Angle[i]);
                                ChangeSQL += "[Index]=" + Index[i];
                            sql.Add("update GifConfig set " + ChangeSQL + " where ID=" + ID[i].Substring(ID[i].IndexOf("v") + 1, ID[i].Length - ID[i].IndexOf("v")-1));
                        }
                    }
                }
                if (IsChangeDT == "1")
                {
                   string  temp=   string.Format("if exists (select *  from GraphConfig where SysCode = '{0}')"+
                                           " begin"+
                                           "    update GraphConfig"+
                                           "    set ImgSrc = '{1}'" +
                                           "    where SysCode = '{0}'"+
                                           " end"+
                                        " else"+
                                          "  begin"+
                                           "    insert into GraphConfig(SysCode,ImgSrc)  values ('{0}','{1}')" +
                                           " end ", SysCode,"../"+ DT.Substring(DT.IndexOf("Content"),DT.Length-DT.IndexOf("Content")));
                   sql.Add(temp);
                }
               
                bool resul = dal.ExcuteSqls(sql);

                return Json(new { data = resul }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { data = "出错："+e.Message }, JsonRequestBehavior.AllowGet);

            }
           
        }

        public ActionResult SaveText(string TextContent, string color, string SensorNum,string SysCode,string ID)
        {
            string sql = "";
            string data="添加失败";
            if (string.IsNullOrEmpty(ID))
            {

                sql = string.Format("insert into GifConfig(Name,SensorNum,[Value],RateL,RateT,[Index],SysCode) values ('{0}','{1}','{2}',0.5,0.5,100,'{3}')", TextContent, SensorNum, color, SysCode);


            }
            else
            {
                sql = string.Format("update GifConfig set Name='{0}',SensorNum='{1}',[Value]='{2}' where ID={3}", TextContent, SensorNum, color, ID);
            }
            try
            {
                bool resul = dal.ExcuteSql(sql);
                if (resul)
                {
                    data = "添加成功";
                }
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data ="出错:"+ e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult LoadSingleText(string ID)
        {  
            ID = ID.Replace("div", "");
            string sql = string.Format("select ID,Name,SensorNum,[Value] from GifConfig where ID={0}", ID);
            try
            {
                DataTable dt = dal.ReturnData(sql);
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                return Json(new { data = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        
        }
       /// <summary>
       /// 修改或保存动画
       /// </summary>
       /// <param name="Name">动画的名称，保存后名称为Name+当前年月日时分秒</param>
       /// <param name="SensorNum">关联测点</param>
       /// <param name="Index">动画加载顺序 数值越低，动画在越底层</param>
       /// <param name="ImgSrc">动画的路径，获取后缀</param>
       /// <param name="ID">动画的id，有id则为修改，没有则是添加</param>
       /// <param name="Tag">关联测点值符号0大于1等于2小于</param>
       /// <param name="Value">关联测点值，与符号连用</param>
        public void SaveAdd(string Name, string SensorNum, string Index, string ImgSrc, string ID, string Tag, string Value, string SysCode)
        {
            string Message = "{data:'操作失败！'}";
            try
            {
               
                if (string.IsNullOrEmpty(ID))
                {
                    byte[] ReadFileResult =  GetFileContent();
               
                    int index = Convert.ToInt32(Index);
                    //byte[] imgByte = System.IO.File.ReadAllBytes(@ImgSrc);
                    GrapSystemController GS = new GrapSystemController();
                    Image img = GS.byteArrayToImage(ReadFileResult);
                    float width = img.Width;
                    float height = img.Height;
                    float rateW = width / 1263;
                    float rateH = height / 555;
                    double showValue;
                    if (string.IsNullOrEmpty(Value))
                    {
                        showValue = -1;
                    }
                    else
                    { 
                         showValue = Convert.ToDouble(Value);
                    }
                    string fo = ImgSrc.Substring(ImgSrc.LastIndexOf('.'), ImgSrc.Length - ImgSrc.LastIndexOf('.'));
                    string imgName = Name+ DateTime.Now.ToString("yyyyMMddHHmmss");
                    string ImgSrcNew = "../Content/gif/" + imgName + fo;

                    if (fo.IndexOf("gif") < 0)
                    {
                        using (Bitmap bitmap = new Bitmap(img))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                bitmap.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\gif\\" + imgName + fo);

                            }
                        }

                    }
                    else
                    {
                        img.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\gif\\" + imgName + fo);
                    }
                        img.Dispose();



                        string sql = string.Format("insert into GifConfig (SensorNum,[Index],ImageSrc,RateW,RateH,RateL,RateT,Name,Tag,Value,SysCode) values ('{0}',{1},'{2}',{3},{4},0.5,0.5,'{5}',{6},'{7}','{8}')", SensorNum, Index, ImgSrcNew, rateW, rateH, Name, Convert.ToInt32(Tag), showValue, SysCode);
                    
                    bool resul = dal.ExcuteSql(sql);

                    if (resul)
                    {
                        Message = "{ error:'', msg:'添加成功！'}";
                    }

                }
                else
                {
                    int index = Convert.ToInt32(Index);
                    string sql = string.Format("update GifConfig set Name='{0}',SensorNum='{1}',[Index]='{2}',Tag='{4}',Value='{5}'  where ID={3}", Name, SensorNum, Index, ID,Convert.ToInt32(Tag), string.IsNullOrEmpty(Value)?-1: Convert.ToDouble(Value));
                    bool resul = dal.ExcuteSql(sql);

                    if (resul)
                    {
                        Message = "{ error:'', msg:'修改成功！'}";
                    }
                  

                }
                Response.Write(Message);
                Response.End();

            }
            catch (Exception e)
            {
                Response.Write("{error:'',msg:'操作失败:" + e.Message + "'}");
                Response.End();

            }
          
        }
      
        
        /// <summary>
        /// 加载单个动画，用于编辑
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult LoadSingleGif(string ID)
        {
            try
            {
                ID = ID.Replace("div", "");
                string sql = "select * from GifConfig where [ID]=" + ID;
                DataTable dt = dal.ReturnData(sql);
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                return Json(new { data = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        
        }
       
        
        
        /// <summary>
        /// 删除动画
        /// </summary>
        /// <param name="DeleteList"></param>
        /// <returns></returns>
        public ActionResult DeleteGif(string DeleteList)
        {
            try
            {
              DeleteList=  DeleteList.Replace("div", "");

                

              string sql = (string.Format("delete from GifConfig where [ID]={0}", DeleteList));

              
                bool resul = dal.ExcuteSql(sql);
                if (resul)
                {
                    return Json(new { data = "删除成功" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { data = "删除失败"}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }
         
        }


        public ActionResult SaveDT(string SysCode,string Src)
        {
            string Sys = (string.IsNullOrEmpty(SysCode)) ? "DH" : SysCode;
            string Message = "";
            try
            {

                string sql = string.Format("if exists (select *  from GraphConfig where SysCode = '{0}')"+
                                           " begin"+
                                           "    update GraphConfig"+
                                           "    set ImgSrc = {1}" +
                                           "    where SysCode = '{0}'"+
                                           " end"+
                                        " else"+
                                          "  begin"+
                                           "    insert into GraphConfig(SysCode,ImgSrc)  values ('{0}',{1})" +
                                           " end ", Sys, Src);
                bool resul = dal.ExcuteSql(sql);

                if (resul)
                {
                    Message = "";
                }
                else
                {
                    Message = "修改失败!";
                }
                return Json(new { data = Message }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                Message =e.Message;
                return Json(new { data = Message }, JsonRequestBehavior.AllowGet);
            }
        
        }



        public ActionResult LoadConfigImg()
        {
          

            try
            {
                string sql = "select * from ImgResource order by [ID]";
                DataTable dt = dal.ReturnData(sql);
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                return Json(new { data = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        
        }


        public void SaveFile(string Remark,string ImgName)
        {
        
            string Messgae = "";
            string Name = "";
          
            try
            {
                string fo = ImgName.Substring(ImgName.LastIndexOf('.'), ImgName.Length - ImgName.LastIndexOf('.'));
               
                if (fo.ToUpper() != ".JPG" && fo.ToUpper() != ".GIF" && fo.ToUpper() != ".BMP" && fo.ToUpper() != ".JPEG" && fo.ToUpper() != ".PNG")
                {
                 
                    Messgae = "{ error:'', msg:'图片格式不正确！图片仅支持jpg、gif、bmp、jpeg、png格式！'}";
                 
                    Response.Write(Messgae);
                    Response.End();
                }
                else
                {
                    //"123\\456-1).789".Substring("123\\456-1).789".LastIndexOf('\\')+1, "123\\456-1).789".LastIndexOf('.')-"123\\456-1).789".LastIndexOf("\\")-1)
                    Name = ImgName.Substring(ImgName.LastIndexOf('\\') + 1, ImgName.LastIndexOf('.') - ImgName.LastIndexOf("\\") - 1);
                    
                    byte[] ReadFileResult = GetFileContent();

                    GrapSystemController GS = new GrapSystemController();
                    Image img = GS.byteArrayToImage(ReadFileResult);
                
                    //原始宽高
                    int ImgWidth=img.Width;
                    int ImgHeight=img.Height;

                    string ImgSrcNew = "";
                    string Path="";
                    switch(Remark)
                    {
                        case "0": Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\gif\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fo;
                            ImgSrcNew = "../Content/gif/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fo;
                                break;
                        case "1": Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\GroundImg\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fo;
                                ImgSrcNew = "../Content/GroundImg/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fo;
                                break;
                        case "2": Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\jpg\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fo;
                                ImgSrcNew = "../Content/jpg/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fo;
                                break;
                    }
                  

                    if (Remark!="0")
                    {
                     
                        if(ImgWidth>5052)
                        {
                           //重新赋宽高 1263*555 
                           img = img.GetThumbnailImage(5052,ImgHeight*5052/ImgWidth, null, System.IntPtr.Zero);
                        
                         }
                          using (Bitmap bitmap = new Bitmap(img))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                bitmap.Save(Path);
                            }
                        }
                    }
              
                    else
                    {
                     
                            img.Save(Path, System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    img.Dispose();
                    string sql = string.Format("insert into ImgResource (ImgName,ImgSrc,Remark) values ('{0}','{1}','{2}')", Name, ImgSrcNew, Remark);
                    bool result = dal.ExcuteSql(sql);
                   
                    if (!result)
                    {
                        Messgae = "{ error:'', msg:'导入失败！'}";
                    }
                    else  
                    {
                        Messgae = "{ error:'', msg:'导入成功！'}";
                    }
                  
                    Response.Write(Messgae);
                    Response.End();
                    }
              
            }
            catch (Exception e)
            {
                if (e.Message == "GDI+ 中发生一般性错误。")
                {
                    Messgae = "{ error:'', msg:'导入出错！请检查导入图片，动态图中不可导入静态图'}";
                }
                else
                {
                    Messgae = "{ error:'', msg:'导入出错！" + e.Message + "'}";
                }
                Response.Write(Messgae);
                Response.End();
            }

        }


        public ActionResult LoadGIFMessage(string ID,string SysCode)
        { 
           if (!string.IsNullOrEmpty(ID))
           {
               string sql = "";
               if (SysCode == "DH")
               {
                   sql = " select  g.ID,Name,g.SensorNum,d.Type,s.Type TypeName,g.Tag,RateW,RateH ,g.Angle from  GifConfig g " +
                       " left join aqss s on s.SensorNum=g.SensorNum " +
                       " left join DeviceType d on d.TypeCode =s.Type " +
                        " where g.id=" + ID;
               }
               else
               {
                   sql = " select  g.ID,Name,g.SensorNum,EquipName Type,EquipName TypeName,RateW,RateH ,g.Angle from  GifConfig g" +
                   " left join VariableFixInfo v on v.VarName=SensorNum"+
                    " where g.id="+ID;
               }

               try
               {
                   DataTable dt = dal.ReturnData(sql);
                   string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                   return Json(new { data = json }, JsonRequestBehavior.AllowGet);
               }
               catch (Exception e)
               {
                   return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
               }

           }
           else  
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
           }
           
        
        }

        public void LoadTypeName(string SysCode,string Type)
        {
            string sql = "";
            if (!string.IsNullOrEmpty(Type))
            {
                if (SysCode == "DH")
                {
                    if (Type == "A")
                    {
                        sql = "select DISTINCT mc.Type TypeCode,TypeName,d.Unit from aqmc mc " +
                            " left join DeviceType d" +
                            " on mc.Type=d.TypeCode  where  d.type='" + Type + "' order by TypeCode";
                    }
                    else if (Type == "D")
                    {
                        sql = "select DISTINCT mc.Type TypeCode,TypeName,d.Unit from aqkc mc " +
                               " left join DeviceType d" +
                               " on mc.Type=d.TypeCode  where  d.type='D' order by TypeCode";
                    }
                    else
                    {
                        sql = "select  TypeCode ,TypeName,Unit from  DeviceType  where type ='" + Type + "' ";
                    }


                }
                else
                {
                    int OPCType = 0;
                    switch (Type)
                    {
                        case "A": OPCType = 0; break;
                        case "D": OPCType = 1; break;
                        case "L": OPCType = 2; break;
                        case "C": OPCType = 3; break;
                    }
                    string TableName = "";
                    TableName = SysCode.Substring(0, 2);
                    TableName += "_Equipment";
                    sql = "select distinct EquipName TypeCode,EquipName  TypeName,Unit from " + TableName + " where EquipType=  " + OPCType;

                }
                try
                {

                    DataTable dt = dal.ReturnData(sql);
                    int total = dt.Rows.Count;
                    string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                    json = "{\"total\": " + total + ",\"rows\":" + json + "}";
                    Response.Write(json);
                    Response.Flush();
                    Response.End();
                }
                catch (Exception e)
                {
                    Response.Write("");
                    Response.Flush();
                    Response.End();
                }

            }
          
        }

        public void LoadSensor(string SysCode, string Type)
        {
            string sql = "";
            if (!string.IsNullOrEmpty(Type))
            {
                if (SysCode == "DH")
             {
                sql = "select SensorNum, Place ,TypeName,Unit from aqss  s " +
                       " left join DeviceType d  on  s.Type =d.TypeCode  where   s.Type='"+Type+"'";

            }
            else
            {
                string TableName = "";
                TableName = SysCode.Substring(0, 2);
                sql = "select PointName SensorNum,Unit,EquipPlace Place,EquipName  TypeName from  " + TableName + "_Points tp left join " + TableName + "_Equipment te " +
                    " on tp.EquipID=te.ID where EquipName='"+Type+"'";
            }
            try
            {

                DataTable dt = dal.ReturnData(sql);
                int total = dt.Rows.Count;
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                json = "{\"total\": " + total + ",\"rows\":" + json + "}";
                Response.Write(json);
                Response.Flush();
                Response.End();
            }
            catch (Exception e)
            {
                Response.Write("");
                Response.Flush();
                Response.End();
            }
            }
          
           
        }



        public void LoadChildSysControl(string ChildSys)
        {
            // EquipType=1 开关量
            try
            {

                string sql = "select row_number() over(ORDER BY pp.PointName) TmpID,  pc.pointid ,ControlValue ,pp.PointName,pe.EquipName,pe.EquipType,pp.EquipPlace," +
                               " case pr.VarStatus when 0 then  (case pe.EquipType when 0 then cast(pr.VarStatus  as varchar(20)) when 1 then pe.KaiGuan_0_Show end ) " +

                               " when 1 then (case pe.EquipType when 0 then '故障' when 1 then pe.KaiGuan_1_Show end )  " +
                               " when 2 then(case pe.EquipType when 0 then 'OPC断线' when 1 then pe.KaiGuan_2_Show end )" +
                                " end as [Value]" +
                                " from " + ChildSys + "_Control  pc" +
                                " left join  " + ChildSys + "_Points pp" +
                                " on pp.PointID=pc.PointID" +
                                " left JOIN " + ChildSys + "_Equipment pe " +
                                " on pe.ID= pp.EquipID" +

                                " left join " + ChildSys + "_RealValues pr " +
                                " on pc.PointID= pr.PointID" +
                                " where EquipType=1";
                DataTable dt = dal.ReturnData(sql);
                int total = dt.Rows.Count;
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                json = "{\"total\": " + total + ",\"rows\":" + json + "}";
                Response.Write(json);
                Response.Flush();
                Response.End();


            }
            catch (Exception e)
            {
                log.WriteTextLog("DHController下方法LoadData错误"+e.Message, DateTime.Now);

                Response.Flush();
                Response.End();
            }

        }

    }
}
