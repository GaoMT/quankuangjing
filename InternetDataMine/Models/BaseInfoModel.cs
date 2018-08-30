using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using InternetDataMine.Models.DataService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace InternetDataMine.Models
{
    public class BaseInfoModel
    {
        DataBLL bll = new DataBLL();

        //首页左侧树
        public string ReturnTreeMine(string type)
        {
            DataTable dt = bll.GetMineTreeInfo(type);

            string json = "[";
            json += "{\"id\":\"ALL\",\"text\":\"全部\",\"minecode\":\"\",\"AQJKState\":\"\",\"RYGLState\":\"\"},";
            string OrderRule = "";
            if (dt != null && dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (OrderRule != dt.Rows[i][type].ToString() && OrderRule == "")
                    {
                        OrderRule = dt.Rows[i][type].ToString();
                        json += "{\"id\":\"" + Guid.NewGuid().ToString() + "\"" +
                       ",\"text\":\"" + OrderRule + "\"" +
                       ",\"minecode\":0 , \"AQJKState\":\"\",\"RYGLState\":\"\"";
                        json += " ,\"children\": [";
                    }
                    else if (OrderRule != dt.Rows[i][type].ToString() && OrderRule != "")
                    {
                        OrderRule = dt.Rows[i][type].ToString();
                        json += "]},{\"id\":\"" + Guid.NewGuid().ToString() + "\"" +
                       ",\"text\":\"" + OrderRule + "\"" +
                       ",\"minecode\":0, \"AQJKState\":\"\",\"RYGLState\":\"\"";
                        json += " ,\"children\": [";
                    }
                    else
                    {
                        json += ",";
                    }

                    DataRow dr = dt.Rows[i];
                    json += string.Format(@"{{ " + "\"id\": \"{0}\",\"text\": \"{1}\",\"AQJKState\":\"{2}\",\"RYGLState\":\"{3}\"}}", dr["minecode"].ToString(),
                                          dr["SimpleName"].ToString(),dr["AQJKState"].ToString(),dr["RYGLState"].ToString());
                    //若要增加子树，在}前增加 ,\"children\":[{{\"id\":\"Id\",\"text\":\"Name\"}}]  (实际为"children":[{"id":"Id"}] 代码中要{{}}两个大括号，用{}转义，而不能用\转义     )
                    if (i < dt.Rows.Count - 1)
                    {
                        //json += ",";
                    }
                }
            }
            json += "]}]";
            return json;// "[{\"id\": 1,\"text\": \"Node 1\",\"state\": \"closed\", \"children\": [{\"id\": 11,\"text\": \"Node 11\" },{ \"id\": 12,\"text\": \"Node 12\"}]},{\"id\": 2,\"text\": \"Node 2\",\"state\": \"closed\"}]";
        }
        /// <summary>
        /// 主页中的树
        /// </summary>
        /// <param name="mineCode"></param>
        /// <returns></returns>
        public string ReturnTreeMineInfo(string mineCode)
        {
            DataTable dt = bll.GetMineInfo(mineCode);

            string json = "[";
            json += "{\"id\":\"All\",\"text\":\"全部(安全监控)\",\"minecode\":\"ALL\"},";
            string city = "";
            if (dt != null && dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //针对i=0的情况
                    if (city != dt.Rows[i]["City"].ToString() && city == "")
                    {
                        city = dt.Rows[i]["City"].ToString();

                        json += "{\"id\":\"" + Guid.NewGuid().ToString() + "\"" +
                       ",\"text\":\"" + city + "\"" +
                       ",\"minecode\":0" +
                       ",\"AQJKState\":\"\"" +
                       ",\"RYGLState\":\"\"";
                        json += " ,\"children\": [";
                    }
                    else if (city != dt.Rows[i]["City"].ToString() && city != "")
                    {
                        //针对i！=0，且上个煤矿的城市与本次煤矿城市不同
                        city = dt.Rows[i]["City"].ToString();
                        json += "]},{\"id\":\"" + Guid.NewGuid().ToString() + "\"" +
                       ",\"text\":\"" + city + "\"" +
                       ",\"minecode\":0" +
                       ",\"AQJKState\":\"\"" +
                       ",\"RYGLState\":\"\"";
                        json += " ,\"children\": [";
                    }
                    else
                    {
                        // 针对i！=0，1.该城市无煤矿信息，2.该城市有煤矿信息切与上个煤矿城市相同
                        json += ",";
                    }

                    DataRow dr = dt.Rows[i];
                    //原来的
                    //json += string.Format(@"{{ " + "\"id\": \"{0}\",\"text\": \"{1}\",\"minecode\":\"{4}\" ,\"children\":[{{\"id\":\"AQJK\",\"text\":\"安全监控\",\"State\":\"{2}\"}},{{\"id\":\"RYGL\",\"text\":\"人员管理\",\"State\":\"{3}\"}} ,{{\"id\":\"KSYL\",\"text\":\"矿山压力\",\"State\":\"{5}\"}} ,{{\"id\":\"HZSG\",\"text\":\"火灾束管\",\"State\":\"{6}\"}}            ]}}", dr["rowid"].ToString(),
                    //                      dr["SimpleName"].ToString(), dr["AQJKState"].ToString(), dr["RYGLState"].ToString(), dr["MineCode"].ToString(), dr["ksylstate"].ToString(), dr["hzsgstate"].ToString());

                    json += string.Format(@"{{ " + "\"id\": \"{2}\",\"text\": \"{1}\",\"minecode\":\"{2}\" ,\"children\":[{{", dr["rowid"].ToString(), dr["SimpleName"].ToString(), dr["MineCode"].ToString());
                    int flag = 0;
                    if (!string.IsNullOrEmpty(dr["AQJK"].ToString()))
                    {
                        json += string.Format("    \"id\":\"{1}\",\"text\":\"安全监控\",\"State\":\"{0}\" }}  ", dr["AQJKState"].ToString(), Guid.NewGuid().ToString());
                        flag = 1;
                    }

                    if (!string.IsNullOrEmpty(dr["RYGL"].ToString()))
                    {
                        if (flag == 1)
                        {
                            json += ",{";
                        }

                        json += string.Format("    \"id\":\"{1}\",\"text\":\"人员管理\",\"State\":\"{0}\" }}  ", dr["RYGLState"].ToString(), Guid.NewGuid().ToString());
                        flag = 1;
                    }
                    if (!string.IsNullOrEmpty(dr["ksyl"].ToString()))
                    {
                        if (flag == 1)
                        {
                            json += ",{";
                        }
                        json += string.Format("    \"id\":\"{1}\",\"text\":\"矿山压力\",\"State\":\"{0}\"  }} ", dr["RYGLState"].ToString(), Guid.NewGuid().ToString());
                        flag = 1;
                    }
                    if (!string.IsNullOrEmpty(dr["hzsg"].ToString()))
                    {
                        if (flag == 1)
                        {
                            json += ",{";
                        }
                        json += string.Format("    \"id\":\"{1}\",\"text\":\"火灾束管\",\"State\":\"{0}\" }}  ", dr["hzsgstate"].ToString(), Guid.NewGuid().ToString());
                        flag = 1;
                    }
                    if (flag == 0)
                    {
                      json=  json.Remove(json.Length-1);
                    }
                    json += "]}";

                    //"children":[{""}]
                    //若要增加子树，在}前增加 ,\"children\":[{{\"id\":\"Id\",\"text\":\"Name\"}}]  (实际为"children":[{"id":"Id"}] 代码中要{{}}两个大括号，用{}转义，而不能用\转义     )
                    if (i < dt.Rows.Count - 1)
                    {
                        //json += ",";
                    }
                }
            }
            json += "]}]";
            return json;// "[{\"id\": 1,\"text\": \"Node 1\",\"state\": \"closed\", \"children\": [{\"id\": 11,\"text\": \"Node 11\" },{ \"id\": 12,\"text\": \"Node 12\"}]},{\"id\": 2,\"text\": \"Node 2\",\"state\": \"closed\"}]";
        }

        /// <summary>
        /// 获取设备类型列表
        /// </summary>
        /// <param name="mineCode"></param>
        /// <returns></returns>
        public string ReturnDeviceType(string mineCode, string Type)
        {
            string OutString = "[";
            if (mineCode == "" || mineCode == "0")
            {
                return "[]";
            }
            else
            {
                DataTable dt = bll.GetDevTypeList(mineCode, Type);
                DataRow[] drs = dt.Select(" type like '%" + Type + "%'");
                //foreach (DataRow dr in dt.Rows)
                foreach (DataRow dr in drs)
                {
                    //if (dr["Type"].ToString().Equals(Type))
                    {
                        OutString += "{\"TypeCode\":\"" + dr["TypeCode"] + "\",";
                        OutString += "\"TypeName\":\"" + dr["TypeName"] + "\",";
                        OutString += "\"Unit\":\"" + dr["Unit"] + "\",";
                        OutString += "\"Type\":\"" + dr["Type"] + "\"},";
                    }
                }

                OutString = OutString.Substring(0, OutString.Length - 1);

                OutString += "]";

                return OutString;
            }

        }
        public string LoadChildSensor(string mineCode, string Type,string SysCode)
        {

            string sql = "select tp.PointID SensorNum ,tp.PointName SensorName ,EquipPlace  Place,te.EquipName TypeName,te.Unit from " + SysCode + "_Points  tp left join " + SysCode + "_Equipment te " +
                        " on tp.EquipID=te.ID ";
             if (!string.IsNullOrEmpty(Type))
             {
                 sql += " where te.EquipName='" + Type + "'";
             }
            try
            {
                DataTable dt = new DataDAL().ReturnData(sql);
                int total = dt.Rows.Count;
                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                json = "{\"total\": " + total + ",\"rows\":" + json + "}";
                return json;
            }
            catch (Exception e)
            {
                return "";
            }
        }





        public string LoadChildTypeName(string mineCode, string Type,string SysCode)
        {
            string sql = "select distinct  EquipName TypeName ,Unit,EquipType,case  EquipType when 0 then '模拟量' when 1 then '开关量' when 2 then '累积量' end as Type  from "+SysCode+"_Equipment ";
         
            if (!string.IsNullOrEmpty(Type))
            {
                sql += " where  EquipType=" + Type;
            }
            try{
                sql += " order by EquipType";
                 DataTable dt=new DataDAL().ReturnData(sql);

                 int total = dt.Rows.Count;
                 string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                 json = "{\"total\": " + total + ",\"rows\":" + json + "}";
                 return json;
        
            }
            catch(Exception e)
            {
                return "";
            }
       
        }

        public string ReturnLDKZ(string MineCode, string page, string Type)
        {
            int Page = 0;
            int StartRow = 0;
            if (!string.IsNullOrEmpty(page))
            {
                Page = Convert.ToInt32(page);
                if (Page > 0)
                {
                    StartRow = 20 * (Page - 1) + 1;
                }
            }
            try
            {

                bll.PageSize = 20;
                bll.PageIndex = StartRow / 20;
                DataTableCollection dts = null;
                switch (Type)
                {
                    //联动控制相关
                    case "SensorNum": dts = bll.GetSensorAll(MineCode); break;
                    case "FZBH": dts = bll.GetRYFZAll(MineCode); break;
                    case "BZKH": dts = bll.GetRYBZKHAll(MineCode); break;
                    case "HZBH": dts = bll.GetHZBHAll(MineCode); break;
                    case "CKBH": dts = bll.GetCKBHAll(MineCode); break;
                    case "DH": dts = bll.GetSensorAll(); break;
                    case "WSCF":
                    case "SPST":
                    case "YFST":
                    case "TFST":
                    case "PDYS":
                    case "GDST":
                    case "GZMJK": dts = bll.GetVInfo(Type); break;
                    default: dts = bll.GetSensorAll(MineCode); break;
                }

                string ReturnString = "";
                if (string.IsNullOrEmpty(MineCode) && Type != "DH" && Type != "WSCF" && Type != "SPST" && Type != "YFST" && Type != "TFST" && Type != "PDYS" && Type != "GDST" && Type != "GZMJK")
                {
                    return "{}";
                }
                else
                {
                    if (dts != null)
                    {
                        int TotalRows = int.Parse(dts[0].Rows[0][0].ToString());

                        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                        string json = JsonConvert.SerializeObject(dts[1], Formatting.Indented, timeConverter);
                        ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";

                    }
                    else return "{}";
                }
                return ReturnString;

            }
            catch (Exception e)
            {

                return "{}";
            }
         
        }




        public string LDKZData(string MineCode,string page,string rows)
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
            dts = bll.GetLDKZ(MineCode);
            if (dts != null)
            {
                int TotalRows = int.Parse(dts[0].Rows[0][0].ToString());

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string json = JsonConvert.SerializeObject(dts[1], Formatting.Indented, timeConverter);
                ReturnString = "{\"total\": " + TotalRows + ",\"rows\":" + json + "}";
                return ReturnString;
            }
            else return "{}";
            }
            catch(Exception e)
            {
                return e.Message;
            }
           
        
        }
    
        /// <summary>
        /// 根据煤矿编号、设备名称编号、设备类型查询测点列表 2016-1-11 申云飞
        /// </summary>
        /// <param name="mineCode">煤矿编号</param>
        /// <param name="SensorNameCode">设备名称编号</param>
        /// <param name="devtype">设备类型 A表示模拟量、D表示开关量,L表示累积量</param>
        /// <returns></returns>
        public string ReturnComboDeviceInfo(string mineCode, string SensorNameCode, string devtype)
        {
            if (devtype == "N")
            {
                return "{\"total\":0,\"rows\":[{}]}";
            }
            else
            {

                DataTableCollection dts = bll.GetSensorInfo(mineCode, SensorNameCode, devtype);
                string ReturnString = "{";
                /*
                 * 
                 * 
                 * 
    wheredata = " select Row_Number() over (order by getdate() asc) as TmpID,SimpleName,o.SensorNum,d.TypeName,d.Unit,o.Place,o.Range,"
    + " o.AlarmLower,o.AlarmHigh,o.AlarmLowerRemove,o.AlarmHighRemove,o.OutPowerLower,o.OutPowerHigh,o.InPowerLower,o.InPowerHigh,"
    + " o.SensorTime from AQMC o left join DeviceType d on o.Type=d.TypeCode left join MineConfig g on o.MineCode=g.MineCode "
                 * 
                */

                if (mineCode == "0" || mineCode == "")
                {
                    return "{}";
                }
                else
                {
                    if (devtype == "A")
                    {
                        if (dts.Count > 0)
                        {
                            ReturnString += "\"total\":" + dts[0].Rows[0][0].ToString() + ",\"rows\":[";
                            foreach (DataRow dr in dts[1].Rows)
                            {
                                ReturnString += "{\"SensorNum\":\"" + dr["SensorNum"] + "\",";
                                ReturnString += "\"TypeName\":\"" + dr["TypeName"] + "\",";
                                ReturnString += "\"Place\":\"" + dr["Place"] + "\",";
                                ReturnString += "\"Unit\":\"" + dr["Unit"] + "\",";
                                ReturnString += "\"AlarmLower\":\"" + dr["AlarmLower"] + "\",";
                                ReturnString += "\"AlarmHigh\":\"" + dr["AlarmHigh"] + "\"},";
                            }

                            ReturnString = ReturnString.Substring(0, ReturnString.Length - 1);
                            ReturnString += "]}";

                        }
                    }
                    else if (devtype == "D")
                    {
                        if (dts.Count > 0)
                        {
                            ReturnString += "\"total\":" + dts[0].Rows[0][0].ToString() + ",\"rows\":[";
                            foreach (DataRow dr in dts[1].Rows)
                            {
                                ReturnString += "{\"SensorNum\":\"" + dr["SensorNum"] + "\",";
                                ReturnString += "\"TypeName\":\"" + dr["TypeName"] + "\",";
                                ReturnString += "\"Place\":\"" + dr["Place"] + "\",";
                                ReturnString += "\"ZeroMeaning\":\"" + dr["0态含义"] + "\",";
                                ReturnString += "\"OneMeaning\":\"" + dr["1态含义"] + "\",";
                                ReturnString += "\"TwoMeaning\":\"" + dr["2态含义"] + "\"},";
                            }

                            ReturnString = ReturnString.Substring(0, ReturnString.Length - 1);
                            ReturnString += "]}";
                        }
                    }
                    else
                    { ReturnString = ""; }
                    return ReturnString;
                }
            }
          
        }



        /// <summary>
        /// 为煤矿下拉控件-树形结构加载内容
        /// </summary>
        /// <param name="mineCode"></param>
        /// <returns></returns>
        public string ReturnComboTreeMineInfo(string mineCode)
        {
            DataTable dt = bll.GetMineInfo(mineCode);

            string json = "[";
   
            string city = "";
            if (dt != null && dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (city != dt.Rows[i]["City"].ToString() && city == "")
                    {
                        city = dt.Rows[i]["City"].ToString();
                        json += "{\"id\":\"0\"" +
                       ",\"text\":\"" + city + "\"" +
                       ",\"state\":\"open\"" +
                       ",\"minecode\":0";

                        json += " ,\"children\": [";
                    }
                    else if (city != dt.Rows[i]["City"].ToString() && city != "")
                    {
                        city = dt.Rows[i]["City"].ToString();
                        json += "]},{\"id\":\"0\"" +
                       ",\"text\":\"" + city + "\"" +
                        ",\"state\":\"closed\"" +
                       ",\"minecode\":0";
                        json += " ,\"children\": [";
                    }
                    else
                    {
                        json += ",";
                    }

                    DataRow dr = dt.Rows[i];
                    json += string.Format(@"{{ " + "\"id\": \"{0}\",\"text\": \"{1}\",\"minecode\":\"{2}\"}}", dr["MineCode"].ToString(),
                                          dr["SimpleName"].ToString(), dr["MineCode"].ToString());

                    if (i < dt.Rows.Count - 1)
                    {
                        //json += ",";
                    }
                }
            }
            json += "]}]";

            return json;// "[{\"id\": 1,\"text\": \"Node 1\",\"state\": \"closed\", \"children\": [{\"id\": 11,\"text\": \"Node 11\" },{ \"id\": 12,\"text\": \"Node 12\"}]},{\"id\": 2,\"text\": \"Node 2\",\"state\": \"closed\"}]";

        }


        public string GetMineTreeList(string mineCode)
        {
            DataTable dt = bll.GetMineInfo(mineCode);

            string json = "[{" +
                          "\"id\":\"0\"" +
                          ",\"text\":\"全部\"";

            if (dt != null && dt.Rows.Count > 0)
            {
                json += " ,\"children\": [";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    json += string.Format(@"{{ " + "\"id\": \"{0}\",\"text\": \"{1}\"}}", dr["MineCode"].ToString(), dr["SimpleName"].ToString());

                    if (i < dt.Rows.Count - 1)
                    {
                        json += ",";
                    }
                }
                json += "]";
            }
            json += "}]";

            return json;// "[{\"id\": 1,\"text\": \"Node 1\",\"state\": \"closed\", \"children\": [{\"id\": 11,\"text\": \"Node 11\" },{ \"id\": 12,\"text\": \"Node 12\"}]},{\"id\": 2,\"text\": \"Node 2\",\"state\": \"closed\"}]";

        }
    }
}