using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyOPCTest
{
    class Program
    {
        const string groupName = "Test1";
        const string itemID1 = "a.b.f";
        const string itemID2 = "a.c.a";
        static OPCServerClass _opcServer = null;


        static void Main(string[] args)
        {
            InitServer();
            Browser();
            InitGroups();
            InitItems();

            Console.ReadLine();
            _opcServer.Disconnect();
        }

        //初始化OPC服务器
        static void InitServer()
        {
            try
            {
                _opcServer = new OPCServerClass();
                _opcServer.ClientName = "Test";
                _opcServer.ServerShutDown += _opcServer_ServerShutDown;
                _opcServer.Connect("Knight.OPC.Server.Demo", "127.0.0.1");
                PrintInfo("ServerName", _opcServer.ServerName);
                PrintInfo("ServerNode", _opcServer.ServerNode);
                PrintInfo("MajorVersion", _opcServer.MajorVersion.ToString());
                PrintInfo("MinorVersion", _opcServer.MinorVersion.ToString());
                PrintInfo("VendorInfo", _opcServer.VendorInfo);

                PrintInfo("Bandwidth", _opcServer.Bandwidth.ToString());
                PrintInfo("BuildNumber", _opcServer.BuildNumber.ToString());
                PrintInfo("LocaleID", _opcServer.LocaleID.ToString());

                PrintInfo("StartTime", _opcServer.StartTime.ToString());
                PrintInfo("CurrentTime", _opcServer.CurrentTime.ToString());
                PrintInfo("LastUpdateTime", _opcServer.LastUpdateTime.ToString());
                PrintInfo("ServerState", _opcServer.ServerState.ToString());
            }
            catch (Exception ee)
            {
                PrintInfo("InitServer Error", ee.Message);
            }
        }

        static void Browser()
        {
            OPCBrowser browser = _opcServer.CreateBrowser();
            OPCNamespaceTypes type = (OPCNamespaceTypes)(browser.Organization);
            PrintInfo("browser Type", type.ToString());

            browser.ShowBranches();

            for (int i = 1; i <= browser.Count; i++)
            {
                Console.WriteLine(browser.Item(i));

                browser.MoveDown(browser.Item(i));
                browser.ShowBranches();

                for (int j = 1; j <= browser.Count; j++)
                {
                    Console.WriteLine(string.Format("   {0}", browser.Item(j)));


                    browser.MoveDown(browser.Item(j));
                    browser.ShowLeafs();

                    for (int k = 1; k <= browser.Count; k++)
                    {
                        Console.WriteLine(string.Format("      {0}   ItemID = {1}", browser.Item(k), browser.GetItemID(browser.Item(k))));
                    }
                }
            }
        }


        //初始化OPC组
        static void InitGroups()
        {
            OPCGroups groups = _opcServer.OPCGroups;
            groups.DefaultGroupIsActive = true;
            groups.DefaultGroupUpdateRate = 1000;
            groups.DefaultGroupDeadband = 0;
            groups.DefaultGroupLocaleID = 1024;

            OPCGroup group = groups.Add(groupName);
            group.IsSubscribed = true;//订阅
            group.DataChange += Group_DataChange;
        }
        

        //初始化OPC行
        static void InitItems()
        {
            OPCGroup group = _opcServer.OPCGroups.GetOPCGroup(groupName);

            OPCItems items = group.OPCItems;

            OPCItem a = items.AddItem(itemID1, 1);
            OPCItem b = items.AddItem(itemID2, 2);


            a = items.Item(2);

            b = items.Item(1);
            int c = 0;
            c = a.ClientHandle;
            c = b.ClientHandle;
            c = 0;

        }

        //服务器关闭
        private static void _opcServer_ServerShutDown(string Reason)
        {
            try
            {
                Console.WriteLine(string.Format("OPC服务器关闭，原因：{0}", Reason));
                _opcServer.Disconnect();
            }
            catch
            { }
        }

        //订阅事件通知
        private static void Group_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            try
            {
                PrintInfo("dataUpdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                for (int i = 1; i <= NumItems; i++)
                {
                    PrintInfo("ClientHandle", ClientHandles.GetValue(i).ToString());
                    PrintInfo("ItemValues", ItemValues.GetValue(i).ToString());
                    PrintInfo("Qualities", Qualities.GetValue(i).ToString());
                    PrintInfo("TimeStamps", TimeStamps.GetValue(i).ToString());
                }

                Console.WriteLine("=======================================================");
            }
            catch
            { }
        }

        static void PrintInfo(string property, string value)
        {
            Console.WriteLine(string.Format("{0}\t\t:{1}", property, value));
        }
    }
}
