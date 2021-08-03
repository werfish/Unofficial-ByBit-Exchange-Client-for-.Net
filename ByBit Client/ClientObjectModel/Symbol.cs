using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class SymbolInfo
    {
        public String name { get; private set; }
        public String alias { get; private set; }
        public ConnectionManager.ContractStatus status { get; private set; }
        public ConnectionManager.Currency baseCurrency { get; private set; }
        public String quoteCurrency { get; private set; }
        public Int32 priceScale { get; private set; }
        public Decimal takerFee { get; private set; }
        public Decimal makerFee { get; private set; }
        public Decimal minLeverage { get; private set; }
        public Decimal maxLeverage { get; private set; }
        public Decimal leverageStep { get; private set; }
        public Decimal minPrice { get; private set; }
        public Decimal maxPrice { get; private set; }
        public Decimal tickSize { get; private set; }
        public Int32 maxTradingQty { get; private set; }
        public Int32 minTradingQty { get; private set; }
        public Int32 qtyStep { get; private set; }

        public String response { get; private set; }

        ConnectionManager conn;

        internal SymbolInfo(String response, ConnectionManager conn)
        {
            this.conn = conn;
            JObject json = JObject.Parse(response);
            this.response = response;
            PopulateFields((JObject)json);
        }

        public void PopulateFields(JObject json)
        {
            name = (String)json["name"];
            alias = (String)json["alias"];
            status = conn.getContractStatus((String)json["status"]);
            baseCurrency = conn.getCurrency((String)json["base_currency"]);
            quoteCurrency = (String)json["order_id"];
            priceScale = (int)json["price_scale"];
            takerFee = (Decimal)json["taker_fee"];
            makerFee = (Decimal)json["maker_fee"];
            minLeverage = (Decimal)json["leverage_filter"]["min_leverage"];
            maxLeverage = (Decimal)json["leverage_filter"]["max_leverage"];
            leverageStep = (Decimal)json["leverage_filter"]["leverage_step"];
            minPrice = (Decimal)json["price_filter"]["min_price"];
            maxPrice = (Decimal)json["price_filter"]["max_price"];
            tickSize = (Decimal)json["price_filter"]["tick_size"];
            maxTradingQty = (int)json["lot_size_filter"]["max_trading_qty"];
            minTradingQty = (int)json["lot_size_filter"]["min_trading_qty"];
            qtyStep = (int)json["lot_size_filter"]["qty_step"];
        }

        public override String ToString()
        {
            return response;
        }
    }
}
