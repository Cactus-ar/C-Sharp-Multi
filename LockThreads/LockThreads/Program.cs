using System;
using System.Threading;


/* Es una tecnica que se utiliza para evitar principalmente
 * las condiciones de carrera en los hilos que quieren
 * acceder al mismo dato.
 * 
 * Basicamente la idea es encapsular en una sola unidad
 * indivisible un fragmento de codigo para que no pueda ser 
 * modificado o accedido sin tener que pasar por todo
 * el procedimiento que le dio origen (llamado comunmente
 * seccion critica)
 * 
 * Para utilizar esta tecnica hay que asegurarse que cada variable
 * y asignacion esta protegida dentro de una seccion critica.
 * De igual forma las operaciones de comparacion y asignacion
 * estan embebidas dentro de la seccion critica.
 * 
 * Mantener a las secciones de codigo criticas lo mas
 * cortas posibles para que puedan terminar cuanto antes.
 */



namespace LockThreads
{
    class Program
    {

        private static int variable_1 = 1;
        private static int variable_2 = 1;


        //El siguiente procedimiento no es safe o estable.
        //si por algun motivo uno de los hilos llega antes a modificar
        //el valor de variable_2 la division podria dar 0 y producir una
        //salida inesperada del programa
        public static void RealizarTarea()
        {
            if (variable_2 > 0)
            {
                Console.WriteLine(variable_1 / variable_2);
                variable_2 = 0;
            }
        }


        //para que no se de la situacion de arriba
        //debemos asegurarnos que el procedimiento 
        //es tratado como una sola unidad indivisible
        //para ello encapsulamos dentro de un tipo de objeto
        //todo el codigo que queremos proteger
        private static object objetoSyncronizacion = new object();

        public static void RealizarTareaSegura()
        {
            lock (objetoSyncronizacion) // el codigo esta protegido contra condiciones de carrera
            {
                if (variable_2 > 0)
                {
                    Console.WriteLine(variable_1 / variable_2);
                    variable_2 = 0;
                }
            }
            
        }


        static void Main(string[] args)
        {
            //Thread t1 = new Thread(RealizarTarea); //inseguro
            //Thread t2 = new Thread(RealizarTarea); //inseguro
            //t1.Start(); //inseguro
            //t2.Start(); //inseguro

            Thread t2 = new Thread(RealizarTareaSegura); 
            Thread t3 = new Thread(RealizarTareaSegura);
            t2.Start();
            t3.Start();
        }
    }
}
