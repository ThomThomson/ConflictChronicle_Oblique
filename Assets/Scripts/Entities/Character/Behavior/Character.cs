using UnityEngine;

namespace ConflictChronicle {

    public class Character : MonoBehaviour {

        private SettingsController settingsController;
        private CameraController cameraController;
        private AssetController assetController;

        private CharacterControlDirect directControl;
        private CharacterControlAuto autoControl;
        private CharacterDisplay display;

        public void Awake () {
            directControl = GetComponent<CharacterControlDirect> ();
            autoControl = GetComponent<CharacterControlAuto> ();
            display = GetComponent<CharacterDisplay> ();
        }

        public void InjectDependencies (SettingsController settingsController, CameraController cameraController, AssetController assetController) {
            this.settingsController = settingsController;
            this.cameraController = cameraController;

            this.directControl.InjectDependencies (cameraController);
            this.display.InjectDependencies (settingsController, assetController, cameraController);
        }

    }
}