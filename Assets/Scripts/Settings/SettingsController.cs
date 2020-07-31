using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle {

    public class SettingsController : MonoBehaviour {
        public int TerrainTopHeight = 32;
        public int TerrainTilesPerChunk = 8;
        public int TerrainMetersPerTile = 2;
        public string TerrainHeightmapFileName = "terrain_height.png";

        public string TerrainDefaultId;
        public int MapDefaultSizeChunks;

        public bool loadMap;
        public Vector3 noMapPlayerStart;

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