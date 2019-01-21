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

using ConflictChronicle.Models;
using ConflictChronicle.Views;

namespace ConflictChronicle.Controllers
{

    public class CC_MapController : MonoBehaviour
    {
        public static CC_MapController instance;
        public string worldFolderLocation;
        public int mapRowLocation;
        public int mapColLocation;
        public bool finishedLoad;
        private CC_MapModel mapModel;
        private List<List<MapChunkView>> chunkViews;
        private int mapLoadPercentage;
        private bool savingToDisk = false;
        private Queue<MapChunkChange> pendingChanges;
        private GridGraph graph;

        public void Start()
        {
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
            if(instance == null)
            {
                instance = this;
            }
            this.finishedLoad = false;
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
            this.finishedLoad = true;
            AstarPath.active.Scan();
            StartCoroutine("EdgeDetectAllChunks");
        }

        public IEnumerator EdgeDetectAllChunks()
        {
            int edgeCalcSinceLastFrame = 0;
            for (int row = 0; row < mapModel.mapTerrain.Count; row++)
            {
                for (int col = 0; col < mapModel.mapTerrain[row].Count; col++)
                {
                    if (!(edgeCalcSinceLastFrame < CC_SettingsController.gameSettings.LOADING_CHUNKS_PER_FRAME))
                    {
                        edgeCalcSinceLastFrame = 0;
                        yield return 0;
                    }
                    chunkViews[row][col].refreshEdges();
                    edgeCalcSinceLastFrame++;
                }
            }
        }

        public void EdgeDetectChunkFromLocation(Vector3 location)
        {
            MapChunkView chunk = getChunkFromPosition(location);
            chunk.refreshEdges(true);
        }

        public MapChunkView setUpChunkView(CC_MapChunkModel model, int row, int col)
        {
            GameObject folder = getChunkFolder();
            GameObject chunk = GameObject.Instantiate(CC_AssetMap.assetMap.chunkTypes[model.terrainKey]);
            chunk.name = "Terrain Chunk at: " + row + " - " + col;
            chunk.transform.parent = folder.transform;
            chunk.transform.position = new Vector3(col * CC_SettingsController.gameSettings.TILES_PER_CHUNK,
                                                   (float)model.chunkHeight,
                                                   row * CC_SettingsController.gameSettings.TILES_PER_CHUNK);
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

        public static int getPositionInChunkFromWorldPosition(float worldPosition, int chunkIndex)
        {
            return (int)(worldPosition - (chunkIndex * CC_SettingsController.gameSettings.TILES_PER_CHUNK));
        }

        public static int getWorldPositionFromPositionInChunk(int indexInChunk, int chunkIndex)
        {
            return (int)(indexInChunk + (chunkIndex * CC_SettingsController.gameSettings.TILES_PER_CHUNK));
        }

        public void SpawnWorldObject(Vector3 position, string objectId, bool overWrite = false)
        {
            if (CC_AssetMap.assetMap.objectTypes.ContainsKey(objectId))
            {
                MapChunkView chunk = getChunkFromPosition(position);
                if (chunk == null) { return; }
                int localz = getPositionInChunkFromWorldPosition(position.z, chunk.rowLocation);
                int localx = getPositionInChunkFromWorldPosition(position.x, chunk.colLocation);
                int spawnx = (int)position.x;
                int spawnz = (int)position.z;
                if (chunk.chunkObjectViews[localz, localx] == null || overWrite)
                {
                    if (overWrite) { RemoveWorldObject(position); }
                    GameObject spawnedObject = GameObject.Instantiate(CC_AssetMap.assetMap.objectTypes[objectId]);
                    spawnedObject.transform.parent = chunk.chunkReference.transform;
                    spawnedObject.transform.position = new Vector3((float)spawnx, CC_CameraController.worldTopToGround(position), (float)spawnz);
                    chunk.chunkObjectViews[localz, localx] = new ChunkObjectView(spawnedObject, objectId);
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
            int localz = getPositionInChunkFromWorldPosition(position.z, chunk.rowLocation);
            int localx = getPositionInChunkFromWorldPosition(position.x, chunk.colLocation);
            if (chunk.chunkObjectViews[localz, localx] != null)
            {
                BoxCollider graphUpdateBox = chunk.chunkObjectViews[localz, localx].chunkObjectReference.GetComponent<BoxCollider>();
                Bounds updateBounds = new Bounds(Vector3.zero, Vector3.zero);
                if (graphUpdateBox) { updateBounds = new Bounds(graphUpdateBox.bounds.center, graphUpdateBox.bounds.size); }
                Destroy(chunk.chunkObjectViews[localz, localx].chunkObjectReference);
                chunk.chunkObjectViews[localz, localx] = null;
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
                    for (int row = 0; row < chunkView.chunkObjectViews.GetLength(0); row++)
                    {
                        for (int col = 0; col < chunkView.chunkObjectViews.GetLength(1); col++)
                        {
                            if (chunkView.chunkObjectViews[row, col] != null)
                                Destroy(chunkView.chunkObjectViews[row, col].chunkObjectReference);
                        }
                    }
                    Destroy(chunkView.chunkReference);
                });
            });
        }

        public MapChunkView GetChunkFromIndices(int row, int col)
        {
            if(row >= chunkViews.Count || row < 0 || col >= chunkViews[0].Count || col < 0)
            {
                return null;
            }
            else
            {
                return chunkViews[row][col];
            }
        }
    }
}