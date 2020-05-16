using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Net Framework: Uso de Task Factory


namespace TaskNetFramework
{
    class Program
    {
        static void Escribir( char c)
        {
            int i = 1000;
            while (i -- > 0)
            {
                Console.Write(c);
            }
        }

        static void Main(string[] args)
        {
            //Existen dos formas basicas de crear una Task:

            //La primera, donde se le pasa una Accion (que puede ser una funcion, o una expresion, etc
            //Ello hace que la tarea se ejecute al instante que la creamos.
            //En este caso le vamos a pasar la funcion
                Task.Factory.StartNew(() => Escribir('G'));


            //La otra forma de crear una instancia de la clase Tarea
            var tarea = new Task(() => Escribir('Q'));
            //esta declaracion no va a inciciar la tarea de forma inmediata
            //se necesita declarar el inicio de forma explicita
            tarea.Start();


            //y obviamente esta la ejecución del thread principal
            Escribir('P');

            //el buffer de la consola de windows se ha mejorado para que
            //acepte multi procesos y por ello la salida del programa se va a ver
            //de forma impredecible


            Console.WriteLine("Principal Finalizo");
            Console.ReadKey();
        }
    }
}
