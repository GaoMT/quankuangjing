using SHH.TF.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SHH.TF.DAL
{
    public class TB_TF_Points
    {
        public static void AddPoints(SHHOPCItem opcItem)
        {
            SqlParameter[] parameterList = new SqlParameter[6]
            {
                new SqlParameter("OPCGroupID",opcItem.Group.ID),
                new SqlParameter("PointID",opcItem.ID),
                new SqlParameter("PointName",opcItem.PointName),
                new SqlParameter("EquipID",(short)opcItem.Equipment.Id),
                new SqlParameter("EquipPlace",opcItem.PointPlace),
                new SqlParameter("PointAddress ",opcItem.OPCItemID)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_AddPoints", parameterList);
        }

        public static void DeletePoints(Guid id)
        {
            SqlParameter[] parameterList = new SqlParameter[1]
            {
                new SqlParameter("PointID",id)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_DeletePoints", parameterList);
        }

        public static void UpdatePoints(SHHOPCItem opcItem)
        {
            SqlParameter[] parameterList = new SqlParameter[6]
            {
                new SqlParameter("OPCGroupID",opcItem.Group.ID),  
                new SqlParameter("PointID",opcItem.ID),
                new SqlParameter("PointName",opcItem.PointName),
                new SqlParameter("EquipID",(short)opcItem.Equipment.Id),
                new SqlParameter("EquipPlace",opcItem.PointPlace),
                new SqlParameter("PointAddress ",opcItem.OPCItemID)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_UpdatePoints", parameterList);
        }

        public static SqlDataReader GetPoints()
        {
            SqlDataReader dt = SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.Text, "Select * from TF_Points", null);
            return dt;
        }
    }
}
