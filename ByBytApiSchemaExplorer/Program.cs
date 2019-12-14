using System;
using System.IO;
using System.Text;
using ByBitClientLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace ByBytApiSchemaExplorer
{
    class Program
    {
        static ByBitClient client;

        static void Main(string[] args)
        {
            Boolean isRunning;

            //Load JSON template file containing Secret key,API Key and API_URL
            string JsonString = File.ReadAllText("ApiKeysTemplate.json",Encoding.UTF8);
            JObject Api_JSON = JObject.Parse(JsonString);
            string API_SECRET = (string)Api_JSON["API_SECRET"];
            string API_KEY = (string)Api_JSON["API_KEY"];
            string API_URL = (string)Api_JSON["API_URL"];

            isRunning = true;
            client = new ByBitClient(API_KEY, API_SECRET, API_URL);

            while (isRunning)
            {
                Console.WriteLine("Hello BytBit World");
                Console.WriteLine("Select Action: ");
                Console.WriteLine("1 -- Get JSON Schema");
                Console.WriteLine("2 -- Get Endpoint Names");
                Console.WriteLine("3 -- Get Endpoint Schemas");

                string Choice;

                Choice = Console.ReadLine();

                if (Choice.Equals("1"))
                {
                    Console.Clear();
                    string Schema = client.getSchemaJSON();
                    Console.WriteLine("-------------------JSON SCHEMA---------------------");
                    Console.WriteLine(Schema);
                    Console.WriteLine("Showed Schema!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("2"))
                {
                    Console.Clear();
                    string[] Names = client.getEndpointNames();
                    Console.WriteLine("-------------------EndPointNames---------------------");
                    StringBuilder NamesString = new StringBuilder();

                    foreach(string Name in Names)
                    {
                        NamesString.Append(Name + Environment.NewLine);
                    }

                    Console.WriteLine(NamesString.ToString());
                    Console.WriteLine("Close an Order!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("3"))
                {
                    Console.Clear();
                    Console.WriteLine("-------------------EndPointNames---------------------");
                    StringBuilder SchemasString = new StringBuilder();
                    string[] Names = client.getEndpointNames();

                    foreach (string Name in Names)
                    {
                        SchemasString.Append(client.getEndpointSchema(Name) + Environment.NewLine);
                        SchemasString.Append("--------------------------------------------------------------------------" + Environment.NewLine);
                    }

                    Console.WriteLine(SchemasString.ToString());
                    Console.WriteLine("Close an Order!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
            }

        }

        private static string[] GetEndPointTitles()
        {
            string[] Titles;
            String Json;
            JObject JsonVar;

            Json = client.getSchemaJSON();
            JsonVar = JObject.Parse(Json);

            Titles = (from p in ((JObject)JsonVar["EndPointSchemas"]).Properties()
                      select p.Name).ToArray<String>();

            return Titles;
        }
    }
}
