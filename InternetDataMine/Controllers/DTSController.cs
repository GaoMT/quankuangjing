using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using InternetDataMine.Models.DataService;
using System.Data;
using Newtonsoft.Json;

namespace InternetDataMine.Controllers
{   
    public class DTSController : Controller
    {
        DataDAL dal = new DataDAL();
        //
        // GET: /DTS/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DTSRealData()
        {
            return View();
        }

        //实时曲线获取设备ID 
        public void LoadDeviceID()
        {

            string sql = "select distinct DeviceID from DTSRawData";
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

        //实时曲线获取通道ID 
        public void LoadChannel(string DeviceID)
        {
            if (string.IsNullOrEmpty(DeviceID))
            {

                Response.Write("");
            }
            else  
            {
                string sql = "select distinct ChannelID from DTSRawData   where DeviceID='" + DeviceID + "'";
                try
                {
                    DataTable dt = dal.ReturnData(sql);
                    string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                    Response.Write(json);


                }
                catch (Exception e)
                {
                    Response.Write("");
                }
                Response.Flush();
                Response.End();
            }
        }


        public ActionResult LoadRealData(string DeviceID, string ChannelID)
        {
            string sql = "select top 1  TemperatureData from DTSRawData where DeviceID='" + DeviceID + "' and ChannelID = '" + ChannelID + "' order by RecordTime desc";
            string json = "";
            int count=0;
            try
            {
                DataTable  dt= dal.ReturnData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string ss = "11.3,11.1,11.1,11.1,11.0,11.1,11.1,11.2,11.3,11.4,11.4,11.4,11.3,11.2,11.1,11.0,11.1,11.1,11.0,10.8,10.5,9.9,9.2,8.6,8.0,7.4,7.4,7.5,7.6,8.2,8.6,9.0,9.1,8.9,8.5,7.9,7.1,6.2,5.5,5.0,4.7,4.6,4.6,4.7,4.8,4.9,5.1,5.5,5.7,5.9,5.9,5.9,5.8,5.7,5.6,5.7,5.7,5.8,5.8,5.8,5.9,5.8,5.6,5.8,5.9,5.7,5.6,5.6,5.5,5.5,5.4,5.6,5.5,5.5,5.4,5.3,5.2,5.2,5.3,5.1,5.1,5.1,5.0,5.0,5.2,5.4,5.5,5.8,5.7,5.8,5.9,6.2,6.5,7.1,7.4,7.6,7.6,7.6,6.4,6.1,4.5,4.8,4.5,4.5,4.2,4.3,4.2,4.1,4.1,4.2,4.2,4.1,4.1,4.0,4.0,4.0,4.0,4.1,4.2,4.2,4.1,4.1,4.0,4.0,3.8,3.9,3.9,4.0,4.0,4.0,4.0,4.0,3.9,4.0,4.2,4.4,4.9,5.4,5.5,5.6,5.5,5.3,5.1,4.9,5.0,5.0,4.8,5.1,5.0,4.9,5.0,5.3,5.6,5.9,6.2,6.4,6.6,6.6,6.5,6.4,6.4,6.2,6.0,5.8,5.6,5.5,5.6,5.6,5.6,5.6,5.5,5.2,5.2,5.2,5.1,5.1,5.1,5.1,4.9,4.9,5.1,5.3,5.5,6.3,6.8,7.4,7.6";
                    //string[] TemperatureData = dt.Rows[0][0].ToString().Split(new char[]{','});
                    string[] TemperatureData = ss.Split(new char[] { ',' });
                    count = TemperatureData.Length;
                    if (count <= 500)
                    {
                        json = JsonConvert.SerializeObject(TemperatureData, Formatting.Indented);
                     
                    }
                    else { 
                    
                    
                    }
                
                }
                return Json(new { data = json, state = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message, state = count }, JsonRequestBehavior.AllowGet);
            
            }

        }
    }
}
