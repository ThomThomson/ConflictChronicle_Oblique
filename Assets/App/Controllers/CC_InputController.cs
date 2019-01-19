using UnityEngine;

namespace ConflictChronicle.Controllers
{

    public class CC_InputController : MonoBehaviour
    {
        public static CC_InputController input;
        private Transform cameratransform;
        // private Vector3 cameraNormalizedInput = Vector3.zero;


        private void Start()
        {
            cameratransform = Camera.main.transform;
            input = this;
        }

        public Vector3 getInputCameraNormalized()
        {
            return cameratransform.TransformDirection(Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), 1f));
        }
    }
}