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

            return new Order(ExecuteWithRetry(request),this);
        }

        public Order LimitOrder(String CryptoPair,int Contracts, double entryPrice, bool reduceOnly = false, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Limit", (int)Math.Abs(Contracts), timeInForce.ToString());
            request["price"] = entryPrice.ToString();
            request["reduce_only"] = reduceOnly.ToString();

            return new Order(ExecuteWithRetry(request),this);
        }

        public String CancelActiveOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelActiveOrder");
            request.AddRequired(cryptoPair);
            request["order_id"] = orderId;

            return ExecuteWithRetry(request);
        }

        public String CancelAllActiveOrders(String cryptoPair)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelAllActiveOrders");
            request.AddRequired(cryptoPair);

            return ExecuteWithRetry(request);
        }

        public String UpdateLimitOrder(String cryptoPair, String orderId, Double newEntryPrice, Int32 newQuantity)
        {
            ByBitRequest request = client.CreateRequest("POST_ReplaceActiveOrder");
            request.AddRequired(orderId, cryptoPair);

            if (newEntryPrice != 0)
            {
                request["p_r_price"] = newEntryPrice.ToString();
            }

            if (newQuantity != 0)
            {
                request["p_r_qty"] = Math.Abs(newQuantity);
            }

            return ExecuteWithRetry(request);
        }


        public String UpdateLimitOrder(Order order, Double newEntryPrice, Int32 newQuantity)
        {
            return UpdateLimitOrder(order.CryptoPair, order.OrderId, newEntryPrice, newQuantity);
        }

        //GET_QueryActiveOrder
        public String QueryActiveOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("GET_QueryActiveOrder");
            request.AddRequired(orderId, cryptoPair);

            return ExecuteWithRetry(request);
        }


        //GET_GetActiveOrder
        public List<Order> GetActiveOrders(String cryptoPair)
        {
            List<Order> orderList = new List<Order>();
            Int32 currentPage = 0;
            Int32 lastPage = 0;

            String firstResponse = ActiveOrdersQuery(cryptoPair, 1);
            JObject firstResponseJson = JObject.Parse(firstResponse);
            currentPage = (int)firstResponseJson["result"]["current_page"];
            lastPage = (int)firstResponseJson["result"]["last_page"];

            for (int i = 1; i <= lastPage;i++)
            {
                if (i == 1)
                {
                    foreach (JObject orderObject in firstResponseJson["result"]["data"].Children())
                    {
                        orderList.Add(new Order(orderObject, this));
                    }
                }
                else
                {
                    String response = ActiveOrdersQuery(cryptoPair, i);
                    JObject responseJson = JObject.Parse(response);

                    foreach (JObject orderObject in responseJson["result"]["data"].Children())
                    {
                        orderList.Add(new Order(orderObject, this));
                    }
                }
            }

            return orderList;

        }

        private String ActiveOrdersQuery(String cryptoPair, Int32 page)
        {
            ByBitRequest request = client.CreateRequest("GET_GetActiveOrder");
            request["symbol"] = cryptoPair;
            request["page"] = page;
            //request["limit"] = 50;    //Used the default whichy is 20

            return ExecuteWithRetry(request);
        }

        public Order ConditionalMarketOrder(String CryptoPair, int Contracts, Double triggerPrice, Double beforeTriggerPrice, TriggerPriceType triggerBy = TriggerPriceType.LastPrice, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceConditionalOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Market", (int)Math.Abs(Contracts), beforeTriggerPrice.ToString(), triggerPrice.ToString(), timeInForce.ToString());

            if (triggerBy != TriggerPriceType.LastPrice)
            {
                request["trigger_by"] = triggerBy.ToString();
            }

            return new Order(ExecuteWithRetry(request), this);
        }

        public Order ConditionalLimitOrder(String CryptoPair,int Contracts, Double entryPrice, Double triggerPrice, Double beforeTriggerPrice, TriggerPriceType triggerBy = TriggerPriceType.LastPrice, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceConditionalOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Limit", (int)Math.Abs(Contracts), beforeTriggerPrice.ToString(), triggerPrice.ToString(), timeInForce.ToString());
            request["price"] = entryPrice.ToString();

            if (triggerBy != TriggerPriceType.LastPrice)
            {
                request["trigger_by"] = triggerBy.ToString();
            }

            return new Order(ExecuteWithRetry(request), this);
        }

        public String CancelConditionalOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelConditionalOrder");
            request.AddRequired(cryptoPair);
            request["stop_order_id"] = orderId;

            return ExecuteWithRetry(request);
        }

        public String CancelAllConditionalOrders(String cryptoPair)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelAllConditionalOrders");
            request.AddRequired(cryptoPair);

            return ExecuteWithRetry(request);
        }

        public String UpdateConditionalOrder(String cryptoPair, String orderId,Int32 newOrderQuantity, Double newTriggerPrice, Double newOrderPrice)
        {
            ByBitRequest request = client.CreateRequest("POST_ReplaceConditionalOrder");
            request.AddRequired(orderId, cryptoPair);

            if (newOrderQuantity != 0)
            {
                request["p_r_qty"] = Math.Abs(newOrderQuantity);
            }

            if (newOrderPrice != 0)
            {
                request["p_r_price"] = newOrderPrice.ToString();
            }

            if (newTriggerPrice != 0)
            {
                request["p_r_trigger_price"] = newTriggerPrice.ToString();
            }
            
            return ExecuteWithRetry(request);
        }

        public String UpdateConditionalOrder(Order order, Int32 newOrderQuantity, Double newTriggerPrice, Double newOrderPrice)
        {
            return UpdateConditionalOrder(order.CryptoPair, order.OrderId, newOrderQuantity, newTriggerPrice, newOrderPrice);
        }

        public List<Order> GetConditionalOrders(String cryptoPair)
        {
            List<Order> orderList = new List<Order>();
            Int32 currentPage = 0;
            Int32 lastPage = 0;

            String firstResponse = ConditionalOrdersQuery(cryptoPair, 1);
            JObject firstResponseJson = JObject.Parse(firstResponse);
            currentPage = (int)firstResponseJson["result"]["current_page"];
            lastPage = (int)firstResponseJson["result"]["last_page"];

            for (int i = 1; i <= lastPage; i++)
            {
                if (i == 1)
                {
                    foreach (JObject orderObject in firstResponseJson["result"]["data"].Children())
                    {
                        orderList.Add(new Order(orderObject, this));
                    }
                }
                else
                {
                    String response = ConditionalOrdersQuery(cryptoPair, i);
                    JObject responseJson = JObject.Parse(response);

                    foreach (JObject orderObject in responseJson["result"]["data"].Children())
                    {
                        orderList.Add(new Order(orderObject, this));
                    }
                }
            }

            return orderList;

        }

        private String ConditionalOrdersQuery(String cryptoPair, Int32 page)
        {
            ByBitRequest request = client.CreateRequest("GET_GetConditionalOrder");
            request["symbol"] = cryptoPair;
            request["page"] = page;
            //request["limit"] = 50;    //Used the default whichy is 20

            return ExecuteWithRetry(request);
        }

        public String QueryConditionalOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("GET_QueryActiveOrder");
            request.AddRequired(orderId, cryptoPair);

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

        public Boolean LiquidatePosition(String CryptoPair)
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

            //If position is still not closed then send a false, else send true
            if (PositionSize > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
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
