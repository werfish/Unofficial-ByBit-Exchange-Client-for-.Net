using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public partial class ConnectionManager
    {

        public Dictionary<String, SymbolInfo> QuerySymbol() 
        {
            ByBitRequest request = client.CreateRequest("GET_QuerySymbol");

            Dictionary<String, SymbolInfo> symbols = new Dictionary<String, SymbolInfo>();
            String response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);

            foreach (JObject symbol in responseJson["result"].Children())
            {
                SymbolInfo newSymbol = new SymbolInfo(symbol.ToString(), this);
                symbols.Add((String)symbol["name"], newSymbol);
            }

            return symbols;
        }

        public String GetLatestInfo(String CryptoPair) //, DateTime startTime, DateTime endTime, ExecType executionType, Int32 page, Int32 limit) 
        {
            ByBitRequest request = client.CreateRequest("GET_LatestInformationForSymbol");
            request["symbol"] = CryptoPair;

            return ExecuteWithRetry(request); ;
        }
        public List<Candle> QueryKLine(String cryptoPair, Interval interval, int Lookback = 1, bool orderDescending = false)
        {
            List<Candle> Candles = new List<Candle>();
            Int32 currentPage = 0;
            Int32 pages = 0;
            Int32 remainder;
            TimeSpan intervalSpan;
            Int32 CandlesCounter = 0;
            DateTime QueryStart;

            if (Lookback < 1)
            {
                throw new Exception("Lookback cannot be below 1.");
            }

            //Calculate the timespan object for the interval used later
            if (interval.ToString().Length >= 2)
            {
                intervalSpan = new TimeSpan(0, (Int32)interval, 0);
            } else if (interval.ToString().Equals("D"))
            {
                intervalSpan = new TimeSpan(24, 0, 0);
            } else if (interval.ToString().Equals("W"))
            {
                intervalSpan = new TimeSpan(168, 0, 0);
            }
            else
            {
                throw new Exception("This method does not support Month");
            }

            pages = Lookback / 200;
            remainder = Lookback % 200;

            if (remainder != 0) 
            {
                pages += 1;
            }

            QueryStart = getServerTime();
            List<Candle> responseList;

            do
            {
                currentPage += 1;
                DateTime PageStartDate;

                if (currentPage == pages && remainder != 0)
                {                 
                    PageStartDate = QueryStart.Subtract(TimeSpan.FromTicks(((CandlesCounter + remainder + 1) * intervalSpan.Ticks)));
                    responseList = QueryKLine(cryptoPair, interval, PageStartDate, remainder);
                }
                else 
                {
                    CandlesCounter += 200;
                    PageStartDate = QueryStart.Subtract(TimeSpan.FromTicks(((CandlesCounter + 1) * intervalSpan.Ticks)));
                    responseList = QueryKLine(cryptoPair, interval, PageStartDate, 200);
                }
                Candles.AddRange(responseList);

            } while (currentPage < pages);

            if (orderDescending)
            {
                return Candles.OrderByDescending(c => c.OpenTime).ToList();
            }
            else 
            {
                return Candles.OrderBy(c => c.OpenTime).ToList();
            }
        }

        public List<Candle> QueryKLine(String CryptoPair, Interval interval, DateTime? from, Int32 limit = 200) //, DateTime startTime, DateTime endTime, ExecType executionType, Int32 page, Int32 limit) 
        {
            ByBitRequest request = client.CreateRequest("GET_QueryKline");
            String response;

            request.AddRequired(CryptoPair,this.parseInterval(interval), ((DateTimeOffset)from).ToUnixTimeSeconds());
            request["limit"] = limit;

            response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);
            List<Candle> CandlesList = new List<Candle>();

            foreach (JObject candle in responseJson["result"].Children())
            {
                CandlesList.Add(new Candle(candle, this));
            }

            return CandlesList;
        }
    }
}
