using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace SHH.TF.DAL
{
    public class Database
    {
        public static bool Init()
        {//初始化数据库
            try
            {
                #region [获取XML配置]
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load("Config.xml");
                    String server = doc.SelectSingleNode("Config/Server").InnerText;
                    String user = doc.SelectSingleNode("Config/UserName").InnerText;
                    String password = doc.SelectSingleNode("Config/PassWord").InnerText;
                    String dataBase = doc.SelectSingleNode("Config/DataBase").InnerText;

                    sqlConStr= "Data Source=" + server + ";Initial Catalog=" + dataBase + ";User Id=" + user + ";Password=" + password + ";";
                }
                catch
                {

                    if (File.Exists("Config.xml"))
                    {
                        File.Delete("Config.xml");
                    }
                    return false;
                }
                #endregion


                con = new SqlConnection(sqlConStr);
                con.Open();

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }


        public static SqlConnection DBConnection { get { return con; } }




        public static string sqlConStr;
        private static SqlConnection con = null;

    }
}
