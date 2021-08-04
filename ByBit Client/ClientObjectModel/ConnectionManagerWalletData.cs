using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public partial class ConnectionManager
    {
        public Decimal GetPairEquity(String coin) 
        {
            ByBitRequest request= client.InversePerpetual.CreateRequest("GET_GetWalletBalance");
            request["coin"] = coin;

            String response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);

            return (Decimal)responseJson["result"][coin.ToString()]["equity"];
        }

        public Decimal GetPairEquity(Currency coin)
        {
            ByBitRequest request = client.InversePerpetual.CreateRequest("GET_GetWalletBalance");
            request["coin"] = coin.ToString();

            String response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);

            return (Decimal)responseJson["result"][coin.ToString()]["equity"];
        }

        public Dictionary<String,Decimal> GetAllPairEquity()
        {
            Dictionary<String, Decimal> pairsBalance = new Dictionary<String, Decimal>();

            ByBitRequest request = client.InversePerpetual.CreateRequest("GET_GetWalletBalance");

            String response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);

            foreach (JProperty coin in responseJson["result"].Children())
            {
                Decimal newValue = (Decimal)coin.Value["equity"];
                pairsBalance.Add((String)coin.Name, newValue);
            }

            return pairsBalance;
        }
    }
}
