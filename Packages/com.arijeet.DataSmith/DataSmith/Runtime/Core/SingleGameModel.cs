namespace Baruah.DataSmith
{
    public abstract class SingleGameModel<T> : GameModel
    {
        public T Value { get; protected set; }
    }
}
