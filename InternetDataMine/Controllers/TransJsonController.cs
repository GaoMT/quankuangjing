using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InternetDataMine.Models;
using System.Windows.Forms;

namespace InternetDataMine.Controllers
{
    public class TransJsonController : Controller
    {
        //
        // GET: /TransJsonToTreeList/

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult TransJsonToTreeList(string DataType)
        //{
        //    TransJsonToTreeListModel model = new TransJsonToTreeListModel(DataType);
        //    return View(model);
        //}


        public void TransJsonToTreeList(string SystemType, string page, string DataType, string MineCode, string SensorNum, string SensorType, string DropListName, string ReportName, string startRow, string rows, string StartTime, string EndTime, string TypeName, string DropName, string Position, string isDG, string Timespan, string TypeKind,string Job,string Department)
        {
            try
            {
            int StartRow = 0;
            int Rows = 0;
            int Page = 0;
          //  if (startRow != null && rows != null && startRow != "" && rows != "" && startRow != "NaN" && startRow != "NaN")
                if (!string.IsNullOrEmpty(rows) && rows!="NaN" )
            {
              //  StartRow = int.Parse(startRow);
                Rows = int.Parse(rows);
            }
                if (!string.IsNullOrEmpty(startRow))
                {
                    StartRow = int.Parse(startRow);
                }
                if (!string.IsNullOrEmpty(page))
                {

                    Page = Convert.ToInt32(page);
                    if (Page > 0)
                    { 
                       StartRow = Rows * (Page - 1) + 1;
                    }
                 
                }
            if (DataType == "AQGZ")
            {
                string a = DropName;
            }
            TransJsonToTreeListModel model = new TransJsonToTreeListModel(SystemType, DataType, MineCode, SensorNum, SensorType, DropListName, ReportName, StartRow, Rows, StartTime, EndTime, TypeName, DropName, Timespan,TypeKind,Job,Department, Position);

            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "No-Cache");
      
                //新增一个标记位  主要区别在于 datagrid返回json格式与supcan返回格式不同

           
            if (isDG == "y")
            {
                if (Position == "Export")
                {
                    Session["Export"] = "y";
                }
                else {
                    Session["Export"] = "n";
                }
                     Response.Write(model.GetDataJsonDG);
            }
            else {

                Response.Write(model.GetDataJson);
            }
            //数据处理并发送数据到前台

           
            //通知前台，数据发送完毕
            Response.Flush();
            Response.End();
           
            }
            catch(Exception ex)
            {
                Response.Write("");
                Response.Flush();
                Response.End();
            }
            finally
            {
               // Response.Close();
            }
        }
    }
}
