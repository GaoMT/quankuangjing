using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace SHH.TF.DAL
{
    //通风系统实时数据表
    public class TB_TF_RealValues
    {
        //批量更新数据n
        public static void UpdateValueBatch()
        {

        }

        public static void UpdateValue(Guid iD, string value, DateTime time, int v)
        {
            //opc客户端读取opc服务端的时间是utc时间，比系统时间少大约8小时,需要转换
            time = time.Add(DateTime.Now - DateTime.UtcNow);

            using (SqlCommand command = new SqlCommand("TF_AddRealValues", Database.DBConnection))
            {
                command.CommandTimeout = 5;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("PointID", iD));
                command.Parameters.Add(new SqlParameter("varValue", value));
                command.Parameters.Add(new SqlParameter("UpdateTime", time));
                command.Parameters.Add(new SqlParameter("VarStatus", v));
                try
                {
                    command.ExecuteNonQuery();
                }
                catch
                { }
            }
        }

    }
}
