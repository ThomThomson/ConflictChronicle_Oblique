using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConflictChronicle {

    public class CC_TerrainChunk_Renderer : MonoBehaviour {
        public GameObject sideFront;
        public GameObject sideRight;
        public GameObject sideLeft;
        public GameObject sideBack;

        public SortingGroup sortGroup;
        public Transform sortLocation;

        private CC_TerrainChunk chunk;
        private CC_CameraDirection lastRenderedDirection = null;
        private Dictionary<CC_Compass, CC_TerrainChunk> neighbors;
        private Dictionary<CC_BlockSide, GameObject> sideObjects;

        private CameraController cameraController;

        private GameObject spriteParent;

        public void Start () {
            sideObjects = new Dictionary<CC_BlockSide, GameObject> ();
            if (sideFront) sideObjects.Add (CC_BlockSide.FRONT, sideFront);
            if (sideRight) sideObjects.Add (CC_BlockSide.RIGHT, sideRight);
            if (sideLeft) sideObjects.Add (CC_BlockSide.LEFT, sideLeft);
            if (sideBack) sideObjects.Add (CC_BlockSide.BACK, sideBack);
        }

        public void setup (CameraController cameraController, CC_TerrainChunk chunk, Dictionary<CC_Compass, CC_TerrainChunk> neighbors, GameObject spriteParent) {
            this.cameraController = cameraController;
            this.chunk = chunk;
            this.neighbors = neighbors;
            this.spriteParent = spriteParent;
        }

        public void enterView () {
            if (lastRenderedDirection != cameraController.CameraDirection) {
                lastRenderedDirection = cameraController.CameraDirection;
                sort ();
                // Vector3 originalPosition = spriteParent.transform.position;
                // spriteParent.transform.position = new Vector3 (originalPosition.x, originalPosition.y - 10, originalPosition.z);
                rotate (lastRenderedDirection, spriteParent);
                // LeanTween.moveY (spriteParent, originalPosition.y, 1);
                recalculateEdges ();
            }
        }

        public void sort () {
            if (this.sortGroup != null && sortLocation != null) {
                this.sortGroup.sortingOrder = cameraController.spriteSort (sortLocation.position);
            }
        }

        public void rotate (CC_CameraDirection newCameraDirection, GameObject spriteParent) {
            spriteParent.transform.rotation = newCameraDirection.CameraRotation;
            sort ();
        }

        public void recalculateEdges () {
            foreach (KeyValuePair<CC_Compass, CC_TerrainChunk> neighbor in neighbors) {
                CC_BlockSide key = cameraController.CameraDirection.compassToSideMap[neighbor.Key];
                if (sideObjects.ContainsKey (key)) {
                    if (!neighbor.Value || neighbor.Value.yPosition < chunk.yPosition) {
                        sideObjects[key].SetActive (true);
                    } else {
                        sideObjects[key].SetActive (false);
                    }

                }
            }
        }
    }
}