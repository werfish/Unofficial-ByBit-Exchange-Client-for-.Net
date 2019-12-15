using ByBitClientLib.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByBitClientLib
{
    public class ByBitRequest : IEnumerator, IEnumerable
    {
        private List<Parameter> RequestParameters;
        private List<Parameter> CachedRequestParameters;
        private ByBitClient clientReference;
        private EndPoint endpointReference;
        private int position;

        public Object Current
        {
            get
            {
                try
                {
                    return RequestParameters.ElementAt(position);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private ByBitRequest() { }

        internal ByBitRequest(EndPoint endpoint, ByBitClient client)
        {
            endpointReference = endpoint;
            clientReference = client;
            RequestParameters = endpointReference.getParameterList();
            position = 0;

        }

        private Parameter GetItemByName(string paramName)
        {

            foreach (Parameter param in RequestParameters)
            {
                if (param.ParameterName.Equals(paramName))
                {
                    return param;
                }
            }

            throw new ArgumentException(paramName, "paramName was not found in the parameters list.");
        }

        private List<Parameter> CreateParameterListDeepCopy(List<Parameter> ListToCopy)
        {
            List<Parameter> newParamsList = new List<Parameter>();

            foreach (Parameter param in ListToCopy)
            {
                newParamsList.Add(param.DeepCopy());
            }

            return newParamsList;
        }

        internal void AddAuthenticationParameter(string paramName, string dataType, Object value)
        {
            RequestParameters.Add(new Parameter(paramName,dataType,true));
            GetItemByName(paramName).Value = value;
        }

        internal void Sort()
        {
            RequestParameters = RequestParameters.OrderBy(x => x.ParameterName).ToList();
        }

        internal void Clear()
        {
            RequestParameters.RemoveAll(item => item.Value == null);
        }

        // The get accessor returns an Object for a given parameter
        public Object this[string paramName]
        {
            get
            {
                return  GetItemByName(paramName);
            }
            set
            {
                GetItemByName(paramName).Value = value;
            }
        }

        public void AddRequired(params object[] values)
        {
            int Counter = 0;
            int ArrLength = values.Length;

            foreach (Parameter param in RequestParameters)
            {
                if (param.IsRequired)
                {
                    if (Counter > ArrLength - 1) { throw new ArgumentException("Amount of Parameters does not meet the amount of required Request Parameters. "); }
                    param.Value = values[Counter];
                    Counter++;
                }
            }

            if (Counter < ArrLength - 1) { throw new ArgumentException("Amount of Function Parameters does not meet the amount of required Request Parameters. "); }
        }

        public int Count()
        {
            return RequestParameters.Count;
        }

        public String Execute()
        {
            string result;
            //Cache the parameters set by library user
            CachedRequestParameters = CreateParameterListDeepCopy(RequestParameters);

            //Make the Request
            result = clientReference.ExecuteRequest(this, endpointReference);

            //Load the cached parameters back
            RequestParameters = CreateParameterListDeepCopy(CachedRequestParameters);
            return result;
        }

        public bool MoveNext()
        {
            if (RequestParameters.Count != 0)
            {
                position++;
                return (position < RequestParameters.Count);
            }
            else { return false; }
        }

        public void Reset()
        {
            position = 0;
        }

        public IEnumerator GetEnumerator()
        {
            return RequestParameters.GetEnumerator();
        }
    }
}
