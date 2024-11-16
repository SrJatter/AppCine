using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace AppCine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsAdmin { get; set; } = true;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = sidebar.SelectedItem as NavButton;

            navframe.Navigate(selected.Navlink);

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

    }
}
  