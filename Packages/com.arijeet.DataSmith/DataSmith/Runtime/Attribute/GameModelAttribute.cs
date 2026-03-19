using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameModelAttribute : Attribute
    {
        public ModelValueType ValueType { get; }

        public GameModelAttribute(ModelValueType valueType = ModelValueType.List)
        {
            ValueType = valueType;
        }
    }
}
