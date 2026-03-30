using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReferenceAttribute : Attribute
    {
        public Type TargetModelType { get; }

        public ReferenceAttribute(Type targetModelType)
        {
            TargetModelType = targetModelType;
        }
    }
}