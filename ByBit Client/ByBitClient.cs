using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using ByBitClientLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using ByBitClientLib.Model;

namespace ByBitClientLib
{
    public class ByBitClient
    {
        private string API_KEY;
        private string API_SECRET;
        public string API_URL;
        private string RECV_WINDOW = String.Empty;
        private string JSONPath = "ApiSchema.json";
        private SchemaInfo Schema;

        private string[] AUTHENTICATION_HEADERS = new string[] { "api_key", "timestamp", "recv_window" };

        //OTHER API HEADERS
        private string[] POST_PlaceActiveOrder_Headers = new string[] { "side", "symbol", "order_type", "qty", "time_in_force" };

        public ByBitClient(string Api_Key, string Api_Secret, string api_url)
        {
            Init(Api_Key, Api_Secret, api_url);
        }

        public ByBitClient(string Api_Key, string Api_Secret, string api_url,string recv_window)
        {
            Init(Api_Key, Api_Secret, api_url);
            this.RECV_WINDOW = recv_window;
        }

        private void Init(string Api_Key, string Api_Secret, string api_url)
        {
            this.API_KEY = Api_Key;
            this.API_SECRET = Api_Secret;
            this.API_URL = api_url;

            Schema = new SchemaInfo(JSONPath);
        }

        private ByBitClient() { }

        //PRivate Functions
        private dynamic ConvertValue(Object value)
        {
            dynamic d = value;
            return d;
        }

        private String ExecuteGET(StringBuilder paramsString, String hexSign, String requestString)
        {
            string result;
            paramsString.Append("&" + "sign" + "=" + hexSign);

            requestString = requestString + "?" + paramsString.ToString();

            //TESTING
            Console.WriteLine(paramsString.ToString());

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest(requestString, Method.GET);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;

            return result;
        }

        private String ExecutePOST(ByBitRequest newRequest, String hexSign, String requestString)
        {
            //Add Sign to Parameters
            newRequest.AddAuthenticationParameter("sign", "String", hexSign);

            //Create JSON with Parameters
            JObject JSON = new JObject();

            //Fill the Json Object with Values
            foreach (Model.Parameter param in newRequest)
            {
                JSON.Add(param.ParameterName, ConvertValue(param.Value));
            }

            //TESTING
            Console.WriteLine(JSON);

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest("/open-api/order/create", Method.POST);

            //Adding Json body as parameter to the post request
            restRequest.AddParameter("application/json", JSON, ParameterType.RequestBody);

            IRestResponse restResponse = restClient.Execute(restRequest);

            string result = restResponse.Content;

            return result;
        }

        internal string ExecuteRequest(ByBitRequest newRequest,EndPoint endpoint)
        {
            //Add authentication Headers ("api_key", "timestamp", "recv_window" )
            newRequest.AddAuthenticationParameter("api_key", "String", API_KEY);
            newRequest.AddAuthenticationParameter("timestamp", "Long", 0);
            if (RECV_WINDOW != String.Empty)
            {
                newRequest.AddAuthenticationParameter("recv_window", "String", RECV_WINDOW);
            }

            //Sort the newRequest parameters ascending by ParameterNames
            newRequest.Sort();

            //Clean all the Parameters not set by the user
            newRequest.Clear();

            //Add Unix TimeStamp
            newRequest["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //create the ParameterString
            StringBuilder paramsString = new StringBuilder();

            foreach (Model.Parameter param in newRequest)
            {
                paramsString.Append(param.ParameterName + "=" + ConvertValue(param.Value) + "&");

            }
            paramsString.Remove(paramsString.ToString().LastIndexOf("&"), 1); //Cut the last &

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());
            string result = "";

            if (endpoint.RequestMethod.Equals("GET"))
            {
                result = ExecuteGET(paramsString, hexSign, endpoint.RequestString);
            }
            else if (endpoint.RequestMethod.Equals("POST"))
            {
                result = ExecutePOST(newRequest, hexSign, endpoint.RequestString);

            }

            return ValidateRequest(result);
        }

        internal string ValidateRequest(string result)
        {
            JObject resultJson = JObject.Parse(result);
            string returnCode = (string)resultJson["ret_msg"];

            if (!returnCode.ToLower().Equals("ok"))
            {
                throw new Exception(result);
            }

            return result;
        }

        //PUBLIC INTERFACE

        //Schema Functions
        public String getSchemaJSON()
        {
            return Schema.JsonSchema;
        }

        public String[] getEndpointNames()
        {
            return Schema.EndPointNames;
        }

        public string getEndpointSchema(string EndpointName)
        {
            return Schema.EndPointSchemas[EndpointName];
        }

        public string getEndpointSchema(int EndPointIndex)
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
            return new ByBitRequest(endpoint, this);
        }

        public ByBitRequest CreateRequest(string endpointTitle)
        {
            return CreateRequest(GetEndPoint(endpointTitle));
        }

        public string MakeRequest(EndPoint endpoint,params object[] parameters)
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
            return ExecuteRequest(newRequest, endpoint);
        }

        public string MakeRequest(String EndPointTitle,params object[] parameters)
        {
            string result;
            result = MakeRequest(GetEndPoint(EndPointTitle), parameters);
            return result;
        }


        public string MarketOrder(string Side, string Symbol, int quantity)
        {
            string result;

            StringBuilder paramsString = new StringBuilder();
            JObject JSON = new JObject();

            //Add the parameters to JSON and ParamsString in Ascending Alphbetical Order
            //1st add the API Key
            JSON.Add(AUTHENTICATION_HEADERS[0], API_KEY);
            paramsString.Append(AUTHENTICATION_HEADERS[0] + "=" + API_KEY + "&");

            //2.order_type
            JSON.Add(POST_PlaceActiveOrder_Headers[2], "Market");
            paramsString.Append(POST_PlaceActiveOrder_Headers[2] + "=" + "Market" + "&");

            //3. qty
            JSON.Add(POST_PlaceActiveOrder_Headers[3], quantity);
            paramsString.Append(POST_PlaceActiveOrder_Headers[3] + "=" + quantity.ToString() + "&");

            //4.SIDE
            JSON.Add(POST_PlaceActiveOrder_Headers[0], Side);
            paramsString.Append(POST_PlaceActiveOrder_Headers[0] + "=" + Side + "&");

            //5.Symbol
            JSON.Add(POST_PlaceActiveOrder_Headers[1], Symbol);
            paramsString.Append(POST_PlaceActiveOrder_Headers[1] + "=" + Symbol + "&");

            //6. time_in_force
            JSON.Add(POST_PlaceActiveOrder_Headers[4], "");
            paramsString.Append(POST_PlaceActiveOrder_Headers[4] + "=" + "" + "&");

            //7 the timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            JSON.Add(AUTHENTICATION_HEADERS[1], timestamp);
            paramsString.Append(AUTHENTICATION_HEADERS[1] + "=" + timestamp.ToString());

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());

            //add the hexString to the JSON
            JSON.Add("sign",hexSign);

            //TESTING
            Console.WriteLine(paramsString.ToString());
            Console.WriteLine(JSON);

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest("/open-api/order/create", Method.POST);

            //Adding Json body as parameter to the post request
            restRequest.AddParameter("application/json", JSON, ParameterType.RequestBody);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;
            return result;
        }



        public string GetWalletBalance()
        {
            string result = "";
            string requestString = "/position/list";

            StringBuilder paramsString = new StringBuilder();

            //1st API KEY
            paramsString.Append(AUTHENTICATION_HEADERS[0] + "=" + API_KEY + "&");

            //4th the timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            paramsString.Append(AUTHENTICATION_HEADERS[1] + "=" + timestamp.ToString());

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());

            paramsString.Append("&" + "sign" + "=" + hexSign);

            requestString = requestString + "?" + paramsString.ToString();

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest(requestString, Method.GET);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;
            return result;
        }

        public string GetLeverage(string Symbol)
        {
            string result = "";
            string requestString = "/user/leverage";

            StringBuilder paramsString = new StringBuilder();

            //1st API KEY
            paramsString.Append(AUTHENTICATION_HEADERS[0] + "=" + API_KEY + "&");

            //4th the timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            paramsString.Append(AUTHENTICATION_HEADERS[1] + "=" + timestamp.ToString());

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());

            paramsString.Append("&" + "sign" + "=" + hexSign);

            requestString = requestString + "?" + paramsString.ToString();

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest(requestString, Method.GET);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;
            return result;

        }

        public string SetLeverage(string symbol, string leverage)
        {
            string result;

            StringBuilder paramsString = new StringBuilder();
            JObject JSON = new JObject();

            //Add the parameters to JSON and ParamsString in Ascending Alphbetical Order
            //1st add the API Key
            JSON.Add(AUTHENTICATION_HEADERS[0], API_KEY);
            paramsString.Append(AUTHENTICATION_HEADERS[0] + "=" + API_KEY + "&");

            //2. Leverage
            JSON.Add("leverage", leverage);
            paramsString.Append("leverage" + "=" + leverage + "&");

            //3. Symbol
            JSON.Add("symbol", symbol );
            paramsString.Append("symbol" + "=" + symbol + "&");

            //4. the timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            JSON.Add(AUTHENTICATION_HEADERS[1], timestamp);
            paramsString.Append(AUTHENTICATION_HEADERS[1] + "=" + timestamp.ToString());

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());

            //add the hexString to the JSON
            JSON.Add("sign", hexSign);

            //TESTING
            Console.WriteLine(paramsString.ToString());
            Console.WriteLine(JSON);

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest("/user/leverage/save", Method.POST);

            //Adding Json body as parameter to the post request
            restRequest.AddParameter("application/json", JSON, ParameterType.RequestBody);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;
            return result;
        }

        public string currentPrice(string symbol)
        {
            string result = "";
            string requestString = "/v2/public/tickers";

            StringBuilder paramsString = new StringBuilder();

            //1st API KEY
            paramsString.Append(AUTHENTICATION_HEADERS[0] + "=" + API_KEY + "&");

            //2nd Symbol
            paramsString.Append("symbol" + "=" + symbol + "&");

            //4th the timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            paramsString.Append(AUTHENTICATION_HEADERS[1] + "=" + timestamp.ToString());

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());

            paramsString.Append("&" + "sign" + "=" + hexSign);

            requestString = requestString + "?" + paramsString.ToString();

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest(requestString, Method.GET);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;
            return result;
        }

        public string getCurrentCandle(string symbol,string interval)
        {
            string result;
            string requestString = "v2/public/kline/list";

             StringBuilder paramsString = new StringBuilder();

            //Add the parameters to JSON and ParamsString in Ascending Alphbetical Order
            //1st add the API Key
            paramsString.Append(AUTHENTICATION_HEADERS[0] + "=" + API_KEY + "&");

            //From TimeStamp
            paramsString.Append("from" + "=" + ((Int32)(DateTime.UtcNow.AddMinutes(-60).Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString() + "&");

            //2. interval
            paramsString.Append("interval" + "=" + interval + "&");

            //3. Symbol
            paramsString.Append("symbol" + "=" + symbol + "&");

            //4. the timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            paramsString.Append(AUTHENTICATION_HEADERS[1] + "=" + timestamp.ToString());

            //Create the hex SIGN
            string hexSign = CryptoGraFX.CreateSignature(API_SECRET, paramsString.ToString());

            paramsString.Append("&" + "sign" + "=" + hexSign);

            requestString = requestString + "?" + paramsString.ToString();

            //TESTING
            Console.WriteLine(paramsString.ToString());

            //Send Request
            RestClient restClient = new RestClient(API_URL);

            RestRequest restRequest = new RestRequest(requestString, Method.GET);

            IRestResponse restResponse = restClient.Execute(restRequest);

            result = restResponse.Content;
            return result;
        }
    }
}
