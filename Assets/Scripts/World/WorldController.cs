using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle {

    [Serializable]
    public class WorldModelMeta {
        public int mapXLocation;
        public int mapZLocation;

        public Vector3 playerLocation;

        public WorldModelMeta () { }
    }

    public class WorldController : MonoBehaviour {
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
            } else {
                Directory.CreateDirectory (worldFolderLocation);
                worldModelMeta = new WorldModelMeta ();
                File.AppendAllText (metaFileLocation, JsonConvert.SerializeObject (worldModelMeta));
            }
            String currentMapLocation = worldFolderLocation + $@"\{worldModelMeta.mapXLocation}-{worldModelMeta.mapZLocation}";
            deserializeMap (currentMapLocation, playerFocusPoint);
        }

        public void deserializeMap (String currentMapFolder, Transform playerFocusPoint) {
            String heightmapFile = $@"{currentMapFolder}\{settingsController.TerrainHeightmapFileName}";
            Byte[, ] heightmapData;
            if (mapExistsAtLocation (currentMapFolder)) {
                heightmapData = fetchHeightmapFromFile (heightmapFile);
            } else {
                // TODO: generate blank world
                heightmapData = null;
            }
            currentMapModel = new MapModel ();
            currentMapModel.terrainHeight = heightmapData;

            mapController = this.gameObject.AddComponent<MapController> ();
            mapController.injectDependencies (settingsController, assetController, cameraController);

            mapController.loadMapIntoScene (currentMapModel);
            playerFocusPoint.position = worldModelMeta.playerLocation;

            mapController.startPlayer (playerFocusPoint);
        }

        public Boolean mapExistsAtLocation (String currentMapFolder) {
            // TODO: Map Objects, terrain types?
            Debug.Log (currentMapFolder);
            Debug.Log ($@"{currentMapFolder}\{settingsController.TerrainHeightmapFileName}");
            return Directory.Exists (currentMapFolder) && File.Exists ($@"{currentMapFolder}\{settingsController.TerrainHeightmapFileName}");
        }

        public MapModel generateBlankWorld () {
            CC_IWorldGenerator generator = new CC_Flat_Generator ();
            return generator.GenerateWorld (settingsController);
        }

        public Byte[, ] fetchHeightmapFromFile (String heightmapLocation) {
            if (!File.Exists (heightmapLocation)) {
                throw new ArgumentException ($"Heightmap file not found at {heightmapLocation}");
            }
            byte[] fileData = File.ReadAllBytes (heightmapLocation);
            Texture2D heightmapTexture = new Texture2D (2, 2);
            heightmapTexture.LoadImage (fileData);

            if (heightmapTexture.width % settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile != 0 ||
                heightmapTexture.height % settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile != 0) {
                throw new ArgumentException ($"Heightmap image must be divisible by {settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile}");
            }
            Color32[] pixels = heightmapTexture.GetPixels32 ();
            Byte[, ] heightmapData = new byte[heightmapTexture.height, heightmapTexture.width];
            for (int z = 0; z < heightmapTexture.height; z++) {
                for (int x = 0; x < heightmapTexture.width; x++) {
                    heightmapData[z, x] = pixels[z * heightmapTexture.width + x].b;
                }
            }
            return heightmapData;
        }

    }
}