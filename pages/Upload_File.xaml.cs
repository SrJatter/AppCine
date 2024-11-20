using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Transactions;
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
            int Veces = 0;

            if (File.Exists(filePath))
            {
                StringBuilder movieData = new StringBuilder();

                // Leer el archivo línea por línea
                foreach (var line in File.ReadLines(filePath))
                {
                    try
                    {
                        string[] parts = line.Split(';');

                        // Validar y limpiar datos
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

                        string query = "INSERT INTO pelicula (titulo, numero_sala, idioma, data_inici, data_fi, hora_inici, duracion, generos) " +
                                       "VALUES (@titulo, @numero_sala, @idioma, @data_inici, @data_fi, @hora_inici, @duracion, @generos)";

                        string query2 = "INSERT INTO asientos (id) VALUES (LAST_INSERT_ID());";

                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();

                            using (MySqlTransaction transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                                    {
                                        command.Parameters.AddWithValue("@titulo", titulo);
                                        command.Parameters.AddWithValue("@numero_sala", sala);
                                        command.Parameters.AddWithValue("@idioma", idioma);
                                        command.Parameters.AddWithValue("@data_inici", data_inici);
                                        command.Parameters.AddWithValue("@data_fi", data_fi);
                                        command.Parameters.AddWithValue("@hora_inici", hora_inici);
                                        command.Parameters.AddWithValue("@duracion", duracion);
                                        command.Parameters.AddWithValue("@generos", generos_string);
                                        command.ExecuteNonQuery();
                                    }

                                    using (MySqlCommand command2 = new MySqlCommand(query2, connection, transaction))
                                    {
                                        command2.ExecuteNonQuery();
                                    }

                                    transaction.Commit();

                                    Veces++;
                                }
                                catch
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }

                        // Agregar información de la película al StringBuilder
                        movieData.AppendLine($"Título: {titulo}");
                        movieData.AppendLine($"Sala: {sala}");
                        movieData.AppendLine($"Idioma: {idioma}");
                        movieData.AppendLine($"Fecha Inicio: {data_inici.ToShortDateString()}");
                        movieData.AppendLine($"Fecha Fin: {data_fi.ToShortDateString()}");
                        movieData.AppendLine($"Hora de Inicio: {hora_inici}");
                        movieData.AppendLine($"Duración: {duracion} minutos");
                        movieData.AppendLine($"Géneros: {generos_string}");
                        movieData.AppendLine(new string('-', 20));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al añadir la película: " + ex.Message);
                    }
                }
                if (Veces >= 6) { MessageBox.Show($"Películas añadidas correctamente."); }
                else if (Veces == 1)
                {
                    MessageBox.Show(movieData.ToString(), "Datos Cargados");
                    MessageBox.Show($"Película añadida correctamente.");
                }else
                {
                    MessageBox.Show(movieData.ToString(), "Datos Cargados");
                    MessageBox.Show($"Películas añadidas correctamente.");
                }
            }
            else
            {
                MessageBox.Show("El archivo no se encuentra en la ruta especificada.");
            }
        }


    }
}
