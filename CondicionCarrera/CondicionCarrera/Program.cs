using System;
using System.Threading;

namespace CondicionCarrera
{

    /* Condiciones de Carrera y como solucionarlas
     * Que son? https://es.wikipedia.org/wiki/Condici%C3%B3n_de_carrera
     * 
     * Basicamente son uno o mas procesos modificando el mismo dato.
     * 
     */


    class Program
    {
        public static int guiones = 0;

        public static void RealizarTarea()
        {
            
            for (guiones = 0; guiones < 5; guiones++)
            {
                Console.Write("-");
            }
            
        }

        static void Main(string[] args)
        {
            
            //iniciar tarea que escribe 5 guiones
            Thread t = new Thread(RealizarTarea);
            t.Start();

            //escribir otros 5 guiones
            RealizarTarea();

            //la variable guiones es modificada por los dos procesos 
            //haciendo que se produzca una condicion de carrera, la cual muestra
            //como los dos procesos van incrementando el mismo dato y dando
            //como resultado que la variable se incremente de forma inpredecible

        }
    }
}
