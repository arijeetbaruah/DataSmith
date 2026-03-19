using System;

namespace Baruah.DataSmith
{
    public static class PredicateExtensions
    {
        public static Func<T, bool> True<T>() => _ => true;

        public static Func<T, bool> And<T>(
            this Func<T, bool> a,
            Func<T, bool> b)
            => x => a(x) && b(x);

        public static Func<T, bool> Or<T>(
            this Func<T, bool> a,
            Func<T, bool> b)
            => x => a(x) || b(x);

        public static Func<T, bool> Not<T>(
            this Func<T, bool> a)
            => x => !a(x);
    }
}
