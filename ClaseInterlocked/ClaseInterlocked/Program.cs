using System;
using System.Diagnostics;
using System.Threading;

namespace ClaseInterlocked
{
    /* Demo del uso de la clase Interlocked
     * https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked?view=netcore-3.1
     * 
     * 10 hilos y tres modalidades para demostrar en que situaciones es util el uso de esta clase
     * en principio tenemos un bucle que incrementa una variable. sin proteccion alguna contra sobreescrituras
     * de la variable que cuenta..el resultado es obviamente impredecible.
     * 
     * en segundo lugar tenemos la variable dentro del cerrojo de seguridad que no permite la multi escritura
     * de los diferente hilos, pero hacemos todo el ejemplo secuencial y perdemos la posibilidad del multihilo
     * 
     * y al final tenemos el uso de la clase Interlock que para ciertas situaciones es muy util ya que permite
     * el uso seguro del multihilo para algunas tareas.
     */


    class Program
    {
        //Numero de Repeticiones
        private const int REPETICIONES = 1000000;

        //Numero de Hilos
        private const int HILOS = 10;

        //Un simple array agrupando los hilos
        public static Thread[] hilos = new Thread[HILOS];

        //contador compartido
        public static int contadorDesprotegido = 0;
        public static int contadorProtegido = 0;
        public static int contadorInterlock = 0;


        //el hilo de trabajo - sin bloqueo
        public static void Trabajar()
        {
            for (int i = 0; i < REPETICIONES; i++)
            {
                contadorDesprotegido++;
            }
        }

        //Con el uso de un objeto de sincronizacion
        private static object ObjSincro = new object();

        //el hilo de trabajo con proteccion de la variable compartida
        public static void TrabajarSincronizado()
        {
            for (int i = 0; i < REPETICIONES; i++)
            {
                lock (ObjSincro)
                {
                    contadorProtegido++;
                }
                
            }
        }

        //el hilo de trabajo con el uso de la clase Interlock
        public static void TrabajarInterBloque()
        {
            for (int i = 0; i < REPETICIONES; i++)
            {
                Interlocked.Increment(ref contadorInterlock);
            }
        }


        //La repeticion de codigo es intencional

        public static void TrabajoDesprotegido()
        {
            //lanzar todos los hilos y cronometrizar
            Stopwatch cronometro = new Stopwatch();
            cronometro.Start();

            for (int i = 0; i < HILOS; i++)
            {
                hilos[i] = new Thread(Trabajar);
                hilos[i].Start();
            }

            //esperar a que terminen
            for (int i = 0; i < HILOS; i++)
            {
                hilos[i].Join();
            }

            cronometro.Stop();

            //Mostrar el valor del contador compartido y el cronometro
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Sin ninguna medida de seguridad adoptada");
            Console.WriteLine("El valor del contador es: {0}", contadorDesprotegido);
            Console.WriteLine("El tiempo transcurrido es: {0}", cronometro.ElapsedMilliseconds);

            //Si no se modificaron las variables de inicio deberiamos esperar un numero cerca del billon no?
            //...quizas no :P
        }

        public static void TrabajoProtegido()
        {
            //lanzar todos los hilos y cronometrizar
            Stopwatch cronometro = new Stopwatch();
            cronometro.Start();

            for (int i = 0; i < HILOS; i++)
            {
                hilos[i] = new Thread(TrabajarSincronizado);
                hilos[i].Start();
            }

            //esperar a que terminen
            for (int i = 0; i < HILOS; i++)
            {
                hilos[i].Join();
            }

            cronometro.Stop();

            //Mostrar el valor del contador compartido y el cronometro
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Metodo Tradicional de seguridad");
            Console.WriteLine("El valor del contador es: {0}", contadorProtegido);
            Console.WriteLine("El tiempo transcurrido es: {0}", cronometro.ElapsedMilliseconds);

            //Quizas ahora llegue a un numero mas sensato
            
        }

        public static void TrabajoInterBloque()
        {
            //lanzar todos los hilos y cronometrizar
            Stopwatch cronometro = new Stopwatch();
            cronometro.Start();

            for (int i = 0; i < HILOS; i++)
            {
                hilos[i] = new Thread(TrabajarInterBloque);
                hilos[i].Start();
            }

            //esperar a que terminen
            for (int i = 0; i < HILOS; i++)
            {
                hilos[i].Join();
            }

            cronometro.Stop();

            //Mostrar el valor del contador compartido y el cronometro
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Metodo InterLock");
            Console.WriteLine("El valor del contador es: {0}", contadorInterlock);
            Console.WriteLine("El tiempo transcurrido es: {0}", cronometro.ElapsedMilliseconds);
        }

        static void Main(string[] args)
        {
            //Sin proteccion las cosas no salen como uno esperaba
            TrabajoDesprotegido();

            //Parece que ahora si pero hemos perdido la capacidad multihilo  al hacer que todos trabajen de forma secuencial
            //ello hace que ademas la tarea lleve mas tiempo de proceso (en teoria..los numeros en el depurador pueden reflejar
            //otra cosa, pero asumieremos que si por un momento
            TrabajoProtegido();

            //Finalmente con el uso de Interlock se puede volver a tener un procedimiento multihilo
            //(dependiendo de si el nucleo del SO y si el microprocesador lo soporta)
            TrabajoInterBloque();

        }
    }
}
