using InternetDataMine.Models.DataService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;

namespace InternetDataMine.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PointContentEditor()
        {
            return View();
        }
        public ActionResult MapConfig()
        {
            return View();
        }

        public ActionResult MapData()
        {
            return View();
        }

        public ActionResult MineInfo()
        {
            return View();
        }

        #region  [ 保存多个标注点 ]

        DataDAL dal = new DataDAL();
        /// <summary>
        /// 保存标注点
        /// </summary>
        public void SaveMarkers()
        {
            if (Request.Form.Count < 1) return;
            string strObj = Request.Form[0];
            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ExtendMarker>));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strObj));
            List<ExtendMarker> models = (List<ExtendMarker>)s.ReadObject(ms);
            ms.Dispose();
            string strSQL = "";
            for (int i = 0; i < models.Count; i++)
            {
                ExtendMarker model = models[i];
                strSQL += string.Format(" Update [MineConfig] Set x={0},y={1} Where Id='{2}'", model.x, model.y, model.markerID);
            }
            if (strSQL == "") return;

            if (dal.ExcuteSql(strSQL))
            {
                Response.Write("{\"Result\":\"Successed\",\"Msg\":\"保存成功\"}");
            }
            else
            {
                Response.Write("{\"Result\":\"Failer\",\"Msg\":\"保存失败\"}");
            }

            Response.End();
        }

        #endregion

        #region [ 修改标注点 ]

        /// <summary>
        /// 修改标注点
        /// </summary>
        /// <param name="mineID"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SaveMarker(string mineID,string x,string y)
        {
            string strSQL = string.Format(" Update [MineConfig] Set x={0},y={1} Where Id='{2}'", x, y, mineID);

            if (dal.ExcuteSql(strSQL))
            {
                Response.Write("{\"Result\":\"Successed\",\"Msg\":\"保存成功\"}");
            }
            else
            {
                Response.Write("{\"Result\":\"Failer\",\"Msg\":\"保存失败\"}");
            }

            Response.End();
        }

        #endregion

        #region [ 获取煤矿信息 ]

        /// <summary>
        /// 获取煤矿信息，在地图上显示
        /// </summary>
        public void GetMineInfo(string mineID)
        {
            DataTable Result = dal.GetMap_MineInfo(mineID);
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            if (Result != null)
            {
               var  TotalRows = Result.Rows.Count;
               string  json = JsonConvert.SerializeObject(Result, Formatting.Indented, timeConverter);
                Response.Write("{\"total\": " + TotalRows + ",\"rows\":" + json + "}");
            }
            Response.End();
        }
        #endregion

        #region [ 删除标注点 ]

        /// <summary>
        /// 删除标注点
        /// </summary>
        /// <param name="mineID"></param>
        public void DeleteMarker(string mineID)
        {
            string strSQL = string.Format(" Update [MineConfig] Set x=null,y=null Where Id='{0}'",  mineID);

            if (dal.ExcuteSql(strSQL))
            {
                Response.Write("{\"Result\":\"Successed\",\"Msg\":\"删除成功\"}");
            }
            else
            {
                Response.Write("{\"Result\":\"Failer\",\"Msg\":\"删除失败\"}");
            }

            Response.End();
        }

        #endregion
    }

    [DataContract]
    public class ExtendMarker
    {
        [DataMember]
        public string markerID { get; set; }

        //public string markerName { get; set; }

        //public object marker { get; set; }
        [DataMember]
        public string markerName { get; set; }
        [DataMember]
        public float x { get; set; }
        [DataMember]
        public float y { get; set; }


    }
}
