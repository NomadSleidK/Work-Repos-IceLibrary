using System.Collections.Generic;

namespace MyIceLibrary.Model
{
    public struct AttributeValues
    {
        public string AttributeName { get; set; }
        public List<object> Values { get; set; }

        public AttributeValues(string attributeName, List<object> values)
        {
            AttributeName = attributeName;
            Values = values;
        }
    }   
}