using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConflictChronicle {

    public class CameraController : MonoBehaviour {

        // P u b l i c  A P I
        public bool Rotating { get { return _rotating; } }
        public CC_CameraDirection CameraDirection { get { return _cameraDirection; } }
        public GameObject PlayerFocusPoint { get { return _playerFocusPoint; } set { _playerFocusPoint = value; } }

        public event EventHandler<CC_CameraDirection> OnCameraRotate;

        // S t a t e
        private bool _rotating;
        private GameObject _playerFocusPoint;
        private int _cameraDirectionIndex = 0;
        private CC_CameraDirection _cameraDirection;

        // C h i l d r e n  S e t u p 
        [SerializeField] private CameraPerspectiveEditor perspectiveEditor;
        public void Awake () {
            perspectiveEditor = GetComponent<CameraPerspectiveEditor> ();
        }

        // D e p e n d e n c i e s
        private SettingsController settingsController;
        public void InjectDependencies (SettingsController settingsController) {
            this.settingsController = settingsController;
            _cameraDirection = settingsController.CameraDefaultDirection;
        }

        public void Update () {
            if (Input.GetKeyDown (KeyCode.JoystickButton5) || Input.GetKeyDown (KeyCode.E)) {
                if (!_rotating) { StartCoroutine ("rotateCamera", 1); }
            }
            if (Input.GetKeyDown (KeyCode.JoystickButton4) || Input.GetKeyDown (KeyCode.Q)) {
                if (!_rotating) { StartCoroutine ("rotateCamera", -1); }
            }
        }

        private void LateUpdate () {
            if (!_playerFocusPoint || !settingsController) {
                return;
            }
            Vector3 newPosition = _playerFocusPoint.transform.position;
            transform.position = Vector3.Slerp (transform.position, newPosition, settingsController.CameraCatchupSpeed * Time.deltaTime);
        }

        public int spriteSort (Vector3 spriteLocation, bool grounded = true) {
            return 0;
            // RaycastHit groundLevel;
            // LayerMask terrainMask = LayerMask.GetMask ("CC_Terrain");
            // float ySort = 0;
            // if (!grounded) {
            //     if (Physics.Raycast (spriteLocation + (Vector3.up / 2), Vector3.down, out groundLevel, spriteLocation.y + 1, terrainMask)) {
            //         ySort = groundLevel.point.y;
            //     }
            // } else {
            //     ySort = spriteLocation.y;
            // }

            // return Mathf.RoundToInt (
            //     ((spriteLocation.x * _cameraDirection.SortDirection.x)) +
            //     (Mathf.RoundToInt (ySort) * settingsController.sortMultiplierHeight) +
            //     ((spriteLocation.z * _cameraDirection.SortDirection.z))
            // );
        }

        public IEnumerator rotateCamera (int rotateChange) {
            _rotating = true;
            int newCameraDirectionIndex = ((_cameraDirectionIndex + CC_CameraDirection.CameraDirectionsInOrder.Count) + rotateChange) % CC_CameraDirection.CameraDirectionsInOrder.Count;
            _cameraDirectionIndex = newCameraDirectionIndex;

            Quaternion targetRotation = CC_CameraDirection.CameraDirectionsInOrder[_cameraDirectionIndex].CameraRotation;
            LeanTween.rotate (this.gameObject, targetRotation.eulerAngles, settingsController.CameraRotationDuration).setEaseInOutExpo ();

            yield return new WaitForSeconds (settingsController.CameraRotationDuration / 2);
            _cameraDirection = CC_CameraDirection.CameraDirectionsInOrder[_cameraDirectionIndex];
            OnCameraRotate?.Invoke (this, _cameraDirection);

            Debug.Log (_cameraDirection.DirectionName);

            yield return new WaitForSeconds (settingsController.CameraRotationDuration / 4);
            _rotating = false;
        }

        public Vector3? screenToWorldCoordinates (Vector3 screencoordinates) {
            Ray ray = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor> ().ScreenPointToRay (screencoordinates);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit)) {
                return hit.point;
            } else {
                return null;
            }
        }

        public Vector3? screenToWorldCoordinatesOnGround (Vector3 screencoordinates) {
            Ray ray = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor> ().ScreenPointToRay (screencoordinates);
            RaycastHit[] hits;
            int layerMask = 1 << settingsController.TerrainLayerIndex;
            hits = Physics.RaycastAll (ray.origin, ray.direction, layerMask);
            if (hits.Length > 0) {
                return hits[0].point;
            }
            return null;
        }

        public float worldTopToGround (Vector3 worldCoordinates) {
            RaycastHit hit;
            Ray ray = new Ray (new Vector3 (worldCoordinates.x, settingsController.TerrainTopHeight, worldCoordinates.z), Vector3.down);
            int layerMask = 1 << settingsController.TerrainTopHeight;
            if (Physics.Linecast (ray.origin, new Vector3 (worldCoordinates.x, 0, worldCoordinates.z), out hit, layerMask)) {
                return hit.point.y;
            }
            return 0f;
        }
    }
}