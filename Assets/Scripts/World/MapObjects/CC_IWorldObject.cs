using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {
    public interface IWorldObject {
        void EnterView ();
        void LeaveView ();

        void addChild (GameObject child);
    }
}