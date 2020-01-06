using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {

        static void Main(string[] args)
        {

            SayHelloRequest req = new SayHelloRequest();
            req.CityNames = new List<string> { "Mumbai", "Delhi", "Chennai", "Pune" };


            PartitionKeyGenerator keygen = new PartitionKeyGenerator();
            keygen.PartitionKey = new List<string> { "input-040120201213" };
            string jsonKey = JsonConvert.SerializeObject(keygen);

            string jsonCity = JsonConvert.SerializeObject(req);
            Console.WriteLine(jsonCity + "\n" + jsonKey);
            Console.ReadLine();
        }
    }

    class SayHelloRequest
    {
        public List<string> CityNames { get; set; }
    }

    class PartitionKeyGenerator
    {
        public List<string> PartitionKey { get; set; }
    }
}
