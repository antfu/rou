using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Rou
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MUTEXNAME = "antfu.com Rou";
        private static readonly Mutex _mutex = new Mutex(false, MUTEXNAME);

        public App()
        {
            if (!_mutex.WaitOne(TimeSpan.Zero))
            {
                MessageBox.Show("Another instance is running. Exiting...");
                Shutdown();
            }
        }
    }
  

}
