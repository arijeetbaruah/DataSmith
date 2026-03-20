namespace Baruah.DataSmith
{
    public abstract class GameModel
    {
        public abstract string Serialize();
        public abstract void Deserialize(string data);
        
        protected virtual void OnDeserialized() { }
    }
}
