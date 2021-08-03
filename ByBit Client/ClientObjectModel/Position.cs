using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class Position
    {
        public  String Symbol { get; private set; }
        public String side { get; private set; }
        public int PositionSize { get; private set; }
        public Decimal PositionValue { get; private set; }
        public Decimal EntryPrice { get; private set; }
        public int Leverage { get; private set; }
        public Decimal PositionMargin { get; private set; }
        public Decimal LiquidationPrice { get; private set; }
        public Decimal BustPrice { get; private set; }
        public Decimal AccountClosingFee { get; private set; }
        public Decimal TakeProfit { get; private set; }
        public Decimal StopLoss { get; private set; }
        public Decimal TrailingStop { get; private set; }
        public String PositionStatus { get; private set; }
        public Decimal OrderMargin { get; private set; }
        public Decimal WalletBalance { get; private set; }
        public Decimal RealisedPnl { get; private set; }
        public String response { get; private set; }

        //Connection Manager
        ConnectionManager conn;

        internal Position(JObject responseJson, ConnectionManager conn)
        {
            this.conn = conn;
            this.response = responseJson.ToString();
            PopulateFields((JObject)responseJson);
        }

        private void PopulateFields(JObject json)
        {
            Symbol = (String)json["symbol"];
            side = (String)json["side"];

            PositionSize = (int)json["size"];
            if (side.Equals("Sell"))
            {
                PositionSize = -PositionSize;
            }

            PositionValue = (Decimal)json["position_value"];
            EntryPrice = (Decimal)json["entry_price"];
            Leverage = (int)json["leverage"];
            PositionMargin = (Decimal)json["position_margin"];
            LiquidationPrice = (Decimal)json["liq_price"];
            BustPrice = (Decimal)json["bust_price"];
            AccountClosingFee = (Decimal)json["occ_closing_fee"];
            TakeProfit = (Decimal)json["take_profit"];
            StopLoss = (Decimal)json["stop_loss"];
            TrailingStop = (Decimal)json["trailing_stop"];
            PositionStatus = (String)json["position_status"];
            OrderMargin = (Decimal)json["order_margin"];
            WalletBalance = (Decimal)json["wallet_balance"];
            RealisedPnl = (Decimal)json["realised_pnl"];
        }

        public Position Refresh()
        {
            String response = conn.GetPositionResponse(this.Symbol);
            JObject responseJson = JObject.Parse(response);
            PopulateFields((JObject)responseJson["result"]);
            return this;
        }
        public void SetLeverage(Int32 newLeverage) 
        {
            conn.SetLeverage(Symbol,newLeverage);
            Leverage = newLeverage;
        }
    }
}
