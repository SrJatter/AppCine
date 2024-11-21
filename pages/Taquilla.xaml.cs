using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AppCine;
using AppCine.dto;
using MySql.Data.MySqlClient;

namespace SideBar_Nav.Pages
{
    public partial class Taquilla : Page
    {
        private List<Pelicula> peliculas;
        bool first = true;
        bool subFilter = false;

        public Taquilla()
        {
            InitializeComponent();
            CargarPeliculas();  // Cargamos las películas al iniciar
            CargarFiltros();  // Cargamos los filtros en el ComboBox
            CargarEstadosAsientos();
        }

        private void CargarPeliculas()
        {
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine;";
            peliculas = new List<Pelicula>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Consulta SQL para cargar las películas
                    string query = @"
                SELECT p.id, p.titulo, p.numero_sala, p.idioma, p.data_inici, p.data_fi, p.hora_inici, p.duracion, p.generos
                FROM Pelicula p
                WHERE p.data_inici <= @fechaActual AND p.data_fi >= @fechaActual";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@fechaActual", DateTime.Now);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string titulo = reader.GetString(1);
                                int salaId = reader.GetInt32(2);
                                string idioma = reader.GetString(3);
                                DateTime fechaInicio = reader.GetDateTime(4);
                                DateTime fechaFin = reader.GetDateTime(5);
                                TimeSpan horaInicio = reader.GetTimeSpan(6);
                                int duracion = reader.GetInt32(7);
                                string generosTexto = reader.GetString(8); // Campo `p.generos`

                                // Convertir el texto de géneros a la lista de enums
                                List<genres> generosLista = generosTexto
                                    .Split(',') // Separar por coma
                                    .Select(g => (genres)Enum.Parse(typeof(genres), g.Trim())) // Convertir a enum
                                    .ToList();

                                Sala sala = new Sala(salaId);
                                idiomas idiomaEnum = (idiomas)Enum.Parse(typeof(idiomas), idioma);

                                Pelicula pelicula = new Pelicula(titulo, sala, idiomaEnum, fechaInicio, fechaFin, horaInicio, duracion, generosLista);
                                peliculas.Add(pelicula);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las películas: " + ex.Message);
            }

            peliculas = peliculas.Where(p => p.data_inici <= DateTime.Now && p.data_fi >= DateTime.Now).ToList();
            list_peliculas.Items.Clear();
            list_peliculas.ItemsSource = peliculas;
        }

        private void CargarGeneros()
        {
            subFilterBox.Items.Clear();
            subFilterBox.Items.Add("-");
            foreach (var genero in Enum.GetValues(typeof(genres)))
            {
                subFilterBox.Items.Add(genero);
            }
            subFilterBox.SelectedIndex = 0;
        }

        private void CargarIdiomas()
        {
            subFilterBox.Items.Clear();
            subFilterBox.Items.Add("-");
            foreach (var idioma in Enum.GetValues(typeof(idiomas)))
            {
                subFilterBox.Items.Add(idioma);
            }
            subFilterBox.SelectedIndex = 0;
        }

        private void CargarFiltros()
        {
            filterBox.Items.Add("-");
            filterBox.Items.Add("Genero");
            filterBox.Items.Add("Fecha");
            filterBox.Items.Add("Idioma");
            filterBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!first)
            {
                subFilter = false;
                subFilterBox.Items.Clear();
                list_peliculas.ItemsSource = peliculas;
            }

            string tipoSeleccionado = filterBox.SelectedItem.ToString();
            switch (tipoSeleccionado)
            {
                case "-":
                    list_peliculas.ItemsSource = peliculas;
                    subFilterBox.Visibility = Visibility.Hidden;
                    dataPicker.Visibility = Visibility.Hidden;
                    break;
                case "Genero":
                    CargarGeneros();
                    subFilterBox.Visibility = Visibility.Visible;
                    dataPicker.Visibility = Visibility.Hidden;
                    subFilter = true;
                    break;
                case "Fecha":
                    subFilterBox.Visibility = Visibility.Hidden;
                    dataPicker.Visibility = Visibility.Visible;
                    subFilter = false;
                    break;
                case "Idioma":
                    CargarIdiomas();
                    subFilterBox.Visibility = Visibility.Visible;
                    dataPicker.Visibility = Visibility.Hidden;
                    subFilter = true;
                    break;
            }
            first = false;
        }

        private void subFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (subFilter)
            {
                string subTipoSeleccionado = subFilterBox.SelectedItem.ToString();
                string tipoSeleccionado = filterBox.SelectedItem.ToString();

                if (subTipoSeleccionado == "-")
                {
                    list_peliculas.ItemsSource = peliculas;
                }
                else if (tipoSeleccionado == "Idioma")
                {
                    var peliculasFiltradas = peliculas.Where(p => p.idioma.ToString() == subTipoSeleccionado).ToList();
                    list_peliculas.ItemsSource = peliculasFiltradas;
                }
                else if (tipoSeleccionado == "Genero")
                {
                    var peliculasFiltradas = peliculas.Where(p => p.generos.Any(g => g.ToString() == subTipoSeleccionado)).ToList();
                    list_peliculas.ItemsSource = peliculasFiltradas;
                }
            }
        }

        private void dataPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataPicker.SelectedDate.HasValue)
            {
                DateTime fechaSeleccionada = dataPicker.SelectedDate.Value;
                var peliculasFiltradas = peliculas.Where(p => p.data_inici <= fechaSeleccionada && p.data_fi >= fechaSeleccionada).ToList();
                list_peliculas.ItemsSource = peliculasFiltradas;
            }
        }

        private void CargarEstadosAsientos()
        {
            
        }
        private void list_peliculas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list_peliculas.SelectedItem is Pelicula peliculaSeleccionada)
            {
                CargarAsientosPorPelicula(peliculaSeleccionada.Id); // Llama al método para cargar asientos
            }
        }
        private void CargarAsientosPorPelicula(int peliculaId)
        {
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM asientos WHERE id = @peliculaId"; // Asume que esta tabla tiene columnas con nombres numéricos.
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@peliculaId", peliculaId);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 1; i <= 9; i++) // Recorre las columnas del 1 al 9 (asume que hay hasta asiento_9).
                                {
                                    string asientoKey = $"asiento_{i}"; // Nombre del rectángulo en el XAML.
                                    int estado = reader.GetInt32(i); // Obtiene el valor de la columna numérica.

                                    // Busca el rectángulo en el XAML.
                                    Rectangle rect = (Rectangle)this.FindName(asientoKey);
                                    if (rect != null)
                                    {
                                        // Cambia el color según el estado (1 = ocupado, 0 = libre).
                                        rect.Fill = estado == 1 ? Brushes.Red : Brushes.Green;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los estados de los asientos: " + ex.Message);
            }
        }

        private void reserveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TaquillaViewModel viewModel)
            {
                // Filtrar los asientos seleccionados
                var asientosSeleccionados = viewModel.Asientos.Where(a => a.IsSelected).ToList();

                if (asientosSeleccionados.Count == 0)
                {
                    MessageBox.Show("Por favor, selecciona al menos un asiento para reservar.");
                    return;
                }

                string asientos = string.Join(", ", asientosSeleccionados.Select(a => a.Id));
                MessageBox.Show($"Asientos reservados: {asientos}");
                foreach (var asiento in asientosSeleccionados)
                {
                    asiento.IsSelected = false; // Resetear la selección (simula reserva finalizada)
                    GuardarReservaEnBaseDeDatos(asiento.Id);
                }
                CargarEstadosAsientos();
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Asiento asiento)
            {
                asiento.IsSelected = !asiento.IsSelected;
            }
        }
        private void GuardarReservaEnBaseDeDatos(int asientoId)
        {
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE asientos SET @asientoId = 1 WHERE id = pelicula"; // Cambia el estado a "reservado"
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@asientoId", asientoId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la reserva en la base de datos: " + ex.Message);
            }
        }

    }
}




        