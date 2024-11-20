using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Upload_Widgets : Window
    {
        public Upload_Widgets()
        {
            InitializeComponent();
        }
        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            // Leer los valores de los controles
            string titulo = TitleTextBox.Text;
            string sala = (SalaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string idioma = (IdiomaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string duracion = DuracionTextBox.Text;

            DateTime? fechaInicio = FechaInicioPicker.SelectedDate;
            DateTime? fechaFin = FechaFinPicker.SelectedDate;

            string horaInicio = $"{HoraInicioTextBox_Hora.Text}:{HoraInicioTextBox_Minutos.Text}";

            string genero1 = (Genero1ComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string genero2 = (Genero2ComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string genero3 = (Genero3ComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            bool genero1Seleccionado = Genero1ComboBox.SelectedIndex >= 0;
            bool genero2Seleccionado = Genero2ComboBox.SelectedIndex >= 0;
            bool genero3Seleccionado = Genero3ComboBox.SelectedIndex >= 0;
            bool generos = false;

            if (genero1Seleccionado == false && genero2Seleccionado == false && genero3Seleccionado == false) { generos = false; }
            else { generos = true; }

            // Validar datos
            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(sala) ||
                string.IsNullOrWhiteSpace(idioma) || string.IsNullOrWhiteSpace(duracion) ||
                fechaInicio == null || fechaFin == null || string.IsNullOrWhiteSpace(horaInicio) ||
                generos == false)
            {
                MessageBox.Show("Por favor, completa todos los campos obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (genero1 == "-") { genero1 = ""; }
            if (genero2 == "-") { genero2 = ""; }
            if (genero3 == "-") { genero3 = ""; }

            List<string> generosValidos = new List<string>();

            if (!string.IsNullOrWhiteSpace(genero1)) generosValidos.Add(genero1);
            if (!string.IsNullOrWhiteSpace(genero2)) generosValidos.Add(genero2);
            if (!string.IsNullOrWhiteSpace(genero3)) generosValidos.Add(genero3);

            string generos_string = string.Join(",", generosValidos);

            // Procesar la información
            string mensaje = $"Título: {titulo}\n" +
                                $"Sala: {sala}\nIdioma: {idioma}\nDuración: {duracion} min\n" +
                                $"Fecha Inicio: {fechaInicio.Value.ToShortDateString()}\n" +
                                $"Fecha Fin: {fechaFin.Value.ToShortDateString()}\n" +
                                $"Hora Inicio: {horaInicio}\n" +
                                $"Género 1: {genero1}\nGénero 2: {genero2}\nGénero 3: {genero3}";

            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "INSERT INTO pelicula (titulo, numero_sala, idioma, data_inici, data_fi, hora_inici, duracion, generos) " +
                           "VALUES (@titulo, @numero_sala, @idioma, @data_inici, @data_fi, @hora_inici, @duracion, @generos)";

                    string query2 = "INSERT INTO asientos (id) VALUES (LAST_INSERT_ID());";

                    connection.Open();
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                // Parámetros de la consulta
                                command.Parameters.AddWithValue("@titulo", titulo);
                                command.Parameters.AddWithValue("@numero_sala", sala);
                                command.Parameters.AddWithValue("@idioma", idioma);
                                command.Parameters.AddWithValue("@data_inici", fechaInicio.Value);
                                command.Parameters.AddWithValue("@data_fi", fechaFin.Value);
                                command.Parameters.AddWithValue("@hora_inici", "00:00"); // Hora fija por defecto
                                command.Parameters.AddWithValue("@duracion", duracion);
                                command.Parameters.AddWithValue("@generos", generos_string); // Aquí podrías añadir géneros reales si tienes un campo para eso

                                // Ejecución
                                command.ExecuteNonQuery();
                            }
                            using (MySqlCommand command2 = new MySqlCommand(query2, connection, transaction))
                            {
                                command2.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al añadir la película: " + ex.Message);
            }
            MessageBox.Show("Película añadida correctamente.");
            MessageBox.Show(mensaje, "Datos Subidos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        // Validar entrada para horas (00-23)
        private void ValidateHourInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
        }

        // Ajustar valor de horas (00-23)
        private void FixHourInput(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    // Si el valor está fuera del rango, ajustarlo
                    if (value < 0) value = 0;
                    if (value > 23) value = 23;
                    textBox.Text = value.ToString("D2"); // Formato de dos dígitos
                }
                else
                {
                    textBox.Text = "00"; // Valor predeterminado
                }
            }
        }

        // Validar entrada para minutos (00-59)
        private void ValidateMinuteInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
        }

        // Ajustar valor de minutos (00-59)
        private void FixMinuteInput(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    // Si el valor está fuera del rango, ajustarlo
                    if (value < 0) value = 0;
                    if (value > 59) value = 59;
                    textBox.Text = value.ToString("D2"); // Formato de dos dígitos
                }
                else
                {
                    textBox.Text = "00"; // Valor predeterminado
                }
            }
        }
    }
}