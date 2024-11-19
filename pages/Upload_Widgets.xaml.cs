using System;
using System.Collections.Generic;
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

            // Procesar la información
            string mensaje = $"Título: {titulo}\n" +
                                $"Sala: {sala}\nIdioma: {idioma}\nDuración: {duracion} min\n" +
                                $"Fecha Inicio: {fechaInicio.Value.ToShortDateString()}\n" +
                                $"Fecha Fin: {fechaFin.Value.ToShortDateString()}\n" +
                                $"Hora Inicio: {horaInicio}\n" +
                                $"Género 1: {genero1}\nGénero 2: {genero2}\nGénero 3: {genero3}";

            MessageBox.Show(mensaje, "Datos Subidos", MessageBoxButton.OK, MessageBoxImage.Information);

            // Aquí puedes añadir lógica para guardar en base de datos, enviar a un servidor, etc.
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