using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using AppCine;
using AppCine.dto;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace SideBar_Nav.Pages
{
    public partial class Upload_File : Window
    {
        //public List<Pelicula> Peliculas { get; set; }  // Propiedad que se enlaza en la UI

        public Upload_File()
        {
            InitializeComponent();
            //Pelicula Peliculas = new List<Pelicula>();  // Inicializa la lista de películas
            DataContext = this;  // Establece el contexto de datos para los bindings
        }

        // Evento al hacer clic en el botón "Examinar" para abrir el explorador de archivos
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Crear un nuevo diálogo para seleccionar un archivo
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*"; // Filtro para archivos de texto

            // Mostrar el cuadro de diálogo y verificar si el usuario seleccionó un archivo
            if (openFileDialog.ShowDialog() == true)
            {
                // Establecer la ruta del archivo seleccionado en el TextBox
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        // Evento al hacer clic en el botón "Cargar Archivo" para leer el archivo
        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (File.Exists(filePath))
            {
                // Limpiar la lista de películas antes de agregar las nuevas
                //Peliculas.Clear();

                // Leer el archivo línea por línea
                foreach (var line in File.ReadLines(filePath))
                {
                    string[] parts = line.Split(';');

                    for (int i = 0; i < parts.Length; i++)
                    {
                        parts[i] = parts[i].Trim();
                    }

                    string titulo = parts[0];
                    int sala = int.Parse(parts[1]);
                    string idioma = parts[2];
                    DateTime data_inici = DateTime.ParseExact(parts[3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime data_fi = DateTime.ParseExact(parts[4], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    TimeSpan hora_inici = TimeSpan.ParseExact(parts[5], "hh\\:mm", CultureInfo.InvariantCulture);
                    int duracion = int.Parse(parts[6]);

                    List<string> generos = new List<string>();
                    for (int i = 7; i < parts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(parts[i]))
                        {
                            generos.Add(parts[i]);
                        }
                    }
                    
                    string generos_string = string.Join(",", generos);

                    string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine;";

                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            string query = "INSERT INTO pelicula (titulo, numero_sala, idioma, data_inici, data_fi, hora_inici, duracion, generos) " +
                                   "VALUES (@titulo, @numero_sala, @idioma, @data_inici, @data_fi, @hora_inici, @duracion, @generos)";
                            connection.Open();
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                // Parámetros de la consulta
                                command.Parameters.AddWithValue("@titulo", titulo);
                                command.Parameters.AddWithValue("@numero_sala", sala);
                                command.Parameters.AddWithValue("@idioma", idioma);
                                command.Parameters.AddWithValue("@data_inici", data_inici);
                                command.Parameters.AddWithValue("@data_fi", data_fi);
                                command.Parameters.AddWithValue("@hora_inici", hora_inici); 
                                command.Parameters.AddWithValue("@duracion", duracion);
                                command.Parameters.AddWithValue("@generos", generos_string);

                                // Ejecución
                                command.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Película añadida correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al añadir la película: " + ex.Message);
                    }
            }

                    //Pelicula movie = new Pelicula(titulo, sala, idioma, data_inici, data_fi, hora_inici, duracion, generos);
                    //Peliculas.Add(movie);

                // Mostrar los datos en un MessageBox para verificación
                StringBuilder movieData = new StringBuilder();
                /*foreach (var movie in Peliculas)
                {
                    movieData.AppendLine($"Título: {movie.titulo}");
                    movieData.AppendLine($"Sala: {movie.Sala}");
                    movieData.AppendLine($"Idioma: {movie.Idioma}");
                    movieData.AppendLine($"Fecha Inicio: {movie.Data_inici.ToShortDateString()}");
                    movieData.AppendLine($"Fecha Fin: {movie.Data_fi.ToShortDateString()}");
                    movieData.AppendLine($"Hora de Inicio: {movie.Hora_inici}");
                    movieData.AppendLine($"Duración: {movie.Duracion} minutos");
                    movieData.AppendLine($"Géneros: {movie.GenerosString}");
                    movieData.AppendLine(new string('-', 20));
                }*/

                // Mostrar el MessageBox con los datos
                MessageBox.Show(movieData.ToString(), "Datos Cargados");
            }
            else
            {
                MessageBox.Show("El archivo no se encuentra en la ruta especificada.");
            }
        }

    }
}
