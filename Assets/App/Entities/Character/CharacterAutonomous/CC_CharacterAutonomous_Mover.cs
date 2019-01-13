using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class CC_CharacterAutonomous_Mover : MonoBehaviour
{
    GameObject playerInterestPoint;
    IAstarAI ai;
    int radius = 30;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        playerInterestPoint = GameObject.FindGameObjectWithTag("CC_PlayerInterestPoint");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ray = new Ray(playerInterestPoint.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                ai.destination = hit.transform.position;
            }
        }
    }
}
