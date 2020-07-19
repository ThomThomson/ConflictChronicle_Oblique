using UnityEngine;

namespace ConflictChronicle {
    public interface ICharacterControlled {
        float getVelocity ();
        float getTopSpeed ();
        Vector2 get2dMovement ();
        bool isMovingUnderOwnForce ();
        CC_CompassHeading getHeading ();
    }
}