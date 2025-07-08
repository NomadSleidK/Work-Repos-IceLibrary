namespace MyIceLibrary.Model
{
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
}