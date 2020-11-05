using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip
{
    class Barco
    {
        int fila_inicia;
        int columna_inicia;
        string orientación;
        int tamaño;

        public Barco(int tamaño, string orientación, int columna_inicia, int fila_inicia)
        {
            Tamaño = tamaño;
            Orientación = orientación;
            Columna_inicia = columna_inicia;
            Fila_inicia = fila_inicia;
        }

        public int Tamaño { get => tamaño; set => tamaño = value; }
        public string Orientación { get => orientación; set => orientación = value; }
        public int Columna_inicia { get => columna_inicia; set => columna_inicia = value; }
        public int Fila_inicia { get => fila_inicia; set => fila_inicia = value; }
    }
}
