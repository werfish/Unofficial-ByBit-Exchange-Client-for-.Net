using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.Model
{
    public class Parameter
    {
        public string ParameterName { get; }
        public string DataType { get; }
        public bool IsRequired { get; }
        public object Value { get; set; }

        private Parameter()
        {

        }

        internal Parameter(string ParameterName, string DataType, bool IsRequired)
        {
            this.ParameterName = ParameterName;
            this.DataType = DataType;
            this.IsRequired = IsRequired;
        }

        internal Parameter DeepCopy()
        {
            Parameter copy = (Parameter)this.MemberwiseClone();
            if (Value != null)
            {
                copy.Value = new Object();
            }
            else
            {
                copy.Value = null;
            }
            return copy;
        }

    }
}
