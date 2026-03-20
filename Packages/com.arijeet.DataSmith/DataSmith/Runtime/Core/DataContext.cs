using System.Collections.Generic;

namespace Baruah.DataSmith
{
    public static class DataContext
    {
        private static readonly Dictionary<System.Type, GameModel> _models = new();

        public static void Register<T>(T model) where T : GameModel
        {
            _models[typeof(T)] = model;
        }

        public static T Get<T>() where T : GameModel
        {
            return (T)_models[typeof(T)];
        }
    }
}