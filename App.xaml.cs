using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
            bool exitApp = false; // Bandera para determinar si se debe cerrar la aplicación

            while (!exitApp)
            {
                Login login = new Login();
                bool? r1 = login.ShowDialog();
                if (r1 != true)
                {
                    if (login.failedAttempts == 3)
                    {
                        exitApp = true;
                    }
                    else if (login.cancelStatus == true){
                        exitApp = true;
                    }
                    else
                    {
                        MainWindow mainWindow = new MainWindow();
                        bool? r2 = mainWindow.ShowDialog();
                        if (r2 != true)
                        {
                            if (mainWindow.exitStatus == true)
                            {
                                exitApp = true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                }
                Current.Shutdown();
            }
        }
    }
}
