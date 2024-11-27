using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SideBar_Nav.Pages
{
    public partial class About : Page
    {
        private string urli = "";
        private string desc = "";
        public About()
        {
            InitializeComponent();
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Obtener información de la imagen seleccionada
            var image = sender as Image;
            if (image != null)
            {
                PhotoName.Text = image.Tag.ToString();
                if (PhotoName.Text == "Hector Apolo Andrade") { urli = "heaploan"; desc = "Creador e Implementador de Login y Registrarse"; }
                else if (PhotoName.Text == "David Moldovan") { urli = "SrJatter"; desc = "Programador del backend, pagina de Reservas y co-sillas"; }
                else if (PhotoName.Text == "Javier Merlo") { urli = "PirataArcade"; desc = "Creador de Cerrar sesion, Info, co-sillas y extras del menu"; }

                PhotoHyperlink.NavigateUri = new System.Uri("https://github.com/" + urli);
                PhotoDesc.Text = desc;

                // Mostrar el menú deslizable
                SlideMenu.Visibility = Visibility.Visible;
                var showStoryboard = (Storyboard)FindResource("ShowMenuAnimation");
                showStoryboard.Begin(this);
            }
        }

        private void CloseMenu_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar el menú deslizable
            var hideStoryboard = (Storyboard)FindResource("HideMenuAnimation");
            hideStoryboard.Completed += (s, ev) => SlideMenu.Visibility = Visibility.Collapsed;
            hideStoryboard.Begin(this);
        }

        private void PhotoHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Abrir enlace en el navegador predeterminado
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
