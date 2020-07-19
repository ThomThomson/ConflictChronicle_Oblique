using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle {

    [Serializable]
    public class WorldModelMeta {
        public int mapXLocation;
        public int mapZLocation;

        public Vector3 playerLocation;

        public WorldModelMeta (MapModel initialMap, SettingsController settings) {
            mapXLocation = 0;
            mapZLocation = 0;

            Debug.Log (initialMap.worldSizeChunksX);
            Debug.Log (settings.TerrainTilesPerChunk);

            playerLocation = new Vector3 (
                (initialMap.worldSizeChunksX * settings.TerrainTilesPerChunk) / 2,
                10,
                (initialMap.worldSizeChunksZ * settings.TerrainTilesPerChunk) / 2
            );
        }

        public WorldModelMeta () { }
    }

    public class WorldController : MonoBehaviour {

        // This will eventually be used to keep track of all maps, and which map the player is currently in etc.
        private SettingsController settingsController;
        private AssetController assetController;
        private CameraController cameraController;
        private MapController mapController;

        private WorldModelMeta worldModelMeta;
        private MapModel currentMapModel;

        public Vector3 PlayerStartingPosition { get { return _playerStartingPosition; } }
        private Vector3 _playerStartingPosition;

        public void injectDependencies (SettingsController settingsController, AssetController assetController, CameraController cameraController) {
            this.settingsController = settingsController;
            this.assetController = assetController;
            this.cameraController = cameraController;
        }

        public void loadWorld (String worldFolderLocation, Transform playerFocusPoint) {
            String metaFileLocation = worldFolderLocation + @"\world_model_meta.json";
            if (Directory.Exists (worldFolderLocation) && File.Exists (metaFileLocation)) {
                worldModelMeta = JsonConvert.DeserializeObject<WorldModelMeta> (File.ReadAllText (metaFileLocation));
                String currentMapLocation = worldFolderLocation + $@"\{worldModelMeta.mapXLocation}-{worldModelMeta.mapZLocation}.json";
                if (File.Exists (currentMapLocation)) {
                    currentMapModel = JsonConvert.DeserializeObject<MapModel> (File.ReadAllText (currentMapLocation));
                } else {
                    Debug.Log ("generating blank world");
                    currentMapModel = generateBlankWorld ();
                }
            } else {
                Directory.CreateDirectory (worldFolderLocation);
                Debug.Log ("generating blank world");
                currentMapModel = generateBlankWorld ();
                worldModelMeta = new WorldModelMeta (currentMapModel, settingsController);
                String currentMapLocation = worldFolderLocation + $@"\{worldModelMeta.mapXLocation}-{worldModelMeta.mapZLocation}.json";
                File.AppendAllText (currentMapLocation, JsonConvert.SerializeObject (currentMapModel));
                File.AppendAllText (metaFileLocation, JsonConvert.SerializeObject (worldModelMeta));
            }
            loadMap (playerFocusPoint);
        }

        public void loadMap (Transform playerFocusPoint) {
            mapController = this.gameObject.AddComponent<MapController> ();
            mapController.injectDependencies (settingsController, assetController, cameraController);

            mapController.loadWorld (currentMapModel);
            playerFocusPoint.position = worldModelMeta.playerLocation;

            mapController.startPlayer (playerFocusPoint);
        }

        public MapModel generateBlankWorld () {
            CC_IWorldGenerator generator = new CC_Flat_Generator ();
            return generator.GenerateWorld (settingsController);
        }

    }
}