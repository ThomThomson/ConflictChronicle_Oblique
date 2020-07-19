using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {
    public class CC_Compass {
        public String directionName;
        public zIntVector2 direction;

        private CC_Compass (zIntVector2 direction, String directionName) {
            this.direction = direction;
            this.directionName = directionName;
        }

        public static CC_Compass NORTH = new CC_Compass (new zIntVector2 (0, 1), "NORTH");
        public static CC_Compass EAST = new CC_Compass (new zIntVector2 (1, 0), "EAST");
        public static CC_Compass SOUTH = new CC_Compass (new zIntVector2 (0, -1), "SOUTH");
        public static CC_Compass WEST = new CC_Compass (new zIntVector2 (-1, 0), "WEST");

        public static CC_Compass[] spiralDirections = { EAST, SOUTH, WEST, NORTH };
    }

    public enum CC_CompassHeading {
        NORTH_EAST,
        SOUTH_EAST,
        SOUTH_WEST,
        NORTH_WEST,
    }

    public enum CC_BlockSide {
        FRONT,
        BACK,
        LEFT,
        RIGHT
    }

    public class CC_CompassUtil {
        public static CC_CompassHeading[] headingsInOrder = {
            CC_CompassHeading.SOUTH_EAST,
            CC_CompassHeading.NORTH_EAST,
            CC_CompassHeading.NORTH_WEST,
            CC_CompassHeading.SOUTH_WEST
        };

        public static CC_CompassHeading compassHeadingFromVector3 (Vector3 heading) {
            if (heading.magnitude == 0) { return CC_CompassHeading.SOUTH_EAST; }

            if (heading.x >= 0 && heading.z >= 0) {
                return CC_CompassHeading.NORTH_EAST;
            } else if (heading.x < 0 && heading.z > 0) {
                return CC_CompassHeading.NORTH_WEST;
            } else if (heading.x <= 0 && heading.z <= 0) {
                return CC_CompassHeading.SOUTH_WEST;
            } else {
                return CC_CompassHeading.SOUTH_EAST;
            }
        }
    }
}