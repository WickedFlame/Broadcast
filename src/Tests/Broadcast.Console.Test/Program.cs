using Broadcast.EventSourcing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IBroadcaster broadcaster = null;
            string format = "";

            System.Console.WriteLine("Possyble commands:");
            System.Console.WriteLine("   async");
            System.Console.WriteLine("   background");
            System.Console.WriteLine("   memory");

            var input = System.Console.ReadLine();

            while (!string.IsNullOrEmpty(input))
            {
                switch (input)
                {
                    case "background":
                        broadcaster = new Broadcaster(ProcessorMode.Background);
                        format = "Test Background {0}";
                        break;

                    case "async":
                        broadcaster = new Broadcaster(ProcessorMode.Async);
                        format = "Test Async {0}";
                        break;

                    case "exit":
                        return;

                    case "memory":
                        DoMemoryTest();
                        break;

                    default:
                        System.Console.WriteLine("Possyble commands:");
                        System.Console.WriteLine("   async");
                        System.Console.WriteLine("   background");
                        System.Console.WriteLine("   memory");
                        format = null;
                        break;
                }

                if (!string.IsNullOrEmpty(format))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        var value = i.ToString();
                        System.Console.WriteLine(string.Format("Sent {0}", value));
                        broadcaster.Send(() => System.Console.WriteLine(string.Format(format, value)));
                    }

                    System.Console.WriteLine(string.Format("Items in Queue: {0}", broadcaster.Context.Store.CountQueue));
                    System.Console.WriteLine(string.Format("Items porcessed: {0}", broadcaster.Context.ProcessedTasks.Count()));

                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

                    System.Console.WriteLine(string.Format("Items in Queue: {0}", broadcaster.Context.Store.CountQueue));
                    System.Console.WriteLine(string.Format("Items porcessed: {0}", broadcaster.Context.ProcessedTasks.Count()));
                }

                input = System.Console.ReadLine();
            }
        }

        private static void DoMemoryTest()
        {
            var broadcaster = new Broadcaster(ProcessorMode.Async);
            for (int i = 0; i < 4; i++)
            {
                var tmp = new BroadcasterMemoryUser();
                broadcaster.Send(() => tmp.useMemory());
            }

            broadcaster.Send(() => System.Console.WriteLine(" - Done"));
        }

        public class BroadcasterMemoryUser
        {
            public void useMemory()
            {
                var broadcaster = new Broadcaster();
                for (int i = 0; i < 1000; i++)
                {
                    var tmp = new MemoryClass();
                    broadcaster.Send(() => tmp.UseMemory());
                }
            }
        }

        public class MemoryClass
        {
            public void UseMemory()
            {
                array = new int[100000];
                for (int i = 0; i < 100000; i++)
                {
                    array[i] = 1000000000;
                }
            }

            private int[] array;
        }
    }
}
