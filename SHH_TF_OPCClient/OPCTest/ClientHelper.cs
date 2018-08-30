using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCTest
{
    public class ClientHelper
    {

        /// <summary>
        /// 获取OPC服务器名
        /// </summary>
        /// <param name="hostName">主机名或IP</param>
        /// <returns>返回服务器列表</returns>
        public static List<String> GetOPCServerName(String hostName)
        {
            try
            {
                object opcServers = (new OPCServer()).GetOPCServers(hostName);
                List<String> serverList = new List<String>();
                foreach (String s in (Array)opcServers)
                {
                    serverList.Add(s);
                }
                return serverList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 连接到指定的OPC服务器
        /// </summary>
        /// <param name="serverName">服务器名称</param>
        /// <param name="serverIP">服务器IP</param>
        /// <returns>返回的OPC服务器</returns>
        public static OPCServer ConnectToServer(String serverName, String serverIP)
        {
            OPCServer opcServer = new OPCServer();
            try
            {
                opcServer.Connect(serverName, serverIP);
                if (opcServer.ServerState != (int)OPCServerState.OPCRunning)
                {
                    opcServer.Disconnect();
                    return null;
                }
            }
            catch
            {
                opcServer.Disconnect();
                return null;
            }

            return opcServer;
        }

        /// <summary>
        /// 获取OPC服务器的相关信息
        /// </summary>
        /// <param name="opcServer">OPC服务器</param>
        /// <returns>返回服务器信息</returns>
        public static OPCServerInfo GetServerInfo(OPCServer opcServer)
        {
            OPCServerInfo serverInfo = new OPCServerInfo
            {
                StartTime = opcServer.StartTime,
                ServerVersion = opcServer.MajorVersion.ToString() +
                "." + opcServer.MinorVersion.ToString() +
                "." + opcServer.BuildNumber.ToString()
            };

            return serverInfo;
        }

        /// <summary>
        /// 得到PC服务器的节点
        /// </summary>
        /// <param name = "opcServer" > OPC服务器 </ param >
        /// < returns > 返回展开后的节点数据 </ returns >
        public static List<String> GetItemList(OPCServer opcServer)
        {
            List<String> list = new List<string>();

            if (opcServer.ServerState == (int)OPCServerState.OPCRunning)
            {
                OPCBrowser opcBrowser = opcServer.CreateBrowser();
                //展开分支
                opcBrowser.ShowBranches();
                //展开叶子
                opcBrowser.ShowLeafs(true);
                //遍历节点
                foreach (String item in opcBrowser)
                {
                    if (item.Contains("_Hint"))
                        continue;
                    list.Add(item.ToString());
                }

                
            }

            return list;
        }
    }
}
