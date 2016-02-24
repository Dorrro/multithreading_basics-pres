namespace Threads
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        #region Private Methods

        private static void Main()
        {
            new CreatingAThread().Go();
            new DangerousThread().Go();
            new ThreadPoolExample().Go();
            new TaskExample().Go();
            new TimersExample().Go();

            Console.ReadKey();
        }

        #endregion
    }

    internal class TimersExample : BaseExample
    {
        #region Public Methods

        public override void Go()
        {
            base.Go();

            var timer = new Timer(
                (parameter) => { Console.WriteLine((string)parameter); }, 
                "x", 
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(1));

            timer.Dispose();
        }

        #endregion
    }

    internal class TaskExample : BaseExample // C# 4.0
    {
        #region Public Methods

        public override void Go()
        {
            base.Go();

            {
                var startNew = Task.Factory.StartNew((() =>
                                                      {
                                                          for (var i = 0; i < 30000; i++)
                                                              Console.WriteLine("x");
                                                      }));

                startNew.Wait();
            }
            {
                var startNew = Task.Factory.StartNew(() =>
                                                     {
                                                         var random = new Random();
                                                         var next = random.Next(30000, 50000);

                                                         for (var i = 0; i < next; i++)
                                                             Console.WriteLine(i);

                                                         return next;
                                                     });

                var result = startNew.Result;
            }
            {
                var task1 = new Task(() => this.Write("x"));
                var task2 = new Task(() => this.Write("y"));
                var task3 = new Task(() => this.Write("z"));

                task1.Start();
                task2.Start();
                task3.Start();

                Task.WaitAll(task1, task2, task3);
//                var waitAny = Task.WaitAny(task1, task2, task3);
            }
        }

        #endregion

        #region Private Methods

        private void Write(string parameter)
        {
            for (var i = 0; i < 3000; i++)
                Console.WriteLine(parameter + i);
        }

        #endregion
    }

    internal class ThreadPoolExample : BaseExample
    {
        #region Public Methods

        public override void Go()
        {
            base.Go();

            var successful = ThreadPool.SetMaxThreads(10, 10);

            ThreadPool.QueueUserWorkItem(this.CallBack);
            ThreadPool.QueueUserWorkItem(this.CallBack, "i");
            ThreadPool.QueueUserWorkItem(this.CallBack, "j");
            ThreadPool.QueueUserWorkItem(this.CallBack, "x");
            ThreadPool.QueueUserWorkItem(this.CallBack, "y");
            ThreadPool.QueueUserWorkItem(this.CallBack, "z");
        }

        #endregion

        #region Private Methods

        private void CallBack(object parameter)
        {
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine((string)parameter + i);
                Thread.Sleep(1000);
            }
        }

        #endregion
    }

    internal class DangerousThread : BaseExample
    {
        #region Public Methods

        public override void Go()
        {
            base.Go();

            var parameter = "1";
            var thread1 = new Thread(() => Console.WriteLine(parameter));

            parameter = "2";
            var thread2 = new Thread(() => Console.WriteLine(parameter));

            thread1.Start();
            thread2.Start();

            thread2.Join();
            parameter = "3";
        }

        #endregion
    }

    internal class CreatingAThread : BaseExample
    {
        #region Public Methods

        public override void Go()
        {
            base.Go();

            {
                var thread = new Thread(this.Write);
                thread.Start();
            }
            {
                var thread = new Thread(() => Console.WriteLine("y"));
                thread.Start();
            }
            {
                var parameter = "z";
                var thread = new Thread(() => this.Start(parameter));
                thread.Start();
            }
            {
                var parameter = "i";
                var paremeter2 = "j";

                var thread = new Thread(this.Start);
                thread.Start(parameter);
            }
        }

        #endregion

        #region Private Methods

        private void Start(object parameter)
        {
            for (var i = 0; i < 100000000000000; i++)
                Console.WriteLine((string)parameter);
        }

        private void Write()
        {
            for (var i = 0; i < 100000000000000; i++)
                Console.WriteLine("x");
        }

        #endregion
    }

    internal abstract class BaseExample
    {
        #region Public Methods

        public virtual void Go()
        {
            Console.Clear();
        }

        #endregion
    }
}