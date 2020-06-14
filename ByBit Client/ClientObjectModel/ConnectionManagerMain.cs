using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

//IMPLEMENT
//GET ORDER INFO
//CONDITIONAL, LIMIT, 
namespace ByBitClientLib.ClientObjectModel
{
    public partial class ConnectionManager
    {
        private ByBitClient client;
        public int RetryAmount;
        public int RetryEveryMilisec;
        public int RetryAmountLiquidation;
        public int RetryIntervalLiquidation;
        public long exchangeSynchronization;

        public ConnectionManager(ByBitClient client, int RetryAmount = 3, int RetryEveryMilisec = 500, int RetryAmountLiquidation = 10, int RetryIntervalLiquidation = 1000)
        {
            this.client = client;
            this.RetryAmount = RetryAmount;
            this.RetryEveryMilisec = RetryEveryMilisec;
            this.RetryAmountLiquidation = RetryAmountLiquidation;
            this.RetryIntervalLiquidation = RetryIntervalLiquidation;
            exchangeSynchronization = 0;
        }

        public void updateSynchronization()
        {
            long serverTime = (DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(getServerTime()))).ToUnixTimeMilliseconds();
            long client = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long diff;

            //Console.WriteLine("Serv: " + serverTime + "------------------" + "Client: " + client);

            if (client >= serverTime + 1000)
            {
                diff = serverTime - client;
                exchangeSynchronization = -2000;
            }

            //Console.WriteLine("Synchro "+ exchangeSynchronization);
        }

        public float getServerTime()
        {
            float serverTime;
            ByBitRequest request = client.CreateRequest("GET_ServerTime");
            String response = ExecuteWithRetry(request);
            JObject jResponse = JObject.Parse(response);
            serverTime = (float)jResponse["time_now"];
            return serverTime;
        }

        private String ExecuteWithRetry(ByBitRequest request, String retryType = "Standard")
        {
            String response = null;
            int retAmount = 0;
            int retryMili = 0;

            if (retryType.Equals("Standard"))
            {
                retAmount = RetryAmount;
                retryMili = RetryEveryMilisec;
            }else if(retryType.Equals("Liquidation"))
            {
                retAmount = RetryAmountLiquidation;
                retryMili = RetryIntervalLiquidation;
            }

            for (int i = 1; i <= retAmount; i++)
            {
                try
                {
                    request.Timestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + exchangeSynchronization);
                    response = request.Execute();
                    break;
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(retryMili);
                }
            }
            return response;
        }

        //Public methods, ORDERS Processing

        public String ChangeLeverage(String CryptoPair, int newLeverage)
        {
            ByBitRequest request = client.CreateRequest("POST_ChangeUserLeverage");
            request.AddRequired(CryptoPair, newLeverage.ToString());

            return ExecuteWithRetry(request);
        }

        //private methods Orders Procecessig 

        public Position GetPosition(String CryptoPair)
        {
            ByBitRequest requestPosition = client.CreateRequest("GET_MyPosition");
            requestPosition["symbol"] = CryptoPair;

            String Response = ExecuteWithRetry(requestPosition);

            return new Position(Response);
        }
    }
}
