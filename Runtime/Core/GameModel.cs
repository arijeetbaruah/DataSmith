namespace Baruah.DataSmith
{
    public interface IGameModel
    {
    }
    
    public abstract class GameModel : IGameModel
    {
        public abstract string Serialize();
        public abstract void Deserialize(string data);
        
        protected virtual void OnDeserialized() { }
    }
}
