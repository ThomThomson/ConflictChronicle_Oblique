using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Timers;
using System;
using Pathfinding;

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
    public int rowLocation { get; set; }
    public int colLocation { get; set; }
    public GameObject chunkReference { get; set; }
    public List<ChunkObjectView> chunkObjectViews { get; set; }
    public int[,] chunkObjectLayoutIndices { get; set; }

    public MapChunkView(CC_MapChunkModel model, GameObject chunkReference, int rowLocation, int colLocation)
    {
        this.rowLocation = rowLocation;
        this.colLocation = colLocation;
        this.chunkReference = chunkReference;
        terrainKey = model.terrainKey;
        setupLayoutIndices();
        chunkObjectViews = new List<ChunkObjectView>();
        for (int i = 0; i < model.chunkObjects.Count; i++)
        {
            GameObject chunkObject = GameObject.Instantiate(CC_AssetMap.assetMap.objectTypes[model.chunkObjects[i].objectKey]);
            chunkObject.transform.parent = chunkReference.transform;
            string[] locationString = model.chunkObjects[i].location.Split(',');
            float xLocation = CC_MapController.getWorldPositionFromPositionInChunk(Int32.Parse(locationString[0]), colLocation);
            float zLocation = CC_MapController.getWorldPositionFromPositionInChunk(Int32.Parse(locationString[1]), colLocation);
            float yLocation = CC_MapController.getHeightFromRay(xLocation, zLocation);
            chunkObject.transform.position = new Vector3(xLocation, yLocation, zLocation);
            chunkObjectViews.Add(new ChunkObjectView(chunkObject, model.chunkObjects[i].objectKey));
            chunkObjectLayoutIndices[Int32.Parse(locationString[0]), Int32.Parse(locationString[1])] = i;
        }
    }

    public void setupLayoutIndices()
    {

        chunkObjectLayoutIndices = new int[CC_SettingsController.gameSettings.TILES_PER_CHUNK, CC_SettingsController.gameSettings.TILES_PER_CHUNK];
        for (int i = 0; i < chunkObjectLayoutIndices.GetLength(0); i++)
        {
            for (int j = 0; j < chunkObjectLayoutIndices.GetLength(1); j++)
            {
                chunkObjectLayoutIndices[i, j] = -1;
            }
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
    public string worldFolderLocation;
    public int mapRowLocation;
    public int mapColLocation;
    private CC_MapModel mapModel;
    private List<List<MapChunkView>> chunkViews;
    private int mapLoadPercentage;
    private bool savingToDisk = false;
    private Queue<MapChunkChange> pendingChanges;
    private GridGraph graph;

    public void Start()
    {
        instance = this;
        graph = (GridGraph)AstarPath.active.data.graphs[0];
    }

    private void Update()
    {
        if (!savingToDisk && pendingChanges != null)
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

    public void LoadMapIntoScene(CC_MapModel map, String worldFolderLocation, int mapRowLocation, int mapColLocation)
    {
        if (chunkViews != null)
        {
            SaveToDisk(false);
            DestroyAllWorldObjects();
        }
        this.mapRowLocation = mapRowLocation;
        this.mapColLocation = mapColLocation;
        this.worldFolderLocation = worldFolderLocation;
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
        return new MapChunkView(model, chunk, row, col);
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
        if (position.z > CC_SettingsController.gameSettings.TILES_PER_CHUNK * this.mapModel.mapTerrain.Count || position.z < 0) { return null; }
        int row = (int)(position.z / CC_SettingsController.gameSettings.TILES_PER_CHUNK);
        if (position.x > CC_SettingsController.gameSettings.TILES_PER_CHUNK * this.mapModel.mapTerrain[row].Count || position.x < 0) { return null; }
        int col = (int)(position.x / CC_SettingsController.gameSettings.TILES_PER_CHUNK);
        return chunkViews[row][col];
    }

    public static float getHeightFromRay(float x, float z)
    {
        Ray ray = new Ray(new Vector3(x, CC_SettingsController.gameSettings.TERRAIN_TOP_HEIGHT, z), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point.y;
        }
        else
        {
            return 0;
        }
    }

    public static int getPositionInChunkFromWorldPosition(float worldPosition, int chunkIndex)
    {
        return (int)(worldPosition - (chunkIndex * CC_SettingsController.gameSettings.TILES_PER_CHUNK));
    }

    public static float getWorldPositionFromPositionInChunk(int indexInChunk, int chunkIndex)
    {
        return (float)(indexInChunk + (chunkIndex * CC_SettingsController.gameSettings.TILES_PER_CHUNK));
    }

    public void SpawnWorldObject(Vector3 position, string objectId, bool overWrite = false)
    {
        Debug.Log("Spawning " + objectId + " at " + position);
        if (CC_AssetMap.assetMap.objectTypes.ContainsKey(objectId))
        {
            MapChunkView chunk = getChunkFromPosition(position);
            if (chunk == null) { return; }
            int localx = getPositionInChunkFromWorldPosition(position.x, chunk.colLocation);
            int localz = getPositionInChunkFromWorldPosition(position.z, chunk.rowLocation);
            int spawnx = (int)position.x;
            int spawnz = (int)position.z;
            if (chunk.chunkObjectLayoutIndices[localx, localz] == -1 || overWrite)
            {
                if (overWrite) { RemoveWorldObject(position); }
                GameObject spawnedObject = GameObject.Instantiate(CC_AssetMap.assetMap.objectTypes[objectId]);
                spawnedObject.transform.parent = chunk.chunkReference.transform;
                spawnedObject.transform.position = new Vector3((float)spawnx, getHeightFromRay((float)spawnx, (float)spawnz), (float)spawnz);
                chunk.chunkObjectViews.Add(new ChunkObjectView(spawnedObject, objectId));
                chunk.chunkObjectLayoutIndices[localx, localz] = chunk.chunkObjectViews.Count - 1;
                MapChunkChange change = new MapChunkChange(chunk, chunk.rowLocation, chunk.colLocation);
                pendingChanges.Enqueue(change);
                BoxCollider graphUpdateBox = spawnedObject.GetComponent<BoxCollider>();
                if (graphUpdateBox)
                {
                    graph.active.UpdateGraphs(graphUpdateBox.bounds);
                }
            }
        }
        else
        {
            throw new KeyNotFoundException("No asset mapping found for key: " + objectId);
        }
    }

    public void RemoveWorldObject(Vector3 position)
    {
        MapChunkView chunk = getChunkFromPosition(position);
        int deleteX = getPositionInChunkFromWorldPosition(position.x, chunk.colLocation);
        int deleteZ = getPositionInChunkFromWorldPosition(position.z, chunk.rowLocation);
        if (chunk.chunkObjectLayoutIndices[deleteX, deleteZ] != -1)
        {
            int index = chunk.chunkObjectLayoutIndices[deleteX, deleteZ];
            chunk.chunkObjectLayoutIndices[deleteX, deleteZ] = -1;
            BoxCollider graphUpdateBox = chunk.chunkObjectViews[index].chunkObjectReference.GetComponent<BoxCollider>();
            Bounds updateBounds = new Bounds(Vector3.zero, Vector3.zero);
            if (graphUpdateBox) { updateBounds = new Bounds(graphUpdateBox.bounds.center, graphUpdateBox.bounds.size); }
            Destroy(chunk.chunkObjectViews[index].chunkObjectReference);
            chunk.chunkObjectViews.RemoveAt(index);
            MapChunkChange change = new MapChunkChange(chunk, chunk.rowLocation, chunk.colLocation);
            pendingChanges.Enqueue(change);
            if (updateBounds.size != Vector3.zero)
            {
                graph.active.UpdateGraphs(updateBounds);
            }
        }
    }

    // -------------------------------------------------------------
    // Saving Functions
    // -------------------------------------------------------------
    public void SaveToDisk(bool useThread = true)
    {
        Debug.Log("Saving to file " + worldFolderLocation + @"\Maps\map-" + mapRowLocation + "-" + mapColLocation + ".json");
        savingToDisk = true;
        if (useThread) // thread makes saving during gameplay seamless
        {
            ThreadStart starter = (() => CC_MapModelUtil.SaveMapToFile(worldFolderLocation, mapModel, mapRowLocation, mapColLocation));
            starter += () =>
            {
                Debug.Log("Saving finished");
                savingToDisk = false;
            };
            Thread thread = new Thread(starter) { IsBackground = true };
            thread.Start();
        }
        else
        {
            CC_MapModelUtil.SaveMapToFile(worldFolderLocation, mapModel, mapRowLocation, mapColLocation);
            savingToDisk = false;
            Debug.Log("Saving Finished");
        }
    }

    public void DestroyAllWorldObjects()
    {
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

