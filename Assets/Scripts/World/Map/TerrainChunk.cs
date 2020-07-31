using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {

    public class TerrainChunk : MonoBehaviour, IWorldObject {
        [HideInInspector] public int xGridPosition;
        [HideInInspector] public int zGridPosition;
        [HideInInspector] public bool isVisible;

        List<IWorldObject> children;

        public void addChild (GameObject child) {
            child.transform.parent = transform;
            IWorldObject worldObject = child.GetComponent<IWorldObject> ();
            if (worldObject != null) {
                if (children == null) {
                    children = new List<IWorldObject> ();
                }
                if (isVisible) {
                    worldObject.EnterView ();
                } else {
                    worldObject.LeaveView ();
                }
                children.Add (worldObject);
            }
        }

        public void EnterView () {
            foreach (IWorldObject child in children) {
                child.EnterView ();
            }
        }

        public void LeaveView () {
            foreach (IWorldObject child in children) {
                child.LeaveView ();
            }
        }
    }
}