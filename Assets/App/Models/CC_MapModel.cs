using System.Collections;
using System.Collections.Generic;
using System.IO;
using ConflictChronicle.Controllers;
using ConflictChronicle.Views;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle.Models
{

    [System.Serializable]
    public class CC_MapObjectModel
    {
        public string location;
        public string objectKey;

        public CC_MapObjectModel(string location, string objectKey)
        {
            this.location = location;
            this.objectKey = objectKey;
        }
    }

    [System.Serializable]
    public class CC_MapChunkModel
    {
        public string terrainKey { get; set; }
        public int chunkHeight { get; set; }
        public List<CC_MapObjectModel> chunkObjects { get; set; }

        public CC_MapChunkModel(string defaultTerrainKey)
        {
            this.terrainKey = defaultTerrainKey;
            this.chunkHeight = 0;
            chunkObjects = new List<CC_MapObjectModel>();
        }

        public CC_MapChunkModel(MapChunkView fromView)
        {
            this.terrainKey = fromView.terrainKey;
            this.chunkHeight = (int)fromView.chunkReference.transform.position.y;
            chunkObjects = new List<CC_MapObjectModel>();
            for (int row = 0; row < fromView.chunkObjectViews.GetLength(0); row++)
            {
                for (int col = 0; col < fromView.chunkObjectViews.GetLength(1); col++)
                {
                    if (fromView.chunkObjectViews[row, col] != null)
                    {
                        this.chunkObjects.Add(new CC_MapObjectModel(row + "," + col, fromView.chunkObjectViews[row, col].chunkObjectID));
                    }
                }
            }
        }

        public CC_MapChunkModel() { }
    }

    [System.Serializable]
    public class CC_MapModel
    {
        public List<List<CC_MapChunkModel>> mapTerrain;
    }

    public class CC_MapModelUtil
    {
        public static CC_MapModel LoadMapFromFile(int mapRowLocation, int mapColLocation, string worldFolderLocation)
        {
            string mapLocation = worldFolderLocation + @"\Maps\map-" + mapRowLocation + "-" + mapColLocation + ".json";
            Debug.Log("Loading from " + mapLocation);
            if (File.Exists(mapLocation))
            {
                string mapString = System.IO.File.ReadAllText(mapLocation);
                CC_MapModel model = JsonConvert.DeserializeObject<CC_MapModel>(mapString);
                return model;
            }
            else
            {
                // generate default map...
                CC_MapModel map = new CC_MapModel();
                Debug.Log("Temp! No map found, generating a new one");
                List<List<CC_MapChunkModel>> worldData = new List<List<CC_MapChunkModel>>();
                for (int row = 0; row < CC_SettingsController.gameSettings.DEFAULT_MAP_SIZE_CHUNKS; row++)
                {
                    List<CC_MapChunkModel> chunkRow = new List<CC_MapChunkModel>();
                    for (int col = 0; col < CC_SettingsController.gameSettings.DEFAULT_MAP_SIZE_CHUNKS; col++)
                    {
                        chunkRow.Add(new CC_MapChunkModel(CC_SettingsController.gameSettings.TERRAIN_DEFAULT_ID));
                    }
                    worldData.Add(chunkRow);
                }
                map.mapTerrain = worldData;
                string mapString = JsonConvert.SerializeObject(map);
                System.IO.File.WriteAllText(mapLocation, mapString);
                return map;
            }
        }

        public static void SaveMapToFile(string worldFolderLocation, CC_MapModel mapToSave, int mapRowLocation, int mapColLocation, bool overWrite = true)
        {
            string mapLocation = worldFolderLocation + @"\Maps\map-" + mapRowLocation + "-" + mapColLocation + ".json";
            string mapString = JsonConvert.SerializeObject(mapToSave);
            System.IO.File.WriteAllText(mapLocation, mapString);
        }
    }
}