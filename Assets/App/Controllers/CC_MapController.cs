using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Timers;
using System;

public class ChunkObjectView
{
    public GameObject chunkObjectReference { get; set; }
    public string chunkObjectID { get; set; }

    public ChunkObjectView(GameObject chunkObjectReference, string chunkObjectID)
    {
        this.chunkObjectID = chunkObjectID;
        this.chunkObjectReference = chunkObjectReference;
    }
}

public class MapChunkView
{
    public string terrainKey { get; set; }
    public GameObject chunkReference { get; set; }
    public List<ChunkObjectView> chunkObjectViews { get; set; }
    public int[,] chunkObjectLayoutIndices { get; set; }

    public MapChunkView(CC_MapChunkModel model, GameObject chunkReference)
    {
        this.chunkReference = chunkReference;
        terrainKey = model.terrainKey;
        chunkObjectLayoutIndices = new int[CC_SettingsController.gameSettings.TILES_PER_CHUNK, CC_SettingsController.gameSettings.TILES_PER_CHUNK];
        chunkObjectViews = new List<ChunkObjectView>();
        for (int i = 0; i < model.chunkObjects.Count; i++)
        {
            GameObject chunkObject = GameObject.Instantiate(CC_AssetMap.assetMap.objectTypes[model.chunkObjects[i].objectKey]);
            chunkObject.transform.parent = chunkReference.transform;
            string[] locationString = model.chunkObjects[i].location.Split(',');
            chunkObject.transform.localPosition = new Vector3(float.Parse(locationString[0]), 0, float.Parse(locationString[1]));
            chunkObjectViews.Add(new ChunkObjectView(chunkObject, model.chunkObjects[i].objectKey));
            chunkObjectLayoutIndices[Int32.Parse(locationString[0]), Int32.Parse(locationString[1])] = i;
        }
    }
}

public class MapChunkChange
{
    public MapChunkView mapChunkView { get; set; }
    public int rowToSaveTo { get; set; }
    public int colToSaveTo { get; set; }

    public MapChunkChange(MapChunkView mapChunkView, int rowToSaveTo, int colToSaveTo)
    {
        this.mapChunkView = mapChunkView;
        this.rowToSaveTo = rowToSaveTo;
        this.colToSaveTo = colToSaveTo;
    }
}

public class CC_MapController : MonoBehaviour
{
    public static CC_MapController instance;
    public string mapLocation;
    private CC_MapModel mapModel;
    private List<List<MapChunkView>> chunkViews;
    private int mapLoadPercentage;
    private bool savingToJson = false;
    private Queue<MapChunkChange> pendingChanges;

    public void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (!savingToJson && pendingChanges != null)
        {
            int recordsChanged = 0;
            while (recordsChanged < CC_SettingsController.gameSettings.MAP_CHANGES_SAVE_PER_FRAME && pendingChanges.Count != 0)
            {
                MapChunkChange change = pendingChanges.Dequeue();
                mapModel.mapTerrain[change.rowToSaveTo][change.colToSaveTo] = new CC_MapChunkModel(change.mapChunkView);
                recordsChanged++;
            }
        }
        
    }

    public void LoadMapIntoScene(CC_MapModel map, String mapLocation)
    {
        this.mapLocation = mapLocation;
        if (chunkViews != null)
        {
            SaveToJSON();
            DestroyAllWorldObjects();
        }
        chunkViews = new List<List<MapChunkView>>();
        pendingChanges = new Queue<MapChunkChange>();
        mapModel = map;
        StartCoroutine("LoadMapChunks");
    }

    // -------------------------------------------------------------
    // Initial Load Functions
    // -------------------------------------------------------------
    public IEnumerator LoadMapChunks()
    {
        int loadedSinceLastFrame = 0;
        for (int row = 0; row < mapModel.mapTerrain.Count; row++)
        {
            List<MapChunkView> chunkReferenceRow = new List<MapChunkView>();
            for (int col = 0; col < mapModel.mapTerrain[row].Count; col++)
            {
                if (!(loadedSinceLastFrame < CC_SettingsController.gameSettings.LOADING_CHUNKS_PER_FRAME))
                {
                    loadedSinceLastFrame = 0;
                    yield return 0;
                }
                chunkReferenceRow.Add(setUpChunkView(mapModel.mapTerrain[row][col], row, col));
                loadedSinceLastFrame++;
            }
            chunkViews.Add(chunkReferenceRow);
        }
        AstarPath.active.Scan();
    }

    public MapChunkView setUpChunkView(CC_MapChunkModel model, int row, int col)
    {
        GameObject folder = getChunkFolder();
        GameObject chunk = GameObject.Instantiate(CC_AssetMap.assetMap.chunkTypes[model.terrainKey]);
        chunk.name = "Terrain Chunk at: " + row + " - " + col;
        chunk.transform.parent = folder.transform;
        chunk.transform.position = new Vector3(col * CC_SettingsController.gameSettings.TILES_PER_CHUNK, 0, row * CC_SettingsController.gameSettings.TILES_PER_CHUNK);
        return new MapChunkView(model, chunk);
    }

    public GameObject getChunkFolder()
    {
        GameObject folder = GameObject.FindWithTag("TerrainChunkFolder");
        if (folder == null)
        {
            folder = new GameObject();
            folder.name = "CC_WorldFolder";
            folder.tag = "TerrainChunkFolder";
        }
        return folder;
    }


    // -------------------------------------------------------------
    // Editing Functions
    // -------------------------------------------------------------
    public MapChunkView getChunkFromPosition(Vector3 position)
    {
        if (position.z > CC_SettingsController.gameSettings.TILES_PER_CHUNK * this.mapModel.mapTerrain.Count) { return null; }
        int row = (int)(position.z / CC_SettingsController.gameSettings.TILES_PER_CHUNK);
        if (position.x > CC_SettingsController.gameSettings.TILES_PER_CHUNK * this.mapModel.mapTerrain[row].Count) { return null; }
        int col = (int)(position.x / CC_SettingsController.gameSettings.TILES_PER_CHUNK);
        return chunkViews[row][col];
    }

    public void SpawnWorldObject(Vector3 position, string objectId)
    {
        if (CC_AssetMap.assetMap.objectTypes.ContainsKey(objectId))
        {
            GameObject spawnedObject = GameObject.Instantiate(CC_AssetMap.assetMap.objectTypes[objectId]);
            int spawnx = (int)position.x;
            int spawnz = (int)position.z;
            MapChunkView chunk = getChunkFromPosition(position);
            spawnedObject.transform.parent = chunk.chunkReference.transform;
            spawnedObject.transform.position = new Vector3((float)spawnx, 0, (float)spawnz);
        }
        else
        {
            throw new KeyNotFoundException("No asset mapping found for key: " + objectId);
        }
    }

    public void RemoveWorldObject(Vector3 position)
    {

    }



    // -------------------------------------------------------------
    // Saving Functions
    // -------------------------------------------------------------
    public void SaveToJSON()
    {

    }

    public void DestroyAllWorldObjects()
    {
        Debug.Log("Destroying everything");
        this.chunkViews.ForEach((List<MapChunkView> viewRow) =>
        {
            viewRow.ForEach((MapChunkView chunkView) =>
            {
                chunkView.chunkObjectViews.ForEach((ChunkObjectView objectView) =>
                {
                    Destroy(objectView.chunkObjectReference);
                });
                Destroy(chunkView.chunkReference);
            });
        });
    }
}

