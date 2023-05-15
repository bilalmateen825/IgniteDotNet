using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;

var cfg = new IgniteClientConfiguration
{
    Endpoints = new[] { "127.0.0.1:10800" }
    
    //Default port: 10800
    //local host 127.0.0.1
};

int nKey = 1;

using (var client = Ignition.StartClient(cfg))
{
    ICacheClient<int, string> cache = client.GetOrCreateCache<int, string>("CustomCache");

    if (cache.ContainsKey(nKey))
    {
        Console.WriteLine($"Cache Contains {cache.Get(nKey)}");
    }
    else
    {
        cache.Put(nKey, "First Item");
        Console.WriteLine($"Item inserted into Cache");
    }
}

Console.ReadKey();