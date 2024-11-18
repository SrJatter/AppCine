using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;

namespace AppCine
{
    public partial class Login : Window
    {
        public bool authMode = true; // Modo de autenticación
        public int failedAttempts = 0; // Contador para los intentos fallidos de login
        public bool cancelStatus = false; // Status del boton de cancelamiento
        public static bool IsAdmin { get; private set; }

        public Login()
        {
            InitializeComponent();
            this.Closing += Login_Closing;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = Username.Text;
            string password = Password.Password;

            // Validar email no vacío y en formato correcto
            if (email == "a" && password == "a") // !!!!bypass borrar cuando se tenga que entregar!!!!!
            {
                IsAdmin = true;
                this.Visibility = Visibility.Hidden; // Oculta la ventana de login
                failedAttempts = 0; // Reinicia los intentos fallidos al loguearse correctamente
            }
            else if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
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
            // Validar credenciales de usuario con base de datos
            else if (await AutenticarUsuario(email, password))
            {
                if (email != "admin@admin.com")
                {
                    MessageBox.Show("Login exitoso");
                    IsAdmin = false;
                }
                else
                {
                    MessageBox.Show("Login Administrativo exitoso");
                    IsAdmin = true;
                }
                this.Visibility = Visibility.Hidden; // Oculta la ventana de login
                failedAttempts = 0; // Reinicia los intentos fallidos al loguearse correctamente
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                failedAttempts++;
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

        // Método para autenticar usuario a través de la base de datos
        private async Task<bool> AutenticarUsuario(string email, string password)
        {
            bool isAuthenticated = false;

            // Cadena de conexión (reemplazar con tu propia cadena de conexión)
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine";

            // Consulta SQL para verificar las credenciales
            string query = "SELECT COUNT(1) FROM Usuarios WHERE email = @Email AND password = @Password";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Asegúrate de almacenar las contraseñas de manera segura (encriptadas)

                try
                {
                    await connection.OpenAsync();
                    int result = Convert.ToInt32(await command.ExecuteScalarAsync());
                    isAuthenticated = result == 1; // Si se encuentra una coincidencia, es un usuario válido
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar con la base de datos: " + ex.Message);
                }
            }

            return isAuthenticated;
        }

        // Métodos para manejar el marcador de posición en el campo de texto del correo electrónico
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

        // Métodos para manejar el marcador de posición en el campo de texto de la contraseña
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
            if (string.IsNullOrWhiteSpace(Password.Password))
            {
                Password.Password = "Contraseña";
                Password.Foreground = Brushes.DimGray; // Vuelve a DimGray para el marcador de posición
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (authMode) authMode = false;
            else authMode = true;
            // Código para otro botón opcional
        }

        private void Login_Closing(object sender, CancelEventArgs e)
        {
            Trace.WriteLine("Cierre de ventana con Alt+F4 o con boton cancel");
            cancelStatus = true;
        }
    }
}
