using System;
using System.Collections.Generic;
using AppCine.dto;
using MySql.Data.MySqlClient;

namespace AppCine
{
    internal class Pelicula
    {
        public int Id { get; set; }  // Opcional, si deseas tener un identificador único para cada película
        public string titulo { get; set; }
        public Sala sala { get; set; }
        public idiomas idioma { get; set; }
        public DateTime data_inici { get; set; }
        public DateTime data_fi { get; set; }
        public TimeSpan hora_inici { get; set; }
        public int duracion { get; set; }
        public List<genres> generos { get; set; } = new List<genres>();

        // Constructor que se puede usar para crear una película desde la base de datos
        public Pelicula(int id, string titulo, Sala sala, idiomas idioma, DateTime data_inici, DateTime data_fi, TimeSpan hora_inici, int duracion, List<genres> generos)
        {
            this.Id = id;  // Si se está utilizando un ID en la base de datos
            this.titulo = titulo;
            this.sala = sala;
            this.idioma = idioma;
            this.data_inici = data_inici;
            this.data_fi = data_fi;
            this.hora_inici = hora_inici;
            this.duracion = duracion;
            this.generos = generos;
        }

        // Constructor alternativo, si no necesitas el Id y quieres omitirlo al cargar desde la base de datos
        public Pelicula(string titulo, Sala sala, idiomas idioma, DateTime data_inici, DateTime data_fi, TimeSpan hora_inici, int duracion, List<genres> generos)
        {
            this.titulo = titulo;
            this.sala = sala;
            this.idioma = idioma;
            this.data_inici = data_inici;
            this.data_fi = data_fi;
            this.hora_inici = hora_inici;
            this.duracion = duracion;
            this.generos = generos;
        }

        // Métodos de formato de fecha y hora
        public string DateFormat()
        {
            return data_fi.ToString("dd/MM/yyyy");
        }

        public string TimeFormat()
        {
            return hora_inici.ToString(@"hh\:mm");
        }

        // Método adicional para cargar géneros de la base de datos si se requiere
        public void CargarGenerosDesdeBaseDeDatos(int peliculaId, MySqlConnection connection)
        {
            string queryGeneros = @"
                SELECT g.nombre
                FROM Generos g
                INNER JOIN Peliculas_Generos pg ON g.id = pg.genero_id
                WHERE pg.pelicula_id = @peliculaId";

            using (MySqlCommand commandGeneros = new MySqlCommand(queryGeneros, connection))
            {
                commandGeneros.Parameters.AddWithValue("@peliculaId", peliculaId);

                using (MySqlDataReader readerGeneros = commandGeneros.ExecuteReader())
                {
                    while (readerGeneros.Read())
                    {
                        string generoNombre = readerGeneros.GetString(0);
                        genres generoEnum = (genres)Enum.Parse(typeof(genres), generoNombre);
                        this.generos.Add(generoEnum);
                    }
                }
            }
        }
    }
}
