using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle
{

    public class CC_AssetMap : MonoBehaviour
    {
        public static CC_AssetMap assetMap;

        [Serializable]
        public struct CC_InspectorAsset
        {
            public string id;
            public GameObject gameObject;
        }

        private void Awake()
        {
            assetMap = this;
            chunkTypes = fillDictionary(editorChunkTypes, chunkTypes);
            entityTypes = fillDictionary(editorEntityTypes, entityTypes);
            decalTypes = fillDictionary(editorDecalTypes, decalTypes);
            objectTypes = fillDictionary(editorObjectTypes, objectTypes);
        }

        public GameObject cameraFollowPoint;
        public CC_InspectorAsset[] editorChunkTypes;
        public CC_InspectorAsset[] editorEntityTypes;
        public CC_InspectorAsset[] editorDecalTypes;
        public CC_InspectorAsset[] editorObjectTypes;

        public Dictionary<string, GameObject> chunkTypes { get; private set; }
        public Dictionary<string, GameObject> entityTypes { get; private set; }
        public Dictionary<string, GameObject> decalTypes { get; private set; }
        public Dictionary<string, GameObject> objectTypes { get; private set; }

        private Dictionary<String, GameObject> fillDictionary(CC_InspectorAsset[] editorAssets, Dictionary<String, GameObject> dictionary)
        {
            if (dictionary == null) { dictionary = new Dictionary<String, GameObject>(); }
            foreach (CC_InspectorAsset asset in editorAssets)
            {
                dictionary.Add(asset.id, asset.gameObject);
            }
            return dictionary;
        }
    }
}
