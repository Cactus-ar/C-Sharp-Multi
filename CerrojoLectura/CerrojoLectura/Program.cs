using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CerrojoLectura
{
    internal static class Program
    {
        static ReaderWriterLockSlim candado = new ReaderWriterLockSlim();
        static Random rand = new Random();

        static void Main(string[] args)
        {
            int dato = 0;
            var tareas = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tareas.Add(Task.Factory.StartNew(() =>
                {
                    candado.EnterReadLock();
                    Console.WriteLine($"Lectura de dato dentro del cerrojo = {dato}");
                    Thread.Sleep(5000);
                    candado.ExitReadLock();

                    Console.WriteLine($"Lectura de dato fuera del cerrojo = {dato}");
                }));
            }

            try
            {
                Task.WaitAll(tareas.ToArray());
            }
            catch (AggregateException ex)
            {
                ex.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
                
            }

            while (true)
            {
                Console.ReadKey();
                candado.EnterWriteLock();
                Console.WriteLine("Cerrojo de Escritura Activo");
                int ran = rand.Next(10);
                dato = ran;
                Console.WriteLine($"Dato ahora vale {dato}");
                candado.ExitWriteLock();
                Console.WriteLine("Cerrojo de Escritura Desactivado");
            }
            
        }
    }
}
