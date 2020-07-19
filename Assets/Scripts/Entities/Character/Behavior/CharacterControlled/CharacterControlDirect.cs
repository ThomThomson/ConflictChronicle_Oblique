using System.Collections;
using UnityEngine;

namespace ConflictChronicle {

    public class CharacterControlDirect : MonoBehaviour, ICharacterControlled {
        public bool Grounded { get { return characterController.isGrounded; } }

        public float speed = 6.0f;
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;

        private CharacterController characterController;
        private CameraController cameraController;

        private CC_CompassHeading heading = CC_CompassHeading.SOUTH_EAST;
        private Vector2 inputDirection = Vector2.zero;
        private Vector3 moveDirection = Vector3.zero;
        private bool movingUnderOwnForce = false;

        void Start () {
            characterController = GetComponent<CharacterController> ();
        }

        public void InjectDependencies (CameraController cameraController) {
            this.cameraController = cameraController;
        }

        public float getTopSpeed () { return speed; }
        public float getVelocity () { return Vector2.Distance (Vector2.zero, new Vector2 (characterController.velocity.x, characterController.velocity.z)); }
        public Vector2 get2dMovement () { return new Vector2 (characterController.velocity.x, characterController.velocity.z); }
        public CC_CompassHeading getHeading () { return heading; }
        public bool isMovingUnderOwnForce () { return movingUnderOwnForce; }

        void Update () {
            inputDirection.x = Input.GetAxis ("Horizontal");
            inputDirection.y = Input.GetAxis ("Vertical");

            movingUnderOwnForce = characterController.isGrounded && inputDirection.magnitude > 0;
            if (movingUnderOwnForce) {
                heading = CC_CompassUtil.compassHeadingFromVector3 (moveDirection);
            }

            if (characterController.isGrounded) {
                // We are grounded, so recalculate
                // move direction directly from axes
                inputDirection = CC_InputDirection.transformInputDirection (inputDirection);
                moveDirection = cameraController.transform.TransformDirection (new Vector3 (inputDirection.x, 0.0f, inputDirection.y));
                moveDirection *= speed;

                if (Input.GetButton ("Jump")) {
                    moveDirection.y = jumpSpeed;
                }
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller
            characterController.Move (moveDirection * Time.deltaTime);
        }

    }
}