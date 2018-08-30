using SHH.TF.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SHH.TF.DAL
{
    public class TB_TF_OPCGroups
    {
        public static void AddOPCGroups(SHHOPCGroup group)
        {
            SqlParameter[] parameterList = new SqlParameter[8]
            {
                new SqlParameter("OPCServerID",group.Server.ID),
                new SqlParameter("OPCGroupID",group.ID),
                new SqlParameter("Name",group.Name),
                new SqlParameter("UpdateRate",group.UpdateRate),
                new SqlParameter("DeadBend",group.DeadBend),
                new SqlParameter("TimeBias",group.TimeBias),
                new SqlParameter("IsActive",group.IsActive),
                new SqlParameter("IsSubscribed",group.IsSubscribed)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_AddOPCGroups", parameterList);
        }

        public static void DeleteOPCGroup(Guid groupId)
        {
            SqlParameter[] parameterList = new SqlParameter[1]
            {
                new SqlParameter("OPCGroupID",groupId)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_DeleteOPCGroups", parameterList);
        }

        public static void UpdateOPCGroups(SHHOPCGroup group)
        {
            SqlParameter[] parameterList = new SqlParameter[8]
            {
                new SqlParameter("OPCServerID",group.Server.ID),
                new SqlParameter("OPCGroupID",group.ID),
                new SqlParameter("Name",group.Name),
                new SqlParameter("UpdateRate",group.UpdateRate),
                new SqlParameter("DeadBend",group.DeadBend),
                new SqlParameter("TimeBias",group.TimeBias),
                new SqlParameter("IsActive",group.IsActive),
                new SqlParameter("IsSubscribed",group.IsSubscribed)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_UpdateOPCGroups", parameterList);
        }

        public static SqlDataReader GetGroups()
        {
            SqlDataReader dt = SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.Text, "Select * from TF_OPCGroups", null);
            return dt;
        }
    }
}
