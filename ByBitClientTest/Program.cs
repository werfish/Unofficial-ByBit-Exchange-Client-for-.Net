using System;
using System.IO;
using ByBitClientLib;

namespace ByBitClientTest
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
            client = new ByBitClient(API_KEY,API_SECRET,API_URL);

            while (isRunning)
            {
                Console.WriteLine("Hello BytBit World");
                Console.WriteLine("Select Action: ");
                Console.WriteLine("1 -- Make an order!");
                Console.WriteLine("2 -- Close Order!");
                Console.WriteLine("3 -- Check Balance!");
                Console.WriteLine("4 -- Check Leverage!");
                Console.WriteLine("5 -- Change Leverage!");
                Console.WriteLine("6 -- Get Current Price!");
                Console.WriteLine("7 -- Get Current Candle!");

                string Choice;

                Choice = Console.ReadLine();

                if (Choice.Equals("1"))
                {
                    Console.Clear();
                    ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
                    request.AddRequired("Buy", "BTCUSD", "Market", 1, "");
                    string order = request.Execute();
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Made an Order!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("2"))
                {
                    Console.Clear();
                    ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
                    request.AddRequired("Sell", "BTCUSD", "Market", 1, "");
                    string order = request.Execute();
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Close an Order!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("3"))
                {
                    Console.Clear();
                    string order = client.MakeRequest("GET_MyPosition");
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Checked Balance!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("4"))
                {
                    Console.Clear();
                    string order = client.MakeRequest("GET_UserLeverage");
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Checked Leverage!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("5"))
                {
                    Console.Clear();
                    string order = client.MakeRequest("POST_ChangeUserLeverage","BTCUSD", "10");
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Changed Leverage!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("6"))
                {
                    Console.Clear();
                    string order = client.MakeRequest("GET_LatestInformationForSymbol","BTCUSD");
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Current Price Fetched!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("7"))
                {
                    Console.Clear();
                    int Timeframe = ((Int32)(DateTime.UtcNow.AddMinutes(-20).Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                    //string order = client.MakeRequest("GET_QueryHistoricalKline","BTCUSD","5",Timeframe,1);
                    ByBitRequest request = client.CreateRequest("GET_QueryHistoricalKline");
                    request.AddRequired("BTCUSD","5",Timeframe);
                    request["limit"] = 1;
                    string order = request.Execute();
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    File.WriteAllText("C:\\temp\\lol.json", order,new System.Text.UTF8Encoding());
                    Console.WriteLine("Current Candle Fetched!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("WRONG CHOICE ERROR!!!!!!!!!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
            }

        }
    }
}
