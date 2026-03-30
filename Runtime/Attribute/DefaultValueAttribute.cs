using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public DefaultValueAttribute(object value)
        {
            if (value is null)
            {
                Value = null;
                return;
            }
            
            bool supported =
                value is string or char or bool ||
                value is byte or sbyte or short or ushort or int or uint or long or ulong ||
                value is float or double or decimal;
            
            if (!supported)
                throw new ArgumentException("Unsupported default value type.", nameof(value));
            
            Value = value;
        }
    }
}
