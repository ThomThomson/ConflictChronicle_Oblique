using UnityEngine;

namespace ConflictChronicle {

    public class SortEveryFrame : MonoBehaviour {

        private CameraController cameraController;
        public SpriteRenderer rendererToSet;

        public void Start () {
            cameraController = GameObject.FindGameObjectsWithTag ("CC_Camera") [0].GetComponent<CameraController> ();
        }

        public void Update () {
            if (rendererToSet) {
                rendererToSet.sortingOrder = cameraController.spriteSort (rendererToSet.transform.position);
            }
        }

    }
}