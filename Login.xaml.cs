using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace AppCine
{
    public partial class Login : Window
    {
        public int failedAttempts = 0; // Contador para los intentos fallidos de login

        public Login()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = Username.Text;
            string password = Password.Password;

            // Validar email no vacío y en formato correcto
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                failedAttempts++;
            }
            // Validar que la contraseña tenga al menos 3 caracteres
            else if (string.IsNullOrWhiteSpace(password) || password.Length < 3)
            {
                MessageBox.Show("La contraseña debe tener al menos 3 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                failedAttempts++;
            }
            // Validar credenciales de usuario
            else if (email == "admin@admin.com" && password == "admin")
            {
                MessageBox.Show("Login exitoso");
                this.Visibility = Visibility.Hidden; // Oculta la ventana de login
                failedAttempts = 0; // Reinicia los intentos fallidos al loguearse correctamente
            }
            else
            {
                MessageBox.Show("Login exitoso");
                this.Visibility = Visibility.Hidden; // Oculta la ventana de login
                failedAttempts = 0;
            }

            // Cerrar aplicación después de tres intentos fallidos
            if (failedAttempts >= 3)
            {
                MessageBox.Show("Demasiados intentos fallidos. La aplicación se cerrará.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
        }

        // Método para validar el formato de email usando regex
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private void Username_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (Username.Text != "E-Mail")
            {
                Username.Foreground = Brushes.DimGray; // Cambia el color para el texto real del usuario
            }
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Username.Text == "E-Mail")
            {
                Username.Text = "";
                Username.Foreground = Brushes.DimGray; // Cambia el color para que sea más legible
            }
        }

        private void Username_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username.Text))
            {
                Username.Text = "E-Mail";
                Username.Foreground = Brushes.DimGray; // Vuelve a DimGray para el marcador de posición
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Password == "Contraseña")
            {
                Password.Password = "";
                Password.Foreground = Brushes.DimGray; // Cambia el color para que sea más legible
            }
        }
        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username.Text))
            {
                Password.Password = "Contraseña";
                Password.Foreground = Brushes.DimGray; // Vuelve a DimGray para el marcador de posición
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Cierra la ventana de login
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}