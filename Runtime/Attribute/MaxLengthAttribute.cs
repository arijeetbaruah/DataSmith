using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MaxLengthAttribute : Attribute
    {
        public int Length { get; }

        public MaxLengthAttribute(int length)
        {
            Length = length;
        }
    }
}
