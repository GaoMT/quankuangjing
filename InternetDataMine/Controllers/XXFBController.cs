﻿using InternetDataMine.Models.DataService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetDataMine.Controllers
{
    public class XXFBController : Controller
    {
        //
        // GET: /XXFB/
        DataDAL dal = new DataDAL();
        DataBLL bll = new DataBLL();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DMLED()
        {
            return View();
        }
        public ActionResult XZQY()
        {
            return View();
        }
        public ActionResult ZFJJK()
        {
            return View();
        }
        public ActionResult SaveLedInfo(string LedIP, string LedAddress, string LedShowConfig, string SGIP, string SGText,string IsSQL ,string SGPort ,string SGSQL,string IsAdd)
        {
          string    LedShowConfigF = LedShowConfig.Replace("'", "''");
          string SGCondition = SGSQL.Replace("'", "''");
          string sql = "";
          string json = "保存成功!";
          string state = "0";
          if (IsAdd == "1")
          {
              if (SGIP != "")
              {
                  sql = "select * from LedShowTable where LedIP='" + LedIP + "' or  SGIP='" + LedIP + "' OR LedIP='" + SGIP + "' OR SGIP='" + SGIP + "'";

              }
              else
              {
                  sql = "select * from LedShowTable where LedIP='" + LedIP + "' or  SGIP='" + LedIP + "'";

              }
          DataTable dts = dal.ReturnData(sql);
           if (dts != null && dts.Rows.Count > 0)
           {
               json = "IP已存在，请重新输入。声光报警IP与LED屏ID均不能重复";
               state = "1";
               return Json(new { data = json,state=state }, JsonRequestBehavior.AllowGet);
           }
           else
           {
               sql = " insert into LedShowTable(LedIP,LedAddresss,LedShowConfig,SGIP,SGText,SGPort,SGCondition,IsSQL) values ('" + LedIP + "','" + LedAddress + "','" + LedShowConfigF + "','" + SGIP + "','" + SGText + "','" + SGPort + "' , '" + SGCondition + "','" + IsSQL + "')  ";
         
           }
         }
          else
          {
              if (SGIP != "")
              {
                  sql = "select * from LedShowTable where LedIP !='" + LedIP + "' and SGIP='" + SGIP + "'";

                  DataTable dts = dal.ReturnData(sql);
                  if (dts != null && dts.Rows.Count > 0)
                  {
                      state = "1";
                      json = "IP已存在，请重新输入。声光报警IP与LED屏ID均不能重复";
                      return Json(new { data = json, state = state }, JsonRequestBehavior.AllowGet);
                  }



              }
              sql = string.Format(" update LedShowTable set LedAddresss='{0}',SGIP='{1}',SGText='{2}',LedShowConfig='{3}',SGPort='{4}',IsSQL='{5}' , SGCondition='{6}' WHERE LEDIP='{7}' ", LedAddress, SGIP, SGText, LedShowConfig, SGPort, IsSQL, SGCondition, LedIP);
             
          }
            try
            {
                bool success = dal.ExcuteSql(sql);
               
                if (!success)
                {
                    json = "保存失败!请检查数据合法性";
                    state = "1";
                }
                return Json(new { data = json, state = state }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                state = "1";
                return Json(new { data = "保存失败:" + e.Message, state = state }, JsonRequestBehavior.AllowGet);
            }
       
        }
        public void LoadLedTable(string page ,string rows)
        {
            string ReturnString = "";
            int Page = 0;
            int StartRow = 0;
            int row = 0;
            try {
            if (!string.IsNullOrEmpty(rows) && rows!="NaN")
            {
                row = int.Parse(rows);
            }
            else {
                row = 20;
            }
            if (!string.IsNullOrEmpty(page))
            {
                Page = Convert.ToInt32(page);
                if (Page > 0)
                {
                    StartRow = row * (Page - 1) + 1;
                }
            }
            bll.PageSize = row;
            bll.PageIndex = StartRow / row;
            DataTableCollection dts = null;
            dts = bll.GetLedTable();
            if (dts != null)
            {
                int TotalRows = int.Parse(dts[0].Rows[0][0].ToString());
               
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string json = JsonConvert.SerializeObject(dts[1], Formatting.Indented, timeConverter);
                ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";

                Response.Write(ReturnString);
                Response.Flush();
                Response.End();
            }
            else
            {

                Response.Write("");
                Response.Flush();
                Response.End();
            }
            }
            catch(Exception e)
            {
               Response.Write("");
               Response.Flush();
               Response.End();
            }

        }


        public ActionResult DeleteLedInfo(string Condition)
        {
            try
            {

                string[] LedIP = Condition.Split(new char[] { ',' });
                if (LedIP.Length > 0)
                {
                    string list = "";
                    for (int i = 0; i < LedIP.Length; i++)
                    {

                        if (i > 0)
                        {
                            list += " or ";
                        }
                        list += " LedIP = '" + LedIP[i] + "'";
                    }
                    string sql = "delete from LedShowTable where " + list;
                    bool success = dal.ExcuteSql(sql);
                    string data = "删除成功!";
                    if (!success)
                    {
                        data = "删除失败";
                    }
                    return Json(new { data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "删除失败：获取LEDIP失败" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { data = "删除失败:"+e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}