using System;

namespace Baruah.DataSmith
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameModelAttribute : Attribute
    {
        public ModelStorageType StorageType { get; }
        public ModelCollectionType CollectionType { get; }

        public GameModelAttribute(ModelStorageType storageType = ModelStorageType.Memory, ModelCollectionType collectionType = ModelCollectionType.List)
        {
            StorageType = storageType;
            CollectionType = collectionType;
        }
    }
}
