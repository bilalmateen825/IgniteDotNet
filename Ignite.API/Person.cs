using Apache.Ignite.Core.Cache.Configuration;

namespace Ignite.API
{
    public class Person
    {
        [QuerySqlField]
        public string Name { get; set; }

        [QuerySqlField]
        public int Age { get; set; }
    }
}