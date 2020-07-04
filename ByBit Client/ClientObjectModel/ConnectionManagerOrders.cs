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

        public Order ConditionalMarketOrder(String CryptoPair, int Contracts, Decimal triggerPrice, Decimal beforeTriggerPrice, TriggerPriceType triggerBy = TriggerPriceType.LastPrice, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceConditionalOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Market", (int)Math.Abs(Contracts), beforeTriggerPrice, triggerPrice, timeInForce.ToString());

            if (triggerBy != TriggerPriceType.LastPrice)
            {
                request["trigger_by"] = triggerBy.ToString();
            }

            return new Order(ExecuteWithRetry(request));
        }

        public Order ConditionalLimitOrder(String CryptoPair,int Contracts, Decimal entryPrice, Decimal triggerPrice, Decimal beforeTriggerPrice, TriggerPriceType triggerBy, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceConditionalOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Limit", (int)Math.Abs(Contracts), beforeTriggerPrice, triggerPrice, timeInForce.ToString());
            request["price"] = entryPrice;

            if (triggerBy != TriggerPriceType.LastPrice)
            {
                request["trigger_by"] = triggerBy.ToString();
            }

            return new Order(ExecuteWithRetry(request));
        }

        public String CancelConditionalOrder(String CryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelConditionalOrder");
            request.AddRequired(CryptoPair);
            request["stop_order_id"] = orderId;

            return ExecuteWithRetry(request);
        }

        public String UpdateConditionalOrder(String CryptoPair, String orderId,int newOrderQuantity, Decimal newOrderPrice, Decimal newTriggerPrice)
        {
            ByBitRequest request = client.CreateRequest("POST_ReplaceConditionalOrder");
            request.AddRequired(orderId, CryptoPair);

            if (newOrderQuantity != 0)
            {
                request["p_r_qty"] = newOrderQuantity;
            }

            if (newOrderPrice != 0)
            {
                request["p_r_price"] = newOrderPrice;
            }

            if (newTriggerPrice != 0)
            {
                request["p_r_trigger_price"] = newTriggerPrice;
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

        public String SetTrailingStopLoss(String CryptoPair, Decimal usdAmount)
        {
            ByBitRequest request = client.CreateRequest("POST_SetTrading-Stop");
            request.AddRequired(CryptoPair);
            request["trailing_stop"] = usdAmount;

            return ExecuteWithRetry(request);
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
