using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public class Candle
    {
        public enum Direction { Green, Red };

        public String CryptoPair { get; private set; }
        public Decimal Open { get; private set; }
        public Decimal Close { get; private set; }
        public Decimal High { get; private set; }
        public Decimal Low { get; private set; }
        public Decimal Volume { get; private set; }
        public DateTime OpenTime { get; private set; }
        public ConnectionManager.Interval Timeframe { get; private set; }
        public Direction direction { get; private set; }
        public String response { get; private set; }

        private ConnectionManager conn;

        internal Candle(JObject response, ConnectionManager conn)
        {
            this.conn = conn;
            this.response = response.ToString();
            PopulateFields(response);
        }

        public void PopulateFields(JObject json)
        {
            this.CryptoPair = (String)json["symbol"];
            this.Open = (Decimal)json["open"];
            this.Close = (Decimal)json["close"];
            this.High = (Decimal)json["high"];
            this.Low = (Decimal)json["low"];
            this.Volume = (Decimal)json["volume"];
            this.OpenTime = (DateTimeOffset.FromUnixTimeSeconds((Int32)json["open_time"])).UtcDateTime;
            this.Timeframe = conn.getInterval((String)json["interval"]);
            this.direction = getDirection();
        }

        private Direction getDirection()
        {
            if (Open > Close)
            {
                return Direction.Red;
            }
            else
            {
                return Direction.Green;
            }
        }

        public override String ToString() 
        {
            return OpenTime.ToString();
        }
    }
}
