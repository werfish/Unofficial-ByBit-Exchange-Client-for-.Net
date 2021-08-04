using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.Model
{
    public class TradingType
    {
        internal SchemaInfo Schema;
        protected ByBitClient client;

        internal TradingType(ByBitClient client, String JsonPath) 
        {
            this.client = client;
            this.Schema = new SchemaInfo(JsonPath);
        }
    }
}
