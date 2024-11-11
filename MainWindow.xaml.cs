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
            // Creamos una lista de 10 películas
            peliculas = new List<Pelicula> {
                    new Pelicula("Oppenheimer", new Sala(1), idiomas.Ingles, new DateTime(2024, 10, 15), new DateTime(2024, 12, 15), new TimeSpan(19, 0, 0), 180, new List<genres> { genres.Drama, genres.Documental, genres.Fantasia }),
                    new Pelicula("Barbie", new Sala(2), idiomas.Ingles, new DateTime(2024, 10, 10), new DateTime(2024, 12, 10), new TimeSpan(21, 0, 0), 114, new List<genres> { genres.Comedia, genres.Fantasia }),
                    new Pelicula("Spider-Man: Across the Spider-Verse", new Sala(5), idiomas.Castellano, new DateTime(2024, 11, 01), new DateTime(2024, 12, 01), new TimeSpan(18, 30, 0), 140, new List<genres> { genres.Musical, genres.Aventura, genres.Accio }),
                    new Pelicula("The Exorcist: Believer", new Sala(4), idiomas.Ingles, new DateTime(2024, 10, 05), new DateTime(2024, 12, 05), new TimeSpan(22, 0, 0), 111, new List<genres> { genres.Terror, genres.Suspense }),
                    new Pelicula("Killers of the Flower Moon", new Sala(3), idiomas.Ingles, new DateTime(2024, 10, 20), new DateTime(2024, 12, 20), new TimeSpan(19, 30, 0), 206, new List<genres> { genres.Ciencia_Ficcio, genres.Comedia })
                };
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

