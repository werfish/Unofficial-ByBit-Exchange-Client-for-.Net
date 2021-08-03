using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class Order
    {
        //To Accomodate functionality from inside this class a ConnectionManager object is needed
        private ConnectionManager connManager;

        public String OrderId { get; private set; }
        //public String OrderLinkId;
        public String CryptoPair { get; private set; }
        public ConnectionManager.Side Side { get; private set; }
        public ConnectionManager.OrderType OrderType { get; private set; }
        public ConnectionManager.StopOrderStatus ConditionalOrderStatus { get; private set; }
        public ConnectionManager.TriggerPriceType TriggerPriceType { get; private set; }
        public Decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public ConnectionManager.TimeInForce TimeInForce { get; private set; }
        public ConnectionManager.OrderStatus OrderStatus { get; private set; }
        
        public String Response { get; private set; }

        //"created_at": "2019-11-30T11:03:43.452Z",
        //"updated_at": "2019-11-30T11:03:43.455Z"

        internal Order(String Response, ConnectionManager connManager)
        {
            this.connManager = connManager;
            JObject json = JObject.Parse(Response);
            this.Response = Response;

            if (Response.Contains("stop_order_id"))
            {
                PopulateFieldsConditional((JObject)json["result"]);
            }
            else
            {
                PopulateFieldsActive((JObject)json["result"]);
            }
        }

        public Order(JObject orderData, ConnectionManager connManager)
        {
            this.connManager = connManager;
            this.Response = orderData.ToString();

            if (Response.Contains("stop_order_id"))
            {
                PopulateFieldsConditional(orderData);
            }
            else
            {
                PopulateFieldsActive(orderData);
            }
        }

        private void PopulateFieldsActive(JObject json)
        {
            this.OrderType = DetermineOrderType(json, false);

            this.OrderId = (String)json["order_id"];
            //this.OrderLinkId = (String)json["order_link_id"];
            this.CryptoPair = (String)json["symbol"];
            this.Side = connManager.getSide((String)json["side"]);
            this.Price = (Decimal)json["price"];
            this.Quantity = (int)json["qty"];
            this.TimeInForce = connManager.getTimeInForce((String)json["time_in_force"]);
            this.OrderStatus = connManager.getOrderStatus((String)json["order_status"]);
            this.ConditionalOrderStatus = ConnectionManager.StopOrderStatus.NONE;
            this.TriggerPriceType = ConnectionManager.TriggerPriceType.NONE;
        }

        private void PopulateFieldsConditional(JObject json)
        {
            this.OrderType = DetermineOrderType(json, true);

            this.OrderId = (String)json["stop_order_id"];
            //this.OrderLinkId = (String)json["order_link_id"];
            this.CryptoPair = (String)json["symbol"];
            this.Side = connManager.getSide((String)json["side"]);
            this.Price = (Decimal)json["price"];
            this.Quantity = (int)json["qty"];
            this.TimeInForce = connManager.getTimeInForce((String)json["time_in_force"]);
            this.OrderStatus = ConnectionManager.OrderStatus.NONE;
            if ((String)json["stop_order_status"] != null)
            {
                this.ConditionalOrderStatus = connManager.getStopOrderStatus((String)json["stop_order_status"]);
            }
            this.TriggerPriceType = connManager.getTriggerPriceType((String)json["trigger_by"]);
        }

        private ConnectionManager.OrderType DetermineOrderType(JObject json, Boolean isConditional)
        {
            if (isConditional)
            {
                if (((String)json["order_type"]).Equals("Limit"))
                {
                   return ConnectionManager.OrderType.ConditionalLimit;
                }
                else
                {
                   return ConnectionManager.OrderType.ConditionalMarket;
                }
            }
            else
            {
                if (((String)json["order_type"]).Equals("Limit"))
                {
                    return ConnectionManager.OrderType.Limit;
                }
                else
                {
                    return ConnectionManager.OrderType.Market;
                }
            }
        }

        //------------PUBLIC interface Methods---------------------
        //Boolean Checks funtions
        public Boolean isLimit()
        {
            return true;
        }

        public Boolean isMarket()
        {
            return true;
        }

        public Boolean isTriggered()
        {
            return true;
        }

        public Boolean isConditionalLimit()
        {
            return true;
        }

        public Boolean isConditionalMarket()
        {
            return true;
        }

        //Functionality functions

        public void Cancel()
        {

        }

        public void Update()
        {

        }

        public void RefreshData()
        {

        }
    }
}
