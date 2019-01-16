﻿using System;
using UnityEngine;

class CC_PlayerInterestHover : MonoBehaviour {
    public float speed = 6.0F;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller; 

    void Start() {
        controller = GetComponent<CharacterController>();
        controller.detectCollisions = false;
    }

    void Update()
    {
        moveDirection = CC_InputController.input.getInputCameraNormalized();
        //Vector3.ClampMagnitude(moveDirection, 1f);
        moveDirection *= speed;
        controller.Move(moveDirection * Time.deltaTime);
    }
}
