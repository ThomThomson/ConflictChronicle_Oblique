using System.Collections;
using UnityEngine;

namespace ConflictChronicle {

    public class ShowCollider : MonoBehaviour {
        public bool alwaysDrawCollider;

        void OnDrawGizmos () {
            if (alwaysDrawCollider) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube (transform.position, transform.lossyScale);
            }
        }
    }

}