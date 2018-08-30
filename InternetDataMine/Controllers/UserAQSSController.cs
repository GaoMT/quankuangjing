using InternetDataMine.Models.DataService;
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
    public class UserAQSSController : Controller
    {
        //
        // GET: /UserAQSS/
        DataDAL dal = new DataDAL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoadSensor(string SensorList)
        {

            string where = "";

            if (!string.IsNullOrEmpty(SensorList))
            {
                where += " and ("; 
                string[] SensorNum = SensorList.Split(',');
                for (int i = 0; i < SensorNum.Count(); i++)
                {
                    if (i != 0)
                    {
                        where += " or ";
                    }

                    where += "  SensorNum='" + SensorNum[i] + "'";
                }
                where += ")";
            }
         
            string sql = @"select SensorNum,Place, 

                        case  StateCode when 1 then '网络中断' when 2 then '传输异常' when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时' 
                        when 0 then ShowValue  end ShowValue,                        case StateCode when 1 then '网络中断' when 2 then '传输异常' when 3 then '通讯中断' when 4 then '网络故障' when 5 then '数据延时' 
                        when 0 then  [dbo].DecToBin(ValueState) end ValueState

                        from aqss s 
                        left join MineConfig mc on mc.minecode =s.minecode 
                        left join SystemConfig sc on sc.minecode =mc.id  where 1=1 and  typecode=1 " + where;

                DataTable dt = new DataTable();
                dt = dal.ReturnData(sql);
        
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
                return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }



        public void LoadComboAQSS()
        {
            string sql = @"select SensorNum,Place,TypeName from aqss s left join  DeviceType d
                            on s.type=d.typecode ";
            DataTable dt = new DataTable();
            dt = dal.ReturnData(sql);
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
            json = "{\"total\": " + dt.Rows.Count + ",\"rows\":" + json + "}";
            Response.Write(json);
            Response.End();
        }

    }
}
