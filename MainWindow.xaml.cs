using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppCine.dto;
using MySql.Data.MySqlClient;

namespace AppCine
{
    public partial class MainWindow : Window
    {
        // Lista de películas
        private List<Pelicula> peliculas;
        bool first = true;
        bool subFilter = false;

        public MainWindow()
        {
            InitializeComponent();
            CargarPeliculas();  // Cargamos las películas al iniciar
            CargarFiltros();  // Cargamos los filtros en el ComboBox

        }

        private void CargarPeliculas()
        {
            // Cadena de conexión para MySQL (ajusta estos valores según tu configuración)
            string connectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;Database=appcine;";
            peliculas = new List<Pelicula>();  // Lista donde se almacenarán las películas

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Obtener las películas
                    string query = @"
                        SELECT p.id, p.titulo, p.numero_sala, p.idioma, p.data_inici, p.data_fi, p.hora_inici, p.duracion, p.generos
                        FROM Pelicula p
                        WHERE p.data_inici <= @fechaActual AND p.data_inici >= @fechaActual";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@fechaActual", DateTime.Now);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Crear la película a partir de los resultados de la consulta
                                int id = reader.GetInt32(0);
                                string titulo = reader.GetString(1);
                                int salaId = reader.GetInt32(2);
                                string idioma = reader.GetString(3);
                                DateTime fechaInicio = reader.GetDateTime(4);
                                DateTime fechaFin = reader.GetDateTime(5);
                                TimeSpan horaInicio = reader.GetTimeSpan(6);
                                int duracion = reader.GetInt32(7);

                                // Crear la Sala y Pelicula
                                Sala sala = new Sala(salaId);  // Ajusta la creación de la sala según cómo la tengas en la base de datos
                                idiomas idiomaEnum = (idiomas)Enum.Parse(typeof(idiomas), idioma);

                                Pelicula pelicula = new Pelicula(titulo, sala, idiomaEnum, fechaInicio, fechaFin, horaInicio, duracion, new List<genres>());
                                peliculas.Add(pelicula);
                            }
                        }
                    }

                    // Obtener los géneros asociados a cada película
                    foreach (var pelicula in peliculas)
                    {
                        string queryGeneros = @"
                            SELECT g.nombre
                            FROM Generos g
                            INNER JOIN Peliculas_Generos pg ON g.id = pg.genero_id
                            WHERE pg.pelicula_id = @peliculaId";

                        using (MySqlCommand commandGeneros = new MySqlCommand(queryGeneros, connection))
                        {
                            commandGeneros.Parameters.AddWithValue("@peliculaId", pelicula.Id); // Asume que tienes una propiedad Id en la película

                            using (MySqlDataReader readerGeneros = commandGeneros.ExecuteReader())
                            {
                                while (readerGeneros.Read())
                                {
                                    string generoNombre = readerGeneros.GetString(0);
                                    genres generoEnum = (genres)Enum.Parse(typeof(genres), generoNombre);
                                    pelicula.generos.Add(generoEnum);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las películas: " + ex.Message);
            }

            // Filtramos las películas que están en el rango de fechas actual
            peliculas = peliculas.Where(p => p.data_inici <= DateTime.Now && p.data_fi >= DateTime.Now).ToList();
        }

    private void CargarGeneros()
        {
            // Añadimos los géneros al ComboBox
            subFilterBox.Items.Add("-");
            subFilterBox.Items.Add(genres.Accio);
            subFilterBox.Items.Add(genres.Aventura);
            subFilterBox.Items.Add(genres.Ciencia_Ficcio);
            subFilterBox.Items.Add(genres.Comedia);
            subFilterBox.Items.Add(genres.Documental);
            subFilterBox.Items.Add(genres.Drama);
            subFilterBox.Items.Add(genres.Fantasia);
            subFilterBox.Items.Add(genres.Musical);
            subFilterBox.Items.Add(genres.Suspense);
            subFilterBox.Items.Add(genres.Terror);

            subFilterBox.SelectedIndex = 0;  // Seleccionamos un género por defecto
        }

        private void CargarIdiomas()
        {
            // Añadimos los idiomas al ComboBox
            subFilterBox.Items.Add("-");
            subFilterBox.Items.Add(idiomas.Ingles);
            subFilterBox.Items.Add(idiomas.Castellano);
            subFilterBox.Items.Add(idiomas.Catalan);
            subFilterBox.SelectedIndex = 0;  // Seleccionamos un idioma por defecto
        }

        private void CargarFiltros()
        {
            // Añadimos los filtros al ComboBox
            filterBox.Items.Add("-");
            filterBox.Items.Add("Genero");
            filterBox.Items.Add("Fecha");
            filterBox.Items.Add("Idioma");
            filterBox.SelectedIndex = 0;  // Seleccionamos un género por defecto
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoSeleccionado = filterBox.SelectedItem.ToString();
            if (!first)
            {
                subFilter = false;
                subFilterBox.Items.Clear();
                list_peliculas.ItemsSource = peliculas;

            }
            if (tipoSeleccionado == "-")
            {
                if (first)
                {
                    list_peliculas.Items.Clear();
                    first = false;
                }
                list_peliculas.ItemsSource = peliculas;
                subFilter = false;
                subFilterBox.Visibility = Visibility.Hidden;
                dataPicker.Visibility = Visibility.Hidden;
            }
            else if (tipoSeleccionado == "Genero")
            {
                CargarGeneros();
                subFilter = true;
                subFilterBox.Visibility = Visibility.Visible;
                dataPicker.Visibility = Visibility.Hidden;
            }
            else if (tipoSeleccionado == "Fecha")
            {
                subFilter = false;
                subFilterBox.Visibility = Visibility.Hidden;
                dataPicker.Visibility = Visibility.Visible;
            }
            else if (tipoSeleccionado == "Idioma")
            {
                CargarIdiomas();
                subFilter = true;
                subFilterBox.Visibility = Visibility.Visible;
                dataPicker.Visibility = Visibility.Hidden;
            }

        }

        private void subFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (subFilter)
            {
                string subTipoSeleccionado = subFilterBox.SelectedItem.ToString();
                string tipoSeleccionado = filterBox.SelectedItem.ToString();
                if (subTipoSeleccionado == "-" || subTipoSeleccionado == null)
                {
                    if (first)
                    {
                        list_peliculas.Items.Clear();
                        first = false;
                    }
                    list_peliculas.ItemsSource = peliculas;
                }
                else if (tipoSeleccionado == "Idioma")
                {
                    //mostrar solo las peliculas que esten en ese idioma
                    var peliculasFiltradas = peliculas.Where(p => p.idioma.ToString() == subTipoSeleccionado).ToList();
                    list_peliculas.ItemsSource = peliculasFiltradas;
                }
                else
                {
                    var peliculasFiltradas = peliculas.Where(p => p.generos.Any(g => g.ToString() == subTipoSeleccionado)).ToList();
                    list_peliculas.ItemsSource = peliculasFiltradas;
                }
            }
        }
        private void dataPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime fechaSeleccionada = dataPicker.SelectedDate.Value;
            //mostrar solo las peliculas que se proyecten en esa fecha  rango inicio y fin
            var peliculasFiltradas = peliculas.Where(p => p.data_inici <= fechaSeleccionada && p.data_fi >= fechaSeleccionada).ToList();
            list_peliculas.ItemsSource = peliculasFiltradas;

        }
    }
}

