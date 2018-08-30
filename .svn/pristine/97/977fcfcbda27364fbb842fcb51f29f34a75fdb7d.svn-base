
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SHH.TF.BLL;
using SHH.TF.DAL;

namespace SHH_TF_SubSystem
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //try
            //{
                Database.Init();
                OPCManager.Load();//加载参数
                OPCManager.StartAll();//启动所有OPC通讯
            //}
            //catch (Exception ee)
            //{
            //    MessageBox.Show(ee.Message);
            //}
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            OPCManager.CloseAll();//关闭所有OPC通讯
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
    }
}
