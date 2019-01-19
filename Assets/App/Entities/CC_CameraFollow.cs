using UnityEngine;
using System.Collections;

namespace ConflictChronicle.Entities
{

    public class CC_CameraFollow : MonoBehaviour
    {
        public GameObject player;

        [Range(0, 10)]
        public float catchupSpeed;
        private float yOffset;
        private float zOffset;
        private Vector3 targetingPosition;

        private void LateUpdate()
        {
            if (!player) return;
            Vector3 newPosition = player.transform.position;
            transform.position = Vector3.Slerp(transform.position, newPosition, catchupSpeed * Time.deltaTime);

        }
    }
}
