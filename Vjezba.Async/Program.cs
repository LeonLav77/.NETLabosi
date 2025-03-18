using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine($"Main started at: {DateTime.Now:HH:mm:ss.fff}");
        await SleepF1();
        Console.WriteLine($"Main completed at: {DateTime.Now:HH:mm:ss.fff}");
    }

    static async Task SleepF1()
    {
        Console.WriteLine($"SleepF1 started at: {DateTime.Now:HH:mm:ss.fff}");
        
        await Task.Delay(1000);
        Console.WriteLine($"SleepF1 after first delay at: {DateTime.Now:HH:mm:ss.fff}");
        
        await SleepF2();
        
        Console.WriteLine($"SleepF1 completed at: {DateTime.Now:HH:mm:ss.fff}");
    }

    static async Task SleepF2()
    {
        Console.WriteLine($"SleepF2 started at: {DateTime.Now:HH:mm:ss.fff}");
        
        await Task.Delay(1500);
        Console.WriteLine($"SleepF2 after delay at: {DateTime.Now:HH:mm:ss.fff}");
        
        Console.WriteLine($"SleepF2 completed at: {DateTime.Now:HH:mm:ss.fff}");
    }
}