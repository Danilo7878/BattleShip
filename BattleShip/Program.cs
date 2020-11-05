using System;
using System.Data;

namespace BattleShip
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            int filas;
            int columnas;
            int cant_barcos;
            string[,] tablero;
            string [,] tablero_op_oculto;
            string[,] tablero_op_mostrar;
            bool no_errors;
            Barco[] barcos;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                BATTLESHIP");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nLeyendo Archivo...");
            Console.ForegroundColor = ConsoleColor.Red;

            //Lectura del archivo de configuraciones
            #region leerArchivo
            no_errors = false;
            filas = 0;
            columnas = 0;
            cant_barcos = 0;
            barcos = new Barco[0];
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader("../../../config.txt");

                //Tamaño del tablero
                string linea1 = file.ReadLine();
                string[] dimensiones = linea1.Split(",");
                if (dimensiones.Length != 2)
                {
                    Console.WriteLine("Error, cantidad de parámetros incorrectos para las dimensiones del tablero");
                }
                else
                {
                    filas = Convert.ToInt32(dimensiones[0]);
                    columnas = Convert.ToInt32(dimensiones[1]);

                    //Cantidad de barcos
                    string linea2 = file.ReadLine();
                    cant_barcos = Convert.ToInt32(linea2);
                    barcos = new Barco[cant_barcos];

                    string line;
                    int count = 1;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (count > cant_barcos)
                        {
                            Console.WriteLine("Error: Se especificaron más barcos de los que se indicaron en la línea 2");
                            break;
                        }
                        else
                        {
                            string[] especs_barco = line.Split(",");
                            if (especs_barco.Length != 4)
                            {
                                Console.WriteLine("Error, cantidad de parámetros incorrectos para las dimensiones del tablero");
                                count = cant_barcos;
                                break;
                            }
                            else
                            {
                                int fila_inicia = Convert.ToInt32(especs_barco[0]);
                                int columna_inicia = Convert.ToInt32(especs_barco[1]);
                                if (fila_inicia < 1 || fila_inicia > 10 || columna_inicia < 1 || columna_inicia > 10)
                                {
                                    Console.WriteLine("Error: cordenada de barco fuera de los límites del tablero");
                                    count = cant_barcos;
                                    break;
                                }
                                string orientación = especs_barco[2];
                                if(orientación == "V" || orientación == "H")
                                {
                                    int tamaño = Convert.ToInt32(especs_barco[3]);
                                    if (tamaño > 1 && tamaño < 5)
                                    {
                                        barcos[count - 1] = new Barco(tamaño, orientación, columna_inicia, fila_inicia);
                                        if(count == cant_barcos)
                                        {
                                            no_errors = true;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error: tamaño de barco no válido");
                                        count = cant_barcos;
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error, no se especificó una orientación de barco válida (V o H)");
                                    count = cant_barcos;
                                    break;
                                }
                            }
                        }
                        count++;
                    }
                    if (count <= cant_barcos)
                    {
                        Console.WriteLine("Error: Se especificaron menos barcos de los que se indicaron en la línea 2");
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine("Error al leer el archivo: " + e );
            }
            #endregion

            if (no_errors)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Lectura de archivo correcta.");
                Console.WriteLine("Construyendo Tableros...");
                Console.ForegroundColor = ConsoleColor.Red;

                //construcción de tableros
                #region tableros
                //tablero vacío jugador
                tablero = DibujarTableroVacío(filas, columnas);
                tablero_op_mostrar = DibujarTableroVacío(columnas, filas);
                tablero_op_oculto = DibujarTableroVacío(columnas, filas);
                //agregar barcos al tablero del jugador
                for (int i = 0; i < cant_barcos; i++)
                {
                    string orientación = barcos[i].Orientación;
                    if(orientación == "H")
                    {
                        for (int k = barcos[i].Columna_inicia; k < barcos[i].Columna_inicia+barcos[i].Tamaño; k++)
                        {
                            if (tablero[barcos[i].Fila_inicia, k] == "  -  ")
                            {
                                tablero[barcos[i].Fila_inicia, k] = "  B  ";
                            }
                            else
                            {
                                Console.WriteLine("Error al construir tablero: traslape de barcos, barco: " + (i+1).ToString());
                                no_errors = false;
                                break;
                            }
                        }
                    } 
                    else
                    {
                        for (int k = barcos[i].Fila_inicia; k < barcos[i].Fila_inicia+barcos[i].Tamaño; k++)
                        {
                            if (tablero[k, barcos[i].Columna_inicia] == "  -  ")
                            {
                                tablero[k, barcos[i].Columna_inicia] = "  B  ";
                            }
                            else
                            {
                                Console.WriteLine("Error al construir tablero: traslape de barcos, barco: " + (i+1).ToString());
                                no_errors = false;
                                break;
                            }
                        }
                    }
                }
                #endregion

                if (no_errors)
                {
                    TransponerTablero(tablero, ref tablero_op_oculto);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Tablero construido correctamente.");
                    Console.WriteLine("Iniciando Juego...\n");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("                ¿LISTO PARA LA BATALLA?\n");
                    Console.ForegroundColor = ConsoleColor.White;


                    //Turnos del juego
                    #region turnos
                    int turno = 0;
                    int turnos_fallidos = 0;
                    string mensaje_salida = "";
                    while (true)
                    {
                        turno++;
                        if (turnos_fallidos == 4)
                        {
                            mensaje_salida = "Se han fallado 4 turnos seguidos";
                            break;
                        }
                        Console.WriteLine("JUGADOR");
                        ImprimirTablero(tablero);
                        Console.WriteLine("\nOPONENTE");
                        ImprimirTablero(tablero_op_mostrar);
                        Console.WriteLine("\nTurno: " + turno);
                        if (turno % 2 == 0)
                        {

                        }
                        else {
                            Console.WriteLine("Su turno, ingrese coordenada: ");
                            string opción = Console.ReadLine();
                            if (opción.ToUpper() == "X")
                            {
                                mensaje_salida = "Jugador eligió la opción de salir";
                                break;
                            }
                            else
                            {
                                string[] posición = opción.Split(",");
                                int x = Convert.ToInt32(posición[0]);
                                int y = Convert.ToInt32(posición[1]);
                                if (x > columnas || y > filas || x < 1 || y < 1)
                                {
                                    mensaje_salida = "Error: coordenadas fuera de los límites del tablero, el juego se detuvo";
                                    break;
                                }
                                else 
                                {
                                    if (tablero_op_oculto[x,y] == "  B  ")
                                    {
                                        tablero_op_oculto[x, y] = "  X  ";
                                        tablero_op_mostrar[x, y] = "  X  ";
                                        //Console.Clear
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("\n      ¡EXCELENTE!");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.WriteLine("\n      ¡MAL! Se ha desperdiciado un turno");
                                        turnos_fallidos++;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            Console.ReadLine();
        }


        //Métodos y funciones
        static string[,] DibujarTableroVacío(int filas, int columnas)
        {
            string[,] tablero = new string[filas + 1, columnas + 1];
            for (int i = 0; i <= filas; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j <= columnas; j++)
                    {
                        if (j == 0)
                        {
                            tablero[i, j] = j.ToString() + "  ";
                        }
                        else if (j == 10)
                        {
                            tablero[i, j] = "  " + j.ToString();
                        }
                        else
                        {
                            tablero[i, j] = "  " + j.ToString() + "  ";
                        }
                    }
                }
                else
                {
                    for (int j = 0; j <= columnas; j++)
                    {
                        if (j == 0)
                        {
                            if (i == 10)
                            {
                                tablero[i, j] = i.ToString() + " ";
                            }
                            else
                            {
                                tablero[i, j] = i.ToString() + "  ";
                            }
                        }
                        else
                        {
                            tablero[i, j] = "  -  ";
                        }
                    }
                }
            }
            return tablero;
        }
        static void TransponerTablero(string[,] tablero, ref string [,] tablero_op_oculto)
        {
            for (int i = 1; i < tablero.GetLength(0); i++)
            {
                for (int j = 1; j < tablero.GetLength(1); j++)
                {
                    tablero_op_oculto[j, i] = tablero[i, j];
                }
            }
        }
        static void ImprimirTablero(string[,] tablero)
        {
            for (int i = 0; i < tablero.GetLength(0); i++)
            {
                for (int j = 0; j < tablero.GetLength(1); j++)
                {
                    Console.Write(tablero[i, j]);
                }
                Console.Write("\n");
            }
        }
    }
}
