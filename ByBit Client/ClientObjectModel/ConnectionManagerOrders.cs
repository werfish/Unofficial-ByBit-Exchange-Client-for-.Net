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
        //Public methods, ORDERS Processing

        public Order MarketOrder(String CryptoPair, int Contracts, bool reduceOnly = false, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Market", (int)Math.Abs(Contracts), timeInForce.ToString());
            request["reduce_only"] = reduceOnly.ToString();

            return new Order(ExecuteWithRetry(request));
        }

        public Order LimitOrder(String CryptoPair,int Contracts, Decimal entryPrice, bool reduceOnly = false, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Limit", (int)Math.Abs(Contracts), timeInForce.ToString());
            request["price"] = entryPrice;
            request["reduce_only"] = reduceOnly.ToString();

            return new Order(ExecuteWithRetry(request));
        }

        public String UpdateLimitOrder(Order order, Decimal newEntryPrice, Decimal newQuantity)
        {
            ByBitRequest request = client.CreateRequest("POST_ReplaceActiveOrder");
            request.AddRequired(order.OrderId, order.CryptoPair);

            if (newEntryPrice != 0m)
            {
                request["p_r_price"] = newEntryPrice;
            }

            if (newQuantity != 0)
            {
                request["p_r_qty"] = newQuantity;
            }

            return ExecuteWithRetry(request);
        }

        public String SetStopLoss(String CryptoPair, Decimal stopPrice)
        {
            ByBitRequest request = client.CreateRequest("POST_SetTrading-Stop");
            request.AddRequired(CryptoPair);
            request["stop_loss"] = (float)stopPrice;

            return ExecuteWithRetry(request);
        }

        public Order SetTrailingStopLoss(String CryptoPair, Decimal usdAmount)
        {
            ByBitRequest request = client.CreateRequest("POST_SetTrading-Stop");
            request.AddRequired(CryptoPair);
            request["trailing_stop"] = usdAmount;

            return new Order(ExecuteWithRetry(request));
        }

        public void LiquidatePosition(String CryptoPair)
        {
            int PositionSize;
            String Side;
            do
            {
                ByBitRequest requestSize = client.CreateRequest("GET_MyPosition");
                requestSize["symbol"] = CryptoPair;

                String Response = ExecuteWithRetry(requestSize, "Liquidation");
                JObject json = JObject.Parse(Response);
                PositionSize = (int)json["result"]["size"];
                Side = (String)json["result"]["side"];

                if (Side.Equals("Sell"))
                {
                    Side = "Buy";
                }
                else if (Side.Equals("Buy"))
                {
                    Side = "Sell";
                }

                if (PositionSize != 0)
                {
                    ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
                    request.AddRequired(Side, CryptoPair, "Market", PositionSize, "GoodTillCancel");
                    request["reduce_only"] = "True";
                    ExecuteWithRetry(request, "Liquidation");
                }
                Thread.Sleep(RetryIntervalLiquidation);
            }
            while (PositionSize != 0);
        }

        //private methods Orders Procecessig

        private String GetSide(int contracts)
        {
            if (contracts > 0)
            {
                return "Buy";
            }
            else if (contracts < 0)
            {
                return "Sell";
            }
            else
            {
                Console.WriteLine("Contracts amount cannot be 0!!!!");
                return "Buy";
            }
        }
    }
}
