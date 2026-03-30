using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Baruah.DataSmith
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
        
        public override string Serialize()
        {
            return JsonConvert.SerializeObject(_items);
        }

        public override void Deserialize(string data)
        {
            _items = JsonConvert.DeserializeObject<List<T>>(data);
            OnDeserialized();
        }
    }
}
