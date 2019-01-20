using System;
using ConflictChronicle.Controllers;
using UnityEngine;

class CC_PlayerInterestHover : MonoBehaviour {
    public float speed = 6.0F;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 100.0f;
    private CharacterController controller; 

    void Start() {
        controller = GetComponent<CharacterController>();
        controller.detectCollisions = false;
    }

    void Update()
    {
        moveDirection = CC_InputController.input.getInputCameraNormalized();
        moveDirection *= speed;
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
    }
}

