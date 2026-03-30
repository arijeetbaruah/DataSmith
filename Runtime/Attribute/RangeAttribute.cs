using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RangeAttribute : Attribute
    {
        public double Min { get; }
        public double Max { get; }

        public RangeAttribute(double min, double max)
        {
            if (double.IsNaN(min) || double.IsNaN(max) ||
                double.IsInfinity(min) || double.IsInfinity(max))
                throw new ArgumentOutOfRangeException(nameof(min), "Range bounds must be finite numbers.");

            if (min > max)
                throw new ArgumentException("Min cannot be greater than Max.");

            Min = min;
            Max = max;
        }
    }
}
