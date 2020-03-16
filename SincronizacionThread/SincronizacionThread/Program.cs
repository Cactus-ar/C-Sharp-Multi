using System;
using System.Threading;

namespace SincronizacionThread
{
    class Program
    {

		/*
		 * si se quiere pasar data de forma segura entre dos o mas hilos se debe sincronizar
		 * las tareas de alguna forma.
		 * la forma mas simple de sincronizacion de hilos es Tarea.Join que como hemos visto
		 * suspende un hilo hasta que otro termina.
		 * Una forma de sincronizar un poco mas compleja se obtiene con el uso del metodo de
		 * AutoResetEvent: la llamada WaitOne suspende el hilo y la llamada Set lo resume.
		 * Para obtener una comunicacion robusta se necesitan dos Eventos para realizar las dos
		 * llamadas respectivamente.
		 */

		
		public static int resultado = 0;
		private static object objBlock = new object();
		public static EventWaitHandle listoParaRecibir = new AutoResetEvent(false);
		public static EventWaitHandle EscribirResultado = new AutoResetEvent(false);
		

		public static void Trabajar()
		{
			while (true)
			{
				int i = resultado;

				// simular un calculo que toma tiempo
				Thread.Sleep(1);

				
				// esperar a que el main reciba un resultado
				listoParaRecibir.WaitOne();
				

				// retornar un resultado
				lock (objBlock)
				{
					resultado = i + 1;
				}

				
				// ordenarle al main que escriba un resultado
				EscribirResultado.Set();
				
			}
		}

		static void Main(string[] args)
        {
			// start the thread
			Thread t = new Thread(Trabajar);
			t.Start();

			// recolectar un resultado cada 10ms
			for (int i = 0; i < 100; i++)
			{
				
				// avisarle al hilo que esta listo para recibir
				listoParaRecibir.Set();
				

				
				// esperar hasta que el thread haya escrito un resultado
				EscribirResultado.WaitOne();
				

				lock (objBlock)
				{
					Console.WriteLine(resultado);
				}

				// simular otras tareas
				Thread.Sleep(10);
			}

			// abortar de forma abrupta
			t.Abort();
		}
    }
}
