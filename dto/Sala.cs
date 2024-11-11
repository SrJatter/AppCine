using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCine.dto
{
    internal class Sala
    {
        public int numero { get; set; }
        public int filas { get; set; }
        public int columnas { get; set; }
        public bool[,] asientos { get; set; } // Matriz de asientos (true si está ocupado)

        public Sala(int numero)
        {
            filas = 3;
            columnas = 3;
            asientos = new bool[filas, columnas];
            this.numero = numero;
        }

        // Marca un asiento como reservado
        public void ReservarAsiento(int numero ,int fila, int columna)
        {
            if (fila < filas && columna < columnas && !asientos[fila, columna])
            {
                asientos[fila, columna] = true;
            }
            else
            {
                Console.WriteLine("El asiento ya está reservado o la posición es inválida.");
            }
        }

        // Devuelve si un asiento está reservado
        public bool EstaOcupado(int fila, int columna)
        {
            return asientos[fila, columna];
        }
    }
}