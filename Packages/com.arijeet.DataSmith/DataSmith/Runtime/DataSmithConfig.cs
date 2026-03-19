using System.Collections.Generic;
using UnityEngine;

namespace Baruah.DataSmith.Editor
{
    [CreateAssetMenu(fileName = nameof(DataSmithConfig), menuName = "DataSmith/" + nameof(DataSmithConfig))]
    public class DataSmithConfig : ScriptableObject
    {
        private const string DefaultOutput = "Assets/Generated Scripts";
        
        public string OutputFolder => _outputFolder;
        public IReadOnlyList<string> IncludePaths => _includePaths;
        public IReadOnlyList<string> ExcludePaths => _excludePaths;
        
        [SerializeField] private string _outputFolder = DefaultOutput;

        [SerializeField] private List<string> _includePaths = new();
        [SerializeField] private List<string> _excludePaths = new();
    }
}
