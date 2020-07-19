using UnityEngine;

namespace ConflictChronicle {

    [RequireComponent (typeof (SettingsController))]
    public class TestCamera : MonoBehaviour {
        public CameraController cameraController;

        public void Start () {
            SettingsController settings = GetComponent<SettingsController> ();
            cameraController.InjectDependencies (settings);
        }
    }
}