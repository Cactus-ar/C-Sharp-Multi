using System;
using System.Threading;

namespace UnionySuspension
{
    class Program
    {

        /*Vimos en ejemplos anteriores como un hilo puede 
         * terminar con condiciones impredecibles al no saber
         * cundo termina su ejecucion
         * 
         * se puede "esperar" a que dicho hilo concluya
         * suspendiendo el hilo principal del programa
         * 
         * claro que ello hace a la aplicacion "monohilo"
         * nuevamente y perdiendo toda capacidad de operar
         * multitarea, pero hay situaciones que lo requieren
         * 
         * de todas formas, esta solucion o garantia de seguridad
         * no es la unica que existe y veremos otras mucho mejores.
         */

        public static void RealizarTarea()
        {
            Console.WriteLine("Hilo de tarea");
            for (int i = 0; i < 100; i++)
            {
                Console.Write(@":{0}: ", i );
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Thread t = new Thread(RealizarTarea);

            //iniciar tarea
            t.Start();

            //forzar a la espera de finalizacion..main se suspende hasta que join devuelva true
            t.Join();

            //iniciar nuevamente..
            RealizarTarea();

            Console.WriteLine("Presione una tecla...");
            Console.ReadKey();

            //La otra posibilidad es poner a dormir al thread principal
            //cierta cantidad de milisegundos
            Thread s = new Thread(RealizarTarea);
            s.Start();

            //cuando main despierte seguira su ejecucion pero el hilo s deberia haber terminado ya
            Thread.Sleep(5000);

            //lo conveniente de este metodo es que mientras el hilo "duerme" no consume recursos del CPU
            
            RealizarTarea();

        }
    }
}
