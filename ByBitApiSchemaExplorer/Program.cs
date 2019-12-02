using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByBitClientLib;

namespace ByBitApiSchemaExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean isRunning;
            string API_SECRET = "osDcZ3Y4gSJHggWBWOVMqB61oqkSLthnUOf0";
            string API_KEY = "HclvL8D7owWw5MePmz";
            string API_URL = @"https://" + @"api-testnet.bybit.com";
            ByBitClient client;

            isRunning = true;
            client = new ByBitClient(API_KEY, API_SECRET, API_URL);

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("ByBit Schema Explorer");
                Console.WriteLine("Select Action: ");
                Console.WriteLine("1 -- Show Shchema in JSON");
                Console.WriteLine("2 -- Show Endpoint Titles");
                Console.WriteLine("3 -- Show Schema in Readable form");

                string Choice;

                Choice = Console.ReadLine();

                if (Choice.Equals("1"))
                {
                    Console.Clear();
                    Console.WriteLine(clien);
                    Console.ReadLine();
                }
            }
        }
    }
}
