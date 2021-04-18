using System;
using System.Linq;
using Broadcast.Composition;

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
            System.Console.WriteLine("   schedule");
            System.Console.WriteLine("   recurring");
            System.Console.WriteLine("   recurring2");
			System.Console.WriteLine("   multy");

            var input = System.Console.ReadLine();

            while (!string.IsNullOrEmpty(input))
            {
                switch (input)
                {
                    case "background":
                        broadcaster = new Broadcaster();
                        format = "Test Background {0}";
                        break;

                    //case "async":
                    //    broadcaster = new Broadcaster(ProcessorMode.Async);
                    //    format = "Test Async {0}";
                    //    break;

                    case "exit":
                        return;

                    case "memory":
                        DoMemoryTest();
                        break;

                    case "schedule":
                        using (var scheduler = new Broadcaster())
                        {
                            scheduler.Schedule(() => System.Console.WriteLine("   Scheduled message 5 sec"), TimeSpan.FromSeconds(5));
                            scheduler.Schedule(() => System.Console.WriteLine("   Scheduled message 3 sec"), TimeSpan.FromSeconds(3));

                            System.Console.WriteLine($"Starting scheduler with {Scheduler.SchedulerCount} schedulers");
                            scheduler.Send(() => System.Console.WriteLine("Direct message"));

                            System.Console.ReadLine();
                        }
                        break;

                    case "recurring":
                        using (var scheduler = new Broadcaster())
                        {
                            scheduler.Recurring(() => System.Console.WriteLine("   Recurring Expression 5 sec"), TimeSpan.FromSeconds(5));
                            scheduler.Recurring(() => System.Console.WriteLine("   Recurring Expression 3 sec"), TimeSpan.FromSeconds(3));

                            System.Console.WriteLine($"Starting recurring with {Scheduler.SchedulerCount} schedulers");
                            scheduler.Send(() => System.Console.WriteLine("Direct message"));

                            System.Console.ReadLine();
                        }
                        break;
                    case "recurring2":
	                    using (var scheduler = new Broadcaster())
	                    {
							scheduler.Recurring(TaskFactory.CreateTask(() => System.Console.WriteLine("   Recurring Action 5 sec")), TimeSpan.FromSeconds(5));
							scheduler.Recurring(TaskFactory.CreateTask(() => System.Console.WriteLine("   Recurring Action 3 sec")), TimeSpan.FromSeconds(3));

							System.Console.WriteLine($"Starting recurring with {Scheduler.SchedulerCount} schedulers");

		                    System.Console.ReadLine();
	                    }
	                    break;

					case "multy":
                        using (var scheduler = new Broadcaster())
                        {
                            scheduler.Recurring(() => System.Console.WriteLine("   Recurring message 5 sec"), TimeSpan.FromSeconds(5));
                            scheduler.Recurring(() => System.Console.WriteLine("   Recurring message 3 sec"), TimeSpan.FromSeconds(3));

                            System.Console.WriteLine($"Starting recurring with {Scheduler.SchedulerCount} schedulers");
                            scheduler.Send(() => System.Console.WriteLine("Direct message"));

                            using (var scheduler2 = new Broadcaster())
                            {
                                scheduler2.Recurring(() => System.Console.WriteLine("   Recurring message 2 5 sec"), TimeSpan.FromSeconds(5));
                                scheduler2.Recurring(() => System.Console.WriteLine("   Recurring message 2 3 sec"), TimeSpan.FromSeconds(3));

                                System.Console.WriteLine($"Starting recurring with {Scheduler.SchedulerCount} schedulers");
                                scheduler2.Send(() => System.Console.WriteLine("Direct 2 message"));

                                System.Console.ReadLine();
                            }
                        }
                        break;

                    default:
                        System.Console.WriteLine("Possyble commands:");
                        System.Console.WriteLine("   async");
                        System.Console.WriteLine("   background");
                        System.Console.WriteLine("   memory");
                        System.Console.WriteLine("   schedule");
                        System.Console.WriteLine("   recurring");
                        System.Console.WriteLine("   multy");
                        format = null;
                        break;
                }

                if (!string.IsNullOrEmpty(format))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        var value = i;
                        System.Console.WriteLine(string.Format("Sent {0}", value));
                        broadcaster.Send(() => System.Console.WriteLine(string.Format(format, value)));
                    }

                    System.Console.WriteLine(string.Format("Items in Queue: {0}", broadcaster.Context.Queue.Count));
                    System.Console.WriteLine(string.Format("Items porcessed: {0}", broadcaster.Context.ProcessedTasks.Count()));

                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

                    System.Console.WriteLine(string.Format("Items in Queue: {0}", broadcaster.Context.Queue.Count));
                    System.Console.WriteLine(string.Format("Items porcessed: {0}", broadcaster.Context.ProcessedTasks.Count()));
                }

                input = System.Console.ReadLine();
            }
        }

        private static void Schedule()
        {
            throw new NotImplementedException();
        }

        private static void DoMemoryTest()
        {
            var broadcaster = new Broadcaster();
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
