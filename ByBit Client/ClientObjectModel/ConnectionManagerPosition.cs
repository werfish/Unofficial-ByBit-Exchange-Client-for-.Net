using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ByBitClientLib.ClientObjectModel
{
    public partial class ConnectionManager
    {
        public Position GetPosition(String CryptoPair)
        {
            String response = GetPositionResponse(CryptoPair);
            JObject responseJson = JObject.Parse(response);
            return new Position((JObject)responseJson["result"], this);
        }

        internal String GetPositionResponse(String CryptoPair)
        {
            ByBitRequest requestPosition = client.CreateRequest("GET_MyPosition");
            requestPosition["symbol"] = CryptoPair;

            return ExecuteWithRetry(requestPosition);
        }

        public Dictionary<String, Position> GetPositions()
        {
            Dictionary<String, Position> positions = new Dictionary<String, Position>();

            ByBitRequest requestPosition = client.CreateRequest("GET_MyPosition");

            String response = ExecuteWithRetry(requestPosition);
            JObject responseJson = JObject.Parse(response);

            foreach (JObject position in responseJson["result"].Children()) 
            {
                Position newPosition = new Position((JObject)position["data"],this);
                positions.Add(newPosition.Symbol, newPosition);
            }

            return positions;
        }

        //Exchange Managed Conditional Order Stop Loss
        public String SetStopLoss(String CryptoPair, Double stopPrice)
        {
            ByBitRequest request = client.CreateRequest("POST_SetTradingStop");
            request.AddRequired(CryptoPair);
            request["stop_loss"] = stopPrice.ToString(CultureInfo.InvariantCulture);

            return ExecuteWithRetry(request);
        }

        public String SetTrailingStopLoss(String CryptoPair, Double usdAmount)
        {
            ByBitRequest request = client.CreateRequest("POST_SetTradingStop");
            request.AddRequired(CryptoPair);
            request["trailing_stop"] = usdAmount.ToString(CultureInfo.InvariantCulture);

            return ExecuteWithRetry(request);
        }

        public String SetLeverage(String CryptoPair, Int32 newLeverage) 
        {
            ByBitRequest request = client.CreateRequest("POST_SetLeverage");
            request.AddRequired(CryptoPair, newLeverage);

            return ExecuteWithRetry(request);
        }

        public List<PnlRecord> GetProfitLossList(String cryptoPair, ExecType executionType = ExecType.NONE, Int32 pageLimit = 50)
        {
            return GetProfitLossList(cryptoPair, null, null, 0, executionType, pageLimit);
        }

        public List<PnlRecord> GetProfitLossList(String cryptoPair, DateTime? startTime = null, DateTime? endTime = null, ExecType executionType = ExecType.NONE, Int32 pageLimit = 50 ) 
        {
            return GetProfitLossList(cryptoPair,startTime,endTime,0,executionType,pageLimit);
        }

        public List<PnlRecord> GetProfitLossList(String cryptoPair, int Lookback, ExecType executionType = ExecType.NONE, Int32 pageLimit = 50)
        {
            if(Lookback < 1 || Lookback > 2500) 
            {
                throw new Exception("Lookback cannot be below 1 or above 2500");
            }
            return GetProfitLossList(cryptoPair, null, null, Lookback, executionType, pageLimit);
        }

        public List<PnlRecord> GetProfitLossList(String cryptoPair, DateTime? startTime = null, DateTime? endTime = null, int Lookback = 0, ExecType executionType = ExecType.NONE, Int32 pageLimit = 50)
        {
            List<PnlRecord> PnlList = new List<PnlRecord>();
            Int32 pageNum = 0;
            Int32 responseCount;

            if (Lookback > 2500)
            {
                throw new Exception("Lookback cannot be above 2500");
            }

            do
            {
                responseCount = 0;
                pageNum += 1;

                String response = GetProfitLossQuery(cryptoPair, startTime, endTime, executionType, pageNum, pageLimit);
                JObject responseJson = JObject.Parse(response);

                Console.WriteLine(response);

                foreach (JObject pnlObject in responseJson["result"]["data"].Children())
                {
                    responseCount =+ 1;
                    PnlList.Add(new PnlRecord(pnlObject.ToString(), this));
                    if (PnlList.Count == Lookback && Lookback != 0) { break; }

                }

            } while ((PnlList.Count <= pageLimit || pageNum <= 50) && ((PnlList.Count < Lookback && Lookback > 0) || (Lookback == 0)) && (responseCount == pageLimit));

            return PnlList;
        }

        public String GetProfitLossQuery(String CryptoPair, DateTime? startTime = null, DateTime? endTime = null, ExecType executionType = ExecType.NONE, Int32 page = 1, Int32 limit = 50) //, DateTime startTime, DateTime endTime, ExecType executionType, Int32 page, Int32 limit) 
        {
            ByBitRequest request = client.CreateRequest("GET_ClosedProfitAndLoss");
            request.AddRequired(CryptoPair);

            if (startTime.HasValue) 
            {
                request["start_time"] = ((DateTimeOffset)startTime).ToUnixTimeSeconds();
            }
            if (endTime.HasValue)
            {
                request["end_time"] = ((DateTimeOffset)endTime).ToUnixTimeSeconds();
            }
            if (executionType != ExecType.NONE)
            {
                request["exec_type"] = executionType.ToString();
            }
                
            request["page"] = page;
            request["limit"] = limit;

            return ExecuteWithRetry(request);
        }
    }
}
