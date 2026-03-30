using UnityEngine;

namespace Baruah.DataSmith.Database
{
    [CreateAssetMenu(fileName = nameof(DatabaseConfigAsset), menuName = "DataSmith/" + nameof(DatabaseConfigAsset))]
    public class DatabaseConfigAsset : ScriptableObject
    {
        [Tooltip("Database provider implementation")]
        [SerializeField, SerializeReference]
        private DatabaseProviderAsset _provider;
        
        public DatabaseProviderAsset Provider => _provider;
    }
}
