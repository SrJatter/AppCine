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
    public partial class Login : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _authMode;
        public bool AuthMode
        {
            get => _authMode;
            set
            {
                if (_authMode != value)
                {
                    _authMode = value;
                    OnPropertyChanged(nameof(AuthMode));
                }
            }
        }
        public int failedAttempts = 0; // Contador para los intentos fallidos de login
        public bool cancelStatus = false; // Status del boton de cancelamiento
        public static bool IsAdmin { get; private set; }

        public Login()
        {
            InitializeComponent();
            this.Closing += Login_Closing;
            AuthMode = true;
            DataContext = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = Username.Text;
            string password = Password.Password;
            if (AuthMode)
            {
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
            }else{
                // Registro de nuevo usuario
                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                {
                    MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (string.IsNullOrWhiteSpace(password) || password.Length < 3)
                {
                    MessageBox.Show("La contraseña debe tener al menos 3 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (await ExisteUsuario(email))
                {
                    MessageBox.Show("Este correo ya está registrado. Por favor, utilice otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    bool registroExitoso = await RegistrarUsuario(email, password);
                    if (registroExitoso)
                    {
                        IsAdmin = false;
                        MessageBox.Show("Registro exitoso.\nIniciando sesion...", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Visibility = Visibility.Hidden; // Oculta la ventana de login
                        failedAttempts = 0; // Reinicia los intentos fallidos al loguearse correctamente
                    }
                    else
                    {
                        MessageBox.Show("Ocurrió un error al registrar al usuario. Intente nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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

        private async Task<bool> ExisteUsuario(string email)
        {
            bool existe = false;

            // Cadena de conexión (reemplazar con tu propia cadena de conexión)
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine";

            // Consulta SQL para verificar si el correo ya existe
            string query = "SELECT COUNT(1) FROM Usuarios WHERE email = @Email";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                try
                {
                    await connection.OpenAsync();
                    int result = Convert.ToInt32(await command.ExecuteScalarAsync());
                    existe = result > 0; // Si el resultado es mayor que 0, el correo ya existe
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al verificar el usuario en la base de datos: " + ex.Message);
                }
            }

            return existe;
        }

        private async Task<bool> RegistrarUsuario(string email, string password)
        {
            bool registrado = false;

            // Cadena de conexión (reemplazar con tu propia cadena de conexión)
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine";

            // Consulta SQL para insertar un nuevo usuario
            string query = "INSERT INTO Usuarios (email, password) VALUES (@Email, @Password)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Asegúrate de encriptar las contraseñas

                try
                {
                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    registrado = rowsAffected > 0; // Si se insertó una fila, el registro fue exitoso
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar el usuario en la base de datos: " + ex.Message);
                }
            }

            return registrado;
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
            if (AuthMode) AuthMode = false;
            else AuthMode = true;
            
        }

        private void Login_Closing(object sender, CancelEventArgs e)
        {
            Trace.WriteLine("Cierre de ventana con Alt+F4 o con boton cancel");
            cancelStatus = true;
        }
    }
}
