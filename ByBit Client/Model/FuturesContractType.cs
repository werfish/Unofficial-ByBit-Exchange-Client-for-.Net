using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByBitClientLib.Model
{
    public class FuturesContractType : TradingType, ITradingType
    {
        internal FuturesContractType(ByBitClient client, String JsonPath) : base(client,JsonPath)
        {
            //nothing Needed
        }

        //Schema Functions
        public String GetSchemaJSON()
        {
            return Schema.JsonSchema;
        }

        public String[] GetEndpointNames()
        {
            return Schema.EndPointNames;
        }

        public string GetEndpointSchema(string EndpointName)
        {
            return Schema.EndPointSchemas[EndpointName];
        }

        public string GetEndpointSchema(int EndPointIndex)
        {
            return Schema.EndPointSchemas.ElementAt(EndPointIndex).Value;
        }

        //Request Functions
        public EndPoint GetEndPoint(string EndPointTitle)
        {
            foreach (EndPoint endpoint in Schema.EndPoints)
            {
                if (endpoint.EndPointTitle.Equals(EndPointTitle))
                {
                    return endpoint;
                }
            }

            throw new ArgumentException(EndPointTitle, "End point Title was not found in the Endpoints schema.");
        }

        public ByBitRequest CreateRequest(EndPoint endpoint)
        {
            return new ByBitRequest(endpoint, client);
        }

        public ByBitRequest CreateRequest(string endpointTitle)
        {
            return CreateRequest(GetEndPoint(endpointTitle));
        }

        public string MakeRequest(EndPoint endpoint, params object[] parameters)
        {
            ByBitRequest newRequest = CreateRequest(endpoint);
            int Counter = 0;

            //Check for Wrong amount of Parameters
            if (parameters.Length != newRequest.Count())
            {
                throw new ArgumentException("Amount of Function Parameters does not meet the amount of Request Parameters. ");
            }

            foreach (Model.Parameter param in newRequest)
            {
                param.Value = parameters[Counter];
                Counter++;
            }

            //Send the request to exchange
            return client.ExecuteRequest(newRequest, endpoint);
        }

        public string MakeRequest(String EndPointTitle, params object[] parameters)
        {
            string result;
            result = MakeRequest(GetEndPoint(EndPointTitle), parameters);
            return result;
        }
    }
}
