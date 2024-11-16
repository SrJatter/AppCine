using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Input;

namespace AppCine
{
    public partial class MainWindow : Window
    {
        public bool IsAdmin { get; set; } = true;
        private bool exitButtonClicked = false; // Bandera para detectar clic en "Exit"
        public bool exitStatus = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.Closing += MainWindow_Closing;
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sidebar.SelectedItem is NavButton selected)
            {
                if (selected.Name == "Exit")
                {
                    exitButtonClicked = true; // Indicar que se hizo clic en el botón "Exit"
                    this.DialogResult = false;
                    this.Close();
                }

                // Navegar solo si no se seleccionó "Exit"
                if (selected.Navlink != null)
                {
                    navframe.Navigate(selected.Navlink);
                }
            }
        }

        private void mouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement targetElement)
            {
                popup_uc.PlacementTarget = targetElement;
                popup_uc.Placement = PlacementMode.Right;
                popup_uc.VerticalOffset = -10;
                popup_uc.IsOpen = true;

                switch (targetElement.Name)
                {
                    case "Reserve":
                        Header.PopupText.Text = "Reservar";
                        break;
                    case "Upload":
                        Header.PopupText.Text = "Subir Peli";
                        break;
                    case "About":
                        Header.PopupText.Text = "Info";
                        break;
                    case "Exit":
                        Header.PopupText.Text = "Salir";
                        break;
                    default:
                        Header.PopupText.Text = "Acción";
                        break;
                }
            }
        }

        private void mouseExit(object sender, MouseEventArgs e)
        {
            popup_uc.IsOpen = false;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!exitButtonClicked)
            {
                Trace.WriteLine("Cierre de ventana con Alt+F4 o X");
                exitStatus = true;
            }
            // Reiniciar la bandera
            exitButtonClicked = false;
        }
    }
}
