using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle {

    public class SettingsController : MonoBehaviour {
        public string TerrainDefaultId = "FlatGrass";
        public int TerrainTopHeight = 128;
        public int TerrainLayerIndex = 9;
        public int TerrainTilesPerChunk = 3;

        public int MapDefaultSizeChunks = 600;
        public int MapChunkChangesPerFrame = 10;
        public float MapManageVisibilityDelay = 0f;

        public float sortMultiplierHeight = 1f;

        [Range (1, 30)]
        public int MapViewDistance = 3;

        public float CameraRotationDuration = 2f;
        public CC_CameraDirection CameraDefaultDirection = CC_CameraDirection.CameraDirectionsInOrder[0];
        public float CameraCatchupSpeed = 5f;

        public CC_Available_CharacterType CharacterDefaultType = CC_Available_CharacterType.MALE_AVERAGE;

        public CC_EnvironmentModel environment;

        public void InitializeEnvironment () {
            string savedSettingsLocation = Application.dataPath + @"/scripts/Settings/ConflictChronicleSettings.json";
            CC_EnvironmentModel env;
            if (File.Exists (savedSettingsLocation)) {
                env = JsonConvert.DeserializeObject<CC_EnvironmentModel> (File.ReadAllText (savedSettingsLocation));
                Debug.Log ("ENV EXISTS at " + savedSettingsLocation);
            } else {
                env = new CC_EnvironmentModel ();
                File.AppendAllText (savedSettingsLocation, JsonConvert.SerializeObject (env));
                Debug.Log ("ENV Created at " + savedSettingsLocation);
            }
            environment = env;
        }
    }
}