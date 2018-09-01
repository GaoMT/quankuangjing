using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using InternetDataMine.Models;
using InternetDataMine.Models.DataService;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
namespace InternetDataMine.Controllers
{
    public class ChartController : Controller
    {
        //
        // GET: /Charts/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MCDay(string PageName, string UserAbility, string MineCode, string Height)
        {
            LoadModel loadModel = new LoadModel();
            loadModel.Height = Height;

            loadModel.UserAbility = UserAbility;
            loadModel.UserMineCode = MineCode;

            return View();
        }

        /// <summary>
        /// 加载开关量查询数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SwitchQantity(string MineCode)
        {
            LoadModel loadModel = new LoadModel();
            loadModel.UserMineCode = MineCode;
            return View(loadModel);
        }


        /// <summary>
        /// 加载模拟量查询数据
        /// </summary>
        /// <returns>返回视图</returns>
        public ActionResult AnalogQantity(string MineCode)
        {
            LoadModel loadModel = new LoadModel();
            loadModel.UserMineCode = MineCode;
            return View(loadModel);
        }


        public ActionResult RtCurve(string MineCode, string IsFromAQSS)
        {
            LoadModel loadModel = new LoadModel();
            loadModel.UserMineCode = MineCode;
            if (!string.IsNullOrEmpty(IsFromAQSS))
            {
                loadModel.PreLoadData = "y";
            }
            else loadModel.PreLoadData = "n";
            return View(loadModel);
        
        }
       
        /// <summary>
        /// 返回模拟量数据
        /// </summary>
        /// <param name="mineCode"></param>
        /// <param name="sensorCodes"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        public void ReturnCurverDatas(string mineCode, string sensorCodes, string BeginTime, string EndTime)
        {
            //****
          //EndTime = Convert.ToDateTime(BeginTime).AddDays(1).AddSeconds(-1).ToString();
          DataBLL bll = new DataBLL();
          DataTableCollection dts = bll.ReturnCurverDatas(mineCode, sensorCodes, BeginTime, EndTime);
            string json="[]";
            //在对DATATABLE进行序列化的时候，规范日期格式
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                if(dts!=null&&dts.Count>0)
                {
                    json = JsonConvert.SerializeObject(dts, Formatting.Indented, timeConverter);
                }
                Response.Write(json);

                Response.End();

        }

        public void ReturnSwitchDatas(string mineCode,string sensorCodes,string BeginTime,string EndTime)
        {
            DataBLL bll = new DataBLL();
            DataTableCollection dts = bll.ReturnSwitchDatas(mineCode, sensorCodes, BeginTime, EndTime);
            string json = "[]";
            //在对DATATABLE进行序列化的时候，规范日期格式
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            if (dts != null && dts.Count > 0)
            {

                json = JsonConvert.SerializeObject(dts, Formatting.Indented, timeConverter);
            }
            Response.Write(json);

            Response.End();
        }
        public void ReturnRtCurverDatas(string mineCode,string time, string sensorCodes,string timeRound)
        {
         
            try
            {
                DataBLL bll = new DataBLL();
                DateTime dt = Convert.ToDateTime(time);
                DataTableCollection dts = bll.RtCurver(mineCode, sensorCodes, dt, timeRound);
                string json = "[]";
                //在对DATATABLE进行序列化的时候，规范日期格式
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                if (dts != null && dts.Count > 0)
                {

                    json = JsonConvert.SerializeObject(dts, Formatting.Indented, timeConverter);
                }
              Response.Write(json) ;


            }
            catch (Exception e)
            { 
            
            }  
         
                
                Response.End();
        }


        public void ReturnSwitchState(string mineCode, string sensorCodes, string BeginTime, string EndTime)
        {
            //****
            //EndTime = Convert.ToDateTime(BeginTime).AddDays(1).AddSeconds(-1).ToString();
            DataBLL bll = new DataBLL();
            DataTableCollection dts = bll.ReturnSwitchData(mineCode, sensorCodes, BeginTime, EndTime);
            string json = "[]";
            //在对DATATABLE进行序列化的时候，规范日期格式
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            if (dts != null && dts.Count > 0)
            {
                json = JsonConvert.SerializeObject(dts, Formatting.Indented, timeConverter);
            }
            Response.Write(json);

            Response.End();

        }

        public void AnalogQantityQuery(string MineCode,string SensorType,string SensorNum)
        {
            
        }
        /// <summary>
        /// 通过sensorNum查找place
        /// </summary>
        /// <param name="mineCode"></param>
        /// <param name="senserNum"></param>
        public void FindPlace(string mineCode, string sensorNum)
        {
            string sql = string.Format("select Place from AQMC where MineCode={0} and sensorNum='{1}'", mineCode, sensorNum);
            var dal = new DataDAL();
            var dt = dal.ReturnDs(sql);
            var data = JsonConvert.SerializeObject(dt);
            Response.Write(data);


            Response.End();
        }

        public void FindPlaceD(string mineCode, string sensorNum)
        {
            string sql = string.Format("select Place from AQKC where MineCode={0} and sensorNum='{1}'", mineCode, sensorNum);
            var dal = new DataDAL();
            var dt = dal.ReturnDs(sql);
            var data = JsonConvert.SerializeObject(dt);
            Response.Write(data);


            Response.End();
        }

        /// <summary>
        /// 获取煤矿下拉列表信息
        /// </summary>
        public void GetMineInfoComboTree()
        {
            BaseInfoModel bim = new BaseInfoModel();
            bim.ReturnComboTreeMineInfo("");
        }

        /// <summary>
        /// 开关量状态曲线
        /// </summary>
        /// <returns></returns>
        public ActionResult SwitchState(string MineCode )
        {
            LoadModel loadModel = new LoadModel();
            loadModel.UserMineCode = MineCode;
            return View(loadModel);
            //return View();
        }
    }
}
