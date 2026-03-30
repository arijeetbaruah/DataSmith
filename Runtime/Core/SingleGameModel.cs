using Newtonsoft.Json;

namespace Baruah.DataSmith
{
    public abstract class SingleGameModel<T> : GameModel
    {
        public T Value { get; protected set; }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(Value);
        }

        public override void Deserialize(string data)
        {
            Value = JsonConvert.DeserializeObject<T>(data);
            OnDeserialized();
        }
    }
}
