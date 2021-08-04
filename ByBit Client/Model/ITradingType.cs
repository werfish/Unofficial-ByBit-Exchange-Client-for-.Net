using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.Model
{
    public interface ITradingType
    {
        //Schema Functions
        String GetSchemaJSON();

        String[] GetEndpointNames();

        String GetEndpointSchema(String EndpointName);

        String GetEndpointSchema(Int32 EndPointIndex);

        //Request Functions
        EndPoint GetEndPoint(String EndPointTitle);

        ByBitRequest CreateRequest(EndPoint endpoint);

        ByBitRequest CreateRequest(String endpointTitle);

        String MakeRequest(EndPoint endpoint, params object[] parameters);

        String MakeRequest(String EndPointTitle, params object[] parameters);
    }
}
