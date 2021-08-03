using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class PnlRecord
    {
        public String response { get; private set; }
        public Int32 Id { get; private set; }
        public Int32 UserId { get; private set; }
        public String Symbol { get; private set; }
        public String OrderId { get; private set; }
        public ConnectionManager.Side Side { get; private set; }
        public Int32 Quantity { get; private set; }
        public Decimal OrderPrice { get; private set; }
        public ConnectionManager.OrderType OrderType { get; private set; }
        public ConnectionManager.ExecType ExecutionType { get; private set; }
        public Int32 ClosedSize { get; private set; }
        public Decimal CumEntryValue { get; private set; }
        public Decimal AvgEntryPrice { get; private set; }
        public Decimal CumExitValue { get; private set; }
        public Decimal AvgExitPrice { get; private set; }
        public Decimal ClosedPnl { get; private set; }
        public Int32 FillCount { get; private set; }
        public Decimal Leverage { get; private set; }
        public DateTime CreatedAt { get; private set; }

        ConnectionManager conn;

        internal PnlRecord(String response, ConnectionManager conn)
        {
            this.conn = conn;
            JObject json = JObject.Parse(response);
            this.response = response;
            PopulateFields((JObject)json);
        }

        internal void PopulateFields(JObject json)
        {
            this.Symbol = (String)json["symbol"];
            Side = conn.getSide((String)json["side"]);
            Id = (int)json["id"];
            UserId = (int)json["user_id"];
            OrderId = (String)json["order_id"];
            Quantity = (int)json["qty"];
            OrderPrice = (Decimal)json["order_price"];
            OrderType = conn.getOrderType((String)json["order_type"]);
            ExecutionType = conn.getExecutionType((String)json["exec_type"]);
            ClosedSize = (int)json["closed_size"];
            CumEntryValue = (Decimal)json["cum_entry_value"];
            AvgEntryPrice = (Decimal)json["avg_entry_price"];
            CumExitValue = (Decimal)json["cum_exit_value"];
            AvgExitPrice = (Decimal)json["avg_exit_price"];
            ClosedPnl = (Decimal)json["closed_pnl"];
            FillCount = (int)json["fill_count"];
            Leverage = (int)json["leverage"];
            CreatedAt = (DateTimeOffset.FromUnixTimeSeconds((int)json["created_at"])).UtcDateTime;
        }

        public override String ToString()
        {
            return response;
        }
    }
}
