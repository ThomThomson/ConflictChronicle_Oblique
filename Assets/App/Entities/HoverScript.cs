using System;
using UnityEngine;

class HoverScript : MonoBehaviour {
    public float speed = 6.0F;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller; 

    void Start() {
        controller = GetComponent<CharacterController>();
        controller.detectCollisions = false;
    }

    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3.ClampMagnitude(moveDirection, 1f);
        moveDirection *= speed;
        controller.Move(moveDirection * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space)) {
            
        }
    }
}

