using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.Model
{
    public class EndPoint
    {
        public string EndPointTitle { get; }
        public string RequestMethod { get; }
        public string RequestString { get; }
        public List<Parameter> ParamListTemplate;

        private EndPoint()
        {

        }

        internal EndPoint(string EndPointTitle, string RequestMethod, string RequestString)
        {
            this.EndPointTitle = EndPointTitle;
            this.RequestMethod = RequestMethod;
            this.RequestString = RequestString;

            ParamListTemplate = new List<Parameter>();
        }

        internal void AddParameter(string ParameterName, string DataType, bool IsRequired)
        {
            Parameter newParam = new Parameter(ParameterName, DataType, IsRequired);
            ParamListTemplate.Add(newParam);
        }

        internal List<Parameter> getParameterList()
        {
            List<Parameter> newList = new List<Parameter>();

            foreach (Parameter param in ParamListTemplate)
            {
                newList.Add(param.DeepCopy());
            }

            return newList;
        }
    }
}
