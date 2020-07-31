using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle {

    [Serializable]
    public class MapModel {
        public Byte[, ] terrainHeight;
    }

    public class MapController : MonoBehaviour {
        public Byte[, ] terrainHeight;
        public TerrainChunk[, ] terrianChunks;
        private Queue<TerrainChunk> chunkViewChangeQueue = new Queue<TerrainChunk> ();
        public int xSizeInChunks;
        public int zSizeInChunks;
        public int xSize;
        public int zSize;

        private Transform playerFocusPoint;
        private GameObject worldFolder;
        private bool worldActive = false;

        private SettingsController settingsController;
        private AssetController assetController;
        private CameraController cameraController;

        public void injectDependencies (SettingsController settingsController, AssetController assetController, CameraController cameraController) {
            this.settingsController = settingsController;
            this.cameraController = cameraController;
            this.assetController = assetController;
            // this.cameraController.OnCameraRotate += onCameraRotate;
        }

        public void loadMapIntoScene (MapModel mapModel) {
            worldFolder = new GameObject ();
            worldFolder.name = "CC_WorldFolder";
            worldFolder.tag = "TerrainChunkFolder";

            terrainHeight = mapModel.terrainHeight;

            zSize = terrainHeight.GetLength (0);
            xSize = terrainHeight.GetLength (1);
            xSizeInChunks = xSize / (settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile);
            xSizeInChunks = zSize / (settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile);

            terrianChunks = new TerrainChunk[zSizeInChunks, xSizeInChunks];
            for (int z = 0; z < zSizeInChunks; z++) {
                for (int x = 0; x < xSizeInChunks; x++) {
                    GameObject currentChunkObject = new GameObject ();
                    currentChunkObject.transform.position = new Vector3 (x * settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile, 0,
                        z * settingsController.TerrainTilesPerChunk * settingsController.TerrainMetersPerTile);
                    currentChunkObject.name = $"Terrain Chunk {z} - {x}";
                    currentChunkObject.transform.parent = worldFolder.transform;
                    TerrainChunk currentChunk = currentChunkObject.AddComponent<TerrainChunk> ();
                    currentChunk.isVisible = false;
                    currentChunk.xGridPosition = x;
                    currentChunk.zGridPosition = z;
                    terrianChunks[z, x] = currentChunk;
                }
            }

            for (int z = 0; z < zSize; z += 2) {
                for (int x = 0; x < xSize; x += 2) {

                    int[] heightsThisTile = new int[4];
                    heightsThisTile[0] = scaleHeight (terrainHeight[z, x]);
                    heightsThisTile[1] = scaleHeight (terrainHeight[z + 1, x]);
                    heightsThisTile[2] = scaleHeight (terrainHeight[z + 1, x + 1]);
                    heightsThisTile[3] = scaleHeight (terrainHeight[z, x + 1]);

                    int lowestHeight = settingsController.TerrainTopHeight;
                    for (int i = 0; i < heightsThisTile.Length; i++) {
                        if (heightsThisTile[i] < lowestHeight) {
                            lowestHeight = heightsThisTile[i];
                        }
                    }

                    int[] relativeHeightsThisTile = new int[4];
                    for (int i = 0; i < heightsThisTile.Length; i++) {
                        relativeHeightsThisTile[i] = heightsThisTile[i] - lowestHeight;
                    }

                    String heightsKey = String.Join ("-", relativeHeightsThisTile);

                    if (assetController.TEMP_TileTypes.ContainsKey (heightsKey)) {
                        GameObject test = Instantiate (assetController.TEMP_TileTypes[heightsKey]);
                        test.transform.position = new Vector3 (x, lowestHeight, z);
                        test.transform.parent = worldFolder.transform;
                        // for (int i = 0; i < heightsThisTile.Length; i++) {

                        // }
                    } else {
                        GameObject Test1 = Instantiate (assetController.temp_cliff);
                        Test1.transform.position = new Vector3 (x, heightsThisTile[0], z);
                        Test1.transform.parent = worldFolder.transform;
                        GameObject Test2 = Instantiate (assetController.temp_cliff);
                        Test2.transform.position = new Vector3 (x, heightsThisTile[1], z + 1);
                        Test2.transform.parent = worldFolder.transform;
                        GameObject Test3 = Instantiate (assetController.temp_cliff);
                        Test3.transform.position = new Vector3 (x + 1, heightsThisTile[2], z + 1);
                        Test3.transform.parent = worldFolder.transform;
                        GameObject Test4 = Instantiate (assetController.temp_cliff);
                        Test4.transform.position = new Vector3 (x + 1, heightsThisTile[3], z);
                        Test4.transform.parent = worldFolder.transform;
                    }
                }
            }

            generateBorders ();
        }

        public int scaleHeight (int originalHeight) {;
            return Mathf.FloorToInt (originalHeight * settingsController.TerrainTopHeight / 255);
        }

        public void startPlayer (Transform playerFocusPoint) {
            this.playerFocusPoint = playerFocusPoint;
            // this.visibleChunks = new List<CC_TerrainChunk> ();
            worldActive = true;
            // StartCoroutine ("manageVisibilityCoroutine");
            // LeanTween.alpha (assetController.blackScreenOverlay.rectTransform, 0, 1f);
        }

        // public void cleanVisibleChunksAndQueue () {
        //     chunkViewChangeQueue.Clear ();
        //     foreach (CC_TerrainChunk chunk in visibleChunks) {
        //         chunk.isVisible = false;
        //         chunk.LeaveView (this);
        //     }
        // }

        // private IEnumerator manageVisibilityCoroutine () {
        //     while (worldActive) {
        //         manageVisibility ();
        //         yield return null;
        //     }
        // }

        // public void Update () {
        //     if (worldActive) {
        //         int changesThisIteration = 0;
        //         while (changesThisIteration < settingsController.MapChunkChangesPerFrame && chunkViewChangeQueue.Count != 0) {
        //             CC_TerrainChunk chunkToChange = chunkViewChangeQueue.Dequeue ();
        //             if (chunkToChange.isVisible) {
        //                 chunkToChange.EnterView (this);
        //                 visibleChunks.Add (chunkToChange);
        //             } else {
        //                 chunkToChange.LeaveView (this);
        //                 visibleChunks.Remove (chunkToChange);
        //             }
        //             changesThisIteration++;
        //         }
        //     }
        // }

        private void generateBorders () {
            GameObject borderWalls = new GameObject ();
            borderWalls.name = "CC_Border_Walls";

            Vector3[] startingPositions = {
                new Vector3 (xSize / 2, settingsController.TerrainTopHeight / 2, 0), // north side, EAST Facing
                new Vector3 (xSize, settingsController.TerrainTopHeight / 2, zSize / 2), // NORTH Facing
                new Vector3 (xSize / 2, settingsController.TerrainTopHeight / 2, zSize), // WEST Facing
                new Vector3 (0, settingsController.TerrainTopHeight / 2, zSize / 2) // SOUTH Facing
            };

            for (int i = 0; i < startingPositions.Length; i++) {
                GameObject currentWall = new GameObject ();
                currentWall.name = $"CC_Wall_{CC_Compass.spiralDirections[i].directionName}";
                currentWall.transform.position = startingPositions[i];
                currentWall.AddComponent<BoxCollider> ();
                Vector3 scale = new Vector3 (
                    (xSize * Mathf.Abs (CC_Compass.spiralDirections[i].direction.x)) + 1,
                    settingsController.TerrainTopHeight,
                    (zSize * Mathf.Abs (CC_Compass.spiralDirections[i].direction.z)) + 1
                );
                currentWall.transform.localScale = scale;
                currentWall.transform.parent = borderWalls.transform;
            }

            GameObject ceiling = new GameObject ();
            ceiling.name = "CC_CEILING";
            ceiling.transform.position = new Vector3 (
                xSize / 2,
                settingsController.TerrainTopHeight - 0.5f,
                zSize / 2
            );
            ceiling.AddComponent<BoxCollider> ();
            Vector3 ceilingScale = new Vector3 (xSize, 1, zSize);
            ceiling.transform.localScale = ceilingScale;
            ceiling.transform.parent = borderWalls.transform;

            GameObject gameFloor = Instantiate (assetController.worldFloor);
            gameFloor.name = "CC_FLOOR";
            gameFloor.transform.position = new Vector3 (xSize / 2, -0.5f, zSize / 2);
            Vector3 floorScale = new Vector3 (xSize, 1, zSize);
            gameFloor.transform.localScale = floorScale;
            gameFloor.transform.parent = borderWalls.transform;
            Vector2 textureScale = new Vector2 (xSize / 4, zSize / 4);
            gameFloor.GetComponent<Renderer> ().material.mainTextureScale = textureScale;
        }

        // private void manageVisibility () {
        //     CC_TerrainChunk playerLocationChunk = getTerrainChunkFromLocation (this.playerFocusPoint.transform.position);
        //     if (playerLocationChunk == null) { return; }
        //     if (!playerLocationChunk.isVisible) {
        //         playerLocationChunk.isVisible = true;
        //         chunkViewChangeQueue.Enqueue (playerLocationChunk);
        //     }

        //     zIntVector2 maxViewNorthWest = new zIntVector2 (
        //         Mathf.Clamp (playerLocationChunk.xGridPosition - settingsController.MapViewDistance, 0, xSizeInChunks - 1),
        //         Mathf.Clamp (playerLocationChunk.zGridPosition - settingsController.MapViewDistance, 0, zSizeInChunks - 1)
        //     );

        //     zIntVector2 maxViewSouthEast = new zIntVector2 (
        //         Mathf.Clamp (playerLocationChunk.xGridPosition + settingsController.MapViewDistance, 0, xSizeInChunks - 1),
        //         Mathf.Clamp (playerLocationChunk.zGridPosition + settingsController.MapViewDistance, 0, zSizeInChunks - 1)
        //     );

        //     zIntVector2 currentLocation = new zIntVector2 (playerLocationChunk.xGridPosition, playerLocationChunk.zGridPosition);

        //     int magnitude = 1;
        //     int currentDirectionIndex = 0;
        //     bool finished = false;
        //     while (!finished) {
        //         for (int cycle = 0; cycle < 2; cycle++) {
        //             if (finished) { break; }
        //             for (int i = 0; i < magnitude; i++) {
        //                 if (finished) { break; }
        //                 currentLocation.x += CC_Compass.spiralDirections[currentDirectionIndex].direction.x;
        //                 currentLocation.z += CC_Compass.spiralDirections[currentDirectionIndex].direction.z;
        //                 CC_TerrainChunk chunkToAdd = GetTerrainChunk (currentLocation.x, currentLocation.z);
        //                 if (chunkToAdd != null && !chunkToAdd.isVisible) {
        //                     if (chunkIsOutOfViewBounds (chunkToAdd, maxViewNorthWest, maxViewSouthEast)) {
        //                         finished = true;
        //                     } else {
        //                         if (!chunkToAdd.isVisible) {
        //                             chunkToAdd.isVisible = true;
        //                             chunkViewChangeQueue.Enqueue (chunkToAdd);
        //                         }
        //                     }
        //                 }
        //             }
        //             currentDirectionIndex = ((currentDirectionIndex + CC_Compass.spiralDirections.Length) + 1) % CC_Compass.spiralDirections.Length;
        //         }
        //         magnitude++;
        //     }

        //     if (visibleChunks.Count != 0) {
        //         for (int i = visibleChunks.Count - 1; i >= 0; i--) {
        //             CC_TerrainChunk visibleChunk = visibleChunks[i];
        //             if (chunkIsOutOfViewBounds (visibleChunk, maxViewNorthWest, maxViewSouthEast)) {
        //                 if (visibleChunk.isVisible) {
        //                     visibleChunk.isVisible = false;
        //                     chunkViewChangeQueue.Enqueue (visibleChunk);
        //                 }
        //             }
        //         }
        //     }
        // }

        // utilities
        // public bool chunkIsOutOfViewBounds (CC_TerrainChunk chunk, zIntVector2 maxViewNorthWest, zIntVector2 maxViewSouthEast) {
        //     if (chunk.zGridPosition > maxViewSouthEast.z ||
        //         chunk.zGridPosition < maxViewNorthWest.z ||
        //         chunk.xGridPosition > maxViewSouthEast.x ||
        //         chunk.xGridPosition < maxViewNorthWest.x
        //     ) {
        //         return true;
        //     }
        //     return false;
        // }

        // public CC_TerrainChunk GetTerrainChunk (int x, int z) {
        //     if (x >= 0 && x < xSizeInChunks && z >= 0 && z < zSizeInChunks) {
        //         return this.terrainChunks[x, z];
        //     } else {
        //         return null;
        //     }
        // }

        // public CC_TerrainChunk getTerrainChunkFromLocation (Vector3 location) {
        //     int x = Mathf.FloorToInt (location.x / settingsController.TerrainTilesPerChunk);
        //     int z = Mathf.FloorToInt (location.z / settingsController.TerrainTilesPerChunk);
        //     return GetTerrainChunk (x, z);
        // }
    }

}