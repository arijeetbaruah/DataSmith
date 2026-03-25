using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baruah.DataSmith
{
    [CreateAssetMenu(fileName = "AssetContextsSO", menuName = "DataSmith/AssetContextsSO")]
    public class AssetContextsSO : ScriptableObject
    {
        public IReadOnlyList<BaseAssetModel> Assets => _assets;
        
        [SerializeField]
        private List<BaseAssetModel> _assets;
        
        #if UNITY_EDITOR
        #if ODIN_INSPECTOR
        [Button("Update Assets")]
        #endif
        [ContextMenu("Update Assets")]
        public void UpdateAssets()
        {
            var assets = UnityEditor.TypeCache.GetTypesDerivedFrom<BaseAssetModel>()
                .Where(a => !a.IsAbstract && !a.IsGenericType);
            
            foreach (var asset in assets)
            {
                if (!_assets.Any(a => a.GetType() == asset.GetType()))
                {
                    var assetFile = ScriptableObject.CreateInstance(asset) as BaseAssetModel;

                    string path = UnityEditor.AssetDatabase.GetAssetPath(this).Replace(this.name, asset.Name);
                    UnityEditor.AssetDatabase.CreateAsset(assetFile, path);
                    _assets.Add(assetFile);
                }
            }
            
            UnityEditor.AssetDatabase.SaveAssets();
        }
        #endif
    }
}
