using UnityEngine;
using ConflictChronicle.Models;
using ConflictChronicle.Controllers;
using System;

namespace ConflictChronicle.Views
{

    public class MapChunkView
    {
        public string terrainKey { get; set; }
        public int rowLocation { get; set; }
        public int colLocation { get; set; }
        public GameObject chunkReference { get; set; }
        public ChunkObjectView[,] chunkObjectViews { get; set; }
        public CC_ChunkEdgeController edgeController { get; set; }

        public MapChunkView(CC_MapChunkModel model, GameObject chunkReference, int rowLocation, int colLocation)
        {
            this.rowLocation = rowLocation;
            this.colLocation = colLocation;
            this.chunkReference = chunkReference;
            terrainKey = model.terrainKey;
            edgeController = this.chunkReference.GetComponent<CC_ChunkEdgeController>();
            chunkObjectViews = new ChunkObjectView[CC_SettingsController.gameSettings.TILES_PER_CHUNK, CC_SettingsController.gameSettings.TILES_PER_CHUNK];
            for (int i = 0; i < model.chunkObjects.Count; i++)
            {
                GameObject chunkObject = GameObject.Instantiate(CC_AssetMap.assetMap.objectTypes[model.chunkObjects[i].objectKey]);
                chunkObject.transform.parent = chunkReference.transform;
                string[] locationString = model.chunkObjects[i].location.Split(',');
                float xLocation = CC_MapController.getWorldPositionFromPositionInChunk(Int32.Parse(locationString[1]), colLocation);
                float zLocation = CC_MapController.getWorldPositionFromPositionInChunk(Int32.Parse(locationString[0]), rowLocation);
                float yLocation = CC_CameraController.worldTopToGround(new Vector3(xLocation, 0,  zLocation));
                chunkObject.transform.position = new Vector3(xLocation, yLocation, zLocation);
                chunkObjectViews[Int32.Parse(locationString[0]), Int32.Parse(locationString[1])] = new ChunkObjectView(chunkObject, model.chunkObjects[i].objectKey);
            }
        }

        public void refreshEdges(bool propigate = false)
        {
            if(edgeController != null)
            {
                edgeController.recalculateEdges(propigate);
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
}