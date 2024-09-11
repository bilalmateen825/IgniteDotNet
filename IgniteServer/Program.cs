using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Event;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Cache.Query.Continuous;
using Apache.Ignite.Core.Communication.Tcp;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Ignite.API;
using System;

public class Program
{
    private const string CacheName = "myCache";

    public static void Main(string[] args)
    {

        IgniteConfiguration cfg = new IgniteConfiguration
        {
            DiscoverySpi = new TcpDiscoverySpi
            {
                IpFinder = new TcpDiscoveryStaticIpFinder
                {
                    Endpoints = new[] { "127.0.0.1:47500..47509" }
                }
            },

            CommunicationSpi = new TcpCommunicationSpi { IdleConnectionTimeout = TimeSpan.FromSeconds(5) },

            //https://ignite.apache.org/docs/latest/persistence/native-persistence
            DataStorageConfiguration = new Apache.Ignite.Core.Configuration.DataStorageConfiguration()
            {
                StoragePath = "/ssd/storage",
                DefaultDataRegionConfiguration = new Apache.Ignite.Core.Configuration.DataRegionConfiguration()
                {
                    Name = "Default_Region",
                    PersistenceEnabled = true,

                }
            }
        };

        using (var ignite = Ignition.Start(cfg))
        {
            ignite.GetCluster().DisableWal(CacheName);
            var cache = ignite.GetOrCreateCache<int, Person>(new CacheConfiguration
            {
                Name = CacheName,
                QueryEntities = new[] { new QueryEntity(typeof(int), typeof(Person)) }
            });


            var qry = new ContinuousQuery<int, Person>(new LocalListener(), new RemoteFilter());

            cache.QueryContinuous(qry);
            ignite.GetCluster().EnableWal(CacheName);
            Console.WriteLine("Server started. Press any key to exit...");
            Console.ReadKey();
        }
    }

    private class LocalListener : ICacheEntryEventListener<int, Person>
    {
        public void OnEvent(IEnumerable<ICacheEntryEvent<int, Person>> events)
        {
            foreach (var evt in events)
            {
                Console.WriteLine($"Intercepted request from client: {evt.EventType} [Key={evt.Key}, Value={evt.Value}]");
                var cache = Ignition.GetIgnite().GetOrCreateCache<int, Person>(CacheName);
                cache[evt.Key] = evt.Value;
                // Perform some operations here before the data is added to the cache and database
            }
        }
    }

    private class RemoteFilter : ICacheEntryEventFilter<int, Person>
    {
        public bool Evaluate(ICacheEntryEvent<int, Person> evt)
        {
            return true;
        }
    }
}
