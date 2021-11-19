using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Scheduling;
using Broadcast.Storage.Redis;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Sample.Farm.Server
{
    internal class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("Broadcast");

            

            Trace.Listeners.Clear();
            var ctl = new ConsoleTraceListener(false) { TraceOutputOptions = TraceOptions.DateTime };
            Trace.Listeners.Add(ctl);


            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };



            var connectionString = "localhost:6379";
            var redisOptions = ConfigurationOptions.Parse(connectionString);
            var redisStorageOptions = new RedisStorageOptions
            {
                Db = redisOptions.DefaultDatabase ?? 0
            };

            var connectionMultiplexer = ConnectionFactory.Connect(connectionString);



            var storage = new RedisStorage(connectionMultiplexer, redisStorageOptions);
            var store = new TaskStore(storage);

            var options = new Options
            {
                ServerName = $"{Environment.MachineName}-Console"
            };

            var processor = new TaskProcessor(store, options);
            var scheduler = new Scheduler();

            var server = new Broadcaster(store, processor, scheduler, options);

            //server.Recurring(() => Console.WriteLine("recurring"), TimeSpan.FromSeconds(15));

            _quitEvent.WaitOne();
        }
    }
}
