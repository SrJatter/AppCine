using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCine.dto
{
    internal class Pelicula
    {
        public string titulo { get; set; }
        public Sala sala { get; set; }
        public idiomas idioma { get; set; }
        public DateTime data_inici { get; set; }
        public DateTime data_fi { get; set; }
        public TimeSpan hora_inici { get; set; }
        public int duracion { get; set; }
        public List<genres> generos { get; set; } = new List<genres>();
        

        public Pelicula (String titulo , Sala sala, idiomas idioma, DateTime data_inici, DateTime data_fi, TimeSpan hora_inici, int duracion, List<genres> genres)
        {
            this.titulo = titulo;
            this.sala = sala;
            this.idioma = idioma;
            this.data_inici = data_inici;
            this.data_fi = data_fi;
            this.hora_inici = hora_inici;
            this.duracion = duracion;
            this.generos = genres;
        }
        public string DateFormat()
        {
            return data_fi.ToString("dd/MM/yyyy");
        }

        public string TimeFormat()
        {
            return hora_inici.ToString(@"hh\:mm");
        }
    }
}
