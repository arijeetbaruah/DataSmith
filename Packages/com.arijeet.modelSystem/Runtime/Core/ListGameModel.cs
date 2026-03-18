using System;
using System.Collections.Generic;

namespace Baruah.ModelSystem
{
    public abstract class ListGameModel<T> : GameModel
    {
        public IReadOnlyList<T> Items => _items;

        protected List<T> _items = new();

        public void Add(T item)
        {
            _items.Add(item);
        }
        
        public void AddRange(IEnumerable<T> item)
        {
            _items.AddRange(item);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }
    }
}
