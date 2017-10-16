using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Numerics;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics;


namespace TestMathNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculate(100, 300, 100, 80, 50, 80, out double alpha, out double l);
            Console.WriteLine($"aplha :{alpha/Math.PI*180}\nl:{l}");
            Console.ReadKey();
        }

        static void TestTask()
        {
            //var t1 = new Task(() => TaskMethod("Task 1"));
            //var t2 = new Task(() => TaskMethod("Task 2"));
            //t2.Start();
            //t1.Start();
            //Task.Run(() => TaskMethod("Task 3"));
            //Task.Factory.StartNew(() => TaskMethod("Task 4"));
            //Task.Factory.StartNew(() => TaskMethod("Task 5"),TaskCreationOptions.LongRunning);
            //Thread.Sleep(TimeSpan.FromSeconds(1));

            //TaskMethod("Main Method");
            //Task<int> task = CreateTask("Task 1");
            //task.Start();
            //int result = task.Result;
            //Console.WriteLine("Result is: {0}", result);

            //task = CreateTask("Task 2");
            //task.RunSynchronously();
            //result = task.Result;
            //Console.WriteLine($"Result is: {result}");

            //task = CreateTask("Task 3");
            //task.Start();
            //while (!task.IsCompleted)
            //{
            //    Console.WriteLine(task.Status);
            //    Thread.Sleep(TimeSpan.FromSeconds(0.5));

            //}
            //Console.WriteLine(task.Status);
            //result = task.Result;
            //Console.WriteLine($"Result is : {result}");

            //var firstTask = new Task<int>(() => TaskMethod("Frist Task", 3));
            //var secondTask = new Task<int>(() => TaskMethod("Second Task", 2));

            //firstTask.ContinueWith(
            //    t => Console.WriteLine($"The first answer is {t.Result}. Thread id {Thread.CurrentThread.ManagedThreadId}," +
            //                           $"is thread pool thread {Thread.CurrentThread.IsThreadPoolThread}"));

            //firstTask.Start();
            //secondTask.Start();

            //Thread.Sleep(TimeSpan.FromSeconds(4));

            //Task continuation = secondTask.ContinueWith(
            //    t => Console.WriteLine($"The second answer is {t.Result}. Thread id {Thread.CurrentThread.ManagedThreadId}," +
            //                           $"is thread pool thread {Thread.CurrentThread.IsThreadPoolThread}"), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

            //continuation.GetAwaiter().OnCompleted(() => Console.WriteLine($"Continuation Task completed! Thread id {Thread.CurrentThread.ManagedThreadId}, " +
            //                            $"is Thread pool Thread: {Thread.CurrentThread.IsThreadPoolThread}"));

            //Thread.Sleep(TimeSpan.FromSeconds(2));
            //Console.WriteLine();

            //firstTask = new Task<int>(() =>
            //{
            //    var innerTask = Task.Factory.StartNew(() => TaskMethod("Second Task", 5), TaskCreationOptions.AttachedToParent);
            //    innerTask.ContinueWith(t => TaskMethod("Third Task", 2), TaskContinuationOptions.AttachedToParent);
            //    return TaskMethod("First Method", 2);
            //});

            //firstTask.Start();

            //while (!firstTask.IsCompleted)
            //{
            //    Console.WriteLine(firstTask.Status);
            //    Thread.Sleep(TimeSpan.FromSeconds(0.5));
            //}
            //Console.WriteLine(firstTask.Status);

            //Thread.Sleep(TimeSpan.FromSeconds(10));

            var cts = new CancellationTokenSource();
            var longTask = new Task<int>(() => TaskMethod("Task 1", 10, cts.Token), cts.Token);
            Console.WriteLine(longTask.Status);
            cts.Cancel();
            Console.WriteLine(longTask.Status);
            Console.WriteLine("First task has been cancelled before execution.");
            cts = new CancellationTokenSource();
            longTask = new Task<int>(() => TaskMethod("Task 2", 10, cts.Token), cts.Token);
            longTask.Start();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine(longTask.Status);
            }
            cts.Cancel();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine(longTask.Status);
            }
            Console.WriteLine($"A task has been completed with result {longTask.Result}");
            Console.ReadKey();
        }

        static int TaskMethod(string name, int seconds, CancellationToken token)
        {
            Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}"
                , name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            for (int i = 0; i < seconds; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                if (token.IsCancellationRequested)
                {
                    return -1;
                }
            }
            return 42 * seconds;
        }

        static void MathNetTest()
        {
            Tuple<Complex, Complex, Complex> results = FindRoots.Cubic(1, 2, 3, 4);
            Console.WriteLine("The result of 1+2x+3x^2+4x^3=0 is:");
            Console.WriteLine(results.Item1);
            Console.WriteLine(results.Item2);
            Console.WriteLine(results.Item3);
        }

        static void MathNetInterpolation()
        {
            var points = new List<double> { 1, 2, 3, 5, 7};
            var values = new List<double> { 2, 4, 6, 4, 8 };
            var f = Interpolate.CubicSplineRobust(points, values);
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine($"f({i}) = {f.Integrate(i)}");
            }
        }

        static void Calculate(double a, double b,double c, double d,double R1,double R2,out double alpha, out double l)
        {
            alpha = FindRoots.OfFunction(x => (d - (1 - Math.Cos(x)) * (R1 + R2)) / (b - Math.Sin(x) * (R1 + R2)) - Math.Tan(x), 0, Math.PI / 2);
            l = (d - (1 - Math.Cos(alpha))* (R1 + R2)) / Math.Sin(alpha);
        }
    }
}
