﻿using System;
using System.Windows.Forms;
using System.Threading;
using fireBwall.UI.Tabs;
using fireBwall.Configuration;
using fireBwall.Updates;
using fireBwall.Filters.NDIS;

namespace fireBwall
{
    public static class Program
    {
        public static event ThreadStart OnShutdown;
        public static MainWindow mainWindow = null;
        public static TrayIcon trayIcon = null;
        public static UpdateChecker updater = new UpdateChecker();

        public static void Shutdown()
        {
            ConfigurationManagement.Instance.SaveAllConfigurations();
            if (OnShutdown != null)
                OnShutdown();
            Application.Exit();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                ConfigurationManagement.Instance.ConfigurationPath = args[0];
            }            
            ConfigurationManagement.Instance.LoadAllConfigurations();
            foreach (INDISFilter filter in ProcessingConfiguration.Instance.NDISFilterList.GetAllAdapters())
            {
                filter.StartProcessing();
            }
            Program.OnShutdown += ProcessingConfiguration.Instance.NDISFilterList.ShutdownAll;
            Program.OnShutdown += ConfigurationManagement.Instance.SaveAllConfigurations;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
