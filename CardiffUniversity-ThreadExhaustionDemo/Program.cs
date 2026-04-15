/* MIT License
 *
 * Copyright (c) 2026 SharpVNC Limited
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 * */

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