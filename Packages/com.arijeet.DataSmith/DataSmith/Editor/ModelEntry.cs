#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Baruah.DataSmith.Editor
{
    public class ModelEntry
    {
        #if ODIN_INSPECTOR
        [TableColumnWidth(200)]
        [DisplayAsString]
        [HideLabel]
        #endif
        public System.Type Type;
        
        #if ODIN_INSPECTOR
        [DisplayAsString]
        [MultiLineProperty(2)]
        [LabelText("Namespace")]
        public string Namespace => Type?.Namespace;
        #endif
        
        [HideInInspector]
        public string Path;
        
        [HideInInspector]
        public GameModelAttribute Attribute;
        
        #if ODIN_INSPECTOR
        private DataSmithConfig _config;
        
        [TableColumnWidth(120)]
        [Button(ButtonSizes.Small)]
        private void Generate()
        {
            DataSmithGenerator.GenerateEntry(this, _config.OutputFolder);
        }

        public void SetConfigFile(DataSmithConfig config)
        {
            _config = config;
        }
        #endif
    }
}
