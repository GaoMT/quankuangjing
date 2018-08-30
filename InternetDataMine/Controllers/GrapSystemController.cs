using InternetDataMine.Models;
using InternetDataMine.Models.DataService;
using InternetDataMine.Models.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

using System.Windows.Forms;
using System.Xml;

namespace InternetDataMine.Controllers
{
    public class GrapSystemController : Controller
    {
        System.Threading.Thread HistoryThread;
        // GET: /GrapSystem/
        public List<Station> StationsList = new List<Station>();//基站组
        DataBLL bll = new DataBLL();
        Graphics_Dpic G_D = new Graphics_Dpic();
        private List<EmpMoverModel> EmpMoverList = new List<EmpMoverModel>();
        private string MoverZFilePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Content\\image\\Zg.GIF";
        private string MoverFFilePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Content\\image\\Fg.GIF";
        string strOutMessage;
        private bool isOutTrue = false;
        List<string> NoRoutePeoples = new List<string>();
      static  public string con = "";
        public ActionResult Index()
        {
            return View();
        }

        #region  128数据库配置
        public ActionResult SetDT()
        {
            return View();
        }

        public string Init128()
        {
            try
            {
                string IP = ConfigurationManager.AppSettings["128IP"].ToString();
                string uid = ConfigurationManager.AppSettings["128UID"].ToString();
                string pwd = ConfigurationManager.AppSettings["128PWD"].ToString();
                string db = ConfigurationManager.AppSettings["128DB"].ToString();
                List<string> info = new List<string>();
                info.Add(IP);
                info.Add(uid);
                info.Add(pwd);
                info.Add(db);
                return JsonConvert.SerializeObject(info); ;
            }
            catch (Exception e)
            {
                return null;
            }

        
        }

        public ActionResult SaveDB(string IP, string UID, string PWD, string DB,string IsSave)
        {
            #region 数据不为空
            if (string.IsNullOrEmpty(IP))
            { 
                return Json(new { data = "数据库IP不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(UID))
            {
                return Json(new { data = "数据库用户名不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(PWD))
            {
                return Json(new { data = "数据库密码不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(DB))
            {
                return Json(new { data = "数据库名不能为空" }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            if (IsSave == "1")
            {
                #region 保存
                try
                {
                    Configuration objConfig = WebConfigurationManager.OpenWebConfiguration("~");
                    AppSettingsSection objAppSettings = (AppSettingsSection)objConfig.GetSection("appSettings");
                    if (objAppSettings != null)
                    {
                        objAppSettings.Settings["128IP"].Value = IP;
                        objAppSettings.Settings["128UID"].Value = UID;
                        objAppSettings.Settings["128PWD"].Value = PWD;
                        objAppSettings.Settings["128DB"].Value = DB;
                        objConfig.Save();
                        return Json(new { data = "保存成功" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = "获取配置信息为空" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception e)
                {
                    return Json(new { data = "获取配置信息出错：" + e.Message }, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            else
            {
                #region 测试
                //server=172.16.19.87;uid=sa;password=sa;database=ShineView_Data
                string sql=string.Format("server={0};uid={1};password={2};database={3}",IP,UID,PWD,DB);
                SqlConnection connecton = new SqlConnection(sql);

                try
                {
                    connecton.Open();
                    return Json(new { data = "数据库连接测试成功",State=1 }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception err)
                {
                    return Json(new { data = "数据库连接失败" ,State=0}, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    connecton.Close();
                }
                #endregion 
            }
        }


      

        #endregion

        #region 实时分布
        public ActionResult RealDistribute(string MineCode)
        {
            LoadModel loadModel = new LoadModel();
            loadModel.UserMineCode = MineCode;
            string sql = "select filename, FileID from G_DPicFile";
            con = Get128Con();
            ViewBag.Url = "";
            DataTable dt = null;
            var dal = new DataDAL();
            try
            {
                string json = "";
                dt = dal.ReturnData(sql, con);
               
                if (dt.Rows.Count > 0)
                {
                    string fo = dt.Rows[0]["filename"].ToString();
                    fo = fo.Substring(fo.LastIndexOf(".") , fo.Length - fo.LastIndexOf("."));
                    loadModel.FileID = dt.Rows[0]["FileID"].ToString();
                    LoadGroundGraph(loadModel.FileID, fo);
                    ViewBag.Url = "temp" + fo;
                }
         
            }
            catch (Exception e)
            { }
            return View(loadModel);
          
        }

        public void LoadGroundGraph(string FileID,string fo)
        {
         
            
            string sql = "";
            string fileID = "";
            DataTable dt = null;
            var dal = new DataDAL();
          
            if (!string.IsNullOrEmpty(FileID))
            {
                sql = string.Format("select FileImg,FileID from G_DPicFile where fILEid='{0}'", FileID);

            }
            else
            {
                sql = string.Format("select top(1) FileImg,FileID from G_DPicFile");
            }


            try
            {
                dt = dal.ReturnData(sql, con);
                byte[] xmlbytes = null;
                bool IsExit = false;
                if (dt.Rows.Count > 0)
                {
                    fileID = dt.Rows[0]["FileID"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FileImg"].ToString()))
                    {
                        xmlbytes = (byte[])dt.Rows[0]["FileImg"];//原图
                        Image img = byteArrayToImage(xmlbytes);//将原图转为image类型
                        DirectoryInfo di = new DirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\128Graph");
                        FileInfo[] fis = di.GetFiles();
                        //根据原图大小转为小图
                        if (img.Width > 6000)
                        {
                            img = img.GetThumbnailImage(6000, img.Height * 6000 / img.Width, null, System.IntPtr.Zero);

                        }
                        if (fis.Length > 0)
                        {
                            byte[] before = SetImageToByteArray(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\128Graph\\"+fis[0].Name);
                            byte[] after = ImageArrayTobyte(img);

                            if (!IsSame(before, after))
                            {
                               System.IO.File.Delete(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\128Graph\\" +fis[0].Name);

                               using (Bitmap bitmap = new Bitmap(img))
                               {
                                   using (MemoryStream stream = new MemoryStream())
                                   {
                                       bitmap.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\128Graph\\temp"  + fo);

                                   }
                               }
                            }
                        }
                        else
                        {
                            using (Bitmap bitmap = new Bitmap(img))
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    bitmap.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\128Graph\\temp" + fo);

                                }
                            }
                        
                        }
                        //for (int i = 0; i < fis.Length; i++)
                        //{
                        //    //如果已存在这个图
                        //    if (fis[i].Name == "temp.wmf")
                        //    {
                        //        IsExit = true;
                        //        byte[] before = SetImageToByteArray(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\image\\temp"+fo);
                        //        byte[] after = ImageArrayTobyte(img);

                        //        if (!IsSame(before, after))
                        //        {
                        //            img.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\image\\temp"+fo);
                        //        }
                        //    }
                         
                        //}
                        //if (!IsExit)
                        //{
                        //    img.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\image\\temp"+fo);
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Response.Write("<script>alert('出错：" + e.Message+ "')</script>");
            }
        }

        public bool IsSame(byte[] before, byte[] after)
        {
            bool tag = true;
            if (before.Length == after.Length)
            {
                for (int i = 0; i < before.Length; i++)
                {
                    if (before[i] != after[i])
                    {
                        tag = false;
                        break;
                    }
                }
            }
            else
            {
                tag = false;
            }
              
                return tag;
           
                              
        
        }
        public byte[] SetImageToByteArray(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            int streamLength = (int)fs.Length;
            byte[] image = new byte[streamLength];
            fs.Read(image, 0, streamLength);
            fs.Close();
            return image;
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public ActionResult LoadRate(string FileID)
        {

            string sql = "";
            string fileID = "";
            if (!string.IsNullOrEmpty(FileID))
            {
                sql = string.Format("select FileImg,FileID from G_DPicFile where fILEid='{0}'", FileID);

            }
            else
            {
                sql = string.Format("select top(1) FileImg,FileID from G_DPicFile");
            }

            DataTable dt = null;
            var dal = new DataDAL();
            try
            {
                dt = dal.ReturnData(sql, con);
                byte[] xmlbytes = null;
                if (dt.Rows.Count > 0)
                {
                    fileID = dt.Rows[0]["FileID"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FileImg"].ToString()))
                    {
                        xmlbytes = (byte[])dt.Rows[0]["FileImg"];
                        // 可行
                        Image img = byteArrayToImage(xmlbytes);
                        return Json(new { width = img.Width, height = img.Height,State=1 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { State = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { State = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { State = 2}, JsonRequestBehavior.AllowGet);
            }
        }

        public Image byteArrayToImage(byte[] Byte)
        {
            using (MemoryStream ms = new MemoryStream(Byte))
            { 
            Image outImg=Image.FromStream(ms);
            return outImg;
            }
        }
        public byte[] ImageArrayTobyte(Image img)
        {
            byte[] bt = null;
            string format;
            System.Drawing.Imaging.ImageFormat _img_format = GetImageFormat(img, out format);

            //if (_img_format.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            //{
                using (MemoryStream mostream = new MemoryStream())
                {
                    Bitmap bmp = new Bitmap(img);
                    bmp.Save(mostream, System.Drawing.Imaging.ImageFormat.Bmp);//将图像以指定的格式存入缓存内存流
                    bt = new byte[mostream.Length];
                    mostream.Position = 0;//设置留的初始位置
                    mostream.Read(bt, 0, Convert.ToInt32(bt.Length));
                }
            //}
            //else
            //{
            //    ImageConverter imgconv = new ImageConverter();

            //    bt = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
            //}
           
            return bt;  
        }

        private System.Drawing.Imaging.ImageFormat GetImageFormat(Image _img, out string format)
        {
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                format = ".jpg";
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
            {
                format = ".gif";
                return System.Drawing.Imaging.ImageFormat.Gif;
            }
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
            {
                format = ".png";
                return System.Drawing.Imaging.ImageFormat.Png;
            }
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            {
                format = ".bmp";
                return System.Drawing.Imaging.ImageFormat.Bmp;
            }
            format = string.Empty;
            return null;
        }
        public void LoadOverGraph(string FileID, string FileName)
        {
            string Sql = "";
          
            string sql = "select ConfigFile from G_DConfigFile ";
            string result = "";
            DataTable dt = null;
            var dal = new DataDAL();
            XmlDocument ConfigXml = null;
            byte[] xmlbytes = null;
            XmlNodeList xnl = null;
            string[] OverList = null;
            try
            {

                if (!string.IsNullOrEmpty(FileID))
                {
                    sql += " Where MapFileID='" + FileID + "'";

                    dt = dal.ReturnData(sql, con);
                 
                    if (dt.Rows.Count > 0)
                    {
                        xmlbytes = (byte[])dt.Rows[0]["ConfigFile"];
                        ConfigXml = G_D.BytesToXml(xmlbytes);
                        xnl = ConfigXml.SelectSingleNode("//Stations").ChildNodes;
                    }
                    if (!string.IsNullOrEmpty(FileName))
                    {
                        OverList = FileName.Split(new char[] { ',' });
                    }

                    result = ReadStation(xnl, OverList);
                }
            }
            catch (Exception e)
            { 
            
            }
            Response.Write(result);
            Response.Flush();
            Response.End();
        }
        public string  ReadStation(XmlNodeList xnl, string[] OverList)

        {
            
            List<string> NameList = new List<string>();
            DataTable dt = null;
            var dal = new DataDAL();
            List<Station> StationList = new List<Station>();
            //这里分钟状态和128里面的不一样 大概是要改的 128里分站状态用的是传输分站 这里是读卡分站
            //读的视图应该是 A_Graphics_StationHeadState



            string ReadStation = "select s.StationHeadID,StationHeadPlace,StationHeadX,StationHeadY," +
                                  "  case(StationHeadState) when 0 then  '未初始化' when 2000 then '正常' when -1000 then  '故障' when -2000 then '休眠'  end State," +
                                  "  isnull(M.Counts,0) as NowCount from Station_Head_Info   s" +
                                  "  left join RT_InStationHeadInfo  r " +
                                  "  on s.StationHeadID=r.StationHeadID";


            ReadStation += " left join (select A.StationHeadID,count(A.CodeSenderAddress) as Counts from dbo.RT_InStationHeadInfo A,RT_InOutMine B where A.CodeSenderAddress=B.CodeSenderAddress group by A.StationHeadID) as M on M.StationHeadID=r.StationHeadID";
            try
            {
                for (int i = 0; i < xnl.Count; i++)
                {
                    string over = xnl[i].InnerText;
                    if (!string.IsNullOrEmpty(over))
                    {
                        string[] ol = over.Split(new char[] { '|' });
                        {
                            for (int j = 0; j < ol.Count(); j++)

                            {
                                for (int k = 0; k< OverList.Count(); k++)
                                {
                                    if (ol[j] == OverList[k])
                                    {
                                        NameList.Add(xnl[i].Attributes[0].InnerText);
                                    }
                                }
                            }
                        }
                    }
                  
                }
                if (NameList.Count() > 0)
                {
                    for (int i = 0; i < NameList.Count(); i++)
                    {
                        if (i == 0)
                        {
                            ReadStation += "  where  StationHeadPlace ='" + NameList[i].ToString() + "'";
                        }
                        else
                        {
                            ReadStation += " or  StationHeadPlace='" + NameList[i].ToString() + "'";
                        }
                    }
                }
                else
                {

                    ReadStation += " where 1=2";
                }
              
               
                ReadStation += " GROUP BY s.StationHeadID,StationHeadPlace,StationHeadX,StationHeadY,StationHeadState,M.Counts";
                dt = dal.ReturnData(ReadStation, con);
                if (dt.Rows.Count > 0)
                {
                    for (int i=0;i<dt.Rows.Count;i++)
                    {
                        string ID = dt.Rows[i]["StationHeadID"].ToString();
                        float X = float.Parse(dt.Rows[i]["StationHeadX"].ToString());
                        float Y = float.Parse(dt.Rows[i]["StationHeadY"].ToString());
                        string State = dt.Rows[i]["State"].ToString();
                        string Name = dt.Rows[i]["StationHeadPlace"].ToString();
                        string NowCount= dt.Rows[i]["NowCount"].ToString();
                        Station s = new Station(X, Y, Name, ID, State, NowCount);
                        StationList.Add(s);
                     }
                   }
            }
            catch (Exception e)
            { 
            
            }

            return JsonConvert.SerializeObject(StationList);
        }

        public void LoadComboGraph(string IP)
        {

            string sql = "select FileID,FileName from G_DPicFile";
           
            DataTable dt = null;
            var dal = new DataDAL();
            try
            {
                string json = "";
                dt = dal.ReturnData(sql, con);
                string ReturnString = "";
                if (dt.Rows.Count > 0)
                {
                    string TotalRows = dt.Rows.Count.ToString(); ;
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    json = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, timeConverter);
                    ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";

                }
                else
                {
                    ReturnString = "{\"total\":0,\"rows\":[]}";
                }

                    Response.Write(ReturnString);
                    Response.Flush();
                    Response.End();
                
            }
            catch (Exception e)
            {
            
            }


        }


        public void LoadComboOver(string FileID)
        {
            log log = new Controllers.log();
            string sql = "select  ConfigFile from G_DConfigFile ";
            if (!string.IsNullOrEmpty(FileID))
            {
                sql += " where MapfileID='" + FileID + "'";
            }
           
            DataTable dt = null;
            var dal = new DataDAL();
            try
            {
                string json = "";
                dt = dal.ReturnData(sql, con);
                log.WriteTextLog("行数："+dt.Rows.Count, DateTime.Now);
                if (dt.Rows.Count > 0)
                {
                    byte[] xmlbytes = (byte[])dt.Rows[0]["ConfigFile"];
                    XmlDocument ConfigXml = G_D.BytesToXml(xmlbytes);
                    XmlNode OverGraph = ConfigXml.SelectSingleNode("//Divs");
                    if (OverGraph.ChildNodes.Count > 0)
                    {
                        json += "{\"total\":" + OverGraph.ChildNodes.Count + ",\"rows\":[";
                        for (int i = 0; i < OverGraph.ChildNodes.Count; i++)
                        {
                            json += "{\"ID\":\""+i+"\",\"FileName\":\"" + ConfigXml.SelectSingleNode("//Divs").ChildNodes[i].InnerText + "\"}";
                            if (i != OverGraph.ChildNodes.Count - 1)
                            {
                                json += ",";
                            }
                        }
                        json += "]}";
                   
                    }
                }
                if (!string.IsNullOrEmpty(json))
                {
                    Response.Write(json);
                    Response.Flush();
                    Response.End();
                }
                else { 
                json="{\"total\":0,\"rows\":[]}";
                Response.Write(json);
                Response.Flush();
                Response.End();
                }
            
            }
            catch (Exception e)
            {
                log.WriteTextLog("Exception:" +e.Message, DateTime.Now);
            }
        }



        public string  ShowPeople(string StationHeadID)
        {
           
               DataTable dt=new DataTable();
            var dal=new DataDAL();
            try{
                string sql = string.Format("select  Row_Number() over (order by EmpNo asc) as  TmpID ,  A.StationHeadID,C.DeptName,c.EmpName,C.EmpNo,A.InAntennaPlace,A.InStationHeadTime " +
                      "  from dbo.RT_InStationHeadInfo A,RT_InOutMine B,Emp_Info C " +
                      "  where A.CodeSenderAddress=B.CodeSenderAddress  and C.EmpID=A.UserID and A.StationHeadID={0} ", StationHeadID);

                dt = dal.ReturnData(sql, con);
                string json = "";

                if (dt != null)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    json = "{\"total\":" + dt.Rows.Count + ",\"rows\":" + JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, timeConverter) + "}";

                }
                else
                {
                    json = "{}";
                }
                return json;
            }
              
           
            catch (Exception e)
            {
                return "{}";
            }
        }

        #endregion


        #region 轨迹回放

        public ActionResult HisTraceShow(string MineCode)
        {
            LoadModel loadModel = new LoadModel();
            loadModel.UserMineCode = MineCode;
            string sql = "select filename, FileID from G_DPicFile";
            con = Get128Con();
            ViewBag.Url = "";
            DataTable dt = null;
            var dal = new DataDAL();
            try
            {
                string json = "";
                dt = dal.ReturnData(sql, con);

                if (dt.Rows.Count > 0)
                {
                    loadModel.FileID = dt.Rows[0]["FileID"].ToString();
                    string fo = dt.Rows[0]["filename"].ToString();
                    fo = fo.Substring(fo.LastIndexOf("."), fo.Length - fo.LastIndexOf("."));
                    ViewBag.Url = "temp" + fo;

                }
            }
            catch (Exception e)  { }
            return View(loadModel);
        }

        public string LoadPerson()
        {
            string sql = "select EmpID,EmpNo,EmpName,DeptName from Emp_Info";
            DataTable dt = null;
            var dal = new DataDAL();
            string json = "{\"total\":";
            try
            {
                dt = dal.ReturnData(sql,con);
                if (dt.Rows.Count > 0)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

                    json += dt.Rows.Count + ",\"rows\":" + JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, timeConverter)+"}";
                }
                else {
                    json = "{\"total\":0,\"rows\":[]}";
                }
            }
            catch (Exception e)  {    }
            return json;
        }
        public void LoadALLOver()
        {
            string sql = "select c.ConfigFile,p.Fileimg from G_DConfigFile c  left join  G_DPicFile p on c.MapFileID= p.FileID";
            string result = "";
            DataTable dt = new DataTable();
            var dal = new DataDAL();
            XmlDocument ConfigXml = null;
            byte[] xmlbytes = null;
            XmlNodeList xnl = null;
            string[] OverList = null;
            XmlNodeList filename = null;
            string list = "";
            try
            {
                    dt = dal.ReturnData(sql, con);
                    if (dt.Rows.Count > 0)
                    {
                        xmlbytes = (byte[])dt.Rows[0]["ConfigFile"];
                        ConfigXml = G_D.BytesToXml(xmlbytes);
                        xnl = ConfigXml.SelectSingleNode("//Stations").ChildNodes;
                        filename = ConfigXml.SelectSingleNode("//Divs").ChildNodes;
                      
                        for (int i = 0; i < filename.Count; i++)
                        {
                            if (i != filename.Count - 1)
                            {
                                list += filename[i].InnerText.ToString() + ",";
                            }
                            else {
                                list += filename[i].InnerText.ToString();
                            }
                          
                        }
                    
                    }
                    if (!string.IsNullOrEmpty(list))
                    {
                        OverList = list.Split(new char[] { ',' });
                    }

                    result = ReadStation(xnl, OverList);
                }
           
            catch (Exception e)
            {

            }
            Response.Write(result);
            Response.Flush();
            Response.End();
        }


        public void LoadTrace(string Name,string Speed,string StartTime,string EndTime,string FileID)
        {
           
            string[] nameList =null ;
            List<EmpMoverModel> EMModel = new List<EmpMoverModel>();
              string json ="";
            if (!string.IsNullOrEmpty(Name))
            {
                nameList = Name.Split(new char[] { ',' });
            }
            try
            {
                EndTime = Convert.ToDateTime(EndTime).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
                for (int i = 0; i < nameList.Count(); i++)
                {
                    //dt_Person = bll.GetStations(nameList[i], StartTime, EndTime, con);
              
                    List<string> list = bll.GetRouteInfoByEmpID(nameList[i], StartTime, EndTime, con, FileID);

                    if (list.Count > 4)
                    {
                        EmpMoverModel em = new EmpMoverModel();
                        em.Route = list[0].Split('|').ToList();
                        em.Time = list[1].Split('|').ToList();
                        em.Place = list[2].Split('|').ToList();
                        em.Station = list[3].Split('|').ToList();
                        em.Emp = list[4].ToString();
                        EMModel.Add(em);
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                json = JsonConvert.SerializeObject(EMModel, Newtonsoft.Json.Formatting.Indented, timeConverter);
            }
            catch (Exception e) {  }
            Response.Write(json);
            Response.Flush();
            Response.End();
          
        }
    
        
        #endregion


        public string Get128Con()
        {

            try
            {
                string IP = ConfigurationManager.AppSettings["128IP"].ToString();
                string UID = ConfigurationManager.AppSettings["128UID"].ToString();
                string PWD = ConfigurationManager.AppSettings["128PWD"].ToString();
                string DB = ConfigurationManager.AppSettings["128DB"].ToString();
                con = string.Format("server={0};uid={1};password={2};database={3}", IP, UID, PWD, DB);
            }
            catch (Exception e)
            {
                con="server=.;uid=sa;password=sa;database=KJ128N";

            }
            return con;
        }



    }
}
