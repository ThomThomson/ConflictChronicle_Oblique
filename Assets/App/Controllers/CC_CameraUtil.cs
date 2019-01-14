using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_CameraUtil
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
}
