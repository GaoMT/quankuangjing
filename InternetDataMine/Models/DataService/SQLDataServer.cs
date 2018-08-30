﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using InternetDataMine.Controllers;
using System.Web.UI;

namespace InternetDataMine.Models.DataService
{

    public static class SQLDataServer
    {
        public static DataTable ToDataTable(string commandtext, string connectionstring)
        {
            DataTable tmp_dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlDataAdapter dat = new SqlDataAdapter(commandtext, conn);
                    dat.Fill(tmp_dt);
                }
            }
            catch { }

            return tmp_dt;
        }

        public static DataSet ToDataSet(string commandtext, string connectionstring)
        {
            DataSet tmp_dt = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlDataAdapter dat = new SqlDataAdapter(commandtext, conn);
                    dat.Fill(tmp_dt);
                }
            }
            catch { }
            return tmp_dt;
        }

        public static DataTable ProcedureDataTable(string procedurename, SqlParameter[] parmenters, string connectionstring)
        {
            DataTable tmp_dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmmd = new SqlCommand();
                    cmmd.CommandType = CommandType.StoredProcedure;
                    cmmd.CommandText = procedurename;
                    cmmd.Connection = conn;

                    if (parmenters != null && parmenters.Length > 0)
                    {
                        foreach (SqlParameter para in parmenters)
                        {
                            cmmd.Parameters.Add(para);
                        }
                    }
                    SqlDataAdapter dat = new SqlDataAdapter();
                    dat.SelectCommand = cmmd;
                    dat.Fill(tmp_dt);
                }
            }
            catch { }
            return tmp_dt;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="procedurename">存储过程名称</param>
        /// <param name="parmenters">存储过程参数</param>
        /// <param name="connectionstring">数据库连接字符串</param>
        /// <returns></returns>
        public static DataSet ProcedureDataSet(string procedurename, SqlParameter[] parmenters, string connectionstring)
        {
            DataSet tmp_ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    try
                    {
                        SqlCommand cmmd = new SqlCommand();
                        cmmd.CommandType = CommandType.StoredProcedure;
                        cmmd.CommandText = procedurename;
                        cmmd.Connection = conn;
                        conn.Open();
                        cmmd.CommandTimeout = 180;
                        if (parmenters != null && parmenters.Length > 0)
                        {
                            foreach (SqlParameter para in parmenters)
                            {
                                cmmd.Parameters.Add(para);
                            }
                        }
                        SqlDataAdapter dat = new SqlDataAdapter();
                        dat.SelectCommand = cmmd;
                        dat.Fill(tmp_ds);
                        cmmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        log.WriteTextLog(ex.Message, DateTime.Now);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                log.WriteTextLog(ex.Message, DateTime.Now);
            }
            return tmp_ds;
        }

        public static bool OperationSQL(string commandtext, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                bool Result = false;
                try
                {
                    SqlCommand cmmd = new SqlCommand(commandtext, conn);
                    conn.Open();
                   int returnrows= cmmd.ExecuteNonQuery();
                   if (returnrows > 0)
                   {
                       Result = true;

                   }
                   else
                   {
                       Result = false;
                   }
                   return Result;
                }
                catch(Exception e)
                {
                    conn.Close();
                    log.WriteTextLog(e.Message, DateTime.Now);
                    return false;
                }
            }
        }



        public static bool OperationSQLs(List<string> commandtext, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                bool Result = false;
                conn.Open();
                SqlTransaction myTrans1 = conn.BeginTransaction();
                try
                {

                    string text = string.Join(" ", commandtext.ToArray());

                    SqlCommand cmmd = new SqlCommand(text, conn);
                    cmmd.Transaction = myTrans1;
                   
                    int returnrows = cmmd.ExecuteNonQuery();
                    if (returnrows == commandtext.Count())
                    {
                        myTrans1.Commit(); 
                        return true;
                    }
                    else
                    {
                        myTrans1.Rollback();
                        return false;
                    }

                }
                catch
                {
                    myTrans1.Rollback();
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public static int ExecSql(string commandtext, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try
                {
                    int result = 0;
                    SqlCommand cmmd = new SqlCommand(commandtext, conn);
                    conn.Open();
                    result = cmmd.ExecuteNonQuery();
                    conn.Close();
                    return result;
                }
                catch
                {
                    conn.Close();
                    return 0;
                }
            }
        }

        public static bool OperationProcedure(string procedurename, SqlParameter[] parmenters, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try
                {
                    SqlCommand cmmd = new SqlCommand();
                    cmmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmmd.CommandText = procedurename;
                    cmmd.Connection = conn;
                    foreach (SqlParameter para in parmenters)
                    {
                        cmmd.Parameters.Add(para);
                    }

                    int count = cmmd.ExecuteNonQuery();
                    conn.Close();

                    if (count > 0)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                }
                catch
                {
                    conn.Close();
                    return false;
                }
            }
        }

        public static bool InsertFile(string sql, byte[] FileContent, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                SqlCommand com = new SqlCommand(sql,conn);
                try
                {
                    conn.Open();
                    com.CommandType = CommandType.Text;
                    com.CommandText = sql;
                    if(FileContent!=null)
                    {
                        SqlParameter spFile = new SqlParameter("@filecontent", SqlDbType.Image);
                        spFile.Value = FileContent;
                        com.Parameters.Add(spFile);
                    }
                    if(com.ExecuteNonQuery()>0)
                    { return true; }
                    else { return false; }
                    
                }
                catch
                {
                    conn.Close();
                    return false;
                }
            }
        }
    }
}