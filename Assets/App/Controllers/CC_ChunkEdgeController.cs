using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ConflictChronicle.Views;
using System;

namespace ConflictChronicle.Controllers
{

    public class CC_ChunkEdgeController : MonoBehaviour
    {

        public SpriteRenderer[] edgeSprites;
        public Vector2[] relativeChunkLocations;

        private List<MapChunkView> chunkViews;
        private MapChunkView chunkView;

        public void recalculateEdges(bool propigate = false)
        {
            if (edgeSprites.Length != relativeChunkLocations.Length)
            {
                throw new ArgumentException("edgeSprites and relativeChunkLocations must be the same length");
            }
            if (chunkView == null)
            {
                chunkView = CC_MapController.instance.getChunkFromPosition(transform.position);
                if (chunkView == null)
                {
                    throw new ArgumentException("Edge Controller should not be used outside of a chunk gameobject");
                }
                chunkViews = new List<MapChunkView>();
            }
            for (int i = 0; i < edgeSprites.Length; i++)
            {
                if (chunkViews.Count <= i)
                {
                    Vector2 relativeChunkLocation = relativeChunkLocations[i];
                    chunkViews.Add(CC_MapController.instance.GetChunkFromIndices(
                        (int)(chunkView.rowLocation + relativeChunkLocation.x),
                        (int)(chunkView.colLocation + relativeChunkLocation.y)
                    ));
                    Debug.Log("Found: " + chunkViews[chunkViews.Count - 1]);
                }
                if (chunkViews[i] == null)
                {
                    // this chunk is on the edge of the map, so it's edge facing that direction should be enabled
                    edgeSprites[i].enabled = true;
                }
                else
                {
                    edgeSprites[i].enabled = chunkViews[i].chunkReference.transform.position.y != chunkView.chunkReference.transform.position.y;
                    if(propigate)
                    {
                        chunkViews[i].refreshEdges(); // propigate only sends once.
                    }
                }
            }

            //TODO: Create separate sprite sorter for terrain and for world objects, make active/passive a toggle.
            //TODO: World object sprite sorters will all use linecasts.
            //TODO: Make a method for recalculating sorting for all objects after recalculate edges
            //TODO: Save changes to chunk height to file.
        }
    }
}