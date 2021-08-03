using System;
using System.Collections.Generic;
using System.Globalization;
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

        public Order MarketOrder(String CryptoPair, int Contracts, bool reduceOnly = false, TimeInForce timeInForce = TimeInForce.GoodTillCancel, bool closeOntrigger = false)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Market", (int)Math.Abs(Contracts), timeInForce.ToString(), reduceOnly, closeOntrigger);

            return new Order(ExecuteWithRetry(request),this);
        }

        public Order GoLong(String CryptoPair, int Contracts)
        {
            return MarketOrder(CryptoPair, (int)Math.Abs(Contracts), false, TimeInForce.GoodTillCancel, false);
        }

        public Order GoShort(String CryptoPair, int Contracts)
        {
            return MarketOrder(CryptoPair, -(int)Math.Abs(Contracts), false, TimeInForce.GoodTillCancel, false);
        }

        public Order Buy(String CryptoPair, int Contracts)
        {
            return MarketOrder(CryptoPair, (int)Math.Abs(Contracts), false, TimeInForce.GoodTillCancel, false);
        }

        public Order Sell(String CryptoPair, int Contracts)
        {
            return MarketOrder(CryptoPair, -(int)Math.Abs(Contracts), false, TimeInForce.GoodTillCancel, false);
        }

        public Order LimitOrder(String CryptoPair,int Contracts, double entryPrice, bool reduceOnly = false, TimeInForce timeInForce = TimeInForce.GoodTillCancel, bool closeOntrigger = false)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceActiveOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Limit", (int)Math.Abs(Contracts), timeInForce.ToString(), reduceOnly, closeOntrigger);
            request["price"] = entryPrice.ToString(CultureInfo.InvariantCulture);

            return new Order(ExecuteWithRetry(request),this);
        }

        public String CancelActiveOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelActiveOrder");
            request.AddRequired(cryptoPair, orderId);

            return ExecuteWithRetry(request);
        }

        public String CancelAllActiveOrders(String cryptoPair)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelAllActiveOrders");
            request.AddRequired(cryptoPair);

            return ExecuteWithRetry(request);
        }

        public String UpdateLimitOrder(String cryptoPair, String orderId, Double newEntryPrice = 0, Int32 newQuantity = 0, Double takeProfit = 0, Double stopLoss = 0, TriggerPriceType tpTrigger = TriggerPriceType.LastPrice, TriggerPriceType slTrigger = TriggerPriceType.LastPrice)
        {
            ByBitRequest request = client.CreateRequest("POST_ReplaceActiveOrder");
            request.AddRequired(orderId, cryptoPair);

            request["tp_trigger_by"] = tpTrigger.ToString();
            request["sl_trigger_by"] = slTrigger.ToString();

            if (newEntryPrice != 0)
            {
                request["p_r_price"] = newEntryPrice.ToString(CultureInfo.InvariantCulture);
            }

            if (newQuantity != 0)
            {
                request["p_r_qty"] = Math.Abs(newQuantity);
            }

            if (takeProfit != 0)
            {
                request["take_profit"] = Math.Abs(newQuantity);
            }

            if (stopLoss != 0)
            {
                request["stop_loss"] = Math.Abs(newQuantity);
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
            request.AddRequired(cryptoPair, orderId);

            return ExecuteWithRetry(request);
        }

        public List<Order> QueryActiveOrders(String cryptoPair)
        {
            ByBitRequest request = client.CreateRequest("GET_QueryActiveOrder");
            request.AddRequired(cryptoPair);

            String response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);
            List<Order> orderList = new List<Order>();

            foreach (JObject orderObject in responseJson["result"].Children())
            {
                orderList.Add(new Order(orderObject, this));
            }

            return orderList;
        }


        //GET_GetActiveOrder
        public List<Order> GetActiveOrderList(String cryptoPair, Int32 limit = 50)
        {
            return GetActiveOrderList(cryptoPair, new OrderStatus[] { OrderStatus.NONE },limit);
        }

        public List<Order> GetActiveOrderList(String cryptoPair, OrderStatus[] status, Int32 limit = 50)
        {
            List<Order> orderList = new List<Order>();
            String cursor = "";
            //Console.ReadLine();

            do
            {
                String response = GetActiveOrder(cryptoPair, status, cursor, limit);
                JObject responseJson = JObject.Parse(response);

                Console.WriteLine(response);

                foreach (JObject orderObject in responseJson["result"]["data"].Children())
                {
                    orderList.Add(new Order(orderObject, this));
                }

                cursor = (String)responseJson["result"]["cursor"];

            } while (orderList.Count == limit);

            return orderList;
        }



        public String GetActiveOrder(String cryptoPair, OrderStatus[] status, String cursor = "", Int32 limit = 50, Direction direction = Direction.next)
        {
            ByBitRequest request = client.CreateRequest("GET_GetActiveOrder");
            request["symbol"] = cryptoPair;
            request["direction"] = direction.ToString();
            request["limit"] = limit;    //Default is 20

            if (!cursor.Equals(String.Empty)) 
            {
                request["cursor"] = cursor;
            }

            ProcessOrderStatus(request, status);

            return ExecuteWithRetry(request);
        }

        private void ProcessOrderStatus(ByBitRequest request,OrderStatus[] status) 
        {
            if (status[0] != OrderStatus.NONE)
            {
                StringBuilder strStatus = new StringBuilder();

                foreach(OrderStatus stat in status)
                {
                    strStatus.Append(stat.ToString() + ",");
                }

                //Remove the last comma
                strStatus.Remove(strStatus.Length - 1,1);

                request["order_status"] = strStatus;
            }
        }

        public Order ConditionalMarketOrder(String CryptoPair, int Contracts, Double triggerPrice, Double beforeTriggerPrice, TriggerPriceType triggerBy = TriggerPriceType.LastPrice, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceConditionalOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Market", (int)Math.Abs(Contracts), beforeTriggerPrice.ToString(CultureInfo.InvariantCulture), triggerPrice.ToString(CultureInfo.InvariantCulture), timeInForce.ToString());

            if (triggerBy != TriggerPriceType.LastPrice)
            {
                request["trigger_by"] = triggerBy.ToString();
            }

            return new Order(ExecuteWithRetry(request), this);
        }

        public Order ConditionalLimitOrder(String CryptoPair,int Contracts, Double entryPrice, Double triggerPrice, Double beforeTriggerPrice, TriggerPriceType triggerBy = TriggerPriceType.LastPrice, TimeInForce timeInForce = TimeInForce.GoodTillCancel)
        {
            ByBitRequest request = client.CreateRequest("POST_PlaceConditionalOrder");
            request.AddRequired(GetSide(Contracts), CryptoPair, "Limit", (int)Math.Abs(Contracts), beforeTriggerPrice.ToString(CultureInfo.InvariantCulture), triggerPrice.ToString(CultureInfo.InvariantCulture), timeInForce.ToString());
            request["price"] = entryPrice.ToString(CultureInfo.InvariantCulture);

            if (triggerBy != TriggerPriceType.LastPrice)
            {
                request["trigger_by"] = triggerBy.ToString();
            }

            return new Order(ExecuteWithRetry(request), this);
        }

        public String CancelConditionalOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("POST_CancelConditionalOrder");
            request.AddRequired(cryptoPair, orderId);

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
                request["p_r_price"] = newOrderPrice.ToString(CultureInfo.InvariantCulture);
            }

            if (newTriggerPrice != 0)
            {
                request["p_r_trigger_price"] = newTriggerPrice.ToString(CultureInfo.InvariantCulture);
            }
            
            return ExecuteWithRetry(request);
        }

        public String UpdateConditionalOrder(Order order, Int32 newOrderQuantity, Double newTriggerPrice, Double newOrderPrice)
        {
            return UpdateConditionalOrder(order.CryptoPair, order.OrderId, newOrderQuantity, newTriggerPrice, newOrderPrice);
        }

        public List<Order> GetConditionalOrderList(String cryptoPair, Int32 limit = 50)
        {
            return GetConditionalOrderList(cryptoPair, new StopOrderStatus[] { StopOrderStatus.NONE }, limit);
        }

        public List<Order> GetConditionalOrderList(String cryptoPair, StopOrderStatus[] status, Int32 limit = 50)
        {
            List<Order> orderList = new List<Order>();
            String cursor = "";
            //Console.ReadLine();

            do
            {
                String response = GetConditionalOrder(cryptoPair, status, cursor, limit);
                JObject responseJson = JObject.Parse(response);

                Console.WriteLine(response);

                foreach (JObject orderObject in responseJson["result"]["data"].Children())
                {
                    orderList.Add(new Order(orderObject, this));
                }

                cursor = (String)responseJson["result"]["cursor"];

            } while (orderList.Count == limit);

            return orderList;
        }



        public String GetConditionalOrder(String cryptoPair, StopOrderStatus[] status, String cursor = "", Int32 limit = 50, Direction direction = Direction.next)
        {
            ByBitRequest request = client.CreateRequest("GET_GetConditionalOrder");
            request["symbol"] = cryptoPair;
            request["direction"] = direction.ToString();
            request["limit"] = limit;    //Default is 20

            if (!cursor.Equals(String.Empty))
            {
                request["cursor"] = cursor;
            }

            ProcessStopOrderStatus(request, status);

            return ExecuteWithRetry(request);
        }

        private void ProcessStopOrderStatus(ByBitRequest request, StopOrderStatus[] status)
        {
            if (status[0] != StopOrderStatus.NONE)
            {
                StringBuilder strStatus = new StringBuilder();

                foreach (StopOrderStatus stat in status)
                {
                    strStatus.Append(stat.ToString() + ",");
                }

                //Remove the last comma
                strStatus.Remove(strStatus.Length - 1, 1);

                request["stop_order_status"] = strStatus;
            }
        }

        //GET_QueryConditionalOrder
        public String QueryConditionalOrder(String cryptoPair, String orderId)
        {
            ByBitRequest request = client.CreateRequest("GET_QueryConditionalOrder");
            request.AddRequired(cryptoPair, orderId);

            return ExecuteWithRetry(request);
        }

        public List<Order> QueryConditionalOrders(String cryptoPair)
        {
            ByBitRequest request = client.CreateRequest("GET_QueryConditionalOrder");
            request.AddRequired(cryptoPair);

            String response = ExecuteWithRetry(request);
            JObject responseJson = JObject.Parse(response);
            List<Order> orderList = new List<Order>();

            foreach (JObject orderObject in responseJson["result"].Children())
            {
                orderList.Add(new Order(orderObject, this));
            }

            return orderList;
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
                    request.AddRequired(Side, CryptoPair, "Market", PositionSize, TimeInForce.GoodTillCancel.ToString(), true, false);
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
