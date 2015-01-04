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

                    default:
                        System.Console.WriteLine("Possyble commands:");
                        System.Console.WriteLine("   async");
                        System.Console.WriteLine("   background");
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

                    System.Console.WriteLine(string.Format("Items in Queue: {0}", broadcaster.Context.Tasks.CountQueue));
                    System.Console.WriteLine(string.Format("Items porcessed: {0}", broadcaster.Context.ProcessedTasks.Count()));

                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

                    System.Console.WriteLine(string.Format("Items in Queue: {0}", broadcaster.Context.Tasks.CountQueue));
                    System.Console.WriteLine(string.Format("Items porcessed: {0}", broadcaster.Context.ProcessedTasks.Count()));
                }

                input = System.Console.ReadLine();
            }
        }
    }
}
