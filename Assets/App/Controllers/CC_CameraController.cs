using System;
using System.Collections;
using System.Collections.Generic;
using ConflictChronicle.Views;
using UnityEngine;


namespace ConflictChronicle.Controllers
{
    public class CC_CameraController : MonoBehaviour
    {

        public static int spriteSort(Vector3 spriteLocation)
        {
            int order = (int)(((int)spriteLocation.z * -10) + ((int)spriteLocation.x));
            return order;
        }

        public static int spriteSortViaRay(Vector3 spriteLocation)
        {
            Vector3 screenPoint = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor>().WorldToScreenPoint(spriteLocation);
            Ray screenRay = Camera.main.gameObject.GetComponent<CameraPerspectiveEditor>().ScreenPointToRay(screenPoint);
            Debug.DrawLine(screenRay.origin, spriteLocation, Color.red, 0.1f);
            RaycastHit hit;
            if (Physics.Linecast(spriteLocation, screenRay.origin, out hit))
            {
                CC_IsoCollider isoCollider = hit.transform.gameObject.GetComponent<CC_IsoCollider>();
                if (isoCollider != null)
                {
                    return isoCollider.GovernedSprite.sortingOrder - 1;
                }
            }
            return spriteSort(spriteLocation);
        }

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

        public static float worldTopToGround(Vector3 worldCoordinates)
        {
            RaycastHit hit;
            Ray ray = new Ray(new Vector3(worldCoordinates.x, CC_SettingsController.gameSettings.TERRAIN_TOP_HEIGHT, worldCoordinates.z), Vector3.down);
            int layerMask = 1 << CC_SettingsController.gameSettings.TERRAIN_LAYER_INDEX;
            if (Physics.Linecast(ray.origin, new Vector3(worldCoordinates.x, 0, worldCoordinates.z), out hit, layerMask))
            {
                return hit.point.y;
            }
            return 0f;
        }
    }
}