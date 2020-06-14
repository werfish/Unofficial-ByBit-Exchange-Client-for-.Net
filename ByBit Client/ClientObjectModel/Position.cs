using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class Position
    {
        //REMEMBER TO READ ONLY THE BELOW VARIABLES
        public  String Symbol;
        public String side;
        public int PositionSize;
        public Decimal PositionValue;
        public Decimal EntryPrice;
        public int Leverage;
        public Decimal PositionMargin;
        public Decimal LiquidationPrice;
        public Decimal BustPrice;
        public Decimal AccountClosingFee;
        public Decimal TakeProfit;
        public Decimal StopLoss;
        public Decimal TrailingStop;
        public String PositionStatus;
        public Decimal OrderMargin;
        public Decimal WalletBalance;
        public Decimal RealisedPnl;
        public String response;

        public Position(String response)
        {
            JObject json = JObject.Parse(response);
            this.response = response;
            PopulateFields((JObject)json["result"]);
        }

        public void PopulateFields(JObject json)
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
    }
}
