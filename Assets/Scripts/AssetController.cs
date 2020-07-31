using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConflictChronicle {

    [Serializable]
    public struct CC_InspectorAsset {
        public string id;
        public GameObject gameObject;
    }

    public enum CC_Available_CharacterType { MALE_AVERAGE, FEMALE_AVERAGE }

    [Serializable]
    public struct CC_Character_Type {
        public string id;
        public Vector3 nearHipLocation;
        public Vector3 farHipLocation;
        public Vector3 nearShoulderLocation;
        public Vector3 farShoulderLocation;
        public Sprite headFrontSprite;
        public Sprite headBackSprite;
        public Sprite stomachFrontSprite;
        public Sprite stomachBackSprite;
        public Sprite chestFrontSprite;
        public Sprite chestBackSprite;
    }

    public class AssetController : MonoBehaviour {

        public GameObject playerFocusPoint;
        public GameObject Camera;

        public GameObject worldFloor;

        public GameObject temp_cliff;
        public Image blackScreenOverlay;

        public CC_InspectorAsset[] editorChunkTypes;
        public CC_InspectorAsset[] editorEntityTypes;
        public CC_InspectorAsset[] editorObjectTypes;
        public CC_Character_Type[] editorCharacterTypes;

        public CC_InspectorAsset[] editor_TEMP_TileTypes;
        public Dictionary<string, GameObject> TEMP_TileTypes { get; private set; }

        public Dictionary<string, GameObject> chunkTypes { get; private set; }
        public Dictionary<string, GameObject> entityTypes { get; private set; }
        public Dictionary<string, GameObject> objectTypes { get; private set; }

        public Dictionary<string, CC_Character_Type> characterTypes { get; private set; }

        private void Awake () {
            chunkTypes = fillDictionaryFromArray (editorChunkTypes, chunkTypes);
            entityTypes = fillDictionaryFromArray (editorEntityTypes, entityTypes);
            objectTypes = fillDictionaryFromArray (editorObjectTypes, objectTypes);

            TEMP_TileTypes = fillDictionaryFromArray (editor_TEMP_TileTypes, TEMP_TileTypes);
            fillCharacterTypesDictionary ();
        }

        private Dictionary<String, GameObject> fillDictionaryFromArray (CC_InspectorAsset[] editorAssets, Dictionary<String, GameObject> dictionary) {
            if (dictionary == null) { dictionary = new Dictionary<String, GameObject> (); }
            foreach (CC_InspectorAsset asset in editorAssets) {
                dictionary.Add (asset.id, asset.gameObject);
            }
            return dictionary;
        }

        private void fillCharacterTypesDictionary () {
            if (characterTypes == null) { characterTypes = new Dictionary<String, CC_Character_Type> (); }
            foreach (CC_Character_Type characterType in editorCharacterTypes) {
                characterTypes.Add (characterType.id, characterType);
            }

        }
    }
}