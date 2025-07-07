using System.Collections.Generic;

namespace MyIceLibrary
{
    public struct AttributeValues
    {
        public string AttributeName { get; set; }
        public List<object> Values { get; set; }

        public AttributeValues(string attributeName, List<object> values = null)
        {
            AttributeName = attributeName;
            Values = values ?? new List<object>();
        }
    }

    public struct AttributeValue
    {
        public string AttributeName { get; set; }
        public object Value { get; set; }

        public AttributeValue(string attributeName, object value)
        {
            AttributeName = attributeName;
            Value = value;
        }
    }

    public struct MainInfoValue
    {
        public object DisplayName { get; set; }
        public object ID { get; set; }
        public object Created { get; set; }
        public object Creator { get; set; }
        public object Type { get; set; }
    }
}