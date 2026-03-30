using System;
using System.Collections;
using System.Collections.Generic;

namespace Baruah.DataSmith
{
    public abstract class ModelQuery<T> : IEnumerable<T>
    {
        protected readonly IReadOnlyList<T> _source;
        protected Func<T, bool> _predicate = PredicateExtensions.True<T>();

        public ModelQuery(IReadOnlyList<T> source)
        {
            _source = source;
        }

        public IEnumerable<T> Execute()
        {
            foreach (var source in _source)
            {
                if (_predicate(source))
                {
                    yield return source;
                }
            }
        }
        
        protected void AddCondition(Func<T, bool> condition)
        {
            _predicate = Combine(_predicate, condition);
        }

        private static Func<T, bool> Combine(
            Func<T, bool> a,
            Func<T, bool> b)
            => x => a(x) && b(x);
        
        public T this[int index] => _source[index];
        
        public T FirstOrDefault()
        {
            foreach (var item in this)
                return item;
            return default;
        }

        public bool Any()
        {
            foreach (var item in this)
                return true;
            return false;
        }

        public bool Any(System.Func<T, bool> predicate)
        {
            foreach (var item in this)
            {{
                if (predicate(item))
                {{
                    return true;
                }}
            }}
            return false;
        }

        public IEnumerator<T> GetEnumerator() => Execute().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
