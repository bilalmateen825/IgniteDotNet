using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Ignite.API;

public class Program
{
    private const string CacheName = "myCache";

    public static void Main(string[] args)
    {
        var cfg = new IgniteClientConfiguration { Endpoints = new[] { "127.0.0.1" } };

        Thread.Sleep(30000); // Waiting for Server to Up

        using (var client = Ignition.StartClient(cfg))
        {
            var cache = client.GetOrCreateCache<int, Person>(new CacheClientConfiguration
            {
                Name = CacheName,
                QueryEntities = new[] { new QueryEntity(typeof(int), typeof(Person)) }
            });

            ////Read
            // var a = cache[1];

            // Create
            cache[1] = new Person { Name = "Person 1", Age = 20 };
            cache[2] = new Person { Name = "Person 2", Age = 22 };

            Console.WriteLine("Data added to cache. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
