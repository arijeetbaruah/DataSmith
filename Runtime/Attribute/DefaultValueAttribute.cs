using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public DefaultValueAttribute(object value)
        {
            Value = value;
        }
    }
}
