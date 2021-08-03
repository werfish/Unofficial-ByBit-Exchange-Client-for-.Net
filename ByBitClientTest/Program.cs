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
        static String TestLogFolderPath;

        static void Main(string[] args)
        {
            Boolean isRunning;            

            String TemplateFilePath = args[0];
            TestLogFolderPath = args[1];

            //Load JSON template file containing Secret key,API Key and API_URL
            String JsonString = File.ReadAllText(TemplateFilePath, Encoding.UTF8);
            JObject Api_JSON = JObject.Parse(JsonString);
            String API_SECRET = (String)Api_JSON["API_SECRET"];
            String API_KEY = (String)Api_JSON["API_KEY"];
            String API_URL = (String)Api_JSON["API_URL"];

            if (!TestLogFolderPath.Equals(String.Empty)) 
            {
                if (!Directory.Exists(TestLogFolderPath))
                {
                    throw new Exception("Please create the folder set as the TestLogFolderPath");
                }
            }

            isRunning = true;
            client = new ByBitClient(API_KEY,API_SECRET,API_URL, "5000");

            while (isRunning)
            {
                Console.Clear();
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
                Console.WriteLine("12 -- RUN Position Connection Manger Test - IN PROGRESS");
                Console.WriteLine("13 -- RUN Exchange data Connection Manager Test - IN PROGRESS");
                Console.WriteLine("14 -- RUN Wallet data Connection Manager Test - IN PROGRESS");

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
                    String order = manager.MarketOrder("ETHUSD",1).Response;
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
                    request.AddRequired("ETHUSD");
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
                    string order = client.MakeRequest("GET_MyPosition","ETHUSD");
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
                    string order = client.MakeRequest("POST_ChangeUserLeverage","ETHUSD", "10");
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
                    PositionConnectionManagerTest(2000);
                    ExchangeDataConnectionManagerTest(2000);
                    WalletDataConnectionManagerTest(2000);
                }
                else if (Choice.Equals("10"))
                {
                    ActiveordersConnectionManagerTest(2000);
                }
                else if (Choice.Equals("11"))
                {
                    ConditionalOrdersConnectionManagerTest(2000);
                }
                else if (Choice.Equals("12"))
                {
                    PositionConnectionManagerTest(2000);
                }
                else if (Choice.Equals("13"))
                {
                    ExchangeDataConnectionManagerTest(2000);
                }
                else if (Choice.Equals("14"))
                {
                    WalletDataConnectionManagerTest(2000);
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

        public static void WriteToLogFile(StringBuilder logs, String Test) 
        {
            String LogFileName = TestLogFolderPath + "/BybitTest_" + Test + "__" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".txt";
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(LogFileName))
            {
                sw.Write(logs.ToString());
            }
        }

        public static void ActiveordersConnectionManagerTest(int delay)
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            dynamic info = JObject.Parse(manager.GetLatestInfo(crypto));
            double price = info.result[0].ask_price;
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
            //log.AppendLine(manager.LiquidatePosition(crypto).ToString());
            Thread.Sleep(delay);
            log.AppendLine(manager.MarketOrder(crypto, 1, false, ConnectionManager.TimeInForce.GoodTillCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LiquidatePosition(crypto).ToString());
            Thread.Sleep(delay);

            //Required for reduce only Limit Orders
            log.AppendLine(manager.MarketOrder(crypto, 40).Response);
            Thread.Sleep(delay);

            //3rd Test, Limit Order, with Different Params
            Order first = manager.LimitOrder(crypto, -10, price + 546.35 , false, ConnectionManager.TimeInForce.GoodTillCancel);
            log.AppendLine(first.Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, -10, price + 392.85, false, ConnectionManager.TimeInForce.FillOrKill).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, -2, price + 450.2, true, ConnectionManager.TimeInForce.GoodTillCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, -10, price + 817.8, true, ConnectionManager.TimeInForce.PostOnly).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.LimitOrder(crypto, 10, price - 417.45, false, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            Order last = manager.LimitOrder(crypto, 10, price - 546.35, false, ConnectionManager.TimeInForce.GoodTillCancel);
            log.AppendLine(last.Response);
            Thread.Sleep(delay);

            //4th Test, Update Active Order (does not make sense for a market order)
            log.AppendLine(manager.UpdateLimitOrder(first, price + 1000, 0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateLimitOrder(first.CryptoPair,first.OrderId, 0,-5));
            Thread.Sleep(delay);
            //log.AppendLine(manager.UpdateLimitOrder(last, 100, 0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateLimitOrder(last.CryptoPair, last.OrderId, 0, 1));
            Thread.Sleep(delay);

            //5th test, Querieng active order test
            log.AppendLine("----------------QUERY ACTIVE ORDER TEST--------------------");
            log.AppendLine(manager.QueryActiveOrder(first.CryptoPair, first.OrderId));
            log.AppendLine("---END query Active Order TEST--------");


            //6th Test, Cancel the 1st Limit Order and the last Limit Orders
            log.AppendLine(manager.CancelActiveOrder(first.CryptoPair,first.OrderId));
            Thread.Sleep(delay);
            log.AppendLine(manager.CancelActiveOrder(last.CryptoPair, last.OrderId));
            Thread.Sleep(delay);


            //7th Test, Cancel All Active Orders and then close position
            log.AppendLine(manager.CancelAllActiveOrders(first.CryptoPair));
            Thread.Sleep(delay);
            log.AppendLine(manager.LiquidatePosition(crypto).ToString());

            //8th test, Create 65 Limit Orders and Check active Order Data
            for (int i = 0; i < 65; i++)
            {
                manager.LimitOrder(crypto, 1, (price - 400) + i, false, ConnectionManager.TimeInForce.GoodTillCancel);
                Thread.Sleep(100);
            }

            log.AppendLine("----------------GET ACTIVE ORDER TEST LIST----------------------------------------");
            Thread.Sleep(1000);
            List<Order> activeOrderList = manager.GetActiveOrderList(crypto, new ConnectionManager.OrderStatus[] { ConnectionManager.OrderStatus.New });
            log.AppendLine("Count of Orders : " + activeOrderList.Count);
            foreach (Order orderData in activeOrderList)
            {
                log.AppendLine(orderData.Response);
            }

            manager.CancelAllActiveOrders(crypto);
            log.AppendLine("----------------END OF ACTIVE ORDER DATA TESTS-------------------------------");

            //9th test, Create 5 Limit Orders and QueryActiveOrders
            for (int i = 0; i <= 5; i++)
            {
                manager.LimitOrder(crypto, 1, (price - 450) + i, false, ConnectionManager.TimeInForce.GoodTillCancel);
                Thread.Sleep(100);
            }
            List<Order> activeOrderQueryList = manager.QueryActiveOrders(crypto);
            log.AppendLine("----------------QUERY ACTIVE ORDER TEST LIST----------------------------------------");
            log.AppendLine("Count of Orders : " + activeOrderList.Count);
            foreach (Order orderData in activeOrderQueryList)
            {
                log.AppendLine(orderData.Response);
            }
            manager.CancelAllActiveOrders(crypto);
            log.AppendLine("----------------END OF QUERY ACTIVE ORDER DATA TESTS-------------------------------");

            Console.WriteLine(log.ToString());
            WriteToLogFile(log, "ActiveOrder");
        }

        public static void ConditionalOrdersConnectionManagerTest(int delay)
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            dynamic info = JObject.Parse(manager.GetLatestInfo(crypto));
            double price = info.result[0].ask_price;
            StringBuilder log = new StringBuilder();

            //1st Test, Conditional MarketOrder, with different Params
            Order first = manager.ConditionalMarketOrder(crypto, 1, price + 200.85, price + 150.7, ConnectionManager.TriggerPriceType.IndexPrice, ConnectionManager.TimeInForce.GoodTillCancel);
            log.AppendLine(first.Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalMarketOrder(crypto, 1, price - 200.5, price - 180.45, ConnectionManager.TriggerPriceType.LastPrice, ConnectionManager.TimeInForce.FillOrKill).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalMarketOrder(crypto, -1, price + 200.5, price + 190.5, ConnectionManager.TriggerPriceType.MarkPrice, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            Order last = manager.ConditionalMarketOrder(crypto, -1, price - 400.35, price - 350.8, ConnectionManager.TriggerPriceType.IndexPrice, ConnectionManager.TimeInForce.ImmediateOrCancel);
            log.AppendLine(last.Response);
            Thread.Sleep(delay);

            //2nd Test, Update ConditionalMarket order with new information
            log.AppendLine(manager.UpdateConditionalOrder(first, 0, price + 237.35, 0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(first.CryptoPair, first.OrderId, 2, 0,0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(first.CryptoPair, first.OrderId, 10, price + 180.5, 0));
            Thread.Sleep(5000);
            log.AppendLine(manager.UpdateConditionalOrder(last, -2, 0, 0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(last.CryptoPair,last.OrderId, 5, price - 385.5, 0)); //Updating Conditional order with a price Higher than Market will make it fill
            Thread.Sleep(delay);

            manager.LiquidatePosition(crypto);

            //3rd Test, Cancel the 1st Condtional Limit Order and the last Conditional Limit Orders
            log.AppendLine(manager.CancelConditionalOrder(first.CryptoPair, first.OrderId));
            Thread.Sleep(delay);
            log.AppendLine(manager.CancelConditionalOrder(last.CryptoPair, last.OrderId));
            Thread.Sleep(delay);


            //4th Test, Cancel All Conditional Order
            log.AppendLine(manager.CancelAllConditionalOrders(first.CryptoPair));
            Thread.Sleep(delay);


            //5th Test, Limit Order, with Different Params
            Order firstLimit = manager.ConditionalLimitOrder(crypto, -1,price - 300.55, price - 350, price - 320, ConnectionManager.TriggerPriceType.LastPrice, ConnectionManager.TimeInForce.GoodTillCancel);
            log.AppendLine(firstLimit.Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalLimitOrder(crypto, -1, price + 380.3, price + 350, price + 320.35, ConnectionManager.TriggerPriceType.MarkPrice, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalLimitOrder(crypto, -1, price - 100,price + 320, price + 310, ConnectionManager.TriggerPriceType.IndexPrice, ConnectionManager.TimeInForce.GoodTillCancel).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalLimitOrder(crypto, 1, price - 100,  price - 120, price - 115.65, ConnectionManager.TriggerPriceType.LastPrice, ConnectionManager.TimeInForce.FillOrKill).Response);
            Thread.Sleep(delay);
            log.AppendLine(manager.ConditionalLimitOrder(crypto, 1, price +  + 250, price +  + 320, price + 310, ConnectionManager.TriggerPriceType.IndexPrice, ConnectionManager.TimeInForce.ImmediateOrCancel).Response);
            Thread.Sleep(delay);
            Order lastLimit = manager.ConditionalLimitOrder(crypto, 1, price + 300, price + 280, price + 270, ConnectionManager.TriggerPriceType.IndexPrice, ConnectionManager.TimeInForce.FillOrKill);
            log.AppendLine(lastLimit.Response);
            Thread.Sleep(delay);

            //6th Test, Update ConditionalLimit order with new information
            log.AppendLine(manager.UpdateConditionalOrder(firstLimit, -5, 0, 0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(firstLimit.CryptoPair, firstLimit.OrderId, 0, price - 250.5, price - 300));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(firstLimit.CryptoPair, firstLimit.OrderId, -10, price -  420, price -  400));
            Thread.Sleep(5000);
            log.AppendLine(manager.UpdateConditionalOrder(lastLimit, 2,price + 350, 0));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(lastLimit.CryptoPair, lastLimit.OrderId, 0, price + 460, price - 340));
            Thread.Sleep(delay);
            log.AppendLine(manager.UpdateConditionalOrder(lastLimit.CryptoPair, lastLimit.OrderId, 5, price +  + 260, price + 100));
            Thread.Sleep(delay);

            //7th test, Querieng Conditional order test
            log.AppendLine("----------------QUERY Conditional ORDER TEST--------------------");
            log.AppendLine(manager.QueryConditionalOrder(first.CryptoPair, first.OrderId));
            log.AppendLine("---END query Conditional Order TEST--------");

            log.AppendLine("----------------GET CONDITIONAL ORDER TEST LIST----------------------------------------");

            List<Order> conditionalOrderList = manager.GetConditionalOrderList(crypto);

            foreach (Order orderData in conditionalOrderList)
            {
                log.AppendLine(orderData.Response);
            }

            manager.CancelAllConditionalOrders(crypto);
            log.AppendLine("----------------END OF CONDTIONAL ORDER DATA TESTS-------------------------------");

            Console.WriteLine(log.ToString());
            WriteToLogFile(log, "ConditionalOrder");
        }

        public static void PositionConnectionManagerTest(int delay) 
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            StringBuilder log = new StringBuilder();

            //1st Test, Get Position Information, Position Object amd Refresh method test
            log.AppendLine("TEST 1 ----------------------------------");
            log.AppendLine("GET POSITION");
            Position pos = manager.GetPosition(crypto);
            log.AppendLine("Leverage :" + pos.Leverage.ToString());
            log.AppendLine("Wallet Balance :" + pos.WalletBalance.ToString());
            log.AppendLine("Position Margin :" + pos.PositionMargin.ToString());
            log.AppendLine("Entry Price :" + pos.EntryPrice.ToString());

            log.AppendLine("GET POSITION After Buying, test Refreshing");
            manager.GoLong(crypto, 1);
            Thread.Sleep(delay);
            pos.Refresh();
            log.AppendLine("Leverage :" + pos.Leverage.ToString());
            log.AppendLine("Wallet Balance :" + pos.WalletBalance.ToString());
            log.AppendLine("Position Margin :" + pos.PositionMargin.ToString());
            log.AppendLine("Entry Price :" + pos.EntryPrice.ToString());

            log.AppendLine("TEST 1  END----------------------------------");

            //2nd Test, Change Leverage of the Position, Check the Leverage Method of the Position Oobject
            log.AppendLine("TEST 2 ----------------------------------");
            Int32 leverage = pos.Leverage;
            log.AppendLine("Leverage :" + leverage);
            log.AppendLine("Change leverage to leverage minus 1");
            pos.SetLeverage(leverage - 1);
            Thread.Sleep(delay);
            leverage = pos.Leverage;
            log.AppendLine("Leverage :" + leverage);
            pos.SetLeverage(leverage + 1);
            log.AppendLine("TEST 2  END----------------------------------");


            //3rd test, Get all positions
            log.AppendLine("TEST 3 ----------------------------------");
            var Positions = manager.GetPositions();
            log.AppendLine("Positions count: " + Positions.Count.ToString());
            foreach (Position position in Positions.Values)
            {
                log.AppendLine("Symbol :" + position.Symbol.ToString());
                log.AppendLine("Leverage :" + position.Leverage.ToString());
                log.AppendLine("Wallet Balance :" + position.WalletBalance.ToString());
                log.AppendLine("Position Margin :" + position.PositionMargin.ToString());
                log.AppendLine("Entry Price :" + position.EntryPrice.ToString());
            }
            manager.LiquidatePosition(crypto);
            log.AppendLine("TEST 3  END----------------------------------");

            //4th test, Check profit and Loss Statistics of the previously closed position with a Lookback
            log.AppendLine("TEST 4 ----------------------------------");
            List<PnlRecord> third =  manager.GetProfitLossList(crypto, 20);
            log.AppendLine(third.Count.ToString());
            foreach (PnlRecord record in third)
            {
                log.AppendLine(record.ToString());
            }
            log.AppendLine("TEST 4  END----------------------------------");

            //5th test, Check profit and Loss Statistics of the previously closed position
            log.AppendLine("TEST 5 ----------------------------------");
            third = manager.GetProfitLossList(crypto,DateTime.Now.Subtract(TimeSpan.FromTicks(new TimeSpan(24,0,0).Ticks)),DateTime.Now, ConnectionManager.ExecType.NONE, 20);
            log.AppendLine(third.Count.ToString());
            foreach (PnlRecord record in third)
            {
                log.AppendLine(record.ToString());
            }
            log.AppendLine("TEST 5  END----------------------------------");

            //6th test, Test The Stop Loss
            log.AppendLine("TEST 6 STOP LOSS ----------------------------------");
            Order order = manager.Buy(crypto, 1);
            Thread.Sleep(delay);
            dynamic response = JObject.Parse(manager.SetStopLoss(crypto, Math.Round((Double)order.Price * 0.98,0)));
            Thread.Sleep(delay);
            manager.LiquidatePosition(crypto);

            log.AppendLine("TEST 6  END----------------------------------");

            WriteToLogFile(log, "Position");
        }

        public static void ExchangeDataConnectionManagerTest(int delay)
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            StringBuilder log = new StringBuilder();

            //1st Test, Get KLine candles, Test Single query, test Lookback query and Candle. to String Method
            log.AppendLine("TEST 1 --------------------------");
            List<Candle> first = manager.QueryKLine(crypto, ConnectionManager.Interval.M5, 2000);
            log.AppendLine(first.Count.ToString());

            foreach (Candle candle in first) 
            {
                log.AppendLine(candle.ToString());
            }
            log.AppendLine("TEST 1 END-----------------------------------------------------");

            //2nd Test, Get KLine candles, Test Descending sorting
            log.AppendLine("TEST 2 --------------------------");
            first = manager.QueryKLine(crypto, ConnectionManager.Interval.M5, 450,true);
            log.AppendLine(first.Count.ToString());

            foreach (Candle candle in first)
            {
                log.AppendLine(candle.ToString());
            }
            log.AppendLine("TEST 2 END-----------------------------------------------------");

            //3rd Test, Get KLine candles below at 200
            log.AppendLine("TEST 3 --------------------------");
            first = manager.QueryKLine(crypto, ConnectionManager.Interval.M60, 200);
            log.AppendLine(first.Count.ToString());

            foreach (Candle candle in first)
            {
                log.AppendLine(candle.ToString());
            }
            log.AppendLine("TEST 3 END-----------------------------------------------------");

            //4th Test, Get KLine candles below 200
            log.AppendLine("TEST 4 --------------------------");
            first = manager.QueryKLine(crypto, ConnectionManager.Interval.D, 80, true);
            log.AppendLine(first.Count.ToString());

            foreach (Candle candle in first)
            {
                log.AppendLine(candle.ToString());
            }
            log.AppendLine("TEST 4 END-----------------------------------------------------");

            //5th Test, Query Symbols
            log.AppendLine("TEST 5 --------------------------");
            log.AppendLine("Query Symbols");
            Dictionary<String, SymbolInfo> symbols = manager.QuerySymbol();
            log.AppendLine(symbols.Count.ToString());

            foreach (SymbolInfo symbol in symbols.Values)
            {
                log.AppendLine(symbol.response);
            }
            log.AppendLine("TEST 5 END-----------------------------------------------------");

            WriteToLogFile(log, "ExchangeData");
        }

        public static void WalletDataConnectionManagerTest(int delay)
        {
            ConnectionManager manager = new ConnectionManager(client);
            String crypto = "ETHUSD";
            ConnectionManager.Currency coin = ConnectionManager.Currency.ETH;
            StringBuilder log = new StringBuilder();

            //1st test, Check the Equity of Ethereum
            log.AppendLine("TEST 1 ----------------------------------");
            Decimal equity = manager.GetPairEquity(coin.ToString());
            log.AppendLine(coin.ToString() + " equity: " + equity);
            log.AppendLine("TEST 1  END----------------------------------");


            //2nd test, Check the Equity of All Crypto in the account
            log.AppendLine("TEST 2 ----------------------------------");
            Dictionary<String,Decimal> coins = manager.GetAllPairEquity();
            log.AppendLine("Count of Coins: " + coins.Count.ToString());
            foreach (String coinStr in coins.Keys)
            {
                log.AppendLine(coinStr + " equity: " + coins[coinStr]);
            }
            log.AppendLine("TEST 2  END----------------------------------");

            //3RD test, Check the Equity of Ethereum
            log.AppendLine("TEST 3 ----------------------------------");
            Decimal equityEth = manager.GetPairEquity(coin);
            Decimal equityLink = manager.GetPairEquity(ConnectionManager.Currency.USDT);
            Decimal equityUsdt = manager.GetPairEquity(ConnectionManager.Currency.XRP);
            log.AppendLine(coin.ToString() + " equity: " + equityEth);
            log.AppendLine("USDT" + " equity: " + equityLink);
            log.AppendLine("XRP" + " equity: " + equityUsdt);
            log.AppendLine("TEST 3  END----------------------------------");


            WriteToLogFile(log, "WalletData");
        }
    }
}
