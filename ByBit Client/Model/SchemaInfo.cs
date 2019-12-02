using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ByBitClientLib.Model
{
    internal class SchemaInfo
    {
        public string JsonSchema { get; }
        private JObject JsonVar;
        public string[] EndPointNames { get; }
        public List<EndPoint> EndPoints { get; }
        public Dictionary<string,string> EndPointSchemas;

        private SchemaInfo()
        {

        }

        public SchemaInfo(string JsonPath)
        {
            //LoadJson String
            this.JsonSchema = LoadJson(JsonPath);

            //LoadJobject
            this.JsonVar = JObject.Parse(JsonSchema);

            //Make List of EndpointsTitles
            this.EndPointNames = GetEndPointTitles();

            //Load Enpoint objects
            this.EndPoints = LoadEndPoints();

            //GetSchemaString
            this.EndPointSchemas = CreateEndpointSchemas();
        }

        //Private Functions
        private String LoadJson(string Path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(this.GetType(), Path);
            string readJson = "";

            try
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Could find embedded ApiSchema resource file.", Path);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    readJson = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return readJson;
        }

        private List<EndPoint> LoadEndPoints()
        {
            List<EndPoint> list = new List<EndPoint>();
            foreach (JProperty property in JsonVar["EndPointSchemas"].Children())
            {
                String Name = property.Name;
                String method = (String)property.Value["method"];
                String requestString = (String)property.Value["requestString"];
                EndPoint endpoint = new EndPoint(Name, method, requestString);

                list.Add(endpoint);

                //Create the Parameters
                foreach (JProperty paramProp in property.Value["Parameters"].Children())
                {
                    String paramName = paramProp.Name;
                    String dataType = (String)paramProp.Value["type"];
                    bool isRequired = (bool)paramProp.Value["isRequired"];
                    endpoint.AddParameter(paramName, dataType, isRequired);
                }               
            }

            return list;
        }

        private string[] GetEndPointTitles()
        {
            string[] Titles;

            Titles = (from p in ((JObject)JsonVar["EndPointSchemas"]).Properties()
                      select p.Name).ToArray<String>();

            return Titles;
        }

        private Dictionary<string,string> CreateEndpointSchemas()
        {
            Dictionary<string,string> dict = new Dictionary<string,string>();
            string separator = "   ";

            foreach (EndPoint endpoint in EndPoints)
            {
                StringBuilder schema = new StringBuilder();
                schema.Append(endpoint.EndPointTitle);
                schema.Append(Environment.NewLine + separator + "Request Method: " + endpoint.RequestMethod);
                schema.Append(Environment.NewLine + separator + "Request Endpoint Link: " + endpoint.RequestString);
                schema.Append(Environment.NewLine + separator +  "Parameters:  " + (endpoint.ParamListTemplate.Count == 0 ? "{ }" : ""));

                foreach (Parameter param in endpoint.ParamListTemplate)
                {
                    schema.Append(Environment.NewLine + separator + separator + param.ParameterName);
                    schema.Append(Environment.NewLine + separator + separator + separator + param.DataType);
                    schema.Append(Environment.NewLine + separator + separator + separator + (param.IsRequired == true ? "Required" : "Optional" ));
                }

                dict.Add(endpoint.EndPointTitle,schema.ToString());
            }

            return dict;
        }
    }
}
