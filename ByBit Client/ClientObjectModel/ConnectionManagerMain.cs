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
        public bool AutoRoundingMode;
        public int RetryAmount;
        public int RetryEveryMilisec;
        public int RetryAmountLiquidation;
        public int RetryIntervalLiquidation;
        public long exchangeSynchronization;

        public ConnectionManager(ByBitClient client) 
        {
            ConstructConnectionManager(client,0,0,0,0);
        }

        public ConnectionManager(ByBitClient client, int RetryAmount = 3, int RetryEveryMilisec = 500, int RetryAmountLiquidation = 10, int RetryIntervalLiquidation = 1000)
        {
            ConstructConnectionManager(client, RetryAmount, RetryEveryMilisec, RetryAmountLiquidation, RetryIntervalLiquidation);
        }

        private void ConstructConnectionManager(ByBitClient client, int RetryAmount, int RetryEveryMilisec, int RetryAmountLiquidation, int RetryIntervalLiquidation) 
        {
            this.client = client;
            exchangeSynchronization = 0;
            SetAutoRetry(RetryAmount, RetryEveryMilisec);
            SetLiquidationRetry(RetryAmountLiquidation, RetryIntervalLiquidation);

        }

        public void SetAutoRetry(int RetryAmount = 3, int RetryEveryMilisec = 500) 
        {
            this.RetryAmount = RetryAmount;
            this.RetryEveryMilisec = RetryEveryMilisec;
        }

        public void SetLiquidationRetry(int RetryAmountLiquidation = 10, int RetryIntervalLiquidation = 1000)
        {
            this.RetryAmountLiquidation = RetryAmountLiquidation;
            this.RetryIntervalLiquidation = RetryIntervalLiquidation;
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

        public DateTime getServerTime()
        {
            float serverTime;
            ByBitRequest request = client.InversePerpetual.CreateRequest("GET_ServerTime");
            String response = ExecuteWithRetry(request);
            JObject jResponse = JObject.Parse(response);
            serverTime = (float)jResponse["time_now"];
            return (DateTimeOffset.FromUnixTimeSeconds((Int32)serverTime)).UtcDateTime;
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
            }
            else if (retryType.Equals("Liquidation"))
            {
                retAmount = RetryAmountLiquidation;
                retryMili = RetryIntervalLiquidation;
            }

            for (int i = 0; i <= retAmount; i++)
            {
                try
                {
                    request.Timestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + exchangeSynchronization);
                    response = request.Execute();
                    break;
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if (i == retAmount)
                    {
                        throw e;
                    }
                    else 
                    {
                        Thread.Sleep(retryMili);
                    }
                }
            }
            return response;
        }
    }
}
