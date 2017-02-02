using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = AsynchronousProcessing1();
            t.Wait();
            Console.ReadKey();
        }

        async static Task AsynchronousProcessing1()
        {
            Console.WriteLine("1. Single Expection");
            try
            {
                string result = await GetInfoAsync("Task 1", 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expection details: {0}", ex);
            }
        }

        async static Task AsynchronousProcessing()
        {
            //Func<string, Task<string>> asyncLambda = async name =>
            // {
            //     await Task.Delay(TimeSpan.FromSeconds(2));
            //     return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            //                      name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            // };

            //string result = await asyncLambda("async Lambda");
            //Console.WriteLine(result);

            Task<string> t1 = GetInfoAsync("Task 1", 2);
            Task<string> t2 = GetInfoAsync("Task 2", 5);
            string[] results = await Task.WhenAll(t1, t2);
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        async static Task AsynchronyWithAwait()
        {
            try
            {
                string result = await GetInfoAsync("Task 1",2);
                Console.WriteLine(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        async static Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            throw new Exception($"Boom from {name}");
            //return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
                                 //name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }
    }
}
