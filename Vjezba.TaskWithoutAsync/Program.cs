using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        Console.WriteLine($"Main thread started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");

        Task t1 = Task.Run(() =>
        {
            Console.WriteLine($"Task 1 sleeping started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            Thread.Sleep(1000);
            Console.WriteLine($"Task 1 sleeping completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        });

        Task t2 = Task.Run(() =>
        {
            Console.WriteLine($"Task 2 sleeping started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            Thread.Sleep(1500);
            Console.WriteLine($"Task 2 sleeping completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        });

        Console.WriteLine($"Waiting for all tasks at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        Task.WaitAll(t1, t2);
        Console.WriteLine($"All tasks completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");

        Console.WriteLine("\n--------------------------------------------\n");

        Task t3 = Task.Run(() =>
        {
            Console.WriteLine($"Task 3 sleeping started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            Thread.Sleep(1000);
            Console.WriteLine($"Task 3 sleeping completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        });

        Task t4 = Task.Run(() =>
        {
            Console.WriteLine($"Task 4 sleeping started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            Thread.Sleep(1500);
            Console.WriteLine($"Task 4 sleeping completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        });

        Console.WriteLine($"Waiting for any task at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        Task.WaitAny(t3, t4);
        
        Console.WriteLine($"Continued execution at: {DateTime.Now.ToString("HH:mm:ss.fff")}");

        Thread.Sleep(2000);
    }
}