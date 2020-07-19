using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {

    [System.Serializable]
    public class CC_TerrainChunk_Model {
        public int xGridPosition;
        public int zGridPosition;
        public int yPosition;
        public string assetId;
        public List<System.Object> children;
    }

    [RequireComponent (typeof (CC_TerrainChunk_Renderer))]
    public class CC_TerrainChunk : MonoBehaviour, CC_IWorldObject {

        [HideInInspector] public int xGridPosition;
        [HideInInspector] public int zGridPosition;
        [HideInInspector] public int yPosition;
        [HideInInspector] public string assetId;
        [HideInInspector] public bool isVisible;

        private List<CC_IWorldObject> children;
        private Dictionary<CC_Compass, CC_TerrainChunk> neighbors;
        private CC_TerrainChunk_Renderer chunkRenderer;

        [SerializeField]
        private GameObject spriteParent;

        private bool pristine = true;
        private CC_TerrainChunk_Model serializableModel;
        private CC_CameraDirection lastCameraDirection;

        private CameraController cameraController;
        private SettingsController settingsController;
        private MapController mapController;

        public void InjectDependencies (MapController mapController, CameraController cameraController, SettingsController settingsController) {
            this.mapController = mapController;
            this.cameraController = cameraController;
            this.settingsController = settingsController;
        }

        public CC_IWorldObject deSerialize (System.Object serialized) {
            serializableModel = (CC_TerrainChunk_Model) serialized;
            xGridPosition = serializableModel.xGridPosition;
            yPosition = serializableModel.yPosition;
            zGridPosition = serializableModel.zGridPosition;
            transform.position = new Vector3 (
                xGridPosition * settingsController.TerrainTilesPerChunk,
                yPosition,
                zGridPosition * settingsController.TerrainTilesPerChunk);
            chunkRenderer = GetComponent<CC_TerrainChunk_Renderer> ();
            spriteParent.SetActive (false);
            return this;
        }

        public void addChild (Vector3 position) {
            throw new System.NotImplementedException ();
        }

        public void setupNeighbors () {
            neighbors = new Dictionary<CC_Compass, CC_TerrainChunk> ();
            neighbors.Add (CC_Compass.NORTH, mapController.GetTerrainChunk (xGridPosition, zGridPosition + 1));
            neighbors.Add (CC_Compass.EAST, mapController.GetTerrainChunk (xGridPosition + 1, zGridPosition));
            neighbors.Add (CC_Compass.SOUTH, mapController.GetTerrainChunk (xGridPosition, zGridPosition - 1));
            neighbors.Add (CC_Compass.WEST, mapController.GetTerrainChunk (xGridPosition - 1, zGridPosition));
            chunkRenderer.setup (cameraController, this, neighbors, spriteParent);
        }

        public void EnterView (MapController worldController) {
            spriteParent.SetActive (true);
            if (lastCameraDirection == null || cameraController.CameraDirection != lastCameraDirection) {
                onCameraRotate (cameraController.CameraDirection);
            }
            chunkRenderer.enterView ();
        }

        public void LeaveView (MapController worldController) {
            spriteParent.SetActive (false);
        }

        public void deactivate () {

        }

        public List<CC_IWorldObject> getChildren () {
            throw new System.NotImplementedException ();
        }

        public CC_IWorldObject getParent () {
            throw new System.NotImplementedException ();
        }

        public System.Object getSerializableObject () {
            if (serializableModel == null) {
                serializableModel = new CC_TerrainChunk_Model ();
            }
            if (!pristine) {
                this.serializableModel.xGridPosition = this.xGridPosition;
                this.serializableModel.zGridPosition = this.zGridPosition;
                this.serializableModel.assetId = this.assetId;
                this.serializableModel.children = new List<System.Object> ();
                foreach (CC_IWorldObject child in this.children) {
                    this.serializableModel.children.Add (child.getSerializableObject ());
                }
                pristine = true;
            }
            return this.serializableModel;
        }

        public void onCameraRotate (CC_CameraDirection newCameraDirection) {
            this.chunkRenderer.rotate (newCameraDirection, spriteParent);
            lastCameraDirection = newCameraDirection;
        }

        public void onRecalculateEdges () {
            this.chunkRenderer.recalculateEdges ();
        }

    }
}