using InternetDataMine.Models;
using InternetDataMine.Models.DataService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetDataMine.Controllers
{
    public class LDKZController : Controller
    {
        //
        // GET: /LDKZ/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LDKZIndex(string MineCode)
        {
            LoadModel lm = new LoadModel();
            lm.UserMineCode = MineCode;
            return View(lm);
        }

        public ActionResult SaveLDKZ(string MineCode, string SensorNums, string IsRYHJ, string FZBH, string BZKH, string IsGBKZ, string HZBH, string Content, string User, string IsNew, string CKBH, string ContentCK, string IsCKKZ)
        {
            string StationCodes = "";
            string JobCardCode = "";
            string GBCodes = "";
            string CKCodes = "";
            int IsRY = 0;
            int IsGB = 0;
            int IsCK = 0;
            try
            {

                if (string.IsNullOrEmpty(MineCode))
                {
                    return Json(new { data = "请选择煤矿！" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(SensorNums))
                {
                    return Json(new { data = "请选择测点！" }, JsonRequestBehavior.AllowGet);
                }
                if (IsGBKZ == "true")
                {
                    IsGB = 1;
                    if (!string.IsNullOrEmpty(Content))
                    {
                        Content = Content.Replace("'", "\"");
                    }
                    else
                    {
                        return Json(new { data = "请输入广播内容！" }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (IsCKKZ == "true")
                {
                    IsCK = 1;
                    if (!string.IsNullOrEmpty(ContentCK))
                    {
                        ContentCK = ContentCK.Replace("'", "\"");
                    }
                    else
                    {
                        return Json(new { data = "请输入广播内容！" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (IsRYHJ == "true")
                {
                    IsRY = 1;
                }

                if (!string.IsNullOrEmpty(FZBH))
                {
                    StationCodes = FZBH.Replace(",", "|");
                }
                if (!string.IsNullOrEmpty(BZKH))
                {
                    JobCardCode = BZKH.Replace(",", "|");
                }
                if (!string.IsNullOrEmpty(HZBH))
                {
                    GBCodes = HZBH.Replace(",", "|");
                }
                if (!string.IsNullOrEmpty(CKBH))
                {
                    CKCodes = CKBH.Replace(",", "|");
                }
                if (IsNew == "1")
                {
                    bool Exist = IsExist(MineCode, SensorNums);
                    if (Exist)
                    {
                        return Json(new { data = "已存在改测点的联动控制信息，请重新选择！" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        string sql = string.Format("insert into LDKZ values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", MineCode, SensorNums, StationCodes, JobCardCode, GBCodes, Content, IsRY, IsGB, User, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),CKCodes,ContentCK,IsCK);
                        var dal = new DataDAL();
                        bool success = dal.ExcuteSql(sql);
                        if (success)
                        {
                            return Json(new { data = "添加成功!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { data = "添加失败，请检查数据" }, JsonRequestBehavior.AllowGet);
                        }

                    }


                }
                else
                {
                    string sql = string.Format("update LDKZ set MineCode='{0}',SensorNums='{1}',StationCodes='{2}',JobCardCodes='{3}',GBCodes='{4}',GBContent='{5}',IsRYHJKZ='{6}',IsGBKZ='{7}',[User]='{8}',UpdateTime='{9}',CKCodes='{10}',CKContent='{11}', IsCKKZ='{12}' where MineCode='{0}' and SensorNums='{1}'",
                        MineCode, SensorNums, StationCodes, JobCardCode, GBCodes, Content, IsRY, IsGB, User, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),CKCodes,ContentCK,IsCK);
                    var dal = new DataDAL();
                    bool success = dal.ExcuteSql(sql);
                    if (success)
                    {
                        return Json(new { data = "更新成功!" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {

                        bool Exist = IsExist(MineCode, SensorNums);
                        if (Exist)
                        {
                            return Json(new { data = "已存在该测点的联动控制，请重新选择" }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json(new { data = "更新失败，请检查数据是否合法" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }



            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        public bool IsExist(string MineCode, string SensorNums)
        {
            bool Success=false;
           
                string sql = string.Format("select * from LDKZ where MineCode='{0}' and SensorNums='{1}'", MineCode, SensorNums);
                var dal = new DataDAL();
                  DataTable dt = null;
            try
            {
                dt = dal.ReturnData(sql);
                if (dt.Rows.Count > 0)
                {
                    Success = true;
                }

            }
            catch (Exception e) { }
            return Success;
        }


        public ActionResult Delete(string MineCode, string SensorNum)
        {

            try
            {
                if (!string.IsNullOrEmpty(MineCode) && !string.IsNullOrEmpty(SensorNum))
                {


                    string[] MineCodes = MineCode.Split(new char[] { ',' });
                    string[] SensorNums = SensorNum.Split(new char[] { ',' });
                    List<string> sql = new List<string>();
                    for (int i = 0; i < MineCodes.Length; i++)
                    {
                        sql.Add(string.Format("delete from  LDKZ where MineCode='{0}' and SensorNums='{1}'", MineCodes[i], SensorNums[i]));

                    }
                    var dal = new DataDAL();
                    bool resul = dal.ExcuteSqls(sql);
                    if (resul)
                    {
                        return Json(new { data = "删除成功" }, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {
                        return Json(new { data = "删除失败，请检查数据" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { data = "删除失败，无需删除的数据" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public void LoadLDKZ(string MineCode, string rows, string page)
        {
            BaseInfoModel model = new BaseInfoModel();

            Response.Write(model.LDKZData(MineCode, page,rows));
            Response.End();
        }



    

    }
}
