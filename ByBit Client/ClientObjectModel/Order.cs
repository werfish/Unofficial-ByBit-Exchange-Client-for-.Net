using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class Order
    {
        public String OrderId;
        public String OrderLinkId;
        public String CryptoPair;
        public String Side;
        public String OrderType;
        public Decimal Price;
        int Quantity;
        public String TimeInForce;
        public String OrderStatus;

        public String Response;

        //"created_at": "2019-11-30T11:03:43.452Z",
        //"updated_at": "2019-11-30T11:03:43.455Z"

        public Order(String Response)
        {
            JObject json = JObject.Parse(Response);
            this.Response = Response;
            PopulateFields((JObject)json["result"]);
        }

        private void PopulateFields(JObject json)
        {
            this.OrderId = (String)json["order_id"];
            this.OrderLinkId = (String)json["order_link_id"];
            this.CryptoPair = (String)json["symbol"];
            this.Side = (String)json["side"];
            this.OrderType = (String)json["order_type"];
            this.Price = (Decimal)json["price"];
            this.Quantity = (int)json["qty"];
            this.TimeInForce = (String)json["time_in_force"];
            this.OrderStatus = (String)json["order_status"];
        }
    }
}
