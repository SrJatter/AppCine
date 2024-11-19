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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _themeChange;
        public bool ThemeChange
        {
            get => _themeChange;
            set
            {
                if (_themeChange != value)
                {
                    _themeChange = value;
                    OnPropertyChanged(nameof(ThemeChange));
                }
            }
        }
    
        public bool IsAdmin { get; private set; }
        private bool exitButtonClicked = false; // Bandera para detectar clic en "Exit"
        public bool exitStatus = false;


        public MainWindow()
        {
            InitializeComponent();
            IsAdmin = Login.IsAdmin;
            ThemeChange = true;
            DataContext = this;
            this.Closing += MainWindow_Closing;
            sidebar.SelectedItem = Taquilla;
            navframe.Navigate(new Uri("/pages/Taquilla.xaml", UriKind.Relative));
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
                    case "Taquilla":
                        Header.PopupText.Text = "Taquilla";
                        Header.PopupText.FontSize = 22;
                        break;
                    case "Upload":
                        Header.PopupText.Text = "Subir Pelicula";
                        Header.PopupText.FontSize = 20;
                        break;
                    case "About":
                        Header.PopupText.Text = "Informacion Aplicacion";
                        Header.PopupText.FontSize = 13;
                        break;
                    case "Exit":
                        Header.PopupText.Text = "Cerrar Sesion";
                        Header.PopupText.FontSize = 20;
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

        private void Moon_Sun_Click(object sender, MouseEventArgs e)
        {
            Trace.WriteLine("Pulsacion");
            ThemeChange = !ThemeChange; // Alterna entre true y false
            var themeUri = ThemeChange
                ? new Uri("Themes/Light.xaml", UriKind.Relative)
                : new Uri("Themes/Dark.xaml", UriKind.Relative);
            ChangeTheme(themeUri);
        }


        private void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = themeuri};

            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);
        }
    }
}
