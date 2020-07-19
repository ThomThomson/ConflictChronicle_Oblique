using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ConflictChronicle {

    [Serializable]
    public class MapModel {
        public int worldSizeChunksX;
        public int worldSizeChunksZ;
        public CC_TerrainChunk_Model[] terrain;
    }

    public class MapController : MonoBehaviour {
        public CC_TerrainChunk[, ] terrainChunks;
        public int xSizeInTiles;
        public int zSizeInTiles;
        public int xSizeInChunks;
        public int zSizeInChunks;

        private Transform playerFocusPoint;
        private GameObject worldFolder;
        private List<CC_TerrainChunk> visibleChunks;
        private bool worldActive = false;

        private SettingsController settingsController;
        private AssetController assetController;
        private CameraController cameraController;

        private Queue<CC_TerrainChunk> chunkViewChangeQueue = new Queue<CC_TerrainChunk> ();
        private Queue<CC_TerrainChunk> edgeRecalculationQueue = new Queue<CC_TerrainChunk> ();

        public void injectDependencies (SettingsController settingsController, AssetController assetController, CameraController cameraController) {
            this.settingsController = settingsController;
            this.cameraController = cameraController;
            this.assetController = assetController;
            this.cameraController.OnCameraRotate += onCameraRotate;
        }

        public void loadWorld (MapModel worldModel) {
            worldFolder = new GameObject ();
            worldFolder.name = "CC_WorldFolder";
            worldFolder.tag = "TerrainChunkFolder";

            xSizeInChunks = worldModel.worldSizeChunksX;
            zSizeInChunks = worldModel.worldSizeChunksZ;
            xSizeInTiles = worldModel.worldSizeChunksX * settingsController.TerrainTilesPerChunk;
            zSizeInTiles = worldModel.worldSizeChunksZ * settingsController.TerrainTilesPerChunk;
            loadWorldFromModel (worldModel);
            generateBorders ();
        }

        public void startPlayer (Transform playerFocusPoint) {
            this.playerFocusPoint = playerFocusPoint;
            this.visibleChunks = new List<CC_TerrainChunk> ();
            worldActive = true;
            StartCoroutine ("manageVisibilityCoroutine");
            // LeanTween.alpha (assetController.blackScreenOverlay.rectTransform, 0, 1f);
        }

        public void cleanVisibleChunksAndQueue () {
            chunkViewChangeQueue.Clear ();
            foreach (CC_TerrainChunk chunk in visibleChunks) {
                chunk.isVisible = false;
                chunk.LeaveView (this);
            }
        }

        public void onCameraRotate (object sender, CC_CameraDirection newCameraDirection) {
            foreach (CC_TerrainChunk chunkToRotate in visibleChunks) {
                chunkToRotate.onCameraRotate (newCameraDirection);
            }
            recalculateVisibleEdges ();
        }

        public void recalculateVisibleEdges () {
            CC_TerrainChunk playerLocationChunk = getTerrainChunkFromLocation (this.playerFocusPoint.transform.position);
            if (playerLocationChunk == null) {
                return;
            }
            edgeRecalculationQueue.Clear ();
            edgeRecalculationQueue.Enqueue (playerLocationChunk);
            zIntVector2 currentLocation = new zIntVector2 (playerLocationChunk.xGridPosition, playerLocationChunk.zGridPosition);
            int magnitude = 1;
            int currentDirectionIndex = 0;
            bool finished = false;
            while (!finished) {
                for (int cycle = 0; cycle < 2; cycle++) {
                    if (finished) { break; }
                    for (int i = 0; i < magnitude; i++) {
                        if (finished) { break; }
                        currentLocation.x += CC_Compass.spiralDirections[currentDirectionIndex].direction.x;
                        currentLocation.z += CC_Compass.spiralDirections[currentDirectionIndex].direction.z;
                        CC_TerrainChunk chunkToAdd = GetTerrainChunk (currentLocation.x, currentLocation.z);
                        if (chunkToAdd != null) {
                            if (!chunkToAdd.isVisible) {
                                finished = true;
                            } else {
                                edgeRecalculationQueue.Enqueue (chunkToAdd);
                            }
                        }
                    }
                    currentDirectionIndex = ((currentDirectionIndex + CC_Compass.spiralDirections.Length) + 1) % CC_Compass.spiralDirections.Length;
                }
                magnitude++;
            }
        }

        private IEnumerator manageVisibilityCoroutine () {
            while (worldActive) {
                manageVisibility ();
                yield return null;
            }
        }

        public void Update () {
            if (worldActive) {
                int edgeRecalculations = 0;
                while (edgeRecalculations < settingsController.MapChunkChangesPerFrame && edgeRecalculationQueue.Count != 0) {
                    CC_TerrainChunk edgeRecalculationChunk = edgeRecalculationQueue.Dequeue ();
                    if (edgeRecalculationChunk.isVisible) {
                        edgeRecalculationChunk.onRecalculateEdges ();
                    }
                    edgeRecalculations++;
                }
                int changesThisIteration = 0;
                while (changesThisIteration < settingsController.MapChunkChangesPerFrame && chunkViewChangeQueue.Count != 0) {
                    CC_TerrainChunk chunkToChange = chunkViewChangeQueue.Dequeue ();
                    if (chunkToChange.isVisible) {
                        chunkToChange.EnterView (this);
                        visibleChunks.Add (chunkToChange);
                    } else {
                        chunkToChange.LeaveView (this);
                        visibleChunks.Remove (chunkToChange);
                    }
                    changesThisIteration++;
                }
            }
        }

        private void loadWorldFromModel (MapModel model) {
            terrainChunks = new CC_TerrainChunk[model.worldSizeChunksX, model.worldSizeChunksZ];
            for (int x = 0; x < model.worldSizeChunksX; x++) {
                for (int z = 0; z < model.worldSizeChunksZ; z++) {

                    CC_TerrainChunk_Model currentModel = model.terrain[x * model.worldSizeChunksX + z];
                    if (currentModel == null || currentModel.assetId == null || currentModel.assetId.Equals ("")) {
                        throw new ArgumentException ($"invalid chunk model at: {x}, {z}");
                    }

                    GameObject chunkPrefab;
                    if (!assetController.chunkTypes.TryGetValue (currentModel.assetId, out chunkPrefab)) {
                        throw new ArgumentException ($"chunk model at: {x}, {z} has invalid assetId: {currentModel.assetId }. Possible values include: {assetController.chunkTypes.Keys}");
                    }
                    GameObject currentChunkGameObject = Instantiate (chunkPrefab);
                    currentChunkGameObject.name = "Terrain Chunk at: " + x + " - " + z;
                    currentChunkGameObject.transform.parent = worldFolder.transform;

                    CC_TerrainChunk currentChunk = currentChunkGameObject.GetComponent<CC_TerrainChunk> ();
                    currentChunk.InjectDependencies (this, cameraController, settingsController);
                    if (currentChunk == null) {
                        throw new ArgumentException ($"chunk model at: {x}, {z} does not have a CC_TerrainChunk script");
                    }
                    currentChunk.deSerialize (currentModel);
                    terrainChunks[x, z] = currentChunk;
                }
            }
            for (int x = 0; x < model.worldSizeChunksX; x++) {
                for (int z = 0; z < model.worldSizeChunksZ; z++) {
                    terrainChunks[x, z].setupNeighbors ();
                }
            }
        }

        private void generateBorders () {
            GameObject borderWalls = new GameObject ();
            borderWalls.name = "CC_Border_Walls";

            Vector3[] startingPositions = {
                new Vector3 (xSizeInTiles / 2, settingsController.TerrainTopHeight / 2, 0), // north side, EAST Facing
                new Vector3 (xSizeInTiles, settingsController.TerrainTopHeight / 2, zSizeInTiles / 2), // NORTH Facing
                new Vector3 (xSizeInTiles / 2, settingsController.TerrainTopHeight / 2, zSizeInTiles), // WEST Facing
                new Vector3 (0, settingsController.TerrainTopHeight / 2, zSizeInTiles / 2) // SOUTH Facing
            };

            for (int i = 0; i < startingPositions.Length; i++) {
                GameObject currentWall = new GameObject ();
                currentWall.name = $"CC_Wall_{CC_Compass.spiralDirections[i].directionName}";
                currentWall.transform.position = startingPositions[i];
                currentWall.AddComponent<BoxCollider> ();
                Vector3 scale = new Vector3 (
                    (xSizeInTiles * Mathf.Abs (CC_Compass.spiralDirections[i].direction.x)) + 1,
                    settingsController.TerrainTopHeight,
                    (zSizeInTiles * Mathf.Abs (CC_Compass.spiralDirections[i].direction.z)) + 1
                );
                currentWall.transform.localScale = scale;
                currentWall.transform.parent = borderWalls.transform;
            }

            GameObject ceiling = new GameObject ();
            ceiling.name = "CC_CEILING";
            ceiling.transform.position = new Vector3 (
                xSizeInTiles / 2,
                settingsController.TerrainTopHeight - 0.5f,
                zSizeInTiles / 2
            );
            ceiling.AddComponent<BoxCollider> ();
            Vector3 ceilingScale = new Vector3 (xSizeInTiles, 1, zSizeInTiles);
            ceiling.transform.localScale = ceilingScale;
            ceiling.transform.parent = borderWalls.transform;

            GameObject gameFloor = Instantiate (assetController.worldFloor);
            gameFloor.name = "CC_FLOOR";
            gameFloor.transform.position = new Vector3 (xSizeInTiles / 2, -0.5f, zSizeInTiles / 2);
            Vector3 floorScale = new Vector3 (xSizeInTiles, 1, zSizeInTiles);
            gameFloor.transform.localScale = floorScale;
            gameFloor.transform.parent = borderWalls.transform;
            Vector2 textureScale = new Vector2 (xSizeInTiles / 4, zSizeInTiles / 4);
            gameFloor.GetComponent<Renderer> ().material.mainTextureScale = textureScale;
        }

        private void manageVisibility () {
            CC_TerrainChunk playerLocationChunk = getTerrainChunkFromLocation (this.playerFocusPoint.transform.position);
            if (playerLocationChunk == null) { return; }
            if (!playerLocationChunk.isVisible) {
                playerLocationChunk.isVisible = true;
                chunkViewChangeQueue.Enqueue (playerLocationChunk);
            }

            zIntVector2 maxViewNorthWest = new zIntVector2 (
                Mathf.Clamp (playerLocationChunk.xGridPosition - settingsController.MapViewDistance, 0, xSizeInChunks - 1),
                Mathf.Clamp (playerLocationChunk.zGridPosition - settingsController.MapViewDistance, 0, zSizeInChunks - 1)
            );

            zIntVector2 maxViewSouthEast = new zIntVector2 (
                Mathf.Clamp (playerLocationChunk.xGridPosition + settingsController.MapViewDistance, 0, xSizeInChunks - 1),
                Mathf.Clamp (playerLocationChunk.zGridPosition + settingsController.MapViewDistance, 0, zSizeInChunks - 1)
            );

            zIntVector2 currentLocation = new zIntVector2 (playerLocationChunk.xGridPosition, playerLocationChunk.zGridPosition);

            int magnitude = 1;
            int currentDirectionIndex = 0;
            bool finished = false;
            while (!finished) {
                for (int cycle = 0; cycle < 2; cycle++) {
                    if (finished) { break; }
                    for (int i = 0; i < magnitude; i++) {
                        if (finished) { break; }
                        currentLocation.x += CC_Compass.spiralDirections[currentDirectionIndex].direction.x;
                        currentLocation.z += CC_Compass.spiralDirections[currentDirectionIndex].direction.z;
                        CC_TerrainChunk chunkToAdd = GetTerrainChunk (currentLocation.x, currentLocation.z);
                        if (chunkToAdd != null && !chunkToAdd.isVisible) {
                            if (chunkIsOutOfViewBounds (chunkToAdd, maxViewNorthWest, maxViewSouthEast)) {
                                finished = true;
                            } else {
                                if (!chunkToAdd.isVisible) {
                                    chunkToAdd.isVisible = true;
                                    chunkViewChangeQueue.Enqueue (chunkToAdd);
                                }
                            }
                        }
                    }
                    currentDirectionIndex = ((currentDirectionIndex + CC_Compass.spiralDirections.Length) + 1) % CC_Compass.spiralDirections.Length;
                }
                magnitude++;
            }

            if (visibleChunks.Count != 0) {
                for (int i = visibleChunks.Count - 1; i >= 0; i--) {
                    CC_TerrainChunk visibleChunk = visibleChunks[i];
                    if (chunkIsOutOfViewBounds (visibleChunk, maxViewNorthWest, maxViewSouthEast)) {
                        if (visibleChunk.isVisible) {
                            visibleChunk.isVisible = false;
                            chunkViewChangeQueue.Enqueue (visibleChunk);
                        }
                    }
                }
            }
        }

        // utilities
        public bool chunkIsOutOfViewBounds (CC_TerrainChunk chunk, zIntVector2 maxViewNorthWest, zIntVector2 maxViewSouthEast) {
            if (chunk.zGridPosition > maxViewSouthEast.z ||
                chunk.zGridPosition < maxViewNorthWest.z ||
                chunk.xGridPosition > maxViewSouthEast.x ||
                chunk.xGridPosition < maxViewNorthWest.x
            ) {
                return true;
            }
            return false;
        }

        public CC_TerrainChunk GetTerrainChunk (int x, int z) {
            if (x >= 0 && x < xSizeInChunks && z >= 0 && z < zSizeInChunks) {
                return this.terrainChunks[x, z];
            } else {
                return null;
            }
        }

        public CC_TerrainChunk getTerrainChunkFromLocation (Vector3 location) {
            int x = Mathf.FloorToInt (location.x / settingsController.TerrainTilesPerChunk);
            int z = Mathf.FloorToInt (location.z / settingsController.TerrainTilesPerChunk);
            return GetTerrainChunk (x, z);
        }
    }

}