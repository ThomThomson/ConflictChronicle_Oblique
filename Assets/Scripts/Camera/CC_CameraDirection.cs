using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {

    public enum CC_CameraDirectionName {
        NORTH_FACING,
        EAST_FACING,
        SOUTH_FACING,
        WEST_FACING
    }

    public enum CC_SortDirection {
        Z,
        X
    }

    public class CC_CameraDirection {
        public static Dictionary<CC_CameraDirectionName, CC_CameraDirection> cameraDirections = populateCameraDirections ();
        public static CC_CameraDirection initialCameraDirection = cameraDirections[CC_CameraDirectionName.NORTH_FACING];
        public static List<CC_CameraDirection> CameraDirectionsInOrder = getCameraDirectionsInOrder ();

        public Quaternion CameraRotation;
        public zIntVector2 SortDirection;
        public CC_CameraDirectionName DirectionName;
        public Dictionary<CC_Compass, CC_BlockSide> compassToSideMap;

        private CC_CameraDirection (CC_CameraDirectionName directionName, Quaternion cameraRotation, zIntVector2 sortDirection, Dictionary<CC_Compass, CC_BlockSide> compassToSideMap) {
            this.CameraRotation = cameraRotation;
            this.SortDirection = sortDirection;
            this.DirectionName = directionName;
            this.compassToSideMap = compassToSideMap;
        }

        private static Dictionary<CC_CameraDirectionName, CC_CameraDirection> populateCameraDirections () {
            Dictionary<CC_CameraDirectionName, CC_CameraDirection> cameraDirections = new Dictionary<CC_CameraDirectionName, CC_CameraDirection> ();

            // NORTH_FACING
            Dictionary<CC_Compass, CC_BlockSide> northCompassToSideMap = new Dictionary<CC_Compass, CC_BlockSide> ();
            northCompassToSideMap.Add (CC_Compass.NORTH, CC_BlockSide.BACK);
            northCompassToSideMap.Add (CC_Compass.EAST, CC_BlockSide.RIGHT);
            northCompassToSideMap.Add (CC_Compass.SOUTH, CC_BlockSide.FRONT);
            northCompassToSideMap.Add (CC_Compass.WEST, CC_BlockSide.LEFT);
            cameraDirections.Add (CC_CameraDirectionName.NORTH_FACING, new CC_CameraDirection (
                CC_CameraDirectionName.NORTH_FACING,
                new Quaternion (0, 0, 0, 1),
                new zIntVector2 (1, -1),
                northCompassToSideMap
            ));

            // EAST_FACING
            Dictionary<CC_Compass, CC_BlockSide> eastCompassToSideMap = new Dictionary<CC_Compass, CC_BlockSide> ();
            eastCompassToSideMap.Add (CC_Compass.NORTH, CC_BlockSide.LEFT);
            eastCompassToSideMap.Add (CC_Compass.EAST, CC_BlockSide.BACK);
            eastCompassToSideMap.Add (CC_Compass.SOUTH, CC_BlockSide.RIGHT);
            eastCompassToSideMap.Add (CC_Compass.WEST, CC_BlockSide.FRONT);
            cameraDirections.Add (CC_CameraDirectionName.EAST_FACING, new CC_CameraDirection (
                CC_CameraDirectionName.EAST_FACING,
                new Quaternion (0, 0.7f, 0, 0.7f),
                new zIntVector2 (-1, -1),
                eastCompassToSideMap
            ));

            // SOUTH_FACING
            Dictionary<CC_Compass, CC_BlockSide> southCompassToSideMap = new Dictionary<CC_Compass, CC_BlockSide> ();
            southCompassToSideMap.Add (CC_Compass.NORTH, CC_BlockSide.FRONT);
            southCompassToSideMap.Add (CC_Compass.EAST, CC_BlockSide.LEFT);
            southCompassToSideMap.Add (CC_Compass.SOUTH, CC_BlockSide.BACK);
            southCompassToSideMap.Add (CC_Compass.WEST, CC_BlockSide.RIGHT);
            cameraDirections.Add (CC_CameraDirectionName.SOUTH_FACING, new CC_CameraDirection (
                CC_CameraDirectionName.SOUTH_FACING,
                new Quaternion (0, 1, 0, 0),
                new zIntVector2 (-1, 1),
                southCompassToSideMap
            ));

            // WEST_FACING
            Dictionary<CC_Compass, CC_BlockSide> westCompassToSideMap = new Dictionary<CC_Compass, CC_BlockSide> ();
            westCompassToSideMap.Add (CC_Compass.NORTH, CC_BlockSide.RIGHT);
            westCompassToSideMap.Add (CC_Compass.EAST, CC_BlockSide.FRONT);
            westCompassToSideMap.Add (CC_Compass.SOUTH, CC_BlockSide.LEFT);
            westCompassToSideMap.Add (CC_Compass.WEST, CC_BlockSide.BACK);
            cameraDirections.Add (CC_CameraDirectionName.WEST_FACING, new CC_CameraDirection (
                CC_CameraDirectionName.WEST_FACING,
                new Quaternion (0, 0.7f, 0, -0.7f),
                new zIntVector2 (1, 1),
                westCompassToSideMap
            ));

            return cameraDirections;
        }

        private static List<CC_CameraDirection> getCameraDirectionsInOrder () {
            List<CC_CameraDirection> CameraDirectionsInOrder = new List<CC_CameraDirection> ();
            CameraDirectionsInOrder.Add (cameraDirections[CC_CameraDirectionName.NORTH_FACING]);
            CameraDirectionsInOrder.Add (cameraDirections[CC_CameraDirectionName.EAST_FACING]);
            CameraDirectionsInOrder.Add (cameraDirections[CC_CameraDirectionName.SOUTH_FACING]);
            CameraDirectionsInOrder.Add (cameraDirections[CC_CameraDirectionName.WEST_FACING]);
            return CameraDirectionsInOrder;
        }
    }
}