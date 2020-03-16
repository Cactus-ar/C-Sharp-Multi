using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Este tipo de patron es muy usado en multitarea
 * el patron soporta multiples productores y consumidores
 * y puede escalar facilmente.
 * 
 * el evento manual de reset cierra o abre cuando presionamos una tecla
 */

namespace ProduceConsume
{
    class Program
    {
		// Array de consumidores
		private static List<Thread> Consumidores = new List<Thread>();

		// Cola de tareas
		private static Queue<Action> Tareas = new Queue<Action>();

		// objeto de sincronizacion
		private static readonly object ObjSyncro = new object();

		// este manejador notifica de una nueva tarea disponible
		private static EventWaitHandle NuevaTareaDisponible = new AutoResetEvent(false);

		//este manejador pausa la entrada de consumidores cuando el ultimo ha terminado
		//private static EventWaitHandle PausarConsumidor = new ManualResetEvent(true);

		private static bool pausa;

		// para manejar el objeto de sincronizacion de la consola al cambiar el color (ya que no soporta multi hilo)
		private static readonly object ConsolaLock = new object();


		// coloca en la pila una nueva tarea
		private static void TareaALaPila(Action task)
		{
			lock (ObjSyncro)
			{
				Tareas.Enqueue(task);
			}
			NuevaTareaDisponible.Set();
		}


		// hilo de trabajo para tareas
		private static void Trabajar(ConsoleColor color)
		{
			while (true)
			{

				//chequear si el producer ha preguntado por una pausa
				//PausarConsumidor.WaitOne();

				// obtener una nueva tarea
				Action task = null;
				lock (ObjSyncro)
				{
					if (Tareas.Count > 0)
					{
						task = Tareas.Dequeue();
					}
				}
				if (task != null)
				{
					// setear la consola del color especificado
					lock (ConsolaLock)
					{
						Console.ForegroundColor = color;
					}

					// ejecutar tarea
					task();
				}
				else
					// pila vacia - esperar por otra tarea
					NuevaTareaDisponible.WaitOne();
			}
		}

		static void Main(string[] args)
        {
			// inicializar consumidores
			Consumidores.Add(new Thread(() => { Trabajar(ConsoleColor.Red); }));
			Consumidores.Add(new Thread(() => { Trabajar(ConsoleColor.Green); }));
			Consumidores.Add(new Thread(() => { Trabajar(ConsoleColor.Blue); }));
			Consumidores.Add(new Thread(() => { Trabajar(ConsoleColor.Cyan); }));
			Consumidores.Add(new Thread(() => { Trabajar(ConsoleColor.Yellow); }));
			Consumidores.Add(new Thread(() => { Trabajar(ConsoleColor.Magenta); }));

			// Comenzar todos los consumiudores
			Consumidores.ForEach((t) => { t.Start(); });

			while (true)
			{
				// agregar Tarea
				Random r = new Random();
				TareaALaPila(() => {

					// la tarea consiste en escribir un numero alaeatorio en la consola
					int number = r.Next(10);
					Console.Write(number + " ");

				});

				// random sleep simulando trabajo en el main
				Thread.Sleep(r.Next(1000));

				//presionar una tecla pausa/resume las tareas
				/*
				if (Console.KeyAvailable)
				{
					Console.Read();
					if(pausa)
					{
						PausarConsumidor.Set();
						Console.WriteLine("Resumir Tareas");
					}else
					{
						PausarConsumidor.Reset();
						Console.WriteLine("Tareas Pausadas");
					}

					pausa = !pausa;
				}
				*/
			}
		}
    }
}
