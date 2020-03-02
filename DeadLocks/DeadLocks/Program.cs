using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DeadLocks
{

    /* Deadlocks: un deadlock (o bloqueo mutuo.. https://es.wikipedia.org/wiki/Bloqueo_mutuo)
     * se produce cuando uno o mas hilos desean acceder a cierto dato manteniendo bloqueado
     * parcialmente otros. eJemplo
     * 
     *     HILO A                              HILO B
     * Esperando por Dato 1             Modificando Dato 1
     * Modificando Dato 2              Esperando por Dato 2
     * 
     * O sea dos o mas hilos esperando el uno al otro de forma indefinida cuando intentan acceder a un
     * recurso que el otro tiene bloqueado.
     * 
     * Existen dos soluciones sencillas para resolver este problema:
     * 
     * Mitigando el acceso de los hilos utilizando la fucnion Monitor.TryEnter con un timer que espera
     * aleatoriamente un numero de milisegundos para volver a intentarlo (mientras el hilo es puesto a dormir)
     * si se devuelve falso.
     * 
     * Utilizar un arbitro que maneje el acceso a datos (un objeto de acceso syncronico) que permita a los hilos
     * saber cuando pueden o no acceder.
     * 
     * Existen soluciones un poco mas complejas pero no las vamos a implementar en esta etapa como son
     * el Algoritmo de Chandy–Misra–Haas inventado en 1984 
     * (https://en.wikipedia.org/wiki/Chandy%E2%80%93Misra%E2%80%93Haas_algorithm_resource_model)
     * 
     * El ejemplo mostrado debajo, hace uso del Problema de la cena de los filósofos..
     * (https://es.wikipedia.org/wiki/Problema_de_la_cena_de_los_fil%C3%B3sofos)
     * 
     * 
     * 
     */

    class Program
    {

        //Numero de Filosofos en la mesa
        public const int NUMERO_FILOSOFOS = 5;

        //Maximo tiempo en milisegundos Pensando
        public const int TIEMPO_PENSANDO = 10;

        //Maximo tiempo en milisegundos Comiendo
        public const int TIEMPO_COMIENDO = 10;

        //Maximo tiempo en milisegundos Buscando otro Tenedor
        public const int TIEMPO_BUSCANDO_TENEDOR = 15;

        //Tiempo Esperando en milisegundos para volver a buscar un tenedor
        public const int TIEMPO_ESPERANDO_VOLVER_A_BUSCAR = 15;

        //Tiempo total que pasan comiendo (aka..tiempo de vida del programa) en milisegundos
        public const int TIEMPO_CENANDO = 10000;

        //Tenedores implementados como objetos sincronos
        public static object[] tenedor = new object[NUMERO_FILOSOFOS];

        //Los filosofos implementados como hilos
        public static Thread[] Filosofo = new Thread[NUMERO_FILOSOFOS];

        //Un cronometro compartido que finaliza el programa luego del tiempo especifico
        public static Stopwatch cronometro = new Stopwatch();

        //Un diccionario para medir y llevar registro de cuantos filosofos han comido
        public static Dictionary<int, int> tiempoComiendo = new Dictionary<int, int>();

        //Como los Diccionarios no son thread safe necesitamos un syncObject para acceder
        public static object sincroComida = new object();

        //Metodo para que los filosofos piensen
        public static void Pensar()
        {
            Random random = new Random();
            Thread.Sleep(random.Next(TIEMPO_PENSANDO));
        }

        //Metodo que deja al filosofo comer
        public static void Comer(int indice)
        {
            Random random = new Random();
            int tiempo_comiendo = random.Next(TIEMPO_COMIENDO);
            Thread.Sleep(tiempo_comiendo);

            //registrar el tiempo comiendo en el diccionario
            lock (sincroComida)
            {
                tiempoComiendo[indice] += tiempo_comiendo;
            }

        }

        //El ciclo de vida del filosofo - Comer y Pensar hasta que se acabe el tiempo
        public static void ClicloDeVida (int indice, object tenedor_1, object tenedor_2)
        {
            do
            {
                bool TieneTenedor_1 = false;
                bool TieneTenedor_2 = false;

                //solo puede pensar si ha podido comer satistactoriamente
                if (TieneTenedor_1 && TieneTenedor_2)
                {
                    Pensar();
                }

                try
                {
                    //Buscar un tenedor..si se agota el tiempo buscando, dormir un tiempo aleatorio
                    Monitor.TryEnter(tenedor_1, TIEMPO_BUSCANDO_TENEDOR, ref TieneTenedor_1);
                    if (TieneTenedor_1)
                    {
                        try
                        {
                            //tiene un tenedor..buscar el otro..si se agota el tiempo buscando, dormir un tiempo aleatorio
                            Monitor.TryEnter(tenedor_2, TIEMPO_BUSCANDO_TENEDOR, ref TieneTenedor_2);
                            if (TieneTenedor_2)
                            {
                                //Tiene los dos tenedores..puede comer
                                Comer(indice);
                            }
                            else
                            {
                                //Dormir hasta que pueda volver a preguntar por un tenedor
                                Random random = new Random();
                                int tiempo_alDope = random.Next(TIEMPO_ESPERANDO_VOLVER_A_BUSCAR);
                                Thread.Sleep(tiempo_alDope);
                            }

                        }
                        finally
                        {
                            if (TieneTenedor_2)
                                Monitor.Exit(tenedor_2);

                        }
                    }
                    else
                    {
                        //Dormir hasta que pueda volver a preguntar por un tenedor
                        Random random = new Random();
                        int tiempo_alDope = random.Next(TIEMPO_ESPERANDO_VOLVER_A_BUSCAR);
                        Thread.Sleep(tiempo_alDope);
                    }

                }
                finally
                {

                    if (TieneTenedor_1)
                        Monitor.Exit(tenedor_1);
                }

            }
            while (cronometro.ElapsedMilliseconds < TIEMPO_CENANDO);
        }


        static void Main(string[] args)
        {
            
            int tiempoTranscurrido = 0;

            //Inicializar el diccionario para registrar los tiempos cenando
            for (int i = 0; i < NUMERO_FILOSOFOS; i++)
            {
                tiempoComiendo.Add(i, 0);
            }

            //Inicializar los tenedores
            for (int i = 0; i < NUMERO_FILOSOFOS; i++)
            {
                tenedor[i] = new object();
            }

            //Inicializar Filosofos
            for (int i = 0; i < NUMERO_FILOSOFOS; i++)
            {
                int indice = i;
                object tenedor_1 = tenedor[i];
                object tenedor_2 = tenedor[(i + 1) % NUMERO_FILOSOFOS];
                Filosofo[i] = new Thread(
                    _ =>
                    {
                        ClicloDeVida(indice, tenedor_1, tenedor_2);
                    });
            }

            //Filosofos inician la comida
            cronometro.Start();
            Console.WriteLine("Se sentaron a comer...");
            for (int i = 0; i < NUMERO_FILOSOFOS; i++)
            {
                Filosofo[i].Start();
            }

            //Esperar que los filosofos terminen
            for (int i = 0; i < NUMERO_FILOSOFOS; i++)
            {
                Filosofo[i].Join();
            }
            Console.WriteLine("Los filosofos han terminado de comer");

            //reportar el tiempo que pasaron comiendo
            int tiempoTotalComiendo = 0;

            for (int i = 0; i < NUMERO_FILOSOFOS; i++)
            {
                Console.WriteLine("El filosofo {0} estuvo comiendo por {1} milisegundos.", i , tiempoComiendo[i]);
                tiempoTotalComiendo += tiempoComiendo[i];
            }

            Console.WriteLine("Tiempo total que pasaron comiendo: {0} milisegundos.", tiempoTotalComiendo);
            Console.WriteLine("Tiempo que duro la cena: {0} milisegundos.", cronometro.ElapsedMilliseconds);

            tiempoTranscurrido = Convert.ToInt32(cronometro.ElapsedMilliseconds);
            var porcentaje = (tiempoTotalComiendo / tiempoTranscurrido).ToString("0.00%");
            
            Console.WriteLine("{0}  del total solamente estuvieron comiendo.", porcentaje);
            
        }
    }
}
