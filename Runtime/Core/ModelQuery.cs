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

        public void Where(Func<T, bool> predicate)
        {
            AddCondition(predicate);
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
        
        public int Count()
        {
            int count = 0;

            foreach (var entry in Execute())
            {
                count++;
            }

            return count;
        }

        public int Sum(Func<T, int> selector)
        {
            int sum = 0;

            foreach (var item in Execute())
            {
                sum += selector(item);
            }

            return sum;
        }
        
        public TProperty Max<TProperty>(Func<T, TProperty> selector)
            where TProperty : IComparable<TProperty>
        {
            bool hasValue = false;
            TProperty max = default;

            foreach (var item in Execute())
            {
                var value = selector(item);

                if (!hasValue)
                {
                    max = value;
                    hasValue = true;
                    continue;
                }

                if (value.CompareTo(max) > 0)
                    max = value;
            }

            return max;
        }

        public float Average(Func<T, int> selector)
        {
            int sum = 0;
            int count = 0;

            foreach (var item in Execute())
            {
                sum += selector(item);
                count++;
            }
            
            return count == 0 ? 0 : (float)sum / count;
        }

        public IEnumerator<T> GetEnumerator() => Execute().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
