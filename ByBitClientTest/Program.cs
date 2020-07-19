using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ByBitClientLib;
using ByBitClientLib.ClientObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByBitClientTest
{
    class Program
    {
        static ByBitClient client;

        static void Main(string[] args)
        {
            Boolean isRunning;            

            string TemplateFilePath = args[0];

            //Load JSON template file containing Secret key,API Key and API_URL
            string JsonString = File.ReadAllText(TemplateFilePath, Encoding.UTF8);
            JObject Api_JSON = JObject.Parse(JsonString);
            string API_SECRET = (string)Api_JSON["API_SECRET"];
            string API_KEY = (string)Api_JSON["API_KEY"];
            string API_URL = (string)Api_JSON["API_URL"];

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
                Console.WriteLine("8 -- Check Server Time!");
                Console.WriteLine("9 -- RUN All CONNECTION MANAGER TESTS");
                Console.WriteLine("10 -- RUN Active Order Connection Manager Test");
                Console.WriteLine("11 -- RUN Conditional Order Connection Manager Test");

                string Choice;

                Choice = Console.ReadLine();

                if (Choice.Equals("1"))
                {
                    Console.Clear();
                    //String pair = new String("ETHUSD");
                    //ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
                    //request.AddRequired("Sell", pair, "Limit", 1, "GoodTillCancel");
                    //request["price"] = 10000.00;
                    ConnectionManager manager = new ConnectionManager(client);
                    String order = manager.LimitOrder("ETHUSD",10,231.9).Response;
                    //string order = request.Execute();
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Made an Order!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("2"))
                {
                    Console.Clear();
                    ByBitRequest request = client.CreateRequest("POST_CancelAllActiveOrders");
                    request.AddRequired("BTCUSD");
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
                    string order = client.MakeRequest("GET_MyPosition","BTCUSD");
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
                    //string order = client.MakeRequest("GET_QueryKline","BTCUSD","5",Timeframe,1);
                    ByBitRequest request = client.CreateRequest("GET_QueryKline");
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
                else if (Choice.Equals("8"))
                {
                    Console.Clear();
                    //string order = client.MakeRequest("GET_QueryKline","BTCUSD","5",Timeframe,1);
                    ByBitRequest request = client.CreateRequest("GET_ServerTime");
                    string order = request.Execute();
                    Console.WriteLine("-------------------RESPONSE---------------------");
                    Console.WriteLine(order);
                    Console.WriteLine("Server time Checked!!");
                    Console.WriteLine("Go Back? Press Enter");
                    Console.ReadLine();
                }
                else if (Choice.Equals("9"))
                {
                    ActiveordersConnectionManagerTest(2000);
                    ConditionalOrdersConnectionManagerTest(2000);
                }
                else if (Choice.Equals("10"))
                {
                    ActiveordersConnectionManagerTest(2000);
                }
                else if (Choice.Equals("11"))
                {
                    ConditionalOrdersConnectionManagerTest(2000);
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

        public static void ActiveordersConnectionManagerTest(int delay)
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            StringBuilder log = new StringBuilder();


            //1st Test Market Order, with different Params
            log.AppendLine(manager.MarketOrder(crypto, 1,false,ConnectionManager.TimeInForce.GoodTillCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.MarketOrder(crypto, 2, false, ConnectionManager.TimeInForce.FillOrKill).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.MarketOrder(crypto, -5, false, ConnectionManager.TimeInForce.PostOnly).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.MarketOrder(crypto, 1,true, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);

            //2nd Test, Close Position
            log.AppendLine(manager.LiquidatePosition(crypto).ToString());
            Thread.Sleep(delay);
            log.AppendLine(manager.MarketOrder(crypto, 1, false, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LiquidatePosition(crypto).ToString());
            Thread.Sleep(delay);

            //Required for reduce only Limit Orders
            log.AppendLine(manager.MarketOrder(crypto, 5, false, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);

            //3rd Test, Limit Order, with Different Params
            Order first = manager.LimitOrder(crypto, -10, 500, false, ConnectionManager.TimeInForce.GoodTillCancel);
            log.AppendLine(first.Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, -10, 550.55, false, ConnectionManager.TimeInForce.FillOrKill).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, -2, 280.35, true, ConnectionManager.TimeInForce.GoodTillCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, -10, 450.25, true, ConnectionManager.TimeInForce.PostOnly).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, 10, 100.20, false, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            Order last = manager.LimitOrder(crypto, 10, 100.85, false, ConnectionManager.TimeInForce.GoodTillCancel);
            log.AppendLine(last.Response);
            Thread.Sleep(delay);

            //4th test, Querieng active order test
            log.AppendLine("----------------QUERY ACTIVE ORDER TEST--------------------");
            log.AppendLine(manager.QueryActiveOrder(first.CryptoPair, first.OrderId));
            log.AppendLine("---END query Active Order TEST--------");


            //5th Test, Cancel the 1st Limit Order and the last Limit Orders
            log.AppendLine(manager.CancelActiveOrder(first.CryptoPair,first.OrderId));
            Thread.Sleep(delay);
            log.AppendLine(manager.CancelActiveOrder(last.CryptoPair, last.OrderId));
            Thread.Sleep(delay);


            //6th Test, Cancel All Active Orders and then close position
            log.AppendLine(manager.CancelAllActiveOrders(first.CryptoPair));
            Thread.Sleep(delay);
            log.AppendLine(manager.LiquidatePosition(crypto).ToString());

            //7th test, Create 65 Limit Orders and Check active Order Data
            for (int i = 0; i < 65; i++)
            {
                manager.LimitOrder(crypto, 1, 70 + i, false, ConnectionManager.TimeInForce.GoodTillCancel);
                Thread.Sleep(100);
            }
            List<Order> activeOrderList = manager.GetActiveOrders(crypto);
            log.AppendLine("----------------GET ACTIVE ORDER TEST LIST----------------------------------------");

            foreach (Order orderData in activeOrderList)
            {
                log.AppendLine(orderData.Response);
            }

            manager.CancelAllActiveOrders(crypto);
            log.AppendLine("----------------END OF ACTIVE ORDER DATA TESTS-------------------------------");

            Console.WriteLine(log.ToString());
        }

        public static void ConditionalOrdersConnectionManagerTest(int delay)
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            StringBuilder log = new StringBuilder();

            //1st Test, Conditional MarketOrder, with different Params
            log.AppendLine(manager.ConditionalMarketOrder(crypto, 1, 450,400,ConnectionManager.TriggerPriceType.IndexPrice,ConnectionManager.TimeInForce.GoodTillCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalMarketOrder(crypto, 1, 400, 380, ConnectionManager.TriggerPriceType.LastPrice, ConnectionManager.TimeInForce.FillOrKill).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalMarketOrder(crypto, -1, 70, 80, ConnectionManager.TriggerPriceType.MarkPrice, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalMarketOrder(crypto, -1, 70, 100, ConnectionManager.TriggerPriceType.IndexPrice, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);



            Console.WriteLine(log.ToString());
        }
    }
}
