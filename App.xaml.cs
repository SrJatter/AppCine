using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AppCine
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Login login = new Login();
            bool? r1 = login.ShowDialog();
            if(r1 != true)
            {
                if (login.failedAttempts == 3)
                {
                    Current.Shutdown();
                } else
                {
                    MainWindow mainWindow = new MainWindow();
                    bool? r2 = mainWindow.ShowDialog();
                    if (r2 != true)
                    {
                        Current.Shutdown();
                    }
                }

            }

        }
    }
}
