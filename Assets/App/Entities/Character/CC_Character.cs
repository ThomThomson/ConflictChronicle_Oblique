using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class CC_Character : MonoBehaviour
{
    private CharacterController controller;

    private float speed;
    public float Speed { get { return speed; } }


    void Start()
    {
        speed = Random.Range(5f, 10f);

    }

    void GetNewDestination()
    {
        // Vector3 newDestination = Random.insideUnitSphere * 100;
        // int mapMax = CC_MapController.Instance.MapModel.mapTerrain.Count * CC_SettingsController.gameSettings.TILES_PER_CHUNK;
        // newDestination.x = Mathf.Clamp(newDestination.x + this.transform.position.x, 0, mapMax);
        // newDestination.y = 0f;
        // newDestination.z = Mathf.Clamp(newDestination.z + this.transform.position.z, 0, mapMax);
        // pathAI.destination = newDestination;
    }
}



