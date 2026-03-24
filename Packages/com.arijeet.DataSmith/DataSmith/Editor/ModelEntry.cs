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
        
        /// <summary>
        /// Generates output for this model entry using the stored configuration's output folder.
        /// If no configuration has been set, the method does nothing.
        /// </summary>
        [TableColumnWidth(120)]
        [Button(ButtonSizes.Small)]
        private void Generate()
        {
            if (_config == null)
                return;

            DataSmithGenerator.GenerateEntry(this, _config.OutputFolder);
        }

        /// <summary>
        /// Assigns the DataSmith configuration used later when generating this model entry.
        /// </summary>
        /// <param name="config">The configuration to store for subsequent generation operations.</param>
        public void SetConfigFile(DataSmithConfig config)
        {
            _config = config;
        }
        #endif
    }
}
