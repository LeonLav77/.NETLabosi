using System;
using System.Threading.Tasks;

namespace Vjezba.Model
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting main thread at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            
            // First demo: Task.WaitAll()
            DemoWaitAll();
            
            Console.WriteLine("\n------------------------------\n");
            
            // Second demo: Task.WaitAny()
            DemoWaitAny();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        static void DemoWaitAll()
        {
            Console.WriteLine("DEMO: Task.WaitAll()");
            Console.WriteLine("---------------------");
            
            // Create two tasks with different wait times
            Task task1 = Task.Run(() => {
                Console.WriteLine($"Task 1 started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
                Task.Delay(1000).Wait(); // Wait for 1 second
                Console.WriteLine($"Task 1 completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            });
            
            Task task2 = Task.Run(() => {
                Console.WriteLine($"Task 2 started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
                Task.Delay(1500).Wait(); // Wait for 1.5 seconds
                Console.WriteLine($"Task 2 completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            });
            
            Console.WriteLine($"Waiting for all tasks to complete at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            
            // Wait for both tasks to complete
            Task.WaitAll(task1, task2);
            
            Console.WriteLine($"All tasks completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        }
        
        static void DemoWaitAny()
        {
            Console.WriteLine("DEMO: Task.WaitAny()");
            Console.WriteLine("--------------------");
            
            // Create two tasks with different wait times
            Task task1 = Task.Run(() => {
                Console.WriteLine($"Task 1 started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
                Task.Delay(1000).Wait(); // Wait for 1 second
                Console.WriteLine($"Task 1 completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            });
            
            Task task2 = Task.Run(() => {
                Console.WriteLine($"Task 2 started at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
                Task.Delay(1500).Wait(); // Wait for 1.5 seconds
                Console.WriteLine($"Task 2 completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            });
            
            Console.WriteLine($"Waiting for any task to complete at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            
            // Wait for any task to complete
            int completedTaskIndex = Task.WaitAny(task1, task2);
            
            Console.WriteLine($"Task {completedTaskIndex + 1} completed first at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            
            // Wait for the remaining task
            Console.WriteLine($"Waiting for remaining task at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            if (completedTaskIndex == 0)
                task2.Wait();
            else
                task1.Wait();
                
            Console.WriteLine($"All tasks completed at: {DateTime.Now.ToString("HH:mm:ss.fff")}");
        }
    }
}