using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {
    public interface CC_IWorldObject {
        void EnterView (MapController worldController);
        void LeaveView (MapController worldController);

        void onCameraRotate (CC_CameraDirection newCameraDirection);

        void addChild (Vector3 position);
        List<CC_IWorldObject> getChildren ();
        CC_IWorldObject getParent ();

        System.Object getSerializableObject ();
        CC_IWorldObject deSerialize (System.Object serialized);
    }
}