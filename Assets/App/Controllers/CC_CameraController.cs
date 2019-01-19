using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ConflictChronicle.Controllers
{

    public class CC_CameraController
    {

        public static Vector3? screenToWorldCoordinates(Vector3 screencoordinates)
        {
            Ray ray = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor>().ScreenPointToRay(screencoordinates);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point;
            }
            else
            {
                return null;
            }
        }

        public static Vector3? screenToWorldCoordinatesOnGround(Vector3 screencoordinates)
        {
            Ray ray = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor>().ScreenPointToRay(screencoordinates);
            Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red, 1f);
            // Bit shift the index of the TERRAIN layer (9) to get a bit mask
            RaycastHit[] hits;
            int layerMask = 1 << CC_SettingsController.gameSettings.TERRAIN_LAYER_INDEX;
            hits = Physics.RaycastAll(ray.origin, ray.direction, layerMask);
            if (hits.Length > 0)
            {
                return hits[0].point;
            }
            return null;
        }

        public static Vector3? worldTopToGround(Vector3 screencoordinates)
        {
            Ray ray = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor>().ScreenPointToRay(screencoordinates);
            // Bit shift the index of the TERRAIN layer (9) to get a bit mask
            RaycastHit hit;
            int layerMask = 1 << CC_SettingsController.gameSettings.TERRAIN_LAYER_INDEX;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, layerMask))
            {
                return hit.point;
            }
            return null;
        }
    }
}