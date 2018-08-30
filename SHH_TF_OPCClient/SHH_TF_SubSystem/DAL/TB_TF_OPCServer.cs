using SHH.TF.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SHH.TF.DAL
{
    public class TB_TF_OPCServer
    {
        public static void AddOPCServer(SHHOPCServer server)
        {
            SqlParameter[] parameterList = new SqlParameter[4]
            {
                new SqlParameter("OPCServerID",server.ID),
                new SqlParameter("Name",server.Name),
                new SqlParameter("MachineIP",server.IP.ToString()),
                new SqlParameter("OPCServerName",server.OPCServerName)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_AddOPCServer", parameterList);
        }

        public static void DeleteOPCServer(Guid id)
        {
            SqlParameter[] parameterList = new SqlParameter[1]
            {
                new SqlParameter("OPCServerID",id)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_DeleteOPCServer", parameterList);
        }

        public static void UpdateOPCServer(SHHOPCServer server)
        {
            SqlParameter[] parameterList = new SqlParameter[4]
            {
                new SqlParameter("OPCServerID",server.ID),
                new SqlParameter("Name",server.Name),
                new SqlParameter("MachineIP",server.IP.ToString()),
                new SqlParameter("OPCServerName",server.OPCServerName)
            };

            SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.StoredProcedure, "TF_UpdateOPCServer", parameterList);
        }


        public static SqlDataReader GetServer()
        {
            SqlDataReader dt = SqlHelper.ExecuteReader(Database.sqlConStr, CommandType.Text, "Select * from TF_OPCServer", null);
            return dt;
        }
    }
}
