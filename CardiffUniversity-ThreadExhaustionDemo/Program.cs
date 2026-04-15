using System.Diagnostics;

public static class Program
{
    public static void Main()
    {
        ExecuteThreads();
    }

    private static void ExecuteThreads()
    {
        const int oneMb = 1024 * 1024;
        using var manualResetEvent = new ManualResetEvent(false);
        
        var threadNumber = 0;

        try
        {
            while (true)
            {
                var thread = new Thread(ThreadExecution);
                thread.Start(manualResetEvent);
                
                using var currentProcess = Process.GetCurrentProcess();
                
                var virtualMemorySize = currentProcess.VirtualMemorySize64;

                Console.WriteLine("{0}: {1}MB", ++threadNumber, virtualMemorySize / oneMb);
            }
        }
        catch (OutOfMemoryException exception)
        {
            Console.WriteLine("Out of memory after {0} threads.", threadNumber);

            // Signal all threads to finish
            manualResetEvent.Set();
        }
        finally
        {
            // Signal all threads to finish
            manualResetEvent.Set();
        }
    }

    private static void ThreadExecution(object? state)
    {
        // This just prevents the thread from finishing until signaled by the caller
        // For demonstrations so we can see thread exhaustion in action
        var manualResetEvent = state as ManualResetEvent;

        manualResetEvent!.WaitOne();
    }
}