using System;
using System.Collections.Generic;

namespace ConflictChronicle {

    public class CC_Flat_Generator : CC_IWorldGenerator {

        public MapModel GenerateWorld (SettingsController settings) {
            return new MapModel ();

            //     var rand = new Random ();
            //     int sizeInChunks = settings.MapDefaultSizeChunks;
            //     MapModel finishedWorld = new MapModel ();
            //     finishedWorld.worldSizeChunksX = sizeInChunks;
            //     finishedWorld.worldSizeChunksZ = sizeInChunks;
            //     CC_TerrainChunk_Model[] terrain = new CC_TerrainChunk_Model[sizeInChunks * sizeInChunks];
            //     for (int x = 0; x < sizeInChunks; x++) {
            //         for (int z = 0; z < sizeInChunks; z++) {
            //             CC_TerrainChunk_Model newChunk = new CC_TerrainChunk_Model ();
            //             newChunk.xGridPosition = x;
            //             newChunk.zGridPosition = z;
            //             newChunk.assetId = settings.TerrainDefaultId;
            //             newChunk.yPosition = settings.TerrainTilesPerChunk;

            //             if (rand.NextDouble () > 0.75) {
            //                 newChunk.yPosition = 3;
            //             } else {
            //                 newChunk.yPosition = 0;
            //             }

            //             terrain[x * sizeInChunks + z] = newChunk;
            //         }
            //     }
            //     finishedWorld.terrain = terrain;
            //     return finishedWorld;
        }
    }
}