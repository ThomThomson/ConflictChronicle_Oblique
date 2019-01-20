using System;
using System.Collections;
using System.Collections.Generic;
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
            Debug.DrawRay(screenRay.origin, screenRay.direction * Vector3.Distance(screenRay.origin, spriteLocation), Color.red, 0.1f);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(screenRay.origin, screenRay.direction, Vector3.Distance(screenRay.origin, spriteLocation));
            Nullable<int> nearestSortOrder = null;
            float nearestDistance = Mathf.Infinity;
            for (int i = 0; i < hits.Length; i++)
            {
                CC_IsoCollider isoCollider = hits[i].transform.gameObject.GetComponent<CC_IsoCollider>();
                if (isoCollider != null)
                {
                    float hitDistance = Vector3.Distance(spriteLocation, hits[i].transform.position);
                    if (hitDistance < nearestDistance)
                    {
                        nearestSortOrder = isoCollider.GovernedSprite.sortingOrder - 1;
                        nearestDistance = hitDistance;
                    }
                }
            }
            if (nearestSortOrder.HasValue)
            {
                return (int)nearestSortOrder;
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