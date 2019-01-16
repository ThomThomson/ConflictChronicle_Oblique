using UnityEngine;

public class CC_InputController : MonoBehaviour
{
    public static CC_InputController input;
    private Transform cameratransform;
    // private Vector3 cameraNormalizedInput = Vector3.zero;
    private Vector2 inputVector;


    private void Start()
    {
        cameratransform = Camera.main.transform;
        input = this;
    }

    public Vector3 getInputCameraNormalized() {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        inputVector = Vector3.ClampMagnitude(inputVector, 1);
        Vector3 camForward = cameratransform.forward;
        Vector3 camRight = cameratransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
        return (camForward * inputVector);
        // cameraNormalizedInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // Debug.Log("camera: " + camera.transform.rotation + " Input: " + cameraNormalizedInput);
    }
}