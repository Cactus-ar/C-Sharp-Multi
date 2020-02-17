using System;
using System.Threading;

namespace IniciarHilo
{
    //Dependiendo del SO, microprocesador y arquitecturas la salida
    //de este programa es impredecible (cada fragmento ejecutado
    //se lo llama generalmente una rebanada de tiempo (time slice)
    //y puede tener diferente duracion)


    class Program
    {

        private const int REPETICIONES = 10000;


        public static void IniciarB()
        {
            
            for (int i = 0; i < REPETICIONES; i++)
            {
                Console.Write("B");
            }
        }

        public static void IniciarC()
        {

            for (int i = 0; i < REPETICIONES; i++)
            {
                Console.Write("C");
            }
        }

        public static void IniciarD()
        {

            for (int i = 0; i < REPETICIONES; i++)
            {
                Console.Write("D");
            }
        }




        static void Main(string[] args)
        {
            //comenzar un nuevo thread

            //forma de construccion clasica
            Thread t1 = new Thread(new ThreadStart(IniciarB));
            t1.Start();

            //c# infiere que al inicializar un nuevo hilo se delega de ThreadStart
            //por ello se puede escribir el mismo codigo un poco mas limpio
            Thread t2 = new Thread(IniciarC);
            t2.Start();

            //tambien puede utilizarse dentro de una expresion Lamda
            //bajo el esquema de expresion del tipo anonima
            Thread t3 = new Thread(() => { IniciarD(); });
            t3.Start();

            
            for (int i = 0; i < REPETICIONES; i++)
            {
                Console.Write("A");
            }



        }
    }
}
