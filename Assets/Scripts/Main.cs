using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

namespace ConflictChronicle {

    [RequireComponent (typeof (AssetController), typeof (SettingsController))]
    public class Main : MonoBehaviour {

        [SerializeField] private AssetController assetController;
        [SerializeField] private SettingsController settingsController;

        void Start () {
            Time.timeScale = 0;
            assetController = GetComponent<AssetController> ();
            settingsController = GetComponent<SettingsController> ();
            settingsController.InitializeEnvironment ();

            GameObject camera = Instantiate (assetController.Camera);
            CameraController cameraController = camera.GetComponent<CameraController> ();
            cameraController.InjectDependencies (settingsController);

            GameObject playerFocusPoint = Instantiate (assetController.playerFocusPoint);
            Character tempPlayerCharacter = playerFocusPoint.GetComponent<Character> ();
            tempPlayerCharacter.InjectDependencies (settingsController, cameraController, assetController);
            cameraController.PlayerFocusPoint = playerFocusPoint;

            if (settingsController.loadMap) {
                WorldController worldController = this.gameObject.AddComponent<WorldController> ();
                worldController.injectDependencies (settingsController, assetController, cameraController);
                worldController.loadWorld (settingsController.environment.SAVE_FILE_LOCATION, playerFocusPoint.transform);
            }
            Time.timeScale = 1;
        }
    }

    public struct zIntVector2 {
        public zIntVector2 (int x, int z) {
            this.x = x;
            this.z = z;
        }

        public Vector2 toVector2 () {
            return new Vector2 (this.x, this.z);
        }

        public int x;
        public int z;
    }
}